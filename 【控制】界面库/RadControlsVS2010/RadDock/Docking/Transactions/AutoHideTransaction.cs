using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a transaction that is associated with a successful AutoHideWindow request.
    /// </summary>
    public class AutoHideTransaction : RadDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Constructs a new <see cref="AutoHideTransaction">AutoHideTransaction</see> instance.
        /// </summary>
        /// <param name="windows">The DockWindow instances that are associated with the transaction.</param>
        /// <param name="pos">The desired auto-hide position</param>
        public AutoHideTransaction(IEnumerable<DockWindow> windows, AutoHidePosition pos)
            : base(DockState.AutoHide, windows)
        {
            this.position = pos;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the <see cref="DockTransactionType.AutoHide">AutoHide</see> transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get 
            {
                return DockTransactionType.AutoHide;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="AutoHidePosition"/> instance associated with the transaction.
        /// </summary>
        public AutoHidePosition Position
        {
            get
            {
                return this.position;
            }
        }

        #endregion

        #region Fields

        private AutoHidePosition position;

        #endregion
    }
}
