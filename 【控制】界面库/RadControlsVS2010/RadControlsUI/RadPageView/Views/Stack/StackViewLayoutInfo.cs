using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    internal class StackViewLayoutInfo : PageViewLayoutInfo
    {
        #region Fields

        public StackViewItemSelectionMode selectionMode;
        public StackViewPosition position;
        public PageViewItemSizeInfo selectedItem;

        #endregion

        #region Ctor

        public StackViewLayoutInfo(RadPageViewStackElement layout, SizeF availableSize)
            : base(layout, availableSize)
        {
        }

        #endregion

        #region Methods

        public override void Update()
        {
            base.Update();

            this.selectionMode = (StackViewItemSelectionMode)this.itemLayout.GetValue(RadPageViewStackElement.ItemSelectionModeProperty);
            this.position = (StackViewPosition)this.itemLayout.GetValue(RadPageViewStackElement.StackPositionProperty);
        }

        public override PageViewItemSizeInfo CreateItemSizeInfo(RadPageViewItem item)
        {
            RadPageViewItem selectedItem = (this.itemLayout as RadPageViewElement).SelectedItem;
            PageViewItemSizeInfo info = base.CreateItemSizeInfo(item);

            if (object.ReferenceEquals(selectedItem, item))
            {
                this.selectedItem = info;
            }

            return info;
        }

        public override SizeF GetMeasureSizeConstraint(RadItem item)
        {
            float marginLength = this.vertical ? item.Margin.Horizontal : item.Margin.Vertical;
            float availableSpace = this.vertical ? this.availableSize.Width : this.availableSize.Height;
            float horizontalSpace = availableSpace - marginLength;
            return new SizeF(horizontalSpace, float.PositiveInfinity);
        }

        public override bool GetIsVertical()
        {
            RadPageViewStackElement stackElement = this.itemLayout as RadPageViewStackElement;
            StackViewPosition stackPosition = stackElement.StackPosition;

            return (stackPosition == StackViewPosition.Top || stackPosition == StackViewPosition.Bottom);
        }

        #endregion
    }
}
