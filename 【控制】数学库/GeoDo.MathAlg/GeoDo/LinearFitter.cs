using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.MathAlg
{
    public abstract class LinearFitter<T>:CurveFitObject
    {
        public void Fit(T[] xs, T[] ys, LinearFitObject fitObj)
        {
            if (xs == null || ys == null)
                throw new ArgumentNullException("xs或ys");
            if (xs.Length != ys.Length || xs.Length == 0)
                throw new ArgumentException("xs.Length为0 或者 xs与ys长度不一致!");
            int n = xs.Length;
            //Σ is E
            double Exy = 0, Ex = 0, Ey = 0,Ex2,Ey2;//Σx^2
            ComputeVars(xs, ys, out Exy, out Ex, out Ey,out Ex2,out Ey2);
            //
            fitObj.b = (n * Exy - Ex * Ey) / (n * Ex2 - Ex * Ex);
            fitObj.a = Ey / n - fitObj.b * Ex / n;
            //
            fitObj.r2 = (Math.Pow((n * Exy - Ex * Ey), 2)) / ((n * Ex2 - Ex * Ex) * (n * Ey2 - Ey * Ey));
        }

        protected abstract void ComputeVars(T[] xs, T[] ys, out double Exy, out double Ex, out double Ey,out double Ex2,out double Ey2);
    }

    public class LinearFitterByte : LinearFitter<byte>
    {
        protected override unsafe void ComputeVars(byte[] xs, byte[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 =0;
            int n = xs.Length;
            fixed (byte* xPointer = xs, yPointer = ys)
            {
                byte* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++,xPtr ++,yPtr ++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterInt16 : LinearFitter<Int16>
    {
        protected override unsafe void ComputeVars(Int16[] xs, Int16[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (Int16* xPointer = xs, yPointer = ys)
            {
                Int16* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterUInt16 : LinearFitter<UInt16>
    {
        protected override unsafe void ComputeVars(UInt16[] xs, UInt16[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (UInt16* xPointer = xs, yPointer = ys)
            {
                UInt16* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterInt32 : LinearFitter<Int32>
    {
        protected override unsafe void ComputeVars(Int32[] xs, Int32[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (Int32* xPointer = xs, yPointer = ys)
            {
                Int32* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterUInt32 : LinearFitter<UInt32>
    {
        protected override unsafe void ComputeVars(UInt32[] xs, UInt32[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (UInt32* xPointer = xs, yPointer = ys)
            {
                UInt32* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterInt64 : LinearFitter<Int64>
    {
        protected override unsafe void ComputeVars(Int64[] xs, Int64[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (Int64* xPointer = xs, yPointer = ys)
            {
                Int64* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterFloat : LinearFitter<float>
    {
        protected override unsafe void ComputeVars(float[] xs, float[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (float* xPointer = xs, yPointer = ys)
            {
                float* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }

    public class LinearFitterDouble : LinearFitter<double>
    {
        protected override unsafe void ComputeVars(double[] xs, double[] ys, out double Exy, out double Ex, out double Ey, out double Ex2, out double Ey2)
        {
            Exy = Ex = Ey = Ex2 = Ey2 = 0;
            int n = xs.Length;
            fixed (double* xPointer = xs, yPointer = ys)
            {
                double* xPtr = xPointer, yPtr = yPointer;
                for (int i = 0; i < n; i++, xPtr++, yPtr++)
                {
                    Exy += (*xPtr * *yPtr);
                    Ex += *xPtr;
                    Ey += *yPtr;
                    Ex2 += (*xPtr * *xPtr);
                    Ey2 += (*yPtr * *yPtr);
                }
            }
        }
    }
}
