using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.CA;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandRgbProcessorReversalColor:BaseCommandImgPro
    {
        public CommandRgbProcessorReversalColor()
        {
            _id = 3013;
            _name = "ReversalColor";
            _text = "反相";
            _toolTip = "反相";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorReversalColor();
        }
    }
}
