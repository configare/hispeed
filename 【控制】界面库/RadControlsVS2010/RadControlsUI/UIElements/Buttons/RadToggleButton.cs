using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>Represents a RadToggleButton. A ToggleButton may have the following states:
    ///     On, Off, and Indeterminate. The button may have only the first two states if the
    ///     IsThreeState property is set to false.</para>
    /// 	<para>
    ///         The RadToggleButton class is a simple wrapper for the
    ///         <see cref="RadToggleButtonElement">RadToggleButtonElement</see>. All UI and
    ///         logic functionality is implemented in the
    ///         <see cref="RadToggleButtonElement">RadToggleButtonElement</see> class. The
    ///         latter can be nested in other telerik controls. RadToggleButton acts to
    ///         transfer events to and from the its corresponding
    ///         <see cref="RadToggleButtonElement">RadToggleButtonElement</see> instance.
    ///     </para>
    /// </summary>
    [RadThemeDesignerData(typeof(RadToggleButtonStyleBuilderData))]
    [ToolboxItem(true)]
    [DefaultProperty("Text"), DefaultBindingProperty("Text"), DefaultEvent("ToggleStateChanged")]
    [Description("Extends the functionality of a checkbox by adding state management")]
    public class RadToggleButton : RadButtonBase
    {
        #region Static Fields

        private static readonly object StateChangingEventKey;
        private static readonly object StateChangedEventKey;
        private static readonly object IsCheckedChangedKey;

        #endregion

        #region Initialization

        /// <summary>Initializes a new instance of the RadToggleButton class.</summary>
        static RadToggleButton()
        {
            StateChangingEventKey = new object();
            StateChangedEventKey = new object();
            IsCheckedChangedKey = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadToggleButton"/> class.
        /// </summary>
        public RadToggleButton()
        {
        }

        /// <summary>
        /// Create main button element that is specific for RadToggleButton.
        /// </summary>
        /// <returns>The element that encapsulates the funtionality of RadToggleButton</returns>
        protected override RadButtonElement CreateButtonElement()
        {
            RadToggleButtonElement res = new RadToggleButtonElement();
            res.UseNewLayoutSystem = true;
            res.ToggleStateChanging += new StateChangingEventHandler(ButtonElement_ToggleStateChanging);
            res.ToggleStateChanged += new StateChangedEventHandler(ButtonElement_ToggleStateChanged);
            res.PropertyChanged += new PropertyChangedEventHandler(res_PropertyChanged);
            return res;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadToggleButtonAccessibleObject(this);
        }

        #endregion

        #region Properties

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 23);
            }
        }

        /// <summary>
        /// Gets the instance of RadToggleButtonElement wrapped by this control. RadToggleButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadToggleButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RadToggleButtonElement ButtonElement
        {
            get { return (RadToggleButtonElement)base.ButtonElement; }
        }

        /// <commentsfrom cref="RadToggleButtonElement.ToggleState" filter=""/>
        [Browsable(true), Bindable(true),
        RadDefaultValue("ToggleState", typeof(RadToggleButtonElement)),
        Description("Gets or sets the toggle state. Toggle state enumeration defines the following values: Off, Indeterminate, and On."),
        Category(RadDesignCategory.AppearanceCategory)]
        public ToggleState ToggleState
        {
            get
            {
                return this.ButtonElement.ToggleState;
            }
            set
            {
                this.ButtonElement.ToggleState = value;
            }
        }

        /// <summary>Gets or sets a boolean value indicating where the button is checked.</summary>
        [Browsable(true), Bindable(true), RefreshProperties(RefreshProperties.All),
        RadDefaultValue("IsChecked", typeof(RadToggleButtonElement)),
        Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsChecked
        {
            get
            {
                return this.ButtonElement.ToggleState == ToggleState.On;
            }
            set
            {
                this.ButtonElement.ToggleState = value ? ToggleState.On : ToggleState.Off;
            }
        }

        /// <summary>Gets or sets a boolean value indicating where the button is checked.</summary>
        [Browsable(true), Bindable(true),
        RadDefaultValue("IsThreeState", typeof(RadToggleButtonElement)),
        Category(RadDesignCategory.BehaviorCategory)]
        public bool IsThreeState
        {
            get
            {
                return this.ButtonElement.IsThreeState;
            }
            set
            {
                this.ButtonElement.IsThreeState = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Occurs when the elements's state is changing.</summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs before the elements's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangingEventHandler ToggleStateChanging
        {
            add
            {
                this.Events.AddHandler(RadToggleButton.StateChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadToggleButton.StateChangingEventKey, value);
            }
        }
        /// <summary>
        /// Raises the StateChanging event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanging(StateChangingEventArgs e)
        {
            StateChangingEventHandler handler1 = (StateChangingEventHandler)this.Events[RadToggleButton.StateChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the element's state changes.  
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the elemnts's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangedEventHandler ToggleStateChanged
        {
            add
            {
                this.Events.AddHandler(RadToggleButton.StateChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadToggleButton.StateChangedEventKey, value);
            }
        }
        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanged(StateChangedEventArgs e)
        {
            StateChangedEventHandler handler1 = (StateChangedEventHandler)this.Events[RadToggleButton.StateChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        private void res_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                base.OnNotifyPropertyChanged("IsChecked");
            }
        }

        private void ButtonElement_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            this.OnToggleStateChanging(args);
        }

        private void ButtonElement_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            this.OnToggleStateChanged(args);
        }

        #endregion
    }
    ///<exclude/>
    /// <summary>Represents helper class used by the Visual Style Builder.</summary>
    public class RadToggleButtonStyleBuilderData : RadControlDesignTimeData
    {
        public RadToggleButtonStyleBuilderData()
        { }

        public RadToggleButtonStyleBuilderData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadToggleButton button = new RadToggleButton();
            button.AutoSize = false;
            button.Size = new Size(120, 65);

            button.Text = "RadToggleButton";

            RadToggleButton buttonStructure = new RadToggleButton();
            button.AutoSize = true;

            buttonStructure.Text = "RadToggleButton";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }

    }
}
