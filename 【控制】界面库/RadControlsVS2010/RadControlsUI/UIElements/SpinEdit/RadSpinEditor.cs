using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a numeric up/down control box. The RadSpinEditor class is a simple wrapper for the
    ///     <see cref="RadCheckBoxElement">RadSpinElement</see> class. The RadSpinEditor acts
    ///     to transfer events to and from its corresponding
    ///     <see cref="RadCheckBoxElement">RadCheckBoxElement</see>. The
    ///     <see cref="RadCheckBoxElement">RadCheckBoxElement</see> which is essentially the
    ///     RadCheckBox control may be nested in other telerik controls.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [ToolboxItem(true)]
    [RadThemeDesignerData(typeof(RadSpinControlDesignTimeData))]
	[Description("Displays a single numeric value that the user can increment and decrement by clicking the up and down buttons on the control")]
	[DefaultProperty("Value"), DefaultEvent("ValueChanged")]
    [DefaultBindingProperty("Value")]
    public partial class RadSpinEditor : RadControl, ISupportInitialize
    {
        //members
        private RadSpinElement spinElement; 

        /// <summary>
        /// Occurs before the value of the SpinEdit is changed.        
        /// </summary>
        [Category("Behavior")]
        public event EventHandler ValueChanged;

        /// <summary>        
        /// Occurs before the value of the SpinEdit is changing.        
        /// </summary>
        [Category("Behavior")]
        public event ValueChangingEventHandler ValueChanging;


        static RadSpinEditor()
        {
        }

        ///<summary>
        ///Initializes a new instance of the RadSpinEditor class
        ///</summary>
        public RadSpinEditor()
        {
            this.AutoSize = true;
            this.TabStop = false;
            base.SetStyle(ControlStyles.Selectable, true);            
        }

        void spinElement_ValueChanged(object sender, EventArgs e)
        {
            this.OnValueChanged(e);
        }

        void spinElement_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            this.OnValueChanging(e);
        }

        /// <summary>
        /// set the default control size
        /// </summary>
        protected override Size DefaultSize
        {
            get { return new Size(100, 20); }
        }
        
        /// <summary>
        /// Gets the instance of RadSpinElement wrapped by this control. RadSpinElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadSpinControl.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadSpinElement SpinElement
        {
            get
            {
                return this.spinElement;
            }
        }

        /// <summary>Turns on and off autosize.</summary>
        [Browsable(false), Category(RadDesignCategory.LayoutCategory), DefaultValue(true)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }


        /// <summary>
        /// Gets or sets the mimimum value for the spin edit
        /// </summary>
        [Description("Minimum")]
        [Category("Data")]
        [DefaultValueAttribute(typeof(System.Decimal), "0")]
        public Decimal Minimum
        {
            get
            {
                return this.SpinElement.MinValue;
            }
            set
            {
                this.SpinElement.MinValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for the spin edit
        /// </summary>
        [Description("Maximum")]
        [Category("Data")]
        [DefaultValueAttribute(typeof(System.Decimal), "100")]
        public Decimal Maximum
        {
            get
            {
                return this.SpinElement.MaxValue;
            }
            set
            {
                this.SpinElement.MaxValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the whether RadSpinEditor will be used as a numeric textbox.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true), Description("Gets or sets the whether RadSpinEditor will be used as a numeric textbox.")]
        public bool ShowUpDownButtons
        {
            get
            {
                return this.SpinElement.ShowUpDownButtons;
            }
            set
            {
                this.SpinElement.ShowUpDownButtons = value;
            }
        }

        /// <summary>
        /// Gets or sets whether by right-mouse clicking the up/down button you reset the value to the Maximum/Minimum value respectively.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior"), Description("Gets or sets whether by right-mouse clicking the up/down button you reset the value to the Maximum/Minimum value respectively.")]
        public bool RightMouseButtonReset
        {
            get
            {
                return this.spinElement.RightMouseButtonReset;
            }
            set
            {
                this.spinElement.RightMouseButtonReset = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether the border is shown.</summary>
        [Description("ShowBorder")]
        [Category("Appearance")]
        public bool ShowBorder
        {
            get
            {
                return this.SpinElement.ShowBorder;
            }
            set
            {
                this.SpinElement.ShowBorder = value;

            }
        }

        /// <summary>
        ///Set or get the Step value
        /// </summary>
        [Description("Step")]
        [DefaultValueAttribute(typeof(System.Decimal), "1")]
        [Category("Data")]        
        [Browsable(false)]
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public Decimal Step
        {
            get
            {
                return this.SpinElement.Step;
            }
            set
            {
                this.SpinElement.Step = value;
            }
        }

        /// <summary>
        ///Set or get the Step value
        /// </summary>
        [Description("Increment")]
        [DefaultValueAttribute(typeof(System.Decimal), "1")]
        [Category("Data")]
        public Decimal Increment
        {
            get
            {
                return this.SpinElement.Step;
            }
            set
            {
                this.SpinElement.Step = value;
            }
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
                return this.SpinElement.Wrap;
            }
            set
            {
                this.SpinElement.Wrap = value;
            }
        }

        /// <summary>
        /// increase or decrease value in the numeric up/dowm with step value
        /// </summary>
        /// <param name="step"></param>
        public void PerformStep(Decimal step)
        {
            this.SpinElement.PerformStep(step);
        }

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ///<summary>
        /// Represents the decimal value in the numeric up/down
        ///</summary>        
        [DefaultValueAttribute(typeof(System.Decimal), "0")]
        [Category("Data")]
        [Bindable(true)]
        public decimal Value
        {
            set
            {
                this.SpinElement.Value = value;
            }
            get
            {
                return this.SpinElement.Value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get
            {
                return this.spinElement.Text;
            }
            set
            {
                this.spinElement.Text = value;
            }
        }

        protected override void Select(bool directed, bool forward)
        {
            this.spinElement.TextBoxControl.Select();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get
            {
                return this.spinElement.InterceptArrowKeys;
            }
            set
            {
                this.spinElement.InterceptArrowKeys = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only. </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.")]
        public bool ReadOnly
        {
            get
            {                
                return this.spinElement.ReadOnly;
            }
            set
            {             
                this.spinElement.ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a thousands separator is displayed in the RadSpinEditor
        /// </summary>
        [Category("Data")]
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("Gets or sets a value indicating whether a thousands separator is displayed in the RadSpinEditor")]
        public bool ThousandsSeparator
        {
            get
            {
                return this.spinElement.ThousandsSeparator;
            }
            set
            {
                this.spinElement.ThousandsSeparator = value;                
            }
        }
        
        /// <summary>
        /// Gets or sets the number of decimal places to display in the RadSpinEditor
        /// </summary>
        [Category("Data")]
        [DefaultValue(0)]
        [Localizable(true)]
        [Description("Gets or sets the number of decimal places to display in the RadSpinEditor")]
        public int DecimalPlaces
        {
            get
            {
                return this.spinElement.DecimalPlaces;
            }
            set
            {
                this.spinElement.DecimalPlaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RadSpinEditor should display the value it contains in hexadecimal format.
        /// </summary>
        [Description("Gets or sets a value indicating whether the RadSpinEditor should display the value it contains in hexadecimal format.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Hexadecimal
        {
            get
            {
                return this.spinElement.Hexadecimal;
            }
            set
            {
                this.spinElement.Hexadecimal = value;
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
                return this.spinElement.TextAlignment;
            }
            set
            {
                this.spinElement.TextAlignment = value;
            }
        }

        #region Themes

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element.GetType() == typeof(RadRepeatArrowElement))
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion

        #region Focus management

        private bool entering = false;

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (!entering)
            {
                entering = true;
                this.spinElement.TextBoxControl.Focus();
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

        private void OnValueChanging(ValueChangingEventArgs args)
        {
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, args);
            }
        }

        private void OnValueChanged(EventArgs args)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, args);
            }
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
            }
            if (value == RightToLeft.No)
            {
                this.SpinElement.RightToLeft = false;
            }
            else if (value == RightToLeft.Yes)
            {
                this.SpinElement.RightToLeft = true;
            }
          

            base.OnRightToLeftChanged(e);
        }

        #region ISupportInitialize Members

        
        public override void BeginInit()
        {
            base.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();
        }

        #endregion

        /// <summary>
        /// CreateChildItems
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.spinElement = new RadSpinElement();
            this.spinElement.RightToLeft = this.RightToLeft == RightToLeft.Yes;
            this.RootElement.Children.Add(spinElement);
            this.RootElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            base.CreateChildItems(parent);

            this.spinElement.ValueChanging += new ValueChangingEventHandler(spinElement_ValueChanging);
            this.spinElement.ValueChanged += new EventHandler(spinElement_ValueChanged);
        }

        /// <summary>
        /// initialize root element
        /// </summary>
        /// <param name="rootElement"></param>
        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }


        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadSpinEditorAccessibleObject(this);
        }      
    }
}
