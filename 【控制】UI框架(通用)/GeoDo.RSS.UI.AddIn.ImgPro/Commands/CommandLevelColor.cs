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
    public class CommandLevelColor:BaseCommandImgPro
    {
        public CommandLevelColor()
        {
            _id = 3008;
            _name = "LeverColor";
            _text = "色阶";
            _toolTip = "色阶";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new LevelColorProcessor();
        } 
    }
}
