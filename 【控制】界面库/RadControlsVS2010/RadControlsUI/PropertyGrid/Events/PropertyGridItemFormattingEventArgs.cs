
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemFormattingEventHandler(object sender, PropertyGridItemFormattingEventArgs e);

    public class PropertyGridItemFormattingEventArgs : PropertyGridItemEventArgs
    {
        public PropertyGridItemFormattingEventArgs(PropertyGridItemElementBase item)
            : base(item)
        {
        }
    }
}
