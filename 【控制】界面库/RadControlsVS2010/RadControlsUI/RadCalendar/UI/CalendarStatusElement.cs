using System;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class CalendarStatusElement : CalendarVisualElement
    {
        #region Fields
        protected RadButtonElement todayButtonElement = null;
        protected RadButtonElement clearButtonElement = null;
        protected BoxLayout boxLayout = null;
        protected Timer timer;
        private RadCalendarElement owner;
        private RadLabelElement label = null;

        private DockLayoutPanel dockLayout;

        private string labelFormat = string.Empty;
        #endregion

        #region Constructors
        internal CalendarStatusElement(RadCalendarElement owner)
            : base(owner)
        {
            this.owner = owner;
            this.owner.Calendar.PropertyChanged += new PropertyChangedEventHandler(Calendar_PropertyChanged);
        }
        #endregion

        #region Properties
        [DefaultValue(false)]
        public override bool StretchVertically
        {
            get { return base.StretchVertically; }
            set { base.StretchVertically = value; }
        }

        public RadButtonElement TodayButton
        {
            get
            {
                return this.todayButtonElement;
            }
        }

        public RadButtonElement ClearButton
        {
            get
            {
                return this.clearButtonElement;
            }
        }

        /// <summary>
        /// today button
        /// </summary>        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadButtonElement TodayButtonElement
        {
            get
            {
                return this.todayButtonElement;
            }
            set
            {
                this.todayButtonElement = value;
            }
        }

        /// <summary>
        /// label element
        /// </summary>        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadLabelElement LabelElement
        {
            get
            {
                return this.label;
            }
        }

        /// <summary>
        /// Gets or sets date time format
        /// </summary>        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LableFormat
        {
            get
            {
                return this.labelFormat;
            }
            set
            {
                this.labelFormat = value;
            }
        }
        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Visibility = ElementVisibility.Collapsed;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.DrawBorder = false;
            this.DrawFill = false;
            this.Margin = new Padding(10, 5, 10, 5);
            this.UseNewLayoutSystem = true;
            this.StretchVertically = false;
            this.DrawFill = true;
            this.Class = "CalendarStatusElement";

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Enabled = true;
            this.timer.Tick += new EventHandler(timer_Tick);
        }

        protected override void DisposeManagedResources()
        {
            if (this.owner != null && this.owner.Calendar != null)
            {
                this.owner.Calendar.PropertyChanged -= new PropertyChangedEventHandler(Calendar_PropertyChanged);
            }
            if (this.timer != null)
            {
                this.timer.Tick -= timer_Tick;
                this.timer.Dispose();
            }
            base.DisposeManagedResources();
        }

        public void HideText()
        {
            StopTimer();
            this.label.Text = "";
        }

        internal protected virtual void StartTimer()
        {
            this.timer.Start();
        }

        internal protected virtual void StopTimer()
        {
            this.timer.Stop();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.dockLayout = new DockLayoutPanel();
            this.dockLayout.StretchHorizontally = true;
            this.dockLayout.StretchVertically = false;

            this.label = new RadLabelElement();
            this.label.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Right : Dock.Left);
            this.label.Class = "FooterDate";
            this.label.TextWrap = false;

            this.todayButtonElement = new RadButtonElement();
            this.todayButtonElement.Text = "Today";
            this.todayButtonElement.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Left : Dock.Right);
            this.todayButtonElement.Margin = new Padding(5, 0, 5, 0);
            this.todayButtonElement.Click += new EventHandler(todayButtonElement_Click);

            this.clearButtonElement = new RadButtonElement();
            this.clearButtonElement.Text = "Clear";
            this.clearButtonElement.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Left : Dock.Right);
            this.clearButtonElement.Margin = new Padding(5, 0, 5, 0);
            this.clearButtonElement.Visibility = ElementVisibility.Visible;
            this.clearButtonElement.Click += new EventHandler(clearButtonElement_Click);

            this.dockLayout.Children.Add(this.todayButtonElement);
            this.dockLayout.Children.Add(this.clearButtonElement);
            this.dockLayout.Children.Add(this.label);

            this.Children.Add(this.dockLayout);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RightToLeftProperty)
            {
                this.label.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Right : Dock.Left);
                this.todayButtonElement.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Left : Dock.Right);
                this.clearButtonElement.SetValue(DockLayoutPanel.DockProperty, (this.RightToLeft) ? Dock.Left : Dock.Right);
            } 
        }

        private static void ClearMonthSelection(MonthViewElement currentMonth)
        {
            if (currentMonth.Calendar != null)
            {
                currentMonth.Calendar.SelectedDates.Clear();
            }

            for (int i = 0; i < currentMonth.TableElement.Children.Count; i++)
            {
                CalendarCellElement cell = currentMonth.TableElement.Children[i] as CalendarCellElement;
                if (cell != null)
                {
                    cell.isChecked = false;
                    cell.Selected = false;
                }
            }
        } 
        #endregion

        #region Events
        private void todayButtonElement_Click(object sender, EventArgs e)
        {
            this.owner.Calendar.FocusedDate = DateTime.Now;
        }

        private void clearButtonElement_Click(object sender, EventArgs e)
        {
            this.Calendar.SelectedDates.Clear();

            if (this.Calendar.CalendarElement.CalendarVisualElement is MultiMonthViewElement)
            {
                foreach (MonthViewElement currentMonth in this.Calendar.CalendarElement.CalendarVisualElement.Children[0].Children[1].Children)
                {
                    ClearMonthSelection(currentMonth);
                }
            }
            else if (this.Calendar.CalendarElement.CalendarVisualElement is MonthViewElement)
            {
                ClearMonthSelection(this.Calendar.CalendarElement.CalendarVisualElement as MonthViewElement);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.Calendar.ShowFooter)
            {
                this.label.Text = DateTime.Now.ToString(this.labelFormat, this.Calendar.Culture);
            }
            else
            {
                StopTimer();
            }
        }

        private void Calendar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowFooter")
            {
                if (this.owner.Calendar.ShowFooter)
                    this.Visibility = ElementVisibility.Visible;
                else
                    this.Visibility = ElementVisibility.Collapsed;
            }
        }
        #endregion
    }
}
