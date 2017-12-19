using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    ///<exclude/>
    public class RadScrollBarDesignTimeData : RadControlDesignTimeData
    {
        public RadScrollBarDesignTimeData()
        { }

        public RadScrollBarDesignTimeData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            RadHScrollBar scrollBarPreview = new RadHScrollBar();
            scrollBarPreview.Value = 50;
            scrollBarPreview.ThemeName = "";
            scrollBarPreview.Bounds = new Rectangle(0, 0, 200, 20);

            RadHScrollBar scrollBarStructure = new RadHScrollBar();
            scrollBarStructure.Bounds = new Rectangle(0, 0, 200, 20);

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(scrollBarPreview, scrollBarStructure.RootElement);
			designed.MainElementClassName = typeof(RadScrollBarElement).FullName;
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
