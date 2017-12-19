using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Localization;

namespace Telerik.WinControls.UI
{
    public class RadPageViewLocalizationProvider : LocalizationProvider<RadPageViewLocalizationProvider>
    {
        /// <summary>
        /// Gets the string corresponding to the given ID.
        /// </summary>
        /// <param name="id">String ID</param>
        /// <returns>The string corresponding to the given ID.</returns>
        public override string GetLocalizedString(string id)
        {
            switch(id)
            {
                case RadPageViewStringId.CloseButtonTooltip:
                    return "Close Selected Page";
                case RadPageViewStringId.ItemListButtonTooltip:
                    return "Available Pages";
                case RadPageViewStringId.LeftScrollButtonTooltip:
                    return "Scroll Strip Left";
                case RadPageViewStringId.RightScrollButtonTooltip:
                    return "Scroll Strip Right";
                case RadPageViewStringId.ShowMoreButtonsItemCaption:
                    return "Show More Buttons";
                case RadPageViewStringId.ShowFewerButtonsItemCaption:
                    return "Show Fewer Buttons";
                case RadPageViewStringId.AddRemoveButtonsItemCaption:
                    return "Add or Remove Buttons";
                case RadPageViewStringId.ItemCloseButtonTooltip:
                    return "Close Page";
                case RadPageViewStringId.NewItemTooltipText:
                    return "Add New Page";
            }

            return string.Empty;
        }
    }

    public static class RadPageViewStringId
    {
        public const string LeftScrollButtonTooltip = "LeftScrollButton";
        public const string RightScrollButtonTooltip = "RightScrollButton";
        public const string ItemListButtonTooltip = "ItemListButton";
        public const string CloseButtonTooltip = "CloseButton";
        public const string ItemCloseButtonTooltip = "ItemCloseButton";
        public const string ScrollStripLeftCaption = "ScrollStripLeftCaption";
        public const string ScrollStripRightCaption = "ScrollStripRightCaption";
        public const string CloseSelectedPageCaption = "CloseSelectedPageCaption";
        public const string ShowFewerButtonsItemCaption = "ShowFewerButtonsItemCaption";
        public const string ShowMoreButtonsItemCaption = "ShowMoreButtonsItemCaption";
        public const string AddRemoveButtonsItemCaption = "AddRemoveButtonsItemCaption";
        public const string NewItemTooltipText = "NewItemTooltipText";
    }
}
