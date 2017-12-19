using System;
using System.Windows.Controls;

namespace Windows.Toolbar.Controls.Specialized
{
    public partial class LinePicker : SelectionContainer
    {
        public LinePicker()
        {
            InitializeComponent();
        }

        private void LinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (LinePickerItem)e.AddedItems[0];

            OnSelectedValueChanged(new SelectedValueChangedEventArgs(selectedItem.Value));
        }
    }
}
