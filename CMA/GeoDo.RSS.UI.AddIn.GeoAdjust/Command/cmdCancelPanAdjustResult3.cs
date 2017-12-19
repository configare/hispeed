using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdCancelPanAdjustResult3 : Command
    {
        public cmdCancelPanAdjustResult3()
            : base()
        {
            _id = 30204;
            _text = _name = "取消平移校正";
        }

        public override void Execute()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            IGeoPanAdjust adjust = rd as IGeoPanAdjust;
            adjust.Cancel();
            canViewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }
    }
}
