using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <exclude/>
    public class RadOffice2007ScreenTipDesignTimeData : RadControlDesignTimeData
    {
        public RadOffice2007ScreenTipDesignTimeData()
        { }

        public RadOffice2007ScreenTipDesignTimeData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            RadOffice2007ScreenTip screenTip = new RadOffice2007ScreenTip();

            RadOffice2007ScreenTip screenTipStructure = new RadOffice2007ScreenTip();

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(screenTip, screenTipStructure.RootElement);
            designed.MainElementClassName = typeof(RadOffice2007ScreenTipElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
