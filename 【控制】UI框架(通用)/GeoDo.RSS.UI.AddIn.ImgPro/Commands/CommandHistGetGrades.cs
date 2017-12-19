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
    public class CommandHistGetGrades:BaseCommandImgPro
    {
        public CommandHistGetGrades()
        {
            _id = 3005;
            _name = "HistGetGrades";
            _text = "直方图拉伸";
            _toolTip = "直方图拉伸";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new HistGetGradesProcessor();
        }
    }
}
