using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public delegate void DesktopAlertManagerEventHandler(object sender, DesktopAlertManagerEventArgs args);

    /// <summary>
    /// This class encapsulates information relevant to the events of the <see cref="DesktopAlertManager"/>.
    /// </summary>
    public class DesktopAlertManagerEventArgs : EventArgs
    {
        #region Fields

        private RadDesktopAlert associatedAlert;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="DesktopAlertManagerEventArgs"/> class
        /// with a specified <see cref="RadDesktopAlert"/>.
        /// </summary>
        /// <param name="alert"></param>
        public DesktopAlertManagerEventArgs(RadDesktopAlert alert)
        {
            this.associatedAlert = alert;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="RadDesktopAlert"/> class
        /// associated with the event context.
        /// </summary>
        public RadDesktopAlert AssociatedAlert
        {
            get
            {
                return this.associatedAlert;
            }
        }

        #endregion
    }
}
