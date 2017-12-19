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
    public partial class dockCorrelate : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private ucCorrelate _content;

        private dockCorrelate()
        {
            InitializeComponent();
            this.Text = "相关系数计算";
        }

        public dockCorrelate(ISmartSession smartSession)
            :this()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            _content = new ucCorrelate();
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
