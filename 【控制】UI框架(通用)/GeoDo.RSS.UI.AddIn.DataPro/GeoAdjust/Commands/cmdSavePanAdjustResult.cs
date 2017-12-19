using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdSavePanAdjustResult : Command
    {
        public cmdSavePanAdjustResult()
            : base()
        {
            _id = 30003;
            _text = _name = "保存平移校正";
        }

        public override void Execute()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            PanAdjustControlLayer layer = canViewer.Canvas.CurrentViewControl as PanAdjustControlLayer;
            if (layer == null)
                return;
            IGeoPanAdjust adjust = rd as IGeoPanAdjust;
            adjust.Save();
            canViewer.Canvas.CurrentViewControl = new DefaultControlLayer();
            canViewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }
    }
}
