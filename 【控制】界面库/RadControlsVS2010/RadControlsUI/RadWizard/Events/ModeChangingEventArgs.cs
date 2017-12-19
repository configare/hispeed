using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the ModeChanging events of RadWizard.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of ModeChangingEventArgs.</param>
    public delegate void ModeChangingEventHandler(object sender, ModeChangingEventArgs e);

    /// <summary>
    /// Provides data for the ModeChanging event.
    /// </summary>
    public class ModeChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets the current mode of the wizard.
        /// </summary>
        public readonly WizardMode CurrentMode;

        /// <summary>
        /// Gets the next mode of the wizard.
        /// </summary>
        public readonly WizardMode NextMode;

        /// <summary>
        /// Initializes a new instance of the ModeChangingEventArgs class.
        /// </summary>
        /// <param name="currentMode">The current mode of the wizard.</param>
        /// <param name="nextMode">The next mode of the wizard.</param>
        public ModeChangingEventArgs(WizardMode currentMode, WizardMode nextMode)
        {
            this.CurrentMode = currentMode;
            this.NextMode = nextMode;
        }
    }
}