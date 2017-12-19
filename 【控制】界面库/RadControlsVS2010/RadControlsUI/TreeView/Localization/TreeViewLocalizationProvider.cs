using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class TreeViewLocalizationProvider : Localization.LocalizationProvider<TreeViewLocalizationProvider>
    {
        public override string GetLocalizedString(string id)
        {
            switch (id)
            {
                case TreeViewStringId.ContextMenuCollapse:
                    return "Collapse";
                case TreeViewStringId.ContextMenuDelete:
                    return "Delete";
                case TreeViewStringId.ContextMenuEdit:
                    return "Edit";
                case TreeViewStringId.ContextMenuExpand:
                    return "Expand";
                case TreeViewStringId.ContextMenuNew:
                    return "New";
            }

            return "";
        }
    }
}
