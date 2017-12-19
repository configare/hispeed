using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.CA;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandRgbProcessorWeightedAveFilter:BaseCommandImgPro
    {
        public CommandRgbProcessorWeightedAveFilter()
        {
            _id = 3015;
            _name = "WeightedAveFilter";
            _text = "自定义滤波";
            _toolTip = "自定义滤波";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorWeightedAveFilter();
        }
    }
}
