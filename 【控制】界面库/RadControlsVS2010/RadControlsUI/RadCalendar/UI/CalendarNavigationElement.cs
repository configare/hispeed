using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Styles;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    public class CalendarNavigationElement : CalendarVisualElement
    {
        #region Fields
        private RadButtonElement fastBackwardButton;
        private RadButtonElement fastForwardButton;
        private RadRepeatButtonElement prevButton;
        private RadRepeatButtonElement nextButton;
        private BoxLayout boxLayout;
        private Timer scrollingTimer;
        private RadDateTimePickerDropDown dropDown;
        private RadCalendarFastNavigationControl hostedControl;
        private bool scrollUp = false;
        private bool shouldScroll = false;

        private FastNavigationItem item1;
        private FastNavigationItem item2;
        private FastNavigationItem item3;
        private FastNavigationItem item4;
        private FastNavigationItem item5;
        private FastNavigationItem item6;
        private FastNavigationItem item7;
        private FastNavigationItem item8;
        private FastNavigationItem item9;
        private FastNavigationItem item10;
        private FastNavigationItem item11;
        private FastNavigationItem item12;
        private FastNavigationItem item13;

        private int fastNavigationItemsCount = 7;

        private int viewIndex;

        private Point offsetPoint;

        #endregion

        #region Constructors
        internal CalendarNavigationElement(RadCalendarElement owner)
        : base(owner)
        {
            this.Calendar.PropertyChanged += new PropertyChangedEventHandler(Calendar_PropertyChanged);
            this.dropDown.OwnerControl = this.Calendar;
            this.SetText();
        }

        #endregion

        #region Properties

        [DefaultValue(false)]
        public override bool StretchVertically
        {
            get
            {
                return base.StretchVertically;
            }
            set
            {
                base.StretchVertically = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of the items in the fast navigation drop down
        /// </summary>
        [Description(" Gets or sets the count of the items in the fast navigation drop down")]
        public int FastNavigationItemsCount
        {
            get
            {
                return this.fastNavigationItemsCount;
            }
            set
            {
                if (this.fastNavigationItemsCount != value)
                {
                    if (value == 3 || value == 7 || value == 11 || value == 13)
                    {
                        this.hostedControl.Items.Clear();
                        this.fastNavigationItemsCount = value;
                    }
                }
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
                return this.fastBackwardButton;
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
                return this.fastForwardButton;
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
                return this.prevButton;
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
                return this.nextButton;
            }
        }

        [Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image RightButtonImage
        {
            get
            {
                return (Image)this.GetValue(RightButtonImageProperty);
            }
            set
            {
                this.SetValue(RightButtonImageProperty, value);
                this.nextButton.Image = value;
            }
        }

        [Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image LeftButtonImage
        {
            get
            {
                return (Image)this.GetValue(LeftButtonImageProperty);
            }
            set
            {
                this.SetValue(LeftButtonImageProperty, value);
                this.prevButton.Image = value;
            }
        }

        [Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image FastRightButtonImage
        {
            get
            {
                return (Image)this.GetValue(FastRightButtonImageProperty);
            }
            set
            {
                this.SetValue(FastRightButtonImageProperty, value);
                this.fastForwardButton.Image = value;
            }
        }

        [Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image FastLeftButtonImage
        {
            get
            {
                return (Image)this.GetValue(FastLeftButtonImageProperty);
            }
            set
            {
                this.SetValue(FastLeftButtonImageProperty, value);
                this.fastBackwardButton.Image = value;
            }
        }

        #endregion

        #region RadProperties
        public static RadProperty LeftButtonImageProperty = RadProperty.Register("LeftButtonImage", typeof(Image), typeof(CalendarNavigationElement),
              new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty RightButtonImageProperty = RadProperty.Register("RightButtonImage", typeof(Image), typeof(CalendarNavigationElement),
              new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty FastRightButtonImageProperty = RadProperty.Register("FastRightButtonImage", typeof(Image), typeof(CalendarNavigationElement),
              new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty FastLeftButtonImageProperty = RadProperty.Register("FastLeftButtonImage", typeof(Image), typeof(CalendarNavigationElement),
              new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.StretchVertically = false;
            this.DrawFill = true;
            this.Class = "CalendarNavigationElement";
            this.TextAlignment = ContentAlignment.MiddleCenter;

            this.scrollingTimer = new Timer();
            this.scrollingTimer.Tick += new EventHandler(scrollingTimer_Tick);
            this.scrollingTimer.Interval = 1000;

            this.dropDown = new RadDateTimePickerDropDown(this);
            this.hostedControl = new RadCalendarFastNavigationControl();
            this.dropDown.HostedControl = this.hostedControl;
            this.dropDown.SizingGrip.Visibility = ElementVisibility.Collapsed;
            this.hostedControl.ThemeName = "ControlDefault";
            this.dropDown.BackColor = Color.White;
            this.dropDown.AnimationEnabled = false;
        }

        protected override void DisposeManagedResources()
        {
            this.Calendar.PropertyChanged -= new PropertyChangedEventHandler(Calendar_PropertyChanged);

            this.View = null;
            this.Calendar = null;

            if (this.prevButton != null)
            {
                this.prevButton.Click -= new EventHandler(prevButton_Click);
                this.prevButton.Dispose();
                this.prevButton = null;
            }

            if (this.nextButton != null)
            {
                this.nextButton.Click -= new EventHandler(nextButton_Click);
                this.nextButton.Dispose();
                this.nextButton = null;
            }

            if (this.fastBackwardButton != null)
            {
                this.fastBackwardButton.MouseDown -= new MouseEventHandler(fastBackwardButton_MouseDown);
                this.fastBackwardButton.Dispose();
                this.fastBackwardButton = null;
            }

            if (this.fastForwardButton != null)
            {
                this.fastForwardButton.MouseDown -= new MouseEventHandler(fastForwardButton_MouseDown);
                this.fastForwardButton.Dispose();
                this.fastForwardButton = null;
            }

            this.DisposeNavigationElements();

            base.DisposeManagedResources();
        }

        private void DisposeNavigationElements()
        {
            if (this.scrollingTimer != null)
            {
                this.scrollingTimer.Stop();
                this.scrollingTimer.Dispose();
            }
            if (this.dropDown != null)
            {
                this.dropDown.Dispose();
                this.dropDown = null;
            }
            if (this.hostedControl != null)
            {
                this.hostedControl.Dispose();
                this.hostedControl = null;
            }
        }

        private void Calendar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowHeader":
                    this.Visibility = this.Calendar.ShowHeader ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                    break;
                case "TitleFormat":
                    //this.Text = this.View.ViewStartDate.ToString(this.Owner.Calendar.TitleFormat, this.Owner.Calendar.Culture);
                    this.Text = this.View.GetTitleContent();
                    UpdateVisuals();
                    break;
                case "ShowFastNavigationButtons":

                    if (!this.Calendar.ShowFastNavigationButtons)
                    {
                        this.fastBackwardButton.Visibility = ElementVisibility.Hidden;
                        this.fastForwardButton.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        this.fastBackwardButton.Visibility = ElementVisibility.Visible;
                        this.fastForwardButton.Visibility = ElementVisibility.Visible;
                    }

                    break;
                case "ShowNavigationButtons":
                    if (!this.Calendar.ShowNavigationButtons)
                    {
                        this.nextButton.Visibility = ElementVisibility.Hidden;
                        this.prevButton.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        this.nextButton.Visibility = ElementVisibility.Visible;
                        this.prevButton.Visibility = ElementVisibility.Visible;
                    }

                    break;

                case "FastNavigationNextText":
                    this.fastForwardButton.Text = this.Calendar.FastNavigationNextText;
                    UpdateVisuals();
                    break;
                case "FastNavigationNextToolTip":
                    this.fastForwardButton.ToolTipText = this.Calendar.FastNavigationNextToolTip;
                    UpdateVisuals();
                    break;
                case "FastNavigationPrevText":
                    this.fastBackwardButton.Text = this.Calendar.FastNavigationPrevText;
                    UpdateVisuals();
                    break;
                case "FastNavigationPrevToolTip":
                    this.fastBackwardButton.ToolTipText = this.Calendar.FastNavigationPrevToolTip;
                    UpdateVisuals();
                    break;
                case "NavigationNextText":
                    this.nextButton.Text = this.Calendar.NavigationNextText;
                    UpdateVisuals();
                    break;
                case "NavigationNextToolTip":
                    this.nextButton.ToolTipText = this.Calendar.NavigationNextToolTip;
                    UpdateVisuals();
                    break;
                case "NavigationPrevText":
                    this.prevButton.Text = this.Calendar.NavigationPrevText;
                    UpdateVisuals();
                    break;
                case "NavigationPrevToolTip":
                    this.prevButton.ToolTipText = this.Calendar.NavigationPrevToolTip;
                    UpdateVisuals();
                    break;
                case "ThemeName":
                    if (this.hostedControl != null)
                    {
                        this.hostedControl.ThemeName = this.Calendar.ThemeName;
                    }
                    break;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.boxLayout = new BoxLayout();
            this.boxLayout.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.boxLayout.StretchHorizontally = true;
            this.boxLayout.StretchVertically = false;
            this.boxLayout.RightToLeft = false;

            this.fastForwardButton = new RadButtonElement();
            this.fastForwardButton.DisplayStyle = DisplayStyle.Image;
            this.fastForwardButton.ThemeRole = "FastNavigateForwardButton";
            this.fastForwardButton.StretchHorizontally = false;
            this.fastForwardButton.StretchVertically = false;
            this.fastForwardButton.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.Last);
            this.fastForwardButton.MouseDown += new MouseEventHandler(fastForwardButton_MouseDown);
            this.fastForwardButton.Class = "fastForwardButton";
            this.fastForwardButton.Children[1].Children[0].Class = "fastForwardButton";
            this.fastForwardButton.MinSize = new Size(16, 16);
            this.fastForwardButton.ShowBorder = false;
            this.fastForwardButton.Children[1].Alignment = ContentAlignment.MiddleCenter;
            this.fastForwardButton.Children[0].Visibility = ElementVisibility.Hidden;
            this.boxLayout.Children.Add(this.fastForwardButton);

            this.nextButton = new RadRepeatButtonElement();
            this.nextButton.DisplayStyle = DisplayStyle.Image;
            this.nextButton.ThemeRole = "NavigateForwardButton";
            this.nextButton.StretchHorizontally = false;
            this.nextButton.StretchVertically = false;
            this.nextButton.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.Last);
            this.nextButton.Click += new EventHandler(nextButton_Click);
            this.nextButton.Class = "nextButton";
            this.nextButton.Children[1].Children[0].Class = "nextButton";
            this.nextButton.MinSize = new Size(16, 16);
            this.nextButton.Children[1].Alignment = ContentAlignment.MiddleCenter;
            this.nextButton.ShowBorder = false;
            this.nextButton.Children[0].Visibility = ElementVisibility.Hidden;
            this.boxLayout.Children.Add(this.nextButton);

            this.fastBackwardButton = new RadButtonElement();
            this.fastBackwardButton.DisplayStyle = DisplayStyle.Image;
            this.fastBackwardButton.ThemeRole = "FastNavigateBackwardButton";
            this.fastBackwardButton.StretchHorizontally = false;
            this.fastBackwardButton.StretchVertically = false;
            this.fastBackwardButton.MouseDown += new MouseEventHandler(fastBackwardButton_MouseDown);
            this.fastBackwardButton.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.First);
            this.fastBackwardButton.Class = "fastBackwardButton";
            this.fastBackwardButton.Children[1].Children[0].Class = "fastBackwardButton";
            this.fastBackwardButton.MinSize = new Size(16, 16);
            this.fastBackwardButton.ShowBorder = false;
            this.fastBackwardButton.Children[0].Visibility = ElementVisibility.Hidden;
            this.fastBackwardButton.Children[1].Alignment = ContentAlignment.MiddleCenter;
            this.boxLayout.Children.Add(this.fastBackwardButton);

            this.prevButton = new RadRepeatButtonElement();
            this.prevButton.DisplayStyle = DisplayStyle.Image;
            this.prevButton.ThemeRole = "NavigateBackwardButton";
            this.prevButton.StretchHorizontally = false;
            this.prevButton.StretchVertically = false;
            this.prevButton.Click += new EventHandler(prevButton_Click);
            this.prevButton.Class = "prevButton";
            this.prevButton.Children[1].Children[0].Class = "prevButton";
            this.prevButton.SetValue(BoxLayout.StripPositionProperty, BoxLayout.StripPosition.First);
            this.prevButton.MinSize = new Size(16, 16);
            this.prevButton.Children[1].Alignment = ContentAlignment.MiddleCenter;
            this.prevButton.ShowBorder = false;
            this.prevButton.Children[0].Visibility = ElementVisibility.Hidden;
            this.boxLayout.Children.Add(this.prevButton);

            this.Children.Add(this.boxLayout);
        }

        private void SetText()
        {
            if (this.Calendar != null)
            {
                this.Text = this.View.GetTitleContent();
            }
            else
            {
                this.Text = DateTime.Now.Year.ToString();
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (this.Calendar.AllowNavigation && !this.Calendar.ReadOnly)
            {
                CalendarView view = this.Calendar.GetNewViewFromStep(-1);
                UpdateView(view);
                UpdateVisuals();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (this.Calendar.AllowNavigation && !this.Calendar.ReadOnly)
            {
                CalendarView view = this.Calendar.GetNewViewFromStep(1);
                UpdateView(view);
            }
        }

        private void fastForwardButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Calendar.AllowFastNavigation && !this.Calendar.ReadOnly)
            {
                {
                    CalendarView view = this.Calendar.GetNewViewFromStep(this.Calendar.FastNavigationStep);
                    UpdateView(view);
                }
            }
        }

        private void fastBackwardButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Calendar.AllowFastNavigation && !this.Calendar.ReadOnly)
            {
                {
                    CalendarView view = this.Calendar.GetNewViewFromStep(-this.Calendar.FastNavigationStep);
                    UpdateView(view);
                }
            }
        }

        internal protected virtual void UpdateVisuals()
        {
            this.Invalidate();
        }

        protected internal override void RefreshVisuals()
        {
            base.RefreshVisuals();
            UpdateView(this.View);
        }

        protected internal override void RefreshVisuals(bool unconditional)
        {
            base.RefreshVisuals(unconditional);
            UpdateView(this.View);
        }

        internal protected virtual void UpdateView(CalendarView view)
        {
            if (this.Calendar != null)
            {
                bool updateView = this.Calendar.DefaultView != view;
                if (updateView)
                {
                    ViewChangingEventArgs args = this.Calendar.CallOnViewChanging(view);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                this.Calendar.SetCalendarView(view);
                this.Calendar.CalendarElement.CalendarVisualElement.View = view;
                if (updateView)
                {
                    this.Calendar.CallOnViewChanged();
                }
            }

            this.Text = this.View.GetTitleContent();
            UpdateVisuals();
        }

        private void InitializeNavItems(int offset)
        {
            if (item1 == null)
            {
                item1 = new FastNavigationItem(this.Calendar, this.View);
                item2 = new FastNavigationItem(this.Calendar, this.View);
                item3 = new FastNavigationItem(this.Calendar, this.View);
                item4 = new FastNavigationItem(this.Calendar, this.View);
                item5 = new FastNavigationItem(this.Calendar, this.View);
                item6 = new FastNavigationItem(this.Calendar, this.View);
                item7 = new FastNavigationItem(this.Calendar, this.View);
                item8 = new FastNavigationItem(this.Calendar, this.View);
                item9 = new FastNavigationItem(this.Calendar, this.View);
                item10 = new FastNavigationItem(this.Calendar, this.View);
                item11 = new FastNavigationItem(this.Calendar, this.View);
                item12 = new FastNavigationItem(this.Calendar, this.View);
                item13 = new FastNavigationItem(this.Calendar, this.View);

                // To be reviewed later Boyko Markov
                item1.PerformLayout();
                item2.PerformLayout();
                item3.PerformLayout();
                item4.PerformLayout();
                item5.PerformLayout();
                item6.PerformLayout();
                item7.PerformLayout();
                item8.PerformLayout();
                item9.PerformLayout();
                item10.PerformLayout();
                item11.PerformLayout();
                item12.PerformLayout();
                item13.PerformLayout();
            }

            int defaultStep = 3;
            switch (this.FastNavigationItemsCount)
            {
                case 3:
                    defaultStep = 1;
                    break;
                case 7:
                    defaultStep = 3;
                    break;
                case 9:
                    defaultStep = 4;
                    break;
                case 11:
                    defaultStep = 5;
                    break;
                case 13:
                    defaultStep = 6;
                    break;
            }

            int yearDif = this.Calendar.DefaultView.ViewStartDate.Year - this.Calendar.RangeMinDate.Year;
            int monthDif = this.Calendar.DefaultView.ViewStartDate.Month - this.Calendar.RangeMinDate.Month;

            int realMonthDif = (yearDif * 12) + monthDif;

            if (realMonthDif < FastNavigationItemsCount)
            {
                defaultStep = realMonthDif;
            }

            if (this.FastNavigationItemsCount > 1)
            {
                CalendarView item1View = this.Calendar.GetNewViewFromStep(-defaultStep + offset);
                CalendarView item2View = this.Calendar.GetNewViewFromStep(-defaultStep + 1 + offset);
                CalendarView item3View = this.Calendar.GetNewViewFromStep(-defaultStep + 2 + offset);

                item1.View = item1View;
                item2.View = item2View;
                item3.View = item3View;

                item1.Text = item1.View.GetTitleContent();
                item2.Text = item2.View.GetTitleContent();
                item3.Text = item3.View.GetTitleContent();
            }

            if (this.FastNavigationItemsCount > 3)
            {
                CalendarView item4View = this.Calendar.GetNewViewFromStep(-defaultStep + 3 + offset);
                CalendarView item5View = this.Calendar.GetNewViewFromStep(-defaultStep + 4 + offset);
                CalendarView item6View = this.Calendar.GetNewViewFromStep(-defaultStep + 5 + offset);
                CalendarView item7View = this.Calendar.GetNewViewFromStep(-defaultStep + 6 + offset);

                item4.View = item4View;
                item5.View = item5View;
                item6.View = item6View;
                item7.View = item7View;

                item4.Text = item4.View.GetTitleContent();
                item5.Text = item5.View.GetTitleContent();
                item6.Text = item6.View.GetTitleContent();
                item7.Text = item7.View.GetTitleContent();
            }

            if (this.FastNavigationItemsCount > 7)
            {
                item8.View = this.Calendar.GetNewViewFromStep(-defaultStep + 7 + offset);
                item9.View = this.Calendar.GetNewViewFromStep(-defaultStep + 8 + offset);
                item8.Text = item8.View.GetTitleContent();
                item9.Text = item9.View.GetTitleContent();
            }

            if (this.FastNavigationItemsCount > 9)
            {
                item10.View = this.Calendar.GetNewViewFromStep(-defaultStep + 9 + offset);
                item11.View = this.Calendar.GetNewViewFromStep(-defaultStep + 10 + offset);
                item10.Text = item10.View.GetTitleContent();
                item11.Text = item11.View.GetTitleContent();
            }

            if (this.fastNavigationItemsCount > 11)
            {
                item12.View = this.Calendar.GetNewViewFromStep(-defaultStep + 11 + offset);
                item13.View = this.Calendar.GetNewViewFromStep(-defaultStep + 12 + offset);
                item12.Text = item12.View.GetTitleContent();
                item13.Text = item13.View.GetTitleContent();
            }

            SetNavigationItemsVisibility();
            this.dropDown.Invalidate();
            this.hostedControl.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.Calendar.ReadOnly && this.Calendar.CalendarElement.AllowDropDownFastNavigation)
            {
                this.Capture = true;

                LoadFastNavigationItems();

                this.dropDown.Location = new Point(this.Calendar.PointToScreen(Point.Empty).X
                                                   + (this.Calendar.Width / 4),
                                                   this.Calendar.PointToScreen(Point.Empty).Y + this.ControlBoundingRectangle.Y
                                                   - (this.dropDown.Size.Height / 2));

                this.scrollingTimer.Start();
                this.dropDown.Show();

                this.hostedControl.PerformLayout();

                this.dropDown.Size = new Size(this.Calendar.Size.Width / 2, 1000);

                if (this.hostedControl.Items.Count > 0)
                    this.hostedControl.Items[0].PerformLayout();

                float itemsSize = 0;
                for (int i = 0; i < this.hostedControl.Items.Count; i++)
                {
                    itemsSize += (int)hostedControl.Items[0].DesiredSize.Height;
                }

                this.dropDown.Size = new Size(this.Calendar.Size.Width / 2,
                                              (int)itemsSize + this.hostedControl.fastNavigationElement.ChildrenMargin.Vertical
                                              + this.hostedControl.fastNavigationElement.Margin.Vertical + 2);

                this.hostedControl.fastNavigationElement.UpdateLayout();

                this.dropDown.Location = new Point(this.Calendar.PointToScreen(Point.Empty).X
                                                   + (this.Calendar.Width / 4),
                                                   this.Calendar.PointToScreen(Point.Empty).Y + this.ControlBoundingRectangle.Y
                                                   - (this.dropDown.Size.Height / 2) + 9);
            }

            base.OnMouseDown(e);
        }

        private void LoadFastNavigationItems()
        {
            this.viewIndex = 0;

            InitializeNavItems(0);

            this.hostedControl.Items.Clear();
            
            if (this.hostedControl.Items.Count == 0)
            {
                this.dropDown.SizingMode = SizingMode.None;

                int fastNavigationItemsCnt = this.FastNavigationItemsCount;
                int months =
                (this.Calendar.RangeMaxDate.Year * 12 + this.Calendar.RangeMaxDate.Month) -
                (this.Calendar.RangeMinDate.Year * 12 + this.Calendar.RangeMinDate.Month) + 1;
                if (fastNavigationItemsCnt > months)
                {
                    fastNavigationItemsCnt = months;
                }

                FastNavigationItem previosItem = null;
                for (int i = 1; i <= fastNavigationItemsCnt; i++)
                {
                    FastNavigationItem item = null;
                    switch (i)
                    {
                        case 1:
                            item = item1;
                            break;
                        case 2:
                            item = item2;
                            break;
                        case 3:
                            item = item3;
                            break;
                        case 4:
                            item = item4;
                            break;
                        case 5:
                            item = item5;
                            break;
                        case 6:
                            item = item6;
                            break;
                        case 7:
                            item = item7;
                            break;
                        case 8:
                            item = item8;
                            break;
                        case 9:
                            item = item9;
                            break;
                        case 10:
                            item = item10;
                            break;
                        case 11:
                            item = item11;
                            break;
                        case 12:
                            item = item12;
                            break;
                        case 13:
                            item = item13;
                            break;
                    }

                    if (item == null)
                        continue;

                    if (previosItem != null && item.View.ViewStartDate == previosItem.View.ViewStartDate && item.View.ViewEndDate == previosItem.View.ViewEndDate)
                    {
                        previosItem = item;
                        continue;
                    }

                    this.hostedControl.Items.Add(item);

                    previosItem = item;
                }
            }

            SetNavigationItemsVisibility();
        }

        private void SetNavigationItemsVisibility()
        {
            for (int i = 0; i < this.hostedControl.Items.Count; i++)
            {
                FastNavigationItem item = this.hostedControl.Items[i] as FastNavigationItem;
                item.Visibility = ElementVisibility.Visible;
            }

            for (int i = 0; i < this.hostedControl.Items.Count; i++)
            {
                FastNavigationItem item = this.hostedControl.Items[i] as FastNavigationItem;

                for (int j = i + 1; j < this.hostedControl.Items.Count; j++)
                {
                    FastNavigationItem item2 = this.hostedControl.Items[j] as FastNavigationItem;
                    if (item.View.Equals(item2.View))
                    {
                        item2.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }

            // Get Visible Items
            List<FastNavigationItem> visibleItems = new List<FastNavigationItem>();

            for (int i = 0; i < this.hostedControl.Items.Count; i++)
            {
                if (this.hostedControl.Items[i].Visibility == ElementVisibility.Visible)
                {
                    visibleItems.Add(this.hostedControl.Items[i] as FastNavigationItem);
                    this.hostedControl.Items[i].Visibility = ElementVisibility.Collapsed;
                }
            }

            for (int i = 0; i < this.hostedControl.Items.Count; i++)
            {
                if (visibleItems.Count > i)
                {
                    (this.hostedControl.Items[i] as FastNavigationItem).View = visibleItems[i].View;
                    (this.hostedControl.Items[i] as FastNavigationItem).Text = visibleItems[i].View.GetTitleContent();
                    (this.hostedControl.Items[i] as FastNavigationItem).Visibility = ElementVisibility.Visible;
                }
                else
                {
                    break;
                }
            }
        }

        private void scrollingTimer_Tick(object sender, EventArgs e)
        {
            if (this.shouldScroll)
            {
                int offset = 0;
                if (this.scrollUp)
                {
                    FastNavigationItem item = this.hostedControl.Items[0] as FastNavigationItem;
                    if (item.View.ViewStartDate <= this.Calendar.RangeMinDate)
                        return;
                    viewIndex--;
                    offset = 1;
                }
                else
                {
                    FastNavigationItem item = this.hostedControl.Items[this.hostedControl.Items.Count - 1] as FastNavigationItem;
                    if (item.View.ViewEndDate >= this.Calendar.RangeMaxDate)
                        return;
                    viewIndex++;
                    offset = -1;
                }

                int a = Calendar.RangeMaxDate.Month + (Calendar.RangeMaxDate.Year * 12);
                int b = Calendar.RangeMinDate.Month + (Calendar.RangeMinDate.Year * 12);
                if ((a - b) > hostedControl.Items.Count)
                {
                    CalendarView maxView = (this.hostedControl.Items[this.hostedControl.Items.Count - 1] as FastNavigationItem).View;
                    maxView = maxView.GetNextView(-offset);

                    int maxViewMonth = maxView.ViewStartDate.Month;
                    int maxViewYear = maxView.ViewStartDate.Year;

                    CalendarView minView = (this.hostedControl.Items[0] as FastNavigationItem).View;
                    minView = minView.GetNextView(-offset);

                    int minViewMonth = minView.ViewStartDate.Month;
                    int minViewYear = minView.ViewStartDate.Year;

                    InitializeNavItems(viewIndex);

                    if ((maxViewMonth >= this.Calendar.RangeMaxDate.Month && maxViewYear >= this.Calendar.RangeMaxDate.Year)
                        || (minViewMonth <= this.Calendar.RangeMinDate.Month && minViewYear <= this.Calendar.RangeMinDate.Year))
                    {
                        viewIndex += offset;
                    }
                }

                this.scrollingTimer.Interval = 2500 / (Math.Abs(offsetPoint.Y) + 1);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            offsetPoint = Point.Empty;

            this.shouldScroll = false;

            if (this.Capture)
            {
                if (this.dropDown.PointToScreen(Point.Empty).Y > Cursor.Position.Y)
                {
                    shouldScroll = true;
                    scrollUp = true;
                    offsetPoint = new Point(0,
                                            this.dropDown.PointToScreen(Point.Empty).Y - Cursor.Position.Y);
                }
                else
                if (this.dropDown.PointToScreen(Point.Empty).Y + this.dropDown.Size.Height < Cursor.Position.Y)
                {
                    shouldScroll = true;
                    scrollUp = false;
                    offsetPoint = new Point(0,
                                            this.dropDown.PointToScreen(Point.Empty).Y + this.dropDown.Size.Height - Cursor.Position.Y);
                }

                Point pt = this.dropDown.PointToClient(Cursor.Position);

                foreach (FastNavigationItem cve in this.hostedControl.Items)
                {
                    cve.Selected = false;

                    if (cve.ControlBoundingRectangle.Contains(pt))
                    {
                        cve.Selected = true;
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Capture = false;

            Point pt = this.dropDown.PointToClient(Cursor.Position);

            foreach (FastNavigationItem cve in this.hostedControl.Items)
            {
                if (cve.ControlBoundingRectangle.Contains(pt))
                {
                    CalendarView view = cve.View;
                    UpdateView(view);
                    UpdateVisuals();
                    break;
                }
            }

            this.dropDown.Hide();
            this.scrollingTimer.Stop();

            base.OnMouseUp(e);
        }
        #endregion
    }

    public class RadCalendarFastNavigationControlDesignTimeData : RadControlDesignTimeData
    {
        public RadCalendarFastNavigationControlDesignTimeData()
        {
        }

        public RadCalendarFastNavigationControlDesignTimeData(string name)
        : base(name)
        {
        }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadCalendarFastNavigationControl button = new RadCalendarFastNavigationControl();
            button.AutoSize = false;
            button.Size = new Size(120, 65);
            button.Items.Add(new FastNavigationItem(null, null));

            button.Text = "RadButton";

            RadCalendarFastNavigationControl buttonStructure = new RadCalendarFastNavigationControl();
            buttonStructure.Items.Add(new FastNavigationItem(null, null));

            button.AutoSize = true;

            buttonStructure.Text = "RadButton";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
            designed.MainElementClassName = typeof(RadCalendarFastNavigationControl).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }

    [RadThemeDesignerData(typeof(RadCalendarFastNavigationControlDesignTimeData))]
    [ToolboxItem(false)]
    public class RadCalendarFastNavigationControl : RadControl
    {
        public RadCalendarFastNavigationElement fastNavigationElement;

        public RadCalendarFastNavigationControl()
        {
            this.UseNewLayoutSystem = true;
            this.AutoSize = true;
        }

        public RadItemOwnerCollection Items
        {
            get
            {
                return this.fastNavigationElement.Items;
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.fastNavigationElement = new RadCalendarFastNavigationElement();
            this.fastNavigationElement.StretchHorizontally = true;
            this.fastNavigationElement.StretchVertically = true;
            parent.Children.Add(this.fastNavigationElement);
            parent.StretchHorizontally = true;
            parent.StretchVertically = true;
        }
    }

    public class FastNavigationItem : CalendarVisualElement
    {
        public FastNavigationItem(RadCalendar calendar, CalendarView view)
        : base(calendar, view)
        {
        }

        static FastNavigationItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new FatNavigationItemItemStateManagerFactory(), typeof(FastNavigationItem));
        }

        public static RadProperty SelectedProperty =
        RadProperty.Register("SelectedProperty", typeof(bool), typeof(FastNavigationItem),
                             new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>Gets or sets the selected cell.</summary>
        [Description("Indicates that current element selected.")]
        [RadPropertyDefaultValue("SelectedProperty", typeof(FastNavigationItem)), Category(RadDesignCategory.BehaviorCategory)]
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
    }

    public class FatNavigationItemItemStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeWithCondition selected = new StateNodeWithCondition("Selected", new SimpleCondition(FastNavigationItem.SelectedProperty, true));
            return selected;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Selected");
        }
    }

    public class RadCalendarFastNavigationElement : LightVisualElement
    {
        private RadItemOwnerCollection items;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(FastNavigationItem) };
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
            this.UseNewLayoutSystem = true;
            this.DrawFill = true;
            this.DrawBorder = true;
            this.Shape = new RoundRectShape(5);
            this.Margin = new Padding(1, 1, 1, 1);
        }

        protected override void DisposeManagedResources()
        {
            this.items.ItemsChanged -= new ItemChangedDelegate(items_ItemsChanged);

            base.DisposeManagedResources();
        }

        static RadCalendarFastNavigationElement()
        {
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadCalendarFastNavigationControl().DeserializeTheme();
        }

        private Padding childrenMargin = new Padding(1, 1, 1, 1);

        public Padding ChildrenMargin
        {
            get
            {
                return this.childrenMargin;
            }
            set
            {
                this.childrenMargin = value;
            }
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                this.Children.Add(target);
            }
            else if (operation == ItemsChangeOperation.Cleared)
            {
                for (int i = this.Children.Count - 1; i >= 0; i--)
                {
                    FastNavigationItem item = this.Children[i] as FastNavigationItem;
                    if (item != null)
                        this.Children.Remove(item);
                }
            }
        }

        /// <summary>
        /// Gets the items collection of the element
        /// </summary>
        [Description("Gets the items collection of the element")]
        [Category("Behavior")]
        [DefaultValue(null)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            if (this.ElementTree != null && this.ElementTree.Control.Parent != null)
            {
                float width = 0;
                float height = 0;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    SizeF itemSize = this.Items[i].DesiredSize;
                    height = Math.Max(height, itemSize.Height);
                    width = Math.Max(width, itemSize.Width);
                }

                for (int j = 0; j < this.Items.Count; j++)
                {
                    this.Items[j].Arrange(new RectangleF(this.ChildrenMargin.Left,
                                                         (j * height) + this.ChildrenMargin.Top,
                                                         finalSize.Width - this.ChildrenMargin.Horizontal - this.Margin.Horizontal,
                                                         height));
                }
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }

            return finalSize;
        }

        protected override void CreateChildElements()
        {
        }
    }
}