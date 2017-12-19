using System;
using System.Windows.Controls;

namespace Windows.Toolbar.Controls.Specialized
{
    public partial class ColorPicker : SelectionContainer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class.
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void ColorPickerItem_Click(object sender, EventArgs e)
        {
            ColorPickerItem colorItem = sender as ColorPickerItem;

            // Send an event with the selected color, this time we need
            // to send an event with custom args

            OnSelectedValueChanged(new SelectedValueChangedEventArgs(colorItem.Color));
        }

        #endregion
    }
}
