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
    public class CommandCurveAdjustProcessor:BaseCommandImgPro
    {
        public CommandCurveAdjustProcessor()
        {
            _id = 3010;
            _name = "CurveAdjust";
            _text = "曲线调整";
            _toolTip = "曲线调整";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new CurveAdjustProcessor();
        }
    }
}
