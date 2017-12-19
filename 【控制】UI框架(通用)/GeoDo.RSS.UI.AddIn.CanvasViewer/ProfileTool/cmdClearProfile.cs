using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer.ProfileTool
{
    /// <summary>
    /// 清除剖面图工具
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdClearProfile : Command
    {
        public cmdClearProfile()
            : base()
        {
            _id = 2104;
            _text = _name = "清除剖面图";
        }

        public override void Execute()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (v == null)
                return;
            ICanvas canvas = v.Canvas;
            ProfileLayer layer = canvas.LayerContainer.GetByName("ProfileLayer") as ProfileLayer;
            if (layer != null)
            {
                layer.Dispose();
                canvas.LayerContainer.Layers.Remove(layer);
            }
            canvas.Refresh(enumRefreshType.FlyLayer);
        }
    }
}
