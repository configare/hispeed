using System;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class MonthViewElement : CalendarVisualElement
    {
        #region Fields
        private CalendarTableElement tableElement;
        protected TitleElement titleElement;
        protected DockLayoutPanel dockLayout;
        internal int Row;
        internal int Column;

        private CalendarView view; 
        #endregion

        #region Constructors
        public MonthViewElement(RadCalendar calendar)
            : this(calendar, null)
        {
        }

        public MonthViewElement(RadCalendar calendar, CalendarView view)
            : base(calendar, view)
        {
            this.view = view;
            this.view.PropertyChanged += new PropertyChangedEventHandler(view_PropertyChanged);
            this.InitializeChildren();
        }
        #endregion

        #region Properties
        public override CalendarView View
        {
            get
            {
                return this.view;
            }
            set
            {
                if ((this.view != value) && (value != null))
                {
                    int count = this.view.Columns * this.view.Rows;
                    int newCount = value.Columns * value.Rows;

                    if (count == newCount && this.tableElement != null)
                    {
                        this.tableElement.View = value;
                        this.titleElement.View = value;
                        this.titleElement.Text = value.GetTitleContent();
                    }

                    this.view = value;

                    if (count != newCount)
                    {
                        this.InitializeChildren();
                    }
                }
            }
        }

        public virtual CalendarTableElement TableElement
        {
            get
            {
                return tableElement;
            }
            set
            {
                tableElement = value;
            }
        }

        public TitleElement TitleElement
        {
            get
            {
                return this.titleElement;
            }
            set
            {
                this.titleElement = value;
            }
        } 
        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "MonthViewElement";
            this.DrawFill = false;
            this.DrawBorder = false;
        }

        protected override void DisposeManagedResources()
        {
            if (this.View != null)
            {
                view.PropertyChanged -= new PropertyChangedEventHandler(view_PropertyChanged);
                this.Calendar = null;
                this.view = null;
                if (this.tableElement != null)
                {
                    this.tableElement.Dispose();
                    this.tableElement = null;
                }

                if (this.titleElement != null)
                {
                    this.titleElement.Dispose();
                    this.titleElement = null;
                }
            }

            base.DisposeManagedResources();
        }

        internal protected virtual void SetProperty(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowViewHeader":

                    if (this.titleElement != null)
                    {
                        this.titleElement.Visibility = this.View.ShowHeader ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                    }
                    break;
                case "Orientation":
                    if (this.tableElement != null)
                    {
                        this.tableElement.Recreate();
                    }
                    break;
            }
        }

        private void view_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetProperty(e);
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

        protected virtual void InitializeChildren()
        {
            this.DisposeChildren();

            this.dockLayout = new DockLayoutPanel();

            this.titleElement = new TitleElement();
            this.titleElement.SetValue(DockLayoutPanel.DockProperty, Dock.Top);
            this.titleElement.StretchVertically = false;
            this.titleElement.Text = "Some Text Here";
            this.titleElement.Visibility = ElementVisibility.Collapsed;

            this.dockLayout.Children.Add(this.titleElement);
            this.CreateTableElement();
            this.Children.Add(this.dockLayout);
        }

        private void CreateTableElement()
        {
            this.tableElement = new CalendarTableElement(this, this.Calendar, this.View);
            this.dockLayout.Children.Add(this.tableElement);
        }

        internal protected virtual void RenderContent(DateTime firstDay, DateTime visibleDate, Orientation orientation)
        {
        }

        public virtual void Initialize()
        {
        }

        internal protected virtual void CreateVisuals()
        {

        } 
        #endregion
    }
}
