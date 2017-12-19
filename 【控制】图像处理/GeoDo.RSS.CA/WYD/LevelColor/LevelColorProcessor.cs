using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Xml;
using GeoDo.RSS.Core.CA;


namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 色阶算法类
    /// </summary>
    [Export(typeof(IRgbProcessor))]
    public unsafe class LevelColorProcessor : RgbProcessorByPixel
    {
        private LevelColorArg _lcArgs = null;
        private const double stepformid = 2.55;
        private byte* _ptr0 = null;
        private int _stride = 0;
        private bool _isFirstCount = true;
        private double _inputMid = 0;
        private int _outputMin = 0;
        private byte[] _rgbs = null;

        public LevelColorProcessor()
            : base()
        {
            Init();
        }

        public LevelColorProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public LevelColorProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "色阶";
        }

        protected override void BeforeProcess()
        {
            _lcArgs = _arg as LevelColorArg;
            if (_isFirstCount)
            {
                _lcArgs.ObjPixelCount.ClearPixelCount();
                CountHistPixel();
                _isFirstCount = false;
            }

            _inputMid = _lcArgs.InputMiddle;
            _outputMin = _lcArgs.OutputMin;

            int outputDif = _lcArgs.OutputMax - _outputMin;
            double inputDif = _lcArgs.InputMax - _lcArgs.InputMin;
            double thanValue = _lcArgs.InputMin / inputDif;

            _rgbs = new byte[256];
            if (_lcArgs.IsChanged)
                for (int i = 0; i < 256; i++)
                    _rgbs[i] = ColorMath.FixByte(outputDif * Math.Pow((i / inputDif - thanValue), _inputMid) + _outputMin);
            else
                for (int i = 0; i < 256; i++)
                    _rgbs[i] = (byte)i;
        }

        public void CountHistPixel()
        {
            if (_pdata == null)
                return;
            _ptr0 = (byte*)_pdata.Scan0;
            _stride = _pdata.Stride;
            switch (_bytesPerPixel)
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
            CountPixel(enumChannel.RGB, pixelValue);
        }

        private void ProcessCount(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            CountPixel(enumChannel.RGB, pixelBlue);
            CountPixel(enumChannel.RGB, pixelGreen);
            CountPixel(enumChannel.RGB, pixelRed);
            CountPixel(enumChannel.R, pixelRed);
            CountPixel(enumChannel.G, pixelGreen);
            CountPixel(enumChannel.B, pixelBlue);
        }

        /// <summary>
        /// 统计每个色阶的像素数量
        /// </summary>
        /// <param name="enumCh"></param>
        /// <param name="pixelValue"></param>
        protected void CountPixel(enumChannel enumCh, byte pixelValue)
        {
            int value = ColorMath.FixByte(pixelValue);
            switch (enumCh)
            {
                case enumChannel.RGB:
                    {
                        _lcArgs.ObjPixelCount.PixelCountRGB[value]++;
                        break;
                    }
                case enumChannel.B:
                    {
                        _lcArgs.ObjPixelCount.PixelCountBlue[value]++;
                        break;
                    }
                case enumChannel.G:
                    {
                        _lcArgs.ObjPixelCount.PixelCountGreen[value]++;
                        break;
                    }
                case enumChannel.R:
                    {
                        _lcArgs.ObjPixelCount.PixelCountRed[value]++;
                        break;
                    }
            }
        }

        /// <summary>
        /// 单色图像处理
        /// </summary>
        /// <param name="pixelValue"></param>
        protected override void Process(ref byte pixelValue)
        {
            pixelValue = _rgbs[pixelValue];
        }

        /// <summary>
        /// RGB图像处理
        /// </summary>
        /// <param name="pixelBlue"></param>
        /// <param name="pixelGreen"></param>
        /// <param name="pixelRed"></param>
        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            switch (_lcArgs.Channel)
            {
                case enumChannel.RGB:
                    {
                        pixelBlue = _rgbs[pixelBlue];
                        pixelGreen = _rgbs[pixelGreen];
                        pixelRed = _rgbs[pixelRed];
                        break;
                    }
                case enumChannel.R:
                    {
                        pixelRed = _rgbs[pixelRed];
                        break;
                    }
                case enumChannel.G:
                    {
                        pixelGreen = _rgbs[pixelGreen];
                        break;
                    }
                case enumChannel.B:
                    {
                        pixelBlue = _rgbs[pixelBlue];
                        break;
                    }
            }
        }

        public override void CreateDefaultArguments()
        {
            _arg = new LevelColorArg();
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_lcArgs == null)
            {
                _lcArgs = new LevelColorArg();
            }
            return _lcArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            LevelColorProcessor pro = new LevelColorProcessor();
            pro._lcArgs = new LevelColorArg();
            switch (elem.ChildNodes[0].InnerText)
            {
                case "RGB":
                    pro._lcArgs.Channel = enumChannel.RGB;
                    break;
                case "Red":
                    pro._lcArgs.Channel = enumChannel.R;
                    break;
                case "Green":
                    pro._lcArgs.Channel = enumChannel.G;
                    break;
                case "Blue":
                    pro._lcArgs.Channel = enumChannel.B;
                    break;
            }
            pro._lcArgs.InputMin = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro._lcArgs.InputMiddle = Convert.ToDouble(elem.ChildNodes[2].InnerText);
            pro._lcArgs.InputMax = Convert.ToInt32(elem.ChildNodes[3].InnerText);
            pro._lcArgs.OutputMin = Convert.ToInt32(elem.ChildNodes[4].InnerText);
            pro._lcArgs.OutputMax = Convert.ToInt32(elem.ChildNodes[5].InnerText);
            pro.Arguments = pro._lcArgs;
            return pro;
        }
    }
}
