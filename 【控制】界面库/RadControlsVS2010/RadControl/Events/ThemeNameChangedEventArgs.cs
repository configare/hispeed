using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle the ThemeNameChanged event.
    /// </summary>
    /// <param name="source">
    /// Initializes the event sender.
    /// </param>
    /// <param name="args">
    /// Initializes the %event arguments:ThemeNameChangedEventArgs%.
    /// </param>
    public delegate void ThemeNameChangedEventHandler(object source, ThemeNameChangedEventArgs args);

    /// <summary>
    /// Represents the event data for the %ThemeNameChanged:ThemeNameChanged% event.
    /// </summary>
    public class ThemeNameChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents the old theme name.
        /// </summary>
        public readonly string oldThemeName;

        /// <summary>
        /// Represents the new theme name.
        /// </summary>
        public readonly string newThemeName;

        /// <summary>
        /// Initializes a new instance of the ThemeNameChangedEventArgs class. 
        /// </summary>
        /// <param name="oldThemeName">
        /// Initializes the old theme name.
        /// </param>
        /// <param name="newThemeName">
        /// Initializes the new theme name.
        /// </param>
        public ThemeNameChangedEventArgs(string oldThemeName, string newThemeName)
        {
            this.oldThemeName = oldThemeName;
            this.newThemeName = newThemeName;
        }
    }
}
