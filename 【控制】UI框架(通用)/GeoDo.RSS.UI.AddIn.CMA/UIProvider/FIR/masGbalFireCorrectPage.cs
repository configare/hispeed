using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.CMA.UIProvider.FIR
{
    public class masGbalFireCorrectPage : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private UCGbalFireCorrect _firstPageContent;

        private masGbalFireCorrectPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
        }

        public masGbalFireCorrectPage(ISmartSession smartSession)
            :this()
        {
            Load_firstPageContent(smartSession);
        }

        private void Load_firstPageContent(ISmartSession smartSession)
        {
            _firstPageContent = new UCGbalFireCorrect(smartSession);
            _firstPageContent.Dock = DockStyle.Fill;
            this.Text = "全球火点修正";
            this.Controls.Add(_firstPageContent);
        }

        public void SetArgsPanelWidth(int width)
        {
            _firstPageContent.argControlWidth = width;
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
            if(_firstPageContent!=null)
                _firstPageContent.Free();
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}
