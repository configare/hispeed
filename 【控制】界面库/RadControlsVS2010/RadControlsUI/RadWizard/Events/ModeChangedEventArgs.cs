using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the ModeChanged events of RadWizard.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of ModeChangedEventArgs.</param>
    public delegate void ModeChangedEventHandler(object sender, ModeChangedEventArgs e);

    /// <summary>
    /// Provides data for the ModeChanged event.
    /// </summary>
    public class ModeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous mode of the wizard.
        /// </summary>
        public readonly WizardMode PreviousMode;

        /// <summary>
        /// Gets the current mode of the wizard.
        /// </summary>
        public readonly WizardMode CurrentMode;

        /// <summary>
        /// Initializes a new instance of the ModeChangedEventArgs class.
        /// </summary>
        /// <param name="previousMode">The previous mode of the wizard.</param>
        /// <param name="currentMode">The current mode of the wizard.</param>
        public ModeChangedEventArgs(WizardMode previousMode, WizardMode currentMode)
        {
            this.PreviousMode = previousMode;
            this.CurrentMode = currentMode;
        }
    }
}