using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	public class RadDateTimePickerDesignTimeData : RadControlDesignTimeData
	{
		public RadDateTimePickerDesignTimeData()
		{ }

		public RadDateTimePickerDesignTimeData(string name)
			: base(name)
		{ }

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
		{
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
			Rectangle editorBounds = new Rectangle(0, 0, 186, 24);
			Rectangle popupBounds = new Rectangle(0, 0, 200, 80);

			// DateTimePicker
			RadDateTimePicker radDateTimePickerPreview = new RadDateTimePicker();
            radDateTimePickerPreview.Text = "RadDropDown";
            radDateTimePickerPreview.Bounds = editorBounds;
            // TODO: remove this PATCH!!!
            radDateTimePickerPreview.LayoutManager.UpdateLayout();

			RadDateTimePicker radDateTimePickerStructure = new RadDateTimePicker();
            radDateTimePickerStructure.Bounds = editorBounds;

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(radDateTimePickerPreview, radDateTimePickerStructure.RootElement);
			designed.Placemenet = PreviewControlPlacemenet.MiddleCenter;
			designed.MainElementClassName = typeof(RadDateTimePickerElement).FullName;
			res.Add(designed);

			// Popup panel
			//ComboBoxPopupControl popupPanelPreview = new ComboBoxPopupControl();
			RadDateTimePickerDropDown popupPanelPreview = new RadDateTimePickerDropDown(null);

			RadCalendar calendar = new RadCalendar();
			calendar.Bounds = popupBounds;

			popupPanelPreview.HostedControl = calendar;
			popupPanelPreview.Bounds = popupBounds;
			//popupPanelPreview.PreviewMode = true;

			RadDateTimePickerDropDown popupPanelStructure = new RadDateTimePickerDropDown(null);

			popupPanelStructure.HostedControl = new RadCalendar();
			popupPanelStructure.Bounds = popupBounds;

		
			designed = new ControlStyleBuilderInfo(popupPanelPreview, popupPanelStructure.RootElement);
			designed.Placemenet = PreviewControlPlacemenet.ParentBottomLeft;
			res.Add(designed);

			return res;
		}
	}
}
