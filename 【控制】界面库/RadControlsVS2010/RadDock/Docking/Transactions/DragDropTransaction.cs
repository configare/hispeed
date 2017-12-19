using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a transaction that is associated with a successful drag-and-drop operation.
    /// </summary>
    public class DragDropTransaction : PerformDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Constructs a new <see cref="DragDropTransaction">DragDropTransaction</see> instance.
        /// </summary>
        /// <param name="state">The target state of the transaction.</param>
        /// <param name="draggedWindow">The DockWindow instance that has been dragged. May be null if the drag context has been DockTabStrip instance.</param>
        /// <param name="panel">The SplitPanel instance that has been dragged. May be null of the drag context has been DockWindow instance.</param>
        /// <param name="anchor">The SplitPanel instance that is used as dock anchor.</param>
        /// <param name="position">The DockPosition where to dock the dragged context.</param>
        public DragDropTransaction(DockState state, DockWindow draggedWindow, SplitPanel panel, SplitPanel anchor, DockPosition position)
            : base(state, null, anchor, position)
        {
            this.associatedPanel = panel;
            this.draggedWindow = draggedWindow;

            if (this.draggedWindow != null)
            {
                this.AssociatedWindows.Add(this.draggedWindow);
            }
            else
            {
                RadDock dockManager = null;
                if (panel is DockTabStrip)
                {
                    dockManager = (panel as DockTabStrip).DockManager;
                }
                else if (panel is RadSplitContainer)
                {
                    FloatingWindow floatingParent = ControlHelper.FindAncestor<FloatingWindow>(panel);
                    if (floatingParent != null)
                    {
                        dockManager = floatingParent.DockManager;
                    }
                }
                this.AssociatedWindows.AddRange(DockHelper.GetDockWindows(panel, true, dockManager));
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the <see cref="DockTransactionType.DragDrop">DragDrop</see> transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get 
            {
                return DockTransactionType.DragDrop;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SplitPanel instance associated with this transaction. May be null, depends on the dragged context.
        /// </summary>
        public SplitPanel AssociatedPanel
        {
            get
            {
                return this.associatedPanel;
            }
        }

        /// <summary>
        /// Gets the DockWindow instance associated with the transaction. May be null, depends on the dragged context.
        /// </summary>
        public DockWindow DraggedWindow
        {
            get
            {
                return this.draggedWindow;
            }
        }
        
        #endregion

        #region Fields

        private SplitPanel associatedPanel;
        private DockWindow draggedWindow;

        #endregion
    }
}
