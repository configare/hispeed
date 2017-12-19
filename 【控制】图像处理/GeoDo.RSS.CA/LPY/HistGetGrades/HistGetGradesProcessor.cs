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
    public class HistGetGradesProcessor : RgbProcessorByPixel
    {
        private HistGetGradesArg _actualArgs = null;
        int[][] lh;
        public HistGetGradesProcessor()
            : base()
        {
            Init();
        }

        public HistGetGradesProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public HistGetGradesProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "直方图拉伸";
        }

        protected override unsafe void BeforeProcess()
        {
            _actualArgs = _arg as HistGetGradesArg;
            byte* ptr0 = (byte*)_pdata.Scan0;
            int w = _pdata.Width;
            int h = _pdata.Height;
            int bytesPerPixel = _bytesPerPixel;
            int pixelnum = w * h;
            int[][] grams = histGetGrades(ptr0, w, h, bytesPerPixel);
            float[][] transArg = new float[][]{new float []{_actualArgs.LhRL*pixelnum,_actualArgs.LhRH*pixelnum},
                new float []{_actualArgs.LhGL*pixelnum,_actualArgs.LhGH*pixelnum},new float []{_actualArgs.LhBL*pixelnum,_actualArgs.LhBH*pixelnum}};
            lh = histGetBounds(grams, transArg);

        }

        public override void CreateDefaultArguments()
        {
            _arg = new HistGetGradesArg();
        }

        protected override void Process(ref byte pixelValue)
        {

        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            int r = (int)((pixelRed - lh[0][0]) * (255.0 / (lh[0][1] - lh[0][0])));
            int g = (int)((pixelGreen - lh[1][0]) * (255.0 / (lh[1][1] - lh[1][0])));
            int b = (int)((pixelBlue - lh[2][0]) * (255.0 / (lh[2][1] - lh[2][0])));
            if (r < 0) r = 0;
            else if (r > 255) r = 255;
            if (g < 0) g = 0;
            else if (g > 255) g = 255;
            if (b < 0) b = 0;
            else if (b > 255) b = 255;
            pixelRed = (byte)r;
            pixelGreen = (byte)g;
            pixelBlue = (byte)b;
        }

        private unsafe int[][] histGetGrades(byte* img, int w, int h, int bytesPerPixel)
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
            return g;
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new HistGetGradesArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            HistGetGradesProcessor pro = new HistGetGradesProcessor();
            pro._actualArgs = new HistGetGradesArg();
            pro._actualArgs.LhRL = (float)Convert.ToDouble(elem.ChildNodes[0].InnerText);
            pro._actualArgs.LhRH = (float)Convert.ToDouble(elem.ChildNodes[1].InnerText);
            pro._actualArgs.LhGL = (float)Convert.ToDouble(elem.ChildNodes[2].InnerText);
            pro._actualArgs.LhGH = (float)Convert.ToDouble(elem.ChildNodes[3].InnerText);
            pro._actualArgs.LhBL = (float)Convert.ToDouble(elem.ChildNodes[4].InnerText);
            pro._actualArgs.LhBH = (float)Convert.ToDouble(elem.ChildNodes[5].InnerText);
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        private int[][] histGetBounds(int[][] gram, float[][] transArg)
        {
            int[][] lh = new int[3][] { new int[2], new int[2], new int[2] };
            for (int k = 0; k < 3; k++)
            {
                for (int i = 0, sum = 0; i < gram[k].Length; ++i)
                {
                    sum += gram[k][i];
                    if (sum >= transArg[k][0])
                    {
                        lh[k][0] = i;
                        break;
                    }
                }
                for (int i = gram[k].Length - 1, sum = 0;
                    i >= 0; --i)
                {
                    sum += gram[k][i];
                    if (sum >= transArg[k][1])
                    {
                        lh[k][1] = i;
                        break;
                    }
                }
            }
            return lh;
        }
    }
}