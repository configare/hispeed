using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Core
{
    public class VectorAOIGenerator : IVectorAOIGenerator, IDisposable
    {
        public byte[] GetRaster(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            Color[] colors = GetColors(geometrys.Length);
            using (Bitmap buffer = GetBitmap(geometrys, colors, dstEnvelope, size))
            {
                using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
                {
                    return c.ToRaster(buffer, colors);
                }
            }
        }

        private Color[] GetColors(int count)
        {
            List<Color> colors = new List<Color>(count);
            Random random = new Random(1);
            Color color;
            for (int i = 0; i < count; i++)
            {
                color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                colors.Add(color);
            }
            return colors.ToArray();
        }

        public int[] GetAOI_OLD(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;

            using (Bitmap buffer = GetBitmap(geometrys, dstEnvelope, size))
            {
                using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
                {
                    return c.ToRaster(buffer, Color.Red);
                }
            }
        }

        public int[] GetAOI(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            double resX = dstEnvelope.Width / size.Width;
            double resY = dstEnvelope.Height / size.Height;
            Envelope aoiEnvelope = GetSrcEnvelope(geometrys);
            aoiEnvelope = aoiEnvelope.IntersectWith(dstEnvelope);
            if (aoiEnvelope == null)
                return null;
            Size aoiSize = new Size((int)GetInteger((aoiEnvelope.Width / resX), resX), (int)GetInteger((aoiEnvelope.Height / resY), resY));
            if (aoiSize.IsEmpty)
                return null;
            // by chennan 修正当AOI区域因精度问题计算出现偏差时，导致索引超出界限问题
            if (aoiSize.Width > size.Width)
                aoiSize.Width = size.Width;
            if (aoiSize.Height > size.Height)
                aoiSize.Height = size.Height;
            //
            int offsetX = (int)((aoiEnvelope.MinX - dstEnvelope.MinX) / resX);
            int offsetY = (int)((dstEnvelope.MaxY - aoiEnvelope.MaxY) / resY);
            using (Bitmap buffer = GetBitmap(geometrys, aoiEnvelope, aoiSize))
            {
                //buffer.Save(@"D:\bmp\" + (bmptext++.ToString()) + ".bmp");
                using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
                {
                    int[] aoi = c.ToRaster(buffer, Color.Red);
                    if (aoi == null)
                        return null;
                    int count = aoi.Length;
                    int newWidth = aoiSize.Width;
                    int oldWidth = size.Width;
                    int row = 0, col = 0;
                    for (int i = 0; i < count; i++)
                    {
                        row = aoi[i] / newWidth;
                        col = aoi[i] % newWidth;
                        row += offsetY;
                        col += offsetX;
                        aoi[i] = row * oldWidth + col;
                    }
                    return aoi;
                }
            }
        }

        public int GetAOICount(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return 0;
            double resX = dstEnvelope.Width / size.Width;
            double resY = dstEnvelope.Height / size.Height;
            Envelope aoiEnvelope = GetSrcEnvelope(geometrys);
            aoiEnvelope = aoiEnvelope.IntersectWith(dstEnvelope);
            if (aoiEnvelope == null)
                return 0;
            Size aoiSize = new Size((int)GetInteger((aoiEnvelope.Width / resX), resX), (int)GetInteger((aoiEnvelope.Height / resY), resX));
            if (aoiSize.IsEmpty)
                return 0;
            int offsetX = (int)((aoiEnvelope.MinX - dstEnvelope.MinX) / resX);
            int offsetY = (int)((dstEnvelope.MaxY - aoiEnvelope.MaxY) / resY);
            using (Bitmap buffer = GetBitmap(geometrys, aoiEnvelope, aoiSize))
            {
                using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
                {
                    int[] aoi = c.ToRaster(buffer, Color.Red);
                    if (aoi == null)
                        return 0;
                    return aoi.Length;
                }
            }
        }

        protected int GetInteger(double fWidth, double res)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > res)
                v++;
            return v;
        }

        private Envelope GetSrcEnvelope(ShapePolygon[] geometrys)
        {
            Envelope evp = geometrys[0].Envelope.Clone() as Envelope;
            for (int i = 1; i < geometrys.Length; i++)
                evp.UnionWith(geometrys[i].Envelope);
            return evp;
        }

        public int[] GetAOI_OLD(PointF[] rasterPoints, byte[] types, Size size)
        {
            Envelope evp = new Envelope(0, 0, size.Width, size.Height);
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = GetBitmapUseRaster(rasterPoints, types, evp, size);
                using (IBitmap2RasterConverter rstc = new Bitmap2RasterConverter())
                {
                    int[] aoi = rstc.ToRaster(buffer, Color.Red);
                    if (aoi != null)
                        Array.Sort<int>(aoi);
                    return aoi;
                }
            }
        }

        /// <summary>
        /// 解决了大图像AOI栅格化内存溢出的问题
        /// 解决方法：
        /// 1、按照AOI的最小外包矩形栅格化计算AOI索引
        /// 2、将计算完成的AOI索引平移到大图
        /// </summary>
        /// <param name="rasterPoints"></param>
        /// <param name="types"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int[] GetAOI(PointF[] rasterPoints, byte[] types, Size size)
        {
            Envelope evp = new Envelope(0, 0, size.Width, size.Height);
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                int offsetX, offsetY;
                Size newSize;
                Bitmap buffer = GetBitmapUseRasterII(rasterPoints, types, evp, size, out offsetX, out offsetY, out newSize);
                if (newSize.IsEmpty || buffer == null)
                    return null;
                using (IBitmap2RasterConverter rstc = new Bitmap2RasterConverter())
                {
                    int[] aoi = rstc.ToRaster(buffer, Color.Red);
                    if (aoi == null || aoi.Length == 0)
                        return null;
                    if (aoi != null)
                        Array.Sort<int>(aoi);
                    int count = aoi.Length;
                    int newWidth = newSize.Width;
                    int oldWidth = size.Width;
                    int oldHeight = size.Height;
                    int row = 0, col = 0;
                    List<int> newAio = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        row = aoi[i] / newWidth;
                        col = aoi[i] % newWidth;
                        row += offsetY;
                        col += offsetX;
                        if (col < 0 || col >= oldWidth || row < 0 || row >= oldHeight)
                            continue;
                        else
                            newAio.Add(row * oldWidth + col);
                    }
                    return newAio.ToArray();
                }
            }
        }

        public int[] GetAOI(PointF[] coordPoints, byte[] types, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = GetBitmap(coordPoints, types, dstEnvelope, size);
                using (IBitmap2RasterConverter rstc = new Bitmap2RasterConverter())
                {
                    return rstc.ToRaster(buffer, Color.Red);
                }
            }
        }

        private Bitmap GetBitmap(ShapePolygon[] geometrys, Color[] colors, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
                Dictionary<ShapePolygon, Color> vectors = new Dictionary<ShapePolygon, Color>();
                int idx = 0;
                foreach (ShapePolygon geo in geometrys)
                {
                    vectors.Add(geo, colors[idx++]);
                }
                c.ToBitmap(vectors, Color.Black, dstEnvelope, size, ref buffer);
                return buffer;
            }
        }

        private Bitmap GetBitmap(ShapePolygon[] geometrys, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
                Dictionary<ShapePolygon, Color> vectors = new Dictionary<ShapePolygon, Color>();
                foreach (ShapePolygon geo in geometrys)
                    vectors.Add(geo, Color.Red);
                c.ToBitmap(vectors, Color.Black, dstEnvelope, size, ref buffer);
                return buffer;
            }
        }

        private Bitmap GetBitmap(PointF[] points, byte[] types, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
                c.ToBitmap(points, types, Color.Red, Color.Black, dstEnvelope, size, ref buffer);
                return buffer;
            }
        }

        private Bitmap GetBitmapUseRaster(PointF[] points, byte[] types, Envelope dstEnvelope, Size size)
        {
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
                c.ToBitmapUseRasterCoord(points, types, Color.Red, Color.Black, dstEnvelope, size, ref buffer);
                return buffer;
            }
        }

        /// <summary>
        /// 函数GetBitmapUseRasterII解决大图像栅格化出现内存溢出的错误
        /// 解决方法：
        /// 1、先按照AOI最小外包矩形栅格化；
        /// 2、将栅格化后的索引平移至大图像。
        /// </summary>
        /// <param name="points"></param>
        /// <param name="types"></param>
        /// <param name="dstEnvelope"></param>
        /// <param name="size"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        private Bitmap GetBitmapUseRasterII(PointF[] points, byte[] types, Envelope dstEnvelope, Size size,
            out int offsetX, out int offsetY, out Size newSize)
        {
            offsetX = offsetY = 0;
            newSize = Size.Empty;
            if (dstEnvelope == null || size.IsEmpty)
                return null;
            //
            float minX = float.MaxValue;
            float maxX = int.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X < minX)
                    minX = points[i].X;
                if (points[i].X > maxX)
                    maxX = points[i].X;
                if (points[i].Y < minY)
                    minY = points[i].Y;
                if (points[i].Y > maxY)
                    maxY = points[i].Y;
            }
            //
            offsetX = (int)minX;
            offsetY = (int)minY;

            int width = (int)(maxX - minX);
            int height = (int)(maxY - minY);
            if (width <= 0 || height <= 0)
                return null;
            newSize = new Size(width, height);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X -= minX;
                points[i].Y -= minY;
            }
            //
            double xSpan = dstEnvelope.Width / size.Width;
            double ySpan = dstEnvelope.Height / size.Height;
            double minXCoord = dstEnvelope.MinX + offsetX * xSpan;
            double maxYCoord = dstEnvelope.MaxY - offsetY * ySpan;
            double maxXCoord = minXCoord + width * xSpan;
            double minYCoord = maxYCoord - height * ySpan;
            Envelope newEnvelope = new Envelope(minXCoord, minYCoord, maxXCoord, maxYCoord);
            //offsetY = Math.Abs(offsetY);
            //
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Bitmap buffer = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                c.ToBitmapUseRasterCoord(points, types, Color.Red, Color.Black, newEnvelope, newSize, ref buffer);
                return buffer;
            }
        }

        public void Dispose()
        {
        }
    }
}
