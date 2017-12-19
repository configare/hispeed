using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.IO;
using GeoDo.Project;

namespace GeoDo.RSS.RasterTools
{
    public class PolyCorrection
    {
        private int _polyOrder = 1;
        //private string _inFileName;
        //private string _outFileName;

        public CoordEnvelope CalCorrectExt(int polyOrder, int width, int height, double[] coefX, double[] coefY)
        {
            double[] CorrectX = new double[4];
            double[] CorrectY = new double[4];
            
            if (polyOrder == 1)
            {
                if ((coefX.Length != 3) || (coefY.Length != 3))
                {
                    return null;
                }
                CorrectX[0] = coefX[0] + coefX[1] * 0 + coefX[2] * 0;
                CorrectX[1] = coefX[0] + coefX[1] * 0 + coefX[2] * height;
                CorrectX[2] = coefX[0] + coefX[1] * width + coefX[2] * 0;
                CorrectX[3] = coefX[0] + coefX[1] * width + coefX[2] * height;

                CorrectY[0] = coefY[0] + coefY[1] * 0 + coefY[2] * 0;
                CorrectY[1] = coefY[0] + coefY[1] * 0 + coefY[2] * height;
                CorrectY[2] = coefY[0] + coefY[1] * width + coefY[2] * 0;
                CorrectY[3] = coefY[0] + coefY[1] * width + coefY[2] * height;
            }
            else if (polyOrder == 2)
            {
                if ((coefX.Length != 6) || (coefY.Length != 6))
                {
                    return null;
                }
                CorrectX[0] = coefX[0] + coefX[1] * 0 + coefX[2] * 0 + coefX[3] * 0 * 0 + coefX[4] * 0 * 0 + coefX[5] * 0 * 0;
                CorrectX[1] = coefX[0] + coefX[1] * 0 + coefX[2] * height + coefX[3] * 0 * 0 + coefX[4] * 0 * height + coefX[5] * height * height;
                CorrectX[2] = coefX[0] + coefX[1] * width + coefX[2] * 0 + coefX[3] * width * width + coefX[4] * width * 0 + coefX[5] * 0 * 0;
                CorrectX[3] = coefX[0] + coefX[1] * width + coefX[2] * height + coefX[3] * width * width + coefX[4] * width * height + coefX[5] * height * height;

                CorrectY[0] = coefY[0] + coefY[1] * 0 + coefY[2] * 0 + coefY[3] * 0 * 0 + coefY[4] * 0 * 0 + coefY[5] * 0 * 0;
                CorrectY[1] = coefY[0] + coefY[1] * 0 + coefY[2] * height + coefY[3] * 0 * 0 + coefY[4] * 0 * height + coefY[5] * height * height;
                CorrectY[2] = coefY[0] + coefY[1] * width + coefY[2] * 0 + coefY[3] * width * width + coefY[4] * width * 0 + coefY[5] * 0 * 0;
                CorrectY[3] = coefY[0] + coefY[1] * width + coefY[2] * height + coefY[3] * width * width + coefY[4] * width * height + coefY[5] * height * height;
            }

            CoordEnvelope coordEnv = new CoordEnvelope(CorrectX.Min(), CorrectX.Max(), CorrectY.Min(), CorrectY.Max());
            return coordEnv;
        }

