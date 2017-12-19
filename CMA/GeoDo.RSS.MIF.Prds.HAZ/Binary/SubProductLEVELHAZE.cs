using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductLEVELHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductLEVELHAZE(SubProductDef subProductDef)
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
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LEVAlgorithm")
            {
                return LEVAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult LEVAlgorithm()
        {
            Int16 levelNum = (Int16)_argumentProvider.GetArg("Level");

            if (levelNum == 0)
            {
                PrintInfo("设置的分级等级数值不正确。");
                return null;
            }
            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "HAZ";
            id.SubProductIdentify = _identify;
            string lastLevelFile = id.ToWksFullFileName(".dat");
            string lastLevelTempFile = Path.Combine(Path.GetDirectoryName(lastLevelFile), Path.GetFileNameWithoutExtension(lastLevelFile) + "temp.dat");
            if (File.Exists(lastLevelFile))
                File.Copy(lastLevelFile, lastLevelTempFile, true);
            return null;
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            if (piexd == null)
                return null;
            Int16 levelNum = (Int16)_argumentProvider.GetArg("Level");
            //生成判识结果文件
            IInterestedRaster<Int16> iir = null;
            RasterIdentify id = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "HAZ";
            id.SubProductIdentify = _identify;
            id.GenerateDateTime = DateTime.Now;
            iir = new InterestedRaster<Int16>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
            int[] idxs = piexd.Indexes.ToArray();
            iir.Put(idxs, 1);

            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider LevelPrd = null;
            IRasterDataProvider CurrPrd = null;
            string lastLevelFile = null;
            string lastLevelTempFile = null;
            try
            {
                CurrPrd = iir.HostDataProvider;
                RasterMaper CurrRm = new RasterMaper(CurrPrd, new int[] { 1 });
                rms.Add(CurrRm);

                bool lastLevelExist = false;
                lastLevelFile = id.ToWksFullFileName(".dat");
                lastLevelTempFile = Path.Combine(Path.GetDirectoryName(lastLevelFile), Path.GetFileNameWithoutExtension(lastLevelFile) + "temp.dat");
                if (File.Exists(lastLevelTempFile))
                {
                    LevelPrd = GeoDataDriver.Open(lastLevelTempFile) as IRasterDataProvider;
                    if (LevelPrd != null)
                    {
                        lastLevelExist = true;
                        RasterMaper lastLevelRm = new RasterMaper(LevelPrd, new int[] { 1 });
                        rms.Add(lastLevelRm);
                    }
                }
                using (IRasterDataProvider outRaster = CreateOutRaster(lastLevelFile, rms.ToArray()))
                {
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        if (rvInVistor[0].RasterBandsData == null || (lastLevelExist && rvInVistor[1].RasterBandsData == null) ||
                            rvInVistor[0].RasterBandsData[0] == null || (lastLevelExist && rvInVistor[1].RasterBandsData[0] == null))
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][index] == 1)
                                rvOutVistor[0].RasterBandsData[0][index] = levelNum;
                            else if (lastLevelExist && rvInVistor[1].RasterBandsData[0] != null)
                                rvOutVistor[0].RasterBandsData[0][index] = rvInVistor[1].RasterBandsData[0][index];
                        }
                    }));
                    //执行
                    rfr.Excute();
                    FileExtractResult res = new FileExtractResult(_identify, lastLevelFile, true);
                    res.SetDispaly(false);
                    return res;
                }
            }
            finally
            {
                iir.Dispose();
                if (File.Exists(iir.FileName))
                    DelteAboutFile(iir.FileName);
                if (LevelPrd != null)
                {
                    LevelPrd.Dispose();
                    if (File.Exists(LevelPrd.fileName))
                        DelteAboutFile(LevelPrd.fileName);
                }
                if (File.Exists(lastLevelTempFile))
                    DelteAboutFile(lastLevelTempFile);
            }
        }

        private void DelteAboutFile(string Filename)
        {
            string[] aboutFiles = Directory.GetFiles(Path.GetDirectoryName(Filename), "*" + Path.GetFileNameWithoutExtension(Filename) + "*", SearchOption.TopDirectoryOnly);
            if (aboutFiles != null && aboutFiles.Length != 0)
            {
                foreach (string file in aboutFiles)
                    File.Delete(file);
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

    }
}
