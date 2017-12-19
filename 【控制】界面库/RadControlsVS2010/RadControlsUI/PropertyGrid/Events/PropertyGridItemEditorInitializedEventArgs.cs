
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemEditorInitializedEventHandler(object sender, PropertyGridItemEditorInitializedEventArgs e);

    public class PropertyGridItemEditorInitializedEventArgs : PropertyGridVisualItemEventArgs
    {
        IValueEditor editor;

        public PropertyGridItemEditorInitializedEventArgs(PropertyGridItemElementBase visualItem, IValueEditor editor)
        : base(visualItem)
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