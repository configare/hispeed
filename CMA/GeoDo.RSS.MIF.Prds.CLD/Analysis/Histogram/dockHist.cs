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

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class dockHist : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private ucHist _content;
        private ISmartSession _smartSession;

        private dockHist()
        {
            InitializeComponent();
            this.Text = "直方图统计";
        }

        public dockHist(ISmartSession smartSession)
            : this()
        {
            _smartSession = smartSession;
            LoadContent();
        }

        private void LoadContent()
        {
            _content = new ucHist(_smartSession);
            _content.Dock = DockStyle.Fill;
            this.Controls.Add(_content);
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
