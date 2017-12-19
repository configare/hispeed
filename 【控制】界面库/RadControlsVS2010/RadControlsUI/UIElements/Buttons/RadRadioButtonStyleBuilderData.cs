using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	/// <exclude/>
	/// <summary>Represents a helper class for the Visual Style Builder.</summary>
	public class RadRadioButtonStyleBuilderData : RadControlDesignTimeData
	{
		/// <summary>
		/// Creates a new instance of RadRadioButtonStyleBuilderData
		/// </summary>
		public RadRadioButtonStyleBuilderData()
		{ }

		/// <summary>
		/// Creates a new instance of RadRadioButtonStyleBuilderData by a given string as a parameter
		/// </summary>
		/// <param name="name"></param>
		public RadRadioButtonStyleBuilderData(string name)
			: base(name)
		{ }

		/// <summary>
		/// Creates and initializes the structure and preview controls in the visual style builder 
		/// </summary>
		/// <param name="previewSurface"></param>
		/// <returns></returns>
		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadRadioButton button = new RadRadioButton();

			button.AutoSize = true;
            button.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;

			button.Text = "RadRadioButton";
            button.Size = new Size(120, 20);

			RadRadioButton buttonStructure = new RadRadioButton();
			button.AutoSize = true;

			buttonStructure.Text = "RadRadioButton";

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
			designed.MainElementClassName = typeof(RadRadioButtonElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}
	}
}
