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
    public class CommandRgbProcessorAverageFilter:BaseCommandImgPro
    {
        public CommandRgbProcessorAverageFilter()
        {
            _id = 3009;
            _name = "AverageFilter";
            _text = "均值滤波";
            _toolTip = "均值滤波";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorAverageFilter();
        }
    }
}
