using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    [Export(typeof(ICommand))]
    public class CommandReset:BaseCommandImgPro
    {
        public CommandReset()
        {
            _id = 3016;
            _name = "Reset";
            _text = "恢复图像";
            _toolTip = "恢复图像";
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            drawing.DataProvider.ResetStretcher();
            drawing.Reset();
            drawing.RgbProcessorStack.Clear();
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
        }

        protected override Core.CA.IRgbProcessor GetRgbProcessor()
        {
            throw new NotImplementedException();
        }
    }
}
