using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    public class RadScrollablePanelThemeDesignerData : RadControlDesignTimeData
    {
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            ControlStyleBuilderInfoList result = new ControlStyleBuilderInfoList();

            RadScrollablePanel panelPreview = new RadScrollablePanel();
            panelPreview.Size = new System.Drawing.Size(450, 450);


            RadScrollablePanel panelStructure = new RadScrollablePanel();
            panelStructure.Size = new System.Drawing.Size(450, 450);

            ControlStyleBuilderInfo info = new ControlStyleBuilderInfo(panelPreview, panelStructure.RootElement);

            result.Add(info);

            return result;
        }
    }
}
