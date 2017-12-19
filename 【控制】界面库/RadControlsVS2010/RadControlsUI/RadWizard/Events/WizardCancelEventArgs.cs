using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle cancelable events of RadWizard.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of WizardCancelEventArgs.</param>
    public delegate void WizardCancelEventHandler(object sender, WizardCancelEventArgs e);

    /// <summary>
    /// Provides data for cancelable events of RadWizard.
    /// </summary>
    public class WizardCancelEventArgs : EventArgs
    {
        #region Fields

        private bool cancel;

        #endregion

        /// <summary>
        /// Initializes a new instance of the WizardCancelEventArgs class.
        /// </summary>
        public WizardCancelEventArgs()
        {
        }

        #region Properties

        /// <summary>
        /// Determines whether the event is canceled or may continue.
        /// </summary>
        public bool Cancel
        {
            get { return this.cancel; }
            set { this.cancel = value; }
        }

        #endregion
    }
}