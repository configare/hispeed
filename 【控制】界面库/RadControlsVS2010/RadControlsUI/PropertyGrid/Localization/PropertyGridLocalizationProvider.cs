namespace Telerik.WinControls.UI
{
    public class PropertyGridLocalizationProvider : Localization.LocalizationProvider<PropertyGridLocalizationProvider>
    {
        public override string GetLocalizedString(string id)
        {
            switch (id)
            {
                case PropertyGridStringId.ContextMenuReset: return "Reset";
                case PropertyGridStringId.ContextMenuEdit: return "Edit";
                case PropertyGridStringId.ContextMenuExpand: return "Expand";
                case PropertyGridStringId.ContextMenuCollapse: return "Collapse";
                case PropertyGridStringId.ContextMenuShowDescription: return "Show description";
                case PropertyGridStringId.ContextMenuShowToolbar: return "Show toolbar";

                case PropertyGridStringId.ContextMenuSort: return "Sort";
                case PropertyGridStringId.ContextMenuNoSort: return "No Sort";
                case PropertyGridStringId.ContextMenuAlphabetical: return "Alphabetical";
                case PropertyGridStringId.ContextMenuCategorized: return "Categorized";
                case PropertyGridStringId.ContextMenuCategorizedAlphabetical: return "Categorized Alphabetical";                
            }

            return "";
        }
    }
}
