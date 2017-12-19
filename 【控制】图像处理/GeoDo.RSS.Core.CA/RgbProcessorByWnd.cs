using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.CA
{
    [Export(typeof(IRgbProcessor))]
    public abstract unsafe class RgbProcessorByWnd : RgbProcessor
    {
        protected RgbWndProcessorArg _actualArg = null;
        protected Bitmap _cloneBitmap = null;
        protected BitmapData _cloneBitmapData = null;
        protected BitmapData _originalBitmapData = null;
        private byte* _ptr0 = null;
        private int _stride = 0;
        private int _halfWndHeight = 0;
        private int _halfWndWidth = 0;
        private int _offsetCols = 0;
        private int _beginCol = 0;
        private int _endCol = 0;
        private int _beginRow = 0;
        private int _endRow = 0;
        private int _width = 0;
        private int _height = 0;
        private int _wndCapacity = 0;
        private byte* _originalPtr0 = null;
        private int _originalStide = 0;

        public RgbProcessorByWnd()
            : base()
        {
        }

        public RgbProcessorByWnd(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public RgbProcessorByWnd(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            if (_actualArg == null)
            {
                if (_arg == null)
                    CreateDefaultArguments();
                _actualArg = _arg as RgbWndProcessorArg;
            }
            CloneBitmapData();
        }

        protected override void AfterProcess()
        {
            base.AfterProcess();
        }

        private void CopyCloneToOriginal()
        {
            byte[] source = new byte[_height * _pdata.Stride];
            Marshal.Copy(_cloneBitmapData.Scan0, source, 0, _height * _cloneBitmapData.Stride);
            Marshal.Copy(source, 0, _pdata.Scan0, _height * _pdata.Stride);
            _pdata = _originalBitmapData;
            _cloneBitmap.UnlockBits(_cloneBitmapData);
            _cloneBitmap.Dispose();
            _cloneBitmapData = null;
            _cloneBitmap = null;
        }

        private void CloneBitmapData()
        {
            _width = _pdata.Width;
            _height = _pdata.Height;
            _cloneBitmap = new Bitmap(_width, _height, _pdata.PixelFormat);
            _cloneBitmapData = _cloneBitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadWrite, _cloneBitmap.PixelFormat);
            byte[] source = new byte[_height * _pdata.Stride];
            Marshal.Copy(_pdata.Scan0, source, 0, _height * _pdata.Stride);
            Marshal.Copy(source, 0, _cloneBitmapData.Scan0, _height * _cloneBitmapData.Stride);
            _originalBitmapData = _cloneBitmapData;
        }

        private int Compare(int[] pdata, int[] cloneBitmapData)
        {
            int firstLong = pdata.Length;
            int secondLong = cloneBitmapData.Length;
            int sum = 0;
            if (firstLong > secondLong || firstLong == secondLong)
            {
                for (int i = 0; i < secondLong; i++)
                {
                    if (pdata[i] == cloneBitmapData[i])
                        sum += 0;
                    else
                        sum += 1;
                }
            }
            else
            {
                for (int i = 0; i < firstLong; i++)
                {
                    if (pdata[i] == cloneBitmapData[i])
                        sum += 0;
                    else
                        sum += 1;
                }
            }
            return sum;
        }

        private unsafe int[] ReadBitmapData(BitmapData bt)
        {
            int width = bt.Width;
            int height = bt.Height;
            int num = width * height * _bytesPerPixel;
            byte* ptr0 = (byte*)bt.Scan0;
            int[] dataRead = new int[num];
            int k = 0;
            for (int y = 0; y < _pdata.Height; y++)
            {
                for (int x = 0; x < _pdata.Width; x++)
                {
                    for (int band = 0; band < _bytesPerPixel; band++)
                    {
                        dataRead[k] = *(ptr0 + (y * _width + x) * band);
                        k++;
                    }
                }
            }
            return dataRead;
        }

        /// <summary>
        /// 灰度图像处理算法（窗口）
        /// </summary>
        protected override void ProcessingGray()
        {
            ProcessingRGB();
        }

        /// <summary>
        /// 彩色图像处理算法（窗口）
        /// </summary>
        protected override void ProcessingRGB()
        {
            InitLocalVars();
#if CHUANXING
            ProcessingRGBSequence();
#else
            if (_indexesOfAOIs == null)
                ProcessingRGBParallel(ref *_ptr0);
            else
                ProcessingByAOI();
#endif
        }

        /// <summary>
        /// 感兴趣区域处理算法（窗口）
        /// </summary>
        private void ProcessingByAOI()
        {
            foreach (int[] aoi in _indexesOfAOIs)
            {
                if (aoi == null || aoi.Length == 0)
                    continue;
                else
                    ProcessAOIRGB(aoi);
            }
        }

        /// <summary>
        /// 彩色图像感兴趣区域处理算法（窗口）
        /// </summary>
        /// <param name="aoi"></param>
        private void ProcessAOIRGB(int[] aoi)
        {
            int len = aoi.Length;
            int idx = 0;
            int row = 0, col = 0;
            for (int i = 0; i < len; i++)
            {
                row = aoi[i] / _width;
                col = aoi[i] % _width;
                idx = row * _stride + col*3;
              // ProcessingRGBParallel(ref *(_ptr0 + idx));
            }
        }

        /// <summary>
        /// 相关参数初始化
        /// </summary>
        private void InitLocalVars()
        {
            _width = _pdata.Width;
            _height = _pdata.Height;
            TryCheckArgIsOK();
            if (_width < _actualArg.WndWidth || _height < _actualArg.WndHeight)
                return;
            _wndCapacity = _actualArg.WndHeight * _actualArg.WndWidth;
            _stride = _pdata.Stride;
            _originalStide = _originalBitmapData.Stride;
            _halfWndHeight = _actualArg.WndHeight / 2;
            _halfWndWidth = _actualArg.WndWidth / 2;
            _beginRow = _halfWndHeight;
            _beginCol = _halfWndWidth;
            _endRow = _height - _halfWndHeight - 1;
            _endCol = _width - _halfWndWidth - 1;
            _offsetCols = _beginCol * _bytesPerPixel;
            _ptr0 = (byte*)_pdata.Scan0;
            _originalPtr0 = (byte*)_originalBitmapData.Scan0;
        }

        /// <summary>
        /// 彩色图像并行处理算法（窗口）
        /// </summary>
        /// <param name="ptr"></param>
        private void ProcessingRGBParallel(ref byte ptr)
        {
            Parallel.For(_beginRow, _endRow, ProcessOneRow);
        }

        /// <summary>
        /// 图像单行像素处理算法（窗口）
        /// </summary>
        /// <param name="row"></param>
        private void ProcessOneRow(int row)
        {
            byte[][] wndPixels = new byte[_bytesPerPixel][];

            //初始化数组wndPixels的成员数组，长度为窗口中的像素容量
            for (int i = 0; i < _bytesPerPixel; i++)
                wndPixels[i] = new byte[_wndCapacity];
            //指针指向窗口的首地址
            byte* ptr = _ptr0 + row * _stride + _offsetCols;
            //窗口首地址所在行
            int wndBeginRow = row - _halfWndHeight;
            //窗口首地址所在列
            int wndEndRow = row + _halfWndHeight;
            //列循环
            for (int col = _beginCol; col <= _endCol; col++)
            {
                for (int i = 0; i < _bytesPerPixel; i++)
                    Array.Clear(wndPixels[i], 0, _wndCapacity);
                //窗口指针
                byte* wndPtr = null;
                //窗口的起始列地址
                int wndBeginCol = col - _halfWndWidth;
                //窗口的结束列地址
                int wndEndCol = col + _halfWndWidth;
                int skipColOffset = (col - _halfWndWidth) * _bytesPerPixel;
                int idx = 0;
                for (int r = wndBeginRow; r <= wndEndRow; r++)
                {
                    wndPtr = _originalPtr0 + r * _originalStide + skipColOffset;
                    for (int c = wndBeginCol; c <= wndEndCol; c++, idx++)
                    {
                        for (int band = 0; band < _bytesPerPixel; band++)
                            wndPixels[band][idx] = wndPtr[band];
                        wndPtr += _bytesPerPixel;
                    }
                }
                Process(wndPixels, ref ptr);
            }
        }

        /// <summary>
        /// 彩色图像串行处理算法
        /// </summary>
        private void ProcessingRGBSequence()
        {
            byte* ptr = _ptr0 + _beginRow * _stride + _offsetCols;
            byte* originalPtr = _originalPtr0 + _beginRow * _originalStide + _offsetCols;
            int wndCapacity = _actualArg.WndWidth * _actualArg.WndHeight;
            byte[][] wndPixels = new byte[_bytesPerPixel][];
            for (int i = 0; i < _bytesPerPixel; i++)
                wndPixels[i] = new byte[_wndCapacity];
            for (int row = _beginRow; row <= _endRow; row++, ptr = _ptr0 + row * _stride + _offsetCols)
            {
                int wndBeginRow = row - _halfWndHeight;
                int wndEndRow = row + _halfWndHeight;
                for (int col = _beginCol; col <= _endCol; col++)
                {
                    for (int i = 0; i < _bytesPerPixel; i++)
                        Array.Clear(wndPixels[i], 0, _wndCapacity);
                    //
                    byte* wndPtr = null;
                    int wndBeginCol = col - _halfWndWidth;
                    int wndEndCol = col + _halfWndWidth;
                    int skipColOffset = (col - _halfWndWidth) * _bytesPerPixel;
                    int idx = 0;
                    for (int r = wndBeginRow; r <= wndEndRow; r++)
                    {
                        wndPtr = _originalPtr0 + r * _originalStide + skipColOffset;
                        for (int c = wndBeginCol; c <= wndEndCol; c++, idx++)
                        {
                            for (int band = 0; band < _bytesPerPixel; band++)
                                wndPixels[band][idx] = wndPtr[band];
                            wndPtr += _bytesPerPixel;
                        }
                    }
                    Process(wndPixels, ref ptr);
                }
            }
        }

        protected abstract unsafe void Process(byte[][] wndPixels, ref byte* ptr);

        /// <summary>
        /// 检查参数（窗口大小）是否有效（大于等于3，小于等于9）
        /// </summary>
        private void TryCheckArgIsOK()
        {
            _actualArg.WndHeight = Math.Max(RgbWndProcessorArg.MIN_WND_SIZE, _actualArg.WndHeight);
            _actualArg.WndHeight = Math.Min(RgbWndProcessorArg.MAX_WND_SIZE, _actualArg.WndHeight);
            _actualArg.WndWidth = Math.Max(RgbWndProcessorArg.MIN_WND_SIZE, _actualArg.WndWidth);
            _actualArg.WndWidth = Math.Min(RgbWndProcessorArg.MAX_WND_SIZE, _actualArg.WndWidth);
        }

        /// <summary>
        /// 创建缺省参数对象
        /// </summary>
        public override void CreateDefaultArguments()
        {
            _arg = new RgbWndProcessorArg();
        }
    }
}
