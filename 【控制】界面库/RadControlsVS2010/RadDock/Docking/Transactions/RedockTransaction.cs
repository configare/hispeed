using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    public enum RedockTransactionReason
    {
        /// <summary>
        /// Transaction is initiated from a double-click within the caption of a ToolTabStrip.
        /// </summary>
        ToolTabStripCaptionDoubleClick,
        /// <summary>
        /// Transaction is initiated from a double-click within the caption of a FloatingWindow.
        /// </summary>
        FloatingWindowCaptionDoubleClick,
        /// <summary>
        /// The associated TabItem of a DockWindow is double-clicked.
        /// </summary>
        TabItemDoubleClick,
        /// <summary>
        /// Transaction is initiated explicitly through code.
        /// </summary>
        Explicit,
    }

    /// <summary>
    /// Implements a transaction that is associated with a successful Redock request.
    /// </summary>
    public class RedockTransaction : RadDockTransaction
    {
        #region Fields

        private RedockTransactionReason reason;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new <see cref="RedockTransaction">RedockTransaction</see> instance.
        /// </summary>
        /// <param name="state">The target <see cref="DockState">dock state</see> for the transaction.</param>
        /// <param name="windows"></param>
        /// <param name="defaultAction"></param>
        public RedockTransaction(DockState state, IEnumerable<DockWindow> windows, bool defaultAction)
            : base(state, windows)
        {
            this.performDefaultAction = defaultAction;
            this.reason = RedockTransactionReason.Explicit;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Returns the <see cref="DockTransactionType.Redock">Redock</see> transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get
            {
                return DockTransactionType.Redock;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reason for the redock transaction.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RedockTransactionReason Reason
        {
            get
            {
                return this.reason;
            }
            internal set
            {
                this.reason = value;
            }
        }

        /// <summary>
        /// Determines whether a default action will be performed for dock windows without previously saved state.
        /// </summary>
        public bool PerformDefaultAction
        {
            get
            {
                return this.performDefaultAction;
            }
        }

        #endregion

        #region Fields

        private bool performDefaultAction;

        #endregion
    }
}
