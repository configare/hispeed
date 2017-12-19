using System.ComponentModel;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	public class MultiMonthViewElement : MonthViewElement
	{
        #region Fields
        private CalendarMultiMonthViewTableElement tableElement; 
        #endregion

        #region Constructors
        public MultiMonthViewElement(RadCalendar calendar)
            : this(calendar, null)
        {
        }

        public MultiMonthViewElement(RadCalendar calendar, CalendarView view)
            : base(calendar, view)
        {
        } 
        #endregion

        #region Properties
        public override CalendarView View
		{
			get
			{
				return base.View;
			}
			set
			{
				if ((base.View != value) && (value != null))
				{
					int count = base.View.MultiViewColumns * base.View.MultiViewRows;
					int newCount = value.MultiViewColumns * value.MultiViewRows;

					if (count == newCount)
					{
						if (this.tableElement != null)
						{
							this.tableElement.View = value;
							this.titleElement.View = value;
							this.titleElement.Text = value.GetTitleContent();
						}
					}

					base.View = value;

					if (count != newCount)
					{
						this.InitializeChildren();
					}
				}
			}
		}
        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "MultiMonthViewElement";
        }

        public CalendarMultiMonthViewTableElement GetMultiTableElement()
        {
            return this.tableElement;
        }

        internal protected override void RefreshVisuals()
        {
            if (this.tableElement != null)
            {
                this.tableElement.RefreshVisuals();
            }
        }

        internal protected override void RefreshVisuals(bool unconditional)
        {
            if (this.tableElement != null)
            {
                this.tableElement.RefreshVisuals(unconditional);
            }
        }

        protected internal override void SetProperty(PropertyChangedEventArgs e)
        {
            base.SetProperty(e);

            switch (e.PropertyName)
            {
                case "MonthLayout":
                case "Orientation":
                case "ShowColumnHeaders":
                case "ShowRowHeaders":
                case "HeaderHeight":
                case "HeaderWidth":
                case "MultiViewRows":
                case "MultiViewColumns":
                case "ShowViewSelector":
                case "Columns":
                case "Culture":
                case "Rows":
                    if (this.tableElement != null)
                        this.tableElement.Recreate();
                    break;
            }
        }

        protected override void InitializeChildren()
        {
            this.DisposeChildren();

            this.dockLayout = new DockLayoutPanel();

            this.titleElement = new TitleElement();
            this.titleElement.SetValue(DockLayoutPanel.DockProperty, Dock.Top);
            this.titleElement.StretchVertically = false;
            this.titleElement.Text = "Some Text Here";
            this.titleElement.Visibility = ElementVisibility.Collapsed;

            this.dockLayout.Children.Add(this.titleElement);

            this.tableElement = new CalendarMultiMonthViewTableElement(this, this.Calendar, this.View);

            this.dockLayout.Children.Add(this.tableElement);

            this.Children.Add(this.dockLayout);
        } 
        #endregion
	}
}
