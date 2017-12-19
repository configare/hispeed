using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
    public class ToolStripElementLayout : LayoutPanel
	{
        private RadToolStripElement parentToolStripElement;

        /// <summary>Gets the toolstrip parent (a ToolStripManager instnace).</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripElement ParentToolStripElement
		{
			get
			{
				if (this.parentToolStripElement == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripElement == null; res = res.Parent)
					{
						this.parentToolStripElement = res as RadToolStripElement;
					}
				}
				return this.parentToolStripElement;
			}
		}

        private Orientation orientation = Orientation.Horizontal;
        /// <summary>
        /// Gets or sets orientation - it could be horizontal or vertical.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadDefaultValue("Orientation", typeof(RadToolStripItem))]
        internal Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (value != this.orientation)
                {
                    this.orientation = value;

                    if (value == Orientation.Horizontal)
                    {
                        for (int i = 0; i < this.Children.Count; i++ )
                        {
                            RadToolStripItem toolStripItem = this.Children[i] as RadToolStripItem;
                            if (toolStripItem != null)
                            {
                                toolStripItem.Margin = new Padding(toolStripItem.Margin.Top, 0, toolStripItem.Margin.Right, toolStripItem.Margin.Bottom);
                                toolStripItem.Orientation = Orientation.Horizontal;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.Children.Count; i++)
                        {
                            RadToolStripItem toolStripItem = this.Children[i] as RadToolStripItem;
                            if (toolStripItem != null)
                            {
                                toolStripItem.Margin = new Padding(0, toolStripItem.Margin.Left, toolStripItem.Margin.Right, toolStripItem.Margin.Bottom);
                                toolStripItem.Orientation = Orientation.Vertical;
                            }
                        }
                    }
                }
            }
        }


        protected virtual int GetInvariantLength(Size size, Padding margin)
        {
            return this.Orientation == Orientation.Horizontal ?
                size.Width + margin.Horizontal :
                size.Height + margin.Vertical;
        }

        private Size GetElementPreferredSize(Rectangle bounds, PreferredSizeData data)
        {
            if (this.Orientation == Orientation.Horizontal)
                return new Size(data.PreferredSize.Width, bounds.Height);
            else
                return new Size(bounds.Width, data.PreferredSize.Height);
        }

        protected virtual void FillPrefSizeList(List<PreferredSizeData> prefSizelist, Rectangle bounds)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                // The constructor of PreferredSizeData will call GetPreferredSize()
                PreferredSizeData prefSizedata = new PreferredSizeData(child, bounds);
                prefSizelist.Add(prefSizedata);
            }
        }

        protected virtual Size GetChildrenListSize(List<PreferredSizeData> list)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                int sumWidth = 0;
                int maxHeight = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    PreferredSizeData data = list[i];
                    sumWidth += data.PreferredSize.Width + data.Element.Margin.Horizontal;
                    if (maxHeight < data.PreferredSize.Height)
                    {
                        maxHeight = data.PreferredSize.Height;
                    }
                }
                return new Size(sumWidth, maxHeight);
            }

            int maxWidth = 0;
            int sumHeight = 0;
            for (int i = 0; i < list.Count; i++)
            {
                PreferredSizeData data = list[i];
                if (maxWidth < data.PreferredSize.Width)
                {
                    maxWidth = data.PreferredSize.Width;
                }
                sumHeight += data.PreferredSize.Height + data.Element.Margin.Vertical;
            }
            return new Size(maxWidth, sumHeight);
        }

        protected virtual void SetElementBoundsAuto(Rectangle bounds, PreferredSizeData data, int offset)
        {
            Point location = new Point();
            if (this.Orientation == Orientation.Horizontal)
            {
                location.X = offset;
            }
            else
            {
                location.Y = offset;
            }

            if (data.Element.AutoSize)
            {
                Size prefSize = this.GetElementPreferredSize(bounds, data);
                data.Element.SetBounds(new Rectangle(location, prefSize));
            }
            else
            {
                data.Element.Location = location;
            }
        }

        // return how many pixels are not compensated
        private int LowerMarginsToFit(List<PreferredSizeData> prefSizelist, int compensation)
        {
            int i;
            for (i = prefSizelist.Count - 1; i >= 0; i--)
            {
                PreferredSizeData data = prefSizelist[i];
                if (data.Element.Visibility != ElementVisibility.Collapsed)
                {
                    Padding margin = data.Element.Margin;
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        if (compensation >= margin.Horizontal)
                        {
                            compensation -= margin.Horizontal;
                            margin.Left = 0;
                            margin.Right = 0;
                        }
                        else if (compensation >= margin.Right)
                        {
                            margin.Right = 0;
                            compensation -= margin.Right;
                            margin.Left = margin.Left - compensation;
                            compensation = 0;
                        }
                        else
                        {
                            margin.Right = margin.Right - compensation;
                            compensation = 0;
                        }
                    }
                    else
                    {
                        if (compensation >= margin.Vertical)
                        {
                            compensation -= margin.Vertical;
                            margin.Top = 0;
                            margin.Bottom = 0;
                        }
                        else if (compensation >= margin.Bottom)
                        {
                            margin.Bottom = 0;
                            compensation -= margin.Bottom;
                            margin.Top = margin.Top - compensation;
                            compensation = 0;
                        }
                        else
                        {
                            margin.Bottom = margin.Bottom - compensation;
                            compensation = 0;
                        }
                    }
                    data.Element.Margin = margin;
                    if (compensation <= 0)
                        return 0;
                }
            }
            return compensation;
        }

        private int LowerSizesToFit(List<PreferredSizeData> prefSizelist, int compensation)
        {
            int i;
            for (i = prefSizelist.Count - 1; i >= 0; i--)
            {
                PreferredSizeData data = prefSizelist[i];
                if (data.Element.Visibility != ElementVisibility.Collapsed)
                {
                    Size preferredSize = data.PreferredSize;
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        int delta = preferredSize.Width - data.Element.MinSize.Width;
                        if (compensation >= delta)
                        {
                            data.PreferredSize = new Size(data.Element.MinSize.Width, preferredSize.Height);
                            compensation -= delta;
                        }
                        else
                        {
                            data.PreferredSize = new Size(preferredSize.Width - compensation, preferredSize.Height);
                            compensation = 0;
                        }
                    }
                    else
                    {
                        int delta = preferredSize.Height - data.Element.MinSize.Height;
                        if (compensation >= delta)
                        {
                            data.PreferredSize = new Size(preferredSize.Width, data.Element.MinSize.Height);
                            compensation -= delta;
                        }
                        else
                        {
                            data.PreferredSize = new Size(preferredSize.Width, preferredSize.Height - compensation);
                            compensation = 0;
                        }
                    }
                    if (compensation <= 0)
                        return 0;
                }
            }
            return compensation;
        }

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            Size totalAvailableSize = TelerikLayoutEngine.CheckSize(this.Parent.Size, this.MaxSize, this.MinSize);
            Rectangle bounds = new Rectangle(Point.Empty, totalAvailableSize);

            List<PreferredSizeData> prefSizelist = new List<PreferredSizeData>();
            FillPrefSizeList(prefSizelist, bounds);

            Size childrenSize = GetChildrenListSize(prefSizelist);

            int compensation = this.Orientation == Orientation.Horizontal ?
                childrenSize.Width - bounds.Width : childrenSize.Height - bounds.Height;
            if (compensation > 0)
                compensation = LowerMarginsToFit(prefSizelist, compensation);
            if (compensation > 0)
                compensation = LowerSizesToFit(prefSizelist, compensation);

            bounds.Size = childrenSize;

            int nextLeftTop = 0;
            foreach (PreferredSizeData data in prefSizelist)
            {
                if (data.Element.Visibility != ElementVisibility.Collapsed)
                {
                    SetElementBoundsAuto(bounds, data, nextLeftTop);
                    nextLeftTop += GetInvariantLength(data.Element.BoundingRectangle.Size, data.Element.Margin);
                }
            }
        }

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = Size.Empty;
            if (AutoSize && this.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize)
            {
                if (this.Orientation == Orientation.Horizontal)
                    res = GetHorizontalSize();
                else
                    res = GetVerticalSize();
            }
            else res = base.GetPreferredSizeCore(proposedSize);

            return res;
        }

        protected virtual Size GetHorizontalSize()
        {
            int maxHeight = 0, width = 0;

            foreach(RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                Rectangle fullRect = child.FullBoundingRectangle;
                width += fullRect.Width;
                maxHeight = Math.Max(maxHeight, fullRect.Height);
            }

            return new Size(width, maxHeight);
        }

        protected virtual Size GetVerticalSize()
        {
            int height = 0, maxWidth = 0;

            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                Rectangle fullRect = child.FullBoundingRectangle;
                height += fullRect.Height;
                maxWidth = Math.Max(maxWidth, fullRect.Width);
            }

            return new Size(maxWidth, height);
        }
    }
}
