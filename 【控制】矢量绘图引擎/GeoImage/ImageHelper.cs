using System;
using System.Collections.Generic;
using System.Text;

namespace GeoVis.GeoCore
{
    public enum LDataType
    {
        L_Unknown = 0,
        /*! Eight bit unsigned integer */
        L_Byte = 1,
        /*! Sixteen bit unsigned integer */
        L_UInt16 = 2,
        /*! Sixteen bit signed integer */
        L_Int16 = 3,
        /*! Thirty two bit unsigned integer */
        L_UInt32 = 4,
        /*! Thirty two bit signed integer */
        L_Int32 = 5,
        /*! Thirty two bit floating point */
        L_Float32 = 6,
        /*! Sixty four bit floating point */
        L_Float64 = 7,
        /*! Complex Int16 */
        L_CInt16 = 8,
        /*! Complex Int32 */
        L_CInt32 = 9,
        /*! Complex Float32 */
        L_CFloat32 = 10,
        /*! Complex Float64 */
        L_CFloat64 = 11,
        L_TypeCount = 12,         /* maximum type # + 1 */
        L_RGB = 13,                /* 全色图像 */
        L_ARGB = 14                /* 带有透明度的全色图像 */
    }

    public enum LCompressFormat
    {
        LNone = 0,
        LJpeg = 1,
        LPng = 2,
        LZip = 3
        //dds/jpeg2000 test,bsq,bip,create 
    }

    enum LFRWFlag
    {
        /*! Read data */
        LF_Read = 0,
        /*! Write data */
        LF_Write = 1
    }

    class ImageHelper
    {

        public int Bands;
        public int BitsPerPixel;

        public ImageHelper(int bands, int bits)
        {
            Bands = bands;
            BitsPerPixel = bits;
        }

        #region sixteen2eight

        public static void ConvertTo8bits(byte[] src, int width, int height, int bands, LDataType dataType, byte[] dst)
        {
            switch (dataType)
            {
                case LDataType.L_Int16:
                case LDataType.L_UInt16:
                    Convert16To8(src, width, height, bands, dataType, dst);
                    break;
                case LDataType.L_Int32:
                    break;
                default:
                    break;
            }
        }

        private static unsafe void Convert16To8(byte[] src, int width, int height, int bands, LDataType dataType, byte[] dst)
        {
            int[] minPixel = new int[bands];
            int[] maxPixel = new int[bands];

            int size = width * height;

            for (int i = 0; i < bands; i++)
            {
                int[] hist = new int[65536];
                float tt = 0;
                fixed (void* p = src)
                {
                    int m_min = 65535000;
                    int m_max = -65535000;
                    switch (dataType)
                    {
                        case LDataType.L_Int16:
                            Int16* fp = (Int16*)p;
                            for (int k = 0; k < size; k++)
                            {
                                tt = fp[bands * k + i];
                                hist[(int)tt]++;
                                if (m_min > tt)
                                    m_min = (int)tt;
                                if (m_max < tt)
                                    m_max = (int)tt;
                            }
                            break;
                        case LDataType.L_UInt16:
                            UInt16* fp1 = (UInt16*)p;
                            for (int k = 0; k < size; k++)
                            {
                                tt = fp1[bands * k + i];
                                hist[(int)tt]++;
                                if (m_min > tt)
                                    m_min = (int)tt;
                                if (m_max < tt)
                                    m_max = (int)tt;
                            }
                            break;
                        default:
                            break;
                    }

                    int sum = 0;

                    int sminSize = (int)((size - hist[0] - hist[65535]) * 0.015);
                    int low = 0;
                    for (low = m_min + 1; low < m_max; low++)
                    {
                        sum += hist[low];
                        if (sum > sminSize)
                            break;
                    }
                    minPixel[i] = low - 1;

                    sum = 0;
                    int j = 0;
                    for (j = m_max - 1; j > m_min; j--)
                    {
                        sum += hist[j];
                        if (sum > sminSize)
                            break;
                    }

                    maxPixel[i] = j + 1;
                }
            }

            #region 量化

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < bands; j++)
                {
                    ushort t = (ushort)((short)(src[i * bands * 2 + j * 2 + 1] << 8) + src[i * bands * 2 + j * 2]);

                    byte b = Clamp(t, minPixel[j], maxPixel[j]);
                    dst[i * bands + j] = b;
                }
            }


