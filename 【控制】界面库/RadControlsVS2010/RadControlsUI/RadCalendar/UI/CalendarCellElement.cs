using System;
using System.Drawing;
using System.ComponentModel; 
using Telerik.WinControls.Design; 

namespace Telerik.WinControls.UI
{
	public class CalendarCellElement : CalendarVisualElement
	{
		// Fields
		private Size defaultSize;
        internal bool isChecked;

		public CalendarCellElement(CalendarVisualElement owner)
			: this(owner, null, null, string.Empty)
		{
		}

		public CalendarCellElement(CalendarVisualElement owner, string text)
			: this(owner, null, null, text)
		{
		}

		public CalendarCellElement(RadCalendar calendar, CalendarView view)
			: this(null, calendar, view, "")
		{
		}

		public CalendarCellElement(RadCalendar calendar, CalendarView view, string text)
			: this(null, calendar, view, text)
		{
		}

		public CalendarCellElement(CalendarVisualElement owner, RadCalendar calendar, CalendarView view, string text)
			: base(owner, calendar, view)
		{
			this.Text = text;
			// DA SE OPRAVI KLASA NA ELEMENTA!
			if (owner == null)
			{
				base.Calendar = calendar;
				base.View = view;
			}
		}

        static CalendarCellElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new CalendarCellStateManagerFactory(), typeof(CalendarCellElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.CanFocus = false;
            this.UseNewLayoutSystem = true;
            this.DefaultSize = new Size(20, 20);

            // DA SE OPRAVI KLASA NA ELEMENTA!
            this.Class = "CalendarVisualCellElement";
            this.DrawBorder = true;
            this.DrawFill = true;

            this.oldFont = this.Font;
            this.FontChanged += new EventHandler(CalendarCellElement_FontChanged);
        }

        private void CalendarCellElement_FontChanged(object sender, EventArgs e)
		{
			if (forwardsFontAnimate != null && this.isAnimating)
			{
				this.Font = (Font)forwardsFontAnimate.GetCurrentValue(this);
				//	Console.WriteLine(this.Font);
				this.Calendar.Invalidate();
			}
		}



		protected override SizeF MeasureOverride(SizeF availableSize)
		{
			base.MeasureOverride(availableSize);
			return new Size(0, 0);
		}



		protected override SizeF ArrangeOverride(SizeF finalSize)
		{
			if (this.Selected && (this.AllowFishEye || this.Calendar.AllowFishEye))
			{
				this.Calendar.Invalidate();
			}

			return base.ArrangeOverride(finalSize);
		}


		[Description("Gets or sets the default cell size.")]
		[DefaultValue(typeof(Size), "20,20")]
		public override Size DefaultSize
		{
			get { return this.defaultSize; }
			set { this.defaultSize = value; }
		}

