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
    public class CommandHisEqualizing:BaseCommandImgPro
    {
        public CommandHisEqualizing()
        {
            _id = 3004;
            _name = "HisEqualizing";
            _text = "直方图均衡";
            _toolTip = "直方图均衡";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new HisEqualizing();
        }
    }
}
