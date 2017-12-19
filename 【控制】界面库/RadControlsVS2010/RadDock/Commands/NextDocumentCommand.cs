using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    internal class NextDocumentCommand : RadDockCommand
    {
        #region Constructor

        public NextDocumentCommand()
        {
            this.Shortcuts.Add(new RadShortcut(Keys.Control, Keys.F6));
            this.Name = PredefinedCommandNames.NextDocument;
        }

        #endregion

        public override object Execute(params object[] settings)
        {
            RadDock dockManager = settings[0] as RadDock;
            if (dockManager == null)
            {
                throw new ArgumentNullException("DockManager");
            }

            dockManager.DocumentManager.ActivateNextDocument();

            return null;
        }
    }
}
