using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines a method signature to handle <see cref="RadDock.FloatingWindowCreated">FloatingWindowCreated</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FloatingWindowEventHandler(object sender, FloatingWindowEventArgs e);

    /// <summary>
    /// Encapsulates the data, associated with the <see cref="RadDock.FloatingWindowCreated">FloatingWindowCreated</see> event.
    /// </summary>
    public class FloatingWindowEventArgs : EventArgs
    {
        #region Fields

        private FloatingWindow window;

        #endregion

        #region Constructor

        public FloatingWindowEventArgs(FloatingWindow instance)
        {
            this.window = instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the FloatingWindow instance to be used.
        /// </summary>
        public FloatingWindow Window
        {
            get
            {
                return this.window;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FloatingWindow");
                }
                this.window = value;
            }
        }

        #endregion
    }
}
