using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    public class GeoCorrectionPage : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private frmGeoCorrection _firstPageContent;

        private GeoCorrectionPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
        }

        public GeoCorrectionPage(ISmartSession smartSession)
            : this()
        {
            Load_firstPageContent(smartSession);
        }

        private void Load_firstPageContent(ISmartSession smartSession)
        {
            _firstPageContent = new frmGeoCorrection(smartSession);
           // _firstPageContent.ProjectionFinished += new EventHandler(_firstPageContent_CorrectionFinished);
            _firstPageContent.Dock = DockStyle.Fill;
            this.Text = _firstPageContent.Text;
            this.Controls.Add(_firstPageContent);
        }

        void _firstPageContent_CorrectionFinished(object sender, EventArgs e)
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
                _firstPageContent.Free();
                _firstPageContent = null;
            }

            if (_onWindowClosed != null)
               _onWindowClosed(this, null);
        }
    }
}
