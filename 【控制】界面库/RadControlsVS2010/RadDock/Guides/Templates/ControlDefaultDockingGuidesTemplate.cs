using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    internal class ControlDefaultDockingGuidesTemplate : PredefinedDockingGuidesTemplate
    {
        internal override string ResourceFolder
        {
            get
            {
                return "ControlDefault";
            }
        }

        protected override Color DefaultDockingHintBackColor
        {
            get
            {
                return Color.FromArgb(77, 69, 129, 211);
            }
        }

        protected override Color DefaultDockingHintBorderColor
        {
            get
            {
                return Color.FromArgb(15, 92, 197);
            }
        }

        internal override void UpdateImagesMetrics()
        {
            //adjust default locations
            this.LeftImage.LocationOnCenterGuide = new Point(0, 35);
            this.TopImage.LocationOnCenterGuide = new Point(35, 0);
            this.RightImage.LocationOnCenterGuide = new Point(88, 35);
            this.BottomImage.LocationOnCenterGuide = new Point(35, 88);
            this.FillImage.LocationOnCenterGuide = new Point(35, 35);
        }
    }
}
