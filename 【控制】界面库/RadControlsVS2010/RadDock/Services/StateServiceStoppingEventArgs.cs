using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines an entry point for methods that can handle events like <see cref="StateService.Stopping">StateService.Stopping</see> one.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StateServiceStoppingEventHandler(object sender, StateServiceStoppingEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="StateService.Stopping">StateService.Stopping</see> event.
    /// </summary>
    public class StateServiceStoppingEventArgs : CancelEventArgs
    {
        #region Fields

        private bool commit;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StateServiceStoppingEventArgs">StateServiceStoppingEventArgs</see> class.
        /// </summary>
        /// <param name="commit"></param>
        public StateServiceStoppingEventArgs(bool commit)
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
