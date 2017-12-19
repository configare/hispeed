#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/4 18:26:14
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using CodeCell.AgileMap.Core;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    /// <summary>
    /// 类名：SubProductRasterFldFloodCount
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/4 18:26:14
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductRasterFldFloodCount:CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        //泛滥水体天数
        public SubProductRasterFldFloodCount(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FLDC")
            {
                if (_argumentProvider.GetArg("SelectedPrimaryFiles") == null)
                    return null;
                string[] binWater = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
                if (_argumentProvider.GetArg("BackWaterFile") == null)
                    return null;
                string backWaterPath = _argumentProvider.GetArg("BackWaterFile").ToString();
                if (!File.Exists(backWaterPath))
                    return null;
                for (int i = 0; i < binWater.Length; i++)
                {
                    if (!File.Exists(binWater[i]))
                        return null;
                }
                if (string.IsNullOrEmpty(backWaterPath) || string.Equals(binWater, backWaterPath))
                    return null;
                //生成判识结果数据统计数据集
                FileExtractResult dblvCount = GreateDBLVCount(binWater, progressTracker);
                //背景水体可能为DAT/MVG/SHP格式文件
                switch (Path.GetExtension(backWaterPath).ToUpper())
                {
                    case ".DAT":
                        return CompareDATFile(backWaterPath, dblvCount, progressTracker);
                    case ".SHP":
                        return CompareSHPFile(backWaterPath, dblvCount, progressTracker);
                    case ".MVG":
                        return null;
                }
            }
            return null;
        }

        private FileExtractResult GreateDBLVCount(string[] dblvFiles, Action<int, string> progressTracker)
        {
            if (dblvFiles == null || dblvFiles.Length < 1)
                return null;
            //文件列表排序 两天内全部为云，认为水
            string[] sortedFiles = SortFileName(dblvFiles);
            //先进行处理，将云水合为一个文件
            string[] dstFiles = CombineCloudAndWaterDBLV(sortedFiles);
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                List<DateTime> timeList = new List<DateTime>();
                RasterIdentify id = null;
                for (int i = 0; i < dstFiles.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(dstFiles[i]) as IRasterDataProvider;
                    id = new RasterIdentify(dstFiles[i]);
                    timeList.Add(id.OrbitDateTime);
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                }
                //时间间隔数组
                int[] interval = GetDataInterval(timeList.ToArray());
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(dstFiles, "WTDC");
                string outFileName = ri.ToWksFullFileName(".dat");
                //创建结果数据
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        List<short> dataValue=new List<short>();
                        short dstValue;
                        for (int i = 0; i < dataLength; i++)
                        {
                            dataValue.Clear();
                            foreach (RasterVirtualVistor<short> rv in rvInVistor)
                            {
                                if (rv.RasterBandsData == null)
                                    continue;
                                dataValue.Add(rv.RasterBandsData[0][i]);
                            }
                            dstValue = StatWaterDayCount(dataValue.ToArray(), interval);
                            rvOutVistor[0].RasterBandsData[0][i] = dstValue;
                        }
                        
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult("WTDC", outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }

            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private int[] GetDataInterval(DateTime[] dateTime)
        {
            if (dateTime == null || dateTime.Length < 1)
                return null;
            int length=dateTime.Length;
            int[] interval = new int[length-1];
            TimeSpan span;
            for (int i = 1; i < length; i++)
            {
                span = dateTime[i].Date - dateTime[i - 1].Date;
                interval[i - 1] = span.Days;
            }
            return interval;
        }

        private short StatWaterDayCount(short[] waterArray, int[] interval)
        {
            short dayCount = 0;
            if (waterArray == null || waterArray.Length == 0)
                return dayCount;
            short cloudCount = 0;
            for (int i = 0; i < waterArray.Length; i++)
            {
                //当天为水
                if (waterArray[i] == 1)
                {
                    //没有连续累计云天数
                    if (cloudCount == 0)
                        if (i == 0)
                            dayCount++;
                        else
                            dayCount += (short)interval[i - 1];
                    //有连续累计云天数
                    else
                    {
                        dayCount += (short)(cloudCount + interval[i - 1]);
                        cloudCount = 0;
                    }
                }
                else if (waterArray[i] == -9999)
                {
                    if (i == 0)
                        cloudCount++;
                    else
                        cloudCount+=(short)interval[i-1];
                }
                //当天为陆地
                else if (waterArray[i] == 0)
                {
                    cloudCount = 0;
                }
            }
            return dayCount;
        }

        private string[] CombineCloudAndWaterDBLV(string[] sortedFiles)
        {
            List<string> cloudFiles = new List<string>();
            foreach (string item in sortedFiles)
            {
                if (item.ToUpper().Contains("_0CLM"))
                    cloudFiles.Add(item);
            }
            if (cloudFiles.Count < 1)
                return sortedFiles;
            List<string> dstFiles = new List<string>();
            //查找水判识结果对应的云结果是否存在
            //存在云结果则进行合成，替换
            int index = 0;
            string clmFile=null;
            bool isExist = false;
            foreach (string item in sortedFiles)
            {
                if (cloudFiles.Contains(item))
                    continue;
                isExist = false;
                clmFile = Path.GetFileName(item).ToUpper().Replace("_DBLV_", "_0CLM_");
                if(index<cloudFiles.Count)
                {
                    foreach (string cldFile in cloudFiles)
                    {
                        if (clmFile == Path.GetFileName(cldFile).ToUpper())
                        {
                            //合并
                            dstFiles.Add(CombineCloudAndWater(cldFile,item));
                            index++;
                            isExist = true;
                            break;
                        }
                    }
                }
                if (!isExist || index > cloudFiles.Count)
                {
                    dstFiles.Add(item);
                }
            }
            return dstFiles.ToArray();
        }

        private string CombineCloudAndWater(string cloudFile, string waterFile)
        {
            IInterestedRaster<Int16> iir = null;
            IRasterDataProvider waterPrd = null;
            IRasterDataProvider cloudPrd = null;
            try
            {
                RasterIdentify id = new RasterIdentify(waterFile);
                waterPrd = GeoDataDriver.Open(waterFile) as IRasterDataProvider;
                cloudPrd = GeoDataDriver.Open(cloudFile) as IRasterDataProvider;
                iir = new InterestedRaster<Int16>(id, new Size(waterPrd.Width, waterPrd.Height), waterPrd.CoordEnvelope.Clone());
                //虚拟文件
                //转换IRasterDataProvider!!!!!
                IVirtualRasterDataProvider vrd = new VirtualRasterDataProvider(new IRasterDataProvider[] { waterPrd, cloudPrd });
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
                visitor.VisitPixel(new int[] { 1, 2 },
                    (index, values) =>
                    {
                        if (values[0] == 1)
                            iir.Put(index, 1);
                        else if (values[1] == 1)
                        {
                            iir.Put(index, -9999);
                        }
                    });
                return iir.FileName;
            }
            finally
            {
                if (iir != null)
                    iir.Dispose();
                if (waterPrd != null)
                    waterPrd.Dispose();
                if (cloudPrd != null)
                    cloudPrd.Dispose();
            }
        }

        private IExtractResult CompareSHPFile(string backWaterPath, FileExtractResult dayCountFile, Action<int, string> progressTracker)
        {
            string shpPrimaryField = null;
            Feature[] features = GetFeatures(backWaterPath, out shpPrimaryField);
            if (features == null || features.Length == 0)
                return null;
            //矢量栅格化
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            IRasterDataProvider dataPrd = GeoDataDriver.Open(backWaterPath) as IRasterDataProvider;
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("FLOD", 1000, new Size(dataPrd.Width, dataPrd.Height), dataPrd.CoordEnvelope, dataPrd.SpatialRef);
            using (Bitmap bitmap = VectorsToBitmap(dataPrd, features, shpPrimaryField, out nameColors))
            {
                int[] aoi;
                Color color;
                string name;
                List<int> items = new List<int>();
                foreach (Feature fea in features)
                {
                    name = fea.GetFieldValue(shpPrimaryField);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    color = nameColors[name];
                    aoi = GetAOIByFeature(bitmap, color);
                    if (aoi != null)
                        items.AddRange(aoi);
                }
                //分情况处理
                //无aoi区域
                if (items.Count == 0)
                {
                    ArgumentProvider ap = new ArgumentProvider(dataPrd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] { 1 },
                    (idx, values) =>
                    {
                         result.Put(idx, values[0]);
                    });
                }
                //有aoi区域
                else
                {
                    ArgumentProvider ap = new ArgumentProvider(dataPrd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] { 1 },
                    (idx, values) =>
                    {
                        result.Put(idx, values[0]);
                    });
                    Size size = new Size(dataPrd.Width, dataPrd.Height);
                    Rectangle aoiRect = AOIHelper.ComputeAOIRect(items.ToArray(), size);
                    rpVisitor.VisitPixel(aoiRect, items.ToArray(), new int[] { 1 },
                    (idx, values) =>
                    {
                        if (values[0] == 0)
                        {
                            result.Put(idx, -2);
                        }
                    });
                }
            }
            return result;
        }

        private Feature[] GetFeatures(string fname, out string primaryField)
        {
            primaryField = string.Empty;
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            primaryField = dr.Fields[0];
            try
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets == null || fets.Length == 0)
                    return null;
                List<Feature> features = new List<Feature>();
                foreach (Feature fet in fets)
                {
                    if (fet.Geometry is ShapePolygon)
                        features.Add(fet);
                }
                return features != null ? features.ToArray() : null;
            }
            finally
            {
                dr.Dispose();
            }
        }

        private Bitmap VectorsToBitmap(IRasterDataProvider prd, CodeCell.AgileMap.Core.Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Dictionary<ShapePolygon, Color> vectors = GetVectorColors(features, shpPrimaryField, out nameColors);
                Bitmap bmp = new Bitmap(prd.Width, prd.Height, PixelFormat.Format24bppRgb);
                Envelope envelop = GetEnvelop(prd);
                c.ToBitmap(vectors, Color.Black, envelop, new Size(prd.Width, prd.Height), ref bmp);
                return bmp;
            }
        }

        private Dictionary<ShapePolygon, Color> GetVectorColors(Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            Dictionary<ShapePolygon, Color> vectorColors = new Dictionary<ShapePolygon, Color>();
            nameColors = new Dictionary<string, Color>();
            int count = features.Count();
            Random random = new Random(1);
            Color color;
            for (int i = 0; i < count; i++)
            {
                color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                if (features[i].GetFieldValue(shpPrimaryField) == null)
                    continue;
                if (!nameColors.Keys.Contains(features[i].GetFieldValue(shpPrimaryField))
                  && !String.IsNullOrEmpty(features[i].GetFieldValue(shpPrimaryField)))
                    nameColors.Add(features[i].GetFieldValue(shpPrimaryField), color);
                if (!vectorColors.Keys.Contains(features[i].Geometry as ShapePolygon))
                    vectorColors.Add(features[i].Geometry as ShapePolygon, color);
            }
            return vectorColors;
        }

        private Envelope GetEnvelop(IRasterDataProvider prd)
        {
            Size size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
            }
        }

        private IExtractResult CompareDATFile(string backWaterPath, FileExtractResult dayCountFile, Action<int, string> progressTracker)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                IRasterDataProvider backWaterRaster = RasterDataDriver.Open(backWaterPath) as IRasterDataProvider;
                RasterMaper backRm = new RasterMaper(backWaterRaster, new int[] { 1 });
                rms.Add(backRm);
                IRasterDataProvider waterRaster = RasterDataDriver.Open(dayCountFile.FileName) as IRasterDataProvider;
                RasterMaper waterRm = new RasterMaper(waterRaster, new int[] { 1 });
                rms.Add(waterRm);
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(new string[]{dayCountFile.FileName},null);
                string outFileName = ri.ToWksFullFileName(".dat");
                //创建结果数据
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        short dayCount;
                        for (int i = 0; i < dataLength; i++)
                        {
                            dayCount = rvInVistor[1].RasterBandsData[0][i];
                            if(rvInVistor[0].RasterBandsData[0][i]==0)
                            {
                                rvOutVistor[0].RasterBandsData[0][i] = dayCount;  //泛滥水体天数
                            }
                            else if (rvInVistor[0].RasterBandsData[0][i] == 1)
                            {
                                if (rvInVistor[1].RasterBandsData[0][i] >= 1)
                                    rvOutVistor[0].RasterBandsData[0][i] = -1;   //未变化水体
                                else if (rvInVistor[1].RasterBandsData[0][i] == 0)
                                    rvOutVistor[0].RasterBandsData[0][i] = -2;   //缩小水体
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }

            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames,string outFileId)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;
            if (!string.IsNullOrEmpty(outFileId))
                rst.SubProductIdentify = outFileId;
            else
            {
                object obj = _argumentProvider.GetArg("OutFileIdentify");
                if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                    rst.SubProductIdentify = obj.ToString();
            }
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
                if(resX < inRaster.Raster.ResolutionX)
                    resX = inRaster.Raster.ResolutionX;
                if(resY < inRaster.Raster.ResolutionY)
                    resY = inRaster.Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
