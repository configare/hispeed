using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the SelectedPageChanged events of RadWizard.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of SelectedPageChangedEventArgs.</param>
    public delegate void SelectedPageChangedEventHandler(object sender, SelectedPageChangedEventArgs e);

    /// <summary>
    /// Provides data for the SelectedPageChanged event.
    /// </summary>
    public class SelectedPageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous selected page of the wizard.
        /// </summary>
        public readonly WizardPage PreviousPage;

        /// <summary>
        /// Gets the selected page of the wizard.
        /// </summary>
        public readonly WizardPage SelectedPage;

        /// <summary>
        /// Initializes a new instance of the SelectedPageChangedEventArgs class.
        /// </summary>
        /// <param name="previousPage">The previous selected page of the wizard.</param>
        /// <param name="selectedPage">The selected page of the wizard.</param>
        public SelectedPageChangedEventArgs(WizardPage previousPage, WizardPage selectedPage)
        {
            this.PreviousPage = previousPage;
            this.SelectedPage = selectedPage;
        }
    }
}