using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridSelectedObjectChangingEventHandler(object sender, PropertyGridSelectedObjectChangingEventArgs e);

    public class PropertyGridSelectedObjectChangingEventArgs : CancelEventArgs
    {
        private object selectedObject;

        public PropertyGridSelectedObjectChangingEventArgs(object selectedObject)
            : base()
        {
            this.selectedObject = selectedObject;
        }

        public PropertyGridSelectedObjectChangingEventArgs(object selectedObject, bool cancel)
            : base(cancel)
        {
            this.selectedObject = selectedObject;
        }

        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
        }
    }
}
