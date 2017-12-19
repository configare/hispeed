using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    public class RadLabelThemeDesignerData : RadControlDesignTimeData
    {
        public RadLabelThemeDesignerData()
        { }

        public RadLabelThemeDesignerData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {

            RadLabel label = new RadLabel();
            label.Text = "RadLabel";
            label.Size = new Size(200, 50);
            label.AutoSize = true;

            RadLabel labelStructure = new RadLabel();
            labelStructure.AutoSize = true;

            labelStructure.Text = "RadLabel";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(label, labelStructure.RootElement);
            designed.MainElementClassName = typeof(RadLabelElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
