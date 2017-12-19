using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewIndexChangedEventArgs : RadPageViewEventArgs
    {
        #region Fields

        private int oldIndex;
        private int newIndex;

        #endregion

        #region Constructor

        public RadPageViewIndexChangedEventArgs(RadPageViewPage page, int oldIndex, int newIndex)
            : base(page)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index at which the page was before the change.
        /// </summary>
        public int OldIndex
        {
            get
            {
                return this.oldIndex;
            }
        }

        /// <summary>
        /// Gets the index at which the page is currently at.
        /// </summary>
        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
        }

        #endregion
    }
}
