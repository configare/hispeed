using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Styles;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    /// <summary>Implements the basic functionality for scrolling.</summary>
    /// <remarks>
    /// 	<para>
    ///         This class can be used both for horizontal and for vertical scrolling via its
    ///         property <see cref="ScrollType"/>. In the Toolbox only the specialized
    ///         children are put: <see cref="RadHScrollBar"/> and
    ///         <see cref="RadVScrollBar"/>.
    ///     </para>
    /// 	<para>
    ///         To adjust the value range of the scroll bar control, set the
    ///         <see cref="Minimum"/> and <see cref="Maximum"/> properties. To adjust
    ///         the distance the scroll thumb moves, set the <see cref="SmallChange"/> and
    ///         <see cref="LargeChange"/> properties. To adjust the starting point of the
    ///         scroll thumb, set the <see cref="Value"/> property when the control is
    ///         initially displayed.
    ///     </para>
    /// </remarks>
    [DefaultProperty("Value")]
    [DefaultEvent("Scroll")]
	[ToolboxItem(false), ComVisible(false)]
	public class RadScrollBarElement : RadItem
    {
        #region Fields

        /// <summary>
        /// <see cref="RadScrollBar.ScrollType"/>
        /// </summary>
        public const ScrollType DefaultScrollType = ScrollType.Horizontal;

        private const float DefaultAngleCorrection = -90.0f;
        private const int DefaultMaximum = 100;
        private const int DefaultMinimum = 0;
        private const int DefaultValue = 0;
        private const int DefaultSmallChange = 1;
        private const int DefaultLargeChange = 10;
        private const double DefaultThumbLengthProportion = -1.0;

        private Size thumbDragDelta = Size.Empty;
        private Point lastMouseDownLocation;
        private Rectangle clientRect;
        private Size firstButtonSize;
        private Size secondButtonSize;
        private Size thumbSize;
        private FillPrimitive background;
        private FillPrimitive pressedForeground;
        private ScrollBarButton firstButton;
        private ScrollBarButton secondButton;
        private ScrollBarThumb thumb;
        private Timer scrollDelay;
        private Timer scrollTimer;
        private ScrollEventType? currentScroll;
        private BorderPrimitive borderElement;
        private bool supressScrollParameterChangedEvent;
        private double thumbLengthProportion = double.NaN;
        private bool protectPressedProperty = true;
        private int thumbLength = 0;
        private int maximum = DefaultMaximum;
        private int minimum = DefaultMinimum;
        private int value = DefaultValue;
        private int smallChange = DefaultSmallChange;        
        private int largeChange = DefaultLargeChange;
        private int scrollTimerDelay = 60;

        #endregion

        #region Constructor, Initialize & Disposal

        static RadScrollBarElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ScrollButtonStateManagerFactory(), typeof(RadScrollBarElement));
            new Themes.ControlDefault.ScrollBar().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.scrollDelay = new Timer();
            this.scrollDelay.Interval = 400;
            this.scrollDelay.Tick += OnScrollDelay;

            this.scrollTimer = new Timer();
            this.scrollTimer.Interval = scrollTimerDelay;
            this.scrollTimer.Tick += OnScrollTimer;
        }

        protected override void DisposeManagedResources()
        {
            this.scrollDelay.Tick -= OnScrollDelay;
            this.scrollTimer.Tick -= OnScrollTimer;
            this.scrollDelay.Dispose();
            this.scrollTimer.Dispose();
            base.DisposeManagedResources();
        }

        protected override void CreateChildElements()
        {
            this.background = new FillPrimitive();
            this.background.Class = "ScrollBarFill";
            this.background.ZIndex = 0;
            this.Children.Add(this.background);

            this.AddBehavior(new GradientAngleBehavior(this.background));

            this.pressedForeground = new FillPrimitive();
            this.pressedForeground.Class = "ScrollBarPressedFill";
            this.pressedForeground.ZIndex = 1;
            this.pressedForeground.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.pressedForeground);

            this.firstButton = new ScrollBarButton(GetFirstButtonDirection(this.ScrollType));
            this.firstButton.NotifyParentOnMouseInput = true;
            this.firstButton.ZIndex = 1;
            this.firstButton.Class = "ScrollBarFirstButton";
            this.firstButton.ThemeRole = "ScrollBarFirstButton";
            this.Children.Add(firstButton);

            //border should be rotated counter other elements
            this.AddBehavior(new GradientAngleBehavior(this.firstButton.ButtonBorder, true));
            this.AddBehavior(new GradientAngleBehavior(this.firstButton.ButtonFill, true));

            this.secondButton = new ScrollBarButton(GetSecondButtonDirection(this.ScrollType));
            this.secondButton.NotifyParentOnMouseInput = true;
            this.secondButton.ZIndex = 1;
            this.secondButton.Class = "ScrollBarSecondButton";
            this.secondButton.ThemeRole = "ScrollBarSecondButton";
            this.Children.Add(secondButton);

            //border should be rotated counter other elements
            this.AddBehavior(new GradientAngleBehavior(this.secondButton.ButtonBorder, true));
            this.AddBehavior(new GradientAngleBehavior(this.secondButton.ButtonFill, true));

            this.thumb = new ScrollBarThumb();
            this.thumb.NotifyParentOnMouseInput = true;
            this.thumb.ZIndex = 1;
            this.thumb.Class = "ScrollBarThumb";
            this.Children.Add(thumb);

            this.AddBehavior(new GradientAngleBehavior(this.thumb.ThumbFill));
            this.AddBehavior(new GradientAngleBehavior(this.thumb.ThumbBorder));

            this.borderElement = new BorderPrimitive();
            this.borderElement.Class = "ScrollBarBorder";
            this.borderElement.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.borderElement);

            this.AddBehavior(new GradientAngleBehavior(this.borderElement));
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.RecalculateAngleCorrection();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the scroll thumb has been moved by either a mouse or keyboard
        /// action.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the user moves the scroll thumb.")]
        public event ScrollEventHandler Scroll;

        /// <summary>
        ///     Occurs when the <see cref="Value"/> property is changed, either by a
        ///     <see cref="Scroll"/> event or programmatically.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the value (i.e. the scroll thumb position) changes.")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when a property that affects the scrolling is changed.
        /// See <see cref="ScrollBarParameters"/> for more information on which properties affect the scrolling.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a scrolling parameter value changes (Maximum, Minimum, LargeChange and SmallChange).")]
        public event EventHandler ScrollParameterChanged;
        
        #endregion

        #region Properties

        public FillPrimitive FillElement
        {
            get { return this.background; }
        }

        public BorderPrimitive BorderElement
        {
            get { return this.borderElement; }
        }

        /// <summary>
        /// Gets the first button element of this scrollbar
        /// </summary>
        public ScrollBarButton FirstButton
        {
            get 
            {
                return this.firstButton; 
            }
        }

        /// <summary>
        /// Gets the second button element of this scrollbar
        /// </summary>
        public ScrollBarButton SecondButton
        {
            get
            {
                return this.secondButton;
            }
        }

        public static readonly RadProperty PressedProperty = RadProperty.Register(
            "Pressed",
            typeof(bool),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static readonly RadProperty IsMouseOverScrollBarProperty = RadProperty.Register(
            "IsMouseOverScrollBar",
            typeof(bool),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.CanInheritValue));

        public static readonly RadProperty GradientAngleCorrectionProperty = RadProperty.Register(
            "GradientAngleCorrection",
            typeof(float),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(DefaultAngleCorrection, ElementPropertyOptions.None));

        public static readonly RadProperty ThumbLengthProportionProperty = RadProperty.Register(
            "ThumbLengthProportion",
            typeof(double),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(DefaultThumbLengthProportion, ElementPropertyOptions.None));

        /// <summary>
        /// Gets or sets a value between 0.0 and 1.0 that indicates what part of the scrollable area
        /// can be occupied by the thumb. If the value is 0.0 then the thumb should be with length 0
        /// but the property MinThumbLength will cause the thumb to be larger.
        /// If the value is 1.0 the the thumb takes the whole area between the two scrolling buttons.
        /// Negative value means that the thumb length should be calculated automatically based on
        /// Minimum, Maximum and LargeChange values.
        /// </summary>
        [RadPropertyDefaultValue("ThumbLengthProportion", typeof(RadScrollBarElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Proportional area occupied by the thumb in the scrolling area of the scroll bar (for values between 0.0 and 0.1; < 0.0 => auto)")]
        public double ThumbLengthProportion
        {
            get
            {
                if (double.IsNaN(this.thumbLengthProportion))
                    this.thumbLengthProportion = (double)this.GetValue(ThumbLengthProportionProperty);
                return this.thumbLengthProportion;
            }
            set
            {
                this.SetValue(ThumbLengthProportionProperty, value);
            }
        }

        public static readonly RadProperty MinThumbLengthProperty = RadProperty.Register(
            "MinThumbLength",
            typeof(int),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(
                10,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        /// 	<para>
        ///         Gets or sets the minimum length of the scrolling thumb. See
        ///         <see cref="ThumbLength"/> for more information about thumb length.
        ///     </para>
        /// </summary>
        /// <value>
        /// An integer value that gives the minimum thumb length. It is taken into account no
        /// matter if the thumb length is calculated automatically or the thumb length is set
        /// explicitly.
        /// The thumb length could be smaller than MinThumbLength if there is no space in the scroll bar.
        /// </value>
        [RadPropertyDefaultValue("MinThumbLength", typeof(RadScrollBarElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Minimum length of the thumb")]
        public int MinThumbLength
        {
            get
            {
                return (int)this.GetValue(MinThumbLengthProperty);
            }
            set
            {
                if (value != this.MinThumbLength)
                {
                    this.SetValue(MinThumbLengthProperty, value);
                    SetupThumb();
                }
            }
        }

        /// <summary>
        /// Gets the length of the scrolling thumb. Thumb length is the thumb's height
        /// for vertical scroll bar and the thumb's width for horizontal scroll bar.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(0)]
        [Description("Thumb length in pixels")]
        public int ThumbLength
        {
            get
            {
                if (this.thumbLength == 0)
                    this.thumbLength = CalcThumbLength(this.ThumbLengthProportion);
                return this.thumbLength;
            }
        }

        /// <summary>
        /// Controls the angle that the fill primitive will be rotated when switching from horizontal to vertical orientation
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]        
        [RadPropertyDefaultValue("GradientAngleCorrection", typeof(RadScrollBarElement))]
        public float GradientAngleCorrection
        {
            get
            {
                return (float)this.GetValue(GradientAngleCorrectionProperty);
            }
            set
            {
                this.SetValue(GradientAngleCorrectionProperty, value);
            }
        }

        /// <summary>Gets or sets the upper limit of the scrollable range.</summary>
        /// <value>A numeric value. The default value is 100.</value>
        /// <remarks>
        /// 	<para>NOTE: The value of a scroll bar cannot reach its maximum value through user
        ///     interaction at run time. The maximum value that can be reached is equal to the
        ///     <b>Maximum</b> property value minus the
        ///     <see cref="LargeChange"/> property
        ///     value plus 1. The maximum value can only be reached programmatically.</para>
        /// </remarks>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("The upper limit value of the scrollable range.")]
        [DefaultValue(DefaultMaximum)]
        public int Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (this.maximum != value)
				{
					if (this.minimum > value)
					{
						this.minimum = value;
					}
					if (value < this.value)
					{
						this.Value = value;
					}
					this.maximum = value;
                    SetupThumb();
                    OnScrollParameterChanged();
                    this.OnNotifyPropertyChanged("Maximum");
				}
			}
		}

        /// <summary>Gets or sets the lower limit for the values of the scrollable range.</summary>
        /// <value>A numeric value. The default value is 0.</value>
        /// <notes>
        /// 	<para>The value of a scroll bar cannot reach its maximum value through user
        ///     interaction at run time. The maximum value that can be reached is equal to the
        ///     <b>Maximum</b> property value minus the
        ///     <see cref="LargeChange"/>property
        ///     value plus 1. The maximum value can only be reached programmatically.</para>
        /// </notes>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("The lower limit value of the scrollable range.")]
        [DefaultValue(DefaultMinimum)]
        public int Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (this.minimum != value)
				{
					if (this.maximum < value)
					{
						this.maximum = value;
					}
					if (value > this.value)
					{
						this.value = value;
					}
					this.minimum = value;
                    SetupThumb();
                    OnScrollParameterChanged();
				}
			}
		}

        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the scroll thumb on
        /// the scroll bar.
        /// </summary>
        /// <value>
        ///     A numeric value that is within the <see cref="Minimum"/> and
        ///     <see cref="Maximum"/> range. The default value is 0.
        /// </value>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("The value that the scroll thumb position represents.")]
        [DefaultValue(DefaultValue)]
        public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					if ((value < this.Minimum) || (value > this.Maximum))
					{
                        string str = string.Format("Value of '{0}' is not valid for 'Value'. 'Value' must be between 'Minimum' and 'Maximum'.", value);
						throw new ArgumentOutOfRangeException("Value", str);
					}
                    this.value = value;
					SetupThumb(value, true);
                    OnValueChanged();
				}
			}
		}

        /// <summary>
        ///     Gets or sets the value to be added to or subtracted from the
        ///     <see cref="Value"/> property when the scroll thumb is moved a small distance.
        /// </summary>
        /// <value>A numeric value. The default value is 1.</value>
        /// <remarks>
        /// 	<para>When the user presses one of the arrow keys, clicks one of the scroll bar
        ///     buttons or calls one of the <b>LineXXX()</b> functions, the <b>Value</b> property changes
        ///     according to the value set in the <b>SmallChange</b> property.</para>
        /// </remarks>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("The amount by which the scroll thumb position changes when a user clicks arrow, presses arrow keys or calls some of LineXXX() functions.")]
        [DefaultValue(DefaultSmallChange)]
        public int SmallChange
		{
			get
			{
				return Math.Min(this.smallChange, this.LargeChange);
			}
			set
			{
				if (this.smallChange != value)
				{
					if (value < 0)
					{
                        string str = string.Format("Value of '{0}' is not valid for 'SmallChange'. 'SmallChange' must be greater than or equal to 0.", value);
						throw new ArgumentOutOfRangeException("SmallChange", str);
					}
					this.smallChange = value;
                    OnScrollParameterChanged();
				}
			}
		}

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the
        /// <see cref="Value"/> property when the scroll
        /// thumb is moved a large distance.
        /// </summary>
        /// <value>A numeric value. The default value is 10.</value>
        /// <remarks>
        /// When the user presses the PAGE UP or PAGE DOWN key, clicks in the scroll bar
        /// track on either side of the scroll thumb, or calls one of the <b>PageXXX()</b> functions, the
        /// <b>Value</b> property changes according to the value set in the <b>LargeChange</b>
        /// property.
        /// </remarks>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("The amount by which the scroll thumb position changes when a user clicks the scroll bar, presses PageUP/PageDown keys or calls some of PageXXX() functions.")]
        [DefaultValue(DefaultLargeChange)]
        public int LargeChange
		{
			get
			{
				return Math.Min(this.largeChange, (this.Maximum - this.Minimum + 1));
			}
			set
			{
				if (this.largeChange != value)
				{
					if (value < 0)
					{
                        string str = string.Format("Value of '{0}' is not valid for 'LargeChange'. 'LargeChange' must be greater than or equal to 0.", value);
						throw new ArgumentOutOfRangeException("LargeChange", str);
					}
                    this.largeChange = value;
                    SetupThumb();
                    OnScrollParameterChanged();
				}
			}
		}

        public static readonly RadProperty ScrollTypeProperty = RadProperty.RegisterAttached(
            "ScrollType",
            typeof(ScrollType),
            typeof(RadScrollBarElement),
            new RadElementPropertyMetadata(
                DefaultScrollType,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue));

        /// <summary>
        ///     Gets or sets the <see cref="ScrollType">scroll type</see> - it could be horizontal
        ///     or vertical.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies whether the scroll bar should be horizontal or vertical.")]
        [RadPropertyDefaultValue("ScrollType", typeof(RadScrollBarElement))]
		public ScrollType ScrollType
		{
			get
			{
                return (ScrollType)GetValue(ScrollTypeProperty);
			}
			set
			{
                SetValue(ScrollTypeProperty, value);
			}
		}

        /// <summary>
        /// Gets the thumb element of this scrollbar
        /// </summary>
        [Description("Gets the thumb element of this scrollbar")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollBarThumb ThumbElement
        {
            get
            {
                return this.thumb;
            }
        }

        /// <summary>
        /// Gets or sets the scroll timer delay
        /// </summary>
        [Description("Gets or sets the scroll timer delay")]
        [Browsable(false)]
        [DefaultValue(60)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollTimerDelay
        {
            get { return this.scrollTimerDelay; }
            set             
            {
                if (value != this.scrollTimerDelay)
                {
                    this.scrollTimerDelay = value;
                    if (this.scrollTimer != null)
                    {
                        this.scrollTimer.Interval = value;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Retrieves the srolling parameters.</summary>
        /// <seealso cref="ScrollBarParameters">ScrollBarParameters Structure</seealso>
        public ScrollBarParameters GetParameters()
        {
            return new ScrollBarParameters(this.Minimum, this.Maximum, this.SmallChange, this.LargeChange);
        }

        /// <summary>Sets the given scroll parameters.</summary>
        /// <seealso cref="ScrollBarParameters">ScrollBarParameters Structure</seealso>
        public void SetParameters(ScrollBarParameters parameters)
        {
            this.supressScrollParameterChangedEvent = true;
            this.Minimum = parameters.Minimum;
            this.Maximum = parameters.Maximum;
            this.SmallChange = parameters.SmallChange;
            this.LargeChange = parameters.LargeChange;
            this.supressScrollParameterChangedEvent = false;
            OnScrollParameterChanged();
        }

        /// <summary>
        /// Simulate scrolling - just like the top / left button is pressed.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        /// <param name="numSteps">Value is decremented with (numSteps * SmallChange)</param>
        public void PerformSmallDecrement(int numSteps)
        {
            PerformScrollWith(numSteps, ScrollEventType.SmallDecrement);
        }

        /// <summary>
        /// Simulate scrolling - just like the bottom / right button is pressed.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        /// <param name="numSteps">Value is incremented with (numSteps * SmallChange)</param>
        public void PerformSmallIncrement(int numSteps)
        {
            PerformScrollWith(numSteps, ScrollEventType.SmallIncrement);
        }

        /// <summary>
        /// Simulate scrolling - just like the top / left area according the thumb is pressed.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        /// <param name="numSteps">Value is decremented with (numSteps * LargeChange)</param>
        public void PerformLargeDecrement(int numSteps)
        {
            PerformScrollWith(numSteps, ScrollEventType.LargeDecrement);
        }

        /// <summary>
        /// Simulate scrolling - just like the bottom / right area according the thumb is pressed.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        /// <param name="numSteps">Value is incremented with (numSteps * LargeChange)</param>
        public void PerformLargeIncrement(int numSteps)
        {
            PerformScrollWith(numSteps, ScrollEventType.LargeIncrement);
        }

        /// <summary>
        /// Simulate scrolling with positioning the thumb on its first position.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        public void PerformFirst()
        {
            PerformScrollToValue(this.Minimum, ScrollEventType.First);
        }

        /// <summary>
        /// Simulate scrolling with positioning the thumb on its last position.
        /// Unlike setting property Value this function fires scrolling events.
        /// </summary>
        public void PerformLast()
        {
            PerformScrollToValue(this.Maximum, ScrollEventType.Last);
        }

        /// <summary>Scrolls just like the thumb is dragged at given position</summary>
        /// <param name="position">Position of the thumb (in screen coordinates).</param>
        public void PerformScrollTo(Point position)
        {
            if (this.Enabled)
            {
                FireThumbTrackMessage();
                Point insidePosition = PointFromScreen(position);
                SetThumbPosition(insidePosition, true);
                SetThumbPosition(insidePosition, false);
            }
        }

        #endregion

        #region Event handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.lastMouseDownLocation = e.Location;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                currentScroll = GetScrollType(e.Location);
                // The check for thumb capture is patch: VisualStyleBuilder->Tree->Horizontal scrollbar.
                // When the scrollbar is shown by default and the thumb i sdragged fast - we get here even though
                // the thumb has captured the mouse input. That problem seems to appera only in that case...
                if (/*!this.thumb.Capture &&*/ currentScroll != null)
                {
                    if (!this.UseNewLayoutSystem)
                        SuspendLayout();
                    this.Capture = true;
                    if (currentScroll == ScrollEventType.SmallDecrement)
                        this.firstButton.SetValue(RadButtonItem.IsPressedProperty, true);
                    else if (currentScroll == ScrollEventType.SmallIncrement)
                        this.secondButton.SetValue(RadButtonItem.IsPressedProperty, true);
                    OnScrollTimer(this, new EventArgs());
                    this.scrollDelay.Start();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!this.Capture)
            {
                if (this.scrollTimer.Enabled)
                {
                    this.scrollTimer.Stop();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.scrollTimer.Stop();
                this.scrollDelay.Stop();

                this.Capture = false;
                if (currentScroll == ScrollEventType.SmallDecrement)
                    this.firstButton.SetValue(RadButtonItem.IsPressedProperty, false);
                else if (currentScroll == ScrollEventType.SmallIncrement)
                    this.secondButton.SetValue(RadButtonItem.IsPressedProperty, false);
                EndScroll(this.Value);
                if (!this.UseNewLayoutSystem)
                    ResumeLayout(true);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PressedProperty && this.protectPressedProperty)
            {
                throw new InvalidOperationException("The property \"PressedProperty\" cannot be set directly");
            }

            if (e.Property == ScrollTypeProperty)
            {
                ScrollType newValue = (ScrollType)e.NewValue;
                if (this.firstButton != null)
                {
                    this.firstButton.Direction = GetFirstButtonDirection(newValue);
                }
                if (this.secondButton != null)
                {
                    this.secondButton.Direction = GetSecondButtonDirection(newValue);
                }

                this.RecalculateAngleCorrection();
            }
            else if (e.Property == GradientAngleCorrectionProperty)
            {
                this.RecalculateAngleCorrection();
            }
            else if (e.Property == IsMouseOverElementProperty)
            {
                this.SetValue(IsMouseOverScrollBarProperty, (bool)e.NewValue);
            }
            else if (e.Property == ThumbLengthProportionProperty)
            {
                this.thumbLengthProportion = double.NaN;
                this.SetupThumb();
            }
        }

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }

        protected virtual void OnScroll(ScrollEventArgs args)
        {
            if (Scroll != null)
            {
                Scroll(this, args);
            }
        }

        protected virtual void OnScrollParameterChanged()
        {
            if (!supressScrollParameterChangedEvent && ScrollParameterChanged != null)
            {
                ScrollParameterChanged(this, new EventArgs());
            }
        }

        private void OnScrollDelay(object sender, EventArgs e)
        {
            this.scrollDelay.Stop();
            OnScrollTimer(sender, e);
            this.scrollTimer.Start();
        }

        private void OnScrollTimer(object sender, EventArgs e)
        {
            if (this.Enabled && this.currentScroll != null)
            {
                ScrollEventType? nullableScrollType = GetScrollType(this.lastMouseDownLocation);

                if (nullableScrollType != null && nullableScrollType == this.currentScroll)
                {
                    ScrollEventType scrollType = (ScrollEventType)nullableScrollType;
                    ScrollWith(GetStepFromScrollEventType(scrollType), scrollType);
                }
            }
        }

        #endregion

        #region Layout

        protected override void SetNewLayoutSystem(bool useNewLayoutSystem)
        {
            bool oldLayoutSystemMode = this.UseNewLayoutSystem;

            base.SetNewLayoutSystem(useNewLayoutSystem);

            if (this.firstButton == null || this.secondButton == null)
                return;

            if (!oldLayoutSystemMode && useNewLayoutSystem)
            {
                this.RecalculateAngleCorrection();
                //Removed for Q1 2009
                ////undo old layout setting
                ////UnApplyOldLayoutScrollType(this.background, false);
                //UnApplyOldLayoutScrollType(this.firstButton.Children[0], false);
                //UnApplyOldLayoutScrollType(this.secondButton.Children[0], false);
                //UnApplyOldLayoutScrollType(this.thumb.Children[0], false);

                ////apply new layout
                //UnApplyOldLayoutScrollType(this.background, true);
                //UnApplyOldLayoutScrollType(this.firstButton.Children[0], true);
                //UnApplyOldLayoutScrollType(this.secondButton.Children[0], true);
                //UnApplyOldLayoutScrollType(this.thumb.Children[0], true);
            }
        }

        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.PerformLayout" filter=""/>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            //base.PerformLayoutCore(affectedElement);

            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list);

            //DumpLayoutInfo("before", affectedElement);

            this.background.Size = this.Size;
            this.borderElement.Size = this.Size;
            SetupThumb();
            SetupButtons();

            //DumpLayoutInfo("after", affectedElement);
        }

        /// <summary>
        /// Retrieves size structure. If the proposed size is not valid, the minimum possible
        /// size is retrieved.
        /// </summary>
        /// <exclude/>
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = Parent.Size;// Size.Empty;
            /*
            if (this.IsPropagateToChildren())
            {
                res = base.GetPreferredSizeCore(proposedSize);
            }
            else
            {
                //List<PreferredSizeData> list = new List<PreferredSizeData>();
                //FillList(list);

                if (this.ScrollType == ScrollType.Horizontal)
                {
                    res.Width = firstButton.Size.Width + thumb.Size.Width + secondButton.Size.Width;
                    res.Height = Math.Max(
                        Math.Max(firstButton.Size.Height, thumb.Size.Height),
                        secondButton.Size.Height);
                }
                else
                {
                    res.Width = Math.Max(
                        Math.Max(firstButton.Size.Width, thumb.Size.Width),
                        secondButton.Size.Width);
                    res.Height = firstButton.Size.Height + thumb.Size.Height + secondButton.Size.Height;
                }
            }
            */
            return res;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF totalSize = SizeF.Empty;

            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                SizeF availableSizeForChild = availableSize;
                if (child is ScrollBarButton)
                {
                    if (this.ScrollType == ScrollType.Horizontal)
                        availableSizeForChild.Width /= 2;
                    else
                        availableSizeForChild.Height /= 2;
                }
                child.Measure(availableSizeForChild);
            }

            totalSize = AccumulateDesiredSize(totalSize, this.firstButton);
            totalSize = AccumulateDesiredSize(totalSize, this.thumb);
            totalSize = AccumulateDesiredSize(totalSize, this.secondButton);

            totalSize = SizeF.Add(totalSize, this.Padding.Size);
            totalSize = SizeF.Add(totalSize, this.BorderThickness.Size);

            return totalSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (!this.IsInValidState(true))
            {
                return finalSize;
            }

            // Border and backgrond
            RectangleF availableRect = new RectangleF(PointF.Empty, finalSize);
            this.borderElement.Arrange(availableRect);

            //Changes for Q1 2009
            //Scollbar buttons and thumbs should appear over top/left border
            RectangleF outerRect = availableRect;

            if (this.background.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
            {
                this.background.Arrange(availableRect);
            }

            availableRect.Location = PointF.Add(availableRect.Location, new SizeF(this.BorderThickness.Left, this.BorderThickness.Top));
            availableRect.Size = SizeF.Subtract(availableRect.Size, this.BorderThickness.Size);

            if (this.background.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
            {
                this.background.Arrange(availableRect);
            }

            availableRect.Location = PointF.Add(availableRect.Location, new SizeF(this.Padding.Left, this.Padding.Top));
            availableRect.Size = SizeF.Subtract(availableRect.Size, this.Padding.Size);

            if (this.background.FitToSizeMode == RadFitToSizeMode.FitToParentContent)
            {
                this.background.Arrange(availableRect);
            }

            //Changes for Q1 2009
            //Scollbar buttons and thumbs should appear over at least the top/left border
            availableRect.Location = PointF.Subtract(availableRect.Location, new SizeF(this.BorderThickness.Left, this.BorderThickness.Top));
            availableRect.Size = SizeF.Add(availableRect.Size, this.BorderThickness.Size);

            // Memberss
            this.clientRect = Rectangle.Round(availableRect);

            if (this.ScrollType == ScrollType.Horizontal)
            {
                //Fix: square scrollbar buttons, unless padding is specified
                float buttonSide = availableRect.Height;
                float buttonWidth = buttonSide + Math.Max(this.firstButton.Padding.Horizontal, this.firstButton.Padding.Vertical);
                this.firstButtonSize = Size.Round(new SizeF(buttonWidth, buttonSide));

                //buttonSide = Math.Min(this.secondButton.DesiredSize.Width, availableRect.Height);

                this.secondButtonSize = Size.Round(new SizeF(buttonWidth, buttonSide));

                this.thumbSize = Size.Round(new SizeF((float)this.ThumbLength, availableRect.Height));
            }
            else
            {
                float buttonSide = availableRect.Width;
                float buttonHeight = buttonSide + Math.Max(this.firstButton.Padding.Horizontal, this.firstButton.Padding.Vertical);
                this.firstButtonSize = Size.Round(new SizeF(buttonSide, buttonHeight));

                buttonSide = Math.Max(availableRect.Width, this.secondButton.DesiredSize.Height);
                //float buttonWidth = buttonSide + Math.Max(this.firstButton.Padding.Horizontal, this.firstButton.Padding.Vertical);
                this.secondButtonSize = Size.Round(new SizeF(buttonSide, buttonHeight));

                this.thumbSize = Size.Round(new SizeF(availableRect.Width, (float)this.ThumbLength));
            }

            // The real arrange
            PointF thumbPos = ValueToThumbPos(this.value);
            thumbPos = PointF.Add(thumbPos, this.thumbDragDelta);
            this.thumbLength = 0;

            if (this.ScrollType == ScrollType.Horizontal)
            {
                this.firstButton.Arrange(new RectangleF(
                    availableRect.X, availableRect.Y,
                    this.firstButtonSize.Width, this.firstButtonSize.Height));
                this.secondButton.Arrange(new RectangleF(
                    availableRect.Right - this.secondButtonSize.Width, availableRect.Y,
                    this.secondButtonSize.Width, this.secondButtonSize.Height));

                this.thumb.Arrange(new RectangleF(
                    //availableRect.X + this.firstButtonSize.Width, availableRect.Y,
                    thumbPos.X, thumbPos.Y,
                    this.thumbSize.Width, this.thumbSize.Height));

                this.pressedForeground.Arrange(availableRect);
            }
            else
            {
                this.firstButton.Arrange(new RectangleF(
                    availableRect.X, availableRect.Y,
                    this.firstButtonSize.Width, this.firstButtonSize.Height));
                this.secondButton.Arrange(new RectangleF(
                    availableRect.X, availableRect.Bottom - this.secondButtonSize.Height,
                    this.secondButtonSize.Width, this.secondButtonSize.Height));

                this.thumb.Arrange(new RectangleF(
                    //availableRect.X, availableRect.Y + this.firstButtonSize.Height,
                    thumbPos.X, thumbPos.Y,
                    this.thumbSize.Width, this.thumbSize.Height));

                this.pressedForeground.Arrange(availableRect);
            }

            return finalSize;
        }

        private SizeF AccumulateDesiredSize(SizeF prevDesiredSize, RadElement element)
        {
            SizeF elementSize = element.DesiredSize;

            if (this.ScrollType == ScrollType.Horizontal)
            {
                prevDesiredSize.Width += elementSize.Width;
                if (prevDesiredSize.Height < elementSize.Height)
                    prevDesiredSize.Height = elementSize.Height;
            }
            else
            {
                if (prevDesiredSize.Width < elementSize.Width)
                    prevDesiredSize.Width = elementSize.Width;
                prevDesiredSize.Height += elementSize.Height;
            }

            return prevDesiredSize;
        }

        private int CalcThumbLength(double thumbProportion)
        {
            int scrollingPixels = GetScrollingPixels(false);
            int res = 0;
            if (thumbProportion < 0.0)
            {
                res = 0;
                int deltaValue = this.Maximum - this.Minimum + 1;
                if (deltaValue > 1)
                {
                    res = (int)Math.Round((double)this.LargeChange * scrollingPixels / deltaValue);
                }
                else if (deltaValue == 1)
                {
                    res = scrollingPixels;
                }
            }
            else
            {
                if (thumbProportion >= 1.0)
                    res = scrollingPixels;
                else
                    res = (int)Math.Round(thumbProportion * scrollingPixels);
            }

            res = Math.Max(res, this.MinThumbLength);
            res = Math.Min(res, scrollingPixels);
            return res;
        }

        #endregion

        #region Thumb Methods

        internal void FireThumbTrackMessage()
        {
            int currentValue = this.Value;
            CallOnScroll(ScrollEventType.ThumbTrack, currentValue, currentValue);
        }

        internal void SetThumbPosition(Point thumbLocation, bool dragging)
        {
            if (this.UseNewLayoutSystem)
            {
                int currentValue = this.Value;

                int newValue = ThumbPosToValue(thumbLocation);
                int scrollMax = GetScrollMax();
                if (newValue > scrollMax)
                {
                    newValue = scrollMax;
                }
                else if (newValue < this.Minimum)
                {
                    newValue = this.Minimum;
                }

                if (dragging)
                {
                    //Console.WriteLine("SetThumbPosition(): thumbLocation = {0}", thumbLocation);
                    //this.thumbDragDelta = new Size(thumbLocation);
                    //this.ArrangeOverride(this.Size);

                    if (newValue != currentValue)
                    {
                        CallOnScroll(ScrollEventType.ThumbTrack, currentValue, newValue);
                        this.value = newValue;
                        OnValueChanged();
                        this.ArrangeOverride(this.Size);
                    }
                }
                else
                {
                    if (newValue != currentValue)
                    {
                        //Console.WriteLine("SetThumbPosition(): current = {0}; new = {1}", this.Value, newValue);
                        CallOnScroll(ScrollEventType.ThumbPosition, currentValue, newValue);
                        this.Value = newValue;
                    }
                    CallOnScroll(ScrollEventType.ThumbPosition, newValue, newValue);
                    EndScroll(newValue);
                }
            }
            else
            {
                if (dragging)
                {
                    Point newThumbLocation = thumbLocation;
                    if (this.ScrollType == ScrollType.Horizontal)
                    {
                        newThumbLocation = new Point(thumbLocation.X, thumb.Location.Y);
                        int min = this.firstButton.Bounds.Right;
                        int max = this.secondButton.Bounds.Left - thumb.Size.Width;
                        if (newThumbLocation.X < min)
                        {
                            newThumbLocation.X = min;
                        }
                        else if (newThumbLocation.X > max)
                        {
                            newThumbLocation.X = max;
                        }
                    }
                    else
                    {
                        newThumbLocation = new Point(thumb.Location.X, thumbLocation.Y);
                        int min = this.firstButton.Bounds.Bottom;
                        int max = this.secondButton.Bounds.Top - thumb.Size.Height;
                        if (newThumbLocation.Y < min)
                        {
                            newThumbLocation.Y = min;
                        }
                        else if (newThumbLocation.Y > max)
                        {
                            newThumbLocation.Y = max;
                        }
                    }

                    //Console.WriteLine("SetThumbPosition() - before new pos: currentPos = {0}; new pos = {1}; value = {2}",
                    //    thumb.Location.Y, newThumbLocation.Y, this.Value);
                    thumb.Location = newThumbLocation;
                    //Console.WriteLine("SetThumbPosition() - after new pos: currentPos = {0}; new pos = {1}; value = {2}",
                    //    thumb.Location.Y, newThumbLocation.Y, this.Value);

                    int newValue = ThumbPosToValue(newThumbLocation);
                    int currentValue = this.Value;
                    if (newValue != currentValue)
                    {
                        CallOnScroll(ScrollEventType.ThumbTrack, currentValue, newValue);
                        this.value = newValue;
                        OnValueChanged();
                    }
                }
                else
                {
                    int newValue = ThumbPosToValue(thumbLocation);
                    int scrollMax = GetScrollMax();
                    if (newValue > scrollMax)
                    {
                        newValue = scrollMax;
                    }
                    else if (newValue < this.Minimum)
                    {
                        newValue = this.Minimum;
                    }

                    if (newValue == this.Value)
                    {
                        SetupThumb(newValue, true);
                    }
                    else
                    {
                        int currentValue = this.Value;
                        //Console.WriteLine("SetThumbPosition(): current = {0}; new = {1}", this.Value, newValue);
                        CallOnScroll(ScrollEventType.ThumbPosition, currentValue, newValue);
                        this.Value = newValue;
                    }
                    CallOnScroll(ScrollEventType.ThumbPosition, newValue, newValue);
                    EndScroll(newValue);
                }
            }
        }

        #endregion

		#region Scroll Methods

        private void PerformScrollWith(int numSteps, ScrollEventType scrollType)
        {
            if (this.Enabled)
            {
                // ScrollWith() fires Scroll and ValueChnaged
                ScrollWith(numSteps * GetStepFromScrollEventType(scrollType), scrollType);
                EndScroll(this.Value);
            }
        }

        private void PerformScrollToValue(int newValue, ScrollEventType scrollType)
        {
            if (this.Enabled)
            {
                // ScrollWith() fires Scroll and ValueChnaged
                ScrollTo(newValue, scrollType);
                EndScroll(this.Value);
            }
        }

        private void ScrollWith(int step, ScrollEventType scrollType)
        {
            int oldValue = this.Value;
            int newValue = oldValue + step;
            if (step > 0)
            {
                int scrollMax = GetScrollMax();
                if (newValue > scrollMax)
                {
                    newValue = scrollMax;
                }
            }
            else
            {
                if (newValue > this.Maximum)
                {
                    newValue = this.Maximum;
                }
            }
            if (newValue < this.Minimum)
            {
                newValue = this.Minimum;
            }

            CallOnScroll(scrollType, oldValue, newValue);
            this.Value = newValue;
        }

        private void ScrollTo(int newValue, ScrollEventType scrollType)
        {
            int oldValue = this.Value;

            int scrollMax = GetScrollMax();
            if (newValue > scrollMax)
            {
                newValue = scrollMax;
            }
            if (newValue < this.Minimum)
            {
                newValue = this.Minimum;
            }

            CallOnScroll(scrollType, oldValue, newValue);

            this.Value = newValue;
        }

        private void EndScroll(int endValue)
        {
            CallOnScroll(ScrollEventType.EndScroll, endValue, endValue);
            currentScroll = null;
        }

        private ScrollButtonDirection GetFirstButtonDirection(ScrollType scrType)
        {
            return scrType == ScrollType.Horizontal ?
                ScrollButtonDirection.Left : ScrollButtonDirection.Up;
        }

        private ScrollButtonDirection GetSecondButtonDirection(ScrollType scrType)
        {
            return scrType == ScrollType.Horizontal ?
                ScrollButtonDirection.Right : ScrollButtonDirection.Down;
        }
		
        #endregion

        #region Support methods

        private bool IsHorizontalRTL()
        {
            return this.ScrollType == ScrollType.Horizontal && this.RightToLeft;
        }

        private int GetScrollMax()
        {
            if (this.LargeChange > 0)
                return this.Maximum - this.LargeChange + 1;
            return this.Maximum;
        }

        private int GetDeltaValue()
        {
            int res = GetScrollMax() - this.Minimum;
            if (res < 0)
                throw new ArithmeticException("ScrollBar: ScrollMax - minimum = " + res.ToString());
            return res;
        }

        // Get the number of pixels for scrolling
        // (the number of different positions the thumb can have)
        private int GetScrollingPixels(bool excludeThumb)
        {
            int res = 0;

            if (this.UseNewLayoutSystem)
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    res = this.clientRect.Width - this.firstButtonSize.Width - this.secondButtonSize.Width;
                    if (excludeThumb)
                    {
                        res -= this.thumbSize.Width;
                    }
                }
                else
                {
                    res = this.clientRect.Height - this.firstButtonSize.Height - this.secondButtonSize.Height;
                    if (excludeThumb)
                    {
                        res -= this.thumbSize.Height;
                    }
                }
            }
            else
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    res = this.Size.Width - firstButton.Size.Width - secondButton.Size.Width;
                    if (excludeThumb)
                    {
                        res -= thumb.Size.Width;
                    }
                }
                else
                {
                    res = this.Size.Height - firstButton.Size.Height - secondButton.Size.Height;
                    if (excludeThumb)
                    {
                        res -= thumb.Size.Height;
                    }
                }
            }
            return res;
        }

        private int ValueToScrollPixel(int value)
        {
            int res = 0;
            
            int scrollingPixels = GetScrollingPixels(true);
            if (scrollingPixels > 0)
            {
                if (value < this.Minimum)
                {
                    res = 0;
                }
                else if (value < GetScrollMax())
                {
                    int deltaValue = GetDeltaValue();
                    if (deltaValue != 0)
                        res = (int)Math.Round((double)(value - this.Minimum) * scrollingPixels / deltaValue);
                    else
                        res = 0;
                }
                else
                {
                    res = scrollingPixels;
                }
                
                if (this.IsHorizontalRTL())
                {
                    res = scrollingPixels - res;
                }
            }

            return res;
        }

        private int ScrollPixelToValue(int scrollPos)
        {
            int scrollingPixels = GetScrollingPixels(true);
            if (scrollingPixels <= 0)
                return this.Minimum;

            int deltaValue = GetDeltaValue();
            if (deltaValue == 0)
                return this.Minimum;

            if (this.IsHorizontalRTL())
            {
                scrollPos = scrollingPixels - scrollPos;
            }

            int res = (int)Math.Round((double)scrollPos * deltaValue / scrollingPixels);

            return res + this.Minimum;
        }

        private Point ValueToThumbPos(int value)
        {
            Point res = Point.Empty;

            int logicalPos = ValueToScrollPixel(value);

            // Fill the result point
            if (this.UseNewLayoutSystem)
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    res.X = this.firstButtonSize.Width + logicalPos;
                    res.Y = 0;
                }
                else
                {
                    res.X = 0;
                    res.Y = this.firstButtonSize.Height + logicalPos;
                }
            }
            else
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    res.X = firstButton.Bounds.Right + logicalPos;
                    res.Y = 0;
                }
                else
                {
                    res.X = 0;
                    res.Y = firstButton.Bounds.Bottom + logicalPos;
                }
            }

            res.X += this.clientRect.X;
            res.Y += this.clientRect.Y;

            return res;
        }

        private int ThumbPosToValue(Point thumbPos)
        {
            int logicalPos;
            if (this.UseNewLayoutSystem)
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    logicalPos = thumbPos.X - this.clientRect.X - this.firstButtonSize.Width;
                }
                else
                {
                    logicalPos = thumbPos.Y - this.clientRect.Y - this.firstButtonSize.Height;
                }
            }
            else
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    logicalPos = thumbPos.X - firstButton.Bounds.Right;
                }
                else
                {
                    logicalPos = thumbPos.Y - firstButton.Bounds.Bottom;
                }
            }

            int res = ScrollPixelToValue(logicalPos);

            return res;
        }

        private ScrollEventType? GetScrollType(Point mouseLocation)
        {
            Point mousePos = mouseLocation;
            
            ScrollEventType? res = null;

            if (this.UseNewLayoutSystem)
            {
                if (this.thumb.ControlBoundingRectangle.Contains(mouseLocation))
                {
                    return null;
                }
                if (this.firstButton.ControlBoundingRectangle.Contains(mouseLocation))
                {
                    return this.IsHorizontalRTL() ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
                }
                else if (this.secondButton.ControlBoundingRectangle.Contains(mouseLocation))
                {
                    return this.IsHorizontalRTL() ? ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement;
                }

                Rectangle thumbRect = this.thumb.ControlBoundingRectangle;

                if (this.ScrollType == ScrollType.Horizontal)
                {
                    if (mousePos.X < thumbRect.Left)
                    {
                        res = this.RightToLeft ? ScrollEventType.LargeIncrement : ScrollEventType.LargeDecrement;
                    }
                    else if (mousePos.X > thumbRect.Right)
                    {
                        res = this.RightToLeft ? ScrollEventType.LargeDecrement : ScrollEventType.LargeIncrement;
                    }
                }
                else
                {
                    if (mousePos.Y < thumbRect.Top)
                    {
                        res = ScrollEventType.LargeDecrement;
                    }
                    else if (mousePos.Y > thumbRect.Bottom)
                    {
                        res = ScrollEventType.LargeIncrement;
                    }
                }
            }
            else
            {
                // The mouse must be inside the ScrollBar
                Rectangle clientRect = new Rectangle(new Point(0, 0), this.Size);
                if (!clientRect.Contains(mousePos))
                {
                    return null;
                }

                // Thumb and buttons
                if (this.thumb.Bounds.Contains(mousePos))
                {
                    return null;
                }
                if (this.firstButton.Bounds.Contains(mousePos))
                {
                    return this.IsHorizontalRTL() ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
                }
                else if (this.secondButton.Bounds.Contains(mousePos))
                {
                    return this.IsHorizontalRTL() ? ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement;
                }

                // Page Up / Down
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    if (mousePos.X < this.thumb.Location.X)
                    {
                        res = this.RightToLeft ? ScrollEventType.LargeIncrement : ScrollEventType.LargeDecrement;
                    }
                    else if (mousePos.X > this.thumb.Bounds.Right)
                    {
                        res = this.RightToLeft ? ScrollEventType.LargeDecrement : ScrollEventType.LargeIncrement;
                    }
                }
                else
                {
                    if (mousePos.Y < this.thumb.Location.Y)
                    {
                        res = ScrollEventType.LargeDecrement;
                    }
                    else if (mousePos.Y > this.thumb.Bounds.Bottom)
                    {
                        res = ScrollEventType.LargeIncrement;
                    }
                }
            }

            return res;
        }

        private int GetStepFromScrollEventType(ScrollEventType scrollEventType)
        {
            switch (scrollEventType)
            {
                case ScrollEventType.LargeIncrement:
                    return this.LargeChange;
                case ScrollEventType.LargeDecrement:
                    return -this.LargeChange;
                case ScrollEventType.SmallIncrement:
                    return this.SmallChange;
                case ScrollEventType.SmallDecrement:
                    return -this.SmallChange;
            }
            return 0;
        }

        private void CallOnScroll(ScrollEventType scrollType, int oldValue, int newValue)
        {
            ScrollOrientation scrollOrientation = this.ScrollType == ScrollType.Vertical ?
                ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;
            OnScroll(new ScrollEventArgs(scrollType, oldValue, newValue, scrollOrientation));
        }

        private void RecalculateAngleCorrection()
        {
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            foreach (PropertyChangeBehavior behavior in this.GetBehaviors())
            {
                GradientAngleBehavior angleBehavior = behavior as GradientAngleBehavior;
                if (angleBehavior != null)
                {
                    angleBehavior.RecalculateOnGradientAngleCorrectionChanged();
                }
            }
        }

        private void FillList(List<PreferredSizeData> list)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, this.Size));
            }
        }

        private void DumpLayoutInfo(string str, RadElement affectedElement)
        {
            if (this.ScrollType == ScrollType.Horizontal)
                Console.Write("Horiz ", affectedElement);
            else
                Console.Write("Vert ", affectedElement);
            Console.Write(str);
            if (Object.ReferenceEquals(this, affectedElement))
            {
                Console.Write(" [THIS]");
            }
            else if (Object.ReferenceEquals(this.Parent, affectedElement))
            {
                Console.Write(" [PARENT]");
            }
            else if (Object.ReferenceEquals(this.firstButton, affectedElement))
            {
                Console.Write(" [FIRST]");
            }
            else if (Object.ReferenceEquals(this.secondButton, affectedElement))
            {
                Console.Write(" [SECOND]");
            }
            else if (Object.ReferenceEquals(this.thumb, affectedElement))
            {
                Console.Write(" [THUMB]");
            }
            else if (Object.ReferenceEquals(this.background, affectedElement))
            {
                Console.Write(" [BACKGROUND]");
            }
            else
            {
                Console.Write(" [UNKNOWN]");
            }
            Console.WriteLine(" ScrollBar = {0}; first = {1}; second = {2}; thumb = {3}",
                this.Size, this.firstButton.Size, this.secondButton.Size, this.thumb.Size);
        }

        private void SetPressed(bool Pressed)
        {
            protectPressedProperty = false;
            this.SetValue(PressedProperty, Pressed);
            protectPressedProperty = true;
        }

        private void SetupThumb()
        {
            SetupThumb(this.Value, false);
        }

        private void SetupThumb(int newValue, bool thumbLocationOnly)
        {
            if (this.UseNewLayoutSystem)
            {
                // Invalidate the thumb length
                this.thumbLength = 0;
                this.ArrangeOverride(new SizeF(this.Size));
            }
            else
            {
                // Set Thumb size first in order ValueToThumbPos() to work correctly
                if (!thumbLocationOnly)
                {
                    this.thumbLength = CalcThumbLength(this.ThumbLengthProportion);
                    Size thumbSize;
                    if (this.ScrollType == ScrollType.Horizontal)
                    {
                        thumbSize = new Size(this.ThumbLength, this.FieldSize.Height);
                    }
                    else
                    {
                        thumbSize = new Size(this.FieldSize.Width, this.ThumbLength);
                    }
                    this.thumb.GetPreferredSize(thumbSize);
                    this.thumb.Size = thumbSize;
                }

                if (!this.thumb.Capture)
                {
                    // ValueToThumbPos() uses thumb size so it must be already set correctly
                    Point thumbPos = ValueToThumbPos(newValue);
                    //Console.WriteLine("SetupThumb() - before new pos: currentPos = {0}; new pos = {1}; value = {2}",
                    //    thumb.Location.Y, thumbPos.Y, newValue);
                    this.thumb.Location = thumbPos;
                    //Console.WriteLine("SetupThumb() - after new pos: currentPos = {0}; new pos = {1}; value = {2}",
                    //    thumb.Location.Y, thumbPos.Y, newValue);
                }
            }
        }

        private void SetupButtons()
        {
            int size = 20;

            Size availSize = this.FieldSize;
            this.firstButton.GetPreferredSize(availSize);
            this.secondButton.GetPreferredSize(availSize);

            if (this.ScrollType == ScrollType.Horizontal)
            {
                this.firstButton.SetBounds(0, 0, size, availSize.Height);
                this.secondButton.SetBounds(availSize.Width - size, 0, size, availSize.Height);
            }
            else
            {
                this.firstButton.SetBounds(0, 0, availSize.Width, size);
                this.secondButton.SetBounds(0, availSize.Height - size, availSize.Width, size);
            }
        }

        #endregion

        #region System skinning

        public override VisualStyleElement GetXPVisualStyle()
        {
            return SystemSkinManager.DefaultElement;
        }

        public override VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();
            this.thumb.SetValue(RadItem.MarginProperty, Padding.Empty);
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();
            this.thumb.ResetValue(RadItem.MarginProperty, ValueResetFlags.Local);
        }

        protected override bool ShouldPaintChild(RadElement element)
        {
            if (this.paintSystemSkin == true)
            {
                if (element is FillPrimitive || element is BorderPrimitive)
                {
                    return false;
                }
            }

            return base.ShouldPaintChild(element);
        }

        protected override void PaintElementSkin(Telerik.WinControls.Paint.IGraphics graphics)
        {
            Rectangle leftPartBounds = new Rectangle(new Point(this.firstButton.ControlBoundingRectangle.Right, 0),
                          new Size(this.thumb.ControlBoundingRectangle.Left - this.firstButton.ControlBoundingRectangle.Right,
                              this.Bounds.Height));

            Rectangle rightPartBounds = new Rectangle(new Point(this.thumb.ControlBoundingRectangle.Right, 0),
               new Size(this.secondButton.ControlBoundingRectangle.Left - this.thumb.ControlBoundingRectangle.Right,
                       this.Bounds.Height));

            Rectangle upPartBounds = new Rectangle(new Point(0, this.firstButton.ControlBoundingRectangle.Bottom),
                       new Size(this.Bounds.Width, this.thumb.ControlBoundingRectangle.Top - this.firstButton.ControlBoundingRectangle.Bottom));

            Rectangle downPartBounds = new Rectangle(new Point(0, this.thumb.ControlBoundingRectangle.Bottom),
                new Size(this.Bounds.Width, this.secondButton.ControlBoundingRectangle.Top - this.thumb.ControlBoundingRectangle.Bottom));

            Graphics g = graphics.UnderlayGraphics as Graphics;

            if (!this.Enabled)
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                   
                    VisualStyleElement leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Disabled;

                    this.PaintVisualStyleElement(g, leftTrackH, leftPartBounds);

                    VisualStyleElement rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled;

                    this.PaintVisualStyleElement(g, rightTrackH, rightPartBounds);

                }
                else
                {
                   

                    VisualStyleElement upTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Disabled;

                    this.PaintVisualStyleElement(g, upTrackH, upPartBounds);

                    VisualStyleElement downTrackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled;

                    this.PaintVisualStyleElement(g, downTrackH, downPartBounds);
                }
            }
            else
            {
                if (this.ScrollType == ScrollType.Horizontal)
                {
                    

                    if (!this.IsMouseDown && !this.IsMouseOver)
                    {
                        VisualStyleElement leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal;

                        this.PaintVisualStyleElement(g, leftTrackH, leftPartBounds);

                        VisualStyleElement rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal;

                        this.PaintVisualStyleElement(g, rightTrackH, rightPartBounds);

                    }
                    else if (this.IsMouseOver && !this.IsMouseDown)
                    {
                        VisualStyleElement leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Hot;
                        Point mousePosition = this.ElementTree.Control.PointToClient(Control.MousePosition);

                        if (!leftPartBounds.Contains(mousePosition))
                        {
                            leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal;
                        }

                        this.PaintVisualStyleElement(g, leftTrackH, downPartBounds);

                        VisualStyleElement rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Hot;

                        if (!rightPartBounds.Contains(mousePosition))
                        {
                            rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal;
                        }

                        this.PaintVisualStyleElement(g, rightTrackH, downPartBounds);
                    }
                    else if (this.IsMouseDown)
                    {
                        VisualStyleElement leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Pressed;
                        Point mousePosition = this.ElementTree.Control.PointToClient(Control.MousePosition);

                        if (!leftPartBounds.Contains(mousePosition))
                        {
                            leftTrackH = VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal;
                        }

                        this.PaintVisualStyleElement(g, leftTrackH, leftPartBounds);

                        VisualStyleElement rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Pressed;

                        if (!rightPartBounds.Contains(mousePosition))
                        {
                            rightTrackH = VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal;
                        }

                        this.PaintVisualStyleElement(g, rightTrackH, rightPartBounds);
                    }


                }
                else
                {
                    if (!this.IsMouseDown && !this.IsMouseOver)
                    {
                        VisualStyleElement upTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Normal;

                        this.PaintVisualStyleElement(g, upTrackH, upPartBounds);

                        VisualStyleElement downTrackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Normal;

                        this.PaintVisualStyleElement(g, downTrackH, downPartBounds);

                    }
                    else if (this.IsMouseOver && !this.IsMouseDown)
                    {
                        VisualStyleElement upperTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Hot;

                        Point mousePosition = this.ElementTree.Control.PointToClient(Control.MousePosition);

                        if (!upPartBounds.Contains(mousePosition))
                        {
                            upperTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Normal;
                        }

                        this.PaintVisualStyleElement(g, upperTrackH, upPartBounds);

                        VisualStyleElement lowerTackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Hot;

                        if (!downPartBounds.Contains(mousePosition))
                        {
                            lowerTackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Normal;
                        }

                        this.PaintVisualStyleElement(g, lowerTackH, downPartBounds);
                    }
                    else if (this.IsMouseDown)
                    {
                        VisualStyleElement upperTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Pressed;

                        Point mousePosition = this.ElementTree.Control.PointToClient(Control.MousePosition);

                        if (!upPartBounds.Contains(mousePosition))
                        {
                            upperTrackH = VisualStyleElement.ScrollBar.UpperTrackVertical.Normal;
                        }

                        this.PaintVisualStyleElement(g, upperTrackH, upPartBounds);

                        VisualStyleElement lowerTackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Pressed;

                        if (!downPartBounds.Contains(mousePosition))
                        {
                            lowerTackH = VisualStyleElement.ScrollBar.LowerTrackVertical.Normal;
                        }

                        this.PaintVisualStyleElement(g, lowerTackH, downPartBounds);
                    }
                }
            }
        }

        protected virtual void PaintVisualStyleElement(Graphics graphics, VisualStyleElement element, Rectangle bounds)
        {
            if (SystemSkinManager.Instance.SetCurrentElement(element))
            {
                SystemSkinManager.Instance.PaintCurrentElement(graphics, bounds);
            }
        }

        protected virtual void PaintVerticalVisualStyleElements()
        {
        }

        protected virtual void PaintHorizontalVisualStylesElements()
        {
        }

        #endregion

        [ComVisible(true)]
        public class RadScrollBarAccessibleObject : AccessibleObject
        {

            protected RadScrollBarElement owner;

            public RadScrollBarAccessibleObject(RadScrollBarElement owner)                
            {
                this.owner = owner;
            }

            public override AccessibleRole Role
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return AccessibleRole.ScrollBar;
                }
            }

            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.Value.ToString();
                }
                set
                {
                    base.Value = value;
                }
            }

            public override string Name
            {
                get
                {
                    return this.Value;
                }
                set
                {
                    base.Name = value;
                }
            }

            public override string Description
            {
                get
                {
                    return this.owner.Value.ToString();
                }
            }
        }
    }
}