        public void DoPolyCorrection(string inImage, string outImage, int coordType, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, string baseSpatialRef, string driver, Action<int, string> progressCallback)
        {
            if (progressCallback != null)
                progressCallback(0, "开始几何精校正");

            using (IRasterDataProvider prd = GeoDataDriver.Open(inImage) as IRasterDataProvider)
            {
                int nHeight = prd.Height;
                int nWidth = prd.Width;
                int nBandCount = prd.BandCount;
                enumDataType dataType = prd.DataType;
                double inResX = prd.ResolutionX;
                double inResY = prd.ResolutionY;
                double outResX, outResY;
                string outSpatialRef = "";
                outResX = outResY = 0.0;
                outSpatialRef = baseSpatialRef;

                CoordEnvelope CoordEnv = CalCorrectExt(_polyOrder, nWidth, nHeight, coefX, coefY);
                switch (coordType)
                {
                    case 0:
                        outResX = 1;
                        outResY = 1;
                        break;
                    case 1:
                    case 2:
                        outResX = CoordEnv.Width / nWidth;
                        outResY = CoordEnv.Height / nHeight;
                        break;
                    case 3:
                        outResX = 1;
                        outResY = 1;
                        break;
                    case 4:
                        outResX = inResX;
                        outResY = inResY;
                        break;
                    case 5:
                        outResX = inResX * 100000;
                        outResY = inResY * 100000;
                        break;
                    case 6:
                        outResX = 1;
                        outResY = 1;
                        break;
                    case 7:
                        outResX = CoordEnv.Width / nWidth;
                        outResY = CoordEnv.Height / nHeight;
                        break;
                    case 8:
                        outResX = inResX;
                        outResY = inResY;
                        break;
                }

                int outWidth = Convert.ToInt32(CoordEnv.Width / outResX);
                int outHeight = Convert.ToInt32(CoordEnv.Height / outResY);

                if (progressCallback != null)
                    progressCallback(1, "几何精校正完成" + 1 + "%");

                string[] optionString = null;

                optionString = GetOptions(outImage, outResX, outResY, outSpatialRef, CoordEnv, out driver);
                IRasterDataDriver rasDestDriver = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
                using (IRasterDataProvider rasDestPrd = rasDestDriver.Create(outImage, outWidth, outHeight, nBandCount, prd.DataType, optionString))
                {

                    if (progressCallback != null)
                        progressCallback(2, "几何精校正完成" + 2 + "%");

                    switch (dataType)
                    {
                        case enumDataType.Byte:
                            Byte_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.Int16:
                            Int16_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.Int32:
                            Int32_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.Int64:
                            Int64_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.UInt16:
                            UInt16_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.UInt32:
                            UInt32_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.UInt64:
                            UInt64_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.Float:
                            Float_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                        case enumDataType.Double:
                            Double_PolyCorrection(prd, rasDestPrd, outWidth, outHeight, nBandCount, coefX, coefY, RcoefX, RcoefY, outResX, outResY, nWidth, nHeight, CoordEnv, progressCallback);
                            break;
                    }
                }
            }
            if (progressCallback != null)
                progressCallback(100, "几何精校正完成");
        }

        private string[] GetOptions(string outImage, double outResX, double outResY, string outSpatialRef, CoordEnvelope CoordEnv, out string driver)
        {
            driver = "LDF";
            string ext = Path.GetExtension(outImage).ToLower();
            switch (ext)
            {
                case ".ldf":
                    {
                        driver = "LDF";
                        string[] optionString = new string[]{
                        "INTERLEAVE=BSQ",
                        "VERSION=LDF",
                        "WITHHDR=TRUE",
                        "SPATIALREF=" + outSpatialRef,
                        "MAPINFO={" + 1 + "," + 1 + "}:{" + CoordEnv.MinX + "," + CoordEnv.MaxY + "}:{" + outResX + "," + outResY + "}"
                        };
                        return optionString;
                    }
                case ".tif":
                case ".tiff":
                    {
                        driver = "GDAL";
                        string[] optionString = new string[]{
                        "DRIVERNAME=GTiff",
                        "TFW=YES",
                        "WKT=" + SpatialReferenceFactory.GetSpatialReferenceByProj4String(outSpatialRef).ToWKTString(),
                        "GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}",CoordEnv.MinX, outResX,0, CoordEnv.MaxY,0, -outResY)
                        };
                        return optionString;
                    }
                default:
                    {
                        throw new Exception("暂不支持的输出数据格式");
                    }
            }
        }

