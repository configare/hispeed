
namespace Telerik.WinControls.UI
{
    public class PropertyGridItemEventArgs : RadPropertyGridEventArgs
    {
        PropertyGridItemElementBase element;

        public PropertyGridItemEventArgs(PropertyGridItemElementBase item)
            : base(item.Data)
        {
            this.element = item;
        }

        public PropertyGridItemElementBase VisualElement
        {
            get
            {
                return this.element;
            }
        }
    }
}
