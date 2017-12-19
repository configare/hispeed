using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using System.Drawing.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The RadWaitingBar class is a simple wrapper for the
    /// <see cref="RadWaitingBarElement">RadWaitingBarElement</see> class.
    /// The latter implements all UI and logic functionality.
    /// The RadWaitingBar class acts to transfer events to and from the
	/// <see cref="RadWaitingBarElement">RadWaitingBarElement</see> class.
	/// <see cref="RadWaitingBarElement">RadWaitingBarElement</see> can be
    /// nested in other telerik controls.
    /// </summary>
    [ToolboxItem(true)]
    [Description("The component indicates that a long-running operation is underway")]
    [DefaultProperty("WaitingSpeed"), DefaultEvent("WaitingStarted")]
    public class RadWaitingBar : RadControl
    {

        #region Fields

        RadWaitingBarElement waitingBarElement;

        #endregion

        #region Events

        /// <summary>
        /// Starts control waiting
        /// </summary>
        public event EventHandler WaitingStarted
        {
            add { WaitingBarElement.WaitingStarted += value; }
            remove { WaitingBarElement.WaitingStarted -= value; }
        }

        /// <summary>
        /// Ends control waiting
        /// </summary>
        public event EventHandler WaitingStopped
        {
            add { WaitingBarElement.WaitingStopped += value; }
            remove { WaitingBarElement.WaitingStopped -= value; }
        }

        [Description("The method starts control's waiting animation")]
        public void StartWaiting()
        {
            this.WaitingBarElement.StartWaiting();
        }
        
        [Description("The method stops control's waiting animation"),
        Obsolete("The EndWaiting() method is obsolete. Use StopWaiting() method instead.")]
        public void EndWaiting()
        {
            this.WaitingBarElement.StopWaiting();
        }

        [Description("The method stops control's waiting animation")]
        public void StopWaiting()
        {
            this.WaitingBarElement.StopWaiting();
        }

        [Description("The method sets the indicator to its initial position")]
        public void ResetWaiting()
        {
            this.WaitingBarElement.ResetWaiting();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of <see cref="WaitingBarIndicatorElement"/> elements
        /// which contains all waiting indicators of RadWaitingBar
        /// </summary>
        [Browsable(false),
        Description("Gets a collection of WaitingBarIndicatorElement elements which contains all waiting indicators of RadWaitingBar"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingIndicatorCollection Indicators
        {
            get { return this.WaitingBarElement.Indicators; }
        }

        /// <summary>
        /// Gets the instance of RadWaitingBarElement wrapped by this control. RadWaitingBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadWaitingBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the instance of RadWaitingBarElement wrapped by this control")]
        public RadWaitingBarElement WaitingBarElement
        {
            get
            {
                return this.waitingBarElement;
            }
        }

        /// <summary>
        /// Sets the DefaultSize od RadWaitingBar
        /// </summary>
        protected override System.Drawing.Size DefaultSize
        {
            get
            {
                return new System.Drawing.Size(150, 30);
            }
        }

        /// <summary>
        /// Gets and sets the image property of the indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the image property of the indicator")]
        [DefaultValue(null)]
        public Image Image
        {
            get
            {
                return this.WaitingBarElement.IndicatorImage;
            }
            set
            {
                this.WaitingBarElement.IndicatorImage = value;
            }
        }
        
        /// <summary>
        /// Gets and sets the image index property of the indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the image index property of the indicator")]
        [DefaultValue(-1)]
        [RefreshProperties(RefreshProperties.All)]
        [RelatedImageList("ImageList")]
        [Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor))]
        [TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public int ImageIndex
        {
            get
            {
                return this.WaitingBarElement.IndicatorImageIndex;
            }
            set
            {
                this.WaitingBarElement.IndicatorImageIndex = value;
            }
        }

        /// <summary>
        /// Gets and sets the image key property of the indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the image key property of the indicator")]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All), Localizable(true)]
        [RelatedImageList("ImageList")]
        [Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor))]
        [TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string ImageKey
        {
            get
            {
                return this.WaitingBarElement.IndicatorImageKey;
            }
            set
            {
                this.WaitingBarElement.IndicatorImageKey = value;
            }
        }


        /// <summary>
        /// Indicates whether the control is currently waiting
        /// </summary>
        [Browsable(false)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates whether the control is currently waiting")]
        [DefaultValue(false)]
        public bool IsWaiting
        {
            get
            {
                return this.waitingBarElement.IsWaiting;
            }
        }

        /// <summary>
        /// Indicates the orientation of the RadWaitingBar
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Indicates the orientation of the RadWaitingBar")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get 
            {
                return this.WaitingBarElement.WaitingBarOrientation;
            }
            set
            {
                if (this.WaitingBarElement.WaitingBarOrientation != value)
                {
                    this.WaitingBarElement.WaitingBarOrientation = value;
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingDirection"));
                }
            }
        }

        /// <summary>
        /// Indicates whether the indicators are stretched horizontally
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Indicates whether the indicators are stretched horizontally")]
        [DefaultValue(false)]        
        public bool StretchIndicatorsHorizontally
        {
            get
            {
                return this.WaitingBarElement.StretchIndicatorsHorizontally;
            }
            set
            {
                this.WaitingBarElement.StretchIndicatorsHorizontally = value;
            }
        }

        /// <summary>
        /// Indicates whether the indicators are stretched vertically
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Indicates whether the indicators are stretched vertically")]
        [DefaultValue(true)]     
        public bool StretchIndicatorsVertically
        {
            get
            {
                return this.WaitingBarElement.StretchIndicatorsVertically;
            }
            set
            {
                this.WaitingBarElement.StretchIndicatorsVertically = value;
            }
        }

        /// <summary>
        /// Sets the style of RadWaitingBar 
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Sets the style of RadWaitingBar")]
        [DefaultValue(WaitingBarStyles.Indeterminate)]
        public WaitingBarStyles WaitingStyle
        {
            get
            {
                return this.WaitingBarElement.WaitingStyle;
            }
            set
            {
                if (this.WaitingBarElement.WaitingStyle != value)
                {
                    this.WaitingBarElement.WaitingStyle = value;
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingStyle"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the text of the control's textElement
        /// </summary>
        [Description("Gets and sets the text of the control's textElement"),
        Category(RadDesignCategory.AppearanceCategory),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Browsable(true)]
        public override string Text
        {
            get
            {
                return this.waitingBarElement.TextElement.Text;
            }
            set
            {
                this.waitingBarElement.TextElement.Text = value;
            }
        }

        /// <summary>
        /// Gets and sets the WaitingDirection of the RadWaitingBarElement
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets and sets the WaitingDirection of the RadWaitingBarElement")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(ProgressOrientation.Right)]
        public ProgressOrientation WaitingDirection
        {
            get 
            {
                return this.WaitingBarElement.WaitingDirection;
            }
            set
            {
                if (this.WaitingBarElement.WaitingDirection != value)
                {
                    this.WaitingBarElement.WaitingDirection = value;
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingDirection"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the width of the indicator in pixels
        /// </summary>
        [Browsable(false),
        Description("Gets and sets the width of the indicator in pixels"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(50),
        Obsolete("WaitingIndicatorWidth is obsolete. Use WaitingIndicatorSize instead")]
        public int WaitingIndicatorWidth
        {
            get
            {
                return this.waitingBarElement.WaitingIndicatorSize.Width;
            }
            set
            {
                if (this.waitingBarElement.WaitingIndicatorSize.Width != value)
                {
                    int height = this.waitingBarElement.WaitingIndicatorSize.Height;
                    this.waitingBarElement.WaitingIndicatorSize = new Size(value, height);
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingIndicatorWidth"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the size of the indicator in pixels
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the size of the indicator in pixels")]
        public Size WaitingIndicatorSize
        {
            get
            {
                return this.waitingBarElement.WaitingIndicatorSize;
            }
            set
            {
                this.waitingBarElement.WaitingIndicatorSize = value;
            }
        }

        /// <summary>
        /// Gets and sets the speed of the animation. Higher value moves the indicator more quickly across the bar
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the speed of the animation. Higher value moves the indicator more quickly across the bar")]
        [DefaultValue(90)]
        public int WaitingSpeed
        {
            get
            {
                return this.waitingBarElement.WaitingSpeed;
            }

            set
            {
                if (WaitingSpeed != value)
                {
                    this.waitingBarElement.WaitingSpeed = value;
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingSpeed"));
                }
            }
        }

        /// <summary>
        /// Gets and sets the number of pixels the indicator moves each step
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the number of pixels the indicator moves each step")]
        [DefaultValue(1)]
        public int WaitingStep
        {
            get
            {
                return this.waitingBarElement.WaitingStep;
            }

            set
            {
                if (WaitingStep != value)
                {
                    this.waitingBarElement.WaitingStep = value;
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("WaitingStep"));
                }
            }
        }

        /// <summary>
        /// Shows text in RadWaitingBar.
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Shows text in RadWaitingBar.")]
        [DefaultValue(false)]
        public bool ShowText
        {
            get
            {
                return this.WaitingBarElement.ShowText;
            }
            set
            {
                this.WaitingBarElement.ShowText = value;
            }
        }

        #endregion

        #region Initialization
        protected virtual RadWaitingBarElement CreateWaitingBarElement()
        {
            return new RadWaitingBarElement();
        }

        protected override void CreateChildItems(RadElement parent)
        {
            waitingBarElement = CreateWaitingBarElement();
            this.RootElement.Children.Add(this.waitingBarElement);
            this.WireEvents();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.UnwireEvents();
            base.Dispose(disposing);
        }

        #endregion

        #region Overrides
        
        protected override void OnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            base.OnToolTipTextNeeded(this.WaitingBarElement, e);
        }

       #endregion

        #region Helpers
        
        private void WireEvents()
        {
            this.WaitingBarElement.TextElement.TextChanged += TextElement_TextChanged;
        }

        private void UnwireEvents()
        {
            this.WaitingBarElement.TextElement.TextChanged -= TextElement_TextChanged;
        }
        
        #endregion

        #region Event Handlers
        
        void TextElement_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(e);
        }

        #endregion
    }
}
