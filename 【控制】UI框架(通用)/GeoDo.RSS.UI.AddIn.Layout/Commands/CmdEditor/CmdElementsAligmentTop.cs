using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.Elements;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsAligmentTop:GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentTop()
            : base()
        {
            _id = 6019;
            _name = "ElementsAligmentTop";
            _text = _toolTip = "顶端对齐";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            if (viewer.LayoutHost == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.Top);
            viewer.LayoutHost.Render();
        }
    }
}
