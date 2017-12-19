using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Interfaces;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents abstact class that provides service capabilities.
    /// </summary>
    public abstract class RadService : RadObject
    {
        #region Fields

        private bool enabled;
        private RadServiceState state;
        private object context;

        private static readonly object StartingEventKey;
        private static readonly object StartedEventKey;
        private static readonly object StoppingEventKey;
        private static readonly object StoppedEventKey;
        private static readonly object EventPropertyChanging;
        private static readonly object EventPropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RadService class.
        /// </summary>
        protected RadService()
        {
            this.state = RadServiceState.Initial;
            this.enabled = true;
        }

        static RadService()
        {
            StartingEventKey = new object();
            StartedEventKey = new object();
            StoppingEventKey = new object();
            StoppedEventKey = new object();
            EventPropertyChanging = new object();
            EventPropertyChanged = new object();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the service is operational and may perform actions.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanOperate()
        {
            return this.enabled;
        }

        /// <summary>
        /// Starts the Service.
        /// If the service was previously paused, it should be re-started with the Resume method.
        /// </summary>
        /// <param name="context">A context passed to the service.</param>
        public void Start(object context)
        {
            if (!this.CanStart(context))
            {
                return;
            }

            RadServiceStartingEventArgs e = new RadServiceStartingEventArgs(context);
            this.OnStarting(e);
            if (e.Cancel)
            {
                return;
            }

            this.SetContext(context);
            this.state = RadServiceState.Working;
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
            if (!(this.state == RadServiceState.Paused || this.state == RadServiceState.Working))
            {
                return;
            }

            RadServiceStoppingEventArgs e = new RadServiceStoppingEventArgs(commit);
            this.OnStopping(e);
            if (e.Cancel)
            {
                return;
            }

            this.state = RadServiceState.Stopped;
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

            if (this.state == RadServiceState.Working)
            {
                this.state = RadServiceState.Paused;
                this.PerformPause();
            }
        }

        /// <summary>
        /// Resumes previously paused operation.
        /// </summary>
        public void Resume()
        {
            //may not resume if disabled or currently working
            if (!this.Enabled || this.state == RadServiceState.Working)
            {
                return;
            }

            if (this.state == RadServiceState.Paused)
            {
                this.state = RadServiceState.Working;
                this.PerformResume();
            }
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
            if (!this.enabled)
            {
                return false;
            }

            if (this.IsDesignMode && !this.AvailableAtDesignTime)
            {
                return false;
            }

            if (!this.IsContextValid(context))
            {
                throw new InvalidOperationException("Invalid context for service " + this.ToString());
            }

            return this.state == RadServiceState.Initial || this.state == RadServiceState.Stopped;
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
        protected virtual void OnStarting(RadServiceStartingEventArgs e)
        {
            EventHandler<RadServiceStartingEventArgs> eh = this.Events[StartingEventKey] as EventHandler<RadServiceStartingEventArgs>;
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
        protected virtual void OnStopping(RadServiceStoppingEventArgs e)
        {
            EventHandler<RadServiceStoppingEventArgs> eh = this.Events[StoppingEventKey] as EventHandler<RadServiceStoppingEventArgs>;
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
        public event EventHandler<RadServiceStartingEventArgs> Starting
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
        public event EventHandler<RadServiceStoppingEventArgs> Stopping
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
        public RadServiceState State
        {
            get
            {
                return this.state;
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
            }
        }

        /// <summary>
        /// Notifies for a change in the Enabled state.
        /// </summary>
        protected virtual void OnEnabledChanged()
        {
            if (this.state == RadServiceState.Working || this.state == RadServiceState.Paused)
            {
                if (!this.Enabled)
                {
                    this.Stop(false);
                }
            }
        }

        #endregion

    }
}
