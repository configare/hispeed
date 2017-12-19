
namespace Telerik.WinControls.UI
{
    public class PropertyGridVisualItemEventArgs : RadPropertyGridEventArgs
    {
        PropertyGridItemElementBase visualItem;

        public PropertyGridVisualItemEventArgs(PropertyGridItemElementBase visualItem)
            : base(visualItem.Data)
        {
            this.visualItem = visualItem;
        }

        public PropertyGridItemElementBase ItemElement
        {
            get
            {
                return this.visualItem;
            }
        }
    }
}
