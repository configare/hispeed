using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.CA;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandExponentEnhance : BaseCommandImgPro
    {
        public CommandExponentEnhance()
        {
            _id = 3021;
            _name = "ExponentEnhance";
            _text = _toolTip = "指数增强";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new ExponentEnhanceProcessor();
        }
    }
}
