
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridItemValidatingEventHandler(object sender, PropertyGridItemValidatingEventArgs e);

    public class PropertyGridItemValidatingEventArgs : RadPropertyGridCancelEventArgs
    {
        private object oldValue;
        private object newValue;

        public PropertyGridItemValidatingEventArgs(PropertyGridItemElementBase visualItem, object oldValue, object newValue)
            : base(visualItem.Data)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public object OldValue
        {
            get
            {
                return oldValue;
            }
        }

        public object NewValue
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
            }
        }
    }
}
