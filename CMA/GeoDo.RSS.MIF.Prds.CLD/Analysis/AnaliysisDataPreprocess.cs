#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：罗战克     时间：2014-2-18 17:25:19
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
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 创建日期：2014-2-18 17:25:19
    /// 根据气象场数据的特点，对气象场数据执行预处理
    /// 降分辨率采样读取规则：采样策略为对采样矩阵网格内数据取中值（即取出最具代表性的数值）
    /// </summary>
    public class AnaliysisDataPreprocess
    {
        /// <summary>
        /// 降分辨率，窗口取中值
        /// </summary>
        /// <param name="dns">输入观测值</param>
        /// <param name="valSize"></param>
        /// <param name="toSize"></param>
        /// <param name="todns">输出降维后的观测值</param>
        public static void MedianRead(short[] dns, Size valSize, Size toSize, out short[] todns)
        {
            double kx = valSize.Width * 1d / toSize.Width;
            double ky = valSize.Height * 1d / toSize.Height;
            int tolength = toSize.Width * toSize.Height;
            todns = new short[toSize.Width * toSize.Height];
            int[] x1 = new int[toSize.Width];
            int[] x2 = new int[toSize.Width];
            int[] y1 = new int[toSize.Height];
            int[] y2 = new int[toSize.Height];
            int[] w = new int[toSize.Width];
            int[] h = new int[toSize.Height];
            for (int i = 0; i < toSize.Width; i++)
            {
                x1[i] = (int)(kx * i + 0.5);
                x2[i] = (int)(kx * (i + 1) + 0.5);
                if (x1[i] < 0)
                    x1[i] = 0;
                if (x2[i] > valSize.Width)
                    x2[i] = valSize.Width;
                w[i] = x2[i] - x1[i];
            }
            for (int j = 0; j < toSize.Height; j++)
            {
                y1[j] = (int)(ky * j + 0.5);
                y2[j] = (int)(ky * (j + 1) + 0.5);
                if (y1[j] < 0)
                    y1[j] = 0;
                if (y2[j] > valSize.Height)
                    y2[j] = valSize.Height;
                h[j] = y2[j] - y1[j];
            }
            int windowsLength; //窗口大小
            short[] tmp = null;//窗口内数据
            int k;
            for (int i = 0; i < toSize.Width; i++)
            {
                for (int j = 0; j < toSize.Height; j++)
                {
                    windowsLength = w[i] * h[j];
                    tmp = new short[windowsLength];
                    k = 0;
                    for (int m = x1[i]; m < x2[i]; m++)
                    {
                        for (int n = y2[j]; n < y2[j]; n++)
                        {
                            tmp[k++] = dns[(m - x1[i]) + (n - y1[j]) * (w[i])];
                        }
                    }
                    //取中值算法
                    for (int m = 0; m < windowsLength / 2 + 1; ++m)
                    {
                        int min = m;
                        for (int n = m + 1; n < windowsLength; ++n)
                            if (tmp[n] < tmp[min])
                                min = n;
                        short temp = tmp[m];
                        tmp[m] = tmp[min];
                        tmp[min] = temp;
                    }
                    todns[i + j * toSize.Width] = tmp[windowsLength / 2];
                }
            }
        }

        public static void MedianRead(float[] dns, Size valSize, Size toSize, out float[] todns)
        {
            int srcWidth = valSize.Width;
            double kx = valSize.Width * 1d / toSize.Width;
            double ky = valSize.Height * 1d / toSize.Height;
            int tolength = toSize.Width * toSize.Height;
            todns = new float[toSize.Width * toSize.Height];
            int[] x1 = new int[toSize.Width];
            int[] x2 = new int[toSize.Width];
            int[] y1 = new int[toSize.Height];
            int[] y2 = new int[toSize.Height];
            int[] w = new int[toSize.Width];
            int[] h = new int[toSize.Height];
            for (int i = 0; i < toSize.Width; i++)
            {
                x1[i] = (int)(kx * i + 0.5);
                x2[i] = (int)(kx * (i + 1) + 0.5);
                if (x1[i] < 0)
                    x1[i] = 0;
                if (x2[i] > valSize.Width)
                    x2[i] = valSize.Width;
                w[i] = x2[i] - x1[i];
            }
            for (int j = 0; j < toSize.Height; j++)
            {
                y1[j] = (int)(ky * j + 0.5);
                y2[j] = (int)(ky * (j + 1) + 0.5);
                if (y1[j] < 0)
                    y1[j] = 0;
                if (y2[j] > valSize.Height)
                    y2[j] = valSize.Height;
                h[j] = y2[j] - y1[j];
            }
            int windowsLength; //窗口大小
            float[] tmp = null;//窗口内数据
            int k;
            for (int i = 0; i < toSize.Width; i++)
            {
                for (int j = 0; j < toSize.Height; j++)
                {
                    windowsLength = w[i] * h[j];
                    tmp = new float[windowsLength];
                    k = 0;
                    for (int m = x1[i]; m < x2[i]; m++)
                    {
                        for (int n = y1[j]; n < y2[j]; n++)
                        {
                            //tmp[k++] = dns[(m - x1[i]) + (n - y1[j]) * (w[i])];
                            tmp[k++] = dns[y1[j] * srcWidth + x1[i]];
                        }
                    }
                    //取中值算法
                    for (int m = 0; m < windowsLength / 2 + 1; ++m)
                    {
                        int min = m;
                        for (int n = m + 1; n < windowsLength; ++n)
                            if (tmp[n] < tmp[min])
                                min = n;
                        float temp = tmp[m];
                        tmp[m] = tmp[min];
                        tmp[min] = temp;
                    }
                    todns[i + j * toSize.Width] = tmp[windowsLength / 2];
                }
            }
        }

        public static void MedianRead(Byte[] dns, Size valSize, Size toSize, out Byte[] todns)
        {
            int srcWidth = valSize.Width;
            double kx = valSize.Width * 1d / toSize.Width;
            double ky = valSize.Height * 1d / toSize.Height;
            int tolength = toSize.Width * toSize.Height;
            todns = new Byte[toSize.Width * toSize.Height];
            int[] x1 = new int[toSize.Width];
            int[] x2 = new int[toSize.Width];
            int[] y1 = new int[toSize.Height];
            int[] y2 = new int[toSize.Height];
            int[] w = new int[toSize.Width];
            int[] h = new int[toSize.Height];
            for (int i = 0; i < toSize.Width; i++)
            {
                x1[i] = (int)(kx * i + 0.5);
                x2[i] = (int)(kx * (i + 1) + 0.5);
                if (x1[i] < 0)
                    x1[i] = 0;
                if (x2[i] > valSize.Width)
                    x2[i] = valSize.Width;
                w[i] = x2[i] - x1[i];
            }
            for (int j = 0; j < toSize.Height; j++)
            {
                y1[j] = (int)(ky * j + 0.5);
                y2[j] = (int)(ky * (j + 1) + 0.5);
                if (y1[j] < 0)
                    y1[j] = 0;
                if (y2[j] > valSize.Height)
                    y2[j] = valSize.Height;
                h[j] = y2[j] - y1[j];
            }
            int windowsLength; //窗口大小
            Byte[] tmp = null;//窗口内数据
            int k;
            for (int i = 0; i < toSize.Width; i++)
            {
                for (int j = 0; j < toSize.Height; j++)
                {
                    windowsLength = w[i] * h[j];
                    tmp = new Byte[windowsLength];
                    k = 0;
                    for (int m = x1[i]; m < x2[i]; m++)
                    {
                        for (int n = y1[j]; n < y2[j]; n++)
                        {
                            //tmp[k++] = dns[(m - x1[i]) + (n - y1[j]) * (w[i])];
                            tmp[k++] = dns[y1[j] * srcWidth + x1[i]];
                        }
                    }
                    //取中值算法
                    for (int m = 0; m < windowsLength / 2 + 1; ++m)
                    {
                        int min = m;
                        for (int n = m + 1; n < windowsLength; ++n)
                            if (tmp[n] < tmp[min])
                                min = n;
                        Byte temp = tmp[m];
                        tmp[m] = tmp[min];
                        tmp[min] = temp;
                    }
                    todns[i + j * toSize.Width] = tmp[windowsLength / 2];
                }
            }
        }

        public static void MedianRead(double[] dns, Size valSize, Size toSize, out double[] todns)
        {
            int srcWidth = valSize.Width;
            double kx = valSize.Width * 1d / toSize.Width;
            double ky = valSize.Height * 1d / toSize.Height;
            int tolength = toSize.Width * toSize.Height;
            todns = new double[toSize.Width * toSize.Height];
            int[] x1 = new int[toSize.Width];
            int[] x2 = new int[toSize.Width];
            int[] y1 = new int[toSize.Height];
            int[] y2 = new int[toSize.Height];
            int[] w = new int[toSize.Width];
            int[] h = new int[toSize.Height];
            for (int i = 0; i < toSize.Width; i++)
            {
                x1[i] = (int)(kx * i + 0.5);
                x2[i] = (int)(kx * (i + 1) + 0.5);
                if (x1[i] < 0)
                    x1[i] = 0;
                if (x2[i] > valSize.Width)
                    x2[i] = valSize.Width;
                w[i] = x2[i] - x1[i];
            }
            for (int j = 0; j < toSize.Height; j++)
            {
                y1[j] = (int)(ky * j + 0.5);
                y2[j] = (int)(ky * (j + 1) + 0.5);
                if (y1[j] < 0)
                    y1[j] = 0;
                if (y2[j] > valSize.Height)
                    y2[j] = valSize.Height;
                h[j] = y2[j] - y1[j];
            }
            int windowsLength; //窗口大小
            double[] tmp = null;//窗口内数据
            int k;
            for (int i = 0; i < toSize.Width; i++)
            {
                for (int j = 0; j < toSize.Height; j++)
                {
                    windowsLength = w[i] * h[j];
                    tmp = new double[windowsLength];
                    k = 0;
                    for (int m = x1[i]; m < x2[i]; m++)
                    {
                        for (int n = y1[j]; n < y2[j]; n++)
                        {
                            //tmp[k++] = dns[(m - x1[i]) + (n - y1[j]) * (w[i])];
                            tmp[k++] = dns[y1[j] * srcWidth + x1[i]];
                        }
                    }
                    //取中值算法
                    for (int m = 0; m < windowsLength / 2 + 1; ++m)
                    {
                        int min = m;
                        for (int n = m + 1; n < windowsLength; ++n)
                            if (tmp[n] < tmp[min])
                                min = n;
                        double temp = tmp[m];
                        tmp[m] = tmp[min];
                        tmp[min] = temp;
                    }
                    todns[i + j * toSize.Width] = tmp[windowsLength / 2];
                }
            }
        }

        /// <summary>
        /// 距平场时间序列：对时序场矩阵数据，执行均差处理
        /// </summary>
        /// <param name="dns"></param>
        public static void MeanDeviation(short[] dns)
        {
            Int64 sum = 0;
            double mean = 0;
            //List<short>.Sum();
            //dns.Sum()
            for (int j = 0; j < dns.Length; j++)
            {
                sum += dns[j];
            }
            mean = (double)sum / dns.Length;
            for (int j = 0; j < dns.Length; j++)
                dns[j] = (short)(dns[j] - mean);
        }

        public static void MeanDeviation(float[] dns)
        {
            double sum = 0;
            double mean = 0;
            for (int j = 0; j < dns.Length; j++)
            {
                sum += dns[j];
            }
            mean = sum / dns.Length;
            for (int j = 0; j < dns.Length; j++)
                dns[j] = (float)(dns[j] - mean);
        }

        public static void MeanDeviation(double[] dns)
        {
            double sum = 0;
            double mean = 0;
            for (int j = 0; j < dns.Length; j++)  //时间维
            {
                sum += dns[j];
            }
            mean = sum / dns.Length;
            for (int j = 0; j < dns.Length; j++)  //时间维
                dns[j] = (dns[j] - mean);
        }



        /// <summary>
        /// 标准化的时间序列
        /// 中心化处理：处理后的数据，均值为0，方差为1
        /// X_t = X_t-Mean(X)/S_x;            (均方差S_x)
        /// S_x = Sqrt(∑(t=1,n)((X_t-Mean)^2))
        /// </summary>
        /// <param name="dns"></param>
        /// <param name="invalids"></param>
        /// <param name="sds"></param>
        public static void StandardDeviation(short[] dns, out double[] sds)
        {
            double sum = 0;                 //样本值求和
            double mean = 0;                //样本值求平均值
            double sumMeanDeviation = 0;    //距平值求平方和
            int dnCount = dns.Length;       //样本个数
            sds = new double[dnCount];
            for (int i = 0; i < dnCount; i++)
            {
                sum += dns[i];
            }
            mean = sum / dnCount;
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = ((dns[i] - mean) * (dns[i] - mean));    //距平，平方
                sumMeanDeviation += ((dns[i] - mean) * (dns[i] - mean));//距平，平方和
            }
            double sqrtS = Math.Sqrt(sumMeanDeviation / (dnCount));
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = sds[i] / sqrtS;
            }
        }

        public static void StandardDeviation(float[] dns, out double[] sds)
        {
            double sum = 0;                 //样本值求和
            double mean = 0;                //样本值求平均值
            double sumMeanDeviation = 0;    //距平值求平方和
            int dnCount = dns.Length;       //样本个数
            sds = new double[dnCount];
            for (int i = 0; i < dnCount; i++)
            {
                sum += dns[i];
            }
            mean = sum / dnCount;
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = (dns[i] - mean);    //距平，平方
                sumMeanDeviation += ((dns[i] - mean) * (dns[i] - mean));//距平，平方和
            }
            double sqrtS = Math.Sqrt(sumMeanDeviation / (dnCount));
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = sds[i] / sqrtS;
            }
        }

        public static void StandardDeviation(double[] dns, out double[] sds)
        {
            double sum = 0;                 //样本值求和
            double mean = 0;                //样本值求平均值
            double sumMeanDeviation = 0;    //距平值求平方和
            int dnCount = dns.Length;       //样本个数
            sds = new double[dnCount];
            for (int i = 0; i < dnCount; i++)
            {
                sum += dns[i];
            }
            mean = sum / dnCount;
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = (dns[i] - mean);    //距平，平方
                sumMeanDeviation += ((dns[i] - mean) * (dns[i] - mean));//距平，平方和
            }
            double sqrtS = Math.Sqrt(sumMeanDeviation / (dnCount));
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = sds[i] / sqrtS;
            }
        }

        public static void StandardDeviation(Byte[] dns, out double[] sds)
        {
            double sum = 0;                 //样本值求和
            double mean = 0;                //样本值求平均值
            double sumMeanDeviation = 0;    //距平值求平方和
            int dnCount = dns.Length;       //样本个数
            sds = new double[dnCount];
            for (int i = 0; i < dnCount; i++)
            {
                sum += dns[i];
            }
            mean = sum / dnCount;
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = ((dns[i] - mean) * (dns[i] - mean));    //距平，平方
                sumMeanDeviation += ((dns[i] - mean) * (dns[i] - mean));//距平，平方和
            }
            double sqrtS = Math.Sqrt(sumMeanDeviation / (dnCount));
            for (int i = 0; i < dnCount; i++)
            {
                sds[i] = sds[i] / sqrtS;
            }
        }


        /// <summary>
        /// 计算方差
        /// </summary>
        /// <param name="dns"></param>
        /// <returns></returns>
        public static double StandardDeviationCalc(double[] dns)
        {
            double sum = 0;                 //样本值求和
            double mean = 0;                //样本值求平均值
            double sumMeanDeviation = 0;    //距平值求平方和
            int dnCount = dns.Length;       //样本个数
            for (int i = 0; i < dnCount; i++)
            {
                sum += dns[i];
            }
            mean = sum / dnCount;
            for (int i = 0; i < dnCount; i++)
            {
                sumMeanDeviation += ((dns[i] - mean) * (dns[i] - mean));
            }
            return Math.Sqrt(sumMeanDeviation / dnCount);
        }

        /// <summary>
        /// 求u每一列向量的方差
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static double[] CalculateCSCF(double[,] u)
        {
            int r = u.GetLength(0);
            int c = u.GetLength(1);
            double sum = 0;
            double mean = 0;
            double sumMeanDeviation = 0;
            double[] ret = new double[c];
            for (int j = 0; j < c; j++) //遍历每一列
            {
                sum = 0;
                mean = 0;
                sumMeanDeviation = 0;
                for (int i = 0; i < r; i++)
                {
                    sum += u[i, j];
                }
                mean = sum / r;
                for (int i = 0; i < r; i++)
                {
                    sumMeanDeviation += ((u[i, j] - mean) * (u[i, j] - mean));
                }
                ret[j] = sumMeanDeviation / r;
            }
            return ret;
        }

        /// <summary>
        /// 计算数组的累计值
        /// </summary>
        /// <param name="scf"></param>
        /// <returns></returns>
        public static double[] CalcCSCF(double[] scf)
        {
            double[] cscf = new double[scf.Length];
            double t = 0;
            for (int i = 0; i < scf.Length; i++)
            {
                cscf[i] = (scf[i] + t);
                t = cscf[i];
            }
            return cscf;
        }

        /// <summary>
        /// 根据中心化数据，计算出来的奇异值，计算每一个方差贡献
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double[] CalcSCF(double[] w)
        {
            double[] scf = new double[w.Length];
            double sum = 0;
            for (int i = 0; i < w.Length; i++)
            {
                sum += w[i];
            }
            for (int i = 0; i < w.Length; i++)
            {
                scf[i] = w[i] / sum;
            }
            return scf;
        }

        /// <summary>
        /// 累计百分值超过MaxPercent的索引号
        /// </summary>
        /// <param name="cscf">累计百分值</param>
        /// <param name="MaxPercent"></param>
        /// <returns></returns>
        internal static int GetMaxCSCFCount(double[] cscf, double MaxPercent)
        {
            int maxlen = cscf.Length;
            for (int i = 0; i < maxlen; i++)
            {
                if (cscf[i] >= MaxPercent)
                {
                    return (i + 1) <= maxlen ? i + 1:maxlen;
                }
            }
            return maxlen;
        }
    }
}
