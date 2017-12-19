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
    public partial class queryPage : ToolWindow, ISmartWindow //, UserControl  ToolWindow
    {
        private EventHandler _onWindowClosed;
        private queryPageControl _queryPage;

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

        private queryPage()
        {
            InitializeComponent();
            this.Text = "查询结果";
        }

        public queryPage(ISmartSession smartSession)
            :this()
        {
            Load_monitoringPage();
        }

        private void Load_monitoringPage()
        {
            _queryPage = new queryPageControl();
            _queryPage.Dock = DockStyle.Fill;
            this.Controls.Add(_queryPage);
        }

        public void SetQuery(string argument)
        {
            _queryPage.SetQueryArgument(argument);
        }

        public void Free()
        {
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}
