using System;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridSelectedObjectChangedEventHandler(object sender, PropertyGridSelectedObjectChangedEventArgs e);

    public class PropertyGridSelectedObjectChangedEventArgs : EventArgs
    {
        private object selectedObject;

        public PropertyGridSelectedObjectChangedEventArgs(object selectedObject)
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
