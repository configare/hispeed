using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the element's property options.
    /// </summary>
	[Flags]
	public enum ElementPropertyOptions
	{   
        /// <summary>
        /// Indicates that there are no property options.
        /// </summary>
		None = 0,
        /// <summary>
        /// Indicates that the property can inherit a value.
        /// </summary>
		CanInheritValue = 2,
        /// <summary>
        /// Indicates that the property invalidates the layout.
        /// </summary>
		InvalidatesLayout = 4,
        /// <summary>
        /// Indicates that the property affects the layout.
        /// </summary>
		AffectsLayout = 8,
		/// <summary>
		/// Invalidates measure
		/// </summary>
		AffectsMeasure = 16,
		/// <summary>
		/// Invalidates arrange
		/// </summary>
		AffectsArrange = 32,
        /// <summary>
        /// Invalidates parent's measure
        /// </summary>
        AffectsParentMeasure = 64,
        /// <summary>
        /// Invalidates parent's arrange
        /// </summary>
        AffectsParentArrange = 128,
        /// <summary>
        /// Indicates that the property affects the display.
        /// </summary>
		AffectsDisplay = 256,
        /// <summary>
        /// Indicates that the property affects the theme.
        /// </summary>
	    AffectsTheme = 512,
        /// <summary>
        /// The property supports cancelation.
        /// </summary>
        Cancelable = AffectsTheme << 1,
	}
}