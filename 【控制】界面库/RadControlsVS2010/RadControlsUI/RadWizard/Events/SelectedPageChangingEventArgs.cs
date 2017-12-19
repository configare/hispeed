using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the SelectedPageChanging events of RadWizard.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Instance of SelectedPageChangingEventArgs.</param>
    public delegate void SelectedPageChangingEventHandler(object sender, SelectedPageChangingEventArgs e);

    /// <summary>
    /// Provides data for the SelectedPageChanging event.
    /// </summary>
    public class SelectedPageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets the selected page of the wizard.
        /// </summary>
        public readonly WizardPage SelectedPage;

        /// <summary>
        /// Gets the wizard page to be selected.
        /// </summary>
        public readonly WizardPage NextPage;

        /// <summary>
        /// Initializes a new instance of the SelectedPageChangingEventArgs class.
        /// </summary>
        /// <param name="selectedPage">The selected page of the wizard./param>
        /// <param name="nextPage">The wizard page to be selected.</param>
        public SelectedPageChangingEventArgs(WizardPage selectedPage, WizardPage nextPage)
        {
            this.SelectedPage = selectedPage;
            this.NextPage = nextPage;
        }
    }
}