using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    internal class VS2010DockingGuidesTemplate : PredefinedDockingGuidesTemplate
    {
        internal override string ResourceFolder
        {
            get
            {
                return "VS2010";
            }
        }

        protected override Color DefaultDockingHintBackColor
        {
            get
            {
                return Color.FromArgb(200, 207, 217, 233);
            }
        }

        protected override Color DefaultDockingHintBorderColor
        {
            get
            {
                return Color.FromArgb(200, SystemColors.Control);
            }
        }

        internal override void UpdateImagesMetrics()
        {
            //adjust default locations
            this.LeftImage.LocationOnCenterGuide = new Point(5, 58);
            this.TopImage.LocationOnCenterGuide = new Point(58, 5);
            this.RightImage.LocationOnCenterGuide = new Point(111, 58);
            this.BottomImage.LocationOnCenterGuide = new Point(58, 111);
            this.FillImage.LocationOnCenterGuide = new Point(58, 58);
        }
    }
}
