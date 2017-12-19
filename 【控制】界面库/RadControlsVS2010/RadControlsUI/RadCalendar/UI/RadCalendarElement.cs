using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	public class RadCalendarElement : CalendarVisualElement
	{
		// Fields
		protected CalendarStatusElement calendarStatusElement;
		private MonthViewElement calendarVisualElement;
		private CalendarNavigationElement calendarNavigationElement;
		protected BoxLayout boxLayout;
		private LightVisualElement selectedElement;
		protected List<LightVisualElement> visualElements;
		protected string dayCellFormat = string.Empty;
		private CalendarView view = null;
		private RadCalendar calendar = null;
		private DockLayoutPanel dockLayout;

        private bool allowDropDownFastNavigation = true;

        /// <summary>
        /// Gets or sets a value whether drop down fast navigation is enabled.
        /// </summary>
        [Description("Gets or sets a value whether drop down fast navigation is enabled.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool AllowDropDownFastNavigation
        {
            get
            {
                return this.allowDropDownFastNavigation;
            }
            set
            {
                this.allowDropDownFastNavigation = value;
            }
        }

		#region Constructors
		public RadCalendarElement(RadCalendar calendar)
			: this(calendar, null)
		{
            this.InitializeChildren();
		}

		public static RadProperty AllowFishEyeProperty =
	RadProperty.Register("AllowFishEye", typeof(bool), typeof(RadCalendarElement),
	 new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets whether the fish eye functionality is enabled</summary>
		[Description("Gets or sets whether the fish eye functionality is enabled")]
		[RadPropertyDefaultValue("AllowFishEye", typeof(bool)), Category(RadDesignCategory.BehaviorCategory)]
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
			RadProperty.Register("ZoomFactor", typeof(float), typeof(RadCalendarElement),
			 new RadElementPropertyMetadata(1.3f, ElementPropertyOptions.AffectsDisplay));

		/// <summary>Gets or sets the zooming factor of a cell which is handled by the fish eye functionality..</summary>
		[Description("Gets or sets the zooming factor of a cell which is handled by the fish eye functionality.")]
		[RadPropertyDefaultValue("ZoomFactor", typeof(float)), Category(RadDesignCategory.BehaviorCategory)]
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

		public RadCalendarElement(RadCalendar calendar, CalendarView view)
			: base(calendar, view)
		{
			this.view = view;
			this.calendar = calendar;
            this.InitializeChildren();
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.CanFocus = false;
            this.UseNewLayoutSystem = true;
            this.Class = "RadCalendarElement";
            this.Alignment = ContentAlignment.TopCenter;
        }

		static RadCalendarElement()
		{
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadCalendar().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadCalendarFastNavigationControl().DeserializeTheme();
		}

		#endregion

		public override CalendarView View
		{
			get
			{
				if (this.Owner == null)
					return this.Calendar.DefaultView;

				return this.view;
			}
			set
			{
				this.view = value;
			}
		}

        protected override void DisposeManagedResources()
        {
            this.calendar = null;
            this.view = null;

            if (this.calendarVisualElement != null)
            {
                this.calendarVisualElement.Dispose();
                this.calendarVisualElement = null;
            }

            if (this.calendarStatusElement != null)
            {
                this.calendarStatusElement.Dispose();
                this.calendarStatusElement = null;
            }

            if (this.calendarNavigationElement != null)
            {
                this.calendarNavigationElement.Dispose();
                this.calendarNavigationElement = null;
            }

            base.DisposeManagedResources();
        }

		internal void ReInitializeChildren()
		{
            this.DisposeChildren();
			InitializeChildren();
		}

		internal protected override void RefreshVisuals()
		{
			if (this.calendarNavigationElement != null)
				this.calendarNavigationElement.RefreshVisuals();

			if (this.calendarStatusElement != null)
				this.calendarStatusElement.RefreshVisuals();

			if (this.calendarVisualElement != null)
				this.calendarVisualElement.RefreshVisuals();
		}

		internal protected override void RefreshVisuals(bool unconditional)
		{
			if (this.calendarVisualElement != null)
				this.calendarVisualElement.RefreshVisuals(unconditional);

			if (this.calendarNavigationElement != null)
				this.calendarNavigationElement.RefreshVisuals(unconditional);

			if (this.calendarStatusElement != null)
				this.calendarStatusElement.RefreshVisuals(unconditional);
		}

        private void InitializeChildren()
        {
            this.dockLayout = new DockLayoutPanel();
            if (this.Calendar.MultiViewRows == 1 && this.Calendar.MultiViewColumns == 1)
            {
                this.calendarVisualElement = new MonthViewElement(this.calendar, this.calendar.DefaultView);
            }
            else
            {
                this.calendarVisualElement = new MultiMonthViewElement(this.calendar, this.calendar.DefaultView);
            }

            this.calendarStatusElement = new CalendarStatusElement(this);
            this.calendarStatusElement.SetValue(DockLayoutPanel.DockProperty, Dock.Bottom);
            this.calendarStatusElement.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.Last);
            if (true == this.Calendar.ShowFooter)
                this.calendarStatusElement.Visibility = ElementVisibility.Visible;

            this.dockLayout.Children.Add(this.calendarStatusElement);
            this.calendarNavigationElement = new CalendarNavigationElement(this);
            this.CalendarNavigationElement.SetValue(DockLayoutPanel.DockProperty, Dock.Top);
            this.calendarNavigationElement.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.First);
            this.calendarNavigationElement.Visibility = (this.Calendar.ShowHeader) ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            this.dockLayout.Children.Add(this.calendarNavigationElement);
            this.dockLayout.Children.Add(this.calendarVisualElement);
            this.Children.Add(this.dockLayout);
        }

		protected internal override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Down:
					this.FastNavigateToNextView();
					break;
				case Keys.Up:
					this.FastNavigateToPrevView();
					break;
				case Keys.Left:
					this.NavigateToPrevView();
					break;
				case Keys.Right:
					this.NavigateToNextView();
					break;
				case Keys.PageDown:
					this.FastNavigateToNextView();
					break;
				case Keys.PageUp:
					this.FastNavigateToPrevView();
					break;
				case Keys.Home:
					this.NavigateToToday();
					break;
				case Keys.F2:
				case Keys.Space:
					this.BeginEdit();
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		internal void BeginEdit()
		{
		}

		internal void NavigateToNextView()
		{
		}

		internal void NavigateToPrevView()
		{
		}

		internal void NavigateToToday()
		{
		}

		internal void FastNavigateToNextView()
		{
		}

		internal void FastNavigateToPrevView()
		{
		}

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnBubbleEvent(sender, args);
			if (args.RoutedEvent.EventName == "CellClickedEvent")
			{
				if (sender is CalendarCellElement)
				{
					this.SelectedElement = sender as LightVisualElement;
				}
			}
		}

		#region Properties

		protected LightVisualElement SelectedElement
		{
			get
			{
				return selectedElement;
			}
			set
			{
				selectedElement = value;
			}
		}

		public CalendarVisualElement CalendarVisualElement
		{
			get
			{
				return this.calendarVisualElement;
			}
		}

		public CalendarNavigationElement CalendarNavigationElement
		{
			get
			{
				return this.calendarNavigationElement;
			}
		}

		public CalendarStatusElement CalendarStatusElement
		{
			get
			{
				return this.calendarStatusElement;
			}
		}

		/// <summary>
		/// first button
		/// </summary>        
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadButtonElement FastBackwardButton
		{
			get
			{
				return this.CalendarNavigationElement.FastBackwardButton;
			}
		}

		/// <summary>
		/// Last button
		/// </summary>        
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadButtonElement FastForwardButton
		{
			get
			{
				return this.CalendarNavigationElement.FastForwardButton;
			}
		}

		/// <summary>
		/// previuos button
		/// </summary>        
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadButtonElement PreviousButton
		{
			get
			{
				return this.CalendarNavigationElement.PreviousButton;
			}
		}

		/// <summary>
		/// next button
		/// </summary>        
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadButtonElement NextButton
		{
			get
			{
				return this.CalendarNavigationElement.NextButton;
			}
		}

		#endregion
	}
}