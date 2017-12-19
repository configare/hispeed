using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    [Export(typeof(ICommand))]
    public class CanvasCommandZoomXScale : CanvasCommand
    {
        public CanvasCommandZoomXScale()
        {
            _id = 1005;
            _name = "ZoomXScale";
            _text = _toolTip = "按比例缩放";
        }

        public override void Execute(string argument)
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (v != null)
            {
                v.Canvas.Scale = float.Parse(argument);
                v.Canvas.CurrentViewControl = new DefaultControlLayer();
            }
        }
    }

}
