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

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class FileAttributeWindow : ToolWindowBase, ISmartToolWindow
    {
        private int _id = 9001;
        private ISmartSession _session = null;

        public FileAttributeWindow()
        {
            InitializeComponent();
            Text = "文件属性";
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}
