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
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.DF.MEM;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    /// <summary>
    /// 类名：SubProductRasterFldFloodLastdays
    /// 属性描述：
    /// 创建者：ChenN        创建日期：2015/9/10
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductRasterFldFloodLastdays : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        //泛滥水体天数
        public SubProductRasterFldFloodLastdays(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("SelectedPrimaryFiles") == null)
                return null;
            string[] binWater = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
            for (int i = 0; i < binWater.Length; i++)
            {
                if (!File.Exists(binWater[i]))
                    return null;
            }
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            IFileExtractResult[] flodFiles = null;
            IExtractResult fldsFile = null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "DayFLDSAlgorithm")
            {
                flodFiles = GreateFLODFile(binWater, progressTracker);
                if (flodFiles == null || flodFiles.Length == 0 || string.IsNullOrEmpty(flodFiles[0].FileName))
                    return null;
                List<string> toFLDSFiles = new List<string>();
                foreach (IFileExtractResult item in flodFiles)
                    toFLDSFiles.Add(item.FileName);
                fldsFile = FLODLastFiles(toFLDSFiles.ToArray(), progressTracker);
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FloodFLDSAlgorithm")
                fldsFile = FLODLastFiles(binWater, progressTracker);
            if (fldsFile != null)
            {
                IExtractResultArray array = new ExtractResultArray("洪涝持续天数");
                if (flodFiles != null && flodFiles.Length != 0)
                {
                    foreach (IFileExtractResult item in flodFiles)
                    {
                        (item as IFileExtractResult).SetDispaly(false);
                        array.Add(item as IExtractResultBase);
                    }
                }
                array.Add(fldsFile as IExtractResultBase);
                return array;
            }
            return null;
        }

        private IFileExtractResult[] GreateFLODFile(string[] dblvFiles, Action<int, string> progressTracker)
        {
            if (dblvFiles == null || dblvFiles.Length < 2)
                return null;
            string[] sortedFiles = SortFileName(dblvFiles);
            IFileExtractResult flodFile = null;
            List<IFileExtractResult> flodFiles = new List<IFileExtractResult>();
            for (int i = 1; i < sortedFiles.Length; i++)
            {
                flodFile = CompareDATFile(sortedFiles[i - 1], sortedFiles[i]);
                if (flodFile != null && !string.IsNullOrEmpty(flodFile.FileName))
                    flodFiles.Add(flodFile);
            }
            return flodFiles.Count == 0 ? null : flodFiles.ToArray();
        }

        private IFileExtractResult CompareDATFile(string backWaterPath, string binWater)
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("backWaterPath", new FilePrdMap(backWaterPath, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
            filePrdMap.Add("binWater", new FilePrdMap(binWater, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (vrd == null)
            {
                PrintInfo("数据间无相交部分,无法进行泛滥缩小水体计算!");
                if (filePrdMap != null && filePrdMap.Count > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
                return null;
            }
            try
            {
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("FLOD", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand,
                                                 filePrdMap["binWater"].StartBand},
                    (idx, values) =>
                    {
                        if (values[0] == 1 && values[1] == 1)
                            result.Put(idx, 1);
                        else if (values[0] == 1 && values[1] == 0)
                            result.Put(idx, 5);
                        else if (values[0] == 0 && values[1] == 1)
                            result.Put(idx, 4);
                    });
                RasterIdentify rid = new RasterIdentify(new string[] { backWaterPath, binWater });
                rid.ProductIdentify = _subProductDef.ProductDef.Identify;
                rid.SubProductIdentify = "FLOD";
                rid.IsOutput2WorkspaceDir = true;
                IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, result.Size, result.CoordEnvelope, result.SpatialRef);
                iir.Put(result);
                iir.Dispose();
                return new FileExtractResult("扩大缩小水体", iir.FileName);
            }
            finally
            {
                vrd.Dispose();
            }
        }

        private IExtractResult FLODLastFiles(string[] flodFiles, Action<int, string> progressTracker)
        {
            if (flodFiles == null || flodFiles.Length < 1)
                return null;
            string[] sortedFiles = SortFileName(flodFiles);
            List<RasterMaper> rms = new List<RasterMaper>();
            RasterIdentify daysByri = null;
            List<UInt16> days = new List<UInt16>();
            List<UInt16> fd3Days = null;//三个数据时的分段日数值
            bool[] fd3Infos = new bool[7] { false, false, false, false, false, false, false };//三个数据时的7种分段情况
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");
            bool autoFJ = bool.Parse(_argumentProvider.GetArg("autoFJ").ToString());
            try
            {
                foreach (string file in sortedFiles)
                {
                    IRasterDataProvider flodRaster = RasterDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(flodRaster, new int[] { 1 });
                    rms.Add(rm);
                    daysByri = new RasterIdentify(file);
                    days.Add((UInt16)((DateTime.Parse(daysByri.MaxOrbitDate.ToShortDateString()) - DateTime.Parse(daysByri.MinOrbitDate.ToShortDateString())).Days));
                }
                int extendHeaderLength = days.Count * 2;
                if (days.Count == 3)
                    fd3Days = SetFD3Days(days);
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(flodFiles, null);
                string outFileName = ri.ToWksFullFileName(".dat");
                //创建结果数据
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray(), extendHeaderLength))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<UInt16, UInt16> rfr = null;
                    rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    int fileCount = fileIns.Length;
                    List<UInt16> everyDays = new List<UInt16>();
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][i] == 4)
                                everyDays.Add(days[0]);
                            else if (rvInVistor[0].RasterBandsData[0][i] == 1 || rvInVistor[0].RasterBandsData[0][i] == 5)
                                everyDays.Add(defWater);
                            for (int fileIndex = 1; fileIndex < fileCount; fileIndex++)
                            {
                                if ((rvInVistor[fileIndex].RasterBandsData[0][i] == 4) || ProIsFlood(rvInVistor, i, fileIndex))
                                    everyDays.Add(days[fileIndex]);
                                else
                                    everyDays.Add(0);
                            }
                            rvOutVistor[0].RasterBandsData[0][i] = AnalysisLastDays(everyDays, fd3Days, ref fd3Infos);
                            everyDays.Clear();
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    res.SetDispaly(false);
                    LastDaysSetValue extHeader = SetColorLegend(fd3Days, fd3Infos);
                    (outRaster as MemoryRasterDataProvider).SetExtHeader<LastDaysSetValue>(extHeader, extendHeaderLength);
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

        private static bool ProIsFlood(RasterVirtualVistor<ushort>[] rvInVistor, int i, int fileIndex)
        {
            for (int j = fileIndex; j > 0; j--)
            {
                if (rvInVistor[fileIndex].RasterBandsData[0][i] == 1 && rvInVistor[j - 1].RasterBandsData[0][i] == 4)
                    return true;
            }
            return false;
        }

        private LastDaysSetValue SetColorLegend(List<ushort> fd3Days, bool[] fd3Infos)
        {
            List<UInt16> colorNum = new List<ushort>();

            if (fd3Infos[0] && fd3Infos[1] && fd3Infos[2])
                colorNum.Add((fd3Days[0] > fd3Days[1] ? fd3Days[0] : fd3Days[1]) > fd3Days[2] ? (fd3Days[0] > fd3Days[1] ? fd3Days[0] : fd3Days[1]) : fd3Days[2]);
            else if (fd3Infos[0] && fd3Infos[1])
                colorNum.Add(fd3Days[0] > fd3Days[1] ? fd3Days[0] : fd3Days[1]);
            else if (fd3Infos[0] && fd3Infos[2])
                colorNum.Add(fd3Days[0] > fd3Days[2] ? fd3Days[0] : fd3Days[2]);
            else if (fd3Infos[1] && fd3Infos[2])
                colorNum.Add(fd3Days[1] > fd3Days[2] ? fd3Days[1] : fd3Days[2]);
            else if (fd3Infos[0])
                colorNum.Add(fd3Days[0]);
            else if (fd3Infos[1])
                colorNum.Add(fd3Days[1]);
            else if (fd3Infos[2])
                colorNum.Add(fd3Days[2]);

            if (fd3Infos[3] && fd3Infos[4] && fd3Infos[5])
                colorNum.Add((fd3Days[3] > fd3Days[4] ? fd3Days[3] : fd3Days[4]) > fd3Days[5] ? (fd3Days[3] > fd3Days[4] ? fd3Days[3] : fd3Days[4]) : fd3Days[5]);
            else if (fd3Infos[3] && fd3Infos[4])
                colorNum.Add(fd3Days[3] > fd3Days[4] ? fd3Days[3] : fd3Days[4]);
            else if (fd3Infos[3] && fd3Infos[5])
                colorNum.Add(fd3Days[3] > fd3Days[5] ? fd3Days[3] : fd3Days[5]);
            else if (fd3Infos[4] && fd3Infos[5])
                colorNum.Add(fd3Days[4] > fd3Days[5] ? fd3Days[4] : fd3Days[5]);
            else if (fd3Infos[3])
                colorNum.Add(fd3Days[3]);
            else if (fd3Infos[4])
                colorNum.Add(fd3Days[4]);
            else if (fd3Infos[5])
                colorNum.Add(fd3Days[5]);

            if (fd3Infos[6])
                colorNum.Add(fd3Days[6]);

            for (int i = 0; i < 3 - colorNum.Count; i++)
                colorNum.Add(0);

            return new LastDaysSetValue(colorNum.ToArray());
        }

        private List<ushort> SetFD3Days(List<ushort> days)
        {
            // 0-a 1-b 2-c 3-ab 4-bc 5-ac 6-abc
            List<UInt16> fd3Days = new List<ushort>();
            fd3Days.Add(days[0]);
            UInt16 bSingle = (UInt16)((days[1] + days[2]) / 2);
            fd3Days.Add(bSingle > days[1] ? days[1] : days[1]);
            fd3Days.Add(days[2]);
            fd3Days.Add((UInt16)(days[0] + days[1]));
            fd3Days.Add((UInt16)(days[1] + days[2]));
            fd3Days.Add((UInt16)(days[0] + days[2]));
            fd3Days.Add((UInt16)(days[0] + days[1] + days[2]));
            return fd3Days;
        }

        private UInt16 AnalysisLastDays(List<UInt16> everyDays, List<UInt16> fd3Days, ref bool[] fd3Info)
        {
            UInt16 defCloudy = (UInt16)_argumentProvider.GetArg("defCloudy");
            UInt16 defWater = (UInt16)_argumentProvider.GetArg("defWater");
            UInt16 lastDays = 0;
            if (everyDays.Count == 3)
            {
                if ((everyDays[0] != 0 && everyDays[0] != defWater) && (everyDays[1] != 0 && everyDays[1] != defWater) && (everyDays[2] != 0 && everyDays[2] != defWater))
                {
                    lastDays = fd3Days[6];
                    fd3Info[6] = true;
                }
                else if ((everyDays[0] != 0 && everyDays[0] != defWater) && (everyDays[2] != 0 && everyDays[2] != defWater))
                {
                    lastDays = fd3Days[5];
                    fd3Info[5] = true;
                }
                else if ((everyDays[1] != 0 && everyDays[1] != defWater) && (everyDays[2] != 0 && everyDays[2] != defWater))
                {
                    lastDays = fd3Days[4];
                    fd3Info[4] = true;
                }
                else if ((everyDays[0] != 0 && everyDays[0] != defWater) && (everyDays[1] != 0 && everyDays[1] != defWater))
                {
                    lastDays = fd3Days[3];
                    fd3Info[3] = true;
                }
                else if ((everyDays[2] != 0 && everyDays[2] != defWater))
                {
                    lastDays = fd3Days[2];
                    fd3Info[2] = true;
                }
                else if ((everyDays[1] != 0 && everyDays[1] != defWater))
                {
                    lastDays = fd3Days[1];
                    fd3Info[1] = true;
                }
                else if ((everyDays[0] != 0 && everyDays[0] != defWater))
                {
                    lastDays = fd3Days[0];
                    fd3Info[0] = true;
                }
                foreach (UInt16 day in everyDays)
                    if (day == defWater)
                    {
                        lastDays = defWater;
                        break;
                    }
            }
            else
                foreach (UInt16 day in everyDays)
                {
                    if (day == defWater)
                    {
                        lastDays = defWater;
                        break;
                    }
                    lastDays += day;
                }
            return lastDays;
        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames, string outFileId)
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

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, int extHeaderLength)
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
                if (resX < inRaster.Raster.ResolutionX)
                    resX = inRaster.Raster.ResolutionX;
                if (resY < inRaster.Raster.ResolutionY)
                    resY = inRaster.Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.UInt16, mapInfo, "EXTHEADERSIZE=" + extHeaderLength) as RasterDataProvider;
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

    public struct LastDaysSetValue
    {
        private UInt16[] _lastDaysColor;

        public LastDaysSetValue(UInt16[] lastDaysColor)
        {
            _lastDaysColor = lastDaysColor;
        }

        public UInt16[] LastDaysColor
        {
            get { return _lastDaysColor; }
        }
    }
}
