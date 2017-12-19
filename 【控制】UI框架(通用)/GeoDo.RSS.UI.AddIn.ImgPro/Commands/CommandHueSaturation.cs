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
    public class CommandHueSaturation:BaseCommandImgPro
    {
        public CommandHueSaturation()
        {
            _id = 3000;
            _name = "HueSaturation";
            _text = "色相/饱和度";
            _toolTip = "色相/饱和度";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new HueSaturationProcess();
        }
    }
}
