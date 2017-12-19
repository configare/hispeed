using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;
using Telerik.WinControls;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
namespace Telerik.WinControls.UI
{
	/// <summary>Represents a trackbar element. RadTrackBarElement can be nested in other 
	/// telerik controls. Essentialy, the RadTrackBar is a simple wrapper for the 
	/// RadTrackBarElement. The former transfers events to and from its corresponding 
	/// RadTrackBarElement instance.</summary>
	public class RadTrackBarElement : RadTrackBarItem
	{
		#region Fields

		private TrackBarPrimitive background;
		private TrackBarThumb thumb;
		private BorderPrimitive border;

        private Size _indent = Size.Empty;
        //private int _indentWidth = 0;
        //private int _indentHeight = 0;

		private bool leftButtonDown;
	
		private int mouseStartPos = -1;

        private Size desiredSize = new Size(100, 100);

		private Timer timer;

		#endregion

		#region Constructors & Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.Size = new Size(100, 30);

            this.timer = new Timer();
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Interval = 300;

            this.leftButtonDown = false;
            this.DefaultSize = new Size(100, 30);
        }

        protected override void DisposeManagedResources()
        {
            if (this.timer != null)
            {
                this.timer.Tick -= new EventHandler(timer_Tick);
                this.timer.Dispose();
            }
            base.DisposeManagedResources();
        }

		static RadTrackBarElement()
		{ 
            new Themes.ControlDefault.TrackBar().DeserializeTheme();
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when the trackBar slider moves
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the trackBar slider moves")]
        public event ScrollEventHandler Scroll;

        /// <summary>
        /// Occurs when the value of the controls changes
        /// </summary>
        [Description("Occurs when the value of the control changes")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event EventHandler ValueChanged;
		
		public virtual void OnValueChanged(EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, e);
			}
		}

