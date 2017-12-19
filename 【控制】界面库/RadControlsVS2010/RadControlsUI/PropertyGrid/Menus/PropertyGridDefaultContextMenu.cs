using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class PropertyGridDefaultContextMenu : RadContextMenu
    {
        #region Fields

        private PropertyGridTableElement propertyGridElement;
        private PropertyGridMenuItem expandCollapseMenuItem;
        private PropertyGridMenuItem resetMenuItem;
        private PropertyGridMenuItem editMenuItem;
        private PropertyGridMenuItem showDescriptionMenuItem;
        private PropertyGridMenuItem showToolbarMenuItem;

        private PropertyGridMenuItem noSortMenuItem;
        private PropertyGridMenuItem alphabeticalMenuItem;
        private PropertyGridMenuItem categorizedMenuItem;
        private PropertyGridMenuItem categorizedAlphabeticalMenuItem;


        #endregion

        #region Constructor

        public PropertyGridDefaultContextMenu(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;
            
            this.resetMenuItem = new PropertyGridMenuItem("Reset",
                PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuReset));
            this.Items.Add(resetMenuItem);

            this.editMenuItem = new PropertyGridMenuItem("Edit",
                PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuEdit));
            this.Items.Add(editMenuItem);

            this.expandCollapseMenuItem = new PropertyGridMenuItem("Expand",
                PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuExpand));
            this.Items.Add(expandCollapseMenuItem);


            this.Items.Add(new RadMenuSeparatorItem());

            RadMenuItem sortItem = new RadMenuItem(PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuSort));
            this.Items.Add(sortItem);
            
            noSortMenuItem = new PropertyGridMenuItem("NoSort", PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuNoSort));
            noSortMenuItem.Click += menuItem_Click;
            sortItem.Items.Add(noSortMenuItem);

            alphabeticalMenuItem = new PropertyGridMenuItem("Alphabetical", PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuAlphabetical));
            alphabeticalMenuItem.Click += menuItem_Click;
            sortItem.Items.Add(alphabeticalMenuItem);

            categorizedMenuItem = new PropertyGridMenuItem("Categorized", PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuCategorized));
            categorizedMenuItem.Click += menuItem_Click;
            sortItem.Items.Add(categorizedMenuItem);

            categorizedAlphabeticalMenuItem = new PropertyGridMenuItem("CategorizedAlphabetical", PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuCategorizedAlphabetical));
            categorizedAlphabeticalMenuItem.Click += menuItem_Click;
            sortItem.Items.Add(categorizedAlphabeticalMenuItem);

            this.Items.Add(new RadMenuSeparatorItem());

            this.showDescriptionMenuItem = new PropertyGridMenuItem("ShowDescription",
                PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuShowDescription));
            this.Items.Add(showDescriptionMenuItem);

            this.showToolbarMenuItem = new PropertyGridMenuItem("ShowToolbar",
                PropertyGridLocalizationProvider.CurrentProvider.GetLocalizedString(PropertyGridStringId.ContextMenuShowToolbar));
            this.Items.Add(showToolbarMenuItem);
            
            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Click += menuItem_Click;
            }
        }

        #endregion

        #region Properties

        public PropertyGridMenuItem ExpandCollapseMenuItem
        {
            get { return this.expandCollapseMenuItem; }
        }

        public PropertyGridMenuItem EditMenuItem
        {
            get { return this.editMenuItem; }
        }

        public PropertyGridMenuItem ResetMenuItem
        {
            get { return this.resetMenuItem; }
        }

        public PropertyGridMenuItem ShowDescriptionMenuItem
        {
            get { return this.showDescriptionMenuItem; }
        }

        public PropertyGridMenuItem ShowToolbarMenuItem
        {
            get { return this.showToolbarMenuItem; }
        }

        #endregion

        #region Event Handlers

        protected override void OnDropDownOpening(CancelEventArgs args)
        {
            PropertyGridItemBase item = this.propertyGridElement.SelectedGridItem;
            if (item != null)
            {
                if (item is PropertyGridGroupItem)
                {
                    EditMenuItem.Visibility = ElementVisibility.Collapsed;
                    ResetMenuItem.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    EditMenuItem.Visibility = ElementVisibility.Visible;
                    ResetMenuItem.Visibility = ElementVisibility.Visible;
                    ResetMenuItem.Enabled = ((PropertyGridItem)item).IsModified;
                }
            }

            showDescriptionMenuItem.IsChecked = propertyGridElement.PropertyGridElement.SplitElement.HelpVisible;
            showToolbarMenuItem.IsChecked = propertyGridElement.PropertyGridElement.ToolbarVisible;

            noSortMenuItem.IsChecked = propertyGridElement.PropertySort == System.Windows.Forms.PropertySort.NoSort;
            alphabeticalMenuItem.IsChecked = propertyGridElement.PropertySort == System.Windows.Forms.PropertySort.Alphabetical;
            categorizedMenuItem.IsChecked = propertyGridElement.PropertySort == System.Windows.Forms.PropertySort.Categorized;
            categorizedAlphabeticalMenuItem.IsChecked = propertyGridElement.PropertySort == System.Windows.Forms.PropertySort.CategorizedAlphabetical;

            base.OnDropDownOpening(args);

            if (args.Cancel)
            {
                return;
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            PropertyGridMenuItem menuItem = sender as PropertyGridMenuItem;

            if (menuItem == null)
            {
                return;
            }

            switch (menuItem.Command)
            {
                case "Reset":
                    Reset();
                    break;
                case "Edit":
                    EditNode();
                    break;
                case "Expand":
                case "Collapse":
                    ExpandNode();
                    break;
                case "ShowToolbar":
                    ShowToolbar();
                    break;
                case "ShowDescription":
                    ShowDescription();
                    break;

                case "NoSort":
                    propertyGridElement.PropertySort = System.Windows.Forms.PropertySort.NoSort;
                    break;
                case "Alphabetical":
                    propertyGridElement.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
                    break;
                case "Categorized":
                    propertyGridElement.PropertySort = System.Windows.Forms.PropertySort.Categorized;
                    break;
                case "CategorizedAlphabetical":
                    propertyGridElement.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
                    break;
            }
        }

        #endregion

        #region Methods

        private void Reset()
        {
            PropertyGridItem item = this.propertyGridElement.SelectedGridItem as PropertyGridItem;
            if (item != null)
            {
                item.ResetValue();
                this.propertyGridElement.EndEdit();
            }
        }

        private void ShowDescription()
        {
            propertyGridElement.PropertyGridElement.SplitElement.HelpVisible = !propertyGridElement.PropertyGridElement.SplitElement.HelpVisible;
        }

        private void ShowToolbar()
        {
            propertyGridElement.PropertyGridElement.ToolbarVisible = !propertyGridElement.PropertyGridElement.ToolbarVisible;
        }

        private void ExpandNode()
        {
            PropertyGridItemBase item = this.propertyGridElement.SelectedGridItem;
            if (item != null)
            {
                item.Expanded = !item.Expanded;
            }
        }

        private void EditNode()
        {
            this.propertyGridElement.BeginEdit();
        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Click -= menuItem_Click;
            }

            editMenuItem.Dispose();
            expandCollapseMenuItem.Dispose();
            showDescriptionMenuItem.Dispose();
            showToolbarMenuItem.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
