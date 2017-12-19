
namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridContextMenuOpeningEventHandler(object sender, PropertyGridContextMenuOpeningEventArgs e);

    public class PropertyGridContextMenuOpeningEventArgs : RadPropertyGridCancelEventArgs
    {
        private RadContextMenu menu;

        public PropertyGridContextMenuOpeningEventArgs(PropertyGridItemBase item, RadContextMenu contextMenu)
            : base(item)
        {
            this.menu = contextMenu;
        }

        public RadContextMenu Menu
        {
            get
            {
                return menu;
            }
        }
    }
}
