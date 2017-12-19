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
    public class CommandSharpen:BaseCommandImgPro
    {
        public CommandSharpen()
        {
            _id = 3006;
            _name = "Sharpen";
            _text = "锐化";
            _toolTip = "锐化";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new SharpenProcessor();
        }
    }
}
