using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a base transaction that is associated with a successful DockWindow request.
    /// Concrete inheritors are <see cref="DockWindowTransaction">DockWindowTransaction</see> and <see cref="DragDropTransaction">DragDropTransaction</see>.
    /// </summary>
    public abstract class PerformDockTransaction : RadDockTransaction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="PerformDockTransaction">PerformDockTransaction</see> instance.
        /// </summary>
        /// <param name="state">The target state for the transaction.</param>
        /// <param name="windows">The associated DockWindow instances</param>
        /// <param name="anchor">The SplitPanel instance, which will be treated as a DockAnchor.</param>
        /// <param name="position">The DockPosition to be used when performing the DockWindow operation.</param>
        public PerformDockTransaction(DockState state, IEnumerable<DockWindow> windows, SplitPanel anchor, DockPosition position)
            : base(state, windows)
        {
            this.dockAnchor = anchor;
            this.position = position;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the DockPosition to be used when performing the DockWindow operation.
        /// </summary>
        public DockPosition Position
        {
            get
            {
                return this.position;
            }
        }

        /// <summary>
        /// Gets the SplitPanel instance, which will be treated as a DockAnchor.
        /// </summary>
        public SplitPanel DockAnchor
        {
            get
            {
                return this.dockAnchor;
            }
        }

        #endregion

        #region Fields

        private DockPosition position;
        private SplitPanel dockAnchor;

        #endregion
    }
}
