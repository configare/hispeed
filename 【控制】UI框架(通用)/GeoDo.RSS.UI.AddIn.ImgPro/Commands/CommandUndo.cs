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
    public class CommandUndo : BaseCommandImgPro
    {
        public CommandUndo()
        {
            _id = 3018;
            _name = "Undo";
            _text = "撤销";
            _toolTip = "撤销";
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            drawing.RgbProcessorStack.UnProcess();
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            throw new NotImplementedException();
        }
    }
}
