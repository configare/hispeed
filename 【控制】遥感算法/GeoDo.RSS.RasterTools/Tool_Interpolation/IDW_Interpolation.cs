#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/5 20:38:55
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

namespace GeoDo.RSS.RasterTools
{
    /// <summary>
    /// 类名：IDW_Interpolation
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/5 20:38:55
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    /// <summary>
    /// 反距离权重插值（Inverse Distance Weighted Interpolation）
    /// </summary>
    public class IDW_Interpolation
    {
        private double[] _coordPointXArr = null;
        private double[] _coordPointYArr = null;
        private double[] _pointValueArr = null;

        public double[] CoordPointXArr
        {
            get { return _coordPointXArr; }
            set
            {
                _coordPointXArr = value;
            }
        }

        public double[] CoordPointYArr
        {
            get { return _coordPointYArr; }
            set
            {
                _coordPointYArr = value;
            }
        }

        public double[] PointValueArr
        {
            get { return _pointValueArr; }
            set
            {
                _pointValueArr = value;
            }
        }

        private double CalPointValue(double coordX, double coordY)
        {
            double distanceSum = 0.0;
            double pointValue = 0.0;

            int nCount = _pointValueArr.Count();
            double[] weightArr = new double[nCount];

            for (int i = 0; i < nCount; i++)
            {
                if (Math.Abs(coordX - _coordPointXArr[i]) <= double.Epsilon && Math.Abs(coordY - _coordPointYArr[i]) <= double.Epsilon)
                {
                    return _coordPointXArr[i];
                }

                double invDistance = 1.0 / ((coordX - _coordPointXArr[i]) * (coordX - _coordPointXArr[i]) + (coordY - _coordPointYArr[i]) * (coordY - _coordPointYArr[i]));
                weightArr[i] = invDistance;
                distanceSum = distanceSum + invDistance;
            }

            for (int i = 0; i < nCount; i++)
            {
                pointValue = pointValue + weightArr[i] * _pointValueArr[i] / distanceSum;
            }
            return pointValue;
        }

        public void DoIDWinterpolation(double resolutionX, double resolutionY, out int nWidth, out int nHeight, out double[] rasPointValue)
        {
            if ((_coordPointXArr == null) || (_coordPointYArr == null) || (_pointValueArr == null))
            {
                nWidth = nHeight = 0;
                rasPointValue = null;
                return;
            }

            CoordEnvelope CoordEnv = new CoordEnvelope(_coordPointXArr.Min(), _coordPointXArr.Max(), _coordPointYArr.Min(), _coordPointYArr.Max());
            nWidth = Convert.ToInt32(CoordEnv.Width / resolutionX);
            nHeight = Convert.ToInt32(CoordEnv.Height / resolutionY);
            rasPointValue = new double[nWidth * nHeight];

            for (int i = 0; i < nHeight; i++)
            {
                double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                for (int j = 0; j < nWidth; j++)
                {
                    double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                    rasPointValue[i * nWidth + j] = CalPointValue(crtX, crtY);
                }
            }
        }

        public void DoIDWinterpolation(double resolutionX, double resolutionY, string outImage, string driver, enumDataType dataType, Action<int, string> progressCallback, string spatialRef = null)
        {
            if ((_coordPointXArr == null) || (_coordPointYArr == null) || (_pointValueArr == null))
                return;

            if (progressCallback != null)
                progressCallback(0, "开始空间插值");

            CoordEnvelope CoordEnv = new CoordEnvelope(_coordPointXArr.Min(), _coordPointXArr.Max(), _coordPointYArr.Min(), _coordPointYArr.Max());
            int nWidth = Convert.ToInt32(CoordEnv.Width / resolutionX);
            int nHeight = Convert.ToInt32(CoordEnv.Height / resolutionY);

            string[] optionString = null;
            optionString = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + spatialRef,
                "MAPINFO={" + 1 + "," + 1 + "}:{" + CoordEnv.MinX + "," + CoordEnv.MaxY + "}:{" + resolutionX + "," + resolutionY + "}"
                };
            IRasterDataDriver rasDestDriver = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            IRasterDataProvider rasDestPrd = rasDestDriver.Create(outImage, nWidth, nHeight, 1, dataType, optionString);

            switch (dataType)
            {
                case enumDataType.Byte:
                    Byte_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.Int16:
                    Int16_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.Int32:
                    Int32_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.Int64:
                    Int64_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.UInt16:
                    UInt16_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.UInt32:
                    UInt32_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.UInt64:
                    UInt64_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.Float:
                    Float_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
                case enumDataType.Double:
                    Double_IDWinterpolation(rasDestPrd, resolutionX, resolutionY, nWidth, nHeight, CoordEnv, progressCallback);
                    break;
            }
            rasDestPrd.Dispose();

            if (progressCallback != null)
                progressCallback(100, "空间插值完成");
        }

        private unsafe void Double_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            double[] lineArray = new double[nWidth];
            fixed (double* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = CalPointValue(crtX, crtY);
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Double, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }
        private unsafe void Float_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            float[] lineArray = new float[nWidth];
            fixed (float* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToSingle(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Float, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void UInt64_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt64[] lineArray = new UInt64[nWidth];
            fixed (UInt64* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToUInt64(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.UInt64, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void UInt32_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt32[] lineArray = new UInt32[nWidth];
            fixed (UInt32* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToUInt32(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.UInt32, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void UInt16_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt16[] lineArray = new UInt16[nWidth];
            fixed (UInt16* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToUInt16(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.UInt16, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void Int64_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int64[] lineArray = new Int64[nWidth];
            fixed (Int64* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToInt64(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Int64, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void Int32_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int32[] lineArray = new Int32[nWidth];
            fixed (Int32* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToInt32(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Int32, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void Int16_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int16[] lineArray = new Int16[nWidth];
            fixed (Int16* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToInt16(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Int16, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }

        private unsafe void Byte_IDWinterpolation(IRasterDataProvider rasDestPrd, double resolutionX, double resolutionY, int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            byte[] lineArray = new byte[nWidth];
            fixed (byte* linePtr = lineArray)
            {
                IntPtr lineBuffer = new IntPtr(linePtr);

                for (int i = 0; i < nHeight; i++)
                {
                    double crtY = CoordEnv.MaxY - resolutionY * i - resolutionY / 2;
                    for (int j = 0; j < nWidth; j++)
                    {
                        double crtX = CoordEnv.MinX + resolutionX * j + resolutionX / 2;
                        lineArray[j] = Convert.ToByte(CalPointValue(crtX, crtY));
                    }
                    rasDestPrd.GetRasterBand(1).Write(0, i, nWidth, 1, lineBuffer, enumDataType.Byte, nWidth, 1);

                    double dPercent = 100.0 * (i + 1) / nHeight;
                    if (progressCallback != null)
                        progressCallback(Convert.ToInt32(dPercent), "空间插值完成" + Convert.ToInt32(dPercent) + "%");
                }
            }
        }
    }
}
