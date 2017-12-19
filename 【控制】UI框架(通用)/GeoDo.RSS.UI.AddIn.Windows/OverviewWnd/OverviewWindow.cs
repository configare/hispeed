using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class OverviewWindow : ToolWindowBase, ISmartToolWindow
    {
        public OverviewWindow()
            :base()
        {
            _id = 9007;
            Text = "鹰眼视图";
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            return new OverviewContent();
        }
    }
}
