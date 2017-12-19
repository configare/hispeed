using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
       [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdPanData : Command
    {
           public CmdPanData()
               : base()
           {
               _id = 6031;
               _name = "PanData";
               _text = _toolTip = "漫游数据";
           }

           public override void Execute()
           {
               ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
               if (view == null)
                   return;
               if (view.LayoutHost == null)
                   return;
               (view.LayoutHost).SetActiveDataFrame2CurrentTool();
               (view.LayoutHost).Render();
           }
    }
}
