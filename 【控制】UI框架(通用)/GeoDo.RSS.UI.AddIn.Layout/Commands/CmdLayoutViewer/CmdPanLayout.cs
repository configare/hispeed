using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdPanLayout : Command
    {
        public CmdPanLayout()
            : base()
        {
            _id = 6004;
            _name = "PanLayout";
            _text = _toolTip = "漫游专题图";
        }

        public override void Execute()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            if (view.LayoutHost == null)
                return;
            if (view.LayoutHost.LayoutRuntime == null)
                return;
            IElement[] elements = view.LayoutHost.LayoutRuntime.Selection;
            if (elements != null)
                view.LayoutHost.LayoutRuntime.ClearSelection();
            view.LayoutHost.CurrentTool = new DefaultZoomTool();
            view.LayoutHost.Render();
        }
    }
}
