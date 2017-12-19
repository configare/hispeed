using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public interface IShortcutProvider
    {
        RadShortcutCollection Shortcuts
        {
            get;
        }
        /// <summary>
        /// Occurs when the complete keyboard combination for a registered RadShortcut is triggerred.
        /// </summary>
        /// <param name="e"></param>
        void OnShortcut(ShortcutEventArgs e);
        /// <summary>
        /// Occurs when a registered shortcut's keyboard combination is partially complete.
        /// E.g. if we have Ctrl+C+V and Ctrl+C is pressed the event will be raised.
        /// </summary>
        /// <param name="e"></param>
        void OnPartialShortcut(PartialShortcutEventArgs e);
        void OnShortcutsChanged();
    }
}
