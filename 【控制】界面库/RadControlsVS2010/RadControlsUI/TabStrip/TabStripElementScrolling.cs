using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.TabStrip
{
	public class TabStripElementScrolling
	{
		public RadRepeatScrollButtonElement leftButton;
        public RadRepeatScrollButtonElement rightButton;
        public RadRepeatScrollButtonElement upButton;
        public RadRepeatScrollButtonElement downButton;

        private TabItem lastItem;

		private Stack<int> scrollWidth = new Stack<int>();
		private Stack<int> scrollHeight = new Stack<int>();

		private RadTabStripElement tabStripElement;
		private TabLayoutPanel tabLayout;
      
        private int lastItemWidth;
        private int lastItemHeight;
		private int scrollingItemIndex = -1;
		private int scrollOffsetStep;

		private Size tabLayoutSize;


		public TabStripElementScrolling(RadTabStripElement tabStripElement)
		{
			this.tabStripElement = tabStripElement;
            tabStripElement.RadPropertyChanged += new RadPropertyChangedEventHandler(tabStripElement_RadPropertyChanged);
        }

        private void tabStripElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadTabStripElement.ScrollItemsOffsetProperty)
            {
                if ((int)e.NewValue == 0)
                {
                    if (this.leftButton != null && this.rightButton != null && this.upButton != null && this.downButton != null)
                    {
                        this.leftButton.Enabled = false;
                        this.rightButton.Enabled = true;
                        this.upButton.Enabled = false;
                        this.downButton.Enabled = true;
                    }
                }
            }
        }

		public void InitializeScrollElements(TabLayoutPanel scrollElementsParent)
		{
			this.tabLayout = scrollElementsParent;

			this.leftButton = new RadRepeatScrollButtonElement(ScrollButtonDirection.Left);      
            this.leftButton.ZIndex = 10000;
            this.leftButton.Class = "TabStripLeftButton";
            this.leftButton.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.rightButton = new RadRepeatScrollButtonElement(ScrollButtonDirection.Right);
            this.rightButton.ZIndex = 10000;
            this.rightButton.Class = "TabStripRightButton";
            this.rightButton.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            
            this.leftButton.Alignment = ContentAlignment.TopLeft;
            this.rightButton.Alignment = ContentAlignment.TopLeft;
            this.leftButton.MinSize = new Size(20, 20);
            this.rightButton.MinSize = new Size(20, 20);
         
            this.upButton = new RadRepeatScrollButtonElement(ScrollButtonDirection.Up);
            this.upButton.ZIndex = 10000;
            this.upButton.Class = "TabStripUpButton";
            this.upButton.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.downButton = new RadRepeatScrollButtonElement(ScrollButtonDirection.Down);
            this.downButton.ZIndex = 10000;
            this.downButton.Class = "TabStripDownButton";
            this.downButton.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
         
            this.upButton.Alignment = ContentAlignment.TopLeft;
            this.downButton.Alignment = ContentAlignment.TopLeft;
            this.upButton.MinSize = new Size(20, 20);
            this.downButton.MinSize = new Size(20, 20);

			this.leftButton.SetValue(TabLayoutPanel.IsLeftScrollButtonProperty, true);
			this.rightButton.SetValue(TabLayoutPanel.IsRightScrollButtonProperty, true);
			this.upButton.SetValue(TabLayoutPanel.IsUpScrollButtonProperty, true);
			this.downButton.SetValue(TabLayoutPanel.IsDownScrollButtonProperty, true);

			this.leftButton.Enabled = false;
			this.upButton.Enabled = false;

            scrollElementsParent.Children.Add(leftButton);
            scrollElementsParent.Children.Add(rightButton);
            scrollElementsParent.Children.Add(upButton);
            scrollElementsParent.Children.Add(downButton);

			if (scrollElementsParent as TabLayoutPanel != null)
			{
				TabLayoutPanel newLayout = scrollElementsParent as TabLayoutPanel;

				newLayout.LeftScrollButton = leftButton;
				newLayout.RightScrollButton = rightButton;
				newLayout.TopScrollButton = upButton;
				newLayout.BottomScrollButton = downButton;

			}
        }

        public void ScrollUp()
        {
			this.tabStripElement.EndEdit(true);

			this.scrollOffsetStep = tabStripElement.ScrollOffsetStep;

			if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
            {
				if (tabLayout.Size != tabLayoutSize)
					this.scrollingItemIndex = -1;

				int nextScrollingIndex = GetNextIndex(false, TabPositions.Left);

				if (nextScrollingIndex != this.scrollingItemIndex)
					this.scrollingItemIndex = nextScrollingIndex;

				if (scrollHeight.Count > 0)
				{
					this.scrollOffsetStep = scrollHeight.Pop();
					this.nextScrollIndx--;
				}
				else
				{
					upButton.Enabled = false;
					this.nextScrollIndx = 0;
				}
			}

            if (tabLayout.ScrollItemsOffset < 0)
            {
                if (!this.downButton.Enabled)
                    this.downButton.Enabled = true;
                tabLayout.ScrollItemsOffset += this.scrollOffsetStep;

                if (tabLayout.ScrollItemsOffset == 0)
                    this.upButton.Enabled = false;
            }
            else
                this.upButton.Enabled = false;
            
        }

    

        public void ScrollLeft()
        {

			scrollOffsetStep = tabStripElement.ScrollOffsetStep;

			this.tabStripElement.EndEdit(true);

			if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
			{
				if (tabLayout.Size != tabLayoutSize)
					this.scrollingItemIndex = -1;

				int nextScrollingIndex = GetNextIndex(false, TabPositions.Top);
			
				if (nextScrollingIndex != this.scrollingItemIndex)
					this.scrollingItemIndex = nextScrollingIndex;

		//		if (nextScrollingIndex == -1)
				{
		//			this.leftButton.Enabled = false;
				}
		//		else
				if (scrollWidth.Count > 0)
				{
					this.scrollOffsetStep = scrollWidth.Pop();
					if (this.tabLayout.ScrollItemsOffset == 0)
						nextScrollIndx = 0;

					nextScrollIndx--;
				}
				else
					this.leftButton.Enabled = false;

			}
	
			if (tabLayout.ScrollItemsOffset < 0)
			{
				if (!this.rightButton.Enabled)
					this.rightButton.Enabled = true;

				tabLayout.ScrollItemsOffset += this.scrollOffsetStep;
				if ( tabLayout.ScrollItemsOffset > tabLayout.ItemsOffset )
					tabLayout.ScrollItemsOffset = tabLayout.ItemsOffset;

				if (tabLayout.ScrollItemsOffset == 0)
					this.leftButton.Enabled = false;
			}
			else
				this.leftButton.Enabled = false;


        }

	

	/*	private int GetNextScrollingIndex(bool previous)
		{
			int newScrollingIndex = GetNextAlignedItemIndex(previous);

			if (newScrollingIndex == -1)
				newScrollingIndex = GePreviousAlignedItemIndex(previous);

			if (newScrollingIndex != -1)
				return newScrollingIndex;

			return this.scrollingItemIndex;
		}

		private int GetNextAlignedItemIndex(bool previous)
		{
			if (!previous)
			{
				for (int i = this.scrollingItemIndex + 1; i < this.tabStripElement.Items.Count - 1; i++)
				{
					bool isRightAligned = TelerikHelper.IsRightAligned(this.tabStripElement.Items[i].Alignment);

					if ((isRightAligned && previous) ||
						(!isRightAligned && !previous))
						return i;
				}
			}
			else
			{
				for (int i = this.tabStripElement.Items.Count - 1; i >= 0; i--)
				{
					bool isRightAligned = TelerikHelper.IsRightAligned(this.tabStripElement.Items[i].Alignment);
					if (this.tabStripElement.Items[i].Bounds.X >= 0)
					{
						if (isRightAligned && previous)
							return i;
					}
					
				}		
			}
			return -1;
		}

		private int GePreviousAlignedItemIndex(bool previous)
		{
			for (int i = this.tabStripElement.Items.Count - 1; i >= this.scrollingItemIndex + 1; i--)
			{
				bool isRightAligned = TelerikHelper.IsRightAligned(this.tabStripElement.Items[i].Alignment);

				if ((isRightAligned && !previous) ||
					(!isRightAligned && previous))
					return i;
			}
			return -1;
		}*/

		private RadItem GetRightMostItem()
		{
			foreach (RadItem item in this.tabStripElement.Items)
			{
				if (TelerikHelper.IsRightAligned(item.Alignment))
					return item;
			}
			return (RadItem) this.tabStripElement.Items[this.tabStripElement.Items.Count - 1];
		}

		private RadItem GetBottomMostItem()
		{
			foreach (RadItem item in this.tabStripElement.Items)
			{
				if (TelerikHelper.IsBottomAligned(item.Alignment))
					return item;
			}
			return (RadItem)this.tabStripElement.Items[this.tabStripElement.Items.Count - 1];
		
		}

		private int GetNextIndex( bool rightBottom, TabPositions tabPosition )
		{
			int itemIndex = -1;
			int minPositiveBounds = int.MaxValue;
			int maxNegativeBounds = int.MinValue;

			if ((tabPosition == TabPositions.Top) || (tabPosition == TabPositions.Bottom))
			{
				if (rightBottom)
				{
					for (int i = 0; i < this.tabStripElement.Items.Count; i++)
					{
						if (this.tabStripElement.Items[i].BoundingRectangle.X >= 0)
							if (minPositiveBounds >= this.tabStripElement.Items[i].BoundingRectangle.X)
							{
								minPositiveBounds = this.tabStripElement.Items[i].BoundingRectangle.X;
								itemIndex = i;
							}
					}
				}
				else
				{
					for (int i = 0; i < this.tabStripElement.Items.Count; i++)
					{
						if (this.tabStripElement.Items[i].BoundingRectangle.X <= 0)
							if (maxNegativeBounds <= this.tabStripElement.Items[i].BoundingRectangle.X)
							{
								maxNegativeBounds = this.tabStripElement.Items[i].BoundingRectangle.X;
								itemIndex = i;
							}
					}
				}
			}
			else
			{
				if (rightBottom)
				{
					for (int i = 0; i < this.tabStripElement.Items.Count; i++)
					{
						if (this.tabStripElement.Items[i].BoundingRectangle.Top >= this.tabLayout.ItemsOverlapFactor)
							if (minPositiveBounds >= this.tabStripElement.Items[i].BoundingRectangle.Top)
							{
								minPositiveBounds = this.tabStripElement.Items[i].BoundingRectangle.Top;
								itemIndex = i;
							}
					}
				}
				else
				{
					for (int i = 0; i < this.tabStripElement.Items.Count; i++)
					{
						if (this.tabStripElement.Items[i].BoundingRectangle.X <= this.tabLayout.ItemsOverlapFactor)
							if (maxNegativeBounds <= this.tabStripElement.Items[i].BoundingRectangle.Top)
							{
								maxNegativeBounds = this.tabStripElement.Items[i].BoundingRectangle.Top;
								itemIndex = i;
							}
					}
				}
			}
			return itemIndex;
		}

		public void ScrollDown()
		{
			this.tabStripElement.EndEdit(true);

			this.scrollOffsetStep = tabStripElement.ScrollOffsetStep;

			int scrollButtonsHeight = this.leftButton.Size.Height + this.rightButton.Size.Height;

			if (!this.tabStripElement.UseNewLayoutSystem)
			{
				int parentHeight = this.tabStripElement.Parent.FullSize.Height - scrollButtonsHeight +
					Math.Abs(tabLayout.ScrollItemsOffset);

				if (tabLayout.Size != tabLayoutSize)
					this.scrollingItemIndex = -1;

				if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
				{
					int nextScrollingIndex = GetNextIndex(true, TabPositions.Left);

					if (nextScrollingIndex != this.scrollingItemIndex)
						this.scrollingItemIndex = nextScrollingIndex;

					if (this.tabStripElement.TabsPosition == TabPositions.Left)
						this.scrollOffsetStep = this.tabStripElement.Items[scrollingItemIndex].Bounds.Top
							+ this.tabStripElement.Items[scrollingItemIndex].Bounds.Right;
					else
						this.scrollOffsetStep = this.tabStripElement.Items[scrollingItemIndex].Bounds.Top
											+ this.tabStripElement.Items[scrollingItemIndex].Bounds.Width;

					this.scrollHeight.Push(this.scrollOffsetStep);

				}

				if ((tabLayout.ScrollItemsOffset == 0) || (tabLayoutSize != tabLayout.Size) || this.tabStripElement.GetBitState(RadTabStripElement.OffsetScrollingStateKey))
				{
					lastItem = (TabItem)GetBottomMostItem();
					lastItemHeight = Math.Abs(this.tabLayout.ScrollItemsOffset) + lastItem.Bounds.Y + lastItem.FullSize.Width;
					tabLayoutSize = this.tabLayout.Size;
					this.tabStripElement.BitState[RadTabStripElement.OffsetScrollingStateKey] = false;
				}

				if (lastItemHeight > parentHeight)
				{
					if (!this.upButton.Enabled)
						this.upButton.Enabled = true;

					tabLayout.ScrollItemsOffset -= this.scrollOffsetStep;

					if (lastItemHeight <= parentHeight + this.scrollOffsetStep)
						this.downButton.Enabled = false;
				}
				else
					this.downButton.Enabled = false;
			}
			else
			{
				if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
				{
					if (this.tabLayout.ScrollItemsOffset == 0)
						this.nextScrollIndx = 0;

					int nextScrollingIndex = this.nextScrollIndx;

					if (this.tabStripElement.RightToLeft)
					{
						nextScrollingIndex = this.tabStripElement.Items.Count -1  - nextScrollingIndex;
					}

					if (nextScrollingIndex != this.scrollingItemIndex)
						this.scrollingItemIndex = nextScrollingIndex;

					this.scrollOffsetStep = (int)this.tabStripElement.Items[scrollingItemIndex].DesiredSize.Height;
					this.scrollHeight.Push(this.scrollOffsetStep);

					this.nextScrollIndx++;
				}
           //     if (tabLayout.BoundingRectangle.Bottom - scrollOffsetStep + tabLayout.ScrollItemsOffset - scrollButtonsHeight < tabStripElement.BoundingRectangle.Height)
             //       this.downButton.Enabled = false;

           
                if (tabStripElement.DesiredSize.Height > this.tabStripElement.BoxLayout.DesiredSize.Height + tabLayout.ScrollItemsOffset)
                {
                    this.downButton.Enabled = false;
                }

			//	if ((tabLayout.BoundingRectangle.Bottom + Math.Abs(tabLayout.ScrollItemsOffset) - scrollButtonsHeight) > tabStripElement.BoxLayout.DesiredSize.Height)
			//		this.downButton.Enabled = false;

				if (!this.upButton.Enabled)
					this.upButton.Enabled = true;

				tabLayout.ScrollItemsOffset -= this.scrollOffsetStep;
			}
		}

	

		internal void PerformScroll(TabItem item, int lastIndex, int index)
		{
			if (this.tabStripElement.ElementTree != null)
			{				
				if (this.tabStripElement.TabsPosition == TabPositions.Top ||
					this.tabStripElement.TabsPosition == TabPositions.Bottom)
				{

					if (lastIndex < index)
					{
						if ( this.tabStripElement.Items[index].ControlBoundingRectangle.Right > this.tabStripElement.ControlBoundingRectangle.Right ) 
						ScrollRight();
					}
					else
						if (this.tabStripElement.Items[index].ControlBoundingRectangle.Left <= 0)
							ScrollLeft();
				}
				else
				{
					if (lastIndex < index)
					{
						if (this.tabStripElement.Items[index].ControlBoundingRectangle.Bottom > this.tabStripElement.ControlBoundingRectangle.Bottom) 				
						ScrollDown();
					}
					else
						if (this.tabStripElement.Items[index].ControlBoundingRectangle.Top <= 0)
							ScrollUp();
				}
			}
		}

		private int nextScrollIndx;

      	public void ScrollRight()
		{
			this.tabStripElement.EndEdit(true);

			this.scrollOffsetStep = this.tabStripElement.ScrollOffsetStep;

			int scrollButtonsWidth = this.leftButton.Size.Width + this.rightButton.Size.Width;

			int width = tabLayout.Size.Width + scrollButtonsWidth + tabLayout.ItemsOffset;

			if (!this.tabStripElement.UseNewLayoutSystem)
			{
				int parentWidth = this.tabStripElement.Parent.Size.Width - scrollButtonsWidth +
					Math.Abs(tabLayout.ScrollItemsOffset);

				if (tabLayout.Size != tabLayoutSize)
					this.scrollingItemIndex = -1;

				if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
				{
					int nextScrollingIndex = GetNextIndex(true, TabPositions.Top);

					if (nextScrollingIndex != this.scrollingItemIndex)
						this.scrollingItemIndex = nextScrollingIndex;

					this.scrollOffsetStep = this.tabStripElement.Items[scrollingItemIndex].Bounds.Right;
					this.scrollWidth.Push(this.scrollOffsetStep);
				}

				if ((tabLayout.ScrollItemsOffset == 0) || (tabLayoutSize != tabLayout.Size) || this.tabStripElement.GetBitState(RadTabStripElement.OffsetScrollingStateKey))
				{
					lastItem = (TabItem)GetRightMostItem();
					lastItemWidth = Math.Abs(tabLayout.ScrollItemsOffset) + lastItem.Bounds.Right + lastItem.Margin.Size.Width;
					tabLayoutSize = this.tabLayout.Size;
                    this.tabStripElement.BitState[RadTabStripElement.OffsetScrollingStateKey] = false;
				}

				if (lastItemWidth > parentWidth)
				{
					if (!this.leftButton.Enabled)
						this.leftButton.Enabled = true;

					tabLayout.ScrollItemsOffset -= this.scrollOffsetStep;

					if (lastItemWidth <= parentWidth + this.scrollOffsetStep)
						this.rightButton.Enabled = false;
				}
				else
					this.rightButton.Enabled = false;
			}
			else
			{
				if (this.tabStripElement.TabScrollStyle == TabStripScrollStyle.ScrollByItem)
				{
					if (this.tabLayout.ScrollItemsOffset == 0)
						nextScrollIndx = 0;

					int nextScrollingIndex = nextScrollIndx;// GetNextIndex(true, TabPositions.Top);

					if (this.tabStripElement.RightToLeft)
					{
						nextScrollingIndex = this.tabStripElement.Items.Count -1 - nextScrollingIndex;
					}

					if (nextScrollingIndex != this.scrollingItemIndex)
						this.scrollingItemIndex = nextScrollingIndex;

					this.scrollOffsetStep = (int)this.tabStripElement.Items[scrollingItemIndex].DesiredSize.Width - this.tabLayout.ItemsOverlapFactor;
					this.scrollWidth.Push(this.scrollOffsetStep);
					nextScrollIndx++;	
				}

            

                if (tabStripElement.DesiredSize.Width > this.tabStripElement.BoxLayout.DesiredSize.Width + tabLayout.ScrollItemsOffset)
                {
                    this.rightButton.Enabled = false;
                }

            //    if (tabLayout.BoundingRectangle.Right -scrollOffsetStep + tabLayout.ScrollItemsOffset - scrollButtonsWidth < tabStripElement.BoundingRectangle.Width)
			//		this.rightButton.Enabled = false;

				if (!this.leftButton.Enabled)
					this.leftButton.Enabled = true;

					tabLayout.ScrollItemsOffset -= this.scrollOffsetStep;

				
			}
		}
	}
}