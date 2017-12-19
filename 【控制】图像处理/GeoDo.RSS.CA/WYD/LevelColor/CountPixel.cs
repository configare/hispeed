using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace GeoDo.RSS.CA
{
    public unsafe class CountPixel
    {
        private Bitmap _originalBitmap = null;
        private IRgbProcessorStack _processorStack = new RgbProcessorStack();
        private byte* _ptr0 = null;
        private int _stride = 0;
        private ObjPixelCount _objPixelCount = new ObjPixelCount();

        public CountPixel()
        {

        }

        public ObjPixelCount ObjPixelCount
        {
            get { return _objPixelCount; }
            set { _objPixelCount = value; }
        }

        public ObjPixelCount GetMap(string filename)
        {
            _originalBitmap = (Bitmap)Bitmap.FromFile(filename);
            using (Bitmap bitmap = _originalBitmap.Clone() as Bitmap)
            {
                Apply(bitmap);
            }
            return _objPixelCount;
        }

        private  BitmapData _pdata = null;
        public ObjPixelCount Apply(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            try
            {
                _pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                CountHistPixel();
            }
            finally
            {
                if (_pdata != null)
                    bitmap.UnlockBits(_pdata);
            }
            return _objPixelCount;
        }


        private  void CountHistPixel()
        {
            if (_pdata == null)
                return;
            _ptr0 = (byte*)_pdata.Scan0;
            _stride = _pdata.Stride;
            switch (GetBytesPerPixel())
            {
                case 1:
                    Parallel.For(0, _pdata.Height, ProcessOneRowGray);
                    break;
                case 3:
                    Parallel.For(0, _pdata.Height, ProcessOneRowRGB);
                    break;
                default:
                    throw new NotSupportedException(_pdata.PixelFormat.ToString());
            }
        }

        private int GetBytesPerPixel()
        {
            switch (_pdata.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                default:
                    throw new NotSupportedException(_pdata.PixelFormat.ToString());
            }
        }

        private void ProcessOneRowRGB(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int width = _pdata.Width;
            for (int col = 0; col < width; col++)
            {
                ProcessCount(ref *(ptr++), ref *(ptr++), ref *(ptr++));
            }
        }

        private void ProcessOneRowGray(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int width = _pdata.Width;
            for (int col = 0; col < width; col++)
            {
                ProcessGrayCount(ref *(ptr++));
            }
        }

        private void ProcessGrayCount(ref byte pixelValue)
        {
            CountPixels("RGB", pixelValue);
        }

        private void ProcessCount(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            CountPixels("RGB", pixelBlue);
            CountPixels("RGB", pixelGreen);
            CountPixels("RGB", pixelRed);
            CountPixels("Red", pixelRed);
            CountPixels("Green", pixelGreen);
            CountPixels("Blue", pixelBlue);
        }

        /// <summary>
        /// 统计每个色阶的像素数量
        /// </summary>
        /// <param name="enumCh"></param>
        /// <param name="pixelValue"></param>
        protected void CountPixels(string strCH, byte pixelValue)
        {
            int value = ColorMath.FixByte(pixelValue);
            switch (strCH)
            {
                case "RGB":
                    {
                        _objPixelCount.PixelCountRGB[value]++;
                        break;
                    }
                case "Blue":
                    {
                        _objPixelCount.PixelCountBlue[value]++;
                        break;
                    }
                case "Green":
                    {
                        _objPixelCount.PixelCountGreen[value]++;
                        break;
                    }
                case "Red":
                    {
                        _objPixelCount.PixelCountRed[value]++;
                        break;
                    }
            }
        }
    }
}
