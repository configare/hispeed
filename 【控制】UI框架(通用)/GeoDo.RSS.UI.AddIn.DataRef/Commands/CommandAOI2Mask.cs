using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAOI2Mask:Command
    {
        public CommandAOI2Mask()
            : base()
        {
            _id = 4042;
            _text = _toolTip = "AOI转换为蒙板";
        }

        public override void Execute()
        {
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            int[] aoi = cv.AOIProvider.GetIndexes();
            if (aoi == null || aoi.Length == 0)
                return;
            TryUpdateMaskLayer(cv,aoi);
        }

        private void TryUpdateMaskLayer(ICanvasViewer cv,int[] aoi)
        {
            ILayer lyr = cv.Canvas.LayerContainer.GetByName("蒙板层");
            if (lyr == null)
            {
                lyr = new MaskLayer();
                cv.Canvas.LayerContainer.Layers.Add(lyr);
            }
            IMaskLayer maskLayer = lyr as IMaskLayer;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            CoordEnvelope coordEnvelope = drawing.OriginalEnvelope.Clone();
            Size rasterSize = drawing.Size;
            maskLayer.Update(Color.Black, rasterSize, coordEnvelope, false, aoi);
            cv.Canvas.Refresh(enumRefreshType.All);
        }
    }
}
