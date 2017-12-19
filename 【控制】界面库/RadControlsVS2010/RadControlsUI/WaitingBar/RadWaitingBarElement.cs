using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Enumerations;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a waiting bar element. It may be included in other telerik controls.
    /// All graphical and logical functionality is implemented in RadWaitingBarElement.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadWaitingBarElement : LightVisualElement
    {
        #region Fields

        protected WaitingBarContentElement contentElement;
        protected Timer timer;
        protected bool continueWaiting;
        protected float offset;
        #endregion

        #region RadProperties

        public static RadProperty IsVerticalProperty = RadProperty.Register(
            "IsVertical", typeof(bool),typeof(RadWaitingBarElement), new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsWaitingProperty = RadProperty.Register(
            "IsWaiting", typeof(bool), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static RadProperty StretchIndicatorsHorizontallyProperty = RadProperty.Register(
            "StretchIndicatorsHorizontally", typeof(bool), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout));

        public static RadProperty StretchIndicatorsVerticallyProperty = RadProperty.Register(
           "StretchIndicatorsVertically", typeof(bool), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout));

        public static RadProperty WaitingBarOrientationProperty = RadProperty.Register(
            "WaitingBarOrientation",typeof(Orientation), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(
            Orientation.Horizontal, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout));

        public static RadProperty WaitingDirectionProperty = RadProperty.Register(
         "WaitingDirection", typeof(ProgressOrientation), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(
          ProgressOrientation.Right, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout));
        
        public static RadProperty WaitingIndicatorSizeProperty = RadProperty.Register(
          "WaitingIndicatorSize", typeof(Size), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(
              new Size(50, 30), ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout));

        public static RadProperty WaitingSpeedProperty = RadProperty.Register(
            "WaitingSpeed", typeof(int), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(90, ElementPropertyOptions.AffectsArrange));

        public static RadProperty WaitingStepProperty = RadProperty.Register(
            "WaitingStep", typeof(int), typeof(RadWaitingBarElement), new RadElementPropertyMetadata(1, ElementPropertyOptions.AffectsArrange));

        #endregion

        #region Properties
       
        /// <summary>
        /// Gets an instance of the <see cref="WaitingBarContentElement"/> class
        /// that represents the waiting bar content element
        /// </summary>
        [Browsable(false)]
        [Description("Gets the instance of the WaitingBarContentElement class which represents the waiting bar content element")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingBarContentElement ContentElement
        {
            get { return this.contentElement; }
        }

        /// <summary>
        /// Gets a collection of <see cref="WaitingBarIndicatorElement"/> elements
        /// which contains all waiting indicators of RadWaitingBar
        /// </summary>
        [Browsable(false),
        Description("Gets a collection of WaitingBarIndicatorElement elements which contains all waiting indicators of RadWaitingBar"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingIndicatorCollection Indicators
        {
            get { return this.contentElement.Indicators; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WaitingBarTextElement"/> class
        /// that represents the waiting bar text element
        /// </summary>
        [Browsable(false),
        Description("Gets an instance of the WaitingBarTextElement class which represents the waiting bar text element"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingBarTextElement TextElement
        {
            get { return this.ContentElement.TextElement; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="WaitingBarSeparatorElement"/> class
        /// that represents the waiting bar separator element
        /// </summary>
        [Browsable(false),
        Description("Gets an instance of the WaitingBarSeparatorElement class which represents the waiting bar separator element"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaitingBarSeparatorElement SeparatorElement
        {
            get { return this.ContentElement.SeparatorElement; }
        }

        /// <summary>
        /// Gets and sets the Image of the element's indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the Image of the element's indicator")]
        [DefaultValue(null)]
        public Image IndicatorImage
        {
            get
            {
                return this.ContentElement.Indicators[0].Image;
            }
            set
            {
                for (int i = 0; i < Indicators.Count; i++)
                {
                    Indicators[i].Image = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the ImageIndex of the element's indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the ImageIndex of the element's indicator")]
        [DefaultValue(-1)]
        public int IndicatorImageIndex
        {
            get
            {
                return this.ContentElement.Indicators[0].ImageIndex;
            }
            set
            {
                for (int i = 0; i < Indicators.Count; i++)
                {
                    Indicators[i].ImageIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the ImageKey of the element's indicator
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets and sets the ImageKey of the element's indicator")]
        [DefaultValue("")]
        public string IndicatorImageKey
        {
            get
            {
                return this.ContentElement.Indicators[0].ImageKey;
            }
            set
            {
                for (int i = 0; i < Indicators.Count; i++)
                {
                    Indicators[i].ImageKey = value;
                }
            }
        }

        /// <summary>
        /// Shows text in RadWaitingBarElement.
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
                return this.TextElement.DrawText;
            }
            set
            {
                this.TextElement.DrawText = value;
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
                return (bool)this.GetValue(StretchIndicatorsHorizontallyProperty);
            }
            set
            {
                if (this.StretchIndicatorsHorizontally != value)
                {
                    this.SetValue(StretchIndicatorsHorizontallyProperty, value);
                    for(int i =0;i< this.ContentElement.Indicators.Count;i++)
                    {
                        this.ContentElement.Indicators[i].StretchHorizontally = value;
                    }
                }
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
                return (bool)this.GetValue(StretchIndicatorsVerticallyProperty);
            }
            set
            {
                if (this.StretchIndicatorsVertically != value)
                {
                    this.SetValue(StretchIndicatorsVerticallyProperty, value);
                    for (int i = 0; i < this.ContentElement.Indicators.Count; i++)
                    {
                        this.ContentElement.Indicators[i].StretchVertically = value;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the style of the WaitingBarElement 
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Sets the style of the WaitingBarElement")]
        [DefaultValue(WaitingBarStyles.Indeterminate)]  
        public WaitingBarStyles WaitingStyle
        {
            get
            {
                return this.ContentElement.WaitingStyle;
            }
            set
            {
                if (this.ContentElement.WaitingStyle != value)
                {
                    this.ContentElement.WaitingStyle = value;
                    this.InvalidateMeasure(true);
                    this.InvalidateArrange(true);
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
                return (Size)this.GetValue(WaitingIndicatorSizeProperty);
            }
            set
            {
                if (this.WaitingIndicatorSize != value)
                {
                    this.SetValue(WaitingIndicatorSizeProperty, new Size(value.Width, value.Height));
                    this.InvalidateMeasure(true);
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
        Obsolete("WaitingIndicatorWidth property is obsolete. Use WaitingIndicatorSize property instead.")]
        public int WaitingIndicatorWidth
        {
            get
            {
                return this.WaitingIndicatorSize.Width;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                int height = WaitingIndicatorSize.Height;
                this.WaitingIndicatorSize = new Size(value, height);
            }
        }

        /// <summary>
        /// Indicates whether the element is currently waiting
        /// </summary>
        [Browsable(false)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates whether the element is currently waiting")]
        [DefaultValue(false)]
        public bool IsWaiting
        {
            get
            {
                return timer.Enabled;
            }
        }    

        /// <summary>
        /// When set to vertical the RadWaitingBar WaitingDirection property is set to Bottom
        /// When set to horizontal the RadWaitingBar WaitingDirection is property is set to Right
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("When set to vertical the RadWaitingBar WaitingDirection property is set to Bottom. When set to horizontal the RadWaitingBar WaitingDirection is property is set to Right")]
        [DefaultValue(Orientation.Horizontal)]
        public virtual Orientation WaitingBarOrientation
        {
            get
            {
                return (Orientation)this.GetValue(WaitingBarOrientationProperty);
            }
            set
            {
                this.SetValue(WaitingBarOrientationProperty, value);
                
                //When the WaitingDirection is either Left or Right and the Orientation is set to Horizontal, the WaitingDirection should not change
                //When the WaitingDirection is either Top or Bottom and the Orientation is set to Vertical, the WaitingDirection should not change
                bool isHorizontal = false;
                if (this.WaitingDirection == ProgressOrientation.Left || this.WaitingDirection == ProgressOrientation.Right)
                {
                    isHorizontal = true;
                }

                if (!isHorizontal && value == System.Windows.Forms.Orientation.Horizontal)
                {
                    this.WaitingDirection = ProgressOrientation.Right;
                }

                if (isHorizontal && value == System.Windows.Forms.Orientation.Vertical)
                {
                    this.WaitingDirection = ProgressOrientation.Bottom;
                }

            }
        }
       
        /// <summary>
        /// Gets and sets the direction of waiting, e.g.
        /// the Right value moves the indicator from left to right
        /// Range: Bottom, Left, Right, Top
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets and sets the direction of waiting")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(ProgressOrientation.Right)]
        public ProgressOrientation WaitingDirection
        {
            get
            {
                return (ProgressOrientation)this.GetValue(WaitingDirectionProperty);
            }
            set
            {
                this.SetValue(WaitingDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the speed of the indicator
        /// Greater value results in faster indicator
        /// Range: [0, 100]
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
                return (int)this.GetValue(WaitingSpeedProperty);
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.SetValue(WaitingSpeedProperty, value);
            }
        }

        /// <summary>
        /// Gets and sets the step in pixels which moves the indicator
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
                return (int)this.GetValue(WaitingStepProperty);
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.SetValue(WaitingStepProperty, value);
            }
        }

        #endregion

        #region Events

        [Description("Occurs when the control starts waiting")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event EventHandler WaitingStarted;

        [Description("Occurs when the control ends waiting")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event EventHandler WaitingStopped;

        protected virtual void OnStartWaiting(EventArgs e)
        {
            EventHandler handler = WaitingStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnStopWaiting(EventArgs e)
        {
            EventHandler handler = WaitingStopped;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Initialization

        static RadWaitingBarElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadWaitingBarStateManager(), typeof(RadWaitingBarElement));
            new Themes.ControlDefault.WaitingBar().DeserializeTheme();
        }

        public RadWaitingBarElement()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            continueWaiting = false;
        }

        protected override void DisposeManagedResources()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            base.DisposeManagedResources();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            contentElement = new WaitingBarContentElement();
            this.Children.Add(contentElement);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawBorder = true;
            this.DrawFill = true;
            this.DrawText = true;
            this.ClipDrawing = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the waiting process
        /// </summary>
        public virtual void StartWaiting()
        {
            if (!this.IsInValidState(false))
            {
                return;
            }

            if (this.WaitingSpeed != 0 && this.WaitingStep != 0 && this.Enabled)
            {
                timer.Interval = 100 - this.WaitingSpeed + 1;
                timer.Start();
                this.ContentElement.IsWaiting = true;
                OnStartWaiting(EventArgs.Empty);
            }
            if (this.WaitingSpeed == 0 || this.WaitingStep == 0)
            {
                continueWaiting = true;
            }
        }

        /// <summary>
        /// Stops the waiting process
        /// </summary>
        public virtual void StopWaiting()
        {
            if (!this.IsInValidState(false))
            {
                return;
            }

            timer.Stop();
            continueWaiting = false;
            this.ContentElement.IsWaiting = false;
            OnStopWaiting(EventArgs.Empty);
        }

        /// <summary>
        /// Sets the indicator to its starting position depending on the WaitingDirection
        /// </summary>
        public void ResetWaiting()
        {
            if (this.Enabled)
            {
                this.ContentElement.ResetWaiting();
            }
        }

        #endregion

        #region Event handlers

        protected virtual void OnWaitingStep()
        {
            ContentElement.IncrementOffset(this.WaitingStep);
            this.InvalidateArrange(true);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            OnWaitingStep();
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            
            Padding borderThikness = GetBorderThickness(false);
            SizeF desiredSize = SizeF.Empty;

            availableSize.Width -= borderThikness.Horizontal + Padding.Horizontal;
            availableSize.Height -= borderThikness.Vertical + Padding.Vertical;
            
            contentElement.Measure(availableSize);

            desiredSize.Width = ContentElement.DesiredSize.Width + borderThikness.Horizontal + this.Padding.Horizontal;
            desiredSize.Height = ContentElement.DesiredSize.Height + borderThikness.Vertical + this.Padding.Vertical;

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF clientRect = GetClientRectangle(finalSize);
         
            ContentElement.Arrange(clientRect);

            return finalSize;
        }

        protected virtual float GetTransformAngle(ProgressOrientation direction)
        {
            float angle = 0;
            if (direction == ProgressOrientation.Top || direction == ProgressOrientation.Bottom)
            {
                angle = 270f;
            }

            return angle;
        }

        protected virtual void TransformElements()
        {
            float angle = GetTransformAngle(WaitingDirection);
            this.ContentElement.TextElement.AngleTransform = angle;

            this.ResetWaiting();
        }

        protected virtual void UpdateElementsState(ProgressOrientation direction)
        {
            bool isVertical = false;
            if (direction == ProgressOrientation.Top || direction == ProgressOrientation.Bottom)
            {
                isVertical = true;
            }

            this.SetValue(RadWaitingBarElement.IsVerticalProperty, isVertical);
            this.SeparatorElement.SetValue(WaitingBarSeparatorElement.IsVerticalProperty, isVertical);
            for (int i = 0; i < this.Indicators.Count; i++)
            {
                this.Indicators[i].SetValue(WaitingBarIndicatorElement.IsVerticalProperty, isVertical);
                this.Indicators[i].SeparatorElement.SetValue(WaitingBarSeparatorElement.IsVerticalProperty, isVertical);
            }
        }

        private void UpdateIndicatorStretch(ProgressOrientation progressOrientation)
        {
            if (progressOrientation == ProgressOrientation.Left || progressOrientation == ProgressOrientation.Right)
            {
                this.StretchIndicatorsHorizontally = false;
                this.StretchIndicatorsVertically = true;
            }
            else
            {
                this.StretchIndicatorsHorizontally = true;
                this.StretchIndicatorsVertically = false;
            }
        }

        private void UpdateElementOrientation(ProgressOrientation oldDirection, ProgressOrientation newDirection)
        {
            bool isVertical = false, wasVertical = false;
            if (oldDirection == ProgressOrientation.Top || oldDirection == ProgressOrientation.Bottom)
            {
                wasVertical = true;
            }
            if (newDirection == ProgressOrientation.Top || newDirection == ProgressOrientation.Bottom)
            {
                isVertical = true;
            }
            if (wasVertical && !isVertical)
            {
                this.WaitingBarOrientation = System.Windows.Forms.Orientation.Horizontal;
                
                if (GetValueSource(RadWaitingBarElement.WaitingIndicatorSizeProperty) != Telerik.WinControls.ValueSource.Local)
                {
                    SetDefaultValueOverride(RadWaitingBarElement.WaitingIndicatorSizeProperty, new System.Drawing.Size(WaitingIndicatorSize.Height, WaitingIndicatorSize.Width));
                }
            }
            if (!wasVertical && isVertical)
            {
                this.WaitingBarOrientation = System.Windows.Forms.Orientation.Vertical;
                
                if (GetValueSource(RadWaitingBarElement.WaitingIndicatorSizeProperty) != Telerik.WinControls.ValueSource.Local)
                {
                    SetDefaultValueOverride(RadWaitingBarElement.WaitingIndicatorSizeProperty, new System.Drawing.Size(WaitingIndicatorSize.Height, WaitingIndicatorSize.Width));
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == WaitingDirectionProperty)
            {
                //Update RadWaitingBar state and WaitingIndicators state
                UpdateElementsState((ProgressOrientation)e.NewValue);

                // Update RadWaitingBarElement Orientation
                UpdateElementOrientation((ProgressOrientation)e.OldValue, (ProgressOrientation)e.NewValue);

                //Update StretchIndicatorsHorizontally and StretchIndicatorsVertically properties
                UpdateIndicatorStretch((ProgressOrientation)e.NewValue);

                //Rotate the text element
                TransformElements();

                //Set the ProgressOrientation of the WaitingBarSeparatorElement
                this.ContentElement.WaitingDirection = (ProgressOrientation)e.NewValue;
            }
            
            if (e.Property == WaitingStepProperty)
            {
                int step = (int)e.NewValue;
                if (step == 0 && this.WaitingSpeed != 0 && IsWaiting)
                {
                    StopWaiting();
                    continueWaiting = true;
                }
                if(step != 0 && this.WaitingSpeed != 0 && continueWaiting)
                {
                    this.StartWaiting();
                    continueWaiting = false;
                }
            }

            if (e.Property == WaitingSpeedProperty)
            {
                int speed = (int)e.NewValue;
                if (speed == 0 && this.WaitingStep != 0 && IsWaiting)
                {
                    this.StopWaiting();
                    continueWaiting = true;
                }
                if (speed != 0 && this.WaitingStep != 0 && IsWaiting)
                {
                    timer.Interval = 100 - speed + 1;
                }
                if (speed != 0 && this.WaitingStep != 0 && !IsWaiting && continueWaiting)
                {
                    timer.Interval = 100 - speed + 1;
                    this.StartWaiting();
                    continueWaiting = false;
                }
            }

            if (e.Property == EnabledProperty)
            {
                if(!(bool)e.NewValue && this.IsWaiting)
                {
                    this.StopWaiting();
                }
            }
        }

        #endregion
    }
}
