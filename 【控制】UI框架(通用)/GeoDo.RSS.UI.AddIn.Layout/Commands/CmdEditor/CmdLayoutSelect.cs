using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdLayoutSelect : GeoDo.RSS.Core.UI.Command
    {
        public CmdLayoutSelect()
            : base()
        {
            _id = 6003;
            _name = "LayoutSelect";
            _text = _toolTip = "选择";
        }

        public override void Execute()
        {
            ILayoutViewer host = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (host != null)
            {
                host.LayoutHost.CurrentTool = new ArrowTool();
                host.LayoutHost.Render();
            }
        }
    }
}
