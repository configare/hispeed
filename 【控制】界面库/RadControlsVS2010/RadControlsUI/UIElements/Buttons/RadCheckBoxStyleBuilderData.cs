using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    /// <summary>Represents a helper class for the Visual Style Builder.</summary>
    public class RadCheckBoxStyleBuilderData : RadControlDesignTimeData
    {
        public RadCheckBoxStyleBuilderData()
        { }

        public RadCheckBoxStyleBuilderData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadCheckBox button = new RadCheckBox();

            button.AutoSize = true;
            button.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;

            button.Text = "RadCheckBox";
            button.Size = new Size(90, 20);

            RadCheckBox buttonStructure = new RadCheckBox();
            button.AutoSize = true;

            buttonStructure.Text = "RadCheckBox";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
            designed.MainElementClassName = typeof(RadCheckBoxElement).FullName;
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
