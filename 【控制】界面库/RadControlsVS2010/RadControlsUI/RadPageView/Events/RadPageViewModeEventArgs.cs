using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewModeEventArgs : EventArgs
    {
        #region Fields

        private PageViewMode viewMode;

        #endregion

        #region Constructor

        public RadPageViewModeEventArgs(PageViewMode mode)
        {
            this.viewMode = mode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the view mode associated with the event.
        /// </summary>
        public PageViewMode ViewMode
        {
            get
            {
                return this.viewMode;
            }
        }

        #endregion
    }
}
