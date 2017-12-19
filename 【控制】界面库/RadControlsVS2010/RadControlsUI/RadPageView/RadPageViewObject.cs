using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class RadPageViewObject : DisposableObject
    {
        #region Fields

        private RadPageView owner;

        #endregion

        #region Poperties

        /// <summary>
        /// Gets the <see cref="RadPageView"/> instance that this object is associated with.
        /// </summary>
        public RadPageView Owner
        {
            get
            {
                return this.owner;
            }
            protected set
            {
                this.owner = value;
            }
        }

        #endregion
    }
}
