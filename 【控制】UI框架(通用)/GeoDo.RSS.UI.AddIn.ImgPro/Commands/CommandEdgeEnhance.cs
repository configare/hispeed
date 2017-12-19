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
    public class CommandEdgeEnhance:BaseCommandImgPro
    {
        public CommandEdgeEnhance()
        {
            _id = 3001;
            _name = "EdgeEnhance";
            _text = "边缘提取";
            _toolTip = "边缘提取";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new EdgeEnhance();
        }
    }
}
