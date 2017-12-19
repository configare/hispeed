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
    public class EliminationColorProcessor : RgbProcessorByPixel
    {
        public EliminationColorProcessor()
            : base()
        {
            Init();
        }

        public EliminationColorProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            Init();
        }

        public EliminationColorProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            Init();
        }

        private void Init()
        {
            _name = "去色";
        }

        protected override void Process(ref byte pixelValue)
        {
            pixelValue = ColorMath.FixByte(pixelValue);
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            double pixelHue = 0, pixelSaturation = 0, pixelLum = 0;
            int min = Math.Min(pixelRed, Math.Min(pixelGreen, pixelBlue));
            int max = Math.Max(pixelRed, Math.Max(pixelGreen, pixelBlue));
            pixelLum = (min + max) / 2d / 255;
            ColorMath.HSL2RGB(pixelHue, pixelSaturation, pixelLum, ref pixelRed, ref pixelGreen, ref pixelBlue);
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            xmlElem.InnerText = "true";
            return xmlElem;
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            EliminationColorProcessor pro = new EliminationColorProcessor();
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new SimpleRgbProcessorArg();
        }
    }
}
