using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
namespace Telerik.WinControls.UI
{
	public class ToolStripFlowLayout : StackLayoutPanel
	{
		private Form containerForm;

		public ToolStripFlowLayout(Form containerForm)
		{
			this.containerForm = containerForm;
			this.containerForm.SizeChanged += new EventHandler(containerForm_SizeChanged);
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AllElementsEqualSize = true;
        }

        /// <summary>
        /// Gets or sets the number of the row. 
        /// </summary>
		public int Row
		{
			get
			{
				return (int)this.GetValue(RowProperty);
			}
			set
			{
				this.SetValue(RowProperty, value);
			}
		}

		private void containerForm_SizeChanged(object sender, EventArgs e)
		{
			this.GetPreferredSizeCore(this.containerForm.Size);
			this.PerformLayoutCore(this);
		}

		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			RadElement child = null;
			if (this.Children.Count > 0)
				child = this.Children[0];

			if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
			{
				return this.containerForm.Size;
			}
			
			return new Size(this.containerForm.Size.Width - 4, 1000);

		}

		private bool IsCollapsedElement(RadElement element)
		{
			ICollapsibleElement collapseElement = element as ICollapsibleElement;

			if (collapseElement == null)
				return false;

			return element.Visibility == ElementVisibility.Collapsed;
		}

		private bool IsExpandedElement(RadElement element)
		{
			ICollapsibleElement collapseElement = element as ICollapsibleElement;

			if (collapseElement == null)
				return false;

			return !(element.Visibility == ElementVisibility.Collapsed);
		}

		private void CollapseElements(List<PreferredSizeData> list, Size availableSize, int nextLeftSaved, int rowElements)
		{
			int nextLeft = nextLeftSaved;
			int firstExpandedIndex = -1;

			for (int i = list.Count - rowElements - 1; i >= 0; i--)
			{
				PreferredSizeData data = list[i];

				int step = GetDataSize(data, false).Width + data.Element.Margin.Horizontal;

				if ((firstExpandedIndex == -1) && IsExpandedElement(data.Element))
				{
					firstExpandedIndex = i;
					nextLeftSaved = nextLeft + step;
				}

				if (nextLeft + step > availableSize.Width)
				{
					if (firstExpandedIndex != -1)
					{
						ICollapsibleElement lastExpandedElement = (ICollapsibleElement)list[firstExpandedIndex].Element;
						lastExpandedElement.ExpandedSize = list[firstExpandedIndex].PreferredSize;
						lastExpandedElement.CollapseElement(list[firstExpandedIndex].PreferredSize);
					}

					CollapseElements(list, availableSize, nextLeftSaved, list.Count - firstExpandedIndex - 1);
					return;
				}

				nextLeft += step;
				rowElements++;
			}
		}

		private void ExpandElements(List<PreferredSizeData> list, Size availableSize, int nextLeftSaved, int rowElements)
		{
			int nextLeft = nextLeftSaved;
			int firstCollapsedIndex = -1;

			for (int i = rowElements; i < list.Count; i++)
			{
				PreferredSizeData data = list[i];

				int step = GetDataSize(data, false).Width + data.Element.Margin.Horizontal;

				if ((firstCollapsedIndex == -1) && IsCollapsedElement(data.Element))
				{
					firstCollapsedIndex = i;
					nextLeftSaved = nextLeft + step;
				}

				if (nextLeft + step > availableSize.Width)
					return;

				nextLeft += step;
			}

			if (firstCollapsedIndex != -1)
			{
				ICollapsibleElement firstCollapsedElement = (ICollapsibleElement)list[firstCollapsedIndex].Element;
				if ((firstCollapsedElement.ExpandedSize != Size.Empty) &&
					(nextLeft - firstCollapsedElement.ExpandedSize.Width + firstCollapsedElement.ExpandedSize.Width <= availableSize.Width))
				{
					firstCollapsedElement.ExpandElement();
					ExpandElements(list, availableSize, nextLeftSaved, firstCollapsedIndex++);
				}
			}
		}

		private Size LayoutHorizontal(Size availableSize)
		{
			int nextTop = 0, nextLeft;
			int maxRowHeight = 0, row = 0;
			int rowElements = 0;
			int width = 0;
			int maxWidth = 0;
			Size maxSize = Size.Empty;

			List<PreferredSizeData> list = new List<PreferredSizeData>();
			FillList(list, availableSize);

			maxSize = GetMaxSize(list);

			nextLeft = !this.RightToLeft ? 0 : availableSize.Width;

	     	if (this.CollapseElementsOnResize && !this.IsDesignMode)
			{
				CollapseElements(list, availableSize, 0, 0);

				ExpandElements(list, availableSize, 0, 0);
			}

			foreach (PreferredSizeData data in list)
			{
				if (this.AllElementsEqualSize)
				{
					if (this.FlipMaxSizeDimensions)
						data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);
					else
						data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);
				}

