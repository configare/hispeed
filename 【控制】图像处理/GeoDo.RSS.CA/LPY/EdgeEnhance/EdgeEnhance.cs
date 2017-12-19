using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Xml;
using GeoDo.RSS.Core.CA;
using System.Threading.Tasks;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessor))]
    public class EdgeEnhance : RgbProcessorByWnd
    {
        int[,] _sobel = null;
        public EdgeEnhance()
            : base()
        {
            Init();
        }

        public EdgeEnhance(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public EdgeEnhance(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }
        private void Init()
        {
            _name = "边缘提取";
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            getsobel();
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {
            int max = 0;
            int length = wndPixels[_bytesPerPixel - 1].Length;

            for (int band = 0; band < _bytesPerPixel; ++band)
            {
                for (int i = 0; i < 8; i++)
                {
                    int d = 0;
                    for (int j = 0; j < length; j++)
                    {
                        d += _sobel[i, j] * wndPixels[band][j];
                    }
                    if (d > max) max = d;
                }
                if (max > 255)
                { max = 255; }
                else if (max < 0)
                    max = 0;
                *(ptr++) = (byte)max;
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
            EdgeEnhance pro = new EdgeEnhance();
            return pro;
        }

        private void getsobel()
        {
            //Sobel算子转变后得到
            int[,] s = new int[,]{
		    {-1,-2,-1,0,0,0,1,2,1},
		    {0,-1,-2,1,0,-1,2,1,0},
		    {1,0,-1,2,0,-2,1,0,-1},
		    {2,1,0,1,0,-1,0,-1,-2},
		    {1,2,1,0,0,0,-1,-2,-1},
		    {0,1,2,-1,0,1,-2,-1,0},
		    {-1,0,1,-2,0,2,-1,0,1},
		    {-2,-1,0,-1,0,1,0,1,2}
            };
            _sobel = s;
        }

    }
}
