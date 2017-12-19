using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdClearSelection : GeoDo.RSS.Core.UI.Command
    {
        public CmdClearSelection()
            : base()
        {
            _id = 6007;
            _name = "ClearSelection";
            _text = _toolTip = "清空所有选择集";
        }

        public override void Execute()
        {
            ILayoutViewer host = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (host == null)
                return;
            ILayoutRuntime runtime = host.LayoutHost.LayoutRuntime;
            if (runtime.Selection != null)
                runtime.ClearSelection();
            host.LayoutHost.Render();
        }
    }
}
