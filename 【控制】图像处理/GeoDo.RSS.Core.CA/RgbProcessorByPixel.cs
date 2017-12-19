using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.Core.CA
{
    public abstract unsafe class RgbProcessorByPixel : RgbProcessor
    {
        private byte* _ptr0 = null;
        private int _stride = 0;

        public RgbProcessorByPixel()
            : base()
        {
        }

        public RgbProcessorByPixel(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public RgbProcessorByPixel(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        /// <summary>
        /// 灰度图像处理算法（像素）
        /// </summary>
        protected override void ProcessingGray()
        {
            _stride = _pdata.Stride;
            _ptr0 = (byte*)_pdata.Scan0;
            if (_indexesOfAOIs == null)
#if debug
                for (int row = 0; row < _pdata.Height; row++)
                {
                    ProcessOneRowGray(row);
                }
#else
                Parallel.For(0, _pdata.Height, ProcessOneRowGray);
#endif
            else
                ProcessingByAOI();
        }

        /// <summary>
        /// 彩色图像处理算法（像素）
        /// </summary>
        protected override void ProcessingRGB()
        {
            _stride = _pdata.Stride;
            _ptr0 = (byte*)_pdata.Scan0;
#if CHUANXING
            int width = _pdata.Width;
            int height = _pdata.Height;
            for (int row = 0; row < height; row++)
            {
                ProcessOneRowRGB(row);
            }
#else
            if (_indexesOfAOIs == null)
#if debug
                for (int row = 0; row < _pdata.Height; row++)
                {
                    ProcessOneRowRGB(row);
                }
#else
                Parallel.For(0, _pdata.Height, ProcessOneRowRGB);
#endif
            else
                ProcessingByAOI();
#endif
        }

        /// <summary>
        /// 感兴趣区域处理算法（像素）
        /// </summary>
        private void ProcessingByAOI()
        {
            foreach (int[] aoi in _indexesOfAOIs)
            {
                if (aoi == null || aoi.Length == 0)
                    continue;
                if (_bytesPerPixel == 1)
                    ProcessAOIGray(aoi);
                else if (_bytesPerPixel == 3)
                    ProcessAOIRGB(aoi);
            }
        }

        /// <summary>
        ///灰度图像感兴趣区域处理算法（像素）
        /// </summary>
        /// <param name="aoi"></param>
        private void ProcessAOIGray(int[] aoi)
        {
            int len = aoi.Length;
            int idx = 0;
            int row = 0, col = 0;
            for (int i = 0; i < len; i++)
            {
                row = aoi[i] / _width;
                col = aoi[i] % _width;
                idx = row * _stride + col;
                Process(ref *(_ptr0 + idx));
            }
        }

        /// <summary>
        /// 彩色图像感兴趣区域处理算法（像素）
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
                idx = row * _stride + col * 3;
                Process(ref *(_ptr0 + idx), ref *(_ptr0 + idx + 1), ref *(_ptr0 + idx + 2));
            }
        }

        /// <summary>
        /// 灰度图像单行数据处理算法（像素）
        /// </summary>
        /// <param name="row"></param>
        private void ProcessOneRowGray(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int width = _pdata.Width;
            for (int col = 0; col < width; col++)
            {
                Process(ref *(ptr++));
            }
        }

        /// <summary>
        /// 真彩色图像单行数据处理算法（像素）
        /// </summary>
        /// <param name="row"></param>
        private void ProcessOneRowRGB(int row)
        {
            byte* ptr = _ptr0 + row * _stride;
            int width = _pdata.Width;
            for (int col = 0; col < width; col++)
            {
                Process(ref *(ptr++), ref *(ptr++), ref *(ptr++));
            }
        }

        protected abstract void Process(ref byte pixelValue);

        protected abstract void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed);
        
        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            throw new NotImplementedException();
        }

        public void ProcessRGB(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            BeforeProcess();
            Process(ref pixelBlue, ref pixelGreen, ref pixelRed);
        }
    }
}
