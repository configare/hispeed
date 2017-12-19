using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    [Export(typeof(ICommand))]
    public class CanvasCommandZoomIn : CanvasCommand
    {
        public CanvasCommandZoomIn()
        {
            _id = 1001;
            _name = "CommandZoomIn";
            _text = _toolTip = "放大";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
                v.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomIn);
        }
    }
}
