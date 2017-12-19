using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.ComponentModel.Composition;
using System.Xml;

namespace GeoDo.RSS.Core.CA
{
    [Export(typeof(IRgbProcessor))]
    public unsafe abstract class RgbProcessor : IRgbProcessor
    {
        protected string _name = null;
        protected RgbProcessorArg _arg = null;
        protected int[][] _indexesOfAOIs = null;
        protected BitmapData _pdata = null;
        protected int _bytesPerPixel = 3;
        protected int _width = 0;

        public RgbProcessor()
        {
        }

        public RgbProcessor(RgbProcessorArg arg)
            : base()
        {
            _arg = arg;
        }

        public RgbProcessor(string name, RgbProcessorArg arg)
            : this(arg)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name ?? string.Empty; }
        }

        public RgbProcessorArg Arguments
        {
            get { return _arg; }
            set { _arg = value; }
        }

        public int BytesPerPixel
        {
            get
            {
                if (_pdata != null)
                    return GetBytesPerPixel();
                else
                    return _bytesPerPixel;
            }
            set { _bytesPerPixel = value; }
        }

        public virtual void Reset()
        {
            _indexesOfAOIs = null;
            _pdata = null;
        }

        public void Process(BitmapData pdata)
        {
            Process(null, pdata);
        }

        public void Process(int[][] indexesOfAOIs, BitmapData pdata)
        {
            if (pdata == null)
                return;
            TryCreateDefaultArguments();
            _indexesOfAOIs = indexesOfAOIs;
            _pdata = pdata;
            _width = pdata.Width;
            _bytesPerPixel = GetBytesPerPixel();
            BeforeProcess();
            DoProcessing();
            AfterProcess();
            Reset();
        }

        protected virtual void AfterProcess()
        {
        }

        protected virtual void BeforeProcess()
        {
        }

        private void TryCreateDefaultArguments()
        {
            if (_arg == null)
                CreateDefaultArguments();
        }

        private void DoProcessing()
        {
            if (_bytesPerPixel == 1)
                ProcessingGray();
            else if (_bytesPerPixel == 3)
                ProcessingRGB();
        }

        protected abstract void ProcessingRGB();

        protected abstract void ProcessingGray();

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

        public abstract XmlElement ToXML(XmlDocument xmldoc);

        public void Dispose()
        {
            _arg = null;
            Reset();
        }

        public virtual void CreateDefaultArguments()
        {
        }
    }
}
