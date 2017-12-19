using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a service that allows a DockWindow state to be saved and restored later on.
    /// </summary>
    public class RedockService : RadDockService
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RedockService()
        {
            this.oldStates = new List<RedockState>();
            this.redockStates = new Dictionary<string, Dictionary<DockState, RedockState>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Temporary suspends any clean-up operations.
        /// Used by the owning RadDock instance to prevent undesired processing during multiple transactions stack.
        /// </summary>
        public void SuspendCleanUp()
        {
            this.cleanUpSuspendCount++;
        }

        /// <summary>
        /// Performs a redock operation upon specified windows.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="defaultAction">True to perform default action if no state is recorded for a window, false otherwise.</param>
        public void Redock(IEnumerable<DockWindow> windows, bool defaultAction)
        {
            this.Redock(windows, RedockTransactionReason.Explicit, defaultAction);
        }

        /// <summary>
        /// Performs a redock operation upon specified windows.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="reason"></param>
        /// <param name="defaultAction">True to perform default action if no state is recorded for a window, false otherwise.</param>
        public void Redock(IEnumerable<DockWindow> windows, RedockTransactionReason reason, bool defaultAction)
        {
            if (!this.CanOperate())
            {
                return;
            }

            this.DockManager.BeginTransactionBlock(true);
            foreach (DockWindow window in windows)
            {
                this.Redock(window, reason, defaultAction);
            }
            this.DockManager.EndTransactionBlock();
        }

        public void Redock(DockWindow window, bool defaultAction)
        {
            this.Redock(window, RedockTransactionReason.Explicit, defaultAction);
        }

        /// <summary>
        /// Performs a redock operation upon specified window.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="reason"></param>
        /// <param name="defaultAction">True to perform default action if no state is recorded for a window, false otherwise.</param>
        public void Redock(DockWindow window, RedockTransactionReason reason, bool defaultAction)
        {
            if (!this.CanOperate())
            {
                return;
            }

            DockState state;
            if (window.DockState == DockState.Hidden)
            {
                state = window.PreviousDockState;
            }
            else
            {
                state = GetNewDockState(window.DockState);
            }
            RedockTransaction transaction = new RedockTransaction(state, new DockWindow[] { window }, defaultAction);
            transaction.Reason = reason;
            this.DockManager.RegisterTransaction(transaction, true);
        }

        /// <summary>
        /// Attempts to restore the state of each window to the specified one.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="state">The target state which is to be restored.</param>
        /// <param name="defaultAction">True to perform default action if no state is recorded for a window, false otherwise.</param>
        public void RestoreState(IEnumerable<DockWindow> windows, DockState state, bool defaultAction)
        {
            if (!this.CanOperate())
            {
                return;
            }

            this.DockManager.BeginTransactionBlock(true);
            foreach (DockWindow window in windows)
            {
                this.RestoreState(window, state, defaultAction);
            }
            this.DockManager.EndTransactionBlock();
        }

        /// <summary>
        /// Attempts to restore the state the window to the desired dock state.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="state">The target state which is to be restored.</param>
        /// <param name="defaultAction">True to perform default action if no state is recorded for a window, false otherwise.</param>
        public void RestoreState(DockWindow window, DockState state, bool defaultAction)
        {
            if (!this.CanOperate())
            {
                return;
            }

            RedockTransaction transaction = new RedockTransaction(state, new DockWindow[] { window }, defaultAction);
            this.DockManager.RegisterTransaction(transaction, true);
        }

        /// <summary>
        /// Resumes previously suspended clean-up.
        /// </summary>
        /// <param name="forceCleanUp">True to force a clean-up pass, false otherwise.</param>
        public void ResumeCleanUp(bool forceCleanUp)
        {
            if (this.cleanUpSuspendCount == 0)
            {
                return;
            }

            this.cleanUpSuspendCount--;
            this.cleanUpSuspendCount = Math.Max(0, this.cleanUpSuspendCount);

            if (this.cleanUpSuspendCount == 0 && forceCleanUp)
            {
                this.CleanUp();
            }
        }

        /// <summary>
        /// Saves the current state of the specified window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public virtual bool SaveState(DockWindow window)
        {
            if (!this.CanOperate())
            {
                return false;
            }

            if (!IsSaveApplicable(window))
            {
                return false;
            }

            return this.SaveStateCore(window);
        }

        /// <summary>
        /// Determines whether a Save operation is applicable for the specified window.
        /// Currently supported states are <see cref="DockState.Docked">Docked</see> and <see cref="DockState.Floating">Floating</see>.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool IsSaveApplicable(DockWindow window)
        {
            return window.DockState == DockState.Docked || window.DockState == DockState.Floating;
        }

        /// <summary>
        /// Removes previously saved redock state for the specified window for the specified DockState.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dockState"></param>
        /// <returns></returns>
        public bool ClearState(DockWindow window, DockState dockState)
        {
            RedockState state = this.GetState(window, dockState, true);
            if (state == null)
            {
                return false;
            }

            this.RemovePreviousState(state);
            //remove from clean-up list
            int index = this.oldStates.IndexOf(state);
            if (index != -1)
            {
                this.oldStates.RemoveAt(index);
            }

            return true;
        }

        /// <summary>
        /// Removes all the redock states saved for the specified window.
        /// </summary>
        /// <param name="window"></param>
        public void ClearAllStates(DockWindow window)
        {
            Dictionary<DockState, RedockState> states;
            this.redockStates.TryGetValue(window.Name, out states);
            if (states == null)
            {
                return;
            }

            foreach (RedockState state in states.Values)
            {
                this.RemovePreviousState(state);
                int index = this.oldStates.IndexOf(state);
                if (index >= 0)
                {
                    this.oldStates.RemoveAt(index);
                }
            }

            states.Clear();
            this.redockStates.Remove(window.Name);
        }

        /// <summary>
        /// Retrieves the new DockState for a redock operation, depending on the provided current DockState.
        /// </summary>
        /// <param name="currState"></param>
        /// <returns></returns>
        public static DockState GetNewDockState(DockState currState)
        {
            DockState state = DockState.Docked;
            switch (currState)
            {
                case DockState.Floating:
                case DockState.AutoHide:
                    state = DockState.Docked;
                    break;
                case DockState.Docked:
                    state = DockState.Floating;
                    break;
                case DockState.TabbedDocument:
                    state = currState;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Removes previous redock states which are no longer valid.
        /// For example, if we save a state for a dock window in a Docked state, we need to remove any previous redock state for the Docked state.
        /// </summary>
        public void CleanUp()
        {
            if (this.cleanUpSuspendCount > 0)
            {
                return;
            }
            foreach (RedockState state in this.oldStates)
            {
                this.RemovePreviousState(state);
            }

            this.oldStates.Clear();
        }

        #endregion

        #region Overridables

        /// <summary>
        /// Preforms the core Save logic.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        protected virtual bool SaveStateCore(DockWindow window)
        {
            Dictionary<DockState, RedockState> states;
            this.redockStates.TryGetValue(window.Name, out states);

            if (states == null)
            {
                states = new Dictionary<DockState, RedockState>();
                this.redockStates[window.Name] = states;
            }

            RedockState savedState;
            if (window.DockState == DockState.Floating)
            {
                savedState = new RedockFloatingState(window);
            }
            else
            {
                savedState = new RedockState(window, window.DockState);
            }

            //invalid state
            if (!savedState.IsValid)
            {
                return false;
            }

            RedockState oldState;
            states.TryGetValue(window.DockState, out oldState);
            if (oldState != null)
            {
                oldStates.Add(oldState);
            }

            states[window.DockState] = savedState;

            return true;
        }

        #endregion

        #region Private Implementation

        /// <summary>
        /// Gets a <see cref="RedockState">RedockState</see> instance for the specified window and DockState.
        /// </summary>
        /// <param name="window">The window for which to look-up a state.</param>
        /// <param name="state">The DockState to look-up for.</param>
        /// <param name="remove">True to remove the redock state from the list, false otherwise.</param>
        /// <returns></returns>
        protected internal RedockState GetState(DockWindow window, DockState state, bool remove)
        {
            if (!this.CanOperate())
            {
                return null;
            }

            Dictionary<DockState, RedockState> states;
            this.redockStates.TryGetValue(window.Name, out states);

            if (states == null)
            {
                return null;
            }

            RedockState redockState;
            states.TryGetValue(state, out redockState);

            if (redockState != null && remove)
            {
                states.Remove(state);
            }

            return redockState;
        }

        /// <summary>
        /// Releases previous redock state.
        /// Current implementation simply notifies the referenced <see cref="DockTabStrip">DockTabStrip</see> that it is not a redock target anymore.
        /// </summary>
        /// <param name="oldState"></param>
        protected void RemovePreviousState(RedockState oldState)
        {
            DockTabStrip oldStrip = oldState.TargetStrip;
            if (oldStrip != null)
            {
                oldStrip.IsRedockTarget = false;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether clean-up is currently suspended.
        /// </summary>
        public bool IsCleanUpSuspended
        {
            get
            {
                return this.cleanUpSuspendCount > 0;
            }
        }

        /// <summary>
        /// Gets an array with all the states that are currently marked for clean-up.
        /// </summary>
        protected internal RedockState[] StatesForCleanup
        {
            get
            {
                return this.oldStates.ToArray();
            }
        }

        #endregion

        #region Fields

        private int cleanUpSuspendCount;
        private List<RedockState> oldStates;
        private Dictionary<string, Dictionary<DockState, RedockState>> redockStates;

        #endregion
    }
}
