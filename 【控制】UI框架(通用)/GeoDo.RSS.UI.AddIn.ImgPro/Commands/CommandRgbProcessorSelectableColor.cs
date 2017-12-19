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
    public class CommandRgbProcessorSelectableColor:BaseCommandImgPro
    {
        public CommandRgbProcessorSelectableColor() 
        {
            _id = 3014;
            _name = "SelectableColor";
            _text = "可选颜色";
            _toolTip = "可选颜色";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new RgbProcessorSelectableColor();
        }
    }
}
