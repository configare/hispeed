using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class mosaicProjectionPage : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private frmMosaicProjection _firstPageContent;

        private mosaicProjectionPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
        }

        public mosaicProjectionPage(ISmartSession smartSession)
            : this()
        {
            Load_firstPageContent(smartSession);
        }

        private void Load_firstPageContent(ISmartSession smartSession)
        {
            _firstPageContent = new frmMosaicProjection(smartSession);
            _firstPageContent.ProjectionFinished += new EventHandler(_firstPageContent_ProjectionFinished);
            _firstPageContent.Dock = DockStyle.Fill;
            this.Text = _firstPageContent.Text;
            this.Controls.Add(_firstPageContent);
        }

        void _firstPageContent_ProjectionFinished(object sender, EventArgs e)
        {
            this.Close();
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

        public new void Free()
        {
            if (_firstPageContent != null)
            {
                _firstPageContent.ProjectionFinished -= new EventHandler(_firstPageContent_ProjectionFinished);
                _firstPageContent.Free();
                _firstPageContent = null;
            }
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}
