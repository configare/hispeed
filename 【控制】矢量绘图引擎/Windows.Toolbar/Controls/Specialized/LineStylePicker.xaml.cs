using System.Windows.Controls;
using System.Linq;

namespace Windows.Toolbar.Controls.Specialized
{
    public partial class LineStylePicker : SelectionContainer
    {
        public LineStylePicker()
        {
            InitializeComponent();
        }

        private void LinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (LineStylePickerItem)e.AddedItems[0];

            OnSelectedValueChanged(new SelectedValueChangedEventArgs(selectedItem.Value));
        }
    }
}
