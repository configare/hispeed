using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.VectorDrawing;


namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 清除量测工具
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CanvasCommandClearMeasure : Command
    {
        public CanvasCommandClearMeasure()
            : base()
        {
            _id = 2102;
            _text = _name = "清除量测";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v == null)
                return;
            ICanvas canvas = v.Canvas;
            MeasureLayer layer = canvas.LayerContainer.GetByName("MeasureLayer") as MeasureLayer;
            if (layer != null)
            {
                layer.Dispose();
                canvas.LayerContainer.Layers.Remove(layer);
            }
            canvas.Refresh(enumRefreshType.FlyLayer);
        }
    }
}
