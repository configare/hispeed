
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemEditingEventHandler(object sender, PropertyGridItemEditingEventArgs e);

    public class PropertyGridItemEditingEventArgs : RadPropertyGridCancelEventArgs
    {
        IValueEditor editor;

        public PropertyGridItemEditingEventArgs(PropertyGridItemBase item, IValueEditor editor)
        : base(item)
        {
            this.editor = editor;
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