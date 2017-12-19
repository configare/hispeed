using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	public class PanelBarBaseLayout : LayoutPanel
	{
		public event EventHandler ArrangeModified;

		protected virtual void OnArrangeModified(EventArgs e)
		{
			if (this.ArrangeModified != null)
			{
				this.ArrangeModified(this, e);
			}
		}

		public event EventHandler MeasureModified;

		protected virtual void OnMeasureModified(EventArgs e)
		{
			if (this.MeasureModified != null)
			{
				this.MeasureModified(this, e);
			}
		}

		internal void CallArrangeModified(EventArgs e)
		{
			this.OnArrangeModified(e);
		}

		internal void CallMeasureModified(EventArgs e)
		{
			this.OnMeasureModified(e);
		}

	}
}
