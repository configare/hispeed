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
    public class CommandRgbProcessorMiddleFilter:BaseCommandImgPro
    {
        public CommandRgbProcessorMiddleFilter()
        {
            _id = 3011;
            _name = "MiddleFilter";
            _text = "中值滤波";
            _toolTip = "中值滤波";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorMiddleFilter();
        }
    }
}
