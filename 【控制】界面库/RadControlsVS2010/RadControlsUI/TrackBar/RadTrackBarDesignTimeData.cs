using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
namespace Telerik.WinControls.UI
{
	///<exclude/>
	public class RadTrackBarDesignTimeData : RadControlDesignTimeData
	{
		
		public RadTrackBarDesignTimeData()
		{
		}

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadTrackBar trackBar = new RadTrackBar();
			trackBar.Bounds = new System.Drawing.Rectangle(0, 0, 150, 30);

			trackBar.TrackBarElement.Bounds = new System.Drawing.Rectangle(0, 0, 150, 30);

			RadTrackBar trackBarStructure = new RadTrackBar();
			trackBarStructure.Bounds = new System.Drawing.Rectangle(0, 0, 150, 30);

			trackBarStructure.TrackBarElement.Bounds = new System.Drawing.Rectangle(0, 0, 150, 30);

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(trackBar, trackBarStructure.RootElement);
			designed.MainElementClassName = typeof(RadTrackBarElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}
	}
}
