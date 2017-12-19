using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdRefreshData : Command
    {
        public CmdRefreshData()
            : base()
        {
            _id = 6032;
            _name = "RefreshData";
            _text = _toolTip = "刷新数据";
        }

        public override void Execute()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            if (view.LayoutHost == null)
                return;
            ILayoutHost host = view.LayoutHost;
            IDataFrame frm = host.ActiveDataFrame;
            if (frm != null)
            {
                //(frm.Provider as IDataFrameDataProvider).Canvas.Refresh(enumRefreshType.All);
                host.Render(true);
            }
        }
    }
}
