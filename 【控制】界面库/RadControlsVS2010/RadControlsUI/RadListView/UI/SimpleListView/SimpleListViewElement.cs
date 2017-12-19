using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class SimpleListViewElement : BaseListViewElement
    {
        #region Ctor

        public SimpleListViewElement(RadListViewElement owner)
            : base(owner)
        {

        }

        #endregion

        #region Overrides

        protected override VirtualizedStackContainer<ListViewDataItem> CreateViewElement()
        {
            return new SimpleListViewContainer(this);
        }

        internal override void UpdateHScrollbarMaximum()
        {
            int newMaximum = 0;
            ITraverser<ListViewDataItem> enumerator = (ITraverser<ListViewDataItem>)this.Scroller.Traverser.GetEnumerator();
            enumerator.Reset();

            bool calcIndent = this.owner.ShowGroups && this.owner.Groups.Count > 0 &&
                (this.owner.EnableGrouping || this.owner.EnableCustomGrouping) && !this.FullRowSelect;

            while (enumerator.MoveNext())
            {
                newMaximum = Math.Max(
                    enumerator.Current.ActualSize.Width +
                    ((!(enumerator.Current is ListViewDataItemGroup) && calcIndent) ? this.GroupIndent : 0),
                    newMaximum);
            }

            if (this.HScrollBar.Maximum != newMaximum)
            {
                if (this.HScrollBar.Value + this.HScrollBar.LargeChange - 1 > newMaximum)
                {
                    this.HScrollBar.Maximum = newMaximum;
                    this.HScrollBar.Value = newMaximum - this.HScrollBar.LargeChange + 1;
                }
                else
                {
                    this.HScrollBar.Maximum = newMaximum;
                }
            }

            UpdateHScrollbarVisibility();
        }

        protected override void UpdateHScrollbarVisibility()
        {
            this.HScrollBar.Visibility =
                this.HScrollBar.LargeChange < this.HScrollBar.Maximum ?
                ElementVisibility.Visible : ElementVisibility.Collapsed;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BoundsProperty)
            {
                if (this.HScrollBar.Value > this.HScrollBar.Maximum - this.HScrollBar.LargeChange + 1)
                {
                    this.HScrollBar.Value = this.HScrollBar.Maximum - this.HScrollBar.LargeChange + 1;
                }
            }
        }

        protected override void EnsureItemVisibleHorizontal(ListViewDataItem item)
        {
            RadElement element = this.GetElement(item);

            if (element != null)
            {
                this.HScrollBar.Value = element.Bounds.Left;
            }
        }

        #endregion
    }
}
