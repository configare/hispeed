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
    public class CmdLockElement:GeoDo.RSS.Core.UI.Command
    {
        public CmdLockElement()
            : base()
        {
            _id = 6010;
            _name = "LockElement";
            _text = _toolTip = "锁住要素";
        }

        public override void Execute()
        {
            ILayoutViewer layout = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (layout == null)
                return;
            ILayoutRuntime runtime = layout.LayoutHost.LayoutRuntime;
            if (runtime.Selection == null || runtime.Selection.Length == 0)
                return;
            IElement[] elements = layout.LayoutHost.LayoutRuntime.Selection;
            foreach (IElement element in elements)
                element.IsLocked = true;                
            layout.LayoutHost.Render();
        }
    }
}
