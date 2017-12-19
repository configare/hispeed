using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a transaction that is associated with a successful CloseWindow request.
    /// </summary>
    public class CloseTransaction : RadDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Constructs a new instance of the <see cref="CloseTransaction">CloseTransaction</see> class.
        /// </summary>
        /// <param name="windows">The <see cref="DockWindow">DockWindows</see> collection associated with the transaction.</param>
        public CloseTransaction(IEnumerable<DockWindow> windows)
            : base(DockState.Hidden, windows)
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the <see cref="DockTransactionType.Close">Close</see> transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get 
            {
                return DockTransactionType.Close;
            }
        }

        #endregion

        #region Fields
        #endregion
    }
}
