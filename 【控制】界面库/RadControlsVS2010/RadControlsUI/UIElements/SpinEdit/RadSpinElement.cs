using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a numeric up/dowm element. The <see cref="RadCheckBox">RadSpinEditor</see>
    ///     class is a simple wrapper for the numeric up/dowm element class. The
    ///     <see cref="RadCheckBox">RadSpinEdit</see> acts to transfer events to and from its
    ///     correspondingnumeric up/dowm element instance. The numeric up/dowm element which is
    ///     essentially the <see cref="RadCheckBox">numeric up/dowm element</see> control may be nested in
    ///     other telerik controls.
    /// </summary>
    public class RadSpinElement : RadEditorElement
    {
        #region BitState Keys

        internal const ulong SuppressEditorStateKey = RadItemLastStateKey << 1;
        internal const ulong TextValueChangedStateKey = SuppressEditorStateKey << 1;
        internal const ulong InterceptArrowKeysStateKey = TextValueChangedStateKey << 1;
        internal const ulong ThousandsSeparatorStateKey = InterceptArrowKeysStateKey << 1;
        internal const ulong HexadecimalStateKey = ThousandsSeparatorStateKey << 1;
        internal const ulong ShowUpDownButtonsStateKey = HexadecimalStateKey << 1;

        #endregion

        #region Fields

        // Active elements (textbox, buttons, etc.)
        private RadTextBoxItem textItem;
        private RadRepeatArrowElement buttonUp;
        private RadRepeatArrowElement buttonDown;


        // Visuals (borders, fills, etc.)
        protected BorderPrimitive border;
        private FillPrimitive textBoxFillPrimitive;

        // Additional
        private DockLayoutPanel dockLayout;
        private BoxLayout stackLayout;

        // Values
        protected decimal value = 0m;
        protected decimal defaultValue = 0m;
        protected decimal oldValue = 0m;

        protected decimal minValue = 0;
        protected decimal maxValue = 100;
        protected decimal step = 1;

        private int decimalPlaces;
        protected string errorDescription = string.Empty;
        private bool wrap;
        private bool rightMouseButtonReset;

        #endregion

        #region Constructors and Initialization

        static RadSpinElement()
        {
            new Themes.ControlDefault.SpinEdit().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[InterceptArrowKeysStateKey] = true;
            this.BitState[ShowUpDownButtonsStateKey] = true;
        }

        /// <summary>
        /// create child elements
        /// </summary>
        protected override void CreateChildElements()
        {
            this.textItem = new RadTextBoxItem();
            this.SetSpinValue(this.value, true);

            this.textItem.TextChanging += new TextChangingEventHandler(textItem_TextChanging);
            this.textItem.TextChanged += new EventHandler(textItem_TextChanged);
            this.textItem.LostFocus += new EventHandler(textItem_LostFocus);
            this.textItem.KeyPress += new KeyPressEventHandler(textItem_KeyPress);
            this.textItem.KeyDown += new KeyEventHandler(textItem_KeyDown);
            this.textItem.KeyUp += new KeyEventHandler(textItem_KeyUp);
            this.textItem.HostedControl.MouseWheel += new MouseEventHandler(textItem_MouseWheel);
            this.textItem.HostedControl.MouseUp += new MouseEventHandler(HostedControl_MouseUp);
            this.textItem.Alignment = ContentAlignment.MiddleLeft;
            this.textItem.StretchHorizontally = true;
            this.textItem.StretchVertically = false;

            this.buttonUp = new RadRepeatArrowElement();
            this.buttonUp.ThemeRole = "UpButton";
            this.buttonUp.Class = "UpButton";
            //hack!!!!!!!!!!!!
            this.buttonUp.Padding = new Padding(1, 0, 3, 2);
            //this.buttonUp.Margin = new Padding(0, 1, 1, 0);
            //this.buttonUp.Border.Visibility = ElementVisibility.Collapsed;
            this.buttonUp.Click += new EventHandler(ButtonUp_Click);
            this.buttonUp.DoubleClick += new EventHandler(ButtonUp_Click);
            this.buttonUp.Direction = ArrowDirection.Up;
            this.buttonUp.Arrow.AutoSize = true;

            this.buttonDown = new RadRepeatArrowElement();
            this.buttonDown.ThemeRole = "DownButton";
            this.buttonDown.Class = "DownButton";
            //hack!!!!!!!!!!!!
            this.buttonDown.Padding = new Padding(1, 1, 3, 1);
            //this.buttonDown.Margin = new Padding(0, 0, 1, 1);
            //this.buttonDown.Border.Visibility = ElementVisibility.Collapsed;
            this.buttonDown.Click += new EventHandler(ButtonDown_Click);
            this.buttonDown.DoubleClick += new EventHandler(ButtonDown_Click);
            this.buttonDown.Arrow.AutoSize = true;
            this.buttonDown.Direction = ArrowDirection.Down;
            this.stackLayout = new BoxLayout();
            this.stackLayout.Orientation = Orientation.Vertical;
            BoxLayout.SetProportion(this.buttonUp, 1);
            BoxLayout.SetProportion(this.buttonDown, 1);
            this.stackLayout.Children.Add(this.buttonUp);
            this.stackLayout.Children.Add(this.buttonDown);

            this.textBoxFillPrimitive = new FillPrimitive();
            this.textBoxFillPrimitive.Class = "SpinElementFill";
            this.textBoxFillPrimitive.NumberOfColors = 1;
            this.textBoxFillPrimitive.MouseDown += delegate { this.textItem.Focus(); };
            this.Children.Add(this.textBoxFillPrimitive);
            //this.textBoxFillPrimitive.BackColor = this.textItem.BackColor;

            this.dockLayout = new DockLayoutPanel();
            this.dockLayout.LastChildFill = true;

            if (!this.RightToLeft)
                DockLayoutPanel.SetDock(stackLayout, Dock.Right);
            else
                DockLayoutPanel.SetDock(stackLayout, Dock.Left);

            DockLayoutPanel.SetDock(this.textBoxFillPrimitive, Dock.Bottom);
            DockLayoutPanel.SetDock(this.textItem, Dock.Top);

            dockLayout.Children.Add(stackLayout);
            dockLayout.Children.Add(this.textItem);

            this.border = new BorderPrimitive();
            this.border.Class = "SpinElementBorder";
            this.Children.Add(border);
            this.Children.Add(dockLayout);

            this.textItem.BindProperty(TextPrimitive.TextProperty, this, TextProperty, PropertyBindingOptions.TwoWay);

            this.textItem.Multiline = false;
            this.textItem.RouteMessages = false;
            this.StretchVertically = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets reference to the SpinControl's Down Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatArrowElement ButtonDown
        {
            get
            {
                return this.buttonDown;
            }
        }

        /// <summary>
        /// Gets reference to the  SpinControl's Up Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatArrowElement ButtonUp
        {
            get
            {
                return this.buttonUp;
            }
        }

        /////////////////////////////////
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal BoxLayout StackLayout
        {
            get
            {
                return this.stackLayout;
            }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to display in the RadSpinEdit
        /// </summary>
        [Category("Data"), DefaultValue(0), Localizable(true)]
        [Description("Gets or sets the number of decimal places to display in the RadSpinEdit")]
        public int DecimalPlaces
        {
            get { return this.decimalPlaces; }

            set
            {
                this.decimalPlaces = value;
                this.SetSpinValue();
            }
        }

        /// <summary>
        /// represent the default value in the numeric up/down
        /// </summary>        
        [DefaultValue(0)]
        protected decimal DefaultValue
        {
            get { return defaultValue; }
            set
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    this.defaultValue = value;
                }
                else
                {
                    if (value < minValue)
                    {
                        //this.textItem.Multiline = true;
                        this.value = minValue;
                        this.textItem.Text = minValue.ToString();
                    }
                    else if (value > maxValue)
                    {
                        //this.textItem.Multiline = true;
                        this.value = maxValue;
                        this.textItem.Text = maxValue.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadSpinEdit should display the value it contains in hexadecimal format.
        /// </summary>
        [Description("Gets or sets a value indicating whether the RadSpinEdit should display the value it contains in hexadecimal format.")]
        [Category("Appearance"), DefaultValue(false)]
        public bool Hexadecimal
        {
            get
            {
                return this.GetBitState(HexadecimalStateKey);
            }
            set
            {
                this.SetBitState(HexadecimalStateKey, value);
                this.SetSpinValue();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.
        /// </summary>
        [Category("Behavior"), DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.")]
        public bool InterceptArrowKeys
        {
            get
            {
                return this.GetBitState(InterceptArrowKeysStateKey);
            }
            set
            {
                this.SetBitState(InterceptArrowKeysStateKey, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only. </summary>
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return this.textItem.ReadOnly;
            }
            set
            {
                this.textItem.ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a thousands separator is displayed in the RadSpinEdit
        /// </summary>
        [Category("Data"), DefaultValue(false), Localizable(true)]
        [Description("Gets or sets a value indicating whether a thousands separator is displayed in the RadSpinEdit")]
        public bool ThousandsSeparator
        {
            get
            {
                return this.GetBitState(ThousandsSeparatorStateKey);
            }
            set
            {
                this.SetBitState(ThousandsSeparatorStateKey, value);
                this.SetSpinValue();
            }
        }

        internal TextBox TextBoxControl
        {
            get
            {
                return this.textItem.TextBoxControl;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return this.textItem.Text;
            }
            set
            {
                this.textItem.Text = value;
                this.SetSpinValue();
            }
        }

        /// <summary>
        /// Gets <see cref="RadTextBoxItem"/> contained in the spin editor.
        /// </summary>
        public virtual RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.textItem;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value that could be set in the spin editor
        /// </summary>
        [DefaultValue(HorizontalAlignment.Left)]
        [Description("Gets or sets the text alignment of RadSpinEditor")]
        [Category("Appearance")]
        public virtual HorizontalAlignment TextAlignment
        {
            get
            {
                return this.TextBoxItem.TextAlign;
            }
            set
            {
                this.TextBoxItem.TextAlign = value;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RightToLeftProperty)
            {
                DockLayoutPanel.SetDock(this.stackLayout, (bool)e.NewValue ? Dock.Left : Dock.Right);
            }
        }

        /// <summary>
        /// allow element to be stretched bertically
        /// </summary>
        [DefaultValue(false)]
        public override bool StretchVertically
        {
            get { return base.StretchVertically; }
            set { base.StretchVertically = value; }
        }

        /// <summary>
        /// represent the decimal in the numeric up/down
        /// </summary>        
        [DefaultValueAttribute(typeof(System.Decimal), "0")]
        public decimal Value
        {
            get { return this.value; }
            set
            {

                if (/*IsDesignMode &&*/ (value < this.minValue || value > this.maxValue))
                    throw new Exception(string.Format("'{0}' is not valid. Provide value between 'Minimum' and 'Maximum'", value));

                this.SetSpinValue(this.Constrain(value), true);


            }
        }

        protected virtual void SetSpinValue(decimal value, bool fromValue)
        {
            decimal newValue = this.Constrain(value);
            bool valueChanged = this.value != newValue;
            this.textItem.Text = GetNumberText(this.value);

            if (this.value != newValue)
            {
                ValueChangingEventArgs args = new ValueChangingEventArgs(newValue, this.value);
                this.OnValueChanging(args);
                if (args.Cancel)
                {
                    return;
                }

                this.value = newValue;

                this.OnValueChanged(EventArgs.Empty);
            }


            if (fromValue)
            {
                this.BitState[SuppressEditorStateKey] = true;
                this.BitState[TextValueChangedStateKey] = false;
                if (this.textItem != null)
                {
                    this.textItem.Text = GetNumberText(this.value);
                }

                this.BitState[SuppressEditorStateKey] = false;
            }

            if (valueChanged)
            {
                this.OnNotifyPropertyChanged("Value");
            }
        }

        private void SetSpinValue()
        {
            if (this.GetBitState(TextValueChangedStateKey))
            {
                this.SetSpinValue(this.GetValueFromText(), false);
            }
            else
            {
                this.SetSpinValue(this.value, true);
            }
        }

        /// <summary>
        /// Gets or sets the value which is added to/subtracted from the current value of the spin editor.
        /// </summary>
        [Description("Step"), DefaultValueAttribute(typeof(System.Decimal), "0"), Category("Appearance")]
        public decimal Step
        {
            get { return this.step; }
            set
            {
                if (this.step != value)
                {
                    this.step = value;
                    this.OnNotifyPropertyChanged("Step");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum value that could be set in the spin editor
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the minimum value that could be set in the spin editor.")]
        public decimal MinValue
        {
            get { return this.minValue; }

            set
            {
                this.minValue = value;
                this.OnNotifyPropertyChanged("MinValue");
                if (this.minValue > this.maxValue)
                {
                    this.maxValue = value;
                }

                SetSpinValue();

            }
        }

        /// <summary>
        /// Gets or sets whether RadSpinEditor will be used as a numeric textbox.
        /// </summary>     
        [DefaultValue(true)]
        [Category("Behavior"), Description("Gets or sets whether RadSpinEditor will be used as a numeric textbox.")]
        public bool ShowUpDownButtons
        {
            get
            {
                return this.GetBitState(ShowUpDownButtonsStateKey);
            }
            set
            {
                this.SetBitState(ShowUpDownButtonsStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets whether by right-mouse clicking the up/down button you set the max/min value respectively.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior"), Description("Gets or sets whether by right-mouse clicking the up/down button you reset the value to the Maximum/Minimum value respectively.")]
        public bool RightMouseButtonReset
        {
            get
            {
                return this.rightMouseButtonReset;
            }
            set
            {
                this.rightMouseButtonReset = value;
            }
        }

        /// <summary>
        /// set or get the Max numeric value in the numeric up/dowm 
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the maximum value that could be set in the spin editor.")]
        public decimal MaxValue
        {
            get { return this.maxValue; }

            set
            {
                this.maxValue = value;
                this.OnNotifyPropertyChanged("MaxValue");
                if (this.minValue > this.maxValue)
                {
                    this.minValue = this.maxValue;
                }
                this.SetSpinValue();
            }
        }

        /// <summary>Gets or sets a value indicating whether the border is shown.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether the border is shown.")]
        public bool ShowBorder
        {
            get
            {
                return border != null ? this.border.Visibility == ElementVisibility.Visible : false;
            }

            set
            {
                if (this.border != null)
                {
                    this.border.Visibility = (value ? ElementVisibility.Visible : ElementVisibility.Hidden);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the element is overriding the default
        ///     layout logic. Typically this property is set to true when the element is a
        ///     <see cref="Telerik.WinControls.Layouts.LayoutPanel">LayoutPanel</see>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool OverridesDefaultLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating that value will revert to minimum value after reaching maximum and to maximum after reaching minimum.
        /// </summary>
        [Description("Gets or sets a value indicating that value will revert to minimum value after reaching maximum and to maximum after reaching minimum")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool Wrap
        {
            get
            {
                return wrap;
            }
            set
            {
                wrap = value;
            }
        }

        #endregion

        #region Obsolete properties and methods


        /// <summary>
        /// This property is obsolete. Please, use <see cref="ButtonUp"/> property.
        /// </summary>
        [Obsolete("This property is obsolete. Please, use ButtonUp property.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatArrowElement UpButton
        {
            get { return this.ButtonUp; }
        }

        /// <summary>
        /// This property is obsolete. Please, use <see cref="ButtonDown"/> property.
        /// </summary>
        [Obsolete("This property is obsolete. Please, use ButtonDown property.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRepeatArrowElement DownButton
        {
            get { return this.ButtonDown; }
        }

        #endregion

        protected virtual decimal Constrain(decimal value)
        {
            if (this.wrap)
            {
                if (value > this.maxValue)
                {
                    value = this.minValue;
                }
                else if (value < this.minValue)
                {
                    value = this.maxValue;
                }

                return value;
            }

            return Math.Max(this.minValue, Math.Min(this.maxValue, value));
        }

        public virtual bool Validate()
        {
            this.EndTextEdit();
            this.ValidateCore();
            if (this.GetValueFromText() != this.value)
            {
                this.textItem.Text = this.GetNumberText();
            }

            return true;
        }

        protected virtual void ValidateCore()
        {
            decimal newValue = this.Constrain(this.value);
            if (newValue != this.value)
            {
                this.value = newValue;
                this.textItem.Text = this.GetNumberText();
            }
        }

        /// <summary>
        /// increase or decrease value in the numeric up/dowm with step value
        /// </summary>
        /// <param name="step"></param>
        public virtual void PerformStep(Decimal step)
        {
            decimal value = this.GetValueFromText();

            try
            {
                decimal incValue = value + step;
                value = incValue;
            }
            catch (OverflowException)
            {
            }

            this.Value = this.Constrain(value);
            this.Validate();
        }

        #region Events

        /// <summary>
        /// Occurs before the value of the SpinEdit is changed.        
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the editor finished the value editing.")]
        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the value is being changed. Cancelable event.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the editor is changing the value during the editing proccess.")]
        public event ValueChangingEventHandler ValueChanging;

        protected virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, e);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.BitState[TextValueChangedStateKey] |= !this.GetBitState(SuppressEditorStateKey);

            base.OnTextChanged(e);
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == ShowUpDownButtonsStateKey)
            {
                if (newValue)
                {
                    this.stackLayout.Visibility = ElementVisibility.Visible;
                    // Wrong calculations. Layout should actually be determined by the TextBoxItem
                    //this.Padding = new Padding(0, this.Padding.Top - 2, 0, this.Padding.Bottom - 1);
                }
                else
                {
                    this.stackLayout.Visibility = ElementVisibility.Collapsed;
                    // Wrong calculations. Layout should actually be determined by the TextBoxItem
                    //this.Padding = new Padding(0, this.Padding.Top + 2, 0, this.Padding.Bottom + 1);
                }
                this.OnNotifyPropertyChanged("ShowUpDownButtons");
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.GetBitState(InterceptArrowKeysStateKey))
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        this.PerformStep(this.Step);
                        e.Handled = true;
                        break;
                    case Keys.Down:
                        this.PerformStep(-this.Step);
                        e.Handled = true;
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.ValidateOnKeyPress(e);
            base.OnKeyPress(e);
            this.OnTextBoxKeyPress(e);
        }

        internal protected virtual void ValidateOnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Validate();
            }
        }

        protected void OnTextBoxKeyPress(KeyPressEventArgs e)
        {
            NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
            string numberGroupSeparator = numberFormat.NumberGroupSeparator;
            string negativeSign = numberFormat.NegativeSign;
            //FR#Issue ID 1037 - ADD Culture specific decimal separator from num keypad
            if (e.KeyChar == '.')
            {
                e.KeyChar = numberDecimalSeparator[0];
            }
            //end fr
            string str4 = e.KeyChar.ToString();

            if (((!char.IsDigit(e.KeyChar) && (!str4.Equals(numberDecimalSeparator) && !str4.Equals(numberGroupSeparator))) && !str4.Equals(negativeSign)) && (e.KeyChar != '\b'))
            {
                if (this.Hexadecimal)
                {
                    if ((e.KeyChar >= 'a') && (e.KeyChar <= 'f'))
                    {
                        return;
                    }
                    if ((e.KeyChar >= 'A') && (e.KeyChar <= 'F'))
                    {
                        return;
                    }
                }
                if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None)
                {
                    e.Handled = true;
                    NativeMethods.MessageBeep(0);
                }
            }
        }




        protected virtual void EndTextEdit()
        {
            if (this.GetBitState(TextValueChangedStateKey))
            {
                this.SetSpinValue(this.GetValueFromText(), false);
            }
        }

        #endregion

        #region Event handlers

        private void textItem_TextChanging(object sender, TextChangingEventArgs e)
        {
            this.OnTextChanging(e);
        }

        private void textItem_TextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(e);
        }

        private void HostedControl_MouseUp(object sender, MouseEventArgs e)
        {
            this.CallDoMouseUp(e);
        }

        private DateTime previosWheel = DateTime.Now;

        // Mouse:

        private void textItem_MouseWheel(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(string.Format("SpinElement going {0}", e.Delta > 0 ? "Up" : "Down"));
            int k = 1;
            if (DateTime.Now < previosWheel.AddMilliseconds(15))
            {
                k = 10;
            }
            previosWheel = DateTime.Now;

            decimal step = (e.Delta > 0) ? Step * k : -Step * k;
            this.PerformStep(step);
        }

        // Keyboard:

        private void textItem_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void textItem_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void textItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        // Others:

        internal protected virtual void textItem_LostFocus(object sender, EventArgs e)
        {
            this.Validate();
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            if (this.rightMouseButtonReset)
            {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    if (mouseEventArgs.Button == MouseButtons.Right)
                    {
                        this.Value = this.minValue;
                        return;
                    }
                }
            }
            PerformStep(-Step);
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            if (this.rightMouseButtonReset)
            {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    if (mouseEventArgs.Button == MouseButtons.Right)
                    {
                        this.Value = this.maxValue;
                        return;
                    }
                }
            }
            PerformStep(Step);
        }



        #endregion

        private string GetNumberText()
        {
            return RadSpinElement.GetNumberText(this.Value, this.Hexadecimal, this.ThousandsSeparator, this.DecimalPlaces);
        }

        protected virtual string GetNumberText(decimal num)
        {
            return RadSpinElement.GetNumberText(num, this.Hexadecimal, this.ThousandsSeparator, this.DecimalPlaces);
        }

        private static string GetNumberText(decimal num, bool hex, bool thousands, int decimalPlaces)
        {
            if (hex)
            {
                return string.Format("{0:X}", (long)num);
            }

            return num.ToString((thousands ? "N" : "F") + decimalPlaces.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
        }

        private decimal GetValueFromText()
        {
            NumberStyles ns = this.Hexadecimal ? NumberStyles.HexNumber : NumberStyles.Number;

            try
            {
                if (!string.IsNullOrEmpty(this.Text) && ((this.Text.Length != 1) || (this.Text != "-")))
                {
                    decimal resultValue = 0m;

                    if (this.Hexadecimal)
                    {
                        resultValue = this.Constrain(Convert.ToDecimal(Convert.ToInt32(this.Text, 0x10)));
                    }
                    else
                    {
                        resultValue = this.Constrain(decimal.Parse(this.Text, CultureInfo.CurrentCulture));
                    }
                    return resultValue;
                }
                else
                    return this.value;
            }
            catch
            {
                return this.value;
            }
        }
    }
}
