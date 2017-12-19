#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2013-10-17 11:13:11
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
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    /// <summary>
    /// 类名：SubProductRasterSNWSD
    /// 属性描述：
    /// 创建者：Administrator   创建日期：2013-10-17 11:13:11
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductRasterSNWSD : CmaMonitoringSubProduct
    {
        const int THICK_DRY_SNOW = 1;
        const int THICK_WET_SNOW = 2;
        const int THIN_DRY_SNOW = 3;
        const int THIN_WET_SNOW_OR_FOREST_SNOW = 4;
        const int VERY_THICK_WET_SNOW = 5;
        const int NO_SNOW = 6;
        const int WET_FACTOR = -35;
        const int WET_FACTOR1 = -5;
        const int NO_SCATTER = 5;

        private IContextMessage _contextMessage;

        public SubProductRasterSNWSD(SubProductDef subProductDef)
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
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorithmName == "SnowPrdAlgorithm")
            {
                return SnowPrdAlgorithm();
            }
            else if (algorithmName == "SnowDepthAlgorithm")
            {
                return SnowDepthAlgorithm();
            }
            return null;
        }

        #region 光学雪深计算算法

        private IExtractResult SnowDepthAlgorithm()
        {
            if (_argumentProvider.DataProvider == null)
                return null;
            string currentFile = _argumentProvider.DataProvider.fileName;
            string dblvFile = _argumentProvider.GetArg("SelectedPrimaryFiles").ToString();
            if (string.IsNullOrEmpty(dblvFile))
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int shortInfraredCH = TryGetBandNo(bandNameRaster, "ShortInfrared");
            int visibleCH = TryGetBandNo(bandNameRaster, "Visible");
            if (shortInfraredCH == -1 || visibleCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            //china_dem_500m.raw
            string systemDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\SNW\\SnowArgFile");
            string roughnessFile = Path.Combine(systemDir, "china_roughness_500m.dat");
            string demFile = Path.Combine(systemDir, "china_dem_500m.raw");
            if (!File.Exists(roughnessFile))
            {
                if (!File.Exists(demFile))
                {
                    PrintInfo("积雪深度所用参数文件不存在！");
                    return null;
                }
                //计算粗糙度文件；
                else
                    ComputeRoughnessFile(demFile, roughnessFile);
            }
            if (_argumentProvider.GetArg("DepthArgs") == null)
            {
                PrintInfo("积雪深度参数获取失败！");
                return null;
            }
            string[] depthArgs = _argumentProvider.GetArg("DepthArgs") as string[];
            if (depthArgs == null || depthArgs.Length < 9)
            {
                PrintInfo("积雪深度参数获取失败！");
                return null;
            }
            bool isCorrectAngle = (bool)_argumentProvider.GetArg("isCorrectAngle");
            string angleFile = depthArgs[0];
            if (!isCorrectAngle || string.IsNullOrEmpty(angleFile) || !File.Exists(angleFile))
                angleFile = null;
            //计算雪深
            IFileExtractResult depthResult = ComputeDepthRaster(currentFile, dblvFile, roughnessFile, angleFile, visibleCH, shortInfraredCH,depthArgs);
            return depthResult;
        }

        private IFileExtractResult ComputeDepthRaster(string currentRasterFile, string dblvFile, string roughnessFile, string angleFile, int visibleCH, int shortInfraredCH,string[] depthArgs)
        {
            float arg;
            float a0 = 0, a1 = 0, a2 = 0, a3 = 0, b0 = 0, b1 = 0, b2 = 0, b3 = 0;
            if (String2Float(depthArgs[1], out arg))
                a0 = arg;
            if (String2Float(depthArgs[2], out arg))
                a1 = arg;
            if (String2Float(depthArgs[3], out arg))
                a2 = arg;
            if (String2Float(depthArgs[4], out arg))
                a3 = arg;
            if (String2Float(depthArgs[5], out arg))
                b0 = arg;
            if (String2Float(depthArgs[6], out arg))
                b1 = arg;
            if (String2Float(depthArgs[7], out arg))
                b2 = arg;
            if (String2Float(depthArgs[8], out arg))
                b3 = arg;
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, float> rfr = null;
            bool isCorrectAngle = false;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider argRaster = GeoDataDriver.Open(roughnessFile) as IRasterDataProvider;
                RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                rms.Add(argRm);
                IRasterDataProvider inRaster = GeoDataDriver.Open(currentRasterFile) as IRasterDataProvider;
                RasterMaper fileIn = new RasterMaper(inRaster, new int[] { visibleCH, shortInfraredCH });
                rms.Add(fileIn);
                IRasterDataProvider dblvRaster = GeoDataDriver.Open(dblvFile) as IRasterDataProvider;
                RasterMaper dblvRm = new RasterMaper(dblvRaster, new int[] { 1 });
                rms.Add(dblvRm);
                if (!string.IsNullOrEmpty(angleFile))
                {
                    IRasterDataProvider angleRaster = GeoDataDriver.Open(angleFile) as IRasterDataProvider;
                    RasterMaper angleRm = new RasterMaper(angleRaster, new int[] { 1 });
                    rms.Add(angleRm);
                    isCorrectAngle = true;
                }
                //string outFileId = _argumentProvider.GetArg("OutFileIdentify").ToString();
                string depthFileName = GetFileName(new string[] { currentRasterFile }, _subProductDef.ProductDef.Identify, "0SSD", ".dat", null);
                outRaster = CreateOutRaster(depthFileName, enumDataType.Float, rms.ToArray(), inRaster.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[2].RasterBandsData[0] != null)
                    {
                        int length = rvInVistor[2].RasterBandsData[0].Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (rvInVistor[2].RasterBandsData[0][i] == 1)
                            {
                                short roughValue = rvInVistor[0].RasterBandsData[0][i];
                                if (roughValue == -9999)
                                    rvOutVistor[0].RasterBandsData[0][i] = -9999f;
                                else
                                {
                                    float value;
                                    float ref1 = rvInVistor[1].RasterBandsData[0][i] / 10f;
                                    float ref6 = rvInVistor[1].RasterBandsData[1][i] / 10f;

                                    if (isCorrectAngle)
                                    {
                                        float angle = rvInVistor[3].RasterBandsData[0][i] / 100f;
                                        ref1 = CorrectBandValue(angle, ref1);
                                        ref6 = CorrectBandValue(angle, ref6);
                                    }
                                    if (roughValue <= 2000)
                                    {
                                        value = a0 + a1 * ref1 + a2 * ref1 / ref6 + a3 * (ref1 - ref6);
                                        value = value <= 0 ? 0.1f : (value > 50 ? 50 : value);
                                        rvOutVistor[0].RasterBandsData[0][i] = value;
                                    }
                                    else
                                    {
                                        value = b0 + b1 * ref1 + b2 * ref1 / ref6 + b3 * (ref1 - ref6);
                                        value = value <= 0 ? 0.1f : (value > 50 ? 50 : value);
                                        rvOutVistor[0].RasterBandsData[0][i] = value;
                                    }
                                }
                            }
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, depthFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        //生成粗糙度文件，为使用RasterModel计算积雪深度，粗糙度扩大10倍并保存为Int16类型（相当于精度保留至十分位）
        private unsafe void ComputeRoughnessFile(string demFile, string roughnessFile)
        {
            IRasterDataProvider inRaster = null;
            IRasterDataProvider outRaster = null;
            try
            {
                inRaster = GeoDataDriver.Open(demFile) as IRasterDataProvider;
                outRaster = CreateRaster(roughnessFile, inRaster);
                //按行处理，每行处理时需要此行及上下两行的数据
                int width = inRaster.Width;
                Int16[] block0 = new Int16[width];
                Int16[] block1 = new Int16[width];
                Int16[] block2 = new Int16[width];
                Int16[] values = new Int16[width];
                //读第一行数据
                fixed (Int16* ptr1 = block1)
                {
                    fixed (Int16* ptr2 = block2)
                    {
                        fixed (Int16* ptrValue = values)
                        {
                            IntPtr bufferValue = new IntPtr(ptrValue);
                            IntPtr buffer1 = new IntPtr(ptr1);
                            IntPtr buffer2 = new IntPtr(ptr2);
                            inRaster.GetRasterBand(1).Read(0, 0, width, 1, buffer1, enumDataType.Int16, width, 1);
                            for (int i = 1; i <= (inRaster.Height - 1); i++)
                            {
                                inRaster.GetRasterBand(1).Read(0, i, width, 1, buffer2, enumDataType.Int16, width, 1);
                                if (i == 1)
                                {
                                    //第一个和最后一个单独处理
                                    if (block1[0] == -9999)
                                        values[0] = -9999;
                                    else
                                        values[0] = ComputeSdev(new Int16[] { block1[0], block1[1], block2[0], block2[1] });
                                    for (int j = 1; j < (width - 1); j++)
                                    {
                                        if (block1[j] == -9999)
                                            values[j] = -9999;
                                        else
                                            values[j] = ComputeSdev(new Int16[] { block1[j - 1], block1[j], block1[j + 1], block2[j - 1], block2[j], block2[j + 1] });
                                    }
                                    if (block1[width - 1] == -9999)
                                        values[width - 1] = -9999;
                                    else
                                        values[width - 1] = ComputeSdev(new Int16[] { block1[width - 2], block1[width - 1], block2[width - 2], block2[width - 1] });
                                }
                                else if (i == (inRaster.Height - 1))
                                {
                                    //第一个和最后一个单独处理
                                    if (block1[0] == -9999)
                                        values[0] = -9999;
                                    else
                                        values[0] = ComputeSdev(new Int16[] { block0[0], block0[1], block1[0], block1[1] });
                                    for (int j = 1; j < (width - 1); j++)
                                    {
                                        if (block1[j] == -9999)
                                            values[j] = -9999;
                                        else
                                            values[j] = ComputeSdev(new Int16[] { block1[j - 1], block1[j], block1[j + 1], block0[j - 1], block0[j], block0[j + 1] });
                                    }
                                    if (block1[width - 1] == -9999)
                                        values[width - 1] = -9999;
                                    else
                                        values[width - 1] = ComputeSdev(new Int16[] { block0[width - 2], block0[width - 1], block1[width - 2], block1[width - 1] });
                                }
                                //若i=1或i=inRaster.Height,只找其下一行或上一行进行计算
                                else
                                {
                                    //第一个和最后一个单独处理
                                    if (block1[0] == -9999)
                                        values[0] = -9999;
                                    else
                                        values[0] = ComputeSdev(new Int16[] { block0[0], block0[1], block1[0], block1[1], block2[0], block2[1] });
                                    for (int j = 1; j < (width - 1); j++)
                                    {
                                        if (block1[j] == -9999)
                                            values[j] = -9999;
                                        else
                                            values[j] = ComputeSdev(new Int16[] { block1[j - 1], block1[j], block1[j + 1],
                                        block0[j - 1], block0[j], block0[j + 1],
                                        block2[j-1], block2[j], block2[j+1] });
                                    }
                                    if (block1[width - 1] == -9999)
                                        values[width - 1] = -9999;
                                    else
                                        values[width - 1] = ComputeSdev(new Int16[] { block1[width - 1], block1[width - 2], block0[width - 1], 
                                    block0[width - 2], block2[width-1], block2[width-2] });
                                }
                                //写数据
                                outRaster.GetRasterBand(1).Write(0, i - 1, width, 1, bufferValue, enumDataType.Float, width, 1);
                                //交换数据值
                                for (int j = 0; j < width; j++)
                                {
                                    block0[j] = block1[j];
                                    block1[j] = block2[j];
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (inRaster != null)
                    inRaster.Dispose();
            }
        }

        //返回某位置的标准差*10四舍五入后整数值
        private short ComputeSdev(Int16[] values)
        {
            List<Int16> valueList = new List<short>();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == -9999)
                    continue;
                else
                    valueList.Add(values[i]);
            }
            if (valueList.Count < 1)
                return -9999;
            Int32 sum = 0;
            float avg = 0f;
            double sdev = 0f;
            for (int i = 0; i < valueList.Count; i++)
            {
                sum += valueList[i];
            }
            avg = sum / valueList.Count;
            //
            for (int i = 0; i < valueList.Count; i++)
            {
                sdev += Math.Pow((valueList[i] - avg), 2);
            }
            //标准差
            sdev = Math.Sqrt(sdev / valueList.Count);
            return (short)Math.Round(sdev * 10);
        }

        private double DegreeToRadian(float degrees)
        {
            return degrees * Math.PI / 180f;
        }

        //修正公式：
        //angle = single(90.00 - SolarZenith)'*pi/180.00;
        //TB_i=TB_i*cos(SolarZenith)/cos(angle*(1.0-1.3*sin(0.05*angle))); i = 1,2,3,4,6,7            
        private float CorrectBandValue(float solarZenith, float bandValue)
        {
            double zenithDegree = DegreeToRadian(solarZenith);
            double complementDegree = DegreeToRadian(90 - solarZenith);
            //angle*(1.0-1.3*sin(0.05*angle))
            return (float)(bandValue * Math.Cos(zenithDegree) / Math.Cos(complementDegree * (1 - 1.3 * Math.Sin(0.05 * complementDegree))));
        }

        private bool String2Float(string stringValue,out float value)
        {
            if (float.TryParse(stringValue, out value))
                return true;
            else
                return false;
        }

        #endregion

        #region 微波积雪参数计算算法

        private IExtractResult SnowPrdAlgorithm()
        {
            string inputFileName = _argumentProvider.DataProvider.fileName;
            if (string.IsNullOrEmpty(inputFileName) || !File.Exists(inputFileName))
                return null;
            int[] bandNos = CheckBandNos();
            if (bandNos == null || bandNos.Length < 10)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            double[] sdParas = GetSDPara();
            if (sdParas == null)
            {
                PrintInfo("雪深计算参数为空。");
                return null;
            }
            //查找参数数据
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\SNW\\SnowArgFile");
            string fileNameBare = Path.Combine(argFileDir, "china_bares.dat");
            string filenameGrass = Path.Combine(argFileDir, "china_grass.dat");
            string filenameForest = Path.Combine(argFileDir, "china_forest.dat");
            string filenameFarmhand = Path.Combine(argFileDir, "china_farmhand.dat");
            string filenameDensity = Path.Combine(argFileDir, "china_snow_density.dat");
            if (!File.Exists(filenameDensity))
                return null;
            //1、查找是否存在各土地类型百分比文件（4个），不存在则生成
            string bareFracFile = Path.ChangeExtension(fileNameBare, "_frac.dat");
            string grassFracFile = Path.ChangeExtension(filenameGrass, "_frac.dat");
            string forestFracFile = Path.ChangeExtension(filenameForest, "_frac.dat");
            string farmhandFracFile = Path.ChangeExtension(filenameFarmhand, "_frac.dat");
            string[] argFiles = new string[] { bareFracFile, grassFracFile, forestFracFile, farmhandFracFile };
            foreach (string file in argFiles)
            {
                string sumFile = null;
                if (!File.Exists(file))
                {
                    //计算
                    //1)生成SummaryFile（不存在）
                    sumFile = GetArgSummaryFile(fileNameBare, filenameGrass, filenameForest, filenameFarmhand);
                    //2)计算某个百分比文件
                    ComputeFracFile(sumFile, file);
                }
            }
            //2、计算雪深、雪水当量
            //雪深
            IExtractResultArray array = new ExtractResultArray("雪深当量");
            IFileExtractResult depthResult = ComputeSnowDepth(inputFileName, bandNos, argFiles, sdParas);
            //雪水当量
            if (!File.Exists(filenameDensity))
                return null;
            string depthFileName = (depthResult as FileExtractResult).FileName;
            if (!File.Exists(depthFileName))
                return null;
            IFileExtractResult sweResult = ComputeSnowSWE(filenameDensity, depthFileName);
            array.Add(depthResult);
            array.Add(sweResult);
            return array;
        }

        private double[] GetSDPara()
        {
            double firPara = (double)_argumentProvider.GetArg("firPara");
            if (firPara > -1.000 || firPara < -29.000)
                return null;
            double secPara = (double)_argumentProvider.GetArg("secPara");
            if (secPara > 1.729 || secPara < 0.065)
                return null;
            double thrPara = (double)_argumentProvider.GetArg("thrPara");
            if (thrPara > 4.085 || thrPara < 0.163)
                return null;
            double fouPara = (double)_argumentProvider.GetArg("fouPara");
            if (fouPara > 21.600 || fouPara < 0.864)
                return null;
            double fivPara = (double)_argumentProvider.GetArg("fivPara");
            if (fivPara > 2.530 || fivPara < 0.101)
                return null;
            double sixPara = (double)_argumentProvider.GetArg("sixPara");
            if (sixPara > 0.655 || sixPara < 0.026)
                return null;
            double sevPara = (double)_argumentProvider.GetArg("sevPara");
            if (sevPara > 0.915 || sevPara < 0.037)
                return null;
            double eigPara = (double)_argumentProvider.GetArg("eigPara");
            if (eigPara > 0.615 || eigPara < 0.025)
                return null;
            double ninPara = (double)_argumentProvider.GetArg("ninPara");
            if (ninPara > 15.740 || ninPara < 0.630)
                return null;
            double tenPara = (double)_argumentProvider.GetArg("tenPara");
            if (tenPara > 2.055 || tenPara < 0.082)
                return null;
            double elePara = (double)_argumentProvider.GetArg("elePara");
            if (elePara > 1.06 || elePara < 0.042)
                return null;
            double twePara = (double)_argumentProvider.GetArg("twePara");
            if (twePara > 53.83 || twePara < 2.153)
                return null;
            double thirteenPara = (double)_argumentProvider.GetArg("thirteenPara");
            if (thirteenPara > 2.105 || thirteenPara < 0.084)
                return null;
            double fourteenPara = (double)_argumentProvider.GetArg("fourteenPara");
            if (fourteenPara > 5.605 || fourteenPara < 0.224)
                return null;
            double fifteenPara = (double)_argumentProvider.GetArg("fifteenPara");
            if (fifteenPara > 3.365 || fifteenPara < 0.135)
                return null;
            return new double[] { firPara, secPara, thrPara, fouPara, fivPara, sixPara, sevPara, eigPara, ninPara, tenPara, elePara, twePara, thirteenPara, fourteenPara, fifteenPara };
        }

        private IFileExtractResult ComputeSnowSWE(string filenameDensity, string depthFileName)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<double, double> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(depthFileName) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                IRasterDataProvider inRaster2 = GeoDataDriver.Open(filenameDensity) as IRasterDataProvider;
                RasterMaper fileIn2 = new RasterMaper(inRaster2, new int[] { 1 });
                rms.Add(fileIn2);

                string sweFileName = GetFileName(new string[] { depthFileName }, _subProductDef.ProductDef.Identify, "MSWE", ".dat", null);
                outRaster = CreateOutRaster(sweFileName, enumDataType.Double, rms.ToArray(), inRaster2.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<double, double>();
                rfr.SetRaster(fileIns, fileOuts);

                rfr.RegisterCalcModel(new RasterCalcHandler<double, double>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        double[] swetmp = new double[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            swetmp[i] = rvInVistor[0].RasterBandsData[0][i] * rvInVistor[1].RasterBandsData[0][i] * 10;
                            if (swetmp[i] < 0)
                                swetmp[i] = 0;
                            rvOutVistor[0].RasterBandsData[0][i] = swetmp[i];
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sweFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        private int[] CheckBandNos()
        {
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int band10v = TryGetBandNo(bandNameRaster, "10v");
            if (band10v < 1 || band10v > 10)
                return null;
            int band10h = TryGetBandNo(bandNameRaster, "10h");
            if (band10h < 1 || band10h > 10)
                return null;
            int band18v = TryGetBandNo(bandNameRaster, "18v");
            if (band18v < 1 || band18v > 10)
                return null;
            int band18h = TryGetBandNo(bandNameRaster, "18h");
            if (band18h < 1 || band18h > 10)
                return null;
            int band23v = TryGetBandNo(bandNameRaster, "23v");
            if (band23v < 1 || band23v > 10)
                return null;
            int band23h = TryGetBandNo(bandNameRaster, "23h");
            if (band23h < 1 || band23h > 10)
                return null;
            int band36v = TryGetBandNo(bandNameRaster, "36v");
            if (band36v < 1 || band36v > 10)
                return null;
            int band36h = TryGetBandNo(bandNameRaster, "36h");
            if (band36h < 1 || band36h > 10)
                return null;
            int band89v = TryGetBandNo(bandNameRaster, "89v");
            if (band89v < 1 || band89v > 10)
                return null;
            int band89h = TryGetBandNo(bandNameRaster, "89h");
            if (band89h < 1 || band89h > 10)
                return null;
            return new int[] { band10v, band10h, band18v, band18h, band23v, band23h, band36v, band36h, band89v, band89h };
        }

        private IFileExtractResult ComputeSnowDepth(string inputFileName, int[] bandNos, string[] argFiles, double[] sdParas)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, double> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster = GeoDataDriver.Open(inputFileName) as IRasterDataProvider;
                RasterMaper fileIn = new RasterMaper(inRaster, bandNos);
                rms.Add(fileIn);
                foreach (string file in argFiles)
                {
                    IRasterDataProvider argRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                    rms.Add(argRm);
                }
                string depthFileName = GetFileName(new string[] { inputFileName }, _subProductDef.ProductDef.Identify, "MWSD", ".dat", null);
                outRaster = CreateOutRaster(depthFileName, enumDataType.Double, rms.ToArray(), inRaster.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, double>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, double>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null &&
                        rvInVistor[2].RasterBandsData[0] != null && rvInVistor[3].RasterBandsData[0] != null &&
                        rvInVistor[4].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        int[] type = new int[dataLength];
                        double[] sdtmp = new double[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            //type
                            type[i] = NO_SCATTER;
                            double ch10v = btValue(rvInVistor[0].RasterBandsData[0][i]);
                            double ch10h = btValue(rvInVistor[0].RasterBandsData[1][i]);
                            double ch18v = btValue(rvInVistor[0].RasterBandsData[2][i]);
                            double ch18h = btValue(rvInVistor[0].RasterBandsData[3][i]);
                            double ch23v = btValue(rvInVistor[0].RasterBandsData[4][i]);
                            double ch23h = btValue(rvInVistor[0].RasterBandsData[5][i]);
                            double ch36v = btValue(rvInVistor[0].RasterBandsData[6][i]);
                            double ch36h = btValue(rvInVistor[0].RasterBandsData[7][i]);
                            double ch89v = btValue(rvInVistor[0].RasterBandsData[8][i]);
                            double ch89h = btValue(rvInVistor[0].RasterBandsData[9][i]);
                            double si1 = ch23v - ch89v;
                            double si2 = ch18v - ch36v;
                            if (si1 >= 5 || si2 >= 5)
                            {
                                if (ch23v <= 260)
                                {
                                    if (ch18v - ch36v >= 20)
                                    {
                                        if (si1 - si2 >= WET_FACTOR)
                                            type[i] = THICK_DRY_SNOW;
                                        else
                                            type[i] = THICK_WET_SNOW;
                                    }
                                    else
                                    {
                                        if (si1 - si2 >= 8)
                                            type[i] = THIN_DRY_SNOW;
                                        else
                                        {
                                            if (si1 - si2 <= WET_FACTOR1)
                                                type[i] = VERY_THICK_WET_SNOW;
                                            else
                                            {
                                                if (ch18v - ch18h <= 6 && ch18v - ch36v >= 10)
                                                    type[i] = THIN_WET_SNOW_OR_FOREST_SNOW;
                                                else
                                                    type[i] = NO_SNOW;
                                            }
                                        }
                                    }
                                }
                                else
                                    type[i] = NO_SNOW;
                            }
                            else
                                type[i] = NO_SNOW;
                            //sdtmp
                            double sdFarmland = sdParas[0] + sdParas[1] * (ch18v - ch36h) + sdParas[2] * (ch89v - ch89h);
                            double sdGrass = sdParas[3] + sdParas[4] * (ch18h - ch36h) - sdParas[5] * (ch18v - ch18h) + sdParas[6] * (ch10v - ch89h) - sdParas[7] * (ch18v - ch89h);
                            double sdBaren = sdParas[8] + sdParas[9] * (ch36h - ch89h) - sdParas[10] * (ch10v - ch89v);
                            double sdForest = sdParas[11] + sdParas[12] * (ch18h - ch36v) - sdParas[13] * (ch18v - ch18h) + sdParas[14] * (ch89v - ch89h);
                            sdtmp[i] = (rvInVistor[1].RasterBandsData[0][i] * sdBaren +
                                rvInVistor[2].RasterBandsData[0][i] * sdGrass +
                                rvInVistor[3].RasterBandsData[0][i] * sdForest +
                                rvInVistor[4].RasterBandsData[0][i] * sdFarmland) / 10000;  //原地类百分比文件扩大了10000倍 
                            //设置输出数据值
                            if (type[i] == NO_SNOW || type[i] == THICK_WET_SNOW || type[i] == THIN_WET_SNOW_OR_FOREST_SNOW || type[i] == VERY_THICK_WET_SNOW)
                            {
                                sdtmp[i] = 0.0;
                            }
                            if (sdtmp[i] < 0)
                                sdtmp[i] = 0;
                            else
                                sdtmp[i] = sdtmp[i];
                            rvOutVistor[0].RasterBandsData[0][i] = sdtmp[i];
                        }
                    }
                    //输出
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, depthFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        private double btValue(Int16 chbt)
        {
            double ch;
            if (chbt == 0)
                ch = 0.0;
            else
                ch = chbt * 0.01 + 327.68;
            return ch;
        }

        private void ComputeFracFile(string sumFile, string file)
        {
            //"china_bares.dat"
            //"china_grass.dat";
            //"china_forest.dat";
            //"china_farmhand.dat"
            file = file.ToLower();
            string argFile = null;
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\SNW\\SnowArgFile");
            if (file.Contains("bares"))
                argFile = Path.Combine(argFileDir, "china_bares.dat");
            else
            {
                if (file.Contains("grass"))
                    argFile = Path.Combine(argFileDir, "china_grass.dat");
                else
                {
                    if (file.Contains("forest"))
                        argFile = Path.Combine(argFileDir, "china_forest.dat");
                    else
                    {
                        if (file.Contains("farmhand"))
                            argFile = Path.Combine(argFileDir, "china_farmhand.dat");
                        else
                            return;
                    }
                }
            }
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            try
            {
                IRasterDataProvider argPrd = GeoDataDriver.Open(argFile) as IRasterDataProvider;
                rms = new List<RasterMaper>();
                RasterMaper rmArg = new RasterMaper(argPrd, new int[] { 1 });
                rms.Add(rmArg);
                IRasterDataProvider sumPrd = GeoDataDriver.Open(sumFile) as IRasterDataProvider;
                RasterMaper rmSum = new RasterMaper(sumPrd, new int[] { 1 });
                rms.Add(rmSum);
                outRaster = CreateOutRaster(file, enumDataType.Int16, rms.ToArray(), argPrd.ResolutionX);
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                RasterProcessModel<double, Int16> rfr = new RasterProcessModel<double, Int16>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<double, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        for (int j = 0; j < dataLength; j++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][j] == 0)
                                rvOutVistor[0].RasterBandsData[0][j] = 0;
                            else
                                rvOutVistor[0].RasterBandsData[0][j] = (Int16)(rvInVistor[0].RasterBandsData[0][j] / rvInVistor[1].RasterBandsData[0][j] * 10000);
                        }
                    }
                }));
                rfr.Excute();
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        private string GetArgSummaryFile(string fileNameBare, string filenameGrass, string filenameForest, string filenameFarmhand)
        {
            string outFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\SNW\\SnowArgFile\\sum.dat");
            if (File.Exists(outFileName))
                return outFileName;
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<double, double> rfr = null;
            try
            {
                IRasterDataProvider barePrd = GeoDataDriver.Open(fileNameBare) as IRasterDataProvider;
                IRasterDataProvider grassPrd = GeoDataDriver.Open(filenameGrass) as IRasterDataProvider;
                IRasterDataProvider forestPrd = GeoDataDriver.Open(filenameForest) as IRasterDataProvider;
                IRasterDataProvider farmhandPrd = GeoDataDriver.Open(filenameFarmhand) as IRasterDataProvider;
                rms = new List<RasterMaper>();
                RasterMaper rmBare = new RasterMaper(barePrd, new int[] { 1 });
                rms.Add(rmBare);
                RasterMaper rmGrass = new RasterMaper(grassPrd, new int[] { 1 });
                rms.Add(rmGrass);
                RasterMaper rmForest = new RasterMaper(forestPrd, new int[] { 1 });
                rms.Add(rmForest);
                RasterMaper rmFarm = new RasterMaper(farmhandPrd, new int[] { 1 });
                rms.Add(rmFarm);
                outRaster = CreateOutRaster(outFileName, enumDataType.Double, rms.ToArray(), 0.1f);
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                rfr = new RasterProcessModel<double, double>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<double, double>((rvInVistor, rvOutVistor, aoi) =>
                {
                    for (int i = 0; i < rvInVistor.Length; i++)
                    {
                        if (rvInVistor[i].RasterBandsData != null)
                        {
                            int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                            for (int j = 0; j < dataLength; j++)
                            {
                                rvOutVistor[0].RasterBandsData[0][j] += rvInVistor[i].RasterBandsData[0][j];
                            }
                        }
                    }
                }));
                rfr.Excute();
                return outFileName;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }

        #endregion

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
        //创建输出删格文件
        private IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Intersect(inRaster.Raster.CoordEnvelope);
                    //outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private IRasterDataProvider CreateRaster(string outFileName, IRasterDataProvider inRaster)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = inRaster.CoordEnvelope.Clone();
            string mapInfo = outEnv.ToMapInfoString(new Size(inRaster.Width, inRaster.Height));
            RasterDataProvider outRaster = raster.Create(outFileName, inRaster.Width, inRaster.Height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

    }
}
