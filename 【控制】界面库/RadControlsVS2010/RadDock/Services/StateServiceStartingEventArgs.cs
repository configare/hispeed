using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines an entry point for methods that can handle events like <see cref="StateService.Starting">StateService.Starting</see> one.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StateServiceStartingEventHandler(object sender, StateServiceStartingEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="StateService.Starting">StateService.Starting</see> event.
    /// </summary>
    public class StateServiceStartingEventArgs : CancelEventArgs
    {
        #region Fields

        private object context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StateServiceStartingEventArgs">StateServiceStartingEventArgs</see> class.
        /// </summary>
        /// <param name="context">The context that is passed prior to the Start request.</param>
        public StateServiceStartingEventArgs(object context)
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
