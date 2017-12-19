using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.CA;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandRgbProcessorReplacedColor:BaseCommandImgPro
    {
        public CommandRgbProcessorReplacedColor()
        {
            _id = 3012;
            _name = "ReplacedColor";
            _text = "颜色替换";
            _toolTip = "颜色替换";
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            //by chennan 上海 修改放大、缩小后，点击颜色调整出现红叉错误
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
                v.Canvas.CurrentViewControl = new DefaultControlLayer();
            //
            return new RgbProcessorReplacedColor();
        }
    }
}
