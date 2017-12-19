using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    /// <summary>
    /// 积雪判识
    /// </summary>
    public class SubProductBinarySnw : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinarySnw(SubProductDef productDef)
            : base(productDef)
        {
            _identify = productDef.Identify;
            _isBinary = true;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
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
            if (string.IsNullOrEmpty(algname))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (algname == "SNWExtract")
            {
                IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
                int visiBandNo = TryGetBandNo(bandNameRaster, "Visible");
                int sIBandNo = TryGetBandNo(bandNameRaster, "ShortInfrared");
                int fIBandNo = TryGetBandNo(bandNameRaster, "FarInfrared");
                double visiBandZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                double siBandZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
                double fiBandZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                if (visiBandNo <= 0 || sIBandNo <= 0 || fIBandNo <= 0 || visiBandZoom <= 0 || siBandZoom <= 0 || fiBandZoom <= 0)
                {
                    PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                    return null;
                }
                string express = string.Format(@"(band{1}/{4}f > var_ShortInfraredMin) && (band{1}/{4}f< var_ShortInfraredMax) && 
                                (band{2}/{5}f< var_FarInfraredMax) && (band{2}/{5}f > var_FarInfraredMin) && (band{0}/{3}f> var_VisibleMin) && 
                                ((float)(band{0}/{3}f-band{1}/{4}f)/(band{0}/{3}f+band{1}/{4}f)> var_NDSIMin) && 
                                ((float)(band{0}/{3}f-band{1}/{4}f)/(band{0}/{3}f+band{1}/{4}f)< var_NDSIMax)&&
                                (band{0}/{3}f< var_VisibleMax)", visiBandNo, sIBandNo, fIBandNo, visiBandZoom, siBandZoom, fiBandZoom);
                int[] bandNos = new int[] { visiBandNo, sIBandNo, fIBandNo };
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(_argumentProvider, bandNos, express);
                int width = _argumentProvider.DataProvider.Width;
                int height = _argumentProvider.DataProvider.Height;
                IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("SNW", width, height, _argumentProvider.DataProvider.CoordEnvelope, _argumentProvider.DataProvider.SpatialRef);
                try
                {
                    SnwFeatureCollection featureInfo = SnwDisplayInfo.GetDisplayInfo(_argumentProvider, visiBandNo, sIBandNo, fIBandNo);
                    result.Tag = featureInfo;
                }
                catch
                {
                    result.Tag = new SnwFeatureCollection("积雪辅助信息计算失败", null);
                }
                extracter.Extract(result);
                return result;
            }
            else
            {
                PrintInfo("指定的算法\"" + algname + "\"没有实现。");
                return null;
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        #region 合并云

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            if (piexd == null)
                return null;
            object obj = _argumentProvider.GetArg("isAppCloud");
            if (obj == null || !(bool)obj)
                return null;
            string cloudFile = GetClmFile(_argumentProvider.DataProvider.fileName);
            if (string.IsNullOrEmpty(cloudFile) || !File.Exists(cloudFile))
                return null;
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");

            //生成判识结果文件
            IInterestedRaster<Int16> iir = null;
            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "SNW";
            id.SubProductIdentify = _identify;
            id.GenerateDateTime = DateTime.Now;
            iir = new InterestedRaster<Int16>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
            int[] idxs = piexd.Indexes.ToArray();
            iir.Put(idxs, 1);

            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider snwPrd = null;
            IRasterDataProvider cloudPrd = GeoDataDriver.Open(cloudFile) as RasterDataProvider;
            try
            {
                snwPrd = iir.HostDataProvider;
                RasterMaper lakRm = new RasterMaper(snwPrd, new int[] { 1 });
                rms.Add(lakRm);

                RasterMaper cloudRm = new RasterMaper(cloudPrd, new int[] { GetCloudCHNO() });
                rms.Add(cloudRm);

                string outFileName = GetFileName(new string[] { _argumentProvider.DataProvider.fileName }, _subProductDef.ProductDef.Identify, _subProductDef.Identify, ".dat", null);
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.SetFeatureAOI(_argumentProvider.AOIs);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][index] == 1 && rvInVistor[0].RasterBandsData[0][index] == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = defCloudy;
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = rvInVistor[0].RasterBandsData[0][index];
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult("SNW", outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                iir.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
                cloudPrd.Dispose();
            }
        }

        public IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private string GetClmFile(string fname)
        {
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(fname));
            rid.ProductIdentify = "SNW";
            rid.SubProductIdentify = "0CLM";
            string clmFile = rid.ToWksFullFileName(".dat");
            if (File.Exists(clmFile))
                return clmFile;
            else
            {
                rid.OrbitDateTime = _argumentProvider.DataProvider.DataIdentify.OrbitDateTime;
                clmFile = rid.ToWksFullFileName(".dat");
                if (File.Exists(clmFile))
                    return clmFile;
            }
            return null;
        }

        private int GetCloudCHNO()
        {
            int cloudCH;
            if (_argumentProvider.GetArg("cloudCH") == null)
                return 1;
            if (string.IsNullOrEmpty(_argumentProvider.GetArg("cloudCH").ToString()))
                return 1;
            if (int.TryParse(_argumentProvider.GetArg("cloudCH").ToString(), out cloudCH))
                return cloudCH;
            else
                return 1;

        }

        #endregion

    }
}
