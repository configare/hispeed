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
    public class CmdAddCurrentDate : CmdAddElementBase
    {
        public CmdAddCurrentDate()
            : base()
        {
            _id = 6023;
            _name = "AddCurrentDate";
            _text = _toolTip = "添加当前日期";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            TextElementCurrentDateTime5 date = new TextElementCurrentDateTime5();
            date.Location = new System.Drawing.PointF(100, 50);
            viewer.LayoutHost.LayoutRuntime.Layout.Elements.Add(date);
            viewer.LayoutHost.Render();
            TryRefreshLayerManager();
        }
    }
}
