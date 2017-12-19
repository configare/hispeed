using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layout;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadCommandBarOverflowPanel : WrapLayoutPanel
    {

        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF constraint)
        {           
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation);
            UVSize size3 = new UVSize(this.Orientation, constraint.Width, constraint.Height);
            float itemWidth = this.ItemWidth;
            float itemHeight = this.ItemHeight;
            bool flag = !float.IsNaN(itemWidth);
            bool flag2 = !float.IsNaN(itemHeight);
            SizeF availableSize = new SizeF(flag ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
            RadElementCollection internalChildren = base.Children;
            int num3 = 0;
            int count = internalChildren.Count;
            while (num3 < count)
            {
                RadCommandBarBaseItem element = internalChildren[num3] as RadCommandBarBaseItem;
                if (element != null)
                {
                    if (!element.VisibleInStrip)
                    {
                        element.Measure(SizeF.Empty);
                    }
                    else
                    {
                        element.Measure(availableSize);
                    }

                    UVSize size5 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(size.U + size5.U, size3.U))
                    {
                        size2.U = Math.Max(size.U, size2.U);
                        size2.V += size.V;
                        size = size5;
                        if (DoubleUtil.GreaterThan(size5.U, size3.U))
                        {
                            size2.U = Math.Max(size5.U, size2.U);
                            size2.V += size5.V;
                            size = new UVSize(this.Orientation);
                        }
                    }
                    else
                    {
                        size.U += size5.U;
                        size.V = Math.Max(size5.V, size.V);
                    }
                }
                num3++;
            }
            size2.U = Math.Max(size.U, size2.U);
            size2.V += size.V;

        
            return new SizeF(size2.Width, size2.Height);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UVSize
        {
            internal float U;
            internal float V;
            private Orientation orientation;
            internal UVSize(Orientation orientation, float width, float height)
            {
                this.U = this.V = 0;
                this.orientation = orientation;
                this.Width = width;
                this.Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                this.U = this.V = 0;
                this.orientation = orientation;
            }

            internal float Width
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.V;
                    }
                    return this.U;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.U = value;
                    }
                    else
                    {
                        this.V = value;
                    }
                }
            }
            internal float Height
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.U;
                    }
                    return this.V;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.V = value;
                    }
                    else
                    {
                        this.U = value;
                    }
                }
            }
        }
    }
}
