using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
	class TemplateEventArgs : EventArgs
	{
		public TemplateEventArgs(RadItem item)
		{
			this.item = item;
		}

		public TemplateEventArgs()
		{
		}

		public RadItem TemplateInstance
		{
			get
			{
				return this.item;
			}
		}

		private RadItem item = null;
	}
}