		public virtual void OnScroll(ScrollEventArgs e)
		{
			if (this.Scroll != null)
			{
				this.Scroll(this, e);
			}
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets an instance of the <see cref="TrackBarThumb"/>class
        /// which represents the part of the track bar that
        /// can be dragged to set the value of the control.
        /// </summary>
        [Browsable(false)]
        public TrackBarThumb Thumb
        {
            get
            {
                return this.thumb;
            }
        }

		/// <summary>
		/// Gets or Sets TrackBar's thumbWidth
		/// </summary>
        [Description("Gets or Sets TrackBar's thumbWidth")]
        [RadPropertyDefaultValue("ThumbWidth", typeof(TrackBarThumb))]
		public virtual int ThumbWidth
		{
			get
			{
				return this.thumb.ThumbWidth;
			}
			set
			{
				this.thumb.ThumbWidth = value;
			}
		}

        /// <summary>
        /// Gets or sets the preferred size of the element
        /// </summary>
        public Size PreferredSize
        {
            get { return this.desiredSize; }

            set { this.desiredSize = value; }
        }

		#endregion

		#region Methods

		private void timer_Tick(object sender, EventArgs e)
		{
			if (this.Bounds.Contains(this.PointFromScreen(Cursor.Position)))
			{
				this.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, this.PointFromScreen(Cursor.Position).X, this.PointFromScreen(Cursor.Position).Y, 1));
			}
			else
				this.timer.Stop();
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);
            if (!this.PreferredSize.Equals(Size.Empty))
            {
                if (this.TrackBarOrientation == Orientation.Horizontal)
                {
                    res.Width = this.PreferredSize.Width;
                }
                else
                {
                    res.Height = this.PreferredSize.Height;
                }
            }

            if (this.thumb != null)
            {
                this.UpdateThumbLocation(availableSize);
            }

            return res;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
			SizeF desiredSize = SizeF.Empty;

            if (this.thumb != null)
            {
                if (this.TrackBarOrientation == Orientation.Horizontal)
				{
					ArrangeHorizontalThumb(ref finalSize, ref desiredSize);
				}
                else
                {
					ArrangeVerticalThumb(ref finalSize, ref desiredSize);               
				}

                this.UpdateThumbLocation(finalSize);
            }

            return finalSize;
        }

		private void ArrangeHorizontalThumb(ref SizeF finalSize, ref SizeF desiredSize)
		{
			if (!(this.FitToAvailableSize))
				desiredSize = new SizeF(this.ThumbWidth, finalSize.Height / 2);
			else
				desiredSize = new SizeF(this.ThumbWidth, finalSize.Height - 2);

			float y = finalSize.Height / 4;

			if (this.TickStyle == TickStyles.BottomRight)
			{
				

				y = 0;
			}
			else
			{
				if (this.TickStyle == TickStyles.TopLeft)
				{				
					y = finalSize.Height / 2 - 2;
				}
			}

			if (this.FitToAvailableSize)
				y = 0;

			this.thumb.Arrange(new RectangleF(0, y, this.thumb.ThumbWidth, desiredSize.Height));

		}

		private void ArrangeVerticalThumb(ref SizeF finalSize, ref SizeF desiredSize)
		{
            if (!this.FitToAvailableSize)
            {
                desiredSize = new SizeF(finalSize.Width / 2, this.ThumbWidth);
            }
            else
            {
                desiredSize = new SizeF(finalSize.Width - 2, this.ThumbWidth);
            }

			float x = finalSize.Width / 4;

			if (this.TickStyle == TickStyles.BottomRight)
			{		
				x = 0;
			}
			else
			{
				if (this.TickStyle == TickStyles.TopLeft)
				{
					x = finalSize.Width / 2 - 2;
				}
			}

			if (this.FitToAvailableSize)
				x = 0;


			this.thumb.Arrange(new RectangleF(x, 0, desiredSize.Width, this.thumb.ThumbWidth));

		}

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="e"></param>
		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == TickFrequencyProperty)
			{
				if ((int)e.NewValue <= 0)
					this.TickFrequency = 1;
			}
			else if (e.Property == TickStyleProperty)
			{
				if (this.thumb.UseDefaultThumbShape)
				{
					if ((TickStyles)e.NewValue == TickStyles.TopLeft)
					{
						ValueSource currentValueSource = this.GetValueSource(TrackBarThumb.ShapeProperty);
						if (currentValueSource != ValueSource.Style)
						{
							if (this.TrackBarOrientation == Orientation.Horizontal)
							{
								this.thumb.Shape = new TrackBarUThumbShape();
							}
							else
							{
								this.thumb.Shape = new TrackBarLThumbShape();						
							}
						}
					}
					else
						if ((TickStyles)e.NewValue == TickStyles.BottomRight)
						{
							ValueSource currentValueSource = this.GetValueSource(TrackBarThumb.ShapeProperty);
							if (currentValueSource != ValueSource.Style)
							{
								if (this.TrackBarOrientation == Orientation.Horizontal)
								{
									this.thumb.Shape = new TrackBarDThumbShape();
								}
								else
								{
									this.thumb.Shape = new TrackBarRThumbShape();	
								}
							}
						}	
				}
			}
            else if ((e.Property == ValueProperty) || (e.Property == MaximumProperty) || (e.Property == MinimumProperty) || (e.Property == RightToLeftProperty) )
			{
                UpdateThumbLocation();
			}
			else if (e.Property == TrackBarOrientationProperty)
			{
				this.Value = 0;
                if ((Orientation)e.NewValue == Orientation.Horizontal)
                {
                    this.thumb.Location = new Point(0, this.thumb.Location.Y);
                    this.thumb.AngleTransform = 0;
                }
                else
                {
                     this.thumb.Location = new Point(this.thumb.Location.X, 0);
                     this.thumb.AngleTransform = 270;
                 }
                
            }

			base.OnPropertyChanged(e);
		}

        protected internal override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);
            this.UpdateThumbLocation();
        }

        private void UpdateThumbLocation()
        {
            this.UpdateThumbLocation(new SizeF( this.Bounds.Width, this.Bounds.Height));
        }
   
        private void UpdateThumbLocation(SizeF finalSize)
        {
            if (this.thumb == null)
                return;

            if (this.Value < this.Minimum)
            {
                this.Value = this.Minimum;
            }

            Rectangle workingRect = Rectangle.Inflate(new Rectangle(0, 0, (int)finalSize.Width, (int)finalSize.Height), -_indent.Width, -_indent.Height);
            float currentTrackerPos;

            if (this.TrackBarOrientation == Orientation.Horizontal)
            {
                if (this.Maximum == this.Minimum)
                {
                    currentTrackerPos = workingRect.Left;
                }
                else
                {
                    currentTrackerPos = (workingRect.Width - this.thumb.Bounds.Width) * (this.Value - this.Minimum) / (this.Maximum - this.Minimum) + workingRect.Left;
                    int tickNumber = this.Maximum - this.Minimum;
                    int i = (this.RightToLeft) ? 
                        this.Maximum - this.Value: 
                        this.Value - this.Minimum;
                    int valueToDiv = workingRect.Width - (int)this.thumb.DesiredSize.Width;
                    int x = (i * valueToDiv / (tickNumber));
                    currentTrackerPos = x;
                }

                this.thumb.Location = new Point((int)currentTrackerPos, this.thumb.Location.Y);
            }
            else
            {
                if (this.Maximum == this.Minimum)
                {
                    currentTrackerPos = workingRect.Top;
                }
                else
                {
                    currentTrackerPos = (workingRect.Height - this.thumb.Bounds.Height) * (this.Value - this.Minimum) / (this.Maximum - this.Minimum) + workingRect.Top;
                    int tickNumber = this.Maximum - this.Minimum;
                    int i = this.Value - this.Minimum;
                    int valueToDiv = workingRect.Height - this.thumb.Bounds.Height;
                    int x = (i * valueToDiv / (tickNumber));
                    currentTrackerPos = x;
                }

                this.thumb.Location = new Point(this.thumb.Location.X, (int)currentTrackerPos);

            }
        }

        /// <summary>
        /// OnBubbleEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnBubbleEvent(sender, args);

			if (args.RoutedEvent == RadElement.MouseDownEvent &&
				sender == this.thumb)
			{
				this.HandleMouseDownEvent((MouseEventArgs)args.OriginalEventArgs, true);
			}
		}
    
        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.HandleMouseDownEvent(e, false);
		}

        /// <summary>
        /// OnMouseMove
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int offsetValue = 0;
				int oldValue = 0;

				PointF currentPoint;

				currentPoint = new PointF(e.X, e.Y);

				if (this.leftButtonDown)
				{
					switch (this.TrackBarOrientation)
					{
						case Orientation.Horizontal:
                            offsetValue = CalculateOffset(e.X - this.mouseStartPos, this.thumb.Bounds.Width, this.Bounds.Width, _indent.Width);
                            break;

						case Orientation.Vertical:
                            offsetValue = CalculateOffset(e.Y - this.mouseStartPos, this.thumb.Bounds.Height, this.Bounds.Height, _indent.Height);
                            break;
					}

                    oldValue = this.Value;
                    this.Value = (this.RightToLeft) ? 
                        this.Maximum - offsetValue: 
                        this.Minimum + offsetValue;

                    Rectangle workingRect = Rectangle.Inflate(new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height), -_indent.Width, -_indent.Height);

                    int currentTrackerPos;

					if (this.TrackBarOrientation == Orientation.Horizontal)
					{
                        currentTrackerPos = CalculatePos(workingRect.Width - this.thumb.Bounds.Width, workingRect.Left);
						
						this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
					
						this.thumb.Location = new Point(currentTrackerPos, this.thumb.Location.Y);

						if (this.thumb.Bounds.Right == this.Bounds.Right)
						{
							this.thumb.Location = new Point(this.thumb.Location.X - 2, this.thumb.Location.Y);
						}
					}
					else
					{
                        currentTrackerPos = CalculatePos(workingRect.Height - this.thumb.Bounds.Height, workingRect.Top);

                        this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
					
						this.thumb.Location = new Point(this.thumb.Location.X, (int)currentTrackerPos);
						if (this.thumb.Bounds.Bottom == this.Bounds.Bottom)
						{
							this.thumb.Location = new Point(this.thumb.Location.X, this.thumb.Location.Y - 2);
						}
					}
				}
			}
			base.OnMouseMove(e);
		}

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.leftButtonDown = false;
			this.timer.Stop();
			this.Capture = false;
			this.thumb.PerformMouseUp(e);
			this.OnValueChanged(EventArgs.Empty);
			base.OnMouseUp(e);
		}

		protected override void CreateChildElements()
		{
			this.background = new TrackBarPrimitive();
			this.background.Class = "TrackBarFill";
			this.background.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
          
            

			this.background.BindProperty(TrackBarPrimitive.TickStyleProperty, this, RadTrackBarElement.TickStyleProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.MinimumProperty, this, MinimumProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.MaximumProperty, this, MaximumProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.TickFrequencyProperty, this, TickFrequencyProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.TickStyleProperty, this, TickStyleProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.TrackBarOrientationProperty, this, TrackBarOrientationProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.SlideAreaWidthProperty, this, SlideAreaWidthProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.ShowSlideAreaProperty, this, ShowSlideAreaProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.ShowTicksProperty, this, ShowTicksProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.FitToAvailableSizeProperty, this, FitToAvailableSizeProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.BackColor5Property, this, SliderAreaGradientColor1Property, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.BackColor6Property, this, SliderAreaGradientColor2Property, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.TickColorProperty, this, TickColorProperty, PropertyBindingOptions.OneWay);
			this.background.BindProperty(TrackBarPrimitive.SliderAreaGradientAngleProperty, this, SliderAreaGradientAngleProperty, PropertyBindingOptions.OneWay);

			this.Children.Add(this.background);

			this.border = new BorderPrimitive();
			this.border.Class = "TrackBarBorder";
			this.border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.Children.Add(this.border);

			this.thumb = new TrackBarThumb();
			this.thumb.Class = "TrackBarThumb";
			this.background.BindProperty(TrackBarPrimitive.ThumbWidthProperty, this.thumb, TrackBarThumb.ThumbWidthProperty, PropertyBindingOptions.OneWay);

			this.Children.Add(thumb);
		}

		#endregion

        #region Private methods

        private int CalculatePos(int range, int other)
        {
            int tickNumber = this.Maximum - this.Minimum;

            if (tickNumber == 0)
                return other;

            if( tickNumber > 0 )
            {
                return (this.RightToLeft) ? 
                    (this.Maximum - this.Value) * range / tickNumber: 
                    (this.Value - this.Minimum) * range / tickNumber;
            }
            else
            {
                return other;
            }
        }

        private int CalculateOffset(int currentDif, int thumb, int width, int indent)
        {
            if ((currentDif + thumb) >= width - indent)
            {
                return this.Maximum - this.Minimum;
            }

            if (currentDif <= indent) return 0;

            int numerator = (currentDif - indent) * (this.Maximum - this.Minimum);
            int denominator = width - 2 * indent - thumb;

            if (denominator == 0) return 0;

            int offset = numerator / denominator;

            if (numerator % denominator > 0)
                offset++;

            return offset;
        }

        private void HandleMouseDownEvent(MouseEventArgs e, bool fromThumb)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = this.PointFromScreen(Cursor.Position);

             
                PointF currentPoint;

                currentPoint = new Point(e.X, e.Y);
                this.Capture = true;

                if (fromThumb)
                {
                    if (!this.leftButtonDown)
                    {
                        this.leftButtonDown = true;

                        switch (this.TrackBarOrientation)
                        {
                            case Orientation.Horizontal:
                                this.mouseStartPos = e.X - this.thumb.Location.X;
                                break;
                            case Orientation.Vertical:
                                this.mouseStartPos = e.Y - this.thumb.Location.Y;
                                break;
                        }
                    }
                }
                else
                {
                    switch (this.TrackBarOrientation)
                    {
                        case Orientation.Horizontal:
                            {
                                this.timer.Start();

                                if (pt.X > this.thumb.Location.X ^ this.RightToLeft)
                                    this.Value += this.Minimum + this.LargeChange;
                                else
                                {
                                    this.Value -= this.LargeChange;
                                }
                                Rectangle workingRect = Rectangle.Inflate(new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height), -_indent.Width, -_indent.Height);
                                float currentTrackerPos;

                                if (this.Maximum == this.Minimum)
                                    currentTrackerPos = workingRect.Left;
                                else
                                {
                                    currentTrackerPos = (workingRect.Width - this.thumb.Bounds.Width) * (this.Value - this.Minimum) / (this.Maximum - this.Minimum) + workingRect.Left;
                                    int tickNumber = this.Maximum - this.Minimum;
                                    int i = (this.RightToLeft) ? 
                                        this.Maximum - this.Value: 
                                        this.Value - this.Minimum;

                                    int valueToDiv = workingRect.Width - this.thumb.Bounds.Width;
                                    int x = (i * valueToDiv / (tickNumber));// (stepWidth);
                                    currentTrackerPos = x;
                                }
                                this.thumb.Location = new Point((int)currentTrackerPos, this.thumb.Location.Y);
                                if (this.thumb.Bounds.Right == this.Bounds.Right)
                                {
                                    this.thumb.Location = new Point(this.thumb.Location.X - 2, this.thumb.Location.Y);
                                }
                                this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
                                this.OnValueChanged(EventArgs.Empty);
                                break;
                            }

                        case Orientation.Vertical:
                            {
                                this.timer.Start();

                                if (pt.Y > this.thumb.Location.Y)
                                    this.Value += this.Minimum + this.LargeChange;
                                else
                                {
                                    this.Value -= this.LargeChange;
                                }
                                Rectangle workingRect = Rectangle.Inflate(new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height), -_indent.Width, -_indent.Height);
                                float currentTrackerPos;

                                if (this.Maximum == this.Minimum)
                                    currentTrackerPos = workingRect.Top;
                                else
                                {
                                    currentTrackerPos = (workingRect.Height - this.thumb.Bounds.Height) * (this.Value - this.Minimum) / (this.Maximum - this.Minimum) + workingRect.Top;
                                    int tickNumber = this.Maximum - this.Minimum;
                                    int i = this.Value - this.Minimum;
                                    int valueToDiv = workingRect.Height - this.thumb.Bounds.Height;
                                    int x = (i * valueToDiv / (tickNumber));// (stepWidth);
                                    currentTrackerPos = x;
                                }

                                this.thumb.Location = new Point(this.thumb.Location.X, (int)currentTrackerPos);
                                if (this.thumb.Bounds.Bottom == this.Bounds.Bottom)
                                {
                                    this.thumb.Location = new Point(this.thumb.Location.X, this.thumb.Location.Y - 2);
                                }
                                this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
                                this.OnValueChanged(EventArgs.Empty);
                                break;
                            }
                    }

                }
            }
        }

        #endregion

    }
}
