using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void SelectedTabChangingEventHandler(object sender, SelectedTabChangingEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class SelectedTabChangingEventArgs : DockWindowCancelEventArgs
    {
        
            /// <summary>
        /// Initializes a new instance of the <see cref="DockWindowCancelEventArgs"/> class.
        /// </summary>
        /// <param name="oldWindow">The old dock window.</param>
        /// <param name="newWindow">The new dock window.</param>
        public SelectedTabChangingEventArgs(DockWindow oldWindow, DockWindow newWindow)
            :base(oldWindow, newWindow)
        {
        
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
