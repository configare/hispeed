using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	///<exclude/>
	public class RadVScrollBarDesignTimeData : RadControlDesignTimeData
	{
		public RadVScrollBarDesignTimeData()
		{ }

		public RadVScrollBarDesignTimeData(string name)
			: base(name)
		{ }

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
		{
			RadVScrollBar scrollBarPreview = new RadVScrollBar();
			scrollBarPreview.Value = 50;
			scrollBarPreview.ThemeName = "";
			scrollBarPreview.Bounds = new Rectangle(0, 0, 20, 100);

			RadVScrollBar scrollBarStructure = new RadVScrollBar();
			scrollBarStructure.Bounds = new Rectangle(0, 0, 20, 100);

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(scrollBarPreview, scrollBarStructure.RootElement);
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}
	}
}
