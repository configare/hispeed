using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    internal class ExplorerBarItemSizeInfo : PageViewItemSizeInfo
    {
        #region Fields

        public bool IsExpanded;
        public SizeF contentSize;

        #endregion

        #region Ctor

        public ExplorerBarItemSizeInfo(RadPageViewExplorerBarItem item, bool vertical, bool isExpanded)
            : base(item, vertical)
        {
            this.IsExpanded = isExpanded;
        }

        #endregion
    }
}
