using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public class PartialShortcutEventArgs : ShortcutEventArgs
    {
        #region Fields

        private Keys[] collectedKeys;

        #endregion

        #region Constructor

        public PartialShortcutEventArgs(Control focused, RadShortcut shortcut, Keys[] collectedKeys)
            : base(focused, shortcut)
        {
            this.collectedKeys = collectedKeys;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an array with the currently collected key strokes.
        /// </summary>
        public Keys[] CollectedKeys
        {
            get
            {
                return this.collectedKeys;
            }
        }

        #endregion
    }
}
