using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    public delegate void SelectedTabChangedEventHandler(object sender, SelectedTabChangedEventArgs e);

    public class SelectedTabChangedEventArgs : DockWindowEventArgs
    {
        private DockWindow newWindow;


        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedTabChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldWindow">The old window.</param>
        /// <param name="dockWindow">The dock window.</param>
        public SelectedTabChangedEventArgs(DockWindow oldWindow, DockWindow dockWindow)
            :base(oldWindow)
        {
            this.newWindow = dockWindow;
        }

        /// <summary>
        /// Gets the old window.
        /// </summary>
        /// <value>The old window.</value>
        public DockWindow OldWindow
        {
            get { return this.DockWindow; }
        }

        /// <summary>
        /// Gets the new window.
        /// </summary>
        /// <value>The new window.</value>
        public DockWindow NewWindow
        {
            get { return newWindow; }
        }

        /// <summary>
        /// Gets the tab strip.
        /// </summary>
        /// <value>The tab strip.</value>
        public DockTabStrip TabStrip
        {
            get
            {
                return this.NewWindow.TabStrip as DockTabStrip;
            }
        }

        /// <summary>
        /// Gets the old tab strip item.
        /// </summary>
        /// <value>The old tab strip item.</value>
        public TabStripItem OldTabStripItem
        {
            get
            {
                return this.OldWindow.TabStripItem;
            }
        }

        /// <summary>
        /// Gets the new tab strip item.
        /// </summary>
        /// <value>The new tab strip item.</value>
        public TabStripItem NewTabStripItem
        {
            get
            {
                return this.NewWindow.TabStripItem;
            }
        }


        /// <summary>
        /// Gets the old index of the selected TabStripItem.
        /// </summary>
        /// <value>The old index of the selected TabStripItem.</value>
        public int OldTabStripItemIndex
        {
            get
            {
                return this.NewWindow.TabStrip.TabPanels.IndexOf(this.OldWindow);
            }
        }


        /// <summary>
        /// Gets the new index of the selected TabStripItem.
        /// </summary>
        /// <value>The new index of the selected TabStripItem.</value>
        public int NewTabStripItemIndex
        {
            get
            {
                return this.NewWindow.TabStrip.TabPanels.IndexOf(this.NewWindow);
            }
        }

    }
}
