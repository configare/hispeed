using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsAligmentHorizontal : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentHorizontal()
            : base()
        {
            _id = 6026;
            _name = "CmdElementsAligmentHorizontal";
            _text = _toolTip = "横向分布";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.Horizontal);
            viewer.LayoutHost.Render();
        }
    }
}
