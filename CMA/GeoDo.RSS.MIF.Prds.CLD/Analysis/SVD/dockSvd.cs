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
    public partial class dockSvd : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private ucSvd2 _content;

        private dockSvd()
        {
            InitializeComponent();
            this.Text = "SVD分解";
        }

        public dockSvd(ISmartSession smartSession)
            :this()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            _content = new ucSvd2();
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
