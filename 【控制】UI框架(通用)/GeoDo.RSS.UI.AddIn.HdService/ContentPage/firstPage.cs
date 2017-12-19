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

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class firstPage : ToolWindow, ISmartWindow //, UserControl  ToolWindow
    {
        private EventHandler _onWindowClosed;
        private firstPageContent _firstPageContent;
        private monitoringPage _monitoringPage;

        private firstPage()
        {
            InitializeComponent();
            this.Text = "首页";
        }

        public firstPage(ISmartSession smartSession)
            :this()
        {
            Load_monitoringPage();
        }

        private void Load_firstPageContent(ISmartSession smartSession)
        {
            _firstPageContent = new firstPageContent(smartSession);
            _firstPageContent.Dock = DockStyle.Fill;
            this.Text = _firstPageContent.Text;
            this.Controls.Add(_firstPageContent);
        }

        private void Load_monitoringPage()
        {
            _monitoringPage = new monitoringPage();
            _monitoringPage.Dock = DockStyle.Fill;
            this.Controls.Add(_monitoringPage);
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
