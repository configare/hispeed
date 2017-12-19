using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Localization;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides localization services for <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarLocalizationProvider : LocalizationProvider<CommandBarLocalizationProvider>
    {
        public override string GetLocalizedString(string id)
        {
            switch(id)
            {
                case CommandBarStringId.CustomizeDialogChooseToolstripLabelText : return "Choose a toolstrip to rearrange:";
                case CommandBarStringId.CustomizeDialogCloseButtonText : return "Close";
                case CommandBarStringId.CustomizeDialogItemsPageTitle: return "Items";
                case CommandBarStringId.CustomizeDialogMoveDownButtonText: return "Move Down";
                case CommandBarStringId.CustomizeDialogMoveUpButtonText: return "Move Up";
                case CommandBarStringId.CustomizeDialogResetButtonText: return "Reset";
                case CommandBarStringId.CustomizeDialogTitle: return "Customize";
                case CommandBarStringId.CustomizeDialogToolstripsPageTitle: return "Toolstrips";
                case CommandBarStringId.OverflowMenuAddOrRemoveButtonsText: return "Add or Remove Buttons";
                case CommandBarStringId.OverflowMenuCustomizeText : return "Customize...";
                case CommandBarStringId.ContextMenuCustomizeText : return "Customize...";
            }

            return string.Empty;
        }
    }
}
