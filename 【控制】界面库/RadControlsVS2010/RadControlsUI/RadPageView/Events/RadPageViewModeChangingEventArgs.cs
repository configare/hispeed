using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewModeChangingEventArgs : RadPageViewModeEventArgs
    {
        #region Fields

        private bool cancel;

        #endregion

        #region Constructor

        public RadPageViewModeChangingEventArgs(PageViewMode mode)
            : base(mode)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the event may continue or it should be canceled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        #endregion
    }
}