            #endregion

        }

        private static byte Clamp(ushort t, float min, float max)
        {
            if (t < min)
                t = (ushort)min;
            if (t > max)
                t = 255;
            else
                t = (ushort)((float)(t - min) / (max
                    - min) * 255);
            return (byte)t;
        }

        #endregion

        #region Resample
        public double b3spline(double x)
        {
            if (x > 2.0)
                return 0.0;
            // thanks to Kristian Kratzenstein
            double a, b, c, d;
            double xm1 = x - 1.0; // Was calculatet anyway cause the "if((x-1.0f) < 0)"
            double xp1 = x + 1.0;
            double xp2 = x + 2.0;

            if ((xp2) <= 0.0f) a = 0.0f; else a = xp2 * xp2 * xp2; // Only float, not float -> double -> float
            if ((xp1) <= 0.0f) b = 0.0f; else b = xp1 * xp1 * xp1;
            if (x <= 0) c = 0.0f; else c = x * x * x;
            if ((xm1) <= 0.0f) d = 0.0f; else d = xm1 * xm1 * xm1;

            return (0.16666666666666666667f * (a - (4.0 * b) + (6.0 * c) - (4.0 * d)));
        }

        public unsafe void BilinearResample(byte[] dstbyte, byte[] srcbyte, int dstW, int dstH, int srcW, int srcH)
        {
            int width = srcW;
            int height = srcH;

            int bytes = this.BitsPerPixel / 8 * Bands;
            int bytesPerBand = bytes;
            int bytePerLine = bytesPerBand * dstW;

            int pixelSize = bytes;
            int srcStride = bytes * srcW;
            int dstOffset = 0;
            double xFactor = (double)width / dstW;
            double yFactor = (double)height / dstH;

            fixed (byte* src = srcbyte)
            fixed (byte* pdst = dstbyte)
            {
                byte* dst = pdst;
                // coordinates of source points
                double ox, oy, dx1, dy1, dx2, dy2;
                int ox1, oy1, ox2, oy2;
                // width and height decreased by 1
                int ymax = height - 1;
                int xmax = width - 1;
                // temporary pointers
                byte* tp1, tp2;
                byte* p1, p2, p3, p4;

                // for each line
                for (int y = 0; y < dstH; y++)
                {
                    // Y coordinates
                    oy = (double)y * yFactor;
                    oy1 = (int)oy;
                    oy2 = (oy1 == ymax) ? oy1 : oy1 + 1;
                    dy1 = oy - (double)oy1;
                    dy2 = 1.0 - dy1;

                    // get temp pointers
                    tp1 = src + oy1 * srcStride;
                    tp2 = src + oy2 * srcStride;

                    // for each pixel
                    for (int x = 0; x < dstW; x++)
                    {
                        // X coordinates
                        ox = (double)x * xFactor;
                        ox1 = (int)ox;
                        ox2 = (ox1 == xmax) ? ox1 : ox1 + 1;
                        dx1 = ox - (double)ox1;
                        dx2 = 1.0 - dx1;

                        // get four points
                        p1 = tp1 + ox1 * pixelSize;
                        p2 = tp1 + ox2 * pixelSize;
                        p3 = tp2 + ox1 * pixelSize;
                        p4 = tp2 + ox2 * pixelSize;

                        // interpolate using 4 points
                        for (int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++)
                        {
                            *dst = (byte)(
                                dy2 * (dx2 * (*p1) + dx1 * (*p2)) +
                                dy1 * (dx2 * (*p3) + dx1 * (*p4)));
                        }
                    }
                    dst += dstOffset;
                }
            }
        }

        public unsafe void BicubicResample(byte[] dstbyte, byte[] srcbyte, int dstW, int dstH, int srcW, int srcH)
        {
            // get source image size
            int width = srcW;
            int height = srcH;
            int bytes = this.BitsPerPixel / 8;
            int bytesPerBand = bytes;
            int bytePerLine = bytesPerBand * dstW;

            int pixelSize = bytes;
            int srcStride = bytes * srcW;
            int dstOffset = 0;
            double xFactor = (double)width / dstW;
            double yFactor = (double)height / dstH;

            // do the job
            fixed (byte* src = srcbyte)
            fixed (byte* pdst = dstbyte)
            {
                byte* dst = pdst;
                // coordinates of source points and cooefficiens
                double ox, oy, dx, dy, k1, k2;
                int ox1, oy1, ox2, oy2;
                // destination pixel values
                double r, g, b;
                // width and height decreased by 1
                int ymax = height - 1;
                int xmax = width - 1;
                // temporary pointer
                byte* p;

                // check pixel format
                if (pixelSize == 1)
                {
                    // grayscale
                    for (int y = 0; y < dstH; y++)
                    {
                        // Y coordinates
                        oy = (double)y * yFactor - 0.5;
                        oy1 = (int)oy;
                        dy = oy - (double)oy1;

                        for (int x = 0; x < dstW; x++, dst++)
                        {
                            // X coordinates
                            ox = (double)x * xFactor - 0.5f;
                            ox1 = (int)ox;
                            dx = ox - (double)ox1;

                            // initial pixel value
                            g = 0;

                            for (int n = -1; n < 3; n++)
                            {
                                // get Y cooefficient
                                k1 = b3spline(dy - (double)n);

                                oy2 = oy1 + n;
                                if (oy2 < 0)
                                    oy2 = 0;
                                if (oy2 > ymax)
                                    oy2 = ymax;

                                for (int m = -1; m < 3; m++)
                                {
                                    // get X cooefficient
                                    k2 = k1 * b3spline((double)m - dx);

                                    ox2 = ox1 + m;
                                    if (ox2 < 0)
                                        ox2 = 0;
                                    if (ox2 > xmax)
                                        ox2 = xmax;

                                    g += k2 * src[oy2 * srcStride + ox2];
                                }
                            }
                            *dst = (byte)g;
                        }
                        dst += dstOffset;
                    }
                }
                else
                {
                    // RGB
                    for (int y = 0; y < dstH; y++)
                    {
                        // Y coordinates
                        oy = (double)y * yFactor - 0.5f;
                        oy1 = (int)oy;
                        dy = oy - (double)oy1;

                        for (int x = 0; x < dstW; x++, dst += 3)
                        {
                            // X coordinates
                            ox = (double)x * xFactor - 0.5f;
                            ox1 = (int)ox;
                            dx = ox - (double)ox1;

                            // initial pixel value
                            r = g = b = 0;

                            for (int n = -1; n < 3; n++)
                            {
                                // get Y cooefficient
                                k1 = b3spline(dy - (double)n);

                                oy2 = oy1 + n;
                                if (oy2 < 0)
                                    oy2 = 0;
                                if (oy2 > ymax)
                                    oy2 = ymax;

                                for (int m = -1; m < 3; m++)
                                {
                                    // get X cooefficient
                                    k2 = k1 * b3spline((double)m - dx);

                                    ox2 = ox1 + m;
                                    if (ox2 < 0)
                                        ox2 = 0;
                                    if (ox2 > xmax)
                                        ox2 = xmax;

                                    // get pixel of original image
                                    p = src + oy2 * srcStride + ox2 * 3;

                                    r += k2 * p[2];
                                    g += k2 * p[1];
                                    b += k2 * p[0];
                                }
                            }

                            dst[2] = (byte)r;
                            dst[1] = (byte)g;
                            dst[0] = (byte)b;
                        }
                        dst += dstOffset;
                    }
                }
            }
        }

        public void Resample(byte[] dst, byte[] src, int dstW, int dstH, int srcW, int srcH)
        {
            int x = 0, y = 0;
            double xscale, yscale, fx, fy;
            xscale = (double)srcW / (double)dstW;
            yscale = (double)srcH / (double)dstH;
            int bytes = this.BitsPerPixel / 8;
            int bytesPerBand = bytes;
            int bytePerLine = bytesPerBand * dstW;

          
            int count = 0;
            //memset(pData,0,size);
            for (int i = 0; i < dstH; i++)
            {
                fy = i * yscale + y;

                if (fy > srcH - 1)
                    fy = srcH - 1;
                //break;
                for (int j = 0; j < dstW; j++)
                {
                    fx = j * xscale + x;
                    if (fx > srcW - 1)
                        fx = srcW - 1;
                    //break;
                    for (int b = 0; b < bytesPerBand; b++)
                    {
                        dst[i * bytePerLine + bytesPerBand * j + b] = src[(int)(fy) * srcW * bytesPerBand + (int)(fx) * bytesPerBand + b];
                    }
                }
            }
        }
        #endregion
    }
}
