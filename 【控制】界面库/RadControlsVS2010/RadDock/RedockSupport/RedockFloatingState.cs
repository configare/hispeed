using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A predefined <see cref="RedockState">RedockState</see>, used by a <see cref="RedockService">RedockService</see> to restore a floating state.
    /// </summary>
    public class RedockFloatingState : RedockState
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedockFloatingState">RedockFloatingState</see> class.
        /// </summary>
        /// <param name="window"></param>
        public RedockFloatingState(DockWindow window)
            : base(window, DockState.Floating)
        {
            Form form = window.FindForm();
            if (form is FloatingWindow)
            {
                this.floatingWindow = new WeakReference(form);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (this.FloatingWindow == null)
                {
                    return false;
                }

                return base.IsValid;
            }
        }

        /// <summary>
        /// Gets the floating window, target of the redock operation.
        /// </summary>
        public FloatingWindow FloatingWindow
        {
            get
            {
                if (this.floatingWindow == null || !this.floatingWindow.IsAlive)
                {
                    return null;
                }

                FloatingWindow window = (FloatingWindow)this.floatingWindow.Target;
                if (!(window.IsDisposed || window.Disposing))
                {
                    return window;
                }

                return null;
            }
        }

        #endregion

        #region Fields

        private WeakReference floatingWindow;

        #endregion
    }
}
