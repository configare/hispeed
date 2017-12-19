using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Encapsulates the data, associated with the IShortcutProvider.OnShortcut callback.
    /// </summary>
    public class ShortcutEventArgs : EventArgs
    {
        #region Fields

        private Control focusedControl;
        private RadShortcut shortcut;
        private bool handled;

        #endregion

        #region Constructor

        public ShortcutEventArgs(Control focused, RadShortcut shortcut)
        {
            this.focusedControl = focused;
            this.shortcut = shortcut;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the control that is currently focused and which will receive the keyboard event.
        /// </summary>
        public Control FocusedControl
        {
            get
            {
                return this.focusedControl;
            }
        }

        /// <summary>
        /// Gets the shortcut that is triggerred.
        /// </summary>
        public RadShortcut Shortcut
        {
            get
            {
                return this.shortcut;
            }
        }

        /// <summary>
        /// Determines whether the event is handled. If true, the keyboard message will not be dispatched to the focused control.
        /// </summary>
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }

        #endregion
    }
}
