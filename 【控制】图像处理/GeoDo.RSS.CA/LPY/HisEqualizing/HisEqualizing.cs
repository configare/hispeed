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
    public class HisEqualizing : RgbProcessorByPixel
    {
        double[][] transRate = null;
        public HisEqualizing()
            : base()
        {
            Init();
        }

        public HisEqualizing(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "直方图均衡化";
        }

        public HisEqualizing(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        protected override unsafe void BeforeProcess()
        {
            byte* ptr0 = (byte*)_pdata.Scan0;
            int bytesPerPixel = _bytesPerPixel;
            transRate = histGetGradesrRates(ptr0, _pdata.Width, _pdata.Height, bytesPerPixel);
        }

        protected override void Process(ref byte pixelValue)
        {
            int greyValue = pixelValue;
            greyValue = (int)((transRate[0][greyValue]) * 255);
            pixelValue = ColorMath.FixByte(greyValue);
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            int r = pixelRed;
            int g = pixelGreen;
            int b = pixelBlue;
            r = (int)((transRate[0][r]) * 255);
            g = (int)((transRate[1][g]) * 255);
            b = (int)((transRate[2][b]) * 255);
            pixelRed = ColorMath.FixByte(r);
            pixelGreen = ColorMath.FixByte(g);
            pixelBlue = ColorMath.FixByte(b);
        }

        public override void CreateDefaultArguments()
        {
            _arg = new SimpleRgbProcessorArg();
        }

        private unsafe double[][] histGetGradesrRates(byte* img, int w, int h, int bytesPerPixel)
        {
            int[][] g = new int[bytesPerPixel][];
            for (int i = 0; i < g.Length; ++i)
            {
                g[i] = new int[256];
            }
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    for (int band = 0; band < bytesPerPixel; ++band)
                    {
                        ++g[band][*(img + w * i + j * bytesPerPixel)];
                    }
                }
            }
            int pixelNum = w * h;
            double[][] imgRate = new double[3][] { new double[256], new double[256], new double[256] };
            for (int band = 0; band < bytesPerPixel; ++band)
            {
                for (int n = 0; n < 256; ++n)
                {

                    imgRate[band][n] = g[band][n] / ((double)pixelNum);

                }
            }
            double[][] imgRatecon = new double[3][] { new double[256], new double[256], new double[256] };
            for (int band = 0; band < bytesPerPixel; ++band)
            {
                for (int i = 0; i < 256; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        imgRatecon[band][i] += imgRate[band][j];
                    }
                }
            }
            return imgRatecon;
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            xmlElem.InnerText = "true";
            return xmlElem;
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            HisEqualizing pro = new HisEqualizing();
            return pro;
        }
    }
}
