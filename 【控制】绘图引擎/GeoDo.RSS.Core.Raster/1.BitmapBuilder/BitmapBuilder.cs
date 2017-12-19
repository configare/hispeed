using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal unsafe class BitmapBuilder<T> : IBitmapBuilder<T>, IDisposable
    {
        //default stretcher
        protected Func<T, byte> _defaultGrayStretcher = null, _defaultRedStretcher = null, _defaultGreenStretcher = null, _defaultBlueStretcher = null;
        //global vars for parallel
        private int _width = 0, _stride = 0;
        private int _offsetX = 0;
        private int _offsetY = 0;
        private byte* _ptr0 = null;
        private T[] _redBuffer = null, _greenBuffer = null, _blueBuffer = null;
        private Func<T, byte> _s1 = null, _s2 = null, _s3 = null;

        internal void SetDefaultStretcher(Func<T, byte> grayStretcher, Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher)
        {
            _defaultGrayStretcher = grayStretcher;
            _defaultRedStretcher = redStretcher;
            _defaultGreenStretcher = greenStretcher;
            _defaultBlueStretcher = blueStretcher;
        }

        public static ColorPalette GetDefaultGrayColorPalette()
        {
            ColorPalette cp = (new Bitmap(1, 1, PixelFormat.Format8bppIndexed)).Palette;
            for (int i = 0; i < 256; i++)
                cp.Entries[i] = Color.FromArgb(255, i, i, i);
            return cp;
        }

        #region IBitmapBuilder<T> 成员

        public void Build(int width, int height, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, ref Bitmap bitmap)
        {
            Build(width, height, redBuffer, greenBuffer, blueBuffer, _defaultRedStretcher, _defaultGreenStretcher, _defaultBlueStretcher, ref bitmap);
        }

        public void Build(int width, int height,int offsetX,int offsetY, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, ref Bitmap bitmap)
        {
            Build(width, height, offsetX, offsetY, redBuffer, greenBuffer, blueBuffer, _defaultRedStretcher, _defaultGreenStretcher, _defaultBlueStretcher, ref bitmap);
        }

        public void Build(int width, int height, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher, ref Bitmap bitmap)
        {
            Build(width, height, 0, 0, redBuffer, greenBuffer, blueBuffer, redStretcher, greenStretcher, blueStretcher, ref bitmap);
        }

        public void Build(int width, int height, int offsetX, int offsetY, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher, ref Bitmap bitmap)
        {
            if (!ArgumentsIsOK(width, height, redBuffer, greenBuffer, blueBuffer, redStretcher, greenStretcher, blueStretcher, bitmap))
                throw new ArgumentException();
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                _redBuffer = redBuffer;
                _greenBuffer = greenBuffer;
                _blueBuffer = blueBuffer;
                _s1 = redStretcher;
                _s2 = greenStretcher;
                _s3 = blueStretcher;
                _width = width;
                _stride = pdata.Stride;
                IntPtr intPtr = pdata.Scan0;
                _ptr0 = (byte*)intPtr;
                _offsetX = offsetX;
                _offsetY = offsetY;
                Parallel.For(_offsetY, height + _offsetY, row => { BuildOneRow(row); });
            }
            finally
            {
                bitmap.UnlockBits(pdata);
            }
        }

        public void Build(int width, int height, T[] grayBuffer, ref Bitmap bitmap)
        {
            Build(width, height, grayBuffer, _defaultGrayStretcher, ref bitmap);
        }

        public void Build(int width, int height,int offsetX,int offsetY, T[] grayBuffer, ref Bitmap bitmap)
        {
            Build(width, height,offsetX,offsetY, grayBuffer, _defaultGrayStretcher, ref bitmap);
        }

        public void Build(int width, int height, T[] grayBuffer, Func<T, byte> grayStretcher, ref Bitmap bitmap)
        {
            Build(width, height, 0, 0, grayBuffer, grayStretcher, ref bitmap);
        }

        public void Build(int width, int height, int offsetX, int offsetY, T[] grayBuffer, Func<T, byte> grayStretcher, ref Bitmap bitmap)
        {
            if (!ArgumentsIsOK(width, height, grayBuffer, grayStretcher, bitmap))
                throw new ArgumentException();
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                _redBuffer = grayBuffer;
                _s1 = grayStretcher;
                _width = width;
                _stride = pdata.Stride;
                IntPtr intPtr = pdata.Scan0;
                _ptr0 = (byte*)intPtr;
                _offsetX = offsetX;
                _offsetY = offsetY;
                Parallel.For(_offsetY, height + _offsetY, row => { BuildOneRowGray(row); });
            }
            finally
            {
                bitmap.UnlockBits(pdata);
            }
        }

        private void BuildOneRow(int row)
        {
            byte* ptr = _ptr0 + row * _stride + _offsetX * 3;
            int idx = (row - _offsetY) * _width;
            for (int col = 0; col < _width; col++, ptr += 3, idx++)
            {
                *ptr = _s3(_blueBuffer[idx]);
                *(ptr + 1) = _s2(_greenBuffer[idx]);
                *(ptr + 2) = _s1(_redBuffer[idx]);
            }
        }

        private void BuildOneRowGray(int row)
        {
            byte* ptr = _ptr0 + row * _stride + _offsetX * 1;
            int idx = (row - _offsetY) * _width;
            for (int col = 0; col < _width; col++, ptr+=1, idx++)
            {
                *ptr = _s1(_redBuffer[idx]);
            }
        }

        private bool ArgumentsIsOK(int width, int height, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher, Bitmap bitmap)
        {
            if (bitmap == null || redBuffer == null || greenBuffer == null || blueBuffer == null)
                return false;
            if (redBuffer.Length != greenBuffer.Length || greenBuffer.Length != blueBuffer.Length)
                return false;
            return true;
        }

        private bool ArgumentsIsOK(int width, int height, T[] grayBuffer, Func<T, byte> grayStretcher, Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");
            if (grayBuffer == null)
                throw new ArgumentNullException("grayBuffer");
            return true;
        }

        #endregion

        public void Dispose()
        {
            _defaultGrayStretcher = null;
            _defaultRedStretcher = null;
            _defaultGreenStretcher = null;
            _defaultBlueStretcher = null;
            _s1 = null;
            _s2 = null;
            _s3 = null;
            _redBuffer = null;
            _greenBuffer = null;
            _blueBuffer = null;
        }
    }
}
