using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ToolbarTextBoxButton : RadButtonElement
    {
        public static RadProperty IsSearchingProperty = RadProperty.Register(
            "IsSearching", typeof(bool), typeof(ToolbarTextBoxButton), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        static ToolbarTextBoxButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new SearchBarTextBoxButtonStateManager(), typeof(ToolbarTextBoxButton));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.MinSize = new Size(15, 15);
        }

        public bool IsSearching
        {
            get
            {
                return (bool)this.GetValue(IsSearchingProperty);
            }
            set
            {
                this.SetValue(IsSearchingProperty, value);
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (this.IsSearching)
            {
                base.OnMouseDown(e);

                ToolbarTextBoxElement searchTextBox = this.FindAncestor<ToolbarTextBoxElement>();
                if (searchTextBox != null)
                {
                    searchTextBox.Text = String.Empty;
                }
            }
        }
    }
}
