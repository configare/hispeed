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
    public class CurveAdjustProcessor : RgbProcessorByPixel
    {
        protected CurveAdjustProcessorArg _actualArg = null;
        private byte[] _rgbValues = null;
        private byte[] _redValues = null;
        private byte[] _greenValues = null;
        private byte[] _blueValues = null;
        private bool _needRgb = false;
        private bool _needRed = false;
        private bool _needGreen = false;
        private bool _needBlue = false;

        public CurveAdjustProcessor()
            : base()
        {
            _name = "曲线调整";
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _actualArg = _arg as CurveAdjustProcessorArg;
            _rgbValues = _actualArg.RGB.Values;
            _redValues = _actualArg.Red.Values;
            _greenValues = _actualArg.Green.Values;
            _blueValues = _actualArg.Blue.Values;
            _needRgb = !_actualArg.RGB.IsEmpty;
            _needRed = !_actualArg.Red.IsEmpty;
            _needGreen = !_actualArg.Green.IsEmpty;
            _needBlue = !_actualArg.Blue.IsEmpty;
        }

        protected override void Process(ref byte pixelValue)
        {
            if (!_actualArg.RGB.IsEmpty)
                pixelValue = _actualArg.RGB.Values[pixelValue];
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_needRgb)
            {
                pixelBlue = _rgbValues[pixelBlue];
                pixelGreen = _rgbValues[pixelGreen];
                pixelRed = _rgbValues[pixelRed];
            }
            if (_needRed)
                pixelRed = _redValues[pixelRed];
            if (_needGreen)
                pixelGreen = _greenValues[pixelGreen];
            if (_needBlue)
                pixelBlue = _blueValues[pixelRed];
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArg == null)
                _actualArg = new CurveAdjustProcessorArg();
            return _actualArg.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            CurveAdjustProcessor pro = new CurveAdjustProcessor();
            pro._actualArg = new CurveAdjustProcessorArg();
            switch (elem.ChildNodes[0].Name.ToString())
            {
                case "RGB":
                    pro._actualArg.RGB.Values = new byte[elem.ChildNodes[0].ChildNodes.Count];
                    break;
                case "Red":
                    pro._actualArg.Red.Values = new byte[elem.ChildNodes[0].ChildNodes.Count];
                    break;
                case "Green":
                    pro._actualArg.Green.Values = new byte[elem.ChildNodes[0].ChildNodes.Count];
                    break;
                case "Blue":
                    pro._actualArg.Blue.Values = new byte[elem.ChildNodes[0].ChildNodes.Count];
                    break;
            }
            for (int i = 0; i < elem.ChildNodes[0].ChildNodes.Count; i++)
            {
                switch (elem.ChildNodes[0].Name.ToString())
                {
                    case "RGB":
                        pro._actualArg.RGB.Values[i] = Convert.ToByte(elem.ChildNodes[0].ChildNodes[i].InnerText);
                        break;
                    case "Red":
                        pro._actualArg.Red.Values[i] = Convert.ToByte(elem.ChildNodes[0].ChildNodes[i].InnerText);
                        break;
                    case "Green":
                        pro._actualArg.Green.Values[i] = Convert.ToByte(elem.ChildNodes[0].ChildNodes[i].InnerText);
                        break;
                    case "Blue":
                        pro._actualArg.Blue.Values[i] = Convert.ToByte(elem.ChildNodes[0].ChildNodes[i].InnerText);
                        break;
                }
            }
            pro.Arguments = pro._actualArg;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new CurveAdjustProcessorArg();
        }
    }
}
