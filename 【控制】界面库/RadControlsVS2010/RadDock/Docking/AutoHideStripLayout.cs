using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    internal class AutoHideStripLayout : DockLayoutPanel
    {
        private RadDock owner;
        private Padding stripsOffset;

        public AutoHideStripLayout(RadDock owner)
        {
            this.owner = owner;
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            Padding padding = this.owner.Padding;
            constraint.Width -= padding.Horizontal;
            constraint.Height -= padding.Vertical;

            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;

            foreach (AutoHideTabStripElement strip in this.Children)
            {
                if (strip.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                strip.Measure(constraint);

                //tabs position is flipped horizontally and vertically
                switch (strip.TabsPosition)
                {
                    case TabPositions.Left:
                        right = (int)strip.DesiredSize.Width;
                        break;
                    case TabPositions.Top:
                        bottom = (int)strip.DesiredSize.Height;
                        break;
                    case TabPositions.Right:
                        left = (int)strip.DesiredSize.Width;
                        break;
                    case TabPositions.Bottom:
                        top = (int)strip.DesiredSize.Height;
                        break;
                }
            }

            this.stripsOffset = new Padding(left, top, right, bottom);

            if (this.stripsOffset != Padding.Empty)
            {
                //second pass to subtract offsets appropriately
                foreach (AutoHideTabStripElement strip in this.Children)
                {
                    if (strip.Visibility == ElementVisibility.Collapsed)
                    {
                        continue;
                    }

                    switch (strip.TabsPosition)
                    {
                        case TabPositions.Left:
                        case TabPositions.Right:
                            strip.Measure(new SizeF(constraint.Width, constraint.Height - (top + bottom)));
                            break;
                        case TabPositions.Top:
                        case TabPositions.Bottom:
                            strip.Measure(new SizeF(constraint.Width - (left + right), constraint.Height));
                            break;
                    }
                }
            }

            return constraint;
        }

        protected override SizeF ArrangeOverride(SizeF arrangeSize)
        {
            RectangleF rect = RectangleF.Empty;

            foreach (AutoHideTabStripElement strip in this.Children)
            {
                //tabs position is flipped horizontally and vertically
                switch (strip.TabsPosition)
                {
                    case TabPositions.Right://left strip
                        rect = new RectangleF(0, this.stripsOffset.Top, strip.DesiredSize.Width, arrangeSize.Height - this.stripsOffset.Vertical);
                        break;
                    case TabPositions.Bottom://top strip
                        rect = new RectangleF(this.stripsOffset.Left, 0, arrangeSize.Width - this.stripsOffset.Horizontal, strip.DesiredSize.Height);
                        break;
                    case TabPositions.Left://right strip
                        rect = new RectangleF(arrangeSize.Width - strip.DesiredSize.Width, this.stripsOffset.Top, strip.DesiredSize.Width, arrangeSize.Height - this.stripsOffset.Vertical);
                        break;
                    case TabPositions.Top://bottom strip
                        rect = new RectangleF(this.stripsOffset.Left, arrangeSize.Height - strip.DesiredSize.Height, arrangeSize.Width - this.stripsOffset.Horizontal, strip.DesiredSize.Height);
                        break;
                }

                strip.Arrange(rect);
            }

            return arrangeSize;
        }
    }
}
