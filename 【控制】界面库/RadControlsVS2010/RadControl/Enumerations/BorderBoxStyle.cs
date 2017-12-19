namespace Telerik.WinControls
{
    /// <summary>Defines the border rendering style.</summary>
	public enum BorderBoxStyle
	{
        /// <summary>
		/// All four borders share same customization, using gradient, regarding parent element's shape.
        /// </summary>
		SingleBorder,
        /// <summary>
        /// Each of the four borders and their "shadow" colors can have disparate customization. Note that shape and gradient would NOT be applied.
        /// </summary>
		FourBorders,
		/// <summary>
		/// Draw inner and outer gradient borders, regarding parent element's shape. Inner and outer borders would share the specified border width.
		/// </summary>
		OuterInnerBorders
	}
}