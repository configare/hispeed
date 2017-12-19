using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A logical representation of a RadDock operation.
    /// </summary>
    public abstract class RadDockTransaction : RadDockObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="RadDockTransaction">RadDockTransaction</see> instance.
        /// </summary>
        /// <param name="state">The target state of the transaction.</param>
        /// <param name="windows">The associated <see cref="DockWindow">DockWindow</see> instances.</param>
        public RadDockTransaction(DockState state, IEnumerable<DockWindow> windows)
        {
            this.addedWindows = new List<DockWindow>();
            this.removedWindows = new List<DockWindow>();
            this.associatedWindows = new List<DockWindow>();
            if (windows != null)
            {
                this.associatedWindows.AddRange(windows);
            }
            this.targetState = state;
            this.restoreState = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of this transaction.
        /// </summary>
        public abstract DockTransactionType TransactionType
        {
            get;
        }

        /// <summary>
        /// Gets all the windows associated with this transaction.
        /// </summary>
        public List<DockWindow> AssociatedWindows
        {
            get
            {
                return this.associatedWindows;
            }
        }

        /// <summary>
        /// Gets all the windows that are new to the manager and are registered by the transaction.
        /// </summary>
        public List<DockWindow> AddedWindows
        {
            get
            {
                return this.addedWindows;
            }
        }

        /// <summary>
        /// Gets all the windows that are removed from the manager by the transaction.
        /// </summary>
        public List<DockWindow> RemovedWindows
        {
            get
            {
                return this.removedWindows;
            }
        }

        /// <summary>
        /// Gets the state, which is targeted by this transaction.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockState TargetState
        {
            get
            {
                return this.targetState;
            }
            internal set
            {
                this.targetState = value;
            }
        }

        /// <summary>
        /// Determines whether the transaction will try to first restore
        /// the state of all associated windows before any other action is performed.
        /// </summary>
        public bool RestoreState
        {
            get
            {
                return this.restoreState;
            }
            set
            {
                this.restoreState = value;
            }
        }

        #endregion

        #region Fields

        private List<DockWindow> addedWindows;
        private List<DockWindow> removedWindows;
        private List<DockWindow> associatedWindows;
        private DockState targetState;
        private bool restoreState;

        #endregion
    }
}
