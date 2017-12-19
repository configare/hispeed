using System;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class ToolbarTextBoxElement : RadTextBoxElement
    {
        ToolbarTextBoxButton searchButton;

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.searchButton = new ToolbarTextBoxButton();
            this.searchButton.DisplayStyle = DisplayStyle.Image;
        }

        public ToolbarTextBoxElement()
        {
            RadTextBoxItem item = this.TextBoxItem;
            
            this.Children.Remove(item);
            
            this.searchButton.SetValue(DockLayoutPanel.DockProperty, Dock.Right);

            DockLayoutPanel dockPanel = new DockLayoutPanel();
            dockPanel.LastChildFill = true;

            dockPanel.Children.Add(this.searchButton);
            dockPanel.Children.Add(item);

            this.Children.Add(dockPanel);
        }

        /// <summary>
        /// Gets the search button.
        /// </summary>
        public ToolbarTextBoxButton SearchButton
        {
            get
            {
                return this.searchButton;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            this.searchButton.IsSearching = !String.IsNullOrEmpty(this.Text);
        }
    }
}
