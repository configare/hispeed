using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdElementsRotate : GeoDo.RSS.Core.UI.Command
    {
        public CmdElementsRotate()
            : base()
        {
            _id = 6030;
            _name = "Rotate";
            _text = _toolTip = "整饰元素旋转";
        }

        public override void Execute(string argument)
        {
            if (String.IsNullOrEmpty(argument))
                return;
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            float angle;
            bool ok = float.TryParse(argument, out angle);
            if (!ok)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            Rotate(host.LayoutRuntime, angle);
            host.Render();
        }

        private void Rotate(ILayoutRuntime runtime, float angle)
        {
            if (runtime == null)
                return;
            IElement[] eles = runtime.Selection;
            if (eles == null || eles.Length == 0)
                return;
            foreach (IElement ele in eles)
            {
                if (!(ele is SizableElement))
                    continue;
                (ele as SizableElement).Angle = angle;
            }
        }
    }
}
