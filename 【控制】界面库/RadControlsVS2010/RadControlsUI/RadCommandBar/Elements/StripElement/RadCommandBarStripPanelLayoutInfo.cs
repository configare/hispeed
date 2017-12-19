using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadCommandBarStripPanelLayoutInfo
    {
        public RadCommandBarStripPanelLayoutInfo(CommandBarStripElement commandBarStripElement)
        {
            this.commandBarStripElement = commandBarStripElement;
            this.ControlBoundingRectangle = commandBarStripElement.ControlBoundingRectangle;
            this.DesiredLocation = commandBarStripElement.DesiredLocation;
            this.ArrangeRectangle = RectangleF.Empty;
            this.DesiredSpaceToEnd = 0;
            this.IntersectionSpaceToEnd = 0;
            this.MinSpaceToEnd = 0;
        }

        //Fields
        public RectangleF ControlBoundingRectangle;
        public PointF DesiredLocation;
        public RectangleF ArrangeRectangle;
        public CommandBarStripElement commandBarStripElement;
        public float DesiredSpaceToEnd;
        public float IntersectionSpaceToEnd;
        public float MinSpaceToEnd;
        public SizeF ExpectedDesiredSize;
    }
}
