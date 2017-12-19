using System;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>Represents a toggle button element. The toggle button supports two or three
    ///     states depending on the IsThreeState property.</para>
    /// 	<para>
    ///         The <see cref="RadToggleButton">RadToggleButton</see> class is a simple wrapper
    ///         for the RadToggleButtonElement class. All UI and logic functionality is
    ///         implemented in the RadToggleButtonElement class. The
    ///         <see cref="RadToggleButton">RadToggleButton</see> acts to transfer events to
    ///         and from its corresponding RadToggleButtonElement instance. The latter can be
    ///         nested in other telerik controls.
    ///     </para>
    /// </summary>
    public class RadToggleButtonElement : RadButtonElement
    {
        #region Dependency Properties

        public static RadProperty ToggleStateProperty = RadProperty.Register("ToggleState", typeof(ToggleState), typeof(RadToggleButtonElement),
            new RadElementPropertyMetadata(ToggleState.Off, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private static readonly object StateChangingEventKey;
        private static readonly object StateChangedEventKey;
        private static readonly object IsCheckedChangedKey;

        private bool isTreeState = false;

        #endregion

        #region Initialization

        /// <summary>Initializes a new instance of the RadToggleButtonElement class.</summary>
        static RadToggleButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ToggleButtonStateManagerFactory(), typeof(RadToggleButtonElement));
            new Themes.ControlDefault.ToggleButton().DeserializeTheme();

            RadToggleButtonElement.StateChangedEventKey = new object();
            RadToggleButtonElement.StateChangingEventKey = new object();
            RadToggleButtonElement.IsCheckedChangedKey = new object();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.fillPrimitive.Class = "ToggleButtonFill";
        }

        #endregion

        #region Events

        #region Routed events

        public static RoutedEvent ToggleStateChangingRoutedEvent = RadToggleButtonElement.RegisterRoutedEvent("ToggleStateChangingRoutedEvent", typeof(RadToggleButtonElement));
        public static RoutedEvent CheckedRoutedEvent = RadToggleButtonElement.RegisterRoutedEvent("CheckedRoutedEvent", typeof(RadToggleButtonElement));
        public static RoutedEvent Indeterminate = RadToggleButtonElement.RegisterRoutedEvent("Indeterminate", typeof(RadToggleButtonElement));
        public static RoutedEvent Unchecked = RadToggleButtonElement.RegisterRoutedEvent("Unchecked", typeof(RadToggleButtonElement));

        #endregion

        /// <summary>
        /// Occurs before the elements's state changes.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs before the elements's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangingEventHandler ToggleStateChanging
        {
            add
            {
                this.Events.AddHandler(RadToggleButtonElement.StateChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadToggleButtonElement.StateChangingEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the elements's state changes.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the elements's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangedEventHandler ToggleStateChanged
        {
            add
            {
                this.Events.AddHandler(RadToggleButtonElement.StateChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadToggleButtonElement.StateChangedEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the element's IsChecked has been changed.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the elemnts's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        [Obsolete("This event is obsolete. The ToggleStateChanged event must be used instead.")] // Skarlatov 1/10/2009
        public event EventHandler IsCheckedChanged
        {
            add
            {
                this.Events.AddHandler(RadToggleButtonElement.IsCheckedChangedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadToggleButtonElement.IsCheckedChangedKey, value);
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Raises the StateChanging event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanging(StateChangingEventArgs e)
        {
            if (e.NewValue == ToggleState.Indeterminate && !this.isTreeState)
            {
                e.Cancel = true;
            }

            StateChangingEventHandler handler1 = (StateChangingEventHandler)base.Events[RadToggleButtonElement.StateChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanged(StateChangedEventArgs e)
        {
            StateChangedEventHandler handler1 = (StateChangedEventHandler)base.Events[RadToggleButtonElement.StateChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the IsCheckedChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnIsCheckedChanged(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[RadToggleButtonElement.IsCheckedChangedKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnChecked()
        {
            RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadToggleButtonElement.CheckedRoutedEvent);
            this.RaiseRoutedEvent(this, args);
            this.OnToggleStateChanged(new StateChangedEventArgs(this.ToggleState));
        }

        protected virtual void OnUnchecked()
        {
            RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadToggleButtonElement.Unchecked);
            this.RaiseRoutedEvent(this, args);
            this.OnToggleStateChanged(new StateChangedEventArgs(this.ToggleState));
        }

        protected virtual void OnIndeterminate()
        {
            RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadToggleButtonElement.Indeterminate);
            this.RaiseRoutedEvent(this, args);
            this.OnToggleStateChanged(new StateChangedEventArgs(this.ToggleState));
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.OnToggle();
        }

        protected internal virtual void OnToggle()
        {
            if (this.isTreeState)
            {
                switch (this.ToggleState)
                {
                    case ToggleState.Off:
                        this.ToggleState = ToggleState.On;
                        break;

                    case ToggleState.On:
                        this.ToggleState = ToggleState.Indeterminate;
                        break;

                    case ToggleState.Indeterminate:
                        this.ToggleState = ToggleState.Off;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (this.ToggleState)
                {
                    case ToggleState.Off:
                        this.ToggleState = ToggleState.On;
                        break;

                    case ToggleState.On:
                        this.ToggleState = ToggleState.Off;
                        break;
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ToggleStateProperty)
            {
                switch (this.ToggleState)
                {
                    case ToggleState.Off:
                        this.OnUnchecked();
                        break;

                    case ToggleState.On:
                        this.OnChecked();
                        break;

                    case ToggleState.Indeterminate:
                        this.OnIndeterminate();
                        break;

                    default:
                        this.OnIndeterminate();
                        break;
                }

                base.OnNotifyPropertyChanged("ToggleState");
                base.OnNotifyPropertyChanged("IsChecked");

                foreach (RadElement element in this.ChildrenHierarchy)
                {
                    element.SetValue(ToggleStateProperty, e.NewValue);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the <see cref="Telerik.WinControls.Enumerations.ToggleState">toggle
        ///     state</see>. Toggle state enumeration defines the following values: Off,
        ///     Indeterminate, and On.
        /// </summary>
        [Browsable(true), Bindable(true),
        Category(RadDesignCategory.AppearanceCategory),
        RadPropertyDefaultValue("ToggleState", typeof(RadToggleButtonElement)),
        Description("Gets or sets the toggle state. Toggle state enumeration defines the following values: Off, Indeterminate, and On.")]
        public ToggleState ToggleState
        {
            get
            {
                return (ToggleState)base.GetValue(RadToggleButtonElement.ToggleStateProperty);
            }
            set
            {
                StateChangingEventArgs eventArgs = new StateChangingEventArgs(this.ToggleState, value, false);
                this.OnToggleStateChanging(eventArgs);
                if (eventArgs.Cancel)
                    return;
                base.SetValue(RadToggleButtonElement.ToggleStateProperty, value);
            }
        }
        /// <summary>Gets or sets a value indicating whether the button is checked.</summary>
        [Browsable(false), Bindable(true), RefreshProperties(RefreshProperties.All),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets a value indicating whether the button is checked.")]
        [Obsolete("This property is obsolete. The ToggleState property must be used instead.")] // Skarlatov 1/10/2009
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsChecked
        {
            get
            {
                return (this.ToggleState != ToggleState.Off);
            }
            set
            {
                if (value != this.IsChecked)
                {
                    this.ToggleState = value ? ToggleState.On : ToggleState.Off;
                }

                OnIsCheckedChanged(EventArgs.Empty);

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toggle button has three or two
        /// states.
        /// </summary>
        [Browsable(true), Bindable(true),
        Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(false),
        Description("Gets or sets a value indicating whether the toggle button has three or two states.")]
        public bool IsThreeState
        {
            get
            {
                return this.isTreeState;
            }
            set
            {
                this.isTreeState = value;
            }
        }
        #endregion

        #region ISupportSystemSkin

        public override VisualStyleElement GetXPVisualStyle()
        {
            if (!Enabled)
            {
                return VisualStyleElement.Button.PushButton.Disabled;
            }

            if (IsMouseDown)
            {
                return this.IsMouseOver ? VisualStyleElement.Button.PushButton.Pressed : VisualStyleElement.Button.PushButton.Hot;
            }

            if (IsMouseOver)
            {
                return VisualStyleElement.Button.PushButton.Hot;
            }

            ToggleState state = this.ToggleState;
            bool isChecked = state == ToggleState.On || state == ToggleState.On;

            return isChecked ? VisualStyleElement.Button.PushButton.Pressed : VisualStyleElement.Button.PushButton.Normal;
        }

        #endregion
    }
}
