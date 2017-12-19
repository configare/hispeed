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
    public class CanvasCommandZoomSiteScale : CanvasCommand
    {
        public CanvasCommandZoomSiteScale()
        {
            _id = 1004;
            _name = "ZoomSiteScale";
            _text = _toolTip = "适合窗口";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v != null)
            {
                if (v.ActiveObject is IRasterDrawing)
                {
                    v.Canvas.CurrentEnvelope = (v.ActiveObject as IRasterDrawing).OriginalEnvelope;
                    v.Canvas.Refresh(enumRefreshType.All);
                }
            }
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
