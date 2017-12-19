using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Telerik.WinControls.Serialization
{
    /// <summary>
    /// Represents an archive package where each stream is a compressed XmlTheme.
    /// </summary>
    [Serializable]
    public class RadThemePackage : RadArchivePackage
    {
        #region Constants

        public const string FileExtension = "tssp";//Telerik StyleSheet Package

        #endregion

        #region Fields

        private string themeName;

        #endregion

        #region Properties

        protected override PackageFormat DefaultFormat
        {
            get
            {
                return PackageFormat.XML;
            }
        }

        public string ThemeName
        {
            get
            {
                return this.themeName;
            }
            set
            {
                this.themeName = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all the themes that reside within this package.
        /// </summary>
        /// <returns></returns>
        public XmlTheme[] DecompressThemes()
        {
            List<XmlTheme> themes = new List<XmlTheme>();
            foreach (RadArchiveStream stream in this.Streams)
            {
                XmlTheme theme = stream.Unzip() as XmlTheme;
                Debug.Assert(theme != null, "Invalid stream in RadThemePackage.");
                if (theme != null)
                {
                    themes.Add(theme);
                }
            }

            return themes.ToArray();
        }

        #endregion
    }
}
