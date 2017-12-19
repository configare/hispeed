using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsAligmentVertical : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentVertical()
            : base()
        {
            _id = 6027;
            _name = "CmdElementsAligmentVertical";
            _text = _toolTip = "纵向分布";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.Vertical);
            viewer.LayoutHost.Render();
        }
    }
}
