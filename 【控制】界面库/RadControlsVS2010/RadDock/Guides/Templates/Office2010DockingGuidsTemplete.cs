using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    internal class Office2010DockingGuidsTemplete : PredefinedDockingGuidesTemplate
    {
        internal override string ResourceFolder
        {
            get
            {
                return "Office2010";
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
            this.LeftImage.LocationOnCenterGuide = new Point(3, 45);
            this.TopImage.LocationOnCenterGuide = new Point(35, 21);
            this.RightImage.LocationOnCenterGuide = new Point(85, 45);
            this.BottomImage.LocationOnCenterGuide = new Point(35, 87);
            this.FillImage.LocationOnCenterGuide = new Point(35, 45);
        }
    }
}
