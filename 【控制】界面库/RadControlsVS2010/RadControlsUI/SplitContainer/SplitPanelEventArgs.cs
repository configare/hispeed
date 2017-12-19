using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    public delegate void SplitPanelEventHandler(object sender, SplitPanelEventArgs e);

    public class SplitPanelEventArgs : EventArgs
    {
        #region Constructor

        public SplitPanelEventArgs(SplitPanel panel)
        {
            this.panel = panel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SplitPanel instance associated with the event.
        /// </summary>
        public SplitPanel Panel
        {
            get
            {
                return this.panel;
            }
        }

        #endregion

        #region Fields

        private SplitPanel panel;

        #endregion
    }

    public delegate void ControlTreeChangedEventHandler(object sender, ControlTreeChangedEventArgs args);

    public class ControlTreeChangedEventArgs : EventArgs
    {
        #region Constructor

        public ControlTreeChangedEventArgs(Control parent, Control child, ControlTreeChangeAction action)
        {
            this.parent = parent;
            this.child = child;
            this.action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Control instance, which Controls collection has changed.
        /// </summary>
        public Control Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the child Control instance, inserted or removed in the Parent's collection.
        /// </summary>
        public Control Child
        {
            get
            {
                return this.child;
            }
        }

        /// <summary>
        /// Gets the action of the notification.
        /// </summary>
        public ControlTreeChangeAction Action
        {
            get
            {
                return this.action;
            }
        }

        #endregion

        #region Fields

        private Control parent;
        private Control child;
        private ControlTreeChangeAction action;

        #endregion
    }

    /// <summary>
    /// Defines the possible actions for a ControlTreeChanged event.
    /// </summary>
    public enum ControlTreeChangeAction
    {
        /// <summary>
        /// A control has been added.
        /// </summary>
        Add,
        /// <summary>
        /// A control has been removed.
        /// </summary>
        Remove,
    }
}
