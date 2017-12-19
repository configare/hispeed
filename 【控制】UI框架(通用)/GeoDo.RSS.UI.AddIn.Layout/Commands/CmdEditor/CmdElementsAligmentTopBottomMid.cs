using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsAligmentTopBottomMid : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentTopBottomMid()
            : base()
        {
            _id = 6025;
            _name = "ElementsAligmentTopBottomMid";
            _text = _toolTip = "上下居中";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.TopBottomMid);
            viewer.LayoutHost.Render();
        }
    }
}
