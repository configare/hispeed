using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    internal class StripViewItemExpandStrategy
    {
        #region Nested Types

        private class ExpandInfo
        {
            public float layoutLength;
        }

        #endregion

        #region Fields

        private StripViewLayoutInfo layoutInfo;
        private ExpandInfo expandInfo;

        #endregion

        #region Constructor

        public StripViewItemExpandStrategy(StripViewLayoutInfo info)
        {
            this.layoutInfo = info;
            this.expandInfo = new ExpandInfo();
        }

        #endregion

        #region Methods

        public void Execute()
        {
            this.UpdateExpandInfo();

            if (this.expandInfo.layoutLength >= this.layoutInfo.availableLength)
            {
                return;
            }

            int expand = (int)(this.layoutInfo.availableLength - this.expandInfo.layoutLength);
            if (expand < this.layoutInfo.itemCount)
            {
                return;
            }

            int modulus = expand % this.layoutInfo.itemCount;
            int singleExpand = (expand - modulus) / this.layoutInfo.itemCount;

            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                item.ChangeLayoutLength((int)item.layoutLength + singleExpand);
            }
        }

        private void UpdateExpandInfo()
        {
            foreach (PageViewItemSizeInfo item in this.layoutInfo.items)
            {
                this.expandInfo.layoutLength += item.layoutLength + item.marginLength;
            }

            this.expandInfo.layoutLength += this.layoutInfo.paddingLength;
            this.expandInfo.layoutLength += (this.layoutInfo.itemCount - 1) * this.layoutInfo.itemSpacing;
        }

        #endregion
    }
}
