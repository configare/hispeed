using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents event data when RadService is stopping.
    /// </summary>
    public class RadServiceStoppingEventArgs : CancelEventArgs
    {
        #region Fields

        private bool commit;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RadServiceStoppingEventArgs">RadServiceStoppingEventArgs</see> class.
        /// </summary>
        /// <param name="commit"></param>
        public RadServiceStoppingEventArgs(bool commit)
        {
            this.commit = commit;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Commit parameter of the Stop request.
        /// </summary>
        public bool Commit
        {
            get
            {
                return this.commit;
            }
            set
            {
                this.commit = value;
            }
        }

        #endregion
    }
}
