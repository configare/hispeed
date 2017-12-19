using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewIndexChangingEventArgs : RadPageViewCancelEventArgs
    {
        #region Fields

        private int newIndex;
        private int currentIndex;

        #endregion

        #region Constructor

        public RadPageViewIndexChangingEventArgs(RadPageViewPage page, int newIndex, int currentIndex)
            : base(page)
        {
            this.newIndex = newIndex;
            this.currentIndex = currentIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index the page is currently at.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return this.currentIndex;
            }
        }

        /// <summary>
        /// Gets or sets the new index to be applied to the associated page.
        /// </summary>
        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
            set
            {
                this.newIndex = value;
            }
        }

        #endregion
    }
}
