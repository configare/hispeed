using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class IconListViewScroller : ItemScroller<ListViewDataItem>
    {
        public override void UpdateScrollRange()
        {
            if (this.Scrollbar != null)
            {
                if (this.Traverser != null)
                {
                    if (this.Scrollbar.ScrollType == ScrollType.Vertical)
                    {
                        UpdateVerticalScrollRange();
                    }
                    else
                    {
                        UpdateHorizontalScrollRange();
                    }
                }
            }
        }

        private void UpdateHorizontalScrollRange()
        {
            int count = 0;
            int x = 0, y = 0;

            object oldPosition = this.Traverser.Position;

            this.Traverser.Reset();
            int maxItemWidth = 0;
            ListViewDataItem previous = this.traverser.Current;

            while (this.Traverser.MoveNext())
            {
                int itemWidth = this.GetScrollHeight(this.Traverser.Current) + this.ItemSpacing;
                int itemHeight = this.currentItemWidth + this.ItemSpacing;

                if (y + itemHeight > this.ClientSize.Height - this.scrollbar.DesiredSize.Height
                    || this.traverser.Current is ListViewDataItemGroup)
                {
                    if (previous != null)
                    {
                        previous.IsLastInRow = true;
                    }
                    y = 0;
                    x += maxItemWidth;
                    maxItemWidth = 0;
                }
                else if (previous != null)
                {
                    previous.IsLastInRow = (previous is ListViewDataItemGroup);
                }

                y += itemHeight;
                count++;
                previous = this.traverser.Current;
                maxItemWidth = Math.Max(maxItemWidth, itemWidth);
            }

            if (previous != null)
            {
                previous.IsLastInRow = true;
            }

            if (y != 0)
            {
                x += maxItemWidth;
            }

            this.traverser.Position = oldPosition;

            if (x < 0)
            {
                return;
            }

            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
            {
                if (this.scrollbar.Maximum != x)
                {
                    this.scrollbar.Maximum = x;
                    this.SetScrollBarVisibility();
                    this.UpdateScrollValue();
                }
            }
            else
            {
                if (this.scrollbar.Maximum != count - 1 && count - 1 > 0)
                {
                    this.scrollbar.Maximum = count - 1;
                    this.UpdateScrollStep();
                    this.SetScrollBarVisibility();
                }
            }
        }

        private void UpdateVerticalScrollRange()
        {
            int count = 0;
            int x = 0, y = 0;
            object oldPosition = this.Traverser.Position;
            this.Traverser.Reset();
            int maxItemHeight = 0;
            ListViewDataItem previous = this.traverser.Current;

            while (this.Traverser.MoveNext())
            {
                int itemHeight = this.GetScrollHeight(this.Traverser.Current) + this.ItemSpacing;
                this.currentItemWidth += this.ItemSpacing;

                if (x + this.currentItemWidth > this.ClientSize.Width - this.scrollbar.DesiredSize.Width
                    || this.traverser.Current is ListViewDataItemGroup)
                {
                    if (previous != null)
                    {
                        previous.IsLastInRow = true;
                    }
                    x = 0;
                    y += maxItemHeight;
                    maxItemHeight = 0;
                }
                else if (previous != null)
                {
                    previous.IsLastInRow = (previous is ListViewDataItemGroup);
                }

                x += this.currentItemWidth;
                count++;
                previous = this.traverser.Current;
                maxItemHeight = Math.Max(maxItemHeight, itemHeight);
            }

            if (previous != null)
            {
                previous.IsLastInRow = true;
            }

            if (x != 0)
            {
                y += maxItemHeight;
            }

            this.traverser.Position = oldPosition;

            if (y < 0)
            {
                return;
            }

            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
            {
                if (this.scrollbar.Maximum != y)
                {
                    this.scrollbar.Maximum = y;
                    this.SetScrollBarVisibility();
                    this.UpdateScrollValue();
                }
            }
            else
            {
                if (this.scrollbar.Maximum != count - 1 && count - 1 > 0)
                {
                    this.scrollbar.Maximum = count - 1;
                    this.UpdateScrollStep();
                    this.SetScrollBarVisibility();
                }
            }
        }

        protected override bool ScrollDown(int step)
        {
            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
            { 
                while (step > 0)
                {
                    int height = 0;
                    object oldPos = this.traverser.Position;
                    object newPos = null;
                    bool found = false;
                    while (traverser.MoveNext())
                    {
                        height = Math.Max(height, this.GetScrollHeight(this.traverser.Current) + ItemSpacing);
                        if (traverser.Current == null || traverser.Current.IsLastInRow)
                        {
                            found = traverser.Current != null;
                            break;
                        }
                    }

                    if (found)
                    {
                        newPos = this.traverser.Position;
                        this.traverser.Position = oldPos; 
                    }
                    else 
                    {
                        this.traverser.Position = oldPos;
                        return true;
                    }

                    int diff = height - this.scrollOffset;

                    if (diff > step)
                    {
                        this.scrollOffset += step;
                        break;
                    }
                    else
                    {
                        step -= diff;
                        this.scrollOffset = 0;
                        this.traverser.Position = newPos;
                    }

                    height = 0;
                } 

                return true;
            }

            while (step-- > 0)
            {
                this.traverser.MoveNext();
            }

            return true;
        }

        protected override bool ScrollUp(int step)
        {
            
            if (this.ScrollMode == ItemScrollerScrollModes.Smooth || this.ScrollMode == ItemScrollerScrollModes.Deferred)
            {
                while (step > 0)
                {
                    if (this.scrollOffset > step)
                    {
                        this.scrollOffset -= step; 
                        break;
                    }

                    int height = this.traverser.Current!=null ? this.GetScrollHeight(this.traverser.Current) + ItemSpacing : 0;

                    while (traverser.MovePrevious())
                    {
                        if (traverser.Current == null || traverser.Current.IsLastInRow)
                            break;

                        height = Math.Max(height, this.GetScrollHeight(this.traverser.Current) + ItemSpacing);
                    }

                    if(this.scrollOffset == 0 && height ==0)
                    {
                        return false;
                    }

                    step -= this.scrollOffset;
                    this.scrollOffset = height;
                    

                    height = 0;
                }

                return true;
            }

            while (step-- > 0)
            {
                this.traverser.MovePrevious();
            }

            return true;
        }
    }
}
