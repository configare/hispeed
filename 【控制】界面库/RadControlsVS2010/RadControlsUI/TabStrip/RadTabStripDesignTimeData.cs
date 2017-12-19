using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    ///<exclude/>
    public class RadTabStripDesignTimeData: RadControlDesignTimeData
    {
        public RadTabStripDesignTimeData()            
        { }

        public RadTabStripDesignTimeData(string name)
            : base(name)
        {}

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadTabStrip tabStrip = new RadTabStrip();
			tabStrip.Bounds = new System.Drawing.Rectangle(30, 30, 300, 50);

			tabStrip.TabStripElement.Bounds = new System.Drawing.Rectangle(0, 0, 298, 50);

			tabStrip.TabStripElement.Items.Add(new TabItem("TabItem1"));
			tabStrip.TabStripElement.Items.Add(new TabItem("TabItem2"));
			tabStrip.TabStripElement.Items.Add(new TabItem("TabItem3"));

			RadTabStrip tabStripStructure = new RadTabStrip();

			tabStripStructure.TabStripElement.Items.Add(new TabItem("Tab Item Text"));

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(tabStrip, tabStripStructure.RootElement);
			designed.MainElementClassName = typeof(RadTabStripElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}
    }
}
