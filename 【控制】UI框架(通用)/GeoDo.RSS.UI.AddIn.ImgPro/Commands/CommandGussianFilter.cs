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
    public class CommandGussianFilter:BaseCommandImgPro
    {
        public CommandGussianFilter()
        {
            _id = 3003;
            _name = "GussianFilter";
            _text = "高斯滤波";
            _toolTip = "高斯滤波";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            return new GaussianFliter();
        }
    }
}
