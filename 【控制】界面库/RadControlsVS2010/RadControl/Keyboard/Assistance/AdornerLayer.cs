using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Assistance
{
	class AdornerLayer : Control
	{
		public AdornerLayer() 
		{ 
		}

		public AdornerLayer(Control owner)
		{
			this.owner = owner;
		}

		// Fields
		private Control owner;
	}
}
