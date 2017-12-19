#region Version Info
/*========================================================================
* 功能概述：积雪雪深、雪水当量
* 
* 创建者：李喜佳    时间：2013-10-17 11:13:11
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
using System.Threading;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.VectorDrawing;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductRasterSNWSD
    /// 属性描述：积雪雪深、雪水当量算法
    /// 创建者：李喜佳  创建日期：2013-10-17 11:13:11
    /// 修改者：陈安             修改日期：2015年2月9日
    /// 修改描述：支持国外地区数据的雪深雪水当量反演
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
        private static Regex DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
       
        public SubProductRasterSNWSD(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "SnowPrdAlgorithm")
            {
                return SnowPrdAlgorithmNew(progressTracker);
            }
            else
                return null;
        }

        private IExtractResult SnowPrdAlgorithm(Action<int, string> progressTracker)
        {
            if (progressTracker != null)
            {
                progressTracker(1, "开始计算");
            }
            bool IsNative = true;//测试变量，稍后替换 国外数据处理入口
            string inputFileName = _argumentProvider.GetArg("RasterFile").ToString();

            string outclipfilename = inputFileName;
            SNWSDSettingPar snwsdpars = _argumentProvider.GetArg("Arguments") as SNWSDSettingPar;
            string clipoutdir = Path.GetDirectoryName(inputFileName);
            AOIContainerLayer aoiContainer = null;//
            // 伪代码
            //int [] aoiindex=默认中国区域AOIindexs;
            //for (int i = 0; i < datalength; i++)
            //{
            //    if (aoiindex.Contains(i))
            //    {
            //        //国外算法 像素点作为参数 传入处理子方法

            //    }
            //    else
            //    {
            //        //国内算法
            //    }
            //}
           

            if (snwsdpars.AoiContainer != null)
            {
                //将输入文件进行按区域裁切
                aoiContainer = snwsdpars.AoiContainer;

                MulRegionsClip muticlip = new MulRegionsClip();
                
                outclipfilename = muticlip.MutiRegionsClip(inputFileName, aoiContainer, clipoutdir);//区域限制 将裁切文件进行下文传递 
            }
            else
            {
                outclipfilename = inputFileName;
            }
           
           
            if (!IsNative)
            {
                //森林
                string argforestFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
                string argforestdensity = Path.Combine(argforestFileDir, "forestdensity.dat");//强度
                string argforestfraction = Path.Combine(argforestFileDir, "forestfraction.dat");//覆盖度
            }

            //解析文件时间，确定波段顺序
            Match m = DataReg.Match(Path.GetFileName(outclipfilename));
            string filedate = "";
            if (m.Success)
            {
                filedate = m.Value;
            }
            Int32 filedateDig = Convert.ToInt32(filedate);
            int[] bandNos = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            if (string.IsNullOrEmpty(outclipfilename) || !File.Exists(outclipfilename))
                return null;
            float[] sdParas = snwsdpars.AlgorithmPars;
            if (sdParas == null)
            {
                PrintInfo("雪深计算参数为空。");
                return null;
            }
            //查找参数数据
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
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
                    //1)生成SummaryFile（不存在） 此行代码跟循环没关系 无需做无用循环？by ca
                    sumFile = GetArgSummaryFile(fileNameBare, filenameGrass, filenameForest, filenameFarmhand);
                    //2)计算某个百分比文件
                    ComputeFracFile(sumFile, file);
                }
            }
            //2、计算雪深、雪水当量
            //雪深
            float outResloution = 0.1f;//(float)_argumentProvider.GetArg("Resolution");
            IExtractResultArray array = new ExtractResultArray("雪深当量");
            IFileExtractResult depthResult = ComputeSnowDepth(outclipfilename, argFiles, sdParas, outResloution, filedateDig);//设置雪深小于0的地区 不显示
            progressTracker(25, "计算完成25%");
            //雪水当量
            if (!File.Exists(filenameDensity))
                return null;
            string depthFileName = (depthResult as FileExtractResult).FileName;
            if (!File.Exists(depthFileName))
                return null;
            IFileExtractResult sweResult = ComputeSnowSWE(filenameDensity, depthFileName);
            progressTracker(50, "计算完成50%");
            array.Add(depthResult);
            array.Add(sweResult);
            //中值滤波
            Int16 smoothwindow = 5;
            string sdfilename = depthResult.FileName;
            IFileExtractResult midSDFilterResult = ComputerMid(sdfilename, smoothwindow);//滤波
            progressTracker(75, "计算完成75%");
            string swefilename = sweResult.FileName;
            IFileExtractResult midSWEFilterResult = ComputerMid(swefilename, smoothwindow);
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(midSDFilterResult.FileName) as IRasterDataProvider;
            if (inRaster1.Width >= 650)
            {
                array.Add(midSDFilterResult);
                array.Add(midSWEFilterResult);
            }
            if (inRaster1.Width < 650)
            {
                //临时条件，判断不是全国范围，就进行滤波后双线性插值
                if (inRaster1 != null)
                    inRaster1.Dispose();
                Int16 zoom = 10;
                string identify = "MBSD";
                IFileExtractResult sdBilinearfile = Bilinear(midSDFilterResult.FileName, identify, zoom);
                File.SetAttributes(midSDFilterResult.FileName, FileAttributes.Normal);
                File.Delete(midSDFilterResult.FileName);//删除原中值滤波的“MFSD”文件
                string sdhdrfile = midSDFilterResult.FileName.Substring(0, midSDFilterResult.FileName.Length - 4) + ".hdr";
                File.Delete(sdhdrfile);
                IFileExtractResult sdReName = ReName(sdBilinearfile.FileName, "MBSD", "MFSD");//将插值的“MBSD”变成“MFSD”；
                array.Add(sdReName);
                identify = "MBWE";
                IFileExtractResult sweBilinearfile = Bilinear(midSWEFilterResult.FileName, identify, zoom);
                File.SetAttributes(midSWEFilterResult.FileName, FileAttributes.Normal);
                File.Delete(midSWEFilterResult.FileName);//删除原中值滤波的“MFWE”文件
                string swehdrfile = midSWEFilterResult.FileName.Substring(0, midSWEFilterResult.FileName.Length - 4) + ".hdr";
                File.Delete(swehdrfile);
                IFileExtractResult sweReName = ReName(sweBilinearfile.FileName, "MBWE", "MFWE");//将插值的“MBWE”变成“MFWE”；
                array.Add(sweReName);
            }
            if (progressTracker != null)
                progressTracker(100, "计算完成");
            return array;
        }
        /// <summary>
        /// 国内外数据统一判识
        /// 先进行国外数据判识，然后进行国内数据判识，最终合成一张
        /// </summary>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        private ExtractResultArray SnowPrdAlgorithmNew(Action<int, string> progressTracker)
        {
            if (progressTracker != null)
            {
                progressTracker(1, "开始计算");
            }
            string inputFileName = _argumentProvider.GetArg("RasterFile").ToString();
            string outclipfilename = inputFileName;
            SNWSDSettingPar snwsdpars = _argumentProvider.GetArg("Arguments") as SNWSDSettingPar;
            string clipoutdir = Path.GetDirectoryName(inputFileName);
            //1.根据选择区域截取文件
            AOIContainerLayer aoiContainer = null;//
            if (snwsdpars.AoiContainer != null)
            {
                //将输入文件进行按区域裁切
                aoiContainer = snwsdpars.AoiContainer;
                MulRegionsClip muticlip = new MulRegionsClip();
                outclipfilename = muticlip.MutiRegionsClip(inputFileName, aoiContainer, clipoutdir);//区域限制 将裁切文件进行下文传递 
            }
            else
            {
                outclipfilename = inputFileName;
            }
             //森林
            string argforestFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
            string argforestdensity = Path.Combine(argforestFileDir, "north_forestdensity.dat");//强度
            string argforestfraction = Path.Combine(argforestFileDir, "north_forestfraction.dat");//覆盖度
            string []argFiles= {argforestdensity,argforestfraction};
            float outResloution = 0.1f;//(float)_argumentProvider.GetArg("Resolution");此处可以通过设置进行
             //解析文件时间，确定波段顺序
            Match m = DataReg.Match(Path.GetFileName(outclipfilename));
            string filedate = "";
            if (m.Success)
            {
                filedate = m.Value;
            }
            int filedateDig = Convert.ToInt32(filedate);
           //截取之后的数据生成的外国反演
           List<string> result= ComputeSnowDepthAndSWE(outclipfilename, argFiles, outResloution, filedateDig);//国外雪深雪水处理
           //生成国内数据
           string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
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
           string[] argFileschina = new string[] { bareFracFile, grassFracFile, forestFracFile, farmhandFracFile };
           foreach (string file in argFileschina)
           {
               string sumFile = null;
               if (!File.Exists(file))
               {
                   //1)生成SummaryFile（不存在） 此行代码跟循环没关系 无需做无用循环？by ca
                   sumFile = GetArgSummaryFile(fileNameBare, filenameGrass, filenameForest, filenameFarmhand);
                   //2)计算某个百分比文件
                   ComputeFracFile(sumFile, file);
               }
           }
           //雪深          
           string depthResult = ComputeSnowDepthNew(outclipfilename, argFileschina, snwsdpars.AlgorithmPars, outResloution, filedateDig);//设置雪深小于0的地区 不显示
           //雪水当量
           if (!File.Exists(filenameDensity))
               return null;
           
           if (!File.Exists(depthResult))
               return null;
           string sweResult = ComputeSnowSWENew(filenameDensity, depthResult);
           progressTracker(25, "计算完成25%");
           List<string> inputfiles = new List<string>();
           inputfiles.AddRange(result);
           inputfiles.Add(depthResult);
           inputfiles.Add(sweResult);
           ExtractResultArray array=GetWholeSDAndSWE(inputfiles);
           progressTracker(50, "计算完成50%");
           //中值滤波
           Int16 smoothwindow = 5;
           string sdfilename = ((array as ExtractResultArray).PixelMappers[0] as IFileExtractResult).FileName;
           IFileExtractResult midSDFilterResult = ComputerMid(sdfilename, smoothwindow);//滤波
           progressTracker(75, "计算完成75%");
           string swefilename = ((array as ExtractResultArray).PixelMappers[1] as IFileExtractResult).FileName; ;
           IFileExtractResult midSWEFilterResult = ComputerMid(swefilename, smoothwindow);
           IRasterDataProvider inRaster1 = GeoDataDriver.Open(midSDFilterResult.FileName) as IRasterDataProvider;
           if (inRaster1.Width >= 650)
           {
               array.Add(midSDFilterResult);
               array.Add(midSWEFilterResult);
           }
           if (inRaster1.Width < 650)
           {
               //临时条件，判断不是全国范围，就进行滤波后双线性插值
               if (inRaster1 != null)
                   inRaster1.Dispose();
               Int16 zoom = 10;
               string identify = "MBSD";
               IFileExtractResult sdBilinearfile = Bilinear(midSDFilterResult.FileName, identify, zoom);
               File.SetAttributes(midSDFilterResult.FileName, FileAttributes.Normal);
               File.Delete(midSDFilterResult.FileName);//删除原中值滤波的“MFSD”文件
               string sdhdrfile = midSDFilterResult.FileName.Substring(0, midSDFilterResult.FileName.Length - 4) + ".hdr";
               File.Delete(sdhdrfile);
               IFileExtractResult sdReName = ReName(sdBilinearfile.FileName, "MBSD", "MFSD");//将插值的“MBSD”变成“MFSD”；
               array.Add(sdReName);
               identify = "MBWE";
               IFileExtractResult sweBilinearfile = Bilinear(midSWEFilterResult.FileName, identify, zoom);
               File.SetAttributes(midSWEFilterResult.FileName, FileAttributes.Normal);
               File.Delete(midSWEFilterResult.FileName);//删除原中值滤波的“MFWE”文件
               string swehdrfile = midSWEFilterResult.FileName.Substring(0, midSWEFilterResult.FileName.Length - 4) + ".hdr";
               File.Delete(swehdrfile);
               IFileExtractResult sweReName = ReName(sweBilinearfile.FileName, "MBWE", "MFWE");//将插值的“MBWE”变成“MFWE”；
               array.Add(sweReName);
           }
           if (progressTracker != null)
               progressTracker(100, "计算完成");
           return array;
        }
        /// <summary>
        /// 计算国外区域的雪深和雪水当量
        /// </summary>
        /// <param name="localfile">局地文件</param>
        /// <param name="argFiles">输入计算文件信息（森林）0：森林覆盖度fd 1：森林强度</param>
        /// <param name="outResolution">输出分辨率</param>
        /// <param name="filedateDig">判断波段顺序条件（根据时间）</param>
        /// <returns>雪深和雪水当量 ExtractResult</returns>
        private List<string> ComputeSnowDepthAndSWE(string localfile, string[] argFiles, float outResolution, int filedateDig)
        {
            //输入输出模型
            List<RasterMaper> rms = null;
            //输出raster
            IRasterDataProvider outSDRaster = null;
            IRasterDataProvider outSWERaster = null;
            RasterProcessModel<Int16, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster = GeoDataDriver.Open(localfile) as IRasterDataProvider;
                List<int> bandnos = new List<int>();
                for (int i = 1; i <= inRaster.BandCount; i++)
                {
                    bandnos.Add(i);
                }
                RasterMaper fileIn = new RasterMaper(inRaster, bandnos.ToArray());
                rms.Add(fileIn);
                foreach (string file in argFiles)
                {
                    IRasterDataProvider argRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                    rms.Add(argRm);
                }
                string depthSDFileName = GetFileName(new string[] { localfile }, _subProductDef.ProductDef.Identify, "MWSD_foreign", ".dat", null);
                string depthSWEFileName = GetFileName(new string[] { localfile }, _subProductDef.ProductDef.Identify, "MSWE_foreign", ".dat", null);
                outSDRaster = CreateOutRaster(depthSDFileName, enumDataType.Float, rms.ToArray(), outResolution);
                outSWERaster = CreateOutRaster(depthSWEFileName, enumDataType.Float, rms.ToArray(), outResolution);
                RasterMaper fileSDOut = new RasterMaper(outSDRaster, new int[] { 1 });
                RasterMaper fileSWEOut = new RasterMaper(outSWERaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileSDOut, fileSWEOut };
                rfr = new RasterProcessModel<Int16, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                    int[] type = new int[dataLength];
                    float[] sdtmp = new float[dataLength];//输出文件sd和swe大小是否一致？和中国区域使用的是同一变量 文件来自于china_snow_density.dat
                    for (int i = 0; i < dataLength; i++)
                    {

                        float ch10v = 0f; float ch10h = 0f; float ch18v = 0f; float ch18h = 0f; float ch23v = 0f;
                        float ch23h = 0f; float ch36v = 0f; float ch36h = 0f; float ch89v = 0f; float ch89h = 0f;
                        //森林覆盖度  森林强度
                        float ff = 0f; float fd = 0f;
                        float snowdensity_piexl = 0f;//这个值是从雪强度而来 是否是积雪判识的结果
                        fd = btValue(rvInVistor[1].RasterBandsData[0][i]);
                        ff = btValue(rvInVistor[2].RasterBandsData[0][i]);
                        if (filedateDig >= 20110412)
                        {
                            ch10v = btValue(rvInVistor[0].RasterBandsData[0][i]);
                            ch10h = btValue(rvInVistor[0].RasterBandsData[1][i]);
                            ch18v = btValue(rvInVistor[0].RasterBandsData[2][i]);
                            ch18h = btValue(rvInVistor[0].RasterBandsData[3][i]);
                            ch23v = btValue(rvInVistor[0].RasterBandsData[4][i]);
                            ch23h = btValue(rvInVistor[0].RasterBandsData[5][i]);
                            ch36v = btValue(rvInVistor[0].RasterBandsData[6][i]);
                            ch36h = btValue(rvInVistor[0].RasterBandsData[7][i]);
                            ch89v = btValue(rvInVistor[0].RasterBandsData[8][i]);
                            ch89h = btValue(rvInVistor[0].RasterBandsData[9][i]);
                        }
                        else
                        {
                            ch10v = btValue(rvInVistor[0].RasterBandsData[9][i]);
                            ch10h = btValue(rvInVistor[0].RasterBandsData[8][i]);
                            ch18v = btValue(rvInVistor[0].RasterBandsData[7][i]);
                            ch18h = btValue(rvInVistor[0].RasterBandsData[6][i]);
                            ch23v = btValue(rvInVistor[0].RasterBandsData[5][i]);
                            ch23h = btValue(rvInVistor[0].RasterBandsData[4][i]);
                            ch36v = btValue(rvInVistor[0].RasterBandsData[3][i]);
                            ch36h = btValue(rvInVistor[0].RasterBandsData[2][i]);
                            ch89v = btValue(rvInVistor[0].RasterBandsData[1][i]);
                            ch89h = btValue(rvInVistor[0].RasterBandsData[0][i]);
                        }
                        if (ch10v == 0)
                        {
                            rvOutVistor[0].RasterBandsData[0][i] = -999.0f;//雪深
                            rvOutVistor[1].RasterBandsData[0][i] = -999.0f;//雪水当量
                            continue;
                        }
                        float po118 = (ch18v - ch18h) < 3.0f ? 3.0f : (ch18v - ch18h);
                        float po110 = (ch10v - ch10h) < 3.0f ? 3.0f : (ch10v - ch10h);
                        float po136 = (ch36v - ch36h) < 3.0f ? 3.0f : (ch36v - ch36h);
                        float t1036v = ch10v - ch36v;
                        float t1036h = ch10h - ch36h;
                        float t18v36h = ch18v - ch36h;
                        float t1836v = ch18v - ch36v;
                        float t1018v = ch10v - ch18v;
                        float t1836h = ch18h - ch36h;
                        float t1018h = ch10h - ch18h;
                        float t2389v = ch23v - ch89v;
                        float t2389h = ch23h - ch89h;
                        float scl = fd * 0.6f;
                        double klvn = 58.08 - 0.39 * ch18v + 1.21 * ch23v - 0.37 * ch36h + 0.36 * ch89v;
                        double invlogpol10 = 1 / Math.Log10((double)po110);
                        double invlogpol18 = 1.0 / Math.Log10((double)po118);
                        double invlogpol36 = 1.0 / Math.Log10((double)po136);
                        if ((ch36v < 255) && (ch36h < 245) && (t1036v > 0))
                        {
                            sdtmp[i] = (float)((ff * (invlogpol36 * t1836v) / (1 - scl)) + ((1 - ff) * ((invlogpol36 * t1036v) + (invlogpol18 * t1018v))));
                        }
                        if ((ch36v < 255) && (ch36h < 245) && (t1036h > 0))
                        {
                            sdtmp[i] = (float)((ff * (invlogpol36 * t1836h) / (1 - scl)) + ((1 - ff) * ((invlogpol36 * t1036h) + (invlogpol18 * t1018h))));
                        }
                        if ((ch89v < 255) && (ch89h < 255) && (t2389v > 0) && (t2389h > 0) && (klvn < 267))
                        {
                            sdtmp[i] = 5.0f;
                        }
                        if (sdtmp[i] < 5f && sdtmp[i] > 0f)
                        {
                            sdtmp[i] = 5.0f;
                        }
                        rvOutVistor[0].RasterBandsData[0][i] = sdtmp[i];
                        if (snowdensity_piexl == 0f)
                        {
                            snowdensity_piexl = 0.242666f;
                        }
                        rvOutVistor[0].RasterBandsData[0][i] = sdtmp[i];//雪深
                        rvOutVistor[1].RasterBandsData[0][i] = (float)(snowdensity_piexl * sdtmp[i] * 10 / 2.0);//雪水当量
                    }
                }));
                rfr.Excute();
                List<string> resultfiles = new List<string>();
                resultfiles.Add(depthSDFileName);
                resultfiles.Add(depthSWEFileName);

                return resultfiles;
            }
            catch (Exception ex)
            {
                //生成国外数据出错
                return null;
            }
            finally
            {
                //释放文件占用
                if (outSDRaster != null)
                    outSDRaster.Dispose();
                if (outSWERaster != null)
                    outSWERaster.Dispose();
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
        /// <summary>
        /// 合成国内外雪深雪水当量
        /// </summary>
        /// <param name="intputfiles"></param>
        /// <returns></returns>
        private ExtractResultArray GetWholeSDAndSWE(List<string> inputfiles)
        {

            List<RasterMaper> rms = null;
            IRasterDataProvider outSDRaster = null;
            IRasterDataProvider outSWERaster=null;
            float Resolution = 0.1f;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();

                for (int i = 0; i < inputfiles.Count; i++)
                {
                    IRasterDataProvider inRaster = GeoDataDriver.Open(inputfiles[i]) as IRasterDataProvider;

                    RasterMaper fileIn = new RasterMaper(inRaster, new int[] { 1 });//需要合成镶嵌的数据是单波段
                    rms.Add(fileIn);
                }
                string SDFilename = inputfiles[0].Replace("_foreign", "");
                string SWEFilename = inputfiles[1].Replace("_foreign", "");
                outSDRaster = CreateOutWholeRaster(SDFilename, enumDataType.Float, rms.ToArray(), Resolution);//这里取相交区域是否合适
                outSWERaster = CreateOutWholeRaster(SWEFilename, enumDataType.Float, rms.ToArray(), Resolution);
                RasterMaper SDfileOut = new RasterMaper(outSDRaster, new int[] { 1 });
                RasterMaper SWEfileOut = new RasterMaper(outSWERaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { SDfileOut, SWEfileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);//输入输出准备

                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                        if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                        {
                            //根据国内数据修改国外数据位置
                            int dataLength = rvOutVistor[0].RasterBandsData[0].Length;
                            for (int i = 0; i < dataLength; i++)
                            {
                                if (rvInVistor[2].RasterBandsData[0] != null)
                                {
                                    if (rvInVistor[2].RasterBandsData[0][i] == -999.0f)//
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        rvOutVistor[1].RasterBandsData[0][i] = rvInVistor[1].RasterBandsData[0][i];
                                    }
                                    else
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[2].RasterBandsData[0][i];
                                        rvOutVistor[1].RasterBandsData[0][i] = rvInVistor[3].RasterBandsData[0][i];
                                    }
                                }
                                else
                                {
                                    rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                    rvOutVistor[1].RasterBandsData[0][i] = rvInVistor[1].RasterBandsData[0][i];
                                }
                            }

                        }
                    
                    
                }));
                rfr.Excute(-999.0f);
                ExtractResultArray array = new ExtractResultArray("SDSWE");
                IFileExtractResult ressd = new FileExtractResult(_subProductDef.Identify, SDFilename, true);
                ressd.SetDispaly(false);
                array.Add(ressd);
                IFileExtractResult resswe = new FileExtractResult(_subProductDef.Identify, SWEFilename, true);
                resswe.SetDispaly(false);
                array.Add(resswe);
                return array;
            }
            catch (Exception ex)
            {
                PrintInfo(ex.StackTrace);
                return null;
            }
            finally
            {
                if (outSDRaster != null)//缺少删除临时文件
                    outSDRaster.Dispose();
                if (outSWERaster != null)
                    outSWERaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
                //if (inputfiles.Count > 0)//清除临时生成的国内外数据
                //{
                //    foreach (string file in inputfiles)
                //    {
                //        if (File.Exists(file))
                //        {
                //            File.Delete(file);
                //        }
                //    }
                //}
            }
        }
        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="oldIdentify"></param>
        /// <param name="newIdentify"></param>
        /// <returns></returns>
        private IFileExtractResult ReName(string filename,string oldIdentify,string newIdentify)
        {
            string fileReName = "";
            string hdrfile = "";
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                fi.MoveTo(filename.Replace(oldIdentify, newIdentify));
                fileReName = fi.FullName;
                hdrfile = filename.Substring(0, filename.Length - 4) + ".hdr";
                FileInfo fihdr = new FileInfo(hdrfile);
                fihdr.MoveTo(hdrfile.Replace(oldIdentify, newIdentify));
            }
            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileReName, true);
            return res;
        }
        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private IFileExtractResult ComputerMid(string filename, Int16 smoothwindow)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            //float outResloution = 0.01f;
            string identyString = "";
            if (filename.Contains("MWSD"))
            {
                identyString = "MFSD";// 雪深平滑文件标识
            }
            if (filename.Contains("MSWE"))
            {
                identyString = "MFWE"; //雪水当量平滑文件标识
            }
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(filename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                string middelFilterFileName = GetFileName(new string[] { filename }, _subProductDef.ProductDef.Identify, identyString, ".dat", null);
                outRaster = CreateOutRaster(middelFilterFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                //outRaster = CreateOutRaster(middelFilterFileName, enumDataType.Float, rms.ToArray(), outResloution);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        //int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        float[] outpixel = new float[dataLength];
                        float[] temp = new float[smoothwindow * smoothwindow];
                        int col = rvInVistor[0].SizeX;
                        if (smoothwindow == 5)
                        {
                            for (int i = 0; i < dataLength; i++)
                            {
                                if (i < 2 * col || i % col == 0 || (i - 1) % col == 0 || (i + 1) % col == 0 || (i + 2) % col == 0 || i > dataLength - 2 * col)
                                {
                                    //rvOutVistor[0].RasterBandsData[0][i] = 0.0f;
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 2];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 1];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - 2 * col];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 2];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i - col - 2];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[9] = rvInVistor[0].RasterBandsData[0][i - col + 2];
                                        temp[10] = rvInVistor[0].RasterBandsData[0][i - 2];
                                        temp[11] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[12] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[13] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[14] = rvInVistor[0].RasterBandsData[0][i + 2];
                                        temp[15] = rvInVistor[0].RasterBandsData[0][i + col - 2];
                                        temp[16] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[17] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[18] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        temp[19] = rvInVistor[0].RasterBandsData[0][i + col + 2];
                                        temp[20] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 2];
                                        temp[21] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 1];
                                        temp[22] = rvInVistor[0].RasterBandsData[0][i + 2 * col];
                                        temp[23] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 1];
                                        temp[24] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 2];
                                        int count = 0;
                                        for (int n = 0; n < 25; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 12)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dataLength; i++)
                            {

                                if (i < col || i % col == 0 || (i + 1) % col == 0 || i > dataLength - col)
                                {
                                    rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        int count = 0;
                                        for (int n = 0; n < 9; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 4)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, middelFilterFileName, true);
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

        private IFileExtractResult ComputeSnowSWE(string filenameDensity, string depthFileName)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
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
                outRaster = CreateOutRaster(sweFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);

                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        //int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        float[] swetmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][i] == 0)
                            {
                                rvInVistor[1].RasterBandsData[0][i] = 0.242666f;
                            }
                            if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                            {
                                swetmp[i] = -999.0f;
                            }
                            else
                            {
                                swetmp[i] = rvInVistor[0].RasterBandsData[0][i] * rvInVistor[1].RasterBandsData[0][i] * 10;
                                if (swetmp[i] < 0.0f)
                                    swetmp[i] = 0.0f;
                            }
                            if (swetmp[i] < 5.0f && swetmp[i] > 0.00f) //20140523 将小于5值改为5
                                swetmp[i] = 5.0f;
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
        /// <summary>
        /// 生成国内雪水当量判识结果
        /// </summary>
        /// <param name="filenameDensity"></param>
        /// <param name="depthFileName"></param>
        /// <returns></returns>
        private string ComputeSnowSWENew(string filenameDensity, string depthFileName)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(depthFileName) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                IRasterDataProvider inRaster2 = GeoDataDriver.Open(filenameDensity) as IRasterDataProvider;
                RasterMaper fileIn2 = new RasterMaper(inRaster2, new int[] { 1 });
                rms.Add(fileIn2);

                string sweFileName = GetFileName(new string[] { depthFileName }, _subProductDef.ProductDef.Identify, "MSWE_Native", ".dat", null);
                outRaster = CreateOutRaster(sweFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);

                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        //int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        float[] swetmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][i] == 0)
                            {
                                rvInVistor[1].RasterBandsData[0][i] = 0.242666f;
                            }
                            if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                            {
                                swetmp[i] = -999.0f;
                            }
                            else
                            {
                                swetmp[i] = rvInVistor[0].RasterBandsData[0][i] * rvInVistor[1].RasterBandsData[0][i] * 10;
                                if (swetmp[i] < 0.0f)
                                    swetmp[i] = 0.0f;
                            }
                            if (swetmp[i] < 5.0f && swetmp[i] > 0.00f) //20140523 将小于5值改为5
                                swetmp[i] = 5.0f;
                            rvOutVistor[0].RasterBandsData[0][i] = swetmp[i];
                        }
                    }
                }));
                rfr.Excute();
                return sweFileName;
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

        private float btValue(Int16 chbt)
        {
            float ch;
            //FY3B MWRI 轨道数据无效值是-999, FY3C MWRI 轨道数据无效值是29999.
            if (chbt == 0||chbt == -999|| chbt == 29999 )
                ch = 0.0f;
            else
                ch = chbt * 0.01f + 327.68f;
            return ch;
        }
        
        private IFileExtractResult ComputeSnowDepth(string outclipfilename, string[] argFiles, float[] sdParas, float outResolution, int filedateDig)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster = GeoDataDriver.Open(outclipfilename) as IRasterDataProvider;
                List<int> bandnums=new List<int>();

                for (int i = 1; i <= inRaster.BandCount; i++)
                {
                    bandnums.Add(i);
                }
                RasterMaper fileIn = new RasterMaper(inRaster, bandnums.ToArray());
                rms.Add(fileIn);
                foreach (string file in argFiles)
                {
                    IRasterDataProvider argRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                    rms.Add(argRm);
                }
                string depthFileName = GetFileName(new string[] { outclipfilename }, _subProductDef.ProductDef.Identify, "MWSD", ".dat", null);
                outRaster = CreateOutRaster(depthFileName, enumDataType.Float, rms.ToArray(), outResolution);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null &&
                        rvInVistor[2].RasterBandsData[0] != null && rvInVistor[3].RasterBandsData[0] != null &&
                        rvInVistor[4].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        int[] type = new int[dataLength];
                        float[] sdtmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            //type
                            //type[i] = NO_SCATTER;
                            float ch10v = 0f; float ch10h = 0f; float ch18v = 0f;float ch18h = 0f;float ch23v = 0f;
                            float ch23h = 0f; float ch36v = 0f; float ch36h = 0f;float ch89v = 0f;float ch89h = 0f;
                            if (filedateDig >= 20110412)
                            {
                                ch10v = btValue(rvInVistor[0].RasterBandsData[0][i]);
                                ch10h = btValue(rvInVistor[0].RasterBandsData[1][i]);
                                ch18v = btValue(rvInVistor[0].RasterBandsData[2][i]);
                                ch18h = btValue(rvInVistor[0].RasterBandsData[3][i]);
                                ch23v = btValue(rvInVistor[0].RasterBandsData[4][i]);
                                ch23h = btValue(rvInVistor[0].RasterBandsData[5][i]);
                                ch36v = btValue(rvInVistor[0].RasterBandsData[6][i]);
                                ch36h = btValue(rvInVistor[0].RasterBandsData[7][i]);
                                ch89v = btValue(rvInVistor[0].RasterBandsData[8][i]);
                                ch89h = btValue(rvInVistor[0].RasterBandsData[9][i]);
                            }
                            else
                            {
                                ch10v = btValue(rvInVistor[0].RasterBandsData[9][i]);
                                ch10h = btValue(rvInVistor[0].RasterBandsData[8][i]);
                                ch18v = btValue(rvInVistor[0].RasterBandsData[7][i]);
                                ch18h = btValue(rvInVistor[0].RasterBandsData[6][i]);
                                ch23v = btValue(rvInVistor[0].RasterBandsData[5][i]);
                                ch23h = btValue(rvInVistor[0].RasterBandsData[4][i]);
                                ch36v = btValue(rvInVistor[0].RasterBandsData[3][i]);
                                ch36h = btValue(rvInVistor[0].RasterBandsData[2][i]);
                                ch89v = btValue(rvInVistor[0].RasterBandsData[1][i]);
                                ch89h = btValue(rvInVistor[0].RasterBandsData[0][i]);
                            }
                            float si1 = ch23v - ch89v;
                            float si2 = ch18v - ch36v;
                            if (ch10v == 0.0f)   //数据未覆盖区或无效值区
                            {
                                sdtmp[i] = -999.0f;
                            }
                            else
                            {
                                if (si1 >= sdParas[15] || si2 >= sdParas[16])
                                {
                                    if (ch23v <= sdParas[17])
                                    {
                                        if (ch18v - ch36v >= sdParas[18])
                                        {
                                            if (si1 - si2 >= sdParas[19])
                                                type[i] = THICK_DRY_SNOW;
                                            else
                                                type[i] = THICK_WET_SNOW;
                                        }
                                        else
                                        {
                                            if (si1 - si2 >= sdParas[20])
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
                                float sdFarmland = sdParas[0] + sdParas[1] * (ch18v - ch36h) + sdParas[2] * (ch89v - ch89h);
                                float sdGrass = sdParas[3] + sdParas[4] * (ch18h - ch36h) - sdParas[5] * (ch18v - ch18h) + sdParas[6] * (ch10v - ch89h) - sdParas[7] * (ch18v - ch89h);
                                float sdBaren = sdParas[8] + sdParas[9] * (ch36h - ch89h) - sdParas[10] * (ch10v - ch89v);
                                float sdForest = sdParas[11] + sdParas[12] * (ch18h - ch36v) - sdParas[13] * (ch18v - ch18h) + sdParas[14] * (ch89v - ch89h);
                                sdtmp[i] = (rvInVistor[1].RasterBandsData[0][i] * sdBaren +
                                    rvInVistor[2].RasterBandsData[0][i] * sdGrass +
                                    rvInVistor[3].RasterBandsData[0][i] * sdForest +
                                    rvInVistor[4].RasterBandsData[0][i] * sdFarmland) / 10000;  //原地类百分比文件扩大了10000倍 
                                //设置输出数据值
                                if (type[i] == NO_SNOW || type[i] == THICK_WET_SNOW || type[i] == THIN_WET_SNOW_OR_FOREST_SNOW || type[i] == VERY_THICK_WET_SNOW)
                                {
                                    sdtmp[i] = 0.0f;
                                }
                                if (sdtmp[i] < 5.0f && sdtmp[i] > 0.00001f) //20140523 将小于5值改为5
                                    sdtmp[i] = 5.0f;
                                if (sdtmp[i] < 0.0000001f)
                                    sdtmp[i] = 0.0f;
                            }
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

        private string ComputeSnowDepthNew(string outclipfilename, string[] argFiles, float[] sdParas, float outResolution, int filedateDig)
        {
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
            string chinarasterfile = Path.Combine(argFileDir, "china_raster.dat");
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster = GeoDataDriver.Open(outclipfilename) as IRasterDataProvider;
                List<int> bandnums = new List<int>();

                for (int i = 1; i <= inRaster.BandCount; i++)
                {
                    bandnums.Add(i);
                }
                RasterMaper fileIn = new RasterMaper(inRaster, bandnums.ToArray());
                rms.Add(fileIn);
                foreach (string file in argFiles)
                {
                    IRasterDataProvider argRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                    rms.Add(argRm);
                }
                //中国区域raster 用以只计算中国区域的雪深判识 第六个文件访问序号rvInVistor[5]
                 IRasterDataProvider chinarater = GeoDataDriver.Open(chinarasterfile) as IRasterDataProvider;
                 RasterMaper rmca = new RasterMaper(chinarater, new int[] { 1 });
                 rms.Add(rmca);
                string depthFileName = GetFileName(new string[] { outclipfilename }, _subProductDef.ProductDef.Identify, "MWSD_Native", ".dat", null);
                outRaster = CreateOutRaster(depthFileName, enumDataType.Float, rms.ToArray(), outResolution);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null &&
                        rvInVistor[2].RasterBandsData[0] != null && rvInVistor[3].RasterBandsData[0] != null &&
                        rvInVistor[4].RasterBandsData[0] != null && rvInVistor[5].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        int[] type = new int[dataLength];
                        float[] sdtmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            //type
                            //type[i] = NO_SCATTER;
                            if (rvInVistor[5].RasterBandsData[0][i] == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                continue;//跳出本次循环
                            }
                            float ch10v = 0f; float ch10h = 0f; float ch18v = 0f; float ch18h = 0f; float ch23v = 0f;
                            float ch23h = 0f; float ch36v = 0f; float ch36h = 0f; float ch89v = 0f; float ch89h = 0f;
                            if (filedateDig >= 20110412)
                            {
                                ch10v = btValue(rvInVistor[0].RasterBandsData[0][i]);
                                ch10h = btValue(rvInVistor[0].RasterBandsData[1][i]);
                                ch18v = btValue(rvInVistor[0].RasterBandsData[2][i]);
                                ch18h = btValue(rvInVistor[0].RasterBandsData[3][i]);
                                ch23v = btValue(rvInVistor[0].RasterBandsData[4][i]);
                                ch23h = btValue(rvInVistor[0].RasterBandsData[5][i]);
                                ch36v = btValue(rvInVistor[0].RasterBandsData[6][i]);
                                ch36h = btValue(rvInVistor[0].RasterBandsData[7][i]);
                                ch89v = btValue(rvInVistor[0].RasterBandsData[8][i]);
                                ch89h = btValue(rvInVistor[0].RasterBandsData[9][i]);
                            }
                            else
                            {
                                ch10v = btValue(rvInVistor[0].RasterBandsData[9][i]);
                                ch10h = btValue(rvInVistor[0].RasterBandsData[8][i]);
                                ch18v = btValue(rvInVistor[0].RasterBandsData[7][i]);
                                ch18h = btValue(rvInVistor[0].RasterBandsData[6][i]);
                                ch23v = btValue(rvInVistor[0].RasterBandsData[5][i]);
                                ch23h = btValue(rvInVistor[0].RasterBandsData[4][i]);
                                ch36v = btValue(rvInVistor[0].RasterBandsData[3][i]);
                                ch36h = btValue(rvInVistor[0].RasterBandsData[2][i]);
                                ch89v = btValue(rvInVistor[0].RasterBandsData[1][i]);
                                ch89h = btValue(rvInVistor[0].RasterBandsData[0][i]);
                            }
                            float si1 = ch23v - ch89v;
                            float si2 = ch18v - ch36v;
                            if (ch10v == 0.0f)   //数据未覆盖区或无效值区
                            {
                                sdtmp[i] = -999.0f;
                            }
                            else
                            {
                                if (si1 >= sdParas[15] || si2 >= sdParas[16])
                                {
                                    if (ch23v <= sdParas[17])
                                    {
                                        if (ch18v - ch36v >= sdParas[18])
                                        {
                                            if (si1 - si2 >= sdParas[19])
                                                type[i] = THICK_DRY_SNOW;
                                            else
                                                type[i] = THICK_WET_SNOW;
                                        }
                                        else
                                        {
                                            if (si1 - si2 >= sdParas[20])
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
                                float sdFarmland = sdParas[0] + sdParas[1] * (ch18v - ch36h) + sdParas[2] * (ch89v - ch89h);
                                float sdGrass = sdParas[3] + sdParas[4] * (ch18h - ch36h) - sdParas[5] * (ch18v - ch18h) + sdParas[6] * (ch10v - ch89h) - sdParas[7] * (ch18v - ch89h);
                                float sdBaren = sdParas[8] + sdParas[9] * (ch36h - ch89h) - sdParas[10] * (ch10v - ch89v);
                                float sdForest = sdParas[11] + sdParas[12] * (ch18h - ch36v) - sdParas[13] * (ch18v - ch18h) + sdParas[14] * (ch89v - ch89h);
                                sdtmp[i] = (rvInVistor[1].RasterBandsData[0][i] * sdBaren +
                                    rvInVistor[2].RasterBandsData[0][i] * sdGrass +
                                    rvInVistor[3].RasterBandsData[0][i] * sdForest +
                                    rvInVistor[4].RasterBandsData[0][i] * sdFarmland) / 10000;  //原地类百分比文件扩大了10000倍 
                                //设置输出数据值
                                if (type[i] == NO_SNOW || type[i] == THICK_WET_SNOW || type[i] == THIN_WET_SNOW_OR_FOREST_SNOW || type[i] == VERY_THICK_WET_SNOW)
                                {
                                    sdtmp[i] = 0.0f;
                                }
                                if (sdtmp[i] < 5.0f && sdtmp[i] > 0.00001f) //20140523 将小于5值改为5
                                    sdtmp[i] = 5.0f;
                                if (sdtmp[i] < 0.0000001f)
                                    sdtmp[i] = 0.0f;
                            }
                            rvOutVistor[0].RasterBandsData[0][i] = sdtmp[i];
                        }
                    }
                    //输出
                }));
                rfr.Excute();
                return depthFileName;
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
        /// <summary>
        /// 双线性插值方法提高原数据文件分辨率
        /// f(i+u,j+v)=(1-u)(1-v)*f(i,j) + (1-v)u*f(i,j+1) + v(1-u)*f(i+1,j) + uv*f(i+1,j+1)
        /// </summary>
        /// <param name="srcFilename">原文件</param>
        /// <param name="factor">分辨率提高比例如0.1度到0.01度，zoom = 10</param>
        /// <returns></returns>                                   
        public IFileExtractResult Bilinear(string srcFilename, string identify, Int16 zoom)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
            rms.Add(fileIn1);
            string sdBilinearfilename = GetFileName(new string[] { srcFilename }, _subProductDef.ProductDef.Identify, identify, ".dat", null);
            outRaster = CreateOutRaster(sdBilinearfilename, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX / zoom);
            float[] inRasterBuffer = new float[inRaster1.Width * inRaster1.Height];
            IRasterBand bandinRaster1 = inRaster1.GetRasterBand(1);
            float[] sd = new float[inRaster1.Width * inRaster1.Height];
            float[] sdSnow = new float[outRaster.Width * outRaster.Height];//输出数组
            unsafe
            {
                fixed (float* pointer = inRasterBuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster1.Read(0, 0, inRaster1.Width, inRaster1.Height, ptr, inRaster1.DataType, inRaster1.Width, inRaster1.Height);
                    for (int j = 0; j < inRaster1.Width * inRaster1.Height; j++)
                    {
                        sd[j] = inRasterBuffer[j];
                    }
                }
            }
            Int32[] index = new Int32[zoom * zoom];
            try
            {
                for (int i = 0; i < inRaster1.Width * inRaster1.Height; i++)
                {
                    if ((i + 1) % inRaster1.Width == 0 || i % inRaster1.Width == 0 || i >= inRaster1.Width * inRaster1.Height - inRaster1.Width || i < inRaster1.Width) //插值后的高分格子落在原低分格子的最后一列或第一列，或最后一行，最后一列
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //sdSnow[index[col + row * zoom]] = 0.000001f;
                                sdSnow[index[col + row * zoom]] = sd[i];
                            }
                        }
                    }
                    else
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //(2)通过u,v所处的象限
                                int xCenter = zoom / 2;
                                int yCenter = zoom / 2;
                                if (col < xCenter && row < yCenter) //第二象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i-1,j),f(i-1,j-1)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom);//列
                                    float v = 0.5f + (float)(row * 1.0f / zoom);//行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width - 1] + (1 - v) * u * sd[i - inRaster1.Width] + v * (1 - u) * sd[i - 1] + u * v * sd[i];
                                }
                                if (col >= xCenter && row < yCenter) //第一象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i-1,j),f(i-1,j+1),f(i,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列  横向 
                                    float v = (float)(row * 1.0f / zoom) + 0.5f;   //行 纵
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width] + (1 - v) * u * sd[i - inRaster1.Width + 1] + v * (1 - u) * sd[i] + u * v * sd[i + 1];
                                }
                                if (col < xCenter && row >= yCenter) //第三象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i+1,j-1),f(i+1,j)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom); //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f; //行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - 1] + (1 - v) * u * sd[i] + v * (1 - u) * sd[i + inRaster1.Width - 1] + v * u * sd[i + inRaster1.Width];
                                }
                                if (col >= xCenter && row >= yCenter) //第四象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j+1),f(i+1,j),f(i+1,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f;//行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i] + (1 - v) * u * sd[i + 1] + v * (1 - u) * sd[i + inRaster1.Width] + u * v * sd[i + inRaster1.Width + 1];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            try
            {
                unsafe
                {
                    fixed (float* ptr = sdSnow)
                    {
                        IntPtr sdSnowPtr = new IntPtr(ptr);
                        IRasterBand bandoutRaster = outRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outRaster.Width, outRaster.Height, sdSnowPtr, outRaster.DataType, outRaster.Width, outRaster.Height);
                    }
                }
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sdBilinearfilename, true);
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
        
        private void ComputeFracFile(string sumFile, string file)
        {
            //"china_bares.dat" //"china_grass.dat";//"china_forest.dat"; //"china_farmhand.dat"
            file = file.ToLower();
            string argFile = null;
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
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
            RasterProcessModel<double, Int16> rfr = null;
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
            rfr = new RasterProcessModel<double, Int16>();
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

        private string GetArgSummaryFile(string fileNameBare, string filenameGrass, string filenameForest, string filenameFarmhand)
        {
            string outFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile\\sum.dat");
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

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
        //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
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
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
        protected IRasterDataProvider CreateOutWholeRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
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
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
