using System;
using System.Collections.Generic;
using System.Text;
using Telerik;

namespace Telerik.WinControls.UI
{
	public class PanelBarGroupEventArgs : EventArgs
	{
		private RadPanelBarGroupElement group;
		public RadPanelBarGroupElement Group
		{
			get
			{
				return this.group;
			}
		}

		public PanelBarGroupEventArgs(RadPanelBarGroupElement group)
		{
			this.group = group;
		}
	}
}
