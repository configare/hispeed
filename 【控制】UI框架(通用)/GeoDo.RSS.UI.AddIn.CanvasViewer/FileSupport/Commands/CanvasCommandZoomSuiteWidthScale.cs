using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    [Export(typeof(ICommand))]
    public class CanvasCommandZoomSuiteWidthScale : CanvasCommand
    {
        public CanvasCommandZoomSuiteWidthScale()
        {
            _id = 1007;
            _name = "ZoomSiteWidthScale";
            _text = _toolTip = "适合窗口【按宽度】";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
            {
                if (v.ActiveObject is IRasterDrawing)
                {
                    (v.Canvas as GeoDo.RSS.Core.DrawEngine.GDIPlus.Canvas).SetToFitWidth();
                }
            }
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