		private int row;
		private int column;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Row
		{
			get
			{
				return this.row;
			}
			internal set
			{
				this.row = value;
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Column
		{
			get
			{
				return this.column;
			}
			internal set
			{
				this.column = value;
			}
		}

	

		internal CalendarTableElement OwnerTableElement
		{
			get
			{
				return this.Owner as CalendarTableElement;
			}
		}

		private Rectangle proposedBounds;
		internal Rectangle ProposedBounds
		{
			get
			{
				return this.proposedBounds;
			}
			set
			{
				this.proposedBounds = value;
			}
		}


		private AnimatedPropertySetting forwardsAnimate = null;
		private AnimatedPropertySetting backwardsAnimate = null;

		private AnimatedPropertySetting forwardsFontAnimate = null;
		private AnimatedPropertySetting backwardsFontAnimate = null;

		private void OnAnimationFinished(object sender, AnimationStatusEventArgs e)
		{
			this.isAnimating = false;

			this.AutoSize = true;
			if (this.Calendar != null)
			{
				this.Calendar.Invalidate();
			}
		}

		private void OnForwardAnimationFinished(object sender, AnimationStatusEventArgs e)
		{
			this.isAnimating = false;

			if (this.Calendar != null)
			{
				if (this.Bounds.Size == this.ProposedBounds.Size)
				{
					this.Bounds = new Rectangle(Point.Empty, this.Bounds.Size);
					this.Font = this.oldFont;
					this.Invalidate();
				}


				this.Calendar.Invalidate();
			}
		}

		private Font oldFont;

		private void InitializeForwardAnimationSetting()
		{
			SizeF cellSize = this.proposedBounds.Size;

			cellSize.Width *= this.ZoomFactor;
			cellSize.Height *= this.ZoomFactor;

			int xOffset = (int)(cellSize.Width - proposedBounds.Width);
			int yOffset = (int)(cellSize.Height - proposedBounds.Height);

			int x = -xOffset;//(int)(proposedBounds.X - xOffset);
			int y = -yOffset;//(int)(proposedBounds.Y - yOffset);
			int width = (int)(cellSize.Width + xOffset);
			int height = (int)(cellSize.Height + yOffset);

			this.forwardsAnimate = new AnimatedPropertySetting(RadElement.BoundsProperty,
					new Rectangle(0, 0, proposedBounds.Width,
					proposedBounds.Height)
					, new Rectangle(
					x,
					y,
					width,
					height),
					8,
					20);



			this.forwardsFontAnimate = new AnimatedPropertySetting(VisualElement.FontProperty,
				this.oldFont, new Font(this.Font.FontFamily, this.oldFont.Size * 1.5f * this.ZoomFactor),
				8,
				20);

            this.forwardsFontAnimate.RemoveAfterApply = true;

		}

		private void InitializeBackwardAnimationSetting()
		{
			SizeF cellSize = this.proposedBounds.Size;

			cellSize.Width *= this.ZoomFactor;
			cellSize.Height *= this.ZoomFactor;

			int xOffset = (int)(cellSize.Width - proposedBounds.Width);
			int yOffset = (int)(cellSize.Height - proposedBounds.Height);

			int x = -xOffset;//(int)(proposedBounds.X - xOffset);
			int y = -yOffset;//(int)(proposedBounds.Y - yOffset);
			int width = (int)(cellSize.Width + xOffset);
			int height = (int)(cellSize.Height + yOffset);

			this.backwardsAnimate = new AnimatedPropertySetting(RadElement.BoundsProperty,
					new Rectangle(
					x,
					y,
					width,
					height),
					proposedBounds,
					10,
					20);

			this.backwardsAnimate.AnimationFinished += new AnimationFinishedEventHandler(OnAnimationFinished);

            this.backwardsFontAnimate = new AnimatedPropertySetting(VisualElement.FontProperty,
            this.oldFont, new Font(this.Font.FontFamily, this.oldFont.Size),
            8,
            20);
        }

		public static RadProperty IsZoomingProperty =
		RadProperty.Register("IsZooming", typeof(bool), typeof(CalendarCellElement),
		 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));


		public static RadProperty AllowFishEyeProperty =
			RadProperty.Register("AllowFishEye", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Indicates the fish eye feature factor of a cell.</summary>
		[Description("Indicates the fish eye feature factor of a cell")]
		[RadPropertyDefaultValue("AllowFishEye", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool AllowFishEye
		{
			get
			{
				return (bool)this.GetValue(AllowFishEyeProperty);
			}
			set
			{
				this.SetValue(AllowFishEyeProperty, value);
			}
		}

		public static RadProperty ZoomFactorProperty =
			RadProperty.Register("ZoomFactor", typeof(float), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(1.3f, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the zooming factor of a cell.</summary>
		[Description("Indicates the zooming factor of a cell")]
		[RadPropertyDefaultValue("ZoomFactor", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual float ZoomFactor
		{
			get
			{
				return (float)this.GetValue(ZoomFactorProperty);
			}
			set
			{
				this.SetValue(ZoomFactorProperty, value);
			}
		}



		internal bool isAnimating;

		internal void PerformForwardAnimation()
		{
			this.isAnimating = true;

			if (this.forwardsAnimate == null)
			{
				this.InitializeForwardAnimationSetting();
				this.forwardsAnimate.AnimationFinished += new AnimationFinishedEventHandler(OnForwardAnimationFinished);
			}

			if (this.AllowFishEye || this.Calendar.AllowFishEye)
			{
				this.AutoSize = false;
				this.forwardsAnimate.ApplyValue(this);
				this.forwardsFontAnimate.ApplyValue(this);
			}
		}


		internal void PerformReverseAnimation()
		{

			if (this.backwardsAnimate == null)
			{
				InitializeBackwardAnimationSetting();
			}

			//if (!this.AutoSize)
			//{
			//    this.isAnimating = true;			
			//    this.backwardsAnimate.StartValue = this.forwardsAnimate.GetCurrentValue(this);
			//    this.backwardsAnimate.ApplyValue(this);	
			//}
			//else
			{


				this.isAnimating = false;

				this.AutoSize = true;


                if (this.forwardsAnimate != null)
                    this.forwardsAnimate.Stop(this);

                if (this.forwardsFontAnimate != null)
                {
                    this.forwardsFontAnimate.Stop(this);
                    this.forwardsFontAnimate.UnapplyValue(this);
                }

				this.Bounds = new Rectangle(Point.Empty, this.ProposedBounds.Size);
                this.Font = this.oldFont;
                

				

                
				this.InitializeForwardAnimationSetting();


				if (this.Calendar != null)
					this.Calendar.Invalidate();

			}
		}

		public static RoutedEvent CellClickedEvent = RadElement.RegisterRoutedEvent("CellClickedEvent", typeof(CalendarCellElement));

		public static RadProperty WeekEndProperty =
			RadProperty.Register("WeekEnd", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the week end cell.</summary>
		[Description("Indicates that current cell is a week end.")]
		[RadPropertyDefaultValue("WeekEnd", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool WeekEnd
		{
			get
			{
				return (bool)this.GetValue(WeekEndProperty);
			}
			set
			{
				this.SetValue(WeekEndProperty, value);
			}
		}

		private DateTime? date;

		/// <summary>Gets or sets the date which that cell is representing.</summary>
		[Description("Gets or sets the date which that cell is representing")]
		[DefaultValue(typeof(DateTime), "1/1/1980"),
		Category(RadDesignCategory.BehaviorCategory)]
		public virtual DateTime Date
		{
			get
			{
				if (this.date.HasValue)
				{
					return this.date.Value;
				}
				return new DateTime(1980, 1, 1);
			}
			set
			{
				if ((this.date.HasValue && this.date.Value != value) ||
					!this.date.HasValue)
				{
					this.date = value;
				}
			}
		}

		public static RadProperty SpecialDayProperty =
		RadProperty.Register("SpecialDay", typeof(bool), typeof(CalendarCellElement),
		 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets a cell representing a special day.</summary>
		[Description("Indicates that current cell is representing a special day")]
		[RadPropertyDefaultValue("TemplateDay", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool SpecialDay
		{
			get
			{
				return (bool)this.GetValue(SpecialDayProperty);
			}
			set
			{
				this.SetValue(SpecialDayProperty, value);
			}
		}


		public static RadProperty FocusedProperty =
			RadProperty.Register("Focused", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the today cell.</summary>
		[Description("Indicates that current cell is representing the focused day")]
		[RadPropertyDefaultValue("Focused", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool Focused
		{
			get
			{
				return (bool)this.GetValue(FocusedProperty);
			}
			set
			{
				this.SetValue(FocusedProperty, value);
			}
		}

		public static RadProperty TodayProperty =
			RadProperty.Register("Today", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the today cell.</summary>
		[Description("Indicates that current cell is representing the current day")]
		[RadPropertyDefaultValue("Today", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool Today
		{
			get
			{
				return (bool)this.GetValue(TodayProperty);
			}
			set
			{
				this.SetValue(TodayProperty, value);
          	}
		}

		public static RadProperty IsHeaderCellProperty =
			RadProperty.Register("IsHeaderCell", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));


		public static RadProperty OutOfRangeProperty =
			RadProperty.Register("OutOfRange", typeof(bool), typeof(CalendarCellElement),
			 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the out of range cell.</summary>
		[Description("Indicates that current element is representing a day which is out of range.")]
		[RadPropertyDefaultValue("OutOfRange", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool OutOfRange
		{
			get
			{
				return (bool)this.GetValue(OutOfRangeProperty);
			}
			set
			{
				this.SetValue(OutOfRangeProperty, value);
			}
		}

		public static RadProperty OtherMonthProperty =
		RadProperty.Register("OtherMonth", typeof(bool), typeof(CalendarCellElement),
		 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));


		/// <summary>Gets or sets the cell which is from other month.</summary>
		[Description("Indicates that current element is representing a cell from another month.")]
		[RadPropertyDefaultValue("OtherMonth", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool OtherMonth
		{
			get
			{
				return (bool)this.GetValue(OtherMonthProperty);
			}
			set
			{
				this.SetValue(OtherMonthProperty, value);
			}
		}

		public static RadProperty SelectedProperty =
		RadProperty.Register("SelectedProperty", typeof(bool), typeof(CalendarCellElement),
		new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>Gets or sets the selected cell.</summary>
		[Description("Indicates that current element selected.")]
		[RadPropertyDefaultValue("SelectedProperty", typeof(CalendarCellElement)), Category(RadDesignCategory.BehaviorCategory)]
		public virtual bool Selected
		{
			get
			{
				return (bool)this.GetValue(SelectedProperty);
			}
			set
			{
				this.SetValue(SelectedProperty, value);
			}
		}

        public override string ToString()
        {
            return this.Date.ToShortDateString();
        }
	}
}
