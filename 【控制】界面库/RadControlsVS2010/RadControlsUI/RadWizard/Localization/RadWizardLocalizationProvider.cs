using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides localization services for RadWizard.
    /// </summary>
    public class RadWizardLocalizationProvider : Localization.LocalizationProvider<RadWizardLocalizationProvider>
    {
        /// <summary>
        /// Gets the string corresponding to the given ID.
        /// </summary>
        /// <param name="id">String ID.</param>
        /// <returns>The string corresponding to the given ID.</returns>
        public override string GetLocalizedString(string id)
        {
            switch (id)
            {
                case RadWizardStringId.BackButtonText: return "<   Back";
                case RadWizardStringId.NextButtonText: return "Next   >";
                case RadWizardStringId.CancelButtonText: return "Cancel";
                case RadWizardStringId.FinishButtonText: return "Finish";
                case RadWizardStringId.HelpButtonText: return "<html><u>Help</u></html>";
                default: return string.Empty;
            }
        }
    }
}