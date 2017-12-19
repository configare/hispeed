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
    public class CmdElementsAligmentLeft:GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentLeft()
            : base()
        {
            _id = 6017;
            _name = "CmdElementsAligmentLeft";
            _text = _toolTip = "左对齐";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.Left);
            viewer.LayoutHost.Render();
        }
    }
}
