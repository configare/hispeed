using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public abstract class Filter
    {
        #region Overridables

        public abstract bool Match(object obj);

        #endregion
    }
}
