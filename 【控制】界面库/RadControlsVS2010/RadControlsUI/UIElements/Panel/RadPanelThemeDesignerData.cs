using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    public class RadPanelThemeDesignerData : RadControlDesignTimeData
    {
        /// <exclude/>
        public RadPanelThemeDesignerData()
        { }

        /// <exclude/>
        public RadPanelThemeDesignerData(string name)
            : base(name)
        { }

        /// <exclude/>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            RadPanel panel = new RadPanel();
            panel.AutoSize = true;

            panel.Text = "RadPanel";

            RadPanel panelStructure = new RadPanel();
            panelStructure.AutoSize = true;

            panelStructure.Text = "RadPanel";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(panel, panelStructure.RootElement);
            designed.MainElementClassName = typeof(RadPanelElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
