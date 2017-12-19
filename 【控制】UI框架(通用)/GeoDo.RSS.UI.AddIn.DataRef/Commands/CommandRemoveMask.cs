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
    public class CommandRemoveMask:Command
    {
        public CommandRemoveMask()
            : base()
        {
            _id = 4043;
            _text = _toolTip = "取消蒙板";
        }

        public override void Execute()
        {
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            ILayer lyr = cv.Canvas.LayerContainer.GetByName("蒙板层");
            if (lyr != null)
            {
                cv.Canvas.LayerContainer.Layers.Remove(lyr);
                cv.Canvas.Refresh(enumRefreshType.All);
            }
        }
    }
}
