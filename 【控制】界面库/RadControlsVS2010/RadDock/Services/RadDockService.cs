using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines base for all services registered with a RadDock instance.
    /// </summary>
    public abstract class RadDockService : RadDockObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="RadDockService">RadDockService</see> instance.
        /// </summary>
        public RadDockService()
        {
            this.enabled = true;
        }

        #endregion

        #region Overriables

        /// <summary>
        /// The service gets notified that its current dock manager has changed.
        /// </summary>
        protected virtual void OnDockManagerChanged()
        {
        }

        /// <summary>
        /// Notifies for a change in the Enabled state.
        /// </summary>
        protected virtual void OnEnabledChanged()
        {
        }

        /// <summary>
        /// Determines whether the service is operational and may perform actions.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanOperate()
        {
            if (!this.enabled)
            {
                return false;
            }

            if (this.dockManager == null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the associated RadDock instance (if any) is in design mode.
        /// </summary>
        protected bool DockManagerDesignMode
        {
            get
            {
                if (this.dockManager == null)
                {
                    return false;
                }

                return this.dockManager.Site != null && this.dockManager.Site.DesignMode;
            }
        }

        /// <summary>
        /// Gets the RadDock instance this service is registered with.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDock DockManager
        {
            get
            {
                return this.dockManager;
            }
            internal set
            {
                if (this.dockManager == value)
                {
                    return;
                }

                this.dockManager = value;
                this.OnDockManagerChanged();
            }
        }

        /// <summary>
        /// Determines whether the Service is enabled (may be started).
        /// If the Service is working and its is disabled, it will end its current operation.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                this.OnEnabledChanged();
                this.OnPropertyChanged("Enabled");
            }
        }

        #endregion

        #region Fields

        private RadDock dockManager;
        private bool enabled;

        #endregion
    }
}
