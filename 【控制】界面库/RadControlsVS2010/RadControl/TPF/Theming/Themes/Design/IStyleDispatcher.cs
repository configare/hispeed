using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes.Design
{
	public interface IStyleDispatcher
	{
		void SetStyleByTargetElement(StyleSheet style, RadElement targetElement);
		StyleSheet GetStyleByTargetElement(RadElement targetElement);
	}
}
