using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents an object that stores information about what was a docking window's previous state.
    /// </summary>
    public class RedockState : RadDockObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="RedockState">RedockState</see> for the specified window and desired DockState.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="state"></param>
        public RedockState(DockWindow window, DockState state)
        {
            this.targetState = state;
            this.tabIndex = -1;
            this.dockWindow = new WeakReference(window);
            this.Initialize();
        }

        private void Initialize()
        {
            DockWindow window = this.DockWindow;
            if (window == null)
            {
                return;
            }

            DockTabStrip tabStrip = (DockTabStrip)window.TabStrip;
            //no strip, state could not be saved
            if (tabStrip == null)
            {
                return;
            }

            Debug.Assert(tabStrip != null, "Not a valid tabstrip");
            tabStrip.IsRedockTarget = true;
            this.tabIndex = tabStrip.TabPanels.IndexOf(window);

            this.sizeInfo = new SplitPanelSizeInfo(tabStrip.SizeInfo);
            this.targetStrip = new WeakReference(tabStrip);
        }

        internal void Reset()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the state is valid (the information needed for the state to be restored later on is valid).
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return this.DockWindow != null && this.TargetStrip != null && this.TargetStrip.IsRedockTarget;
            }
        }

        /// <summary>
        /// Gets the redock position this state targets.
        /// </summary>
        public DockState TargetState
        {
            get
            {
                return this.targetState;
            }
        }

        /// <summary>
        /// Gets the redock state provider.
        /// </summary>
        public DockWindow DockWindow
        {
            get
            {
                if (this.dockWindow.IsAlive)
                {
                    DockWindow window = (DockWindow)this.dockWindow.Target;
                    if (!(window.IsDisposed || window.Disposing))
                    {
                        return window;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the target strip for the redock operation.
        /// </summary>
        public DockTabStrip TargetStrip
        {
            get
            {
                if (targetStrip == null)
                {
                    return null;
                }

                if (this.targetStrip.IsAlive)
                {
                    DockTabStrip strip = (DockTabStrip)this.targetStrip.Target;
                    if (strip != null && !(strip.IsDisposed || strip.Disposing))
                    {
                        return strip;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the recorded size info.
        /// </summary>
        public SplitPanelSizeInfo SizeInfo
        {
            get
            {
                return this.sizeInfo;
            }
        }

        /// <summary>
        /// Gets the index of the state provider on the redock target TabStrip.
        /// </summary>
        public int TabIndex
        {
            get
            {
                return this.tabIndex;
            }
        }

        #endregion

        #region Fields

        private SplitPanelSizeInfo sizeInfo;
        private WeakReference dockWindow;
        private WeakReference targetStrip;
        private int tabIndex;
        private DockState targetState;

        #endregion
    }
}
