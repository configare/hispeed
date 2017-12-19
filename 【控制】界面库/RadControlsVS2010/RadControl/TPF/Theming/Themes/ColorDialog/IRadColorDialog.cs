using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls
{
    public interface IRadColorDialog
    {
        /// <summary>
        /// Gets the color selector
        /// </summary>
        UserControl RadColorSelector { get; }

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        Color SelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        HslColor SelectedHslColor { get; set; }

        /// <summary>
        /// Gets or sets the old color
        /// </summary>
        Color OldColor { get; set; }

        /// <summary>
        /// Gets or sets the active mode of the color tabstrip
        /// </summary>
        ColorPickerActiveMode ActiveMode { get; set; }

        /// <summary>
        /// Shows or hides the basic colors tab
        /// </summary>
        bool ShowBasicColors { get; set; }

        /// <summary>
        /// Shows or hides the system colors tab
        /// </summary>
        bool ShowSystemColors { get; set; }

        /// <summary>
        /// Shows or hides the web colors tab
        /// </summary>
        bool ShowWebColors { get; set; }

        /// <summary>
        /// Shows or hides whe professional colors tab
        /// </summary>
        bool ShowProfessionalColors { get; set; }

        /// <summary>
        /// Shows or hides the custom colors tab
        /// </summary>
        bool ShowCustomColors { get; set; }

        /// <summary>
        /// Shows or hides the hex color value
        /// </summary>
        bool ShowHEXColorValue { get; set; }

        /// <summary>
        /// Allows or disallows editing the HEX value
        /// </summary>
        bool AllowEditHEXValue { get; set; }

        /// <summary>
        /// Allows or disallows color picking from the screen
        /// </summary>
        bool AllowColorPickFromScreen { get; set; }

        /// <summary>
        /// Allows or disallows color saving
        /// </summary>
        bool AllowColorSaving { get; set; }

        /// <summary>
        /// Gets the custom colors
        /// </summary> 
        Color[] CustomColors  { get; }

        /// <summary>
        /// Gets or sets the heading of the basic colors tab
        /// </summary>
        string BasicTabHeading { get; set; }

        /// <summary>
        /// Gets or sets the heading of the system colors tab
        /// </summary>
        string SystemTabHeading { get; set; }

        /// <summary>
        /// Gets or sets the heading of the web colors tab
        /// </summary>
        string WebTabHeading { get; set; }

        /// <summary>
        /// Gets or sets the heading of the professional colors tab
        /// </summary>
        string ProfessionalTabHeading { get; set; }

        /// <summary>
        /// Gets or sets the heading of the selected color label
        /// </summary>
        string SelectedColorLabelHeading { get; set; }

        /// <summary>
        /// Gets or sets the heading of the old color label
        /// </summary>
        string OldColorLabelHeading { get; set; }

        /// <summary>
        /// Fires when the selected color has changed
        /// </summary>
        event ColorChangedEventHandler ColorChanged;
    }
}
