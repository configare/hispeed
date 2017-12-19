using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void TabStripPanelSelectedIndexChangingEventHandler(object sender, TabStripPanelSelectedIndexChangingEventArgs args);

    public class TabStripPanelSelectedIndexChangingEventArgs: CancelEventArgs
    {
        int oldIndex;
        int newIndex;
        TabPanel oldTabPanel;
        TabPanel newTabPanel;

        public TabStripPanelSelectedIndexChangingEventArgs(int oldIndex, int newIndex, TabPanel oldTabPanel, TabPanel newTabPanel)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            this.oldTabPanel = oldTabPanel;
            this.newTabPanel = newTabPanel;
        }

        public int OldIndex
        {
            get { return oldIndex; }
        }

        public int NewIndex
        {
            get { return newIndex; }
        }

        public TabPanel OldTabPanel
        {
            get { return this.oldTabPanel; }
        }

        public TabPanel NewTabPanel
        {
            get { return this.newTabPanel; }
        }
    }
}
