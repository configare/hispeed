using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.Office
{
    //[Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class frmExcelStatResultWnd : ToolWindow, IStatResultDisplayWindow, Telerik.WinControls.IGeoDoFree, ISmartToolWindow
    {
        private WinExcelControl winExcelControl1;
        private int _id = 9004;

        public frmExcelStatResultWnd()
        {
            InitializeComponent();
        }

        public void Open(string fname)
        {
            CreateExcelWnd(fname);
        }

        void Telerik.WinControls.IGeoDoFree.Free()
        {
            winExcelControl1.CloseControl();
        }

        void ISmartWindow.Free()
        {
            winExcelControl1.CloseControl();
        }

        public void CreateExcelWnd(string filename)
        {
            winExcelControl1 = new WinExcelControl();
            winExcelControl1.Dock = DockStyle.Fill;
            this.Controls.Add(winExcelControl1);
            if (string.IsNullOrEmpty(filename))
                winExcelControl1.OnNewCreateXls();
            else
                winExcelControl1.Open(filename);
        }

        public EventHandler OnWindowClosed
        {
            get { return null; }
            set { ;}
        }

        public int Id
        {
            get { return _id; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
        }
    }
}
