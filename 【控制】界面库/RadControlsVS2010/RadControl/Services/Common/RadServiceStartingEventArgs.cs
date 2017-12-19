using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents event data when RadService is starting.
    /// </summary>
    public class RadServiceStartingEventArgs : CancelEventArgs
    {
        #region Fields

        private object context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RadServiceStartingEventArgs">RadServiceStartingEventArgs</see> class.
        /// </summary>
        /// <param name="context">The context that is passed prior to the Start request.</param>
        public RadServiceStartingEventArgs(object context)
        {
            this.context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Context, passed to the service as a start parameter.
        /// </summary>
        public object Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion
    }
}
