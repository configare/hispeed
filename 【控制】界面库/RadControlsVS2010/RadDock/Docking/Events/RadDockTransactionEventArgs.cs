using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle all <see cref="RadDockTransaction">Transaction</see> related events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RadDockTransactionEventHandler(object sender, RadDockTransactionEventArgs e);

    /// <summary>
    /// Represents the arguments associated with all <see cref="RadDockTransaction">Transaction</see> related events.
    /// </summary>
    public class RadDockTransactionEventArgs : EventArgs
    {
        #region Fields

        private RadDockTransaction transaction;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance of the <see cref="RadDockTransactionEventArgs">RadDockTransactionEventArgs</see> class.
        /// </summary>
        /// <param name="transaction"></param>
        public RadDockTransactionEventArgs(RadDockTransaction transaction)
        {
            this.transaction = transaction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="RadDockTransaction">Transaction</see> instance associated with the event.
        /// </summary>
        public RadDockTransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }

        #endregion
    }
}
