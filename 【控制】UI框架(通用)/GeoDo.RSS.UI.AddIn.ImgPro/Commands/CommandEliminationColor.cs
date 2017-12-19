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
    public class CommandEliminationColor:BaseCommandImgPro
    {
        public CommandEliminationColor()
        {
            _id = 3002;
            _name = "EliminationColor";
            _text = "去色";
            _toolTip = "去色";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new EliminationColorProcessor();
        }
    }
}