        private unsafe void Double_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                       int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            double[] lineArray = new double[outWidth];
            double[] bandArray = new double[nWidth * nHeight];
            fixed (double* dPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(dPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Double, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Double, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        private unsafe void Float_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                               int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            float[] lineArray = new float[outWidth];
            float[] bandArray = new float[nWidth * nHeight];
            fixed (float* fPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(fPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Float, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Float, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }


        private unsafe void UInt64_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                               int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt64[] lineArray = new UInt64[outWidth];
            UInt64[] bandArray = new UInt64[nWidth * nHeight];
            fixed (UInt64* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.UInt64, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.UInt64, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        private unsafe void UInt32_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                               int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt32[] lineArray = new UInt32[outWidth];
            UInt32[] bandArray = new UInt32[nWidth * nHeight];
            fixed (UInt32* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.UInt32, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.UInt32, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        private unsafe void UInt16_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                                       int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            UInt16[] lineArray = new UInt16[outWidth];
            UInt16[] bandArray = new UInt16[nWidth * nHeight];
            fixed (UInt16* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.UInt16, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.UInt16, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        private unsafe void Int64_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                                       int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int64[] lineArray = new Int64[outWidth];
            Int64[] bandArray = new Int64[nWidth * nHeight];
            fixed (Int64* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Int64, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Int64, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }


        private unsafe void Int32_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                                        int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int32[] lineArray = new Int32[outWidth];
            Int32[] bandArray = new Int32[nWidth * nHeight];
            fixed (Int32* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Int32, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Int32, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        private unsafe void Int16_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                                                int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            Int16[] lineArray = new Int16[outWidth];
            Int16[] bandArray = new Int16[nWidth * nHeight];
            fixed (Int16* iPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(iPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Int16, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Int16, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }


        private unsafe void Byte_PolyCorrection(IRasterDataProvider prd, IRasterDataProvider rasDestPrd, int outWidth, int outHeight, int nBandCount, double[] coefX, double[] coefY, double[] RcoefX, double[] RcoefY, double outResX, double outResY,
                                                int nWidth, int nHeight, CoordEnvelope CoordEnv, Action<int, string> progressCallback)
        {
            byte[] lineArray = new byte[outWidth];
            byte[] bandArray = new byte[nWidth * nHeight];
            fixed (byte* bPtr = bandArray, linePtr = lineArray)
            {
                IntPtr buffer = new IntPtr(bPtr);
                IntPtr lineBuffer = new IntPtr(linePtr);
                for (int k = 1; k <= nBandCount; k++)
                {
                    prd.GetRasterBand(k).Read(0, 0, nWidth, nHeight, buffer, enumDataType.Byte, nWidth, nHeight);
                    for (int i = 0; i < outHeight; i++)
                    {
                        double crtY = CoordEnv.MaxY - outResY * i - outResY / 2;
                        for (int j = 0; j < outWidth; j++)
                        {
                            double crtX = CoordEnv.MinX + outResX * j + outResX / 2;
                            double originX = 0;
                            double originY = 0;
                            if (_polyOrder == 1)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY;
                            }
                            else if (_polyOrder == 2)
                            {
                                originX = RcoefX[0] + RcoefX[1] * crtX + RcoefX[2] * crtY + RcoefX[3] * crtX * crtX + RcoefX[4] * crtX * crtY + RcoefX[5] * crtY * crtY;
                                originY = RcoefY[0] + RcoefY[1] * crtX + RcoefY[2] * crtY + RcoefY[3] * crtX * crtX + RcoefY[4] * crtX * crtY + RcoefY[5] * crtY * crtY;
                            }

                            if ((originX < 0) || (originY < 0) || (originX > nWidth - 1) || (originY > nHeight - 1))
                            {
                                lineArray[j] = 0;
                            }
                            else
                            {
                                lineArray[j] = bandArray[Convert.ToInt32(originY) * nWidth + Convert.ToInt32(originX)];
                            }
                        }
                        rasDestPrd.GetRasterBand(k).Write(0, i, outWidth, 1, lineBuffer, enumDataType.Byte, outWidth, 1);

                        double dPercent = (2.0 + 100.0 * k * (i + 1)) / (outHeight * nBandCount + 2);
                        if (progressCallback != null)
                            progressCallback(Convert.ToInt32(dPercent), "几何精校正完成" + Convert.ToInt32(dPercent) + "%");
                    }
                }
            }
        }

        public int PolyOrder
        {
            get { return _polyOrder; }
            set
            {
                _polyOrder = value;
            }
        }




    }
}
