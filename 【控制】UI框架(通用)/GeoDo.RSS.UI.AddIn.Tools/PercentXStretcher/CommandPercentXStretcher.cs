using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandPercentXStretcher : Command
    {
        public CommandPercentXStretcher()
            : base()
        {
            _name = "CommandPercentXStretcher";
            _text = _toolTip = "3%拉升";
            _id = 7111;
        }

        public override void Execute()
        {
            Execute("0.03");
        }

        public override void Execute(string argument)
        {
            float percent = float.Parse(argument);
            IRasterDrawing drawing = GetRasterDrawing();
            if (drawing == null)
                return;
            if (drawing.TileBitmapProvider.TileCountOfLoading > 0)
                return;
            PercentXStretcher st = new PercentXStretcher(_smartSession);
            object[] sts = st.GetStretcher(drawing.DataProvider, drawing.SelectedBandNos, percent);
            int idx = 0;
            foreach (int bandNo in drawing.SelectedBandNos)
            {
                drawing.DataProvider.GetRasterBand(bandNo).Stretcher = sts[idx++];
            }
            drawing.Reset();
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            viewer.Canvas.Refresh(enumRefreshType.All);
        }

        private IRasterDrawing GetRasterDrawing()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            return drawing;
        }
    }
}
