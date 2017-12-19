using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the possible reasons for displaying an auto-hidden window.
    /// </summary>
    public enum AutoHideDisplayReason
    {
        /// <summary>
        /// An auto-hidden window has become the currently active one.
        /// </summary>
        Activate,
        /// <summary>
        /// Associated tab item is hovered.
        /// </summary>
        TabItemHovered,
        /// <summary>
        /// Associated tab item is clicked.
        /// </summary>
        TabItemClicked,
    }

    /// <summary>
    /// Defines a method signature to handle <see cref="RadDock.AutoHideWindowDisplaying">AutoHideWindowDisplaying</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AutoHideWindowDisplayingEventHandler(object sender, AutoHideWindowDisplayingEventArgs e);

    /// <summary>
    /// Encapsulates the data associated with the <see cref="RadDock.AutoHideWindowDisplaying">AutoHideWindowDisplaying</see> event.
    /// </summary>
    public class AutoHideWindowDisplayingEventArgs : DockWindowCancelEventArgs
    {
        #region Fields

        private AutoHideDisplayReason displayReason;

        #endregion

        #region Constructor

        public AutoHideWindowDisplayingEventArgs(DockWindow window, AutoHideDisplayReason reason)
            : base(window)
        {
            this.displayReason = reason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reason for the display request.
        /// </summary>
        public AutoHideDisplayReason DisplayReason
        {
            get
            {
                return this.displayReason;
            }
        }

        #endregion
    }
}
