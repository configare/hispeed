using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsAligmentLeftRightMid : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentLeftRightMid()
            : base()
        {
            _id = 6022;
            _name = "ElementsAligmentLeftRightMid";
            _text = _toolTip = "左右居中";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.LeftRightMid);
            viewer.LayoutHost.Render();
        }
    }
}
