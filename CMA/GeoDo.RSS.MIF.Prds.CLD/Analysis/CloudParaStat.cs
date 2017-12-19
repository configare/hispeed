using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class CloudParaStat
    {
        public double MAXCSCF = 0.9d;

        #region SVD分解
        /// <summary>
        /// SVD分解
        /// </summary>
        /// <param name="filename">左标量场时间序列矩阵</param>
        /// <param name="filename">右边标量场时间序列矩阵</param>
        /// <param name="outDir">输出文件目录</param>
        /// <param name="inputFileName">输入文件名之一</param>
        /// <param name="outSize">输出栅格大小</param>
        /// <returns>各模态栅格文件名与模态系数txt文本</returns>
        public string[] AlglibSVD(double[,] marixA, double[,] marixB, string outDir, string inputFileName, Size leftSize, Size rightSize, Action<int, string> progressCallback,bool lismicaps =false,bool rismicaps=false,List<ShapePoint> matchedpos=null)
        {
            if (progressCallback != null)
                progressCallback(18, "计算待分解矩阵...");
            double[,] matrixM = GetMatrixToSVD(marixA,marixB);
            if (matrixM==null)
            {
                if (progressCallback != null)
                    progressCallback(0, "计算待分解矩阵失败！");
                return null;
            }
            //svd分解
            double[] w = new double[] { };//奇异值
            double[,] u = new double[,] { };//左场同质模态
            double[,] vt = new double[,] { };//右场同质模态
            try
            {
                if (progressCallback != null)
                    progressCallback(25, "开始进行SVD分解...");
                //bool ok = alglib.rmatrixsvd(matrixM, matrixM.GetLength(0), matrixM.GetLength(1), 2, 2, 2, out w, out u, out vt);
                var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(matrixM);
                MathNet.Numerics.Control.UseNativeMKL();
                MathNet.Numerics.LinearAlgebra.Factorization.Svd<double> svd = matrix.Svd(true);
                u = svd.U.ToArray();
                vt = svd.VT.ToArray();
                w = svd.S.ToArray();                //if (ok)
                {
                    if (w == null || w.Length <= 0)
                    {
                        progressCallback(0, "SVD分解结果不正确！请重试！");
                        return null;
                    }
                    if (u == null || u.GetLength(1) <= 0)
                    {
                        progressCallback(0, "SVD分解结果不正确！请重试！");
                        return null;
                    }
                    if (w == null || w.GetLength(0) <= 0)
                    {
                        progressCallback(0, "SVD分解结果不正确！请重试！");
                        return null;
                    }
                    if (progressCallback != null)
                        progressCallback(85, "SVD分解完成，正在输出结果...");
                    //计算方差贡献, 累计方差贡献(利用奇异值计算累计方差贡献，需要数据为中心化的数据)
                    double[] scf, cscf;
                    //cscf = CalculateCSCF(w);
                    scf = AnaliysisDataPreprocess.CalcSCF(w);
                    cscf = AnaliysisDataPreprocess.CalcCSCF(scf);
                    //计算输出模态个数，最多输出5个
                    double MaxPercent = 0.80;
                    int outCount = AnaliysisDataPreprocess.GetMaxCSCFCount(cscf, MaxPercent);       
                    outCount = outCount > 5 ? 5 : outCount;//最多输出5个模态
                    //计算时间系数
                    double[,] leftCoef, rightCoef;
                    CalculateTimeCoefficient(marixA, marixB, u, vt, outCount, out leftCoef, out rightCoef);
                    //计算相关系数
                    double[] correlateC = CalculateCorrelationCoefficient(leftCoef, rightCoef);
                    //输出结果文件
                    ////输出各个模态栅格
                    List<string> results = new List<string>();
                    string[] resultNames;
                    if (!lismicaps&&!rismicaps)
                    {
                        //resultNames = SaveArrayToRaster(outDir, inputFileName, leftSize, rightSize, u, vt, outCount);
                        resultNames = SaveArrayToRaster(outDir, inputFileName, leftSize, u, outCount,true);
                        results.AddRange(resultNames);
                        resultNames = SaveArrayToRaster(outDir, inputFileName, rightSize, vt, outCount, false);
                        results.AddRange(resultNames);
                    } 
                    else
                    {
                        resultNames = SaveArrayToVector(outDir,u, outCount,true,matchedpos.ToArray());
                        results.AddRange(resultNames);
                        resultNames = SaveArrayToVector(outDir, vt, outCount, false, matchedpos.ToArray());
                        results.AddRange(resultNames);
                    }
                    if (progressCallback != null)
                        progressCallback(95, "SVD分解完成，正在输出结果...");
                    ////输出时间系数、相关系数、累计方差贡献（文本）
                    string regionName = "all";
                    if (StatRegionSet.UseRegion)
                        regionName = StatRegionSet.RegionName;
                    string txtFName;
                    txtFName = SaveArrayParameterToFile(outDir, w, "0.奇异值_" + regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, scf, "1.方差贡献_"+ regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, cscf, "2.累计方差贡献_"+ regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, correlateC, "3.相关系数_"+ regionName);
                    results.Add(txtFName);
                    string header ="{0}场时间系数_{1}.txt";
                    txtFName = SaveTimeParameterToFile(outDir, string.Format(header, "左", regionName), leftCoef);
                    //SaveTimeParameterToExcel(outDir,inputFileName, leftCoef, true);
                    results.Add(txtFName);
                    txtFName = SaveTimeParameterToFile(outDir, string.Format(header, "右", regionName), rightCoef);
                    //SaveTimeParameterToExcel(outDir, inputFileName, rightCoef, false);
                    results.Add(txtFName);
                    if (progressCallback != null)
                        progressCallback(100, "完成SVD分解");
                    return results.ToArray();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            if (progressCallback != null)
                progressCallback(100, "SVD分解失败");
            return null;
        }

        #region SVD分解(AOI输出)
        /// <summary>
        /// SVD分解(AOI输出)
        /// </summary>
        /// <param name="filename">左标量场时间序列矩阵</param>
        /// <param name="filename">右边标量场时间序列矩阵</param>
        /// <param name="outDir">输出文件目录</param>
        /// <param name="inputFileName">输入文件名之一</param>
        /// <param name="outSize">输出栅格大小</param>
        /// <returns>各模态栅格文件名与模态系数txt文本</returns>
        public string[] AlglibSVDWithAOI(double[,] marixA, double[,] marixB, string outDir, string inputFileName, Size leftSize, Size rightSize, Action<int, string> progressCallback, bool lismicaps = false, bool rismicaps = false, List<ShapePoint> matchedpos = null)
        {
            if (progressCallback != null)
                progressCallback(18, "计算待分解矩阵...");
            double[,] matrixM = GetMatrixToSVD(marixA, marixB);
            //svd分解
            double[] w = new double[] { };//奇异值
            double[,] u = new double[,] { };//左场同质模态
            double[,] vt = new double[,] { };//右场同质模态
            try
            {
                if (progressCallback != null)
                    progressCallback(25, "开始进行SVD分解...");
                bool ok = alglib.rmatrixsvd(matrixM, matrixM.GetLength(0), matrixM.GetLength(1), 2, 2, 2, out w, out u, out vt);
                if (ok)
                {
                    if (progressCallback != null)
                        progressCallback(85, "SVD分解完成，正在输出结果...");
                    //计算方差贡献, 累计方差贡献(利用奇异值计算累计方差贡献，需要数据为中心化的数据)
                    double[] scf, cscf;
                    //cscf = CalculateCSCF(w);
                    scf = AnaliysisDataPreprocess.CalcSCF(w);
                    cscf = AnaliysisDataPreprocess.CalcCSCF(scf);
                    //计算输出模态个数，最多输出5个
                    double MaxPercent = 0.80;
                    int outCount = AnaliysisDataPreprocess.GetMaxCSCFCount(cscf, MaxPercent);
                    outCount = outCount > 5 ? 5 : outCount;//最多输出5个模态
                    //计算时间系数
                    double[,] leftCoef, rightCoef;
                    CalculateTimeCoefficient(marixA, marixB, u, vt, outCount, out leftCoef, out rightCoef);
                    //计算相关系数
                    double[] correlateC = CalculateCorrelationCoefficient(leftCoef, rightCoef);
                    //输出结果文件
                    ////输出各个模态栅格
                    List<string> results = new List<string>();
                    string[] resultNames;
                    if (!lismicaps && !rismicaps)
                    {
                        resultNames = SaveSVDAnaResult(u, outDir, StatRegionSet.SubRegionEnvLeft, StatRegionSet.SubRegionOutSizeLeft, StatRegionSet.SubRegionOutIndexLeft,outCount, true);
                        results.AddRange(resultNames);
                        resultNames = SaveSVDAnaResult(vt, outDir, StatRegionSet.SubRegionEnvRight, StatRegionSet.SubRegionOutSizeRight, StatRegionSet.SubRegionOutIndexRight, outCount, false);
                        results.AddRange(resultNames);
                    }
                    if (progressCallback != null)
                        progressCallback(95, "SVD分解完成，正在输出结果...");
                    ////输出时间系数、相关系数、累计方差贡献（文本）
                    string regionName = "all";
                    if (StatRegionSet.UseVectorAOIRegion)
                        regionName = StatRegionSet.AOIName;
                    string txtFName;
                    txtFName = SaveArrayParameterToFile(outDir, w, "0.奇异值_" + regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, scf, "1.方差贡献_" + regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, cscf, "2.累计方差贡献_" + regionName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, correlateC, "3.相关系数_" + regionName);
                    results.Add(txtFName);
                    string header = "{0}场时间系数_{1}.txt";
                    txtFName = SaveTimeParameterToFile(outDir, string.Format(header, "左", regionName), leftCoef);
                    //SaveTimeParameterToExcel(outDir,inputFileName, leftCoef, true);
                    results.Add(txtFName);
                    txtFName = SaveTimeParameterToFile(outDir, string.Format(header, "右", regionName), rightCoef);
                    //SaveTimeParameterToExcel(outDir, inputFileName, rightCoef, false);
                    results.Add(txtFName);
                    if (progressCallback != null)
                        progressCallback(100, "完成SVD分解");
                    return results.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            if (progressCallback != null)
                progressCallback(100, "SVD分解失败");
            return null;
        }

        public string[] SaveSVDAnaResult(double[,] result, string outDir, CoordEnvelope subFileEnv, Size outsize, int[] outIndex, int outCount = 1, bool isLeft = true)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            List<string> outFileList = new List<string>();
            //Envelope env = StatRegionSet.AOIEnvelope;
            int lines = outsize.Height;
            int samples = outsize.Width;
            for (int m = 0; m < outCount; m++)//输出模态数
            {
                string outFileName = CloudParaStat.GetMatrixFileName(outDir, isLeft, m + 1);
                using (IRasterDataProvider dataPrd = CreateOutputRaster(outFileName, subFileEnv, outsize))
                {
                    double[] databuffer = new double[lines * samples];
                    for (int oRow = 0; oRow < outIndex.Length; oRow++)
                    {
                        int no = outIndex[oRow];
                        if (isLeft)//输出左奇异向量第m模态
                        {
                            databuffer[no] = result[oRow, m];//u的i列为A矩阵第i模态，oRow为行，m为列
                        }
                        else//输出右奇异向量第m模态
                        {
                            databuffer[no] = result[m, oRow];//vt的i行为A矩阵第i模态，m为行，oRow为列
                        }
                    }
                    unsafe
                    {
                        fixed (double* ptr = databuffer)
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            dataPrd.GetRasterBand(1).Write(0, 0, samples, lines, buffer, enumDataType.Double, samples, lines);
                        }
                    }
                }
                outFileList.Add(outFileName);
            }
            return outFileList.ToArray();
        }

        public static IRasterDataProvider CreateOutputRaster(string outFileName, CoordEnvelope outenv, Size outSize)
        {
            IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            if (rasterDriver == null)
                throw new Exception("数据驱动获取失败");
            List<string> opts = new List<string>();
            opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + SpatialReference.GetDefault(),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + outenv.MinX + "," + outenv.MaxY + "}:{" + outenv.Width/(float)outSize.Width + "," +  outenv.Height/(float)outSize.Height + "}"
                    });
            IRasterDataProvider tProviders = rasterDriver.Create(outFileName, outSize.Width, outSize.Height, 1, enumDataType.Double, opts.ToArray());
            return tProviders;
        }


#endregion

        public string[] AlglibSVDWithMicaps(double[,] marixA, double[,] marixB, string outDir, string inputFileName,Action<int, string> progressCallback)
        {
            return null;
        }

        private string SaveTimeParameterToExcel(string outDir, string inputFileName,double[,] array, bool isleft)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string filename;
            if (isleft)
                filename = Path.Combine(outDir, "SVD分解_左场时间系数_"+DateTime.Now.ToShortDateString());
            else
                filename = Path.Combine(outDir, "SVD分解_右场时间系数") + DateTime.Now.ToShortDateString();
            List<string[]> resultList = new List<string[]>();
            int row = array.GetLength(0);//模态数
            int column = array.GetLength(1);//时间序列个数
            string outMode = "第{0}模态";
            int k;
            for (int i = 0; i < row; i++)
            {
                k = i + 1;
                string valueStr = string.Format(outMode, k);
                for (int j = 0; j < column; j++)
                {
                    valueStr += (","+array[i, j]);
                }
                resultList.Add(valueStr.Split(','));
            }
            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string[] columns = new string[] { "模态", "时间序列" };
            IStatResult fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string title = Path.GetFileName(filename).Split('_')[1];
            int notStatCol = 1;
            bool isTotal=true;
            int statImage = 1;
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
                {
                    excelControl.Init();
                    excelControl.Zoom = 1;
                    //excelControl.Add(true, title, results, isTotal, 1);
                    excelControl.Add(true, title, fresult, fresult.Columns.Length - notStatCol, isTotal, 1, statImage);
                    if (!filename.ToUpper().EndsWith(".XLSX"))
                        filename += ".XLSX";
                    excelControl.SaveFile(filename);
                }
            }
            catch (Exception)
            {
                using (StatResultToTxtFile txtControl = new StatResultToTxtFile())
                {
                    if (!filename.ToUpper().EndsWith(".TXT"))
                        filename += ".TXT";
                    txtControl.WriteResultToTxt(title + "\n");
                    txtControl.WriteResultToTxt("统计日期：" + DateTime.Today.Date.ToShortDateString() + "\n");
                    txtControl.WriteResultToTxt(fresult);
                    bool isSave = txtControl.SaveFile(filename);
                    if (!isSave)
                        return String.Empty;
                }
            }
            return filename;
        }

        private string SaveTimeParameterToFile(string outDir, string txtFname,double[,] array)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string outfile = Path.Combine(outDir, txtFname);
            using (StreamWriter sw = new StreamWriter(outfile, false, Encoding.Default))
            {
                int row = array.GetLength(0);//模态数
                int column=array.GetLength(1);//时间序列个数
                string valueStr;
                string outMode = "第{0}模态：";
                int k;
                for (int i = 0; i < row; i++)
                {
                    k = i+1;
                    valueStr = string.Empty;
                    sw.Write(string.Format(outMode, k));
                    for (int j = 0; j < column; j++)
                    {
                        valueStr += (array[i, j] + ","); 
                    }
                    valueStr.Remove(valueStr.Length - 1);
                    sw.WriteLine(valueStr);
                }
            }
            return txtFname;
        }

        //输出一维数组至文本文件
        private string SaveArrayParameterToFile(string outDir, double[] array,string fileName)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string txtFname = Path.Combine(outDir, fileName + ".txt");
            using (StreamWriter sw = new StreamWriter(txtFname, false, Encoding.Default))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    //if (array[i] != 0)
                        sw.WriteLine(array[i]);
                }
            }
            return txtFname;
        }

        /// <summary>
        /// 计算前outCount个时间系数
        /// </summary>
        /// <param name="marixA">原始左矩阵</param>
        /// <param name="marixB">原始右矩阵</param>
        /// <param name="marixU">左场模态变量</param>
        /// <param name="marixVt">右场模态变量</param>
        /// <param name="outCount">输出前outCount个模态</param>
        /// <param name="leftCoef">左场模态变量时间系数矩阵（第i个左场模态时间系数为第i行,）</param>
        /// <param name="rightCoef">右场模态变量时间系数矩阵（第i个右场模态时间系数为第i行）</param>
        private void CalculateTimeCoefficient(double[,] marixA, double[,] marixB, double[,] marixU, double[,] marixVt, int outCount, out double[,] leftCoef, out double[,] rightCoef)
        {
            //时间序数
            int times=marixA.GetLength(0);
            leftCoef = new double[outCount, times];
            rightCoef = new double[outCount, times];
            double[] timeCoef = new double[marixA.GetLength(0)];
            for (int i = 0; i < outCount; i++)
            { 
                timeCoef=CalculateTimeCoefficient(marixA, marixU,i, true);
                for (int j = 0; j < times; j++)
                {
                    leftCoef[i, j] = timeCoef[j];
                }
            }
            for (int i = 0; i < outCount; i++)
            {
                timeCoef = CalculateTimeCoefficient(marixB, marixVt,i, false);
                for (int j = 0; j < times; j++)
                {
                    rightCoef[i, j] = timeCoef[j];
                }
            }

        }

        /// <summary>
        /// 计算模态index的时间系数
        /// </summary>
        /// <param name="marixA">原始标量场时间序列</param>
        /// <param name="marixU">奇异向量矩阵</param>
        /// <param name="index">模态序号</param>
        /// <param name="isLeft">是否为左场</param>
        /// <returns>时间系数（个数为时间序数）</returns>
        private double[] CalculateTimeCoefficient(double[,] marixA, double[,] marixU, int index, bool isLeft)
        {
            double[] timeC = new double[marixA.GetLength(0)];//时间序数n个,行数
            int columnLength=marixA.GetLength(1);//空间点数，列数
            if (isLeft)
            {
                for(int i=0;i<timeC.Length;i++)//时间序列
                {
                    for (int j = 0; j < columnLength; j++)//空间点数
                    {
                        timeC[i] += marixA[i, j] * marixU[j, index];//
                    }
                }
            }
            else
            {
                for (int i = 0; i < timeC.Length; i++)
                {
                    for (int j = 0; j < columnLength; j++)
                    {
                        timeC[i] += marixA[i, j] * marixU[index, j];
                    }
                }
            }
            return timeC;
        }

        /// <summary>
        /// 计算相关系数
        /// </summary>
        /// <param name="u">左场时间系数矩阵</param>
        /// <param name="vt">右场时间系数矩阵</param>
        /// <returns>相关系数数组</returns>
        private double[] CalculateCorrelationCoefficient(double[,] leftCoef, double[,] rightCoef)
        {
            if (leftCoef.Length == 0 || rightCoef.Length == 0 || leftCoef.Length != rightCoef.Length)
                return null;
            int length0=leftCoef.GetLength(0);
            int length1=leftCoef.GetLength(1);
            double[] corelateC = new double[length0];
            double[] x = new double[length1];
            double[] y = new double[length1];
            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    x[j] = leftCoef[i, j];
                    y[j] = rightCoef[i, j]; 
                }
                corelateC[i] = CalculateCorrelationCoefficient(x, y);
            }
            return corelateC;
        }

        /// <summary>
        /// 计算变量序列x.y相关系数
        /// </summary>
        /// <param name="x">变量序列x</param>
        /// <param name="y">变量序列y</param>
        public double CalculateCorrelationCoefficient(double[] x, double[] y)
        {
            if (x == null || y == null || x.Length == 0 || y.Length == 0 || x.Length != y.Length)
                throw new ArgumentOutOfRangeException("左右场待统计数据大小不一致");
            double sumX = 0, sumY = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sumX += x[i];
                sumY += y[i];
            }
            double avgX = sumX / x.Length;//中心化的数据，均值应为0；
            double avgY = sumY / y.Length;//中心化的数据，均值应为0；
            double sx2 = 0, sy2 = 0, cov = 0;
            double disx, disy;
            for (int i = 0; i < x.Length; i++)
            {
                disx = x[i] - avgX;
                disy = y[i] - avgY;
                cov += disx * disy;
                sx2 += Math.Pow(disx, 2);
                sy2 += Math.Pow(disy, 2);
            }
            double cor =cov / (Math.Sqrt(sx2) * Math.Sqrt(sy2));
            return cor>=1?1:cor;
        }

        public double CalculateCorrelationCoefficient(double[] x, double[] y,long scX,long scY)
        {
            if (x==null||y==null||x.Length == 0 || y.Length == 0 )//|| x.Length != y.Length)
                return 0;
            long sc = Math.Min(scX, scY);
            double sumX = 0, sumY = 0;
            for (int i = 0; i < sc; i++)
            {
                sumX +=x[i];
                sumY += y[i];
            }
            double avgX = sumX / sc;
            double avgY = sumY / sc;
            double sx2 = 0, sy2 = 0, cov= 0;
            double disx, disy;
            for (int i = 0; i < sc; i++)
            {
                disx = x[i] - avgX;
                disy = y[i] - avgY;
                cov += disx * disy;
                sx2 += Math.Pow(disx, 2);
                sy2 += Math.Pow(disy, 2);
            }
            return cov / (Math.Sqrt(sx2) * Math.Sqrt(sy2));

        }

        public double CalculateCorrelationCoefficient(float[] x, float[] y, long scX, long scY)
        {
            if (x == null || y == null || x.Length == 0 || y.Length == 0)//|| x.Length != y.Length)
                return 0;
            long sc = Math.Min(scX, scY);
            double sumX = 0, sumY = 0;
            for (int i = 0; i < sc; i++)
            {
                sumX += x[i];
                sumY += y[i];
            }
            double avgX = sumX / sc;
            double avgY = sumY / sc;
            double sx2 = 0, sy2 = 0, cov = 0;
            double disx, disy;
            for (int i = 0; i < sc; i++)
            {
                disx = x[i] - avgX;
                disy = y[i] - avgY;
                cov += disx * disy;
                sx2 += Math.Pow(disx, 2);
                sy2 += Math.Pow(disy, 2);
            }
            return cov / (Math.Sqrt(sx2) * Math.Sqrt(sy2));

        }
        /// <summary>
        /// 利用奇异值计算累计方差贡献，需要数据为中心化的数据
        /// </summary>
        /// <param name="w"></param>
        /// <returns>前n个模态的累计方差贡献</returns>
        private double[] CalculateCSCF(double[] w)
        {
            if (w.Length < 1)
                return null;
            double[] cscf;
            cscf = AnaliysisDataPreprocess.CalcSCF(w);
            cscf = AnaliysisDataPreprocess.CalcCSCF(cscf);
            return cscf;
        }

        /// <summary>
        /// 将左右场的分解后的奇异向量另存为栅格
        /// </summary>
        /// <param name="outDir"></param>
        /// <param name="inputFilename"></param>
        /// <param name="outSize"></param>
        /// <param name="u"></param>
        /// <param name="outCount"></param>
        /// <param name="isleft"></param>
        /// <returns></returns>
        private string[] SaveArrayToRaster(string outDir, string inputFilename, Size outSize,double[,] u,int outCount,bool isleft)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            List<string> outFileList = new List<string>();
            for (int m = 0; m < outCount; m++)
            {
                string outFileName = GetMatrixFileName(outDir, isleft, m + 1);
                using (IRasterDataProvider dataPrd = CreateOutputRaster(outFileName, inputFilename, outSize))
                {
                    //写栅格
                    int rowStep = ClipCutHelper.ComputeRowStep(dataPrd, 0, dataPrd.Height);
                    int sample = dataPrd.Width;
                    int bufferSize = sample * rowStep;
                    int typeSize = ClipCutHelper.GetSize(dataPrd.DataType);
                    int index = 0;
                    for (int oRow = 0; oRow < dataPrd.Height; oRow += rowStep)
                    {
                        if (oRow + rowStep > dataPrd.Height)
                        {
                            rowStep = dataPrd.Height - oRow;
                            bufferSize = sample * rowStep;
                        }
                        double[] databuffer = new double[bufferSize];
                        if (isleft)//输出左奇异向量第m模态
                        {
                            for (int i = index, j = 0; j < bufferSize; i++, j++)
                            {
                                databuffer[j] = u[i, m];//u的i列为A矩阵第i模态
                            }
                        }
                        else//输出右奇异向量第m模态
                        {
                            for (int i = index, j = 0; j < bufferSize; i++, j++)
                            {
                                databuffer[j] = u[m, i];//vt的i行为B第i模态
                            }
                        }
                        unsafe
                        {
                            fixed (double* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                dataPrd.GetRasterBand(1).Write(0, oRow, sample, rowStep, buffer, enumDataType.Double, sample, rowStep);
                            }
                        }
                        index += bufferSize;
                    }
                }
                outFileList.Add(outFileName);
            }
            return outFileList.ToArray();
        }

        /// <summary>
        /// 将左右场的对应点分解后的奇异向量另存为矢量，txt
        /// </summary>
        /// <param name="outDir"></param>
        /// <param name="inputFilename"></param>
        /// <param name="outSize"></param>
        /// <param name="u"></param>
        /// <param name="outCount"></param>
        /// <param name="isleft"></param>
        /// <returns></returns>
        private string[] SaveArrayToVector(string outDir, double[,] u, int outCount, bool isleft,ShapePoint[]matchedpos)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            List<string> outFileList = new List<string>();
            string ext = ".txt";
            int width;
            for (int m = 0; m < outCount; m++)
            {
                string outFileName = GetMatrixFileName(outDir, isleft, m + 1, ext);
                using (StreamWriter sw = new StreamWriter(outFileName, false, Encoding.Default))
                {
                    if (isleft)//输出左奇异向量第m模态
                    {
                        width = u.GetLength(0);
                        for (int i = 0; i < width; i++)
                        {
                            sw.Write(matchedpos[i].Y.ToString("#0.00").PadLeft(6, ' '));
                            sw.Write(",");
                            sw.Write(matchedpos[i].X.ToString("#0.00").PadLeft(7, ' '));
                            sw.Write(",");
                            sw.WriteLine(u[i, m]);//u的i列为A矩阵第i模态
                        }
                    }
                    else//输出右奇异向量第m模态
                    {
                        width = u.GetLength(1);
                        for (int i = 0; i < width; i++)
                        {
                            sw.Write(matchedpos[i].Y.ToString("#0.00").PadLeft(6, ' '));
                            sw.Write(",");
                            sw.Write(matchedpos[i].X.ToString("#0.00").PadLeft(7, ' '));
                            sw.Write(",");
                            sw.WriteLine(u[m, i]);//u的i列为A矩阵第i模态
                        }
                    }
                }
                outFileList.Add(outFileName);
            }
            return outFileList.ToArray();
        }

        private string[] SaveArrayToRaster(string outDir, string inputFilename, Size leftSize, Size rightSize, double[,] u, double[,] vt, int outCount)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            List<string> outFileList = new List<string>();
            //u的i列为A矩阵第i模态 
            //vt的i行为B第i模态
            for (int m = 0; m < outCount; m++)
            {
                //1、输出左奇异向量第m模态
                string outFileName = GetMatrixFileName(outDir, true, m + 1);
                using (IRasterDataProvider dataPrd = CreateOutputRaster(outFileName, inputFilename, leftSize))
                {
                    //写栅格
                    int rowStep = ClipCutHelper.ComputeRowStep(dataPrd, 0, dataPrd.Height);
                    int sample = dataPrd.Width;
                    int bufferSize = sample * rowStep;
                    int typeSize = ClipCutHelper.GetSize(dataPrd.DataType);
                    int index = 0;
                    for (int oRow = 0; oRow < dataPrd.Height; oRow += rowStep)
                    {
                        if (oRow + rowStep > dataPrd.Height)
                        {
                            rowStep = dataPrd.Height - oRow;
                            bufferSize = sample * rowStep;
                        }
                        double[] databuffer = new double[bufferSize];
                        for (int i = index, j = 0; j < bufferSize; i++, j++)
                        {
                            databuffer[j] = u[i, m];
                        }
                        unsafe
                        {
                            fixed (double* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                dataPrd.GetRasterBand(1).Write(0, oRow, sample, rowStep, buffer, enumDataType.Double, sample, rowStep);
                            }
                        }
                        index += bufferSize;
                    }
                }
                outFileList.Add(outFileName);
                //2、输出右奇异向量第m模态
                outFileName = GetMatrixFileName(outDir, false, m + 1);
                using (IRasterDataProvider dataPrd = CreateOutputRaster(outFileName, inputFilename, rightSize))
                {
                    //写栅格
                    int rowStep = ClipCutHelper.ComputeRowStep(dataPrd, 0, dataPrd.Height);
                    int sample = dataPrd.Width;
                    int bufferSize = sample * rowStep;
                    int typeSize = ClipCutHelper.GetSize(dataPrd.DataType);
                    int index = 0;
                    for (int oRow = 0; oRow < dataPrd.Height; oRow += rowStep)
                    {
                        if (oRow + rowStep > dataPrd.Height)
                        {
                            rowStep = dataPrd.Height - oRow;
                            bufferSize = sample * rowStep;
                        }
                        double[] databuffer = new double[bufferSize];
                        for (int i = index, j = 0; j < bufferSize; i++, j++)
                        {
                            databuffer[j] = vt[m, i];
                        }
                        unsafe
                        {
                            fixed (double* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                dataPrd.GetRasterBand(1).Write(0, oRow, sample, rowStep, buffer, enumDataType.Double, sample, rowStep);
                            }
                        }
                        index += bufferSize;
                    }
                }
                outFileList.Add(outFileName);
            }
            return outFileList.ToArray();
        }

        public static string GetMatrixFileName(string outDir, bool isleft, int index, string ext = ".dat")
        {
            string regionName = "all";
            if (StatRegionSet.UseRegion)
                regionName = StatRegionSet.RegionName;
            else if(StatRegionSet.UseVectorAOIRegion)
                regionName = StatRegionSet.AOIName;
            string outHeader = "SVD分解_{0}场第";
            if(isleft)
                outHeader =string.Format(outHeader,"左");
            else
                outHeader =string.Format(outHeader,"右");
            return Path.Combine(outDir, outHeader + index + "模态_" + regionName + ext);  

        }

        /// <summary>
        /// 获取AT*B 待分解矩阵
        /// A、B矩阵均为每行表示一个时间维的场数据
        /// 则，S = At*B
        /// </summary>
        /// <param name="marixA">左标量场时间序列矩阵</param>
        /// <param name="marixB">右标量场时间序列矩阵</param>
        /// <returns></returns>
        private unsafe double[,] GetMatrixToSVD(double[,] marixA,double[,] marixB)
        {
            if (marixA.GetLength(0) != marixB.GetLength(0))//矩阵A的行数（转秩后的列数）要求和矩阵B的行数相等
            {
                throw new Exception("参数错误，要相乘的矩阵A列数和矩阵B的行数不一致，无法相乘");
            }
            //try
            int rowCount=marixA.GetLength(1);
            int columnCount=marixB.GetLength(1);
            int times=marixA.GetLength(0);//行方向为时间序列
            double[,] matrixM = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    for (int m = 0; m < times; m++)
                        matrixM[i, j] += marixA[m, i] * marixB[m, j];//对应列的元素相乘后加和
                }
            }
            return matrixM;
        }

        private unsafe double[] GetDataValue(IRasterDataProvider dataPrd)
        {
            int width = dataPrd.Width;
            int height = dataPrd.Height;
            int length = width * height;
            double[] array = new double[length];
            enumDataType dataType = dataPrd.DataType;
            IRasterBand band;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[width * height];
                        band = dataPrd.GetRasterBand(23);
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                        }
                        for (int j = 0; j < length; j++)
                        {
                            array[j] = buffer[j];
                        }
                        return array;
                    }
                case enumDataType.Double:
                    {
                        double[] buffer = new double[width * height];
                        band = dataPrd.GetRasterBand(1);
                        fixed (double* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Double, dataPrd.Width, dataPrd.Height);
                        }
                        for (int j = 0; j < length; j++)
                        {
                            array[j] = buffer[j];
                        }
                        return array;
                    }
                case enumDataType.Int16:
                    {
                        Int16[] buffer = new Int16[width * height];
                        band = dataPrd.GetRasterBand(1);
                        fixed (Int16* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Int16, dataPrd.Width, dataPrd.Height);
                        }
                        for (int j = 0; j < length; j++)
                        {
                            array[j] = buffer[j];
                        }
                        return array;
                    }
            }
            return null;
        }

        #endregion
        private bool CheckFiles(string[] files, out int width, out int height,out enumDataType dataType)
        {
            width = height = 0;
            dataType = enumDataType.Unknow;
            if (files == null || files.Length < 1)
                return false;
            for (int i = 0; i < files.Length; i++)
            {
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[i]) as IRasterDataProvider)
                {
                    if (width == 0 && height == 0)
                    {
                        width = dataPrd.Width;
                        height = dataPrd.Height;
                        dataType = dataPrd.DataType;
                    }
                    else
                    {
                        if (width != dataPrd.Width || height != dataPrd.Height || dataType != dataPrd.DataType)
                            return false;
                    }
                }
            }
            return true;
        }

        public static IRasterDataProvider CreateOutputRaster(string outFileName, string inputFileName, Size outSize)
        {
            IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            if (rasterDriver == null)
                throw new Exception("数据驱动获取失败");
            List<string> opts = new List<string>();
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(inputFileName) as IRasterDataProvider)
            {
                double r = dataPrd.Width * 1d / outSize.Width;
                opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + (dataPrd.SpatialRef==null?"":dataPrd.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + dataPrd.CoordEnvelope.MinX + "," + dataPrd.CoordEnvelope.MaxY + "}:{" + dataPrd.ResolutionX*r + "," + dataPrd.ResolutionY*r + "}"
                    });
            }
            IRasterDataProvider tProviders = rasterDriver.Create(outFileName, outSize.Width, outSize.Height, 1, enumDataType.Double, opts.ToArray());
            return tProviders;
        }

        #region  EOF分解

        private double[,] CalEOFTimeEfficients(double[,] meanvalues, int ntimes, int npoints, double[,] eigVectors)
        {
            double[,] timeEfficients = new double[ntimes, npoints];
            for (int n = 0; n < ntimes; n++)//1~n
            {
                for (int p = 0; p < npoints; p++)//1~m
                {
                    for (int k = 0; k < npoints; k++)//1~m
                    {
                        timeEfficients[n, p] += eigVectors[k, p] * meanvalues[n, k];
                    }
                }
            }
            return timeEfficients;
        }

        private string _eofAnaBandName=null;
        public string[] AlglibEOF(double[,] matrixM, string outDir, string bandname, Action<int, string> progressCallback, bool ismicaps = true, List<ShapePoint> matchedpos = null)
        {
            if (progressCallback != null)
                progressCallback(18, "计算待分解矩阵...");
            _eofAnaBandName=bandname;
            int ntimes = matrixM.GetLength(0);//时间序列个数
            int npoints = matrixM.GetLength(1);//点数
            int info = 0;
            double[] eigValues;
            double[,] eigVectors;
            try
            {
                if (progressCallback != null)
                    progressCallback(25, "开始进行EOF分解...");
                alglib.pcabuildbasis(matrixM, ntimes, npoints, out info, out eigValues, out eigVectors);
                if (info == 1)
                {
                    if (progressCallback != null)
                        progressCallback(85, "EOF分解完成，正在输出结果...");
                    //计算方差贡献, 累计方差贡献
                    double[] scf, cscf;
                    scf = AnaliysisDataPreprocess.CalcSCF(eigValues);
                    cscf = AnaliysisDataPreprocess.CalcCSCF(scf);
                    ////计算输出模态个数，最多输出5个
                    //double MaxPercent = 0.80;
                    int outCount = AnaliysisDataPreprocess.GetMaxCSCFCount(cscf, MAXCSCF);
                    //计算主分量（时间系数）
                    double[,] timeCoef = CalEOFTimeEfficients(matrixM, ntimes, npoints, eigVectors);
                    //输出结果文件
                    string regionName = "all";
                    if (StatRegionSet.UseRegion)
                        regionName = StatRegionSet.RegionName;
                    if (StatRegionSet.UseVectorAOIRegion)
                    {
                        regionName ="AOI"+ StatRegionSet.AOIName;
                    }
                    ////输出各个主分量（时间系数）
                    List<string> results = new List<string>();
                    string resultName;
                    if (true)
                    {
                        resultName = SaveEOFTimeCoffToVector(outDir, timeCoef, regionName);
                        results.Add(resultName);
                    }
                    string txtFName;
                    txtFName = SaveArrayParameterToFile(outDir, eigValues, "0.EOF分解_奇异值_" + regionName + "_" + _eofAnaBandName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, scf, "1.EOF分解_方差贡献_" + regionName + "_" + _eofAnaBandName);
                    results.Add(txtFName);
                    txtFName = SaveArrayParameterToFile(outDir, cscf, "2.EOF分解_累计方差贡献_" + regionName + "_" + _eofAnaBandName);
                    results.Add(txtFName);
                    ////输出各个特征向量--典型场，npoints×npoints
                    txtFName = SaveEOFEigvectorToFile(outDir, eigVectors,outCount,regionName);
                    ////SaveTimeParameterToExcel(outDir,inputFileName, leftCoef, true);
                    results.Add(txtFName);
                    if (progressCallback != null)
                        progressCallback(100, "完成EOF分解");
                    return results.ToArray();
                }
                else if (info == -4)
                {
                    progressCallback(100, "SVD subroutine haven't converged,SVD分解不收敛！");
                }
                else if (info == -1)
                {
                    progressCallback(100, "wrong parameters has been passed (NPoints<0,NVars<1),参数不对!");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            if (progressCallback != null)
                progressCallback(100, "EOF分解失败");
            return null;
        }

        private string SaveEOFTimeCoffToVector(string outDir, double[,] timeCoff, string regionName,bool isTimeCoff=true)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            List<string> outFileList = new List<string>();
            string headerformat =isTimeCoff?"3.EOF分解_时间系数_{0}_{1}.txt":"4.EOF分解_特征向量_{0}_{1}.txt";
            string outHeader = Path.Combine(outDir, string.Format(headerformat, regionName, _eofAnaBandName));
            string lineHeader = "第{0}时间：";
            int height = timeCoff.GetLength(0);//时间序列×空间点数
            int width = timeCoff.GetLength(1);//空间点数
            using (StreamWriter sw = new StreamWriter(outHeader, false, Encoding.Default))
            {
                for (int m = 0; m < height; m++)
                {
                    string lineName = string.Format(lineHeader,m+1);
                    sw.Write(lineName);
                    for (int i = 0; i < width; i++)
                    {
                        sw.Write(timeCoff[m, i]);
                        sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }           
            return outHeader;
        }

        /// <summary>
        /// 将EOF分解后的特征矢量另存为文本文件，若为AOI区域则另存为栅格；
        /// </summary>
        /// <param name="outDir"></param>输出路径
        /// <param name="eigvector"></param>特征矢量,m*m,m为点数
        /// <param name="outCount"></param>要输出的模态数量
        /// <param name="regionName"></param>AOI区域名称
        /// <returns></returns>
        private string SaveEOFEigvectorToFile(string outDir, double[,] eigvector, int outCount, string regionName)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            List<string> outFileList = new List<string>();
            string headerformat = "4.EOF分解_特征向量_{0}_{1}.txt";
            string outHeader = Path.Combine(outDir, string.Format(headerformat, regionName, _eofAnaBandName));
            string lineHeader = "第{0}主分量 ";
            int width = eigvector.GetLength(0);//空间点数
            using (StreamWriter sw = new StreamWriter(outHeader, false, Encoding.Default))
            {
                for (int m = 0; m < outCount; m++)//
                {
                    string lineName = string.Format(lineHeader, m + 1) + ":";
                    sw.Write(lineName);
                    for (int i = 0; i < width; i++)//特征向量矩阵的每一列为每一个特征模态
                    {
                        sw.Write(eigvector[i, m]);
                        sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
            if (StatRegionSet.UseVectorAOIRegion)
            {
                CoordEnvelope env = StatRegionSet.SubRegionEnv;
                Size outsize = StatRegionSet.SubRegionOutSize;
                int samples = outsize.Width;
                int lines = outsize.Height;
                int[] outIndex = StatRegionSet.SubRegionOutIndex;
                string outFnameformat ="EOF分解_特征向量_{0}_{1}_{2}.dat";
                string outfname, lineName;
                double[] databuffer = new double[samples * lines];
                for (int m = 0; m < outCount; m++)
                {
                    lineName = string.Format(lineHeader, m + 1);
                    outfname = Path.Combine(outDir, string.Format(outFnameformat, regionName, _eofAnaBandName, lineName));
                    using (IRasterDataProvider dataPrd = CreateOutputRaster(outfname, env, outsize))
                    {
                        for (int oRow = 0; oRow < outIndex.Length; oRow++)
                        {
                            int no = outIndex[oRow];
                            databuffer[no] = eigvector[oRow, m];//i列为A矩阵第i模态
                        }
                        unsafe
                        {
                            fixed (double* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                dataPrd.GetRasterBand(1).Write(0, 0, samples, lines, buffer, enumDataType.Double, samples, lines);
                            }
                        }
                    }
                }
            }
            else
            {
                CoordEnvelope env = StatRegionSet.SubRegionEnv;
                Size outsize = StatRegionSet.SubRegionOutSize;
                int samples = outsize.Width;
                int lines = outsize.Height;
                string outFnameformat = "EOF分解_特征向量_{0}_{1}_{2}.dat";
                string outfname, lineName;
                int alls = samples * lines;
                double[] databuffer = new double[alls];
                for (int m = 0; m < outCount; m++)
                {
                    lineName = string.Format(lineHeader, m + 1);
                    outfname = Path.Combine(outDir, string.Format(outFnameformat, regionName, _eofAnaBandName, lineName));
                    using (IRasterDataProvider dataPrd = CreateOutputRaster(outfname, env, outsize))
                    {
                        for (int oRow = 0; oRow < alls; oRow++)
                        {
                            databuffer[oRow] = eigvector[oRow, m];//i列为A矩阵第i模态,m
                        }
                        unsafe
                        {
                            fixed (double* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                dataPrd.GetRasterBand(1).Write(0, 0, samples, lines, buffer, enumDataType.Double, samples, lines);
                            }
                        }
                    }
                }
            }
            return outHeader;
        }

        #endregion

    }
}
