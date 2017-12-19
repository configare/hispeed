using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Telerik.WinControls.UI
{

    /// <summary>
    /// Provides localization.
    /// </summary>
    public class RadToolStripLocalizationProvider
    {
        public static event EventHandler CurrentProviderChanged;

        private static void OnCurrentProviderChanged()
        {
            if (RadToolStripLocalizationProvider.CurrentProviderChanged != null)
            {
                RadToolStripLocalizationProvider.CurrentProviderChanged(
                    RadToolStripLocalizationProvider.CurrentProviderChanged, EventArgs.Empty);
            }
        }

        private static RadToolStripLocalizationProvider currentLocalizationProvider =
                RadToolStripLocalizationProvider.CreateDefaultLocalizationProvider();

        /// <summary>
        /// Gets or sets the current localization provider.
        /// </summary>
        [Browsable(false)]
        public static RadToolStripLocalizationProvider CurrentProvider
        {
            get
            {
                return RadToolStripLocalizationProvider.currentLocalizationProvider;
            }
            set
            {
                if (value == null)
                {
                    RadToolStripLocalizationProvider.currentLocalizationProvider =
                        RadToolStripLocalizationProvider.CreateDefaultLocalizationProvider();
                }
                else
                {
                    RadToolStripLocalizationProvider.currentLocalizationProvider = value;
                }

                RadToolStripLocalizationProvider.OnCurrentProviderChanged();
            }
        }

        public static RadToolStripLocalizationProvider CreateDefaultLocalizationProvider()
        {
            return new RadToolStripLocalizationProvider();
        }

        /// <summary>
        /// Gets a CultureInfo object corresponding to the current localization provider.
        /// </summary>
        public virtual CultureInfo Culture
        {
            get
            {
                return CultureInfo.CreateSpecificCulture("en-US");
            }
        }

        public virtual string GetLocalizationString(RadToolStripLocalizationStringId localizationString)
        {

            switch (localizationString)
            {
                case RadToolStripLocalizationStringId.CloseButton: 
                    return "Close";
                case RadToolStripLocalizationStringId.ResetButton: 
                    return "Reset";
                case RadToolStripLocalizationStringId.ToolBarsTabItem: 
                    return "Toolbars";
                case RadToolStripLocalizationStringId.CustomizeDialogCaption: 
                    return "Customize";
                case RadToolStripLocalizationStringId.AddorRemoveButtons: 
                    return "Add or Remove Buttons";
                case RadToolStripLocalizationStringId.NoTextAssociated: 
                    return "No Text Associated";
                case RadToolStripLocalizationStringId.ResetToolBar: 
                    return "Reset Toolbar";
                case RadToolStripLocalizationStringId.Customize: 
                    return "Customize";
                case RadToolStripLocalizationStringId.ResetToolBarsAlertMessage: 
                    return "Are you really sure that you want to reset the toolbars?";
            }
            return string.Empty;
        }
    }
}
