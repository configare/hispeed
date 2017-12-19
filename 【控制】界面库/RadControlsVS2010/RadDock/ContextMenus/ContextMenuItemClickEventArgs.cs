using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    public delegate void ContextMenuItemClickEventHandler(object sender, ContextMenuItemClickEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class ContextMenuItemClickEventArgs : DockWindowEventArgs
    {
        #region Fields

        private RadMenuItemBase item;
        private bool handled;

        #endregion

        #region Constructor

        public ContextMenuItemClickEventArgs(DockWindow window, RadMenuItemBase item)
            : base(window)
        {
            this.item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Telerik.WinControls.UI.RadMenuItemBase">RadMenuItemBase</see> instance, that is clicked.
        /// </summary>
        public RadMenuItemBase Item
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Determines whether the event is handled by the user and default action should not be performed.
        /// </summary>
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }

        #endregion
    }
}
