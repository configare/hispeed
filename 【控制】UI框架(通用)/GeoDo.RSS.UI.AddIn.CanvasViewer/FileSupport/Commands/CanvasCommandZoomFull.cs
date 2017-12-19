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
    public class CanvasCommandZoomFull : CanvasCommand
    {
        public CanvasCommandZoomFull()
        {
            _id = 1000;
            _name = "ZoomFull";
            _text = _toolTip = "100%";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
            {
                v.Canvas.Scale = 1f;
                v.Canvas.CurrentViewControl = new DefaultControlLayer();
            }
        }
    }
}
