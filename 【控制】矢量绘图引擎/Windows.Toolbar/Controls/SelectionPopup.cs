using System;
using System.Windows.Controls;

namespace Windows.Toolbar.Controls
{
    public class SelectionContainer : UserControl
    {
        #region Events

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event EventHandler<SelectedValueChangedEventArgs> SelectedValueChanged;
        /// <summary>
        /// Raises the <see cref="E:SelectedValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="SelectedValueChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedValueChanged(SelectedValueChangedEventArgs e)
        {
            EventHandler<SelectedValueChangedEventArgs> eventHandler = SelectedValueChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion
    }
}
