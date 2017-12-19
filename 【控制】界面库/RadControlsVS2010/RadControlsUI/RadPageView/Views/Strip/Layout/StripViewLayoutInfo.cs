using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    internal class StripViewLayoutInfo : PageViewLayoutInfo
    {
        public StripViewLayoutInfo(LightVisualElement layout, SizeF available)
            : base(layout, available)
        {

        }

        public override void Update()
        {
            //cache properties that will be used during the layout pass
            this.fitMode = (StripViewItemFitMode)this.itemLayout.GetValue(RadPageViewStripElement.ItemFitModeProperty);
            this.align = (StripViewAlignment)this.itemLayout.GetValue(RadPageViewStripElement.StripAlignmentProperty);
            this.itemAlign = (StripViewItemAlignment)this.itemLayout.GetValue(RadPageViewStripElement.ItemAlignmentProperty);
            base.Update();
        }

        public override PageViewItemSizeInfo CreateItemSizeInfo(RadPageViewItem item)
        {
            if (!item.IsSystemItem)
            {
                this.nonSystemItemCount++;
            }

            return base.CreateItemSizeInfo(item);
        }

        public override bool GetIsVertical()
        {
            return this.align == StripViewAlignment.Left || this.align == StripViewAlignment.Right;
        }

        public StripViewAlignment align;
        public StripViewItemAlignment itemAlign;
        public StripViewItemFitMode fitMode;
        public int nonSystemItemCount;
    }
}
