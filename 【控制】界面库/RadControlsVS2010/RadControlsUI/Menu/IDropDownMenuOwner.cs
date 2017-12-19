using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	public interface IDropDownMenuOwner
	{
		bool DropDownInheritsThemeClassName
		{
			get;
		}

	    bool ControlDefinesThemeForElement(RadElement element);
	}
}
