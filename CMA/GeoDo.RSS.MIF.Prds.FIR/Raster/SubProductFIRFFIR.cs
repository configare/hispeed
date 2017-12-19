using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductFIRFFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductFIRFFIR(SubProductDef subProductDef)
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
            IRasterDataProvider currPrd = _argumentProvider.DataProvider;
            if (currPrd == null)
                return null;
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (string.IsNullOrEmpty(algorithmName))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            switch (algorithmName)
            {
                case "FIRFAlgorithm":
                    return FIRFMack(progressTracker);
                default:
                    PrintInfo("指定的算法\"" + algorithmName + "\"没有实现。");
                    return null;
            }

        }

        private IExtractResult FIRFMack(Action<int, string> progressTracker)
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int NearInfrared = TryGetBandNo(bandNameRaster, "NearInfrared");
            int CoverageBand = (int)_argumentProvider.GetArg("CoverageBand");
            double NearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double CoverageZoom = (double)_argumentProvider.GetArg("CoverageBand_Zoom");
            float NearInfraredMax = (float)_argumentProvider.GetArg("NearInfraredMax");
            float CoverageMin = (float)_argumentProvider.GetArg("CoverageMin");
            float FIRLZoom = (float)_argumentProvider.GetArg("FIRFZoom");

            if (NearInfrared == -1 || CoverageBand == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            string coverageFile = _argumentProvider.GetArg("coverageFile") == null ? null : _argumentProvider.GetArg("coverageFile").ToString();
            if (string.IsNullOrEmpty(coverageFile))
            {
                PrintInfo("请设置背景农田百分比数据！");
                return null;
            }

            float maxAvgValue;
            float minAvgValue;
            string[] nearInfValues = _argumentProvider.GetArg("NearInfraredValues") as string[];
            if (nearInfValues == null || nearInfValues.Count() != 2)
                return null;
            if (!float.TryParse(nearInfValues[0], out maxAvgValue) || !float.TryParse(nearInfValues[1], out minAvgValue))
                return null;
            if (maxAvgValue == minAvgValue)
                return null;
            float dltValue = maxAvgValue - minAvgValue;
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider curPrd = _argumentProvider.DataProvider;
            IRasterDataProvider coveragePrd = null;
            try
            {
                RasterMaper nearRm = new RasterMaper(curPrd, new int[] { NearInfrared });
                rms.Add(nearRm);

                coveragePrd = RasterDataDriver.Open(coverageFile) as IRasterDataProvider;
                if (coveragePrd.BandCount < CoverageBand)
                {
                    PrintInfo("请选择正确的农田百分比数据文件通道值!");
                    return null;
                }
                RasterMaper coverageRm = new RasterMaper(coveragePrd, new int[] { CoverageBand });
                rms.Add(coverageRm);

                string outFileName = GetFileName(new string[] { curPrd.fileName }, _subProductDef.ProductDef.Identify, _identify, ".dat", null);
                IPixelIndexMapper result = null;
                IPixelFeatureMapper<Int16> resultTag = null;
                int totalDatalength = 0;
                float tempValue = 0;
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", outRaster.Width, outRaster.Height, outRaster.CoordEnvelope, outRaster.SpatialRef);
                    if (this.Tag == null || (this.Tag as MemPixelFeatureMapper<Int16>) == null)
                        resultTag = new MemPixelFeatureMapper<Int16>("FIFLT", 1000, new Size(outRaster.Width, outRaster.Height), outRaster.CoordEnvelope, outRaster.SpatialRef);
                    else
                        resultTag = this.Tag as MemPixelFeatureMapper<Int16>;
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.SetFeatureAOI(_argumentProvider.AOIs);
                    rfr.RegisterCalcModel(new RasterCalcHandlerFun<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                        {
                            totalDatalength += dataLength;
                            return false;
                        }
                        if (_argumentProvider.AOIs == null)
                            for (int index = 0; index < dataLength; index++)
                            {
                                if (IsFirA(rvInVistor, index, NearInfraredZoom, NearInfraredMax))
                                {
                                    result.Put(totalDatalength + index);

                                    tempValue = (maxAvgValue - rvInVistor[0].RasterBandsData[0][index]) / dltValue;
                                    if (tempValue < rvInVistor[1].RasterBandsData[0][index] / CoverageZoom)
                                        resultTag.Put(totalDatalength + index, tempValue < 0 ? (Int16)0 : (tempValue > 1 ? (Int16)(FIRLZoom) : (Int16)(tempValue * FIRLZoom)));
                                    else
                                        resultTag.Put(totalDatalength + index, (Int16)(rvInVistor[1].RasterBandsData[0][index] / CoverageZoom * FIRLZoom));
                                }
                            }
                        else if (_argumentProvider.AOIs != null && aoi != null && aoi.Length != 0)
                        {
                            int indexFromAOI = 0;
                            for (int i = 0; i < aoi.Length; i++)
                            {
                                indexFromAOI = aoi[i];
                                if (IsFirA(rvInVistor, indexFromAOI, NearInfraredZoom, NearInfraredMax))
                                {
                                    result.Put(totalDatalength + indexFromAOI);

                                    tempValue = (maxAvgValue - rvInVistor[0].RasterBandsData[0][aoi[i]]) / dltValue;
                                    if (tempValue < rvInVistor[1].RasterBandsData[0][aoi[i]] / CoverageZoom)
                                        resultTag.Put(totalDatalength + indexFromAOI, tempValue < 0 ? (Int16)0 : (tempValue > 1 ? (Int16)(FIRLZoom) : (Int16)(tempValue * FIRLZoom)));
                                    else
                                        resultTag.Put(totalDatalength + indexFromAOI, (Int16)(rvInVistor[1].RasterBandsData[0][aoi[i]] / CoverageZoom * FIRLZoom));
                                }
                            }
                        }
                        totalDatalength += dataLength;
                        return false;
                    }));
                    //执行
                    rfr.Excute();
                    this.Tag = resultTag;
                    return result;
                }
            }
            finally
            {
                if (coveragePrd != null)
                    coveragePrd.Dispose();
            }
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            if (piexd == null || this.Tag == null)
                return null;
            IPixelFeatureMapper<Int16> resultTag = this.Tag as MemPixelFeatureMapper<Int16>;
            if (resultTag == null)
                return null;
            //生成判识结果文件
            IInterestedRaster<Int16> iir = null;
            IInterestedRaster<Int16> iirFIFLT = null;

            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = _identify;
            id.GenerateDateTime = DateTime.Now;
            iir = new InterestedRaster<Int16>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
            int[] idxs = piexd.Indexes.ToArray();
            iir.Put(idxs, 1);

            id.SubProductIdentify = "FIFLT";
            id.GenerateDateTime = DateTime.Now;
            iirFIFLT = new InterestedRaster<Int16>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
            iirFIFLT.Put(resultTag);

            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider FIRLPrd = null;
            IRasterDataProvider FIFLTPrd = null;
            try
            {
                FIRLPrd = iir.HostDataProvider;
                RasterMaper fiflRm = new RasterMaper(FIRLPrd, new int[] { 1 });
                rms.Add(fiflRm);

                FIFLTPrd = iirFIFLT.HostDataProvider;
                RasterMaper fifltRm = new RasterMaper(FIFLTPrd, new int[] { 1 });
                rms.Add(fifltRm);

                string outFileName = GetFileName(new string[] { _argumentProvider.DataProvider.fileName }, _subProductDef.ProductDef.Identify, "FIFL", ".dat", null);
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
                        // if (_argumentProvider.AOIs == null)
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][index] == 0)
                                rvOutVistor[0].RasterBandsData[0][index] = (Int16)0;
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = rvInVistor[1].RasterBandsData[0][index];
                        }
                        //else if (_argumentProvider.AOIs != null && aoi != null && aoi.Length != 0)
                        //{
                        //    for (int i = 0; i < aoi.Length; i++)
                        //        if (rvInVistor[0].RasterBandsData[0][aoi[i]] == 0)
                        //            rvOutVistor[0].RasterBandsData[0][aoi[i]] = (Int16)0;
                        //        else
                        //            rvOutVistor[0].RasterBandsData[0][aoi[i]] = rvInVistor[1].RasterBandsData[0][aoi[i]];
                        //}
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult("FIFL", outFileName, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                iir.Dispose();
                if (File.Exists(iir.FileName))
                    File.Delete(iir.FileName);
                iirFIFLT.Dispose();
                if (File.Exists(iirFIFLT.FileName))
                    File.Delete(iirFIFLT.FileName);
            }
        }

        private bool IsFirA(RasterVirtualVistor<short>[] rvInVistor, int index, double NearInfraredZoom, float NearInfraredMax)
        {
            Int16 value0 = 0;
            value0 = rvInVistor[0].RasterBandsData[0][index];
            if (value0 == 0)
                return false;
            else if (rvInVistor[0].RasterBandsData[0][index] / NearInfraredZoom < NearInfraredMax)
                return true;
            return false;
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
