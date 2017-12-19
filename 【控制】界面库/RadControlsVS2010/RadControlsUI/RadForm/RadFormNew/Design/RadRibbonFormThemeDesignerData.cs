using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Contains information which is used when loading the control
    /// in the Visual Style Builder
    /// </summary>
    public class RadRibbonFormThemeDesignerData : RadControlDesignTimeData
    {
        #region Methods

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadRibbonForm radRibbonFormPreview = new RadRibbonForm();
            
            radRibbonFormPreview.Size = new Size(320, 240);
            RadRibbonBar ribbonbar = new RadRibbonBar();
            ribbonbar.Dock = System.Windows.Forms.DockStyle.Top;
            ribbonbar.Enabled = false;
            radRibbonFormPreview.Controls.Add(ribbonbar);

            radRibbonFormPreview.Text = "RadRibbonForm";

            radRibbonFormPreview.Anchor = 
                System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right
                | System.Windows.Forms.AnchorStyles.Top;
            radRibbonFormPreview.AllowAero = false;

            RadRibbonForm radRibbonFormStructure = new RadRibbonForm();
            radRibbonFormStructure.AutoSize = true;

            radRibbonFormStructure.Text = "RadRibbonForm";
            radRibbonFormStructure.Size = new Size(320, 240);

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(radRibbonFormPreview, radRibbonFormStructure.RootElement);
            designed.MainElementClassName = radRibbonFormStructure.FormElement.GetThemeEffectiveType().FullName;

            res.Add(designed);

            return res;
        }

        #endregion
    }
}
