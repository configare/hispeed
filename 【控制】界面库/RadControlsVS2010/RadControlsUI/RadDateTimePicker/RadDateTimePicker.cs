using System;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;
using System.Drawing;
using System.Globalization;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents the RadDateTimePicker class
	/// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [RadThemeDesignerData(typeof(RadDateTimePickerDesignTimeData))]
	[Description("Enables the user to select a date and time, and display the date and time in a specified format")]
	[DefaultBindingProperty("Value"), DefaultEvent("ValueChanged"), DefaultProperty("Value")]
    [ToolboxItem(true)]
	public class RadDateTimePicker : RadControl
	{
		private RadDateTimePickerElement dateTimePickerElement;
        private static readonly object EventKeyDown = new object();
        private static readonly object EventKeyPress = new object();
        private static readonly object EventKeyUp = new object();
        
		/// <summary>
		/// Represents the RadDateTimePicker constructor
		/// </summary>
		public RadDateTimePicker()
		{
            this.TabStop = false;
            this.AutoSize = true;
            this.SetStyle(ControlStyles.Selectable, true);
		}

		#region Events
		/// <summary>
		/// Occurs when the format of the control has changed
		/// </summary>
		[Description("Occurs when the value of the control has changed"), Category("Action")]
		public event EventHandler FormatChanged;

		/// <summary>
		/// Raises the FormatChanged event
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnFormatChanged(EventArgs e)
		{
			if (this.FormatChanged != null)
			{
				this.FormatChanged(this, e);
			}
		}

		/// <summary>
		/// Occurs when the value of the control has changed
		/// </summary>
		[Description("Occurs when the value of the control has changed"), Category("Action")]
		public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when the value of the control is changing
        /// </summary>
        [Description("Occurs when the value of the control is changing"), Category("Action")]
        public event ValueChangingEventHandler ValueChanging;

		/// <summary>
		/// Raises the ValueChanged event
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, e);
			}
		}

        /// <summary>
        /// Raises the ValueChanging event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, e);
            }
        }
		#endregion

		/// <summary>
		/// Gets the control's default size
		/// </summary>
		protected override Size DefaultSize
		{
			get
			{
				return new Size(150, 18);
			}
		}

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            if (this.dateTimePickerElement.GetCurrentBehavior() as RadDateTimePickerCalendar != null)
            {
                (this.dateTimePickerElement.GetCurrentBehavior() as RadDateTimePickerCalendar).Calendar.ThemeName = this.ThemeName;
                (this.dateTimePickerElement.GetCurrentBehavior() as RadDateTimePickerCalendar).PopupControl.ThemeName = this.ThemeName;
            }
        }

		/// <summary>
		/// Gets or sets the culture supported by this calendar.
		/// </summary>
		[Category("Localization Settings")]
		[Description("Gets or sets the culture supported by this calendar.")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(CultureInfoConverter))]
		[RefreshProperties(RefreshProperties.Repaint),
		Localizable(true)]
		public CultureInfo Culture
		{
			get
			{
				return this.dateTimePickerElement.Culture;
			}
			set
			{
				this.dateTimePickerElement.Culture = value;
			}
		}

        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

		public bool ShouldSerializeCulture()
		{
			if (String.IsNullOrEmpty(this.dateTimePickerElement.Culture.ToString()))
				return false;

			if (this.dateTimePickerElement.Culture.ToString() == "en-US")
				return false;

			return true;
		}

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element.GetType().Equals(typeof(RadMaskedEditBoxElement)))
            {
                return true;
            }
            
            return base.ControlDefinesThemeForElement(element);
        }

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			System.Windows.Forms.RightToLeft value = base.RightToLeft;
			if (value == RightToLeft.Inherit)
			{
				Control rtlSource = this.Parent;
				while (rtlSource != null && rtlSource.RightToLeft == RightToLeft.Inherit)
				{
					rtlSource = rtlSource.Parent;
				}
				value = (rtlSource != null) ? rtlSource.RightToLeft : RightToLeft.No;

				if (value == RightToLeft.Yes)
				{
					this.dateTimePickerElement.RightToLeft = true;
				}
				else
				{
					this.dateTimePickerElement.RightToLeft = false;
				}
			}
			if (value == RightToLeft.No)
			{
				this.dateTimePickerElement.RightToLeft = false;
			}
			else if (value == RightToLeft.Yes)
			{
				this.dateTimePickerElement.RightToLeft = true;
			}

			base.OnRightToLeftChanged(e);
		}

		public void ResetCulture()
		{
			this.dateTimePickerElement.Culture = new CultureInfo("en-US");
		}

		/// <summary>
		/// Gets the instance of RadDateTimePickerElement wrapped by this control. RadDateTimePickerElement
		/// is the main element in the hierarchy tree and encapsulates the actual functionality of RadDateTimePicker.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RadDateTimePickerElement DateTimePickerElement
		{
			get
			{
				return this.dateTimePickerElement;
			}
            set
            {
                this.dateTimePickerElement = value;
            }

		}

		/// <summary>
		/// Gets or sets the date/time value assigned to the control.
		/// </summary>
		[Bindable(true), RefreshProperties(RefreshProperties.All), Description("Gets or sets the date/time value assigned to the control."), Category("Behavior")]
		public virtual DateTime Value
		{
			get
			{
                DateTime? date = this.dateTimePickerElement.Value;
                if (date.HasValue)
                {
                    return date.Value;
                }

				return this.MinDate;
			}
			set
			{
				this.dateTimePickerElement.Value = value;
                this.OnNotifyPropertyChanged("Value");
			}
		}

        /// <summary>
        /// Gets or sets the date/time value assigned to the control.
        /// </summary>
        [Bindable(true),
        RefreshProperties(RefreshProperties.All),
        Description("Gets or sets the date/time value assigned to the control."),
        Category("Behavior"),
        Browsable(false)]
        public virtual DateTime? NullableValue
        {
            get
            {
                return this.dateTimePickerElement.Value;
            }
            set
            {
                this.dateTimePickerElement.Value = value;
                this.OnNotifyPropertyChanged("NullableValue");
            }
        }

		/// <summary>
		/// Gets or sets the format of the date and time displayed in the control.
		/// </summary>
		[DefaultValue(typeof(DateTimePickerFormat), "DateTimePickerFormat.Long")]
		[Description("Gets or sets the format of the date and time displayed in the control."), RefreshProperties(RefreshProperties.Repaint), Category("Appearance")]
		public virtual DateTimePickerFormat Format
		{
			get
			{
				return this.dateTimePickerElement.Format;
			}
			set
			{
				this.dateTimePickerElement.Format = value;
                this.OnNotifyPropertyChanged("Format");            
    		}
		}

		/// <summary>
		/// Indicates whether a check box is displayed in the control. When the check box is unchecked no value is selected
		/// </summary>
		[Description("Indicates whether a check box is displayed in the control. When the check box is unchecked no value is selected")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool ShowCheckBox
		{
			get
			{
				return this.dateTimePickerElement.ShowCheckBox;
			}
			set
			{
				this.dateTimePickerElement.ShowCheckBox = value;
			}
		}

		/// <summary>
		/// Gets or sets the custom date/time format string.
		/// </summary>
		[DefaultValue((string)null), RefreshProperties(RefreshProperties.Repaint), Description("Gets or sets the custom date/time format string."), Localizable(true), Category("Behavior")]
		public string CustomFormat
		{
			get
			{
				return this.dateTimePickerElement.CustomFormat;
			}
			set
			{
				this.dateTimePickerElement.CustomFormat = value;
			}
		}

		/// <summary>
		/// When ShowCheckBox is true, determines that the user has selected a value
		/// </summary>
		[Description("When ShowCheckBox is true, determines that the user has selected a value")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Checked
		{
			get
			{
				return this.dateTimePickerElement.Checked;
			}
			set
			{
				this.dateTimePickerElement.Checked = value;
			}
		}

		/// <summary>
		/// Gets or sets the minimum date and time that can be selected in the control.
		/// </summary>
		[Description("Gets or sets the minimum date and time that can be selected in the control."), Category("Behavior")]
		public DateTime MinDate
		{
			get
			{
				return this.dateTimePickerElement.MinDate;
			}
			set
			{
				this.dateTimePickerElement.MinDate = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum date and time that can be selected in the control.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets the maximum date and time that can be selected in the control.")]
		public DateTime MaxDate
		{
			get
			{
				return this.dateTimePickerElement.MaxDate;
			}
			set
			{
				this.dateTimePickerElement.MaxDate = value;
			}
		}

        /// <summary>
        /// Gets or sets the location of the drop down showing the calendar
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Browsable(false)]
        [Description("Gets or sets the location of the drop down showing the calendar")]
        [Category("Behavior")]
        public Point CalendarLocation
        {
            get
            {
                return this.dateTimePickerElement.CalendarLocation;
            }
            set
            {
                this.dateTimePickerElement.CalendarLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the calendar in the drop down
        /// </summary>
        [DefaultValue(typeof(Size), "100, 156")]
        [Browsable(false)]
        [Description("Gets or sets the size of the calendar in the drop down")]
        [Category("Behavior")]
        public Size CalendarSize
        {
            get
            {
                return this.dateTimePickerElement.CalendarSize;
            }
            set
            {
                this.dateTimePickerElement.CalendarSize = value;
            }
        }


        /// <summary>
        /// The DateTime value assigned to the date picker when the Value is null
        /// </summary>
        [Category("Data"), Description("The DateTime value assigned to the date picker when the Value is null"), Bindable(false), DefaultValue(0)]
        public DateTime NullDate
        {
            get
            {
                return this.dateTimePickerElement.NullDate;
            }
            set
            {
                this.dateTimePickerElement.NullDate = value;
            }
        }

        /// <summary>
        /// Sets the current value to behave as a null value
        /// </summary>
        public void SetToNullValue()
        {
            this.dateTimePickerElement.SetToNullValue();
        }

		/// <summary>
		/// Indicates whether a spin box rather than a drop down calendar is displayed for editing the control's value
		/// </summary>
		[Description("Indicates whether a spin box rather than a drop down calendar is displayed for editing the control's value")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool ShowUpDown
		{
			get
			{
				return this.dateTimePickerElement.ShowUpDown;
			}
			set
			{
				this.dateTimePickerElement.ShowUpDown = value;
			}
		}

		/// <summary>
		/// Gets or sets the text that is displayed when the DateTimePicker contains a null 
		/// reference.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory),
		Localizable(true),
		Description("Gets or sets the text that is displayed when the DateTimePicker contains a null reference")
		]
		public string NullText
		{
			get
			{
				return this.dateTimePickerElement.NullText;
			}

			set
			{
				this.dateTimePickerElement.NullText = value;
			}
		}

		public bool ShouldSerializeNullText()
		{
			if (String.IsNullOrEmpty(this.dateTimePickerElement.NullText))
				return false;

			return true;
		}

		public void ResetNullText()
		{
			this.dateTimePickerElement.NullText = "";
		}

        ///// <summary>
        ///// handles the delete key
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="keyData"></param>
        ///// <returns></returns>
        //protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        //{
        //    if (keyData == System.Windows.Forms.Keys.Delete)
        //    {
        //        this.dateTimePickerElement.Value = this.dateTimePickerElement.NullDate;

        //        return true;
        //    }

            
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

		/// <summary>
		/// Occurs when the drop down is opened
		/// </summary>
		[Description("Occurs when the drop down is opened")]
		[Category("Action")]
		public event EventHandler Opened;

		/// <summary>
		/// Occurs when the drop down is opening
		/// </summary>
		[Description("Occurs when the drop down is opening")]
		[Category("Action")]
		public event CancelEventHandler Opening;


		/// <summary>
		/// Occurs when the drop down is closing
		/// </summary>
		[Description("Occurs when the drop down is closing")]
		[Category("Action")]
		public event RadPopupClosingEventHandler Closing;

		/// <summary>
		/// Occurs when the drop down is closed
		/// </summary>
		[Description("Occurs when the drop down is closed")]
		[Category("Action")]
		public event RadPopupClosedEventHandler Closed;

		protected virtual void OnClosed(RadPopupClosedEventArgs args)
		{
			if (this.Closed != null)
			{
				this.Closed(this, args);
			}
		}

		protected virtual void OnOpened(EventArgs args)
		{
			if (this.Opened != null)
			{
				this.Opened(this, args);
			}
		}

		protected virtual void OnOpening(CancelEventArgs args)
		{
			if (this.Opening != null)
			{
				this.Opening(this, args);
			}
		}

		protected virtual void OnClosing(RadPopupClosingEventArgs args)
		{
			if (this.Closing != null)
			{
				this.Closing(this, args);
			}
		}

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }

        public virtual void OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyEventHandler KeyUp = (KeyEventHandler)base.Events[EventKeyUp];
            if (KeyUp != null)
            {
                KeyUp(sender, e);
            }
        }

        public virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEventHandler KeyPress = (KeyPressEventHandler)base.Events[EventKeyPress];
            if (KeyPress != null)
            {
                KeyPress(sender, e);
            }
        }

        public virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            KeyEventHandler KeyDown = (KeyEventHandler)base.Events[EventKeyDown];
            if (KeyDown != null)
            {
                KeyDown(sender, e);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key down
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public event KeyEventHandler KeyDown
        {
            add
            {
                base.Events.AddHandler(EventKeyDown, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyDown, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key")]
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.Events.AddHandler(EventKeyPress, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyPress, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user releases the pressed key up
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user releases the pressed key up")]
        public event KeyEventHandler KeyUp
        {
            add
            {
                base.Events.AddHandler(EventKeyUp, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventKeyUp, value);
            }
        }


        /// <summary>
        /// creates and initializes the RadDateTimePickerElement
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.dateTimePickerElement = new RadDateTimePickerElement();
            parent.Children.Add(this.dateTimePickerElement);

            this.dateTimePickerElement.FormatChanged += delegate(object sender, EventArgs e) { this.OnFormatChanged(e); };
            this.dateTimePickerElement.ValueChanged += delegate(object sender, EventArgs e) { this.OnValueChanged(e); };
            this.dateTimePickerElement.ValueChanging += delegate(object sender, ValueChangingEventArgs e) { this.OnValueChanging(e); };
            this.dateTimePickerElement.Opening += delegate(object sender, CancelEventArgs e) { this.OnOpening(e); };
            this.dateTimePickerElement.Opened += delegate(object sender, EventArgs e) { this.OnOpened(e); };
            this.dateTimePickerElement.Closing += delegate(object sender, RadPopupClosingEventArgs e) { this.OnClosing(e); };
            this.dateTimePickerElement.Closed += delegate(object sender, RadPopupClosedEventArgs e) { this.OnClosed(e); };

            this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyDown += new KeyEventHandler(OnKeyDown);
            this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyUp += new KeyEventHandler(OnKeyUp);
            this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyPress += new KeyPressEventHandler(OnKeyPress);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dateTimePickerElement != null)
                {
                    this.dateTimePickerElement.FormatChanged -= delegate(object sender, EventArgs e) { this.OnFormatChanged(e); };
                    this.dateTimePickerElement.ValueChanged -= delegate(object sender, EventArgs e) { this.OnValueChanged(e); };
                    this.dateTimePickerElement.ValueChanging -= delegate(object sender, ValueChangingEventArgs e) { this.OnValueChanging(e); };
                    this.dateTimePickerElement.Opening -= delegate(object sender, CancelEventArgs e) { this.OnOpening(e); };
                    this.dateTimePickerElement.Opened -= delegate(object sender, EventArgs e) { this.OnOpened(e); };
                    this.dateTimePickerElement.Closing -= delegate(object sender, RadPopupClosingEventArgs e) { this.OnClosing(e); };
                    this.dateTimePickerElement.Closed -= delegate(object sender, RadPopupClosedEventArgs e) { this.OnClosed(e); };

                    this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyDown -= new KeyEventHandler( this.OnKeyDown);
                    this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyUp -= new KeyEventHandler(this.OnKeyUp);
                    this.dateTimePickerElement.TextBoxElement.TextBoxItem.HostedControl.KeyPress -= new KeyPressEventHandler(this.OnKeyPress);
                    this.dateTimePickerElement.Dispose();
                    this.dateTimePickerElement.DisposeChildren();
                    this.dateTimePickerElement = null;
                }
            }

            base.Dispose(disposing);
        }

        

        //private void HostedControl_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    this.OnKeyPress(e);    
        //}

        //private void HostedControl_KeyUp(object sender, KeyEventArgs e)
        //{
        //    this.OnKeyUp(e);
        //}

        //private void HostedControl_KeyDown(object sender, KeyEventArgs e)
        //{
        //    this.OnKeyDown(e);
        //}

        #region Focus management
        private bool entering = false;
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (!entering)
            {
                entering = true;
                this.dateTimePickerElement.TextBoxElement.TextBoxItem.TextBoxControl.Focus();
                this.OnGotFocus(e);
                entering = false;
            }
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.OnLostFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!entering)
            {
                base.OnLostFocus(e);
            }
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadDateTimePickerAccessibleObject(this);
        }
    }
}
