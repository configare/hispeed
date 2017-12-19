using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Licensing
{
    internal sealed class LicenseManager
    {
        #region Constructors
        static LicenseManager()
        {
            LicenseManager.licenseKey = string.Empty;
        }

        private LicenseManager()
        {
        } 
        #endregion

        #region Methods
        internal static void CheckLicense(object licensee)
        {
        }
        internal static bool IsLicenseValid()
        {
            return false;
        } 
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the license key used to license this product. 
        /// </summary>
        public static string LicenseKey
        {
            get
            {
                return LicenseManager.licenseKey;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("LicenseKey");
                }
                LicenseManager.licenseKey = value;
            }
        } 
        #endregion

        #region Fields

        private static string licenseKey; 

        #endregion
    }
}
