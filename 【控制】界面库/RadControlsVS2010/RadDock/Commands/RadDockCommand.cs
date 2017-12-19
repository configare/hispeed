using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents base command that is associated with a RadDock instance.
    /// </summary>
    public abstract class RadDockCommand : CommandBase
    {
        #region Fields

        private List<RadShortcut> shortcuts;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RadDockCommand()
        {
            this.shortcuts = new List<RadShortcut>();
        }

        #endregion

        #region Override

        /// <summary>
        /// Determines whether the command may execute.
        /// The command may be executed in the following cases:
        /// - The currently active form is the one where the associated RadDock resides.
        /// - The currently active form is a FloatingWindow instance, owned by the associated RadDock.
        /// - The currently active form is an AutoHidePopup instance, owned by the associated RadDock.
        /// </summary>
        /// <param name="parameter">The additional parameter provided. Should be a RadDock instance.</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            RadDock dock = parameter as RadDock;
            if (dock == null)
            {
                return false;
            }

            Form dockParentForm = dock.FindForm();
            Form activeForm = Form.ActiveForm;

            if (activeForm == dockParentForm)
            {
                return true;
            }

            //check from floating windows or auto-hide popup, keeping in mind that we may have more than one RadDock on a Form.
            FloatingWindow floatingWindow = activeForm as FloatingWindow;
            if (floatingWindow != null && floatingWindow.DockManager == dock)
            {
                return true;
            }

            AutoHidePopup autoHidePopup = activeForm as AutoHidePopup;
            if (autoHidePopup != null && autoHidePopup.DockManager == dock)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the keyboard combination is valid for any of the registered <see cref="RadShortcut">Shortcut</see> instance.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public bool IsShortcut(Keys modifiers, params Keys[] mappings)
        {
            if (this.shortcuts.Count == 0)
            {
                return false;
            }

            bool match = true;
            foreach (RadShortcut shortcut in this.shortcuts)
            {
                if (!shortcut.IsShortcutCombination(modifiers, mappings))
                {
                    match = false;
                    break;
                }
            }

            return match;
        }

        /// <summary>
        /// Determines whether the specified key is a mapping for any of the associated shortcuts.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsMappingKey(Keys key)
        {
            if (this.shortcuts.Count == 0)
            {
                return false;
            }

            bool mapping = true;
            foreach (RadShortcut shortcut in this.shortcuts)
            {
                if (!shortcut.IsMappingKey(key))
                {
                    mapping = false;
                    break;
                }
            }

            return mapping;
        }

        /// <summary>
        /// Determines whether the keyboard combination is partial for any of the registered <see cref="RadShortcut">Shortcut</see> instance.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public bool IsPartialShortcut(Keys modifiers, params Keys[] mappings)
        {
            if (this.shortcuts.Count == 0)
            {
                return false;
            }

            bool match = true;
            foreach (RadShortcut shortcut in this.shortcuts)
            {
                if (!shortcut.IsPartialShortcutCombination(modifiers, mappings))
                {
                    match = false;
                    break;
                }
            }

            return match;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list with all the <see cref="RadShortcut">Shortcuts</see> registered for this command.
        /// </summary>
        public List<RadShortcut> Shortcuts
        {
            get
            {
                return this.shortcuts;
            }
        }

        #endregion
    }
}
