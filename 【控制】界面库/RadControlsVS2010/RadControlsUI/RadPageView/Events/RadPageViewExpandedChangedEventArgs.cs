using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewExpandedChangedEventArgs: EventArgs
    {
        #region Fields

        private RadPageViewExplorerBarItem item;

        #endregion

        #region Constructor

        public RadPageViewExpandedChangedEventArgs(RadPageViewExplorerBarItem item)
        {
            this.item = item;
        }

        #endregion

        #region Properties

        public RadPageViewExplorerBarItem Item
        {
            get
            {
                return this.item;
            }
        }

        public bool Expanded
        {
            get
            {
                return this.item.IsExpanded;
            }
        }

        #endregion
    }
}
