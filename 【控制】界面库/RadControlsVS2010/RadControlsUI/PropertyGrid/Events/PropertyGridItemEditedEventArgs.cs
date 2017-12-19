
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemEditedEventHandler(object sender, PropertyGridItemEditedEventArgs e);

    public class PropertyGridItemEditedEventArgs : PropertyGridVisualItemEventArgs
    {
        IValueEditor editor;
        bool canceled;

        public PropertyGridItemEditedEventArgs(PropertyGridItemElement visualItem, IValueEditor editor, bool canceled)
        : base(visualItem)
        {
            this.editor = editor;
            this.canceled = canceled;
        }

        public bool Canceled
        {
            get
            {
                return this.canceled;
            }
        }

        public IValueEditor Editor
        {
            get
            {
                return this.editor;
            }
        }
    }
}
