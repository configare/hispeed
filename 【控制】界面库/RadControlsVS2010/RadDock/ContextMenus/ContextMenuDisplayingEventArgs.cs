using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    public delegate void ContextMenuDisplayingEventHandler(object sender, ContextMenuDisplayingEventArgs e);

    /// <summary>
    /// Encapsulates the arguments, associated with 
    /// </summary>
    public class ContextMenuDisplayingEventArgs : CancelEventArgs
    {
        #region Fields

        private List<RadMenuItemBase> menuItems;
        private ContextMenuType menuType;
        private DockWindow window;
        private DocumentTabStrip documentStrip;
        private Point displayPosition;

        #endregion

        #region Constructor

        private ContextMenuDisplayingEventArgs(List<RadMenuItemBase> items, Point displayPos)
        {
            this.menuItems = items;
            this.displayPosition = displayPos;
        }

        public ContextMenuDisplayingEventArgs(DockWindow window, List<RadMenuItemBase> items, Point displayPos)
            : this(items, displayPos)
        {
            this.window = window;
            this.menuType = ContextMenuType.DockWindow;
        }

        public ContextMenuDisplayingEventArgs(DocumentTabStrip strip, List<RadMenuItemBase> items, Point displayPos)
            : this(items, displayPos)
        {
            this.documentStrip = strip;
            this.menuType = ContextMenuType.ActiveWindowList;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the context menu requested.
        /// </summary>
        public ContextMenuType MenuType
        {
            get
            {
                return this.menuType;
            }
        }

        /// <summary>
        /// Gets the List of menu items, which are about to be displayed.
        /// </summary>
        public List<RadMenuItemBase> MenuItems
        {
            get
            {
                return this.menuItems;
            }
        }

        /// <summary>
        /// Gets the <see cref="DockWindow">DockWindow</see> instance, associated with the event.
        /// Valid when the MenuType is <see cref="ContextMenuType.DockWindow">DockWindow</see>.
        /// </summary>
        public DockWindow DockWindow
        {
            get
            {
                return this.window;
            }
        }

        /// <summary>
        /// Gets the <see cref="DocumentTabStrip">DocumentTabStrip</see> instance, associated with the event.
        /// Valid when the MenuType is <see cref="ContextMenuType.ActiveWindowList">ActiveWindowList</see>.
        /// </summary>
        public DocumentTabStrip DocumentStrip
        {
            get
            {
                return this.documentStrip;
            }
        }

        /// <summary>
        /// Gets or sets the position (in screen coordinates) where the context menu will be displayed.
        /// </summary>
        public Point DisplayPosition
        {
            get
            {
                return this.displayPosition;
            }
            set
            {
                this.displayPosition = value;
            }
        }

        #endregion
    }
}
