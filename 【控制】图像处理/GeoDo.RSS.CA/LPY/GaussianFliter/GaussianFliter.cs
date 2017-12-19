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
    public class GaussianFliter : RgbProcessorByWnd
    {
        private GaussianFliterARG _actualArgs = null;
        public double[] _conv2 = null;
        private decimal _gaussianSigma = 0;
        private int _gaussianRadius = 0;

        public GaussianFliter()
            : base()
        {
            Init();
        }

        public GaussianFliter(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public GaussianFliter(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "高斯滤波";
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _actualArgs = _arg as GaussianFliterARG;
            _gaussianRadius = _actualArgs.GaussianRadius;
            _gaussianSigma = _actualArgs.GaussianSigma;
            convComputation(_gaussianRadius);
        }

        private void convComputation(int n)
        {
            double[,] conv = new double[n, n];
            double[] con1 = new double[n * n];
            double hg = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {

                    int u = i - 1;
                    int v = j - 1;

                    conv[i, j] = (float)Math.Exp(-((u * u) + (v * v)) / (2 * (float)_gaussianSigma * (float)_gaussianSigma));
                    hg += conv[i, j];
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    con1[i * n + j] = conv[i, j] / hg;
                }
            }
            _conv2 = con1;
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {
            double sumvalue = 0;

            //Convolution kernel
            for (int band = 0; band < _bytesPerPixel; ++band)
            {
                for (int i = 0; i < _gaussianRadius * _gaussianRadius; i++)
                {
                    sumvalue += (wndPixels[band][i]) * _conv2[i];
                }
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
            if (_actualArgs == null)
            {
                _actualArgs = new GaussianFliterARG();
            }
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            GaussianFliter pro = new GaussianFliter();
            pro._actualArgs = new GaussianFliterARG();
            pro._actualArgs.GaussianRadius = Convert.ToInt32(elem.ChildNodes[0].InnerText);
            pro._actualArgs.GaussianSigma = Convert.ToDecimal(elem.ChildNodes[1].InnerText);
            pro._actualArgs.WndHeight = Convert.ToInt32(elem.ChildNodes[2].InnerText);
            pro._actualArgs.WndWidth = Convert.ToInt32(elem.ChildNodes[3].InnerText);
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new GaussianFliterARG();
        }
    }
}
