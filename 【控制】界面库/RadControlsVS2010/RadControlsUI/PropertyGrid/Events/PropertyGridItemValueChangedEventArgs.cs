
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemValueChangedEventHandler(object sender, PropertyGridItemValueChangedEventArgs e);

    public class PropertyGridItemValueChangedEventArgs : RadPropertyGridEventArgs
    {
        public PropertyGridItemValueChangedEventArgs(PropertyGridItemBase item)
            : base(item)
        {
        }
    }
}