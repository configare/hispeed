using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    internal class QuickNavigatorDisplayCommand : RadDockCommand
    {
        #region Constructor

        public QuickNavigatorDisplayCommand()
        {
            this.Shortcuts.Add(new RadShortcut(Keys.Control, Keys.Tab));
            this.Name = PredefinedCommandNames.DisplayQuickNavigator;
        }

        #endregion

        public override bool CanExecute(object parameter)
        {
            if (!base.CanExecute(parameter))
            {
                return false;
            }

            RadDock dock = parameter as RadDock;
            if (dock != null)
            {
                Form dockParentForm = dock.FindForm();
                Form activeForm = Form.ActiveForm;
                return activeForm == dockParentForm || activeForm.Owner == dockParentForm;
            }

            return false;
        }

        public override object Execute(params object[] settings)
        {
            RadDock dockManager = settings[0] as RadDock;
            if (dockManager == null)
            {
                throw new ArgumentNullException("DockManager");
            }

			return dockManager.DisplayQuickNavigator();
        }
    }
}
