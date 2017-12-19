using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public delegate void TabStripPanelEventHandler(object sender, TabStripPanelEventArgs e);

    public class TabStripPanelEventArgs : EventArgs
    {
        private TabControlAction action;
        private TabPanel tabPanel;
        private int tabPanelIndex;

        public TabStripPanelEventArgs(TabPanel tabPanel, int tabPanelIndex, TabControlAction action)
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