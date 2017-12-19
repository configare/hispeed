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
    public class CanvasCommandZoomPan : CanvasCommand
    {
        public CanvasCommandZoomPan()
        {
            _id = 1003;
            _name = "ZoomPan";
            _text = _toolTip = "漫游";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
                v.Canvas.CurrentViewControl = new DefaultControlLayer();
        }
    }
}
