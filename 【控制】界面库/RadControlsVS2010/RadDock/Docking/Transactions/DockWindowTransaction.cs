using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a transaction that is associated with a DockWindow request.
    /// </summary>
    public class DockWindowTransaction : PerformDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Constructs a new <see cref="DockWindowTransaction">DockWindowTransaction</see> instance.
        /// </summary>
        /// <param name="state">The target state for the transaction.</param>
        /// <param name="window">The window associated with the transaction.</param>
        /// <param name="targetWindow">The target window for the transaction (may be null).</param>
        /// <param name="anchor">The <see cref="SplitPanel">SplitPanel</see> instance that is used as a dock anchor.</param>
        /// <param name="position">The <see cref="DockPosition">DockPosition</see> where the window should be docked.</param>
        public DockWindowTransaction(DockState state, DockWindow window, DockWindow targetWindow, SplitPanel anchor, DockPosition position)
            : base(state, new DockWindow[] { window }, anchor, position)
        {
            this.targetWindow = targetWindow;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the DockWindow instance that is target for the transaction.
        /// </summary>
        public DockWindow TargetWindow
        {
            get
            {
                return this.targetWindow;
            }
        }

        /// <summary>
        /// Returns the <see cref="DockTransactionType.DockWindow">DockWindow</see> transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get
            {
                return DockTransactionType.DockWindow;
            }
        }

        /// <summary>
        /// Gets the DockWindow instance associated with the transaction.
        /// </summary>
        public DockWindow AssociatedWindow
        {
            get
            {
                return this.AssociatedWindows[0];
            }
        }

        #endregion

        #region Fields

        private DockWindow targetWindow;

        #endregion
    }
}
