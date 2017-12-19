
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemValueChangingEventHandler(object sender, PropertyGridItemValueChangingEventArgs e);

    public class PropertyGridItemValueChangingEventArgs : ValueChangingEventArgs
    {
        PropertyGridItemBase item;

        public PropertyGridItemValueChangingEventArgs(PropertyGridItemBase item, object newValue, object oldValue)
        : base(newValue, oldValue)
        {
            this.item = item;
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
        }

        public PropertyGridTableElement PropertyGridElement
        {
            get
            {
                return this.item.PropertyGridTableElement;
            }
        }
    }
}