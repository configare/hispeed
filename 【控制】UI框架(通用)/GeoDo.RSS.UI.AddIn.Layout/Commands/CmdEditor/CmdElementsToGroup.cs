using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.Elements;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsToGroup : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsToGroup()
            : base()
        {
            _id = 6016;
            _name = "ElementsToGroup";
            _text = _toolTip = "元素编组";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Group();
            viewer.LayoutHost.Render();
        }
    }
}
