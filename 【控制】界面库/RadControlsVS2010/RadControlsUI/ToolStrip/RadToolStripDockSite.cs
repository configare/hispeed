using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
namespace Telerik.WinControls.UI
{
	public class RadToolStripDockSite
	{
		private ArrayList toolStripsList;

		public RadToolStripDockSite()
		{
			this.toolStripsList = new ArrayList();
		}

		#if RIBBONBAR
		internal void AddToolStrip(RadToolStrip ToolStrip)
		#else
		public void AddToolStrip(RadToolStrip ToolStrip)
		#endif
		{
			this.toolStripsList.Add(ToolStrip);
			ToolStrip.ToolStripManager.DockingSites = this.toolStripsList;
		}

		public ArrayList ToolStrips
		{
			get
			{
				return this.toolStripsList;
			}
		}
	}

}
