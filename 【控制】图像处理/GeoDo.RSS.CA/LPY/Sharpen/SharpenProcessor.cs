using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public class SharpenProcessor : RgbProcessorByWnd
    {
        private int[] _templates = { -1, -4,  -7,   -4,  -1,
                                     -4, -16, -26,  -16, -4, 
                                     -7, -26,  505, -26, -7,
                                     -4, -16, -26,  -16, -4, 
                                     -1, -4,  -7,   -4,  -1 };
        private const int SIZE = 25;

        public SharpenProcessor()
            : base()
        {
            Init();
        }

        public SharpenProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public SharpenProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }
        private void Init()
        {
            _name = "锐化";
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _actualArg.WndHeight = _actualArg.WndWidth = 5;
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {
            int sumvalue = 0;
            for (int band = 0; band < _bytesPerPixel; ++band)
            {
                for (int i = 0; i < SIZE; i++)
                {
                    sumvalue += (wndPixels[band][i]) * _templates[i];
                }
                sumvalue /= 273;
                if (sumvalue > 255)
                    sumvalue = 255;
                else if (sumvalue < 0)
                    sumvalue = 0;
                *(ptr++) = (byte)sumvalue;
                sumvalue = 0;
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            xmlElem.InnerText = "true";
            return xmlElem;
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            SharpenProcessor pro = new SharpenProcessor();
            return pro;
        }

    }
}

