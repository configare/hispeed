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
    public class RgbProcessorWeightedAveFilter : RgbProcessorByWnd
    {
        private WeightedAveFilterArg _actualArgs = null;


        public RgbProcessorWeightedAveFilter()
            : base()
        {
            _name = "自定义滤波";
        }

        public RgbProcessorWeightedAveFilter(RgbProcessorArg arg)
            : base(arg)
        {
        }

        public RgbProcessorWeightedAveFilter(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
        }

        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _actualArgs = _arg as WeightedAveFilterArg;
        }

        protected override unsafe void Process(byte[][] wndPixels, ref byte* ptr)
        {

            int size = _actualArgs.WndHeight * _actualArgs.WndWidth;
            int sumPixels = 0;
            int sumWeights = 0;

            for (int band = 0; band < _bytesPerPixel; band++)
            {
                sumPixels = 0;
                sumWeights = 0;
                for (int i = 0; i < size; i++)
                {
                    sumPixels += wndPixels[band][i] * (byte)_actualArgs.Weight[i];
                    sumWeights += _actualArgs.Weight[i];
                }
                if (sumWeights == 0)
                    return;
                *(ptr++) = (byte)(sumPixels / sumWeights);
            }
        }

        public override XmlElement ToXML(XmlDocument xmldoc)
        {
            if (_actualArgs == null)
                _actualArgs = new WeightedAveFilterArg();
            return _actualArgs.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement elem)
        {
            RgbProcessorWeightedAveFilter pro = new RgbProcessorWeightedAveFilter();
            pro._actualArgs = new WeightedAveFilterArg();
            pro._actualArgs.WndHeight = Convert.ToInt32(elem.ChildNodes[0].InnerText);
            pro._actualArgs.WndWidth = Convert.ToInt32(elem.ChildNodes[1].InnerText);
            pro._actualArgs.Weight = new int[pro._actualArgs.WndHeight * pro._actualArgs.WndWidth];
            for (int i = 0; i < elem.ChildNodes[2].ChildNodes.Count; i++)
            {
                pro._actualArgs.Weight[i] = Convert.ToInt32(elem.ChildNodes[2].ChildNodes[i].InnerText);
            }
            pro.Arguments = pro._actualArgs;
            return pro;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new WeightedAveFilterArg();
        }
    }
}
