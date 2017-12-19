using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void TabStripPanelCancelEventHandler(object sender, TabStripPanelCancelEventArgs e);

    public class TabStripPanelCancelEventArgs : CancelEventArgs
    {
        private TabControlAction action;
        private TabPanel tabPanel;
        private int tabPanelIndex;

        public TabStripPanelCancelEventArgs(TabPanel tabPanel, int tabPanelIndex, bool cancel, TabControlAction action)
        {
            this.tabPanel = tabPanel;
            this.tabPanelIndex = tabPanelIndex;
            this.action = action;
        }

        public TabControlAction Action 
        {
            get { return this.action; } 
        }

        public TabPanel TabPanel 
        {
            get { return this.tabPanel; }
        }

        public int TabPanelIndex 
        {
            get { return this.tabPanelIndex; }
        }
    }
}