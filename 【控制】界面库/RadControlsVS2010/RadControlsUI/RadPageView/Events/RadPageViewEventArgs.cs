using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewEventArgs : EventArgs
    {
        #region Fields

        private RadPageViewPage page;

        #endregion

        #region Constructor

        public RadPageViewEventArgs(RadPageViewPage page)
        {
            this.page = page;
        }

        #endregion

        #region Properties

        public RadPageViewPage Page
        {
            get
            {
                return this.page;
            }
        }

        #endregion
    }
}
