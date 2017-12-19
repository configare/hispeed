using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Commands;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    internal class ActiveDocumentCloseCommand : RadDockCommand
    {
        #region Constructor

        public ActiveDocumentCloseCommand()
        {
            this.Shortcuts.Add(new RadShortcut(Keys.Control, Keys.F4));
            this.Name = PredefinedCommandNames.CloseActiveDocument;
        }

        #endregion

        public override object Execute(params object[] settings)
        {
            RadDock dockManager = settings[0] as RadDock;
            if (dockManager == null)
            {
                throw new ArgumentNullException("DockManager");
            }

            DockWindow activeDoc = dockManager.DocumentManager.ActiveDocument;
            if (activeDoc != null)
            {
                dockManager.CloseWindow(activeDoc);
            }

            return base.Execute(settings);
        }
    }
}
