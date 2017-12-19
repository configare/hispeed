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
    public class CommandNSMCExpEnhance:BaseCommandImgPro
    {
        public CommandNSMCExpEnhance()
        {
            _id = 3022;
            _name = "ExpEnhance";
            _text = "指数增强";
            _toolTip = "指数增强";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new NSMCExpEnhanceProcessor();
        } 
    }
}
