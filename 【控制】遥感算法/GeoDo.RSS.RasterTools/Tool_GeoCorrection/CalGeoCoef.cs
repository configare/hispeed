using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    /// <summary>
    /// 计算多项式校正系数
    /// </summary>
    public class CalGeoCoef
    {
        private void agaus(ref double[] a,ref double[] b,int n)
        {
            int l, k, i, j, is_, p, q;
            is_ = 0;
            double d, t;
            int[] js = new int[n];
            l = 1;
            for (k = 0; k <= n - 2; k++)
            {
                d = 0.0;
                for (i = k; i <= n - 1; i++)
                    for (j = k; j <= n - 1; j++)
                    {
                        t = Math.Abs(a[i * n + j]);
                        if (t > d)
                        {
                            d = t;
                            js[k] = j;
                            is_ = i;
                        }
                    }
                if (d + 1.0 == 1.0)
                    l = 0;
                else
                {
                    if (js[k] != k)
                        for (i = 0; i <= n - 1; i++)
                        {
                            p = i * n + k;
                            q = i * n + js[k];
                            t = a[p];
                            a[p] = a[q];
                            a[q] = t;
                        }
                    if (is_ != k)
                    {
                        for (j = k; j <= n - 1; j++)
                        {
                            p = k * n + j; q = is_ * n + j;
                            t = a[p]; a[p] = a[q]; a[q] = t;
                        }
                        t = b[k]; b[k] = b[is_]; b[is_] = t;
                    }
                }
                if (l == 0)
                    return;
                d = a[k * n + k];
                for (j = k + 1; j <= n - 1; j++)
                { p = k * n + j; a[p] = a[p] / d; }
                b[k] = b[k] / d;
                for (i = k + 1; i <= n - 1; i++)
                {
                    for (j = k + 1; j <= n - 1; j++)
                    {
                        p = i * n + j;
                        a[p] = a[p] - a[i * n + k] * a[k * n + j];
                    }
                    b[i] = b[i] - a[i * n + k] * b[k];
                }
            }
            d = a[(n - 1) * n + n - 1];
            if (Math.Abs(d) + 1.0 == 1.0)
                return;
            b[n - 1] = b[n - 1] / d;
            for (i = n - 2; i >= 0; i--)
            {
                t = 0.0;
                for (j = i + 1; j <= n - 1; j++)
                    t = t + a[i * n + j] * b[j];
                b[i] = b[i] - t;
            }
            js[n - 1] = n - 1;
            for (k = n - 1; k >= 0; k--)
                if (js[k] != k)
                { t = b[k]; b[k] = b[js[k]]; b[js[k]] = t; }
        }


        private void EqNorm(double[] a, int n, double b, ref double[] aa, ref double[] ab, double p)
        {
            int i, j;

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                    aa[i*n+j] += p * a[i] * a[j];

                ab[i] += p * a[i] * b;
            }
        }

        public void PolyCoef1(double[] xOrigin, double[] yOrigin, double[] xBase, double[] yBase, int nCount, out double[] xCoef, out double[] yCoef, out double[] xRMS, out double[] yRMS)
        {

            double[] temp1 = new double[3];
            double[] temp2 = new double[9];
            double[] tempCoef = new double[3];

            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                EqNorm(temp1, 3, xOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 3);
            xCoef = (double[])tempCoef.Clone();

            temp2 = new double[9];
            tempCoef = new double[3];
            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                EqNorm(temp1, 3, yOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 3);
            yCoef = (double[])tempCoef.Clone();

            xRMS = new double[nCount];
            yRMS = new double[nCount];
            for (int i = 0; i < nCount; i++)
                xRMS[i] = (xCoef[0] + xCoef[1] * xBase[i] + xCoef[2] * yBase[i]) - xOrigin[i];
            for (int i = 0; i < nCount; i++)
                yRMS[i] = (yCoef[0] + yCoef[1] * xBase[i] + yCoef[2] * yBase[i]) - yOrigin[i];
        }

        public void PolyCoef1(double[] xOrigin, double[] yOrigin, double[] xBase, double[] yBase, int nCount, out double[] xCoef, out double[] yCoef)
        {

            double[] temp1 = new double[3];
            double[] temp2 = new double[9];
            double[] tempCoef = new double[3];

            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                EqNorm(temp1, 3, xOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 3);
            xCoef = (double[])tempCoef.Clone();

            temp2 = new double[9];
            tempCoef = new double[3];
            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                EqNorm(temp1, 3, yOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 3);
            yCoef = (double[])tempCoef.Clone();
        }

        public void PolyCoef2(double[] xOrigin, double[] yOrigin, double[] xBase, double[] yBase, int nCount, out double[] xCoef, out double[] yCoef, out double[] xRMS, out double[] yRMS)
        {

            double[] temp1 = new double[6];
            double[] temp2 = new double[36];
            double[] tempCoef = new double[6];

            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                temp1[3] = xBase[i] * xBase[i];
                temp1[4] = xBase[i] * yBase[i];
                temp1[5] = yBase[i] * yBase[i];
                EqNorm(temp1, 6, xOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 6);
            xCoef = (double[])tempCoef.Clone();

            temp2 = new double[36];
            tempCoef = new double[6];
            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                temp1[3] = xBase[i] * xBase[i];
                temp1[4] = xBase[i] * yBase[i];
                temp1[5] = yBase[i] * yBase[i];
                EqNorm(temp1, 6, yOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 6);
            yCoef = (double[])tempCoef.Clone();

            xRMS = new double[nCount];
            yRMS = new double[nCount];
            for (int i = 0; i < nCount; i++)
            {
                xRMS[i] = (xCoef[0] +
                        xCoef[1] * xBase[i] + xCoef[2] * yBase[i] +
                        xCoef[3] * xBase[i] * xBase[i] + xCoef[4] * xBase[i] * yBase[i] + xCoef[5] * yBase[i] * yBase[i]) - xOrigin[i];
            }
            for (int i = 0; i < nCount; i++)
            {
                yRMS[i] = (yCoef[0] +
                        yCoef[1] * xBase[i] + yCoef[2] * yBase[i] +
                        yCoef[3] * xBase[i] * xBase[i] + yCoef[4] * xBase[i] * yBase[i] + yCoef[5] * yBase[i] * yBase[i]) - yOrigin[i];
            }
        }

        public void PolyCoef2(double[] xOrigin, double[] yOrigin, double[] xBase, double[] yBase, int nCount, out double[] xCoef, out double[] yCoef)
        {

            double[] temp1 = new double[6];
            double[] temp2 = new double[36];
            double[] tempCoef = new double[6];

            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                temp1[3] = xBase[i] * xBase[i];
                temp1[4] = xBase[i] * yBase[i];
                temp1[5] = yBase[i] * yBase[i];
                EqNorm(temp1, 6, xOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 6);
            xCoef = (double[])tempCoef.Clone();

            temp2 = new double[36];
            tempCoef = new double[6];
            for (int i = 0; i < nCount; i++)
            {
                temp1[0] = 1.0;
                temp1[1] = xBase[i];
                temp1[2] = yBase[i];
                temp1[3] = xBase[i] * xBase[i];
                temp1[4] = xBase[i] * yBase[i];
                temp1[5] = yBase[i] * yBase[i];
                EqNorm(temp1, 6, yOrigin[i], ref temp2, ref tempCoef, 1.0);
            }
            agaus(ref temp2, ref tempCoef, 6);
            yCoef = (double[])tempCoef.Clone();
        }
    }
}
