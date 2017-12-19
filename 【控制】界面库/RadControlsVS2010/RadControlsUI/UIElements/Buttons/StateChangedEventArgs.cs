using System;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadToggleButtonElement.ToggleStateChanging">ToggleStateChanging</see>
    ///     event.
    /// </summary>
    /// <param name="sender">Represents the event sender.</param>
    /// <param name="args">Represents the <see cref="StateChangingEventArgs">event arguments</see>.</param>
    public delegate void StateChangingEventHandler(object sender, StateChangingEventArgs args);

    /// <summary>
    ///     Represents event data of the
    ///     <see cref="RadToggleButtonElement.ToggleStateChanging">ToggleStateChanging</see>
    ///     event.
    /// </summary>
    public class StateChangingEventArgs : CancelEventArgs
    {
        private ToggleState oldValue;
        private ToggleState newValue;
        private bool canceled;
        /// <summary>
        /// Gets or sets the old toggle state.
        /// </summary>
        public ToggleState OldValue
        {
            get { return this.oldValue; }
        }
        /// <summary>
        /// Gets or sets the new toggle state.
        /// </summary>
        public ToggleState NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }
        /// <summary>
        /// If set to false, the event is not handled. 
        /// </summary>
        [Obsolete("This property will be removed in the next version. The Cancel property should be used instead.")]
        public bool Canceled
        {
            get { return this.Cancel; }
            set { this.Cancel = value; }
        }
        /// <summary>
        /// Initializes a new instance of the StateChangingEventArgs class using the old toggle state, the new toggle state and  
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="canceled"></param>
        public StateChangingEventArgs(ToggleState oldValue, ToggleState newValue, bool canceled)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.canceled = canceled;
        }
    }
    /// <summary>
    ///     Represents event data of the
    ///     <see cref="RadToggleButtonElement.ToggleStateChanged">ToggleStateChanged</see>.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        private ToggleState state;
        /// <summary>
        /// Gets the toggle state Off, On, or Indeterminate
        /// </summary>
        public ToggleState ToggleState
        {
            get
            {
                return this.state;
            }
        }
        /// <summary>
        /// Initializes a new instance of the StateChangedEventArgs class.
        /// </summary>
        /// <param name="state"></param>
        public StateChangedEventArgs(ToggleState state)
        {
            this.state = state;
        }
    }
    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadToggleButtonElement.ToggleStateChanged">ToggleStateChanged</see>
    ///     event.
    /// </summary>
    /// <param name="sender">Represents the event sender.</param>
    /// <param name="args">Represents the <see cref="StateChangedEventArgs">event arguments</see>.</param>
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs args);
}
