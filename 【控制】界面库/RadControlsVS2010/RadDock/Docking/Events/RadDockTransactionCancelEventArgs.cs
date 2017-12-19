using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template that is used to handle a <see cref="RadDock.TransactionCommitting">TransactionCommitting</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RadDockTransactionCancelEventHandler(object sender, RadDockTransactionCancelEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="RadDock.TransactionCommitting">TransactionCommitting</see> event.
    /// </summary>
    public class RadDockTransactionCancelEventArgs : RadDockTransactionEventArgs
    {
        #region Fields

        private bool cancel;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public RadDockTransactionCancelEventArgs(RadDockTransaction transaction)
            : base(transaction)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the transaction should be canceled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        #endregion
    }
}
