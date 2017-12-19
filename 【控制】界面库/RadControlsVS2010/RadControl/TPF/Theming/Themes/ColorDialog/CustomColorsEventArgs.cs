using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes.ColorDialog
{
    public delegate void CustomColorsEventHandler(object sender, CustomColorsEventArgs args);
    /// <summary>
    /// This class is used to hold the event arguments
    /// for the CustomColorsConfigLocationNeeded event of the CustomColors control.
    /// </summary>
    public class CustomColorsEventArgs : EventArgs
    {
        private string configLocation;
        private string configFilename;

        /// <summary>
        /// Creates an instance of the CustomColorsEventArgs class.
        /// </summary>
        /// <param name="location">The location of the config file.</param>
        /// <param name="fileName">The name of the config file.</param>
        public CustomColorsEventArgs(string location, string fileName)
        {
            this.configLocation = location;
            this.configFilename = fileName;
        }

        /// <summary>
        /// Gets or sets the file name of the configuration file.
        /// </summary>
        public string ConfigFilename
        {
            get
            {
                return this.configFilename;
            }
            set
            {
                this.configFilename = value;
            }
        }

        /// <summary>
        /// Gets or sets the path where the configuration file will be stored.
        /// </summary>
        public string ConfigLocation
        {
            get
            {
                return this.configLocation;
            }
            set
            {
                this.configLocation = value;
            }
        }
    }
}
