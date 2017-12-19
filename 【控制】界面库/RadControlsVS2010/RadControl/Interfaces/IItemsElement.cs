using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
	public interface IItemsElement
	{
		RadItemOwnerCollection Items { get; }

        void ItemClicked(RadItem item);
	}
}
