using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewCancelEventArgs : RadPageViewEventArgs
    {
        #region Fields

        private bool cancel;

        #endregion

        #region Constructor

        public RadPageViewCancelEventArgs(RadPageViewPage page)
            : base(page)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the event is canceled or may continue.
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
