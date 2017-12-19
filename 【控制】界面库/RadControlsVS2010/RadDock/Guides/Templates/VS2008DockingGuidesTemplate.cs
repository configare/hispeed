using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    internal class VS2008DockingGuidesTemplate : PredefinedDockingGuidesTemplate
    {
        internal override string ResourceFolder
        {
            get
            {
                return "VS2008";
            }
        }

        protected override Color DefaultDockingHintBackColor
        {
            get
            {
                return Color.FromArgb(150, SystemColors.Highlight);
            }
        }

        protected override Color DefaultDockingHintBorderColor
        {
            get
            {
                return Color.FromArgb(150, SystemColors.Control);
            }
        }

        internal override void UpdateImagesMetrics()
        {
            //adjust default locations
            this.LeftImage.LocationOnCenterGuide = new Point(0, 28);
            this.TopImage.LocationOnCenterGuide = new Point(28, 0);
            this.RightImage.LocationOnCenterGuide = new Point(65, 28);
            this.BottomImage.LocationOnCenterGuide = new Point(28, 65);
            this.FillImage.LocationOnCenterGuide = new Point(28, 28);
        }
    }
}
