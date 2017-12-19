using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdStartPanAdjusting3:Command
    {
        private EventHandler _finishedHandler;

        public cmdStartPanAdjusting3()
        {
            _id = 30202;
            _name = "GeoAdjustByPanKeepImage";
            _text = _toolTip = "";
            _finishedHandler = new EventHandler(AdjustFinished);
        }

        public override void Execute()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            (rd as IGeoPanAdjust).Start();
            IRasterDataProvider rdp = rd.DataProvider;
            canViewer.Canvas.CurrentViewControl = new PanAdjustControlLayer();
            (canViewer.Canvas.CurrentViewControl as PanAdjustControlLayer).AdjustFinishedHandler += _finishedHandler;
            canViewer.Canvas.Refresh(enumRefreshType.All);
        }

        private void AdjustFinished(object sender, EventArgs e)
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            (canViewer.Canvas.CurrentViewControl as PanAdjustControlLayer).AdjustFinishedHandler -= _finishedHandler;
            canViewer.Canvas.CurrentViewControl = new DefaultControlLayer();
            canViewer.Canvas.Refresh(enumRefreshType.All);
        }

        public override void Execute(string argument)
        {
        }
    }
}
