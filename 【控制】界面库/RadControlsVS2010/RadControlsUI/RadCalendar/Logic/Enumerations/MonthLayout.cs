using System;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Summary description for MonthLayout.
    /// Layout_7columns_x_6rows  - horizontal layout
	/// Layout_14columns_x_3rows - horizontal layout     
	/// Layout_21columns_x_2rows - horizontal layout
    /// Layout_7rows_x_6columns  - vertical layout, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
    /// Layout_14rows_x_3columns - vertical layout, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
	/// Layout_21rows_x_2columns - vertical layout, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
	/// </summary>
    public enum MonthLayout 
    {
		/// <summary>
		/// Allows the calendar to display the days in a 7  by 6 matrix.
		/// </summary>
		/// <value>1</value>
        Layout_7columns_x_6rows = 1,
		/// <summary>
		/// Alows the calendar to display the days in a 14 by 3 matrix.
		/// </summary>
		/// <value>2</value>
        Layout_14columns_x_3rows = 2,
		/// <summary>
		/// Allows the calendar to display the days in a 21 by 2 matrix.
		/// </summary>
		/// <value>4</value>
        Layout_21columns_x_2rows = 4,
		/// <summary>
		/// Allows the calendar to display the days in a 7  by 6 matrix, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
		/// </summary>
		/// <value>8</value>
		Layout_7rows_x_6columns = 8,
		/// <summary>
        /// Allows the calendar to display the days in a 14 by 3 matrix, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
		/// </summary>
		/// <value>16</value>
		Layout_14rows_x_3columns = 16,
		/// <summary>
        /// Allows the calendar to display the days in a 21 by 2 matrix, required when UseColumnHeadersAsSelectors is true and Orientation is set to RenderInColumns.
		/// </summary>
		/// <value>32</value>
		Layout_21rows_x_2columns = 32
    }
}