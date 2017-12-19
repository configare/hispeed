using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MEM;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductBinaryBag : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        public double MinNDVI = 0;
        public double MaxNDVI = 0;
        public double MinNear = 0;
        public SubProductBinaryBag(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _isBinary = true;
            _name = subProductDef.Name;
            _algorithmDefs = new List<AlgorithmDef>(subProductDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {

            _contextMessage = contextMessage;
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            string algname = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algname == "BAGExtract_New")
            {
                //return GetBAG();
                return GetCloudBAG();

            }

            if (!string.IsNullOrEmpty(algname))
            {
                double visiBandRoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                double niBandRoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                if (algname == "BAGExtract")
                {
                    MinNDVI = (double)_argumentProvider.GetArg("NDVIMin");
                    MaxNDVI = (double)_argumentProvider.GetArg("NDVIMax");
                }
                else if (algname == "BAGExtract_Test")
                {
                    UCSetNDVITool uccontrl = _argumentProvider.GetArg("ucSetNDVITool") as UCSetNDVITool;
                    if (uccontrl.ckbaoi.Checked)
                    {
                        uccontrl.btnGetAOIIndex(null, null);
                    }
                    else
                    {
                        MinNDVI = uccontrl.txtndvimin.Value;
                        MaxNDVI = uccontrl.txtndvimax.Value;
                    }
                }



                IRasterDataProvider prd = _argumentProvider.DataProvider;
                if (prd == null)
                {
                    PrintInfo("未能获取当前影像数据。");
                    return null;
                }
                IBandNameRaster bandNameRaster = prd as IBandNameRaster;
                int visiBandNo = TryGetBandNo(bandNameRaster, "Visible");
                int niBandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                if (visiBandNo == -1 || niBandNo == -1 || visiBandRoom == -1 || niBandRoom == -1)
                {
                    PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                    return null;
                }
                string express = string.Format(@"({0}==0||{1}==0 )? false :((float)(band{3}/{1}f - band{2}/{0}f) / (band{2}/{0}f+ band{3}/{1}f) < {5})
                       && ((float)(band{3} /{1}f- band{2}/{0}f) / (band{3}/{1}f + band{2}/{0}f) > {4})", visiBandRoom, niBandRoom, visiBandNo, niBandNo, MinNDVI, MaxNDVI);
                int[] bandNos = new int[] { visiBandNo, niBandNo };
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(_argumentProvider, bandNos, express);
                int width = prd.Width;
                int height = prd.Height;
                IPixelIndexMapper memResult = PixelIndexMapperFactory.CreatePixelIndexMapper("BAG", width, height, prd.CoordEnvelope, prd.SpatialRef);
                extracter.Extract(memResult);
                memResult.Tag = new BagFeatureCollection("蓝藻辅助信息", GetDisplayInfo(memResult, visiBandNo, niBandNo));
                //计算NDVI文件
                IPixelFeatureMapper<float> ndvi = null;
                try
                {
                    ndvi = ComputeNDVIResult(_argumentProvider.DataProvider, memResult, visiBandNo, niBandNo);
                    IExtractResultBase bPCDResult = CreatPixelCoverRate(ndvi);
                    ExtractResultArray results = new ExtractResultArray("BAG_BINS");
                    results.Add(memResult);
                    //results.Add(ndvi);
                    results.Add(bPCDResult);
                    return results;
                }
                finally
                {
                    if (ndvi != null)
                        ndvi.Dispose();
                }
            }
            else
            {
                PrintInfo("指定的算法\"" + algname + "\"没有实现。");
                return null;
            }
        }
        public IExtractResult GetBAG()
        {

            double visiBandRoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double niBandRoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            bool needcloud = (bool)_argumentProvider.GetArg("isAppCloud");
            UCSetNearTool uccontrl = _argumentProvider.GetArg("UCSetNearTool") as UCSetNearTool;
            if (uccontrl.ckbone.Checked)
            {
                uccontrl.btnGetAOIIndex(null, null);

            }
            MinNear = double.Parse(uccontrl.txtnearmin.Text) * 100;//放大调节 跟界面参数设置有关
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            if (prd == null)
            {
                PrintInfo("未能获取当前影像数据。");
                return null;
            }
            IBandNameRaster bandNameRaster = prd as IBandNameRaster;
            int visiBandNo = TryGetBandNo(bandNameRaster, "Visible");
            int niBandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
            int shortBandNo = TryGetBandNo(bandNameRaster, "ShortInfrared");

            if (visiBandNo == -1 || niBandNo == -1 || visiBandRoom == -1 || niBandRoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            string express = string.Format("band{0}/{1}f" + " >= " + MinNear, niBandNo, niBandRoom);
            string cloudexpress = GetCloudExpress();
            if (needcloud)
            {
                express = string.Format("{0} && ({1})", express, cloudexpress);
            }
            int[] bandNos = new int[] { visiBandNo, niBandNo, shortBandNo };
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            int width = prd.Width;
            int height = prd.Height;
            IPixelIndexMapper memResult = PixelIndexMapperFactory.CreatePixelIndexMapper("BAG", width, height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(memResult);

            memResult.Tag = new BagFeatureCollection("蓝藻辅助信息", GetDisplayInfo(memResult, visiBandNo, niBandNo));
            //计算NDVI文件
            IPixelFeatureMapper<float> ndvi = null;
            try
            {
                ndvi = ComputeNDVIResult(_argumentProvider.DataProvider, memResult, visiBandNo, niBandNo);
                IExtractResultBase bPCDResult = CreatPixelCoverRate(ndvi);
                ExtractResultArray results = new ExtractResultArray("BAG_BINS");
                results.Add(memResult);
                //results.Add(ndvi);
                results.Add(bPCDResult);
                return results;
            }
            finally
            {
                if (ndvi != null)
                    ndvi.Dispose();
            }
        }
        public IExtractResult GetCloudBAG()
        {

            double niBandRoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            bool needcloud = (bool)_argumentProvider.GetArg("isAppCloud");

            UCSetNearTool uccontrl = _argumentProvider.GetArg("UCSetNearTool") as UCSetNearTool;
            if (uccontrl.ckbone.Checked)
            {
                uccontrl.btnGetAOIIndex(null, null);

            }
            string cloudfile = GetClmFile(_argumentProvider.DataProvider);
            bool isexistcloud = File.Exists(cloudfile);

            MinNear = double.Parse(uccontrl.txtnearmin.Text) * 100;//放大调节 跟界面参数设置有关
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            if (prd == null)
            {
                PrintInfo("未能获取当前影像数据。");
                return null;
            }
            IBandNameRaster bandNameRaster = prd as IBandNameRaster;
            int niBandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
            if (niBandNo == -1 || niBandRoom == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            List<RasterMaper> rms = new List<RasterMaper>();

            rms.Add(new RasterMaper(_argumentProvider.DataProvider, new int[] { niBandNo }));
            if (isexistcloud)
            {
                rms.Add(new RasterMaper(GeoDataDriver.Open(cloudfile) as IRasterDataProvider, new int[] { 1 }));
            }
            RasterIdentify rid = new RasterIdentify(_argumentProvider.DataProvider);
            rid.ProductIdentify = "BAG";
            rid.SubProductIdentify = "DBLV";
            string outfile = rid.ToPrjWksFullFileName(".dat");
            IRasterDataProvider outRaster = null;
            outRaster = CreateOutRaster(outfile, rms.ToArray(), enumDataType.Int16);
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("BAG", _argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height,
                _argumentProvider.DataProvider.CoordEnvelope, _argumentProvider.DataProvider.SpatialRef);
            try
            {
                RasterProcessModel<Int16, Int16> rfr = null;
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                rfr = new RasterProcessModel<Int16, Int16>();
                int totalindex = 0;
                rfr.SetRaster(fileIns, fileOuts);
                rfr.SetArgumentProviderAOI(_argumentProvider.AOI);
                rfr.RegisterCalcModel(new RasterCalcHandlerFun<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        for (int i = 0; i < aoi.Length; i++)
                        {
                            int index = aoi[i];
                            if (rvInVistor[0].RasterBandsData[0][index] / niBandRoom > MinNear)
                            {
                                if (needcloud)
                                {
                                    if (!isexistcloud)
                                    {
                                        result.Put(totalindex + index);
                                        continue;
                                    }
                                    else if (rvInVistor[1].RasterBandsData[0][index] == 0)//非云
                                    {
                                        result.Put(totalindex + index);

                                    }
                                    else
                                    {

                                        //rvOutVistor[0].RasterBandsData[0][aoi[i]] = -9999;// 云 这里是否需要配置一下？
                                    }
                                }
                                else
                                {
                                    result.Put(totalindex + index);
                                    //rvOutVistor[0].RasterBandsData[0][aoi[i]] = 1;

                                }
                            }
                            else
                            {


                            }
                        }
                        totalindex = totalindex += dataLength;
                    }

                    return false;
                }));
                rfr.Excute();
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                outRaster.Dispose();

            }
        }
        public string GetCloudExpress()
        {
            //可见光波段
            int VisibleCH = Obj2int(_argumentProvider.GetArg("Visible"));
            //近红外波段
            int NearInfraredCH = Obj2int(_argumentProvider.GetArg("NearInfrared"));
            //短波红外波段
            int ShortInfraredCH = Obj2int(_argumentProvider.GetArg("ShortInfrared"));
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            //尝试根据当前卫星载荷数据自动匹配波段信息
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(VisibleCH, out newbandNo))
                    VisibleCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(NearInfraredCH, out newbandNo))
                    NearInfraredCH = newbandNo;
                if (bandNameRaster.TryGetBandNoFromBandName(ShortInfraredCH, out newbandNo))
                    ShortInfraredCH = newbandNo;
            }
            //获取放大倍数信息
            double VisibleZoom = Obj2Double(_argumentProvider.GetArg("Visible_Zoom"));
            double NearInfraredZoom = Obj2Double(_argumentProvider.GetArg("NearInfrared_Zoom"));
            double ShortInfraredZoom = Obj2Double(_argumentProvider.GetArg("ShortInfrared_Zoom"));
            //获取配置阈值
            double minvisiable = double.Parse(_argumentProvider.GetArg("MinVisiable").ToString());
            double minnearinfrared = double.Parse(_argumentProvider.GetArg("MinNearInFrared").ToString());
            double maxnsvi = double.Parse(_argumentProvider.GetArg("MaxNSVI").ToString());
            double minndvi = double.Parse(_argumentProvider.GetArg("MinNDVI").ToString());

            int bandCount = _argumentProvider.DataProvider == null ? -1 : _argumentProvider.DataProvider.BandCount;
            List<int> bandNos = new List<int>();
            StringBuilder express = new StringBuilder();
            //波段信息
            bandNos.Add(VisibleCH);
            bandNos.Add(NearInfraredCH);
            bandNos.Add(ShortInfraredCH);
            express.Append(string.Format("band{0}/{1}f" + " >= " + minvisiable + "||", VisibleCH, VisibleZoom));
            express.Append(string.Format("(band{0}/{1}f)/(band{2}/{3}f)" + "  <= " + maxnsvi + " && ", NearInfraredCH, NearInfraredZoom, ShortInfraredCH, ShortInfraredZoom));
            express.Append(string.Format("band{0}/{1}f" + " >= " + minnearinfrared + " && ", NearInfraredCH, NearInfraredZoom));
            express.Append(string.Format("(float)((band{2}/{1}f - band{0}/{3}f) / (band{2}/{1}f+band{0}/{3}f)) >= " + minndvi, VisibleCH, VisibleZoom, NearInfraredCH, NearInfraredZoom));
            return string.Format("{0}?false:true", express.ToString());
        }


        //public override void SetExtHeader(IExtHeaderSetter setter, object header)
        //{
        //    if (setter == null || header == null)
        //        return;
        //    setter.SetExtHeader<NDVISetValue>((NDVISetValue)header);
        //}

        private IPixelFeatureMapper<float> ComputeNDVIResult(IRasterDataProvider orbitDataProvider, IPixelIndexMapper result, int visiBandNo, int niBandNo)
        {
            //生成判识结果文件
            IRasterDataProvider batDataProvider = null;
            IInterestedRaster<UInt16> iir = null;
            try
            {
                RasterIdentify id = new RasterIdentify();
                id.ThemeIdentify = "CMA";
                id.ProductIdentify = "BAG";
                id.SubProductIdentify = "DBLV";
                id.Sensor = orbitDataProvider.DataIdentify.Sensor;
                id.Satellite = orbitDataProvider.DataIdentify.Satellite;
                id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
                id.GenerateDateTime = DateTime.Now;
                iir = new InterestedRaster<UInt16>(id, new Size(orbitDataProvider.Width, orbitDataProvider.Height), orbitDataProvider.CoordEnvelope.Clone());
                int[] idxs = result.Indexes.ToArray();
                iir.Put(idxs, 1);
                batDataProvider = iir.HostDataProvider;
                //内存结果
                IPixelFeatureMapper<float> memresult = new MemPixelFeatureMapper<float>("NDVI", 1000, new Size(batDataProvider.Width, batDataProvider.Height), batDataProvider.CoordEnvelope, batDataProvider.SpatialRef);
                //虚拟文件
                //转换IRasterDataProvider!!!!!
                IVirtualRasterDataProvider vrd = new VirtualRasterDataProvider(new IRasterDataProvider[] { batDataProvider, orbitDataProvider });
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
                visitor.VisitPixel(new int[] { 1, visiBandNo + 1, niBandNo + 1 },
                    (index, values) =>
                    {
                        if (values[0] == 0)
                            memresult.Put(index, -9999);
                        else
                            memresult.Put(index, GetOnePixelNDVI(values[1], values[2]));
                    });
                iir.Dispose();
                return memresult;
            }
            finally
            {
                if (batDataProvider != null)
                    batDataProvider.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
            }
        }

        private float GetOnePixelNDVI(float visiValue, float niValue)
        {
            return (visiValue + niValue) == 0 ? 0f : (niValue - visiValue) / (visiValue + niValue);
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {


            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int visiBandNo = TryGetBandNo(bandNameRaster, "Visible");
            int niBandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
            IPixelFeatureMapper<float> bpcd;
            using (IPixelFeatureMapper<float> ndvi = ComputeNDVIResult(_argumentProvider.DataProvider, piexd, visiBandNo, niBandNo))
            {
                bpcd = CreatPixelCoverRate(ndvi);
                bpcd.SetDispaly(false);
            }
            return bpcd;
        }

        private IPixelFeatureMapper<float> CreatPixelCoverRate(IPixelFeatureMapper<float> ndviResult)
        {
            if (ndviResult == null)
                return null;
            //生成NDVI结果文件
            IRasterDataProvider ndviDataProvider = null;
            IInterestedRaster<float> iir = null;
            try
            {
                RasterIdentify id = new RasterIdentify();
                id.ThemeIdentify = "CMA";
                id.ProductIdentify = "BAG";
                id.SubProductIdentify = "BPCD";
                id.Sensor = _argumentProvider.DataProvider.DataIdentify.Sensor;
                id.Satellite = _argumentProvider.DataProvider.DataIdentify.Satellite;
                id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
                id.GenerateDateTime = DateTime.Now;
                iir = new InterestedRaster<float>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
                iir.Put(ndviResult);
                ndviDataProvider = iir.HostDataProvider;
                double dst;


                dst = MaxNDVI - MinNDVI;
                //判断是否使用端元值计算
                NDVISettingItem[] settings = _argumentProvider.GetArg("NDVISetting") as NDVISettingItem[];
                if (settings != null)
                    ResetArgNDVIMaxMin(settings, ref MinNDVI, ref dst);
                IPixelFeatureMapper<float> memresult = new MemPixelFeatureMapper<float>("BPCD", 1000, new Size(ndviDataProvider.Width, ndviDataProvider.Height), ndviDataProvider.CoordEnvelope, ndviDataProvider.SpatialRef);
                ArgumentProvider ap = new ArgumentProvider(ndviDataProvider, null);
                RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
                visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                {
                    if (values[0] == -9999f)
                        memresult.Put(index, -9999);
                    else if (dst == 0)
                        memresult.Put(index, -9999);
                    else
                        memresult.Put(index, (float)((values[0] - MinNDVI) / dst));
                });
                iir.Dispose();
                return memresult;
            }
            finally
            {
                if (ndviDataProvider != null)
                    ndviDataProvider.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
            }
        }

        private void ResetArgNDVIMaxMin(NDVISettingItem[] settings, ref double ndviMin, ref double dst)
        {
            if (settings == null || settings.Length < 1)
                return;
            CoordEnvelope envelope = _argumentProvider.DataProvider.CoordEnvelope;
            if (envelope == null)
                return;
            foreach (NDVISettingItem item in settings)
            {
                if (item.Envelope == null)
                    continue;
                if (item.IsUse)
                {
                    if (envelope.MaxX >= item.Envelope.MaxX && envelope.MinX <= item.Envelope.MinX
                        && envelope.MaxY >= item.Envelope.MaxY && envelope.MinY <= item.Envelope.MinY)
                    {
                        ndviMin = item.MinValue;
                        dst = item.MaxValue - item.MinValue;
                        return;
                    }
                }
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private Dictionary<int, BagFeature> GetDisplayInfo(IPixelIndexMapper result, int visiBandNo, int niBandNo)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            try
            {
                Dictionary<int, BagFeature> features = new Dictionary<int, BagFeature>();
                BagFeature tempFeature = null;
                ArgumentProvider ap = new ArgumentProvider(_argumentProvider.DataProvider, null);
                RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
                visitor.VisitPixel(new int[] { visiBandNo, niBandNo },
                    (index, values) =>
                    {
                        tempFeature = new BagFeature();
                        tempFeature.Ndvi = GetOnePixelNDVI(values[0], values[1]);
                        features.Add(index, tempFeature);
                    });
                return features;
            }
            finally
            {
            }
        }

        private double Obj2Double(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            double value = -1;
            if (double.TryParse(obj.ToString(), out value))
                return value;
            return -1;
        }

        private int Obj2int(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            int value = -1;
            if (int.TryParse(obj.ToString(), out value))
                return value;
            return -1;
        }
        private string GetClmFile(IRasterDataProvider currPrd)
        {
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(currPrd.fileName));
            rid.ProductIdentify = "BAG";
            rid.SubProductIdentify = "0CLM";
            string clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            rid = new RasterIdentify(currPrd);
            rid.ProductIdentify = "BAG";
            rid.SubProductIdentify = "0CLM";
            clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            return null;
        }
    }
}
