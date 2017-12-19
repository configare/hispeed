using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandRedo : BaseCommandImgPro
    {
        public CommandRedo()
        {
            _id = 3017;
            _name = "Redo";
            _text = "重做";
            _toolTip = "重做";
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            drawing.RgbProcessorStack.ReProcess();
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            throw new NotImplementedException();
        }
    }
}