				int step = GetDataSize(data, true).Width + data.Element.Margin.Horizontal;

				if (!this.RightToLeft)
				{
					if ((nextLeft + step > availableSize.Width) && (rowElements > 0))
					{
						nextTop += maxRowHeight;
						nextLeft = 0;
						maxRowHeight = 0;
						rowElements = 0;
						row++;
					}
					data.Element.Bounds = new Rectangle(new Point(nextLeft, nextTop), GetDataSize(data, false));
				}
				else
				{
					if ((nextLeft - step < 0) && (rowElements > 0))
					{
						nextTop += maxRowHeight;
						nextLeft = availableSize.Width;
						maxRowHeight = 0;
						rowElements = 0;
						row++;
						width = 0;
					}
					data.Element.Bounds = new Rectangle(new Point(nextLeft - step, nextTop), GetDataSize(data, false));
				}

				if (!this.RightToLeft)
				{
					nextLeft += step;
				}
				else
				{
					nextLeft -= step;
				}
				data.Element.SetValue(RowProperty, row);
				rowElements++;
				width += GetDataSize(data, true).Width;
				maxWidth = Math.Max(maxWidth, width);
				maxRowHeight = Math.Max(maxRowHeight, GetDataSize(data, true).Height + data.Element.Margin.Vertical);
			}

			if (RightToLeft && (this.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize))
			{
				int difference = availableSize.Width - maxWidth;
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
				{
					child.Location = new Point(child.Location.X - difference, child.Location.Y);
				}
			}
			
			return new Size(maxWidth, Math.Max(maxRowHeight, nextTop));
		}

		private Size LayoutVertical(Size availableSize)
		{
			int row = 0;
			int nextTop = 0;
			Size maxSize = Size.Empty;

			List<PreferredSizeData> list = new List<PreferredSizeData>();
			FillList(list, availableSize);

			maxSize = GetMaxSize(list);

			foreach (PreferredSizeData data in list)
			{
				data.Element.SetValue(RowProperty, row);

				if (this.AllElementsEqualSize)
				{
					if (this.FlipMaxSizeDimensions)
						data.PreferredSize = new Size(data.PreferredSize.Width, maxSize.Height);
					else
						data.PreferredSize = new Size(maxSize.Width, data.PreferredSize.Height);
				}

				if (this.RightToLeft)
				{
					data.Element.Location = new Point((this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize ? availableSize.Width : maxSize.Width) - data.PreferredSize.Width, nextTop);
				}
				else
				{
					data.Element.Location = new Point(0, nextTop);
				}

				if (data.Element.AutoSize)
				{
					if (this.AllElementsEqualSize)
						data.Element.CoercedSize = data.PreferredSize;
					else
						data.Element.Size = data.PreferredSize;
				}

				nextTop += data.Element.BoundingRectangle.Height + data.Element.Margin.Vertical;
				row++;
			}

			return new Size(maxSize.Width, nextTop);
		}

		public override void PerformLayoutCore(RadElement affectedElement)
		{
			Size availableSize = this.UseParentSizeAsAvailableSize ? this.Parent.Size : this.AvailableSize;
			Size maxPanelSize;
			if (this.containerForm != null)
				if (this.Orientation == Orientation.Horizontal)
					maxPanelSize = LayoutHorizontal(new Size(this.containerForm.Size.Width - 4, this.containerForm.Height - 4));
				else
					maxPanelSize = LayoutVertical(new Size(this.containerForm.Size.Width - 4, this.containerForm.Height - 4));
			else
			{
				if (this.Orientation == Orientation.Horizontal)
					maxPanelSize = LayoutHorizontal(availableSize);
				else
					maxPanelSize = LayoutVertical(availableSize);

			}
		}
		private Size GetHorizontalSize()
		{
			int row = 0;
			int maxRowHeight = 0, maxWidth = 0;
			int width = 0, height = 0;

			foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
			{
				int newRow = (int)child.GetValue(RowProperty);

				if (newRow != row)
				{
					height += maxRowHeight;
					maxWidth = Math.Max(maxWidth, width);

					maxRowHeight = 0;
					width = 0;
					row = newRow;
				}

				width += child.FullBoundingRectangle.Width;
				maxRowHeight = Math.Max(maxRowHeight, child.FullBoundingRectangle.Height);
			}
			if (this.LayoutableChildrenCount > 0)
			{
				height += maxRowHeight;
				maxWidth = Math.Max(maxWidth, width);
			}

			return new Size(maxWidth, height);
		}

		private Size GetVerticalSize()
		{
			int height = 0, maxWidth = 0;

			foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
			{
				height += child.FullBoundingRectangle.Height;
				maxWidth = Math.Max(maxWidth, child.FullBoundingRectangle.Width);
			}

			return new Size(maxWidth, height);
		}
	}
}
