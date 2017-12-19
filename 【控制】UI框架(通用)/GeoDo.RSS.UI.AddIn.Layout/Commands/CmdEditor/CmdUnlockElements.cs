using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdUnlockElements:GeoDo.RSS.Core.UI.Command
    {
        public CmdUnlockElements()
            : base()
        {
            _id = 6011;
            _name = "UnlockElements";
            _text = _toolTip = "元素解锁";
        }

        public override void Execute()
        {
            ILayoutViewer layout = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (layout == null)
                return;
            ILayoutRuntime runtime = layout.LayoutHost.LayoutRuntime;
            if (runtime.LockedElements == null || runtime.LockedElements.Length == 0)
                return;
            IElement[] eles  = runtime.LockedElements;
            foreach (IElement element in eles)
                element.IsLocked = false;
            layout.LayoutHost.Render();
        }
    }
}
