using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public class OutLookStyleLayout : PanelBarBaseLayout
    {
        private RadPanelBarVisualElement captionElement;
        private RadPanelBarOverFlow panelBarOverFlow;
        private OutLookGripPrimitive grip;
        private RadHostItem host;

        public OutLookStyleLayout(RadPanelBarVisualElement captionElement, RadPanelBarOverFlow panelBarOverFlow, OutLookGripPrimitive grip, RadHostItem host)
        {
            this.captionElement = captionElement;
            this.panelBarOverFlow = panelBarOverFlow;
            this.grip = grip;
            this.host = host;

            this.Children.Add(this.captionElement);
            this.captionElement.StretchHorizontally = true;
            this.Children.Add(this.panelBarOverFlow);
            this.Children.Add(this.grip);

            if (this.host != null)
            {
                this.Children.Add(this.host);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF sz = base.MeasureOverride(availableSize);

            if (Double.IsNaN(availableSize.Width) || Double.IsNaN(availableSize.Height))
            {
                return sz;
            }

            if (Double.IsInfinity(availableSize.Width) || Double.IsInfinity(availableSize.Height))
            {
                return sz;
            }
       
            return availableSize;
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
         //   base.ArrangeOverride(finalSize);
            float y = 0;
            float groupsHeight = 0;

            SizeF captionSize = new SizeF(
                        finalSize.Width - 2,
                        this.captionElement.DesiredSize.Height);

            this.captionElement.Arrange(new RectangleF(1, 1, captionSize.Width, captionSize.Height));

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is RadPanelBarGroupElement)
                {
                    RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];

                    if (group.Visibility == ElementVisibility.Visible)
                    {
                        groupsHeight += group.CaptionHeight -1 ;
                    }
                }
            }

            RectangleF hostBounds = new RectangleF(0, 1 + captionSize.Height, finalSize.Width - 2,
                finalSize.Height - groupsHeight - this.panelBarOverFlow.DesiredSize.Height - captionSize.Height - 8);

            this.host.Arrange(hostBounds);

            groupsHeight += 8;
            y += finalSize.Height - groupsHeight - this.panelBarOverFlow.DesiredSize.Height;
            this.grip.Arrange(new RectangleF(1, y, finalSize.Width - 2, 8));

            y += 8;

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] is RadPanelBarGroupElement)
                {
                    RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.Children[i];

                    if (group.Visibility == ElementVisibility.Visible)
                    {
                        PointF groupLocation = new PointF(1, y);
                        SizeF groupSize = new SizeF(
                            finalSize.Width - 2,
                            group.CaptionHeight-1);

                        RectangleF groupBounds = new RectangleF(groupLocation, groupSize);
                        group.Arrange(groupBounds);
                        y += groupSize.Height;
                    }
                }
            }

            this.panelBarOverFlow.Arrange(new RectangleF(1, y, finalSize.Width - 2, this.panelBarOverFlow.DesiredSize.Height));
            


            this.OnArrangeModified(EventArgs.Empty);
            return finalSize;
        }




    }
}
