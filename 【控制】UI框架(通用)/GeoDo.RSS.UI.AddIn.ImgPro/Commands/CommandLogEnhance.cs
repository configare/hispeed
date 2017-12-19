using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.CA;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandLogEnhance : BaseCommandImgPro
    {
        public CommandLogEnhance()
        {
            _id = 3020;
            _name = "LogEnhance";
            _text = _toolTip = "对数增强";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorLogEnhance();
        }
    }
}
