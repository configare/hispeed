using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    public partial class ReportPage : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private ReportPageControl _reportPageControl;

        public ReportPage()
        {
            InitializeComponent();
            InitControl();
        }

        private void InitControl()
        {
            this.Text = "报告素材生产";
            _reportPageControl = new ReportPageControl();
            _reportPageControl.Dock = DockStyle.Fill;
            this.Controls.Add(_reportPageControl);
        }

        public void UpdateSession(ISmartSession smartSession)
        {
            _reportPageControl.UpdateData(smartSession);
        }

        public EventHandler OnWindowClosed
        {
            get
            {
                return _onWindowClosed;

            }
            set
            {
                _onWindowClosed = value;
            }
        }

        public void Free()
        {
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}
