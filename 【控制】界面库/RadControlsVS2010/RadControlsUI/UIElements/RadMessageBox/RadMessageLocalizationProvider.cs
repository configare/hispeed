using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using Telerik.WinControls.Localization;

namespace Telerik.WinControls
{
    /// <summary>
    /// Provides Localization service for RadMessageBox
    /// </summary>
    public class RadMessageLocalizationProvider : LocalizationProvider<RadMessageLocalizationProvider>
    {
        /// <summary>
        /// Gets the string corresponding to the given ID.
        /// </summary>
        /// <param name="id">String ID</param>
        /// <returns>The string corresponding to the given ID.</returns>
        public override string GetLocalizedString(string id) //RadGridStringId id
        {
            switch (id)
            {
                case RadMessageStringID.AbortButton: return "Abort";
                case RadMessageStringID.CancelButton: return "Cancel";
                case RadMessageStringID.IgnoreButton: return "Ignore";
                case RadMessageStringID.NoButton: return "No";
                case RadMessageStringID.OKButton: return "OK";
                case RadMessageStringID.RetryButton: return "Retry";
                case RadMessageStringID.YesButton: return "Yes";
            }

            return string.Empty;
        }
    }
}
