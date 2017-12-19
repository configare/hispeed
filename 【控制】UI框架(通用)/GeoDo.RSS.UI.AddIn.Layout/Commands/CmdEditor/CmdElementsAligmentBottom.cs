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
    public class CmdElementsAligmentBottom:GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsAligmentBottom()
            : base()
        {
            _id = 6020;
            _name = "CmdElementsAligmentBottom";
            _text = _toolTip = "底端对齐";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            viewer.LayoutHost.Aligment(RSS.Layout.enumElementAligment.Bottom);
            viewer.LayoutHost.Render();
        }
    }
}
