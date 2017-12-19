using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsUnGroup:GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsUnGroup()
            : base()
        {
            _id = 6021;
            _name = "ElementsUnGroup";
            _text = _toolTip = "取消编组";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Ungroup();
            viewer.LayoutHost.Render();
        }
    }
}
