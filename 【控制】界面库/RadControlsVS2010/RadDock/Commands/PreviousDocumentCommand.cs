using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    internal class PreviousDocumentCommand : RadDockCommand
    {
        #region Constructor

        public PreviousDocumentCommand()
        {
            this.Shortcuts.Add(new RadShortcut(Keys.Control | Keys.Shift, Keys.F6));
            this.Name = PredefinedCommandNames.PreviousDocument;
        }

        #endregion

        public override object Execute(params object[] settings)
        {
            RadDock dockManager = settings[0] as RadDock;
            if (dockManager == null)
            {
                throw new ArgumentNullException("DockManager");
            }

            dockManager.DocumentManager.ActivatePreviousDocument();

            return null;
        }
    }
}
