using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.Office
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class frmTextStatResultWnd : ToolWindow, IStatResultDisplayWindow, Telerik.WinControls.IGeoDoFree, ISmartToolWindow
    {
        private int _id = 9005;

        public frmTextStatResultWnd()
        {
            InitializeComponent();
        }

        public void Open(string filename)
        {
            if (!File.Exists(filename))
                return;
            richTextBox1.Text = File.ReadAllText(filename);
        }

        void Telerik.WinControls.IGeoDoFree.Free()
        {
        }

        public int Id
        {
            get { return _id; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
        }


        public EventHandler OnWindowClosed
        {
            get { return null; }
            set { ;}
        }
    }
}
