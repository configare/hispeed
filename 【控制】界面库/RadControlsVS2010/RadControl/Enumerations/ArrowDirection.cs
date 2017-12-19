using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{	
	/// <summary>
	/// Specifies arrow directions for the arrow primitive: Up, Right, Down, and
	/// Left.
	/// </summary>
	[Flags]
	public enum ArrowDirection
	{
		/// <summary>
		/// Indicates left pointed arrow.
		/// </summary>
		Left = 1,

		/// <summary>
		/// Indicates up pointed arrow.
		/// </summary>
		Up = 2,

		/// <summary>
		/// Indicates right pointed arrow.
		/// </summary>
		Right = 4,

		/// <summary>
		/// Indicates down pointed arrow.
		/// </summary>
		Down = 8
	}
}
