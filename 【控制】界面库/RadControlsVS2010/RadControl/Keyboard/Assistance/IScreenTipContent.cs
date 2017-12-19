using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Elements;
using System.Drawing;

namespace Telerik.WinControls
{
	public interface IScreenTipContent
	{
		RadItemReadOnlyCollection TipItems {get;}
		string Description { get; set; }
		Type TemplateType { get; set; }
        Size TipSize { get; }
	}
}
