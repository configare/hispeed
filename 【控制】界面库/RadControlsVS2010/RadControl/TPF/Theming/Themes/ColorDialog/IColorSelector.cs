using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.UI;

namespace Telerik.WinControls
{
	/// <summary>
	/// Wraps the functionality provided by the color picker
	/// </summary>
	public interface IColorSelector
	{
		/// <summary>
		/// Gets or sets the selected color
		/// </summary>
		Color SelectedColor
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        HslColor SelectedHslColor
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets the old color
		/// </summary>
		Color OldColor
		{
			get;
			set;
		}
		/// <summary>
		/// Shows or hides the basic colors tab
		/// </summary>
		bool ShowBasicColors
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the active mode of the color tabs
		/// </summary>
		ColorPickerActiveMode ActiveMode
		{
			get;
			set;
		}
		/// <summary>
		/// Shows or hides the system colors tab
		/// </summary>
		bool ShowSystemColors { get;set;}
		/// <summary>
		/// Shows or hides the web colors tab
		/// </summary>
		bool ShowWebColors { get;set;}
		/// <summary>
		/// Shows or hides the professional colors tab
		/// </summary>
		bool ShowProfessionalColors { get;set;}
		/// <summary>
		/// Shows or hides the custom colors panel
		/// </summary>
		bool ShowCustomColors { get;set;}
		/// <summary>
		/// Shows or hides the hex color textbox
		/// </summary>
		bool ShowHEXColorValue { get;set;}
		/// <summary>
		/// Allows or disallows editing the hex value
		/// </summary>
		bool AllowEditHEXValue { get;set;}
		/// <summary>
		/// Allows or disallows picking colors from the screen
		/// </summary>
		bool AllowColorPickFromScreen { get;set;}
		/// <summary>
		/// Allows or disallows color saving
		/// </summary>
		bool AllowColorSaving { get;set;}
		/// <summary>
		/// Gets the custom colors
		/// </summary>
		Color[] CustomColors { get;}
		/// <summary>
		/// Gets or sets the heading of the basic colors tab
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the heading of the basic tab label.")]
		string BasicTabHeading { get;set;}
		/// <summary>
		/// Gets or sets the heading of the system colors tab
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the heading of the system tab label.")]
		string SystemTabHeading { get;set;}
		/// <summary>
		/// Gets or sets the heading of the web colors tab
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the heading of the web tab label.")]
		string WebTabHeading { get;set;}
		/// <summary>
		/// Gets or sets the heading of the professional colors tab
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the heading of the professional tab label.")]
		string ProfessionalTabHeading { get;set;}
		/// <summary>
		/// Gets or sets the heading of the selected color label
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the heading of the selected color label.")]
		string SelectedColorLabelHeading { get;set;}
		/// <summary>
		/// Gets or sets the heading of the old color label
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
	   Description("Gets or sets the heading of the old color label.")]
		string OldColorLabelHeading { get;set;}

		/// <summary>
		/// Fires when the OK Button is clicked
		/// </summary>
		event ColorChangedEventHandler OkButtonClicked;
		/// <summary>
		/// Fires when the Cancel Button is clicked
		/// </summary>
		event ColorChangedEventHandler CancelButtonClicked;
	}
}
