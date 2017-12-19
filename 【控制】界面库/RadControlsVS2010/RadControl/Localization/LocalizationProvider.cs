using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace Telerik.WinControls.Localization
{
    /// <summary>
    /// This class is used as a base class for all Localization Provider classes 
    /// used in RadControls.
    /// </summary>
    public abstract class LocalizationProvider<T> where T: class, new()
    {
        /// <summary>
        /// Fires when the current localization provider has changed.
        /// </summary>
        public static event EventHandler CurrentProviderChanged;

        private static void OnCurrentProviderChanged()
        {
            EventHandler eh = CurrentProviderChanged;
            if (eh != null)
            {
                eh(currentProvider, EventArgs.Empty);
            }
        }

        private static T currentProvider =
            CreateDefaultLocalizationProvider();

        /// <summary>
        /// Gets or sets the current localization provider.
        /// </summary>
        [Browsable(false)]
        public static T CurrentProvider
        {
            get
            {
                return LocalizationProvider<T>.currentProvider;
            }
            set
            {
                if (value == null)
                {
                    LocalizationProvider<T>.currentProvider =
                        LocalizationProvider<T>.CreateDefaultLocalizationProvider();
                }
                else
                {
                    LocalizationProvider<T>.currentProvider = value;
                }
                LocalizationProvider<T>.OnCurrentProviderChanged();
            }
        }

        /// <summary>
        /// Creates a default localization provider.
        /// </summary>
        /// <returns>A new instance of the default localization provider.</returns>
        public static T CreateDefaultLocalizationProvider()
        {
            return new T();
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

        /// <summary>
        /// Gets the string corresponding to the given ID.
        /// </summary>
        /// <param name="id">String ID</param>
        /// <returns>The string corresponding to the given ID.</returns>
        public abstract string GetLocalizedString(string id);
       
    }
}
