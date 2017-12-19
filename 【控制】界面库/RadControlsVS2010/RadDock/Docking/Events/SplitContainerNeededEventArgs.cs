using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template used to handle a <see cref="RadDock.DockTabStripNeeded">DockTabStripNeeded</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SplitContainerNeededEventHandler(object sender, SplitContainerNeededEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="RadDock.DockTabStripNeeded">DockTabStripNeeded</see> event.
    /// </summary>
    public class SplitContainerNeededEventArgs : EventArgs
    {
        #region Fields

        private RadSplitContainer container;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="RadSplitContainer">RadSplitContainer</see> instance to be used.
        /// </summary>
        public RadSplitContainer Container
        {
            get
            {
                return this.container;
            }
            set
            {
                this.container = value;
            }
        }

        #endregion
    }
}
