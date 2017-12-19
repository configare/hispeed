using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 激活剖面图工具
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdActiveProfile : Command
    {
        public cmdActiveProfile()
            : base()
        {
            _id = 2103;
            _text = _name = "激活剖面图";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v == null)
                return;
            IRasterDrawing drawing = v.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProviderCopy == null)
                return;
            ICanvas canvas = v.Canvas;
            ProfileLayer layer = canvas.LayerContainer.GetByName("ProfileLayer") as ProfileLayer;
            if (layer == null)
            {
                layer = new ProfileLayer();
                layer.Name = "ProfileLayer";
                canvas.LayerContainer.Layers.Add(layer);
            }
            layer.NewProfile();
            canvas.Refresh(enumRefreshType.FlyLayer);
        }
    }
}
