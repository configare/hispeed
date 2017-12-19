
namespace Telerik.WinControls.UI
{
    public class PropertyGridContentElement: LightVisualElement
    {
        public PropertyGridTableElement PropertyGridTableElement
        {
            get
            {
                return this.FindAncestor<PropertyGridTableElement>();
            }
        }

        public PropertyGridItemElementBase VisualItem
        {
            get
            {
                return this.FindAncestor<PropertyGridItemElementBase>();
            }
        }
    }
}
