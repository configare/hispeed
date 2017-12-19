using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Diagnostics;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;
using System.IO;
using OSGeo.GDAL;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductBinaryFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Dictionary<int, PixelFeature> _curFeatures = null;

        public SubProductBinaryFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override void Reset()
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            _contextMessage = contextMessage;
            string algorithmName = Obj2String(_argumentProvider.GetArg("AlgorithmName"));
            if (string.IsNullOrEmpty(algorithmName))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            switch (algorithmName)
            {
                case "EasyAlgorithm":
                    return DoMakeUseEasyAlgorithm(progressTracker);
                case "NightAlgorithm":
                    return DoMakeUseNightAlgorithm(progressTracker);
                case "ExactAlgorithm":
                    return DoMakeUseAdvanceAlgorithm(progressTracker);
                case "ImportAlgorithm":
                    return DoMakeUseImportAlgorithm(progressTracker);
                default:
                    PrintInfo("指定的算法\"" + algorithmName + "\"没有实现。");
                    return null;
            }
        }

        private IExtractResult DoMakeUseNightAlgorithm(Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int band3 = TryGetBandNo(bandNameRaster, "MiddleInfrared");
            int band4 = TryGetBandNo(bandNameRaster, "FarInfrared");
            if (band3 == -1 || band4 == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            //
            double band3_zoom = Obj2Double(_argumentProvider.GetArg("MiddleInfrared_Zoom"));
            double band4_zoom = Obj2Double(_argumentProvider.GetArg("FarInfrared_Zoom"));
            if (double.IsNaN(band3_zoom))
                band3_zoom = 1;
            if (double.IsNaN(band4_zoom))
                band4_zoom = 1;
            //
            return DoNightMake(progressTracker, new int[] { band3, band4 }, new double[] { band3_zoom, band4_zoom });
        }

        private IExtractResult DoNightMake(Action<int, string> progressTracker, int[] bandNos, double[] bandZooms)
        {
            string express = "band{0} / " + bandZooms[0] + "f >= var_MiddleInfraredMin && band{0} / " + bandZooms[0] + "f <= var_MiddleInfraredMax && " +
                "band{1} / " + bandZooms[1] + "f >= var_FarInfraredMin && band{1} / " + bandZooms[1] + "f <= var_FarInfraredMax";
            express = string.Format(express, bandNos[0], bandNos[1]);
            //
            IPixelIndexMapper result = DoMake(express, bandNos, bandZooms);//疑似火点判识

            //不实时计算背景温度
            //int[] filteredAOI = result.Indexes.ToArray();
            //if (filteredAOI == null || filteredAOI.Length == 0)
            //    return result;
            //int[] firePoints = GetPreExtractIndex();

            //Size size = new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            //DoubtFirPixelFilter doubtFilter = CreateArgument.CreateDoubtFilter(_argumentProvider, _contextMessage);
            //if (doubtFilter == null)
            //    return result;

            //filteredAOI = MergeFirePoints(result, firePoints, filteredAOI);
            //Rectangle aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            ////背景温度计算
            //PrintInfo("[开始]背景亮温计算...");
            //BackTmpComputer backTmpComputer = CreateArgument.CreateBackTmpComputer(_argumentProvider, doubtFilter as IBackTmpComputerHelper, _contextMessage);
            //if (backTmpComputer == null)
            //    return null;
            //aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            //_curFeatures = backTmpComputer.ComputeNight(_argumentProvider, aoiRect, filteredAOI);
            //VertifyFirPixel(ref _curFeatures);
            //result.Tag = new FirFeatureCollection("火点像元特征", _curFeatures);
            //if (_curFeatures.Count != 0)
            //    GetFirePointIndeiex.WriteFireIndexiexFilename(_argumentProvider, _curFeatures.Keys.ToArray());

            return result;
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            try
            {
                piexd = CalcBackTmp(piexd);
                if (piexd == null || _curFeatures == null || _argumentProvider == null)
                    return null;

                Dictionary<int, PixelFeature> features = new Dictionary<int, PixelFeature>();
                foreach (int index in piexd.Indexes)
                {
                    if (_curFeatures.ContainsKey(index))
                        features.Add(index, _curFeatures[index]);
                }
                if (features.Count == 0)
                    return null;
                return GetOtherExtractResult.GetExtractResult(_argumentProvider, features, piexd, null, progressTracker);
            }
            finally 
            {
                _curFeatures = null;
            }
        }

        private IPixelIndexMapper CalcBackTmp(IPixelIndexMapper result)
        {
            int[] filteredAOI = result.Indexes.ToArray();
            if (filteredAOI == null || filteredAOI.Length == 0)
                return result;
            int[] firePoints = GetPreExtractIndex();

            Size size = new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            DoubtFirPixelFilter doubtFilter = CreateArgument.CreateDoubtFilter(_argumentProvider, _contextMessage);
            if (doubtFilter == null)
                return result;

            filteredAOI = MergeFirePoints(result, firePoints, filteredAOI);
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            //背景温度计算
            PrintInfo("[开始]背景亮温计算...");
            BackTmpComputer backTmpComputer = CreateArgument.CreateBackTmpComputer(_argumentProvider, doubtFilter as IBackTmpComputerHelper, _contextMessage);
            if (backTmpComputer == null)
                return null;
            aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            _curFeatures = backTmpComputer.ComputeNight(_argumentProvider, aoiRect, filteredAOI);
            VertifyFirPixel(ref _curFeatures);
            if (_curFeatures.Count != 0)
                GetFirePointIndeiex.WriteFireIndexiexFilename(_argumentProvider, _curFeatures.Keys.ToArray());
            return result;
        }

        private IExtractResult DoMakeUseImportAlgorithm(Action<int, string> progressTracker)
        {
            string gfrFile = Obj2String(_argumentProvider.GetArg("GFRFile"));
            if (string.IsNullOrEmpty(gfrFile) || !File.Exists(gfrFile))
            {
                PrintInfo("全球火点数据导入文件为空或不存在。");
                return null;
            }
            if (_argumentProvider.DataProvider == null)
                return null;
            return GenerateExtractResult(gfrFile, _argumentProvider.DataProvider);
        }

        private IExtractResult GenerateExtractResult(string gfrFile, IRasterDataProvider dataPrd)
        {
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", dataPrd.Width, dataPrd.Height, dataPrd.CoordEnvelope, dataPrd.SpatialRef);
            Dataset _dataset = Gdal.Open(gfrFile, Access.GA_ReadOnly);
            if (_dataset.RasterCount == 0)
                return result;
            else
            {
                CoordEnvelope envelope = dataPrd.CoordEnvelope.Clone();
                double maxX = envelope.MaxX;
                double minX = envelope.MinX;
                double maxY = envelope.MaxY;
                double minY = envelope.MinY;
                using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(gfrFile) as IVectorFeatureDataReader)
                {
                    Feature[] features = dr.FetchFeatures();
                    for (int i = 0; i < features.Length; i++)
                    {
                        double x, y;
                        if (double.TryParse(features[i].FieldValues[4], out x) && double.TryParse(features[i].FieldValues[3], out y))
                        {
                            if (IsInRange(minX, maxX, x) && IsInRange(minY, maxY, y))
                            {
                                int index = GetIndex(x, y);
                                if (index >= result.Count)
                                    break;
                                result.Put(index);
                            }
                        }
                    }
                }
                return result;
            }
        }

        private int GetIndex(double x, double y)
        {
            IRasterDataProvider dataPrd = _argumentProvider.DataProvider;
            double maxX = dataPrd.CoordEnvelope.MaxX;
            double minX = dataPrd.CoordEnvelope.MinX;
            double maxY = dataPrd.CoordEnvelope.MaxY;
            double minY = dataPrd.CoordEnvelope.MinY;
            double xRange = (maxX - minX) / dataPrd.Width;
            double yRange = (maxY - minY) / dataPrd.Height;
            int columnIndex = (int)Math.Round((x - minX) / xRange);
            int lineIndex = (int)Math.Round((maxY - y) / yRange);
            if (lineIndex == 0)
                lineIndex++;
            if (lineIndex > dataPrd.Height)
                lineIndex = dataPrd.Height;
            if (columnIndex == 0)
                columnIndex++;
            if (columnIndex > dataPrd.Width)
                columnIndex = dataPrd.Width;
            return lineIndex * dataPrd.Width + columnIndex;
        }

        private IExtractResult DoMakeUseAdvanceAlgorithm(Action<int, string> progressTracker)
        {
            return (new ExactExtracter()).Extracting(_argumentProvider, out _curFeatures, _contextMessage, progressTracker);
        }

        private IExtractResult DoMakeUseEasyAlgorithm(Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int band1 = TryGetBandNo(bandNameRaster,"Visible");
            int band2 = TryGetBandNo(bandNameRaster,"NearInfrared");
            int band3 = TryGetBandNo(bandNameRaster,"MiddleInfrared");
            int band4 = TryGetBandNo(bandNameRaster,"FarInfrared");
            if (band1 == -1 || band2 == -1 || band3 == -1 || band4 == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            //
            double band1_zoom = Obj2Double(_argumentProvider.GetArg("Visible_Zoom"));
            double band2_zoom = Obj2Double(_argumentProvider.GetArg("NearInfrared_Zoom"));
            double band3_zoom = Obj2Double(_argumentProvider.GetArg("MiddleInfrared_Zoom"));
            double band4_zoom = Obj2Double(_argumentProvider.GetArg("FarInfrared_Zoom"));
            if (double.IsNaN(band1_zoom))
                band1_zoom = 1;
            if (double.IsNaN(band2_zoom))
                band2_zoom = 1;
            if (double.IsNaN(band3_zoom))
                band3_zoom = 1;
            if (double.IsNaN(band4_zoom))
                band4_zoom = 1;
            //
            return DoMake(progressTracker, new int[] { band1, band2, band3, band4 }, new double[] { band1_zoom, band2_zoom, band3_zoom, band4_zoom });
        }

        private int[] Obj2IntArray(string argument)
        {
            object obj = _argumentProvider.GetArg(argument);
            if (obj == null)
                return null;
            return obj as int[];
        }

        private IExtractResult DoMake(Action<int, string> progressTracker, int[] bandNos, double[] bandZooms)
        {
            string express = "band{0} / " + bandZooms[0] + "f >= var_VisibleMin && band{0} / " + bandZooms[0] + "f <= var_VisibleMax && " +
                "band{1} / " + bandZooms[1] + "f >= var_NearInfraredMin && band{1} / " + bandZooms[1] + "f <= var_NearInfraredMax && " +
                "band{2} / " + bandZooms[2] + "f >= var_MiddleInfraredMin && band{2} / " + bandZooms[2] + "f <= var_MiddleInfraredMax && " +
                "band{3} / " + bandZooms[3] + "f >= var_FarInfraredMin && band{3} / " + bandZooms[3] + "f <= var_FarInfraredMax";
            express = string.Format(express, bandNos[0], bandNos[1], bandNos[2], bandNos[3]);
            //
            IPixelIndexMapper result = DoMake(express, bandNos, bandZooms);//疑似火点判识

            //不实时计算背景温度
            //int[] filteredAOI = result.Indexes.ToArray();
            //if (filteredAOI == null || filteredAOI.Length == 0)
            //    return result;
            //int[] firePoints = GetPreExtractIndex();

            //Size size = new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            //DoubtFirPixelFilter doubtFilter = CreateArgument.CreateDoubtFilter(_argumentProvider, _contextMessage);
            //if (doubtFilter == null)
            //    return result;

            //filteredAOI = MergeFirePoints(result, firePoints, filteredAOI);
            //Rectangle aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            ////背景温度计算
            //PrintInfo("[开始]背景亮温计算...");
            //BackTmpComputer backTmpComputer = CreateArgument.CreateBackTmpComputer(_argumentProvider, doubtFilter as IBackTmpComputerHelper, _contextMessage);
            //if (backTmpComputer == null)
            //    return null;
            //aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            //_curFeatures = backTmpComputer.Compute(_argumentProvider, aoiRect, filteredAOI);
            //VertifyFirPixel(ref _curFeatures);
            //result.Tag = new FirFeatureCollection("火点像元特征", _curFeatures);
            //if (_curFeatures.Count != 0)
            //    GetFirePointIndeiex.WriteFireIndexiexFilename(_argumentProvider, _curFeatures.Keys.ToArray());

            return result;
        }

        private int[] GetPreExtractIndex()
        {
            if (_argumentProvider.EnvironmentVarProvider is IMonitoringSession)
            {
                IMonitoringSession mSession = _argumentProvider.EnvironmentVarProvider as IMonitoringSession;
                if (mSession.ExtractingSession != null)
                {
                    IPixelIndexMapper preExtract = mSession.ExtractingSession.GetBinaryValuesMapper("FIR", "DBLV");
                    if (preExtract != null && preExtract.Indexes != null)
                        return preExtract.Indexes.ToArray();
                }
            }
            return null;
        }

        private int[] MergeFirePoints(IPixelIndexMapper result, int[] firePoints, int[] filteredAOI)
        {
            int[] resultIndex = null;
            UpdateFirePoints(ref firePoints);
            if (firePoints == null)
                resultIndex = filteredAOI;
            else
            {
                if (filteredAOI == null)
                {
                    result.Put(firePoints);
                    resultIndex = firePoints;
                }
                else
                {
                    List<int> firePointsArray = new List<int>();
                    firePointsArray.AddRange(firePoints);
                    List<int> filteredAOIArray = new List<int>();
                    filteredAOIArray.AddRange(filteredAOI);
                    resultIndex = firePointsArray.Union(filteredAOI).ToArray<int>();
                    result.Put(resultIndex);
                }
            }
            return resultIndex;
        }

        private void UpdateFirePoints(ref int[] firePoints)
        {
            int[] aoiIndex = _argumentProvider.AOI;
            if (aoiIndex == null || aoiIndex.Length == 0)
                firePoints = null;
            else
            {
                List<int> firePointsArray = new List<int>();
                int length = aoiIndex.Length;
                firePointsArray.AddRange(firePoints);
                for (int i = 0; i < length; i++)
                    firePointsArray.Remove(aoiIndex[i]);
                if (firePointsArray.Count != 0)
                    firePoints = firePointsArray.ToArray();
                else
                    firePoints = null;
            }
        }

        private void VertifyFirPixel(ref Dictionary<int, PixelFeature> features)
        {
            if (features == null || features.Count == 0)
                return;
            foreach (int item in features.Keys)
                features[item].IsVertified = true;
        }

        private IPixelIndexMapper DoMake(string express, int[] bandNos, double[] bandZooms)
        {
            IInterestedPixelExtracter extracter = CreateThresholdExtracter(_argumentProvider.DataProvider.DataType);
            Size size = new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", size.Width, size.Height, _argumentProvider.DataProvider.CoordEnvelope, _argumentProvider.DataProvider.SpatialRef);
            extracter.Reset(_argumentProvider, bandNos, express);
            extracter.Extract(result);
            return result;
        }

        private bool BandNosIsNotEquals(int[] bandNos)
        {
            return false;
        }

        private IInterestedPixelExtracter CreateThresholdExtracter(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new SimpleThresholdExtracter<byte>();
                case enumDataType.Int16:
                    return new SimpleThresholdExtracter<Int16>();
                case enumDataType.UInt16:
                    return new SimpleThresholdExtracter<UInt16>();
                case enumDataType.Int32:
                    return new SimpleThresholdExtracter<Int32>();
                case enumDataType.UInt32:
                    return new SimpleThresholdExtracter<UInt32>();
                case enumDataType.Float:
                    return new SimpleThresholdExtracter<float>();
                case enumDataType.Double:
                    return new SimpleThresholdExtracter<double>();
                case enumDataType.Int64:
                    return new SimpleThresholdExtracter<Int64>();
                case enumDataType.UInt64:
                    return new SimpleThresholdExtracter<UInt64>();
                default:
                    throw new NotSupportedException("简单阈值判识器不支持\"" + dataType.ToString() + "\"数据类型的栅格数据。");
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private double Obj2Double(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return double.NaN;
            return (double)v;
        }

        private float Obj2Float(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return float.NaN;
            return (float)v;
        }

        private string Obj2String(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return null;
            return (string)v;
        }

        private int Obj2Int(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return -1;
            return (int)v;
        }

        private bool IsInRange(double min, double max, double value)
        {
            if (value <= max && value >= min)
                return true;
            else return false;
        }
    }
}
