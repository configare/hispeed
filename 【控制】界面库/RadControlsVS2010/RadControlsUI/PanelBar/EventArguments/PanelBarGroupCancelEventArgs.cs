using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	public class PanelBarGroupCancelEventArgs : PanelBarGroupEventArgs
	{
		private bool cancel;
		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				this.cancel = value;
			}
		}

		public PanelBarGroupCancelEventArgs(RadPanelBarGroupElement group, bool cancel)
			: base(group)
		{
			this.cancel = cancel;
		}

	}
}
