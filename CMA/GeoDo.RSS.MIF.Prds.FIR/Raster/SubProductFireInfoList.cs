using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductFireInfoList : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProductFireInfoList(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            _progressTracker = progressTracker;
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FireAreaAlgorithm")
            {
                return FRILAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult FRILAlgorithm()
        {
            int VisibleCH = Obj2Int(_argumentProvider.GetArg("Visible"));
            int NearInfraredCH = Obj2Int(_argumentProvider.GetArg("NearInfrared"));
            int MiddleInfraredCH = Obj2Int(_argumentProvider.GetArg("MiddleInfrared"));
            int FarInfraredCH = Obj2Int(_argumentProvider.GetArg("FarInfrared"));
            string currRaster = Obj2String(_argumentProvider.GetArg("CurrentRasterFile"));
            string currDBLV = Obj2String(_argumentProvider.GetArg("DBLVFile"));

            if (VisibleCH == -1 || FarInfraredCH == -1 || NearInfraredCH == -1 || MiddleInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误。");
                return null;
            }
            if (string.IsNullOrEmpty(currRaster) || string.IsNullOrEmpty(currDBLV))
            {
                PrintInfo("获取算法所用文件失败。");
                return null;
            }

            List<int> vertifyIndexiex = null;
            IPixelIndexMapper result = CreataPixelMapper(currDBLV, out vertifyIndexiex);
            if (result == null || result.Indexes.ToArray().Length == 0)
            {
                PrintInfo("当前判识结果中无火点信息。");
                return null;
            }
            int[] filteredAOI = result.Indexes.ToArray();
            Size size = new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height);
            DoubtFirPixelFilter doubtFilter = CreateArgument.CreateDoubtFilter(_argumentProvider, _contextMessage);
            if (doubtFilter == null)
                return null;
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            //背景温度计算
            PrintInfo("[开始]背景亮温计算...");
            BackTmpComputer backTmpComputer = CreateArgument.CreateBackTmpComputer(_argumentProvider, doubtFilter as IBackTmpComputerHelper, _contextMessage);
            if (backTmpComputer == null)
                return null;
            aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            Dictionary<int, PixelFeature> curFeatures = backTmpComputer.Compute(_argumentProvider, aoiRect, filteredAOI);
            VertifyFirPixel(ref curFeatures, vertifyIndexiex, ref result);
            return GetOtherExtractResult.GetExtractResult(_argumentProvider, curFeatures, result, _contextMessage, _progressTracker);
        }

        private void VertifyFirPixel(ref Dictionary<int, PixelFeature> features, List<int> vertifyIndexiex, ref IPixelIndexMapper result)
        {
            if (features == null || features.Count == 0)
                return;
            List<int> pixelIndex = new List<int>();
            foreach (int item in features.Keys)
                if (vertifyIndexiex.Contains(item))
                    features[item].IsVertified = true;
                else
                {
                    features[item].IsVertified = false;
                    pixelIndex.Add(item);
                }
            if (pixelIndex.Count != 0)
                result.Remove(pixelIndex.ToArray());
        }


        private unsafe IPixelIndexMapper CreataPixelMapper(string currDBLV, out List<int> vertifyIndexiex)
        {
            IPixelIndexMapper dblvInfo = null;
            vertifyIndexiex = new List<int>();
            using (IRasterDataProvider gd = GeoDataDriver.Open(currDBLV) as IRasterDataProvider)
            {
                string fiifFile = GetFirePointIndeiex.GetFireIndexiexFilename(currDBLV);
                dblvInfo = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", gd.Width, gd.Height, gd.CoordEnvelope, gd.SpatialRef);
                int[] indexiex = null;
                if (!string.IsNullOrEmpty(fiifFile))
                {
                    GetFirePointIndeiex.ReadFireIndexiexFilename(_argumentProvider, out indexiex);
                    dblvInfo.Put(indexiex);
                }
                Int16[] dataBlock = new Int16[gd.Width * gd.Height];
                IRasterBand band = gd.GetRasterBand(1);
                fixed (Int16* buffer = dataBlock)
                {
                    IntPtr ptr = new IntPtr(buffer);
                    band.Read(0, 0, gd.Width, gd.Height, ptr, enumDataType.Int16, gd.Width, gd.Height);
                }
                int length = dataBlock.Length;
                if (string.IsNullOrEmpty(fiifFile))
                    for (int i = 0; i < length; i++)
                    {
                        if (dataBlock[i] == (Int16)1)
                        {
                            dblvInfo.Put(i);
                            vertifyIndexiex.Add(i);
                        }
                    }
                else
                    for (int i = 0; i < length; i++)
                    {
                        if (dataBlock[i] == (Int16)1)
                        {
                            vertifyIndexiex.Add(i);
                        }
                    }
            }
            return dblvInfo;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
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
    }
}
