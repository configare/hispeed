using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductLAODHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductLAODHAZE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LAODAlgorithm")
            {
                return LAODAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult LAODAlgorithm(Action<int, string> progressTracker)
        {
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            string[] argFileArg = _argumentProvider.GetArg("RegionFileName") as string[];
            string argFileName = argFileArg[0];
            if (string.IsNullOrEmpty(argFileName))
            {
                PrintInfo("请设置等级参数文件！");
                return null;
            }
            SortedDictionary<float, float[]> levelRegions = GetArgFileRegion(argFileName);
            if (levelRegions == null || levelRegions.Count == 0)
                return null;
            string AODFile = Convert.ToString(_argumentProvider.GetArg("AODFile"));
            if (string.IsNullOrWhiteSpace(AODFile))
                return null;
            string[] aodFiles = AODFile.Split(new char[] { ',' });
            if (aodFiles.Length != 2)
                return null;
            string aodFile = aodFiles[0];
            int bandNo = 1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int aodNo = TryGetBandNo(bandNameRaster, "AODNO");
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                IRasterDataProvider dblv = RasterDataDriver.Open(files[0]) as IRasterDataProvider;
                RasterMaper rmDBLV = new RasterMaper(dblv, new int[] { bandNo });
                rms.Add(rmDBLV);
                IRasterDataProvider aod = RasterDataDriver.Open(aodFile) as IRasterDataProvider;
                if (aod.BandCount < bandNo)
                {
                    PrintInfo("请选择正确的AOD数据进行定量产品计算。");
                    return null;
                }
                RasterMaper rmAOD = new RasterMaper(aod, new int[] { aodNo });
                rms.Add(rmAOD);
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = GetRasterIdentifyID(files);
                string outFileName = MifEnvironment.GetTempDir() + "\\" + ri.ToWksFileName(".dat");
                bool isVaild = false;
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        for (int index = 0; index < dataLength; index++)

                            if (rvInVistor[0].RasterBandsData != null && rvInVistor[0].RasterBandsData[0] != null &&
                                rvInVistor[0].RasterBandsData[0][index] != 0)
                            {
                                if (rvInVistor[1].RasterBandsData != null && rvInVistor[1].RasterBandsData[0] != null)
                                {
                                    foreach (float minValue in levelRegions.Keys)
                                    {
                                        if (rvInVistor[1].RasterBandsData[0][index] >= minValue && rvInVistor[1].RasterBandsData[0][index] < levelRegions[minValue][0])
                                        {
                                            rvOutVistor[0].RasterBandsData[0][index] = (Int16)levelRegions[minValue][1];
                                            isVaild = true;
                                        }
                                    }
                                    if (!isVaild)
                                        rvOutVistor[0].RasterBandsData[0][index] = (Int16)1;
                                    isVaild = false;
                                }
                            }
                    }));
                    //执行
                    rfr.Excute();
                    if (File.Exists(outFileName))
                    {
                        string dstFilename = ri.ToWksFullFileName(".dat");
                        CopyFileToDstDir(outFileName, dstFilename);
                        FileExtractResult res = new FileExtractResult(_subProductDef.Identify, dstFilename, true);
                        res.SetDispaly(false);
                        CreateThemegrahic(dstFilename);
                        return res;
                    }
                    return null;
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

        private void CopyFileToDstDir(string srcFileName, string dstFilename)
        {
            string dir = Path.GetDirectoryName(dstFilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(dstFilename))
                File.Delete(dstFilename);
            File.Copy(srcFileName, dstFilename);
        }

        private void CreateThemegrahic(string filename)
        {
            IMonitoringSession ms = _argumentProvider.EnvironmentVarProvider as IMonitoringSession;
            IMonitoringSubProduct subDef = ms.ActiveMonitoringProduct.GetSubProductByIdentify("0IMG");
            if (subDef != null)
                try
                {
                    subDef.Definition.IsKeepUserControl = true;
                    ms.ChangeActiveSubProduct("0IMG");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "LAOI");
                    ms.DoAutoExtract(false);
                    ms.ChangeActiveSubProduct("LAOD");
                }
                finally
                {
                    if (subDef != null)
                        subDef.Definition.IsKeepUserControl = false;
                }
        }

        public IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = inrasterMaper[0].Raster.CoordEnvelope;
            //GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            //foreach (RasterMaper inRaster in inrasterMaper)
            //{
            //    if (outEnv == null)
            //        outEnv = inRaster.Raster.CoordEnvelope;
            //    else
            //        outEnv.Union(inRaster.Raster.CoordEnvelope);
            //}
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.CYCFlag = "";
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;

            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();

            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private SortedDictionary<float, float[]> GetArgFileRegion(string argFileName)
        {
            if (string.IsNullOrEmpty(argFileName) || !File.Exists(argFileName))
                return null;
            SortedDictionary<float, float[]> regionValues = new SortedDictionary<float, float[]>();
            string[] lines = File.ReadAllLines(argFileName, Encoding.Default);
            if (lines == null || lines.Length < 1)
                return null;
            for (int i = 0; i < lines.Length; i++)
            {
                float[] minmax = ParseRegionToFloat(lines[i]);
                if (minmax == null || minmax.Length == 0)
                    continue;
                regionValues.Add(MathCompare.FloatCompare(minmax[0], float.MinValue) ? float.MinValue : (minmax[0]),
                   new float[] { MathCompare.FloatCompare(minmax[1], float.MaxValue) ? float.MaxValue : (minmax[1]), minmax[2] });
            }
            return regionValues;
        }

        private float[] ParseRegionToFloat(string re)
        {
            if (string.IsNullOrEmpty(re))
                return null;
            string[] parts = re.Split(new char[] { '~', ' ' });
            if (parts == null || parts.Length == 0)
                return null;
            float min, max, level;
            if (float.TryParse(parts[0], out min) && float.TryParse(parts[1], out max) && float.TryParse(parts[2], out level))
                return new float[] { min, max, level };
            return null;
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
