using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
	public class RadControlDefaultDesignTimeData : RadControlDesignTimeData
	{
		RadControl control;

		public RadControlDefaultDesignTimeData(Control control)
			: base(control.Name)
		{
			this.control = control as RadControl;
		}

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
		{
			ControlStyleBuilderInfo designedControlStyleBuilderInfo = new ControlStyleBuilderInfo(control, control.RootElement); 

			ControlStyleBuilderInfoList styleBuilderInfoList = new ControlStyleBuilderInfoList();
			styleBuilderInfoList.Add(designedControlStyleBuilderInfo);

			return styleBuilderInfoList;
		}
	}
}
