using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandRemoveBkgrd : Command
    {
        public CommandRemoveBkgrd()
            :base()
        {
            _id = 4045;
            _text = _toolTip = "移除默认地图背景";
        }

        public override void Execute()
        {
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            ILayer layer = cv.Canvas.LayerContainer.GetByName("海陆背景");
            if (layer == null)
                return;
            cv.Canvas.LayerContainer.Layers.Remove(layer);
            cv.Canvas.Refresh(enumRefreshType.All);
        }
    }
}
