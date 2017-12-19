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
    public partial class dockSeriesMean : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private ucSeriesMean _content;
        private ISmartSession _smartSession;

        private dockSeriesMean()
        {
            InitializeComponent();
            this.Text = "长时间序列均值统计";
        }

        public dockSeriesMean(ISmartSession smartSession)
            : this()
        {
            _smartSession = smartSession;
            LoadContent();
        }

        private void LoadContent()
        {
            _content = new ucSeriesMean(_smartSession);
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
