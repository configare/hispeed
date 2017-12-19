using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
	public class RadPanelBarDesignerData : RadControlDesignTimeData
	{
		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadPanelBar pane = new RadPanelBar();
			pane.GroupStyle = PanelBarStyles.OutlookNavPane;

			RadPanelBarGroupElement group = new RadPanelBarGroupElement();
            RadPanelBarGroupElement group1 = new RadPanelBarGroupElement();
            RadPanelBarGroupElement group2 = new RadPanelBarGroupElement();
            RadPanelBarGroupElement group3 = new RadPanelBarGroupElement();

			pane.Items.Add(group);
            pane.Items.Add(group1);
            pane.Items.Add(group2);
            pane.Items.Add(group3);
			pane.Size = new Size(200, 220);

            OutlookStyle outlookStyle = pane.PanelBarElement.CurrentStyle as OutlookStyle;

            if (outlookStyle != null)
            {
                outlookStyle.ShowFewerButtons();
                outlookStyle.ShowFewerButtons();
            }

			ControlStyleBuilderInfoList controlStyleBuilderInfoList = new ControlStyleBuilderInfoList();
			ControlStyleBuilderInfo designControlStyleBuilderInfo = new ControlStyleBuilderInfo(pane, pane.RootElement);
			designControlStyleBuilderInfo.MainElementClassName = typeof(RadPanelBarElement).FullName;
			controlStyleBuilderInfoList.Add(designControlStyleBuilderInfo);
			return controlStyleBuilderInfoList;
		}
	}
}