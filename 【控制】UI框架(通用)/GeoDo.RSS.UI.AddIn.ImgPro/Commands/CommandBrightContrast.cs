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
    public class CommandBrightContrast:BaseCommandImgPro
    {
        public CommandBrightContrast()
        {
            _id = 3007;
            _name = "BrightContrast";
            _text = "亮度/对比度";
            _toolTip = "亮度/对比度";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new BrightContrastProcessor();
        }
    }
}
