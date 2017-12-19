using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductFRDSSNW : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductFRDSSNW(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FRDSAlgorithm")
            {
                return FRDSAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult FRDSAlgorithm(Action<int, string> progressTracker)
        {
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与积雪天数统计的数据！");
                return null;
            }
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }
            int bandNo = 1;
            ExtractResultArray array = new ExtractResultArray("SNW");
            //输出文件准备（作为输入栅格并集处理）
            RasterIdentify ri = GetRasterIdentifyID(ref fileNames);
            string outFileName = ri.ToWksFullFileName(".dat");
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            string tempFilename;
            try
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    tempFilename = ProcessCloud(fileNames[i], bandNo);
                    IRasterDataProvider inRaster = RasterDataDriver.Open(tempFilename) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                    {
                        PrintInfo("请选择正确的数据进行积雪天数统计。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { tempFilename == fileNames[i] ? bandNo : 1 });
                    rms.Add(rm);
                }
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    Int16[] nanValues = GetNanValues("CloudyValue");
                    Int16[] waterValues = GetNanValues("WaterValue");
                    Int16[] invailValues = GetNanValues("InvailValue");
                    Int16 currTimeValue = 0;
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        Int16[] timeValue = new Int16[dataLength];
                        List<Int16> tempValue = new List<Int16>();
                        for (int i = 0; i < dataLength; i++)
                            timeValue[i] = 0;
                        for (int index = 0; index < dataLength; index++)
                        {
                            foreach (RasterVirtualVistor<Int16> rvs in rvInVistor)
                            {
                                Int16[] dt = rvs.RasterBandsData[0];
                                if (dt == null)
                                    continue;
                                tempValue.Add(dt[index]);
                            }

                            if (tempValue.Count == 0)
                                continue;
                            if (TimeValue(tempValue.ToArray(), nanValues, invailValues, waterValues, out currTimeValue))
                                timeValue[index] = currTimeValue;
                            tempValue.Clear();
                        }
                        for (int index = 0; index < dataLength; index++)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = timeValue[index];
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                    FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                    array.Add(res);
                    res.SetDispaly(false);
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }

            _argumentProvider.SetArg("SelectedPrimaryFiles", new string[] { outFileName });
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return array;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            IExtractResult ress = ThemeGraphyByInstance(instance);
            if (ress != null)
                array.Add(ress as IFileExtractResult);
            return array;
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] rasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in rasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = rasterMaper[0].Raster.ResolutionX;
            float resY = rasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private RasterIdentify GetRasterIdentifyID(ref string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;
            rst.IsOutput2WorkspaceDir = true;
            fileNames = rst.SortFiles;
            return rst;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private bool TimeValue(Int16[] pixelValues, Int16[] cloudValues, Int16[] invaildValues, Int16[] waterValues, out Int16 currTimeValue)
        {
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            Int16 defWater = (Int16)_argumentProvider.GetArg("defWater");
            Int16 intervalDays = (Int16)_argumentProvider.GetArg("intervalDays");
            int pixelValueCount = pixelValues.Length;
            int beforeIndex = 1;
            int nextIndex = 1;
            currTimeValue = 0;
            for (int i = 0; i < pixelValueCount; i++)
            {
                if (IsNanValue(pixelValues[i], waterValues))
                {
                    currTimeValue = defWater;
                    return true;
                }
                beforeIndex = 1;
                nextIndex = 1;
                if (IsNanValue(pixelValues[i], cloudValues) || IsNanValue(pixelValues[i], invaildValues))
                {
                    while ((i - beforeIndex) >= 0 && (IsNanValue(pixelValues[i - beforeIndex], cloudValues) || IsNanValue(pixelValues[i - beforeIndex], invaildValues)))
                    {
                        beforeIndex++;
                    }
                    while ((i + nextIndex) < pixelValueCount && (IsNanValue(pixelValues[i + nextIndex], cloudValues) || IsNanValue(pixelValues[i + nextIndex], invaildValues)))
                    {
                        nextIndex++;
                    }
                    if (beforeIndex + nextIndex + 1 <= intervalDays)
                        currTimeValue++;
                }
                else
                    currTimeValue++;
            }
            if (currTimeValue == 0 && IsNanValue(pixelValues[0], cloudValues))
                currTimeValue = defCloudy;
            else if (currTimeValue == 0 && IsNanValue(pixelValues[0], invaildValues))
                currTimeValue = invaildValues[0];
            return true;
        }

        private bool IsNanValue(short pixelValue, short[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (short value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }

        private Int16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<short> values = new List<short>();
                    short value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }

        #region 合并云

        private string ProcessCloud(string srcFilename, int bandNo)
        {
            string cloudFile = GetClmFile(srcFilename);
            if (string.IsNullOrEmpty(cloudFile) || !File.Exists(cloudFile))
                return srcFilename;
            Int16 defCloudy = (Int16)_argumentProvider.GetArg("defCloudy");
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider snwPrd = GeoDataDriver.Open(srcFilename) as RasterDataProvider;
            IRasterDataProvider cloudPrd = GeoDataDriver.Open(cloudFile) as RasterDataProvider;
            try
            {
                RasterMaper snwRm = new RasterMaper(snwPrd, new int[] { bandNo });
                rms.Add(snwRm);

                RasterMaper cloudRm = new RasterMaper(cloudPrd, new int[] { GetCloudCHNO() });
                rms.Add(cloudRm);

                MifConfig mifConfig = new MifConfig();
                string outFileName = mifConfig.GetConfigValue("TEMP") + "\\" + Guid.NewGuid() + ".dat";
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
                {
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>();
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
                    return outFileName;
                }
            }
            finally
            {
                snwPrd.Dispose();
                cloudPrd.Dispose();
            }
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
                if (_argumentProvider.DataProvider != null)
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
