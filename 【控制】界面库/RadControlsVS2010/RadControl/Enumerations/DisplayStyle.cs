using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the display style of an item. 
    /// </summary>
	public enum DisplayStyle
	{
		// Summary:     
        ///<summary>
        ///Specifies that neither image nor text is rendered.
        ///</summary>
		None = 0,
		//
		// Summary:
		///     Specifies that only text is rendered.
		Text = 1,
		//
		// Summary:
		/// <summary>
        ///  Specifies that only an image is rendered.
		/// </summary>
    
		Image = 2,
		//
		// Summary:
		/// <summary>
        ///  Specifies that both an image and text are to be rendered.
		/// </summary>
    
		ImageAndText = 3,
	}
}