using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class is used to prepare a RadForm instance
    /// for designing in the Visual Style Builder
    /// </summary>
    public class RadFormThemeDesignerData : RadControlDesignTimeData
    {
         public RadFormThemeDesignerData()
        { }

         public RadFormThemeDesignerData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadForm radFormPreview = new RadForm();
            radFormPreview.Behavior.DisableMouseEvents = true;
            radFormPreview.Size = new Size(320, 240);
            radFormPreview.AutoScroll = true;

            System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
            btn.Location = new Point(3000, 3000);

            radFormPreview.Controls.Add(btn);
            radFormPreview.Text = "RadForm";

            radFormPreview.Anchor =
              System.Windows.Forms.AnchorStyles.Bottom
              | System.Windows.Forms.AnchorStyles.Left
              | System.Windows.Forms.AnchorStyles.Right
              | System.Windows.Forms.AnchorStyles.Top;

            RadForm radFormStructure = new RadForm();
            radFormStructure.AutoSize = true;

            radFormStructure.Text = "RadForm"; 
            radFormStructure.Size = new Size(320, 240);

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(radFormPreview, radFormStructure.RootElement);
            designed.MainElementClassName = radFormStructure.FormElement.GetThemeEffectiveType().FullName;
            designed.ExcludeStructureElementsAndHierarchy.Add(radFormStructure.FormElement.TitleBar);
            designed.ExcludeStructureElementsAndHierarchy.Add(radFormStructure.FormElement.HorizontalScrollbar);
            designed.ExcludeStructureElementsAndHierarchy.Add(radFormStructure.FormElement.VerticalScrollbar);
            res.Add(designed);

            return res;
        }
    }
}
