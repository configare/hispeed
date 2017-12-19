using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Design
{
	/// <summary>
	/// Attribute that can be applied to hide a class when searching for possible new-item-types when a RadControl 
	/// is in design mode
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class RadToolboxItemAttribute: Attribute
	{
		private bool visibleAsNewItem;

		public RadToolboxItemAttribute(bool visibleAsNewItem)
		{
		    this.visibleAsNewItem = visibleAsNewItem;            
		}

		public bool VisibleAsNewItem
		{
			get { return visibleAsNewItem; }
		}
	}
}
