using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a service that is state-based. E.g. it may start, perform some action and stop.
    /// </summary>
    public abstract class StateService : RadDockService
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StateService()
        {
            this.state = ServiceState.Initial;
        }

        static StateService()
        {
            StartingEventKey = new object();
            StartedEventKey = new object();
            StoppingEventKey = new object();
            StoppedEventKey = new object();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the Service.
        /// If the service was previously paused, it should be re-started with the Resume method.
        /// </summary>
        public void Start(object context)
        {
            //check whether we may start the service
            if (!this.CanStart(context))
            {
                return;
            }

            StateServiceStartingEventArgs e = new StateServiceStartingEventArgs(context);
            this.OnStarting(e);
            if (e.Cancel)
            {
                return;
            }

            this.SetContext(context);
            this.state = ServiceState.Working;
            this.PerformStart();
            this.OnStarted();
        }

        /// <summary>
        /// Stops currently working or previously stopped service.
        /// </summary>
        /// <param name="commit">True to indicate that current operation ended successfully, false otherwise.</param>
        public void Stop(bool commit)
        {
            //the service is not started, do nothing
            if (!(this.state == ServiceState.Paused || this.state == ServiceState.Working))
            {
                return;
            }

            StateServiceStoppingEventArgs e = new StateServiceStoppingEventArgs(commit);
            this.OnStopping(e);
            if (e.Cancel)
            {
                return;
            }

            this.state = ServiceState.Stopped;
            //use the Commit member of the event arguments as the user may have changed it.
            if (e.Commit)
            {
                this.Commit();
            }
            else
            {
                this.Abort();
            }
            this.PerformStop();
            this.SetContext(null);
            this.OnStopped();
        }

        /// <summary>
        /// Pauses a currently running operation.
        /// </summary>
        public void Pause()
        {
            //may not pause if not enabled
            if (!this.Enabled)
            {
                return;
            }

            if (this.state == ServiceState.Working)
            {
                this.state = ServiceState.Paused;
                this.PerformPause();
            }
        }

        /// <summary>
        /// Resumes previously paused operation.
        /// </summary>
        public void Resume()
        {
            //may not resume if disabled or currently working
            if (!this.Enabled || this.state == ServiceState.Working)
            {
                return;
            }

            if (this.state == ServiceState.Paused)
            {
                this.state = ServiceState.Working;
                this.PerformResume();
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Provides additional processing for a change in the Enabled state.
        /// </summary>
        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();

            if (this.state == ServiceState.Working || this.state == ServiceState.Paused)
            {
                if (!this.Enabled)
                {
                    this.Stop(false);
                }
            }
        }

        /// <summary>
        /// Provides additional processing when a change in the owning RadDock instance occurs.
        /// </summary>
        protected override void OnDockManagerChanged()
        {
            base.OnDockManagerChanged();

            this.Stop(false);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines whether the service may be started.
        /// Validation is as follows:
        /// 1. Check whether Enabled is true.
        /// 2. Check the context through IsContextValid method. An exception is thrown if context is invalid.
        /// 3. Checks the current state - it should be Initial or Stopped.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanStart(object context)
        {
            if (!this.Enabled || this.DockManager == null)
            {
                return false;
            }

            //do not allow service at design-time
            if (this.DockManagerDesignMode && !this.AvailableAtDesignTime)
            {
                return false;
            }

            if (!this.IsContextValid(context))
            {
                throw new InvalidOperationException("Invalid context for service " + this.ToString());
            }

            return this.state == ServiceState.Initial || this.state == ServiceState.Stopped;
        }

        /// <summary>
        /// Notifies that the service has been successfully started.
        /// Allows inheritors to perform some additional logic upon start.
        /// </summary>
        protected virtual void OnStarted()
        {
            EventHandler eh = this.Events[StartedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies that a start request has occured. Cancelable.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStarting(StateServiceStartingEventArgs e)
        {
            StateServiceStartingEventHandler eh = this.Events[StartingEventKey] as StateServiceStartingEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Notifies that a running operation has stopped.
        /// Allows inheritors to perform some additional logic upon stop.
        /// </summary>
        protected virtual void OnStopped()
        {
            EventHandler eh = this.Events[StoppedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies that a stop request has occured. Cancelable.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStopping(StateServiceStoppingEventArgs e)
        {
            StateServiceStoppingEventHandler eh = this.Events[StoppingEventKey] as StateServiceStoppingEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Evaluates the provided context. Some services may not operate without certain context provided.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool IsContextValid(object context)
        {
            return true;
        }

        /// <summary>
        /// Performs the core Start logic.
        /// </summary>
        protected virtual void PerformStart()
        {
        }

        /// <summary>
        /// Stops the service. Performs the core logic.
        /// </summary>
        protected virtual void PerformStop()
        {
        }

        /// <summary>
        /// Aborts the current operation without applying any changes.
        /// </summary>
        protected virtual void Abort()
        {
        }

        /// <summary>
        /// Ends the current operation and applies all changes.
        /// </summary>
        protected virtual void Commit()
        {
        }

        /// <summary>
        /// Performs the core Resume logic.
        /// </summary>
        protected virtual void PerformResume()
        {
        }

        /// <summary>
        /// Performs the core Pause logic.
        /// </summary>
        protected virtual void PerformPause()
        {
        }

        /// <summary>
        /// Sets the provided object as the current context.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void SetContext(object context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the context associated with the current operation.
        /// This member is valid only while the Service is started or paused.
        /// </summary>
        public object Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the service is about to be started.
        /// </summary>
        public event StateServiceStartingEventHandler Starting
        {
            add
            {
                this.Events.AddHandler(StartingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(StartingEventKey, value);
            }
        }

        /// <summary>
        /// Raised right after the service is started.
        /// </summary>
        public event EventHandler Started
        {
            add
            {
                this.Events.AddHandler(StartedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(StartedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the service is about to be stopped.
        /// </summary>
        public event StateServiceStoppingEventHandler Stopping
        {
            add
            {
                this.Events.AddHandler(StoppingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(StoppingEventKey, value);
            }
        }

        /// <summary>
        /// Raised when the service is stopped.
        /// </summary>
        public event EventHandler Stopped
        {
            add
            {
                this.Events.AddHandler(StoppedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(StoppedEventKey, value);
            }
        }


        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the service is available at design-time. False by default.
        /// </summary>
        public virtual bool AvailableAtDesignTime
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current state of the service.
        /// </summary>
        public ServiceState State
        {
            get
            {
                return this.state;
            }
        }

        #endregion

        #region Fields

        private object context;
        private ServiceState state;

        private static readonly object StartingEventKey;
        private static readonly object StartedEventKey;
        private static readonly object StoppingEventKey;
        private static readonly object StoppedEventKey;

        #endregion
    }
}
