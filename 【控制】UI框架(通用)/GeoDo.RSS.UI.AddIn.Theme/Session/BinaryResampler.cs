using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    /// <summary>
    /// 扫描线二值图重采样
    /// </summary>
    internal interface IBinaryResampler
    {
        void Resample(ScanLineSegment[] segs, float scaleX, float scaleY, IPixelIndexMapper mapper);
        void Resample(ScanLineSegment[] segs, float scaleX, float scaleY, IPixelIndexMapper mapper, int[] aoi);
    }

    internal class BinaryResampler : IBinaryResampler
    {
        public unsafe void Resample(ScanLineSegment[] segs, float scaleX, float scaleY, IPixelIndexMapper mapper, int[] aoi)
        {
            /*
             * 计算当前视窗分辨率下扫描线外包矩形位图(为了节省内存）
             */
            int offsetX, offsetY;
            Bitmap oBitmap = GetOBitmap(segs, out offsetX, out offsetY);
            //按找缩放比将扫描线位图恢复到1:1分辨率
            using (Bitmap newBitmap = GetNewBitmap(oBitmap, scaleX, scaleY))
            {
                oBitmap.Dispose();
                if (newBitmap == null)
                    return;
                //拾取二值图
                WriteResampledResult(newBitmap, (int)(offsetX * scaleX), (int)(offsetY * scaleY), mapper, aoi);
            }
        }

        public unsafe void Resample(ScanLineSegment[] segs, float scaleX, float scaleY, IPixelIndexMapper mapper)
        {
            Resample(segs, scaleX, scaleY, mapper, null);
        }

        private unsafe void WriteResampledResult(Bitmap newBitmap, int offsetX, int offsetY, IPixelIndexMapper mapper, int[] aoi)
        {
            BitmapData pData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadOnly, newBitmap.PixelFormat);
            List<int> indexies = new List<int>();
            try
            {
                byte* ptr0 = (byte*)pData.Scan0;
                byte* ptr = ptr0;
                int stride = pData.Stride;
                int height = newBitmap.Height;
                int width = newBitmap.Width;
                int oWidth = mapper.Size.Width;
                int pixelWidth = 3;
                int curIndex = 0;
                for (int r = 0; r < height; r++, ptr = ptr0 + r * stride)
                    for (int c = 0; c < width; c++, ptr += pixelWidth)
                        if (*ptr == 255)//blue
                        {
                            curIndex = (r + offsetY) * oWidth + c + offsetX;
                            indexies.Add(curIndex);
                        }
                if (indexies.Count == 0)
                    return;
                if (aoi == null)
                    mapper.Put(indexies.ToArray());
                else
                {
                    mapper.Remove(aoi);
                    var intersect = aoi.Intersect(indexies);
                    if (intersect != null && intersect.Count() != 0)
                        mapper.Put(intersect.ToArray());
                }
            }
            finally
            {
                newBitmap.UnlockBits(pData);
            }
        }

        private Bitmap GetNewBitmap(Bitmap oBitmap, float scaleX, float scaleY)
        {
            try
            {
                int newWidth = (int)(oBitmap.Width * scaleX);
                int newHeight = (int)(oBitmap.Height * scaleY);
                Bitmap newBitmap = new Bitmap(newWidth, newHeight, oBitmap.PixelFormat);
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    Matrix matrix = new Matrix();
                    matrix.Scale(scaleX, scaleY);
                    g.Transform = matrix;
                    g.DrawImage(oBitmap, 0, 0);
                }
                return newBitmap;
            }
            catch//可能发生图像太大内存溢出的错误，后续修改为分块操作
            {
                return null;
            }
        }

        private unsafe Bitmap GetOBitmap(ScanLineSegment[] segs, out int offsetX, out int offsetY)
        {
            int oWidth, oHeight;
            GetOSize(segs, out oWidth, out oHeight, out offsetX, out offsetY);
            Bitmap bitmap = new Bitmap(oWidth, oHeight, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                BitmapData pData = bitmap.LockBits(new Rectangle(0, 0, oWidth, oHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                try
                {
                    byte* ptr0 = (byte*)pData.Scan0;
                    byte* ptr;
                    int pixelWidth = 3;
                    int stride = pData.Stride;
                    foreach (ScanLineSegment seg in segs)
                    {
                        ptr = ptr0 + (seg.Row - offsetY) * stride + (seg.BeginCol - offsetX) * pixelWidth;
                        for (int c = seg.BeginCol; c < seg.EndCol; c++, ptr += pixelWidth)
                            *ptr = 255;//blue
                    }
                }
                finally
                {
                    bitmap.UnlockBits(pData);
                }
            }
            return bitmap;
        }

        private void GetOSize(ScanLineSegment[] segs, out int oWidth, out int oHeight, out int offsetX, out int offsetY)
        {
            int minRow = int.MaxValue;
            int maxRow = int.MinValue;
            int minCol = int.MaxValue;
            int maxCol = int.MinValue;
            foreach (ScanLineSegment seg in segs)
            {
                minRow = Math.Min(seg.Row, minRow);
                maxRow = Math.Max(seg.Row, maxRow);
                minCol = Math.Min(seg.BeginCol, minCol);
                maxCol = Math.Max(seg.EndCol, maxCol);
            }
            oWidth = maxCol - minCol + 1;
            oHeight = maxRow - minRow + 1;
            offsetX = minCol;
            offsetY = minRow;
        }
    }
}
