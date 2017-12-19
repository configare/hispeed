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
    public partial class FeatureListWindow : ToolWindowBase, ISmartToolWindow
    {
        private IToolWindowContent _toolWindowContent = null;

        public FeatureListWindow()
            : base()
        {
            _id = 9008;
            Text = "交互统计窗口";
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            _toolWindowContent = new FeatureListContent();
            return _toolWindowContent;
        }

        public IToolWindowContent ToolWindowContent
        {
            get { return _toolWindowContent; }
        }
    }
}
