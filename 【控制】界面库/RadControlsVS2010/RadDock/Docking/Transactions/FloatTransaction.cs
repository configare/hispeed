using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a transaction that is associated with a FloatWindow operation.
    /// </summary>
    public class FloatTransaction : RadDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="FloatTransaction">FloatTransaction</see> instance.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="panel"></param>
        /// <param name="bounds"></param>
        public FloatTransaction(IEnumerable<DockWindow> windows, SplitPanel panel, Rectangle bounds)
            : base(DockState.Floating, windows)
        {
            this.bounds = bounds;
            this.associatedPanel = panel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="SplitPanel">SplitPanel</see> instance, associated with the transaction.
        /// Valid when a ToolTabStrip has been floated.
        /// </summary>
        public SplitPanel AssociatedPanel
        {
            get
            {
                return this.associatedPanel;
            }
        }

        /// <summary>
        /// Gets the bounds at which the associated windows should be floated.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the <see cref="DockTransactionType.Float">Float</see> dock transaction type.
        /// </summary>
        public override DockTransactionType TransactionType
        {
            get 
            {
                return DockTransactionType.Float;
            }
        }

        #endregion

        #region Fields

        private SplitPanel associatedPanel;
        private Rectangle bounds;

        #endregion
    }
}
