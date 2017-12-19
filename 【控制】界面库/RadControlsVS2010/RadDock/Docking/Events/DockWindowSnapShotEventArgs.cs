using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle the <see cref="RadDock.QuickNavigatorSnapshotNeeded">QuickNavigatorSnapshotNeeded</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockWindowSnapshotEventHandler(object sender, DockWindowSnapshotEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="RadDock.QuickNavigatorSnapshotNeeded">QuickNavigatorSnapshotNeeded</see> event.
    /// </summary>
    public class DockWindowSnapshotEventArgs : DockWindowEventArgs
    {
        #region Fields

        private Image snapShot;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance of the <see cref="DockWindowSnapshotEventArgs">DockWindowSnapshotEventArgs</see> class.
        /// </summary>
        /// <param name="window"></param>
        public DockWindowSnapshotEventArgs(DockWindow window)
            : base(window)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the custom image to be used as a Preview snapshot for the associated window in the <see cref="QuickNavigator">QuickNavigator</see>.
        /// </summary>
        public Image SnapShot
        {
            get
            {
                return this.snapShot;
            }
            set
            {
                this.snapShot = value;
            }
        }

        #endregion
    }
}
