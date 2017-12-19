using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class CalendarVisualElement : LightVisualElement
    {
        #region Fields
        private CalendarView view = null;
        private RadCalendar calendar = null;
        private CalendarVisualElement owner = null; 
        #endregion

        #region Constructors

        public CalendarVisualElement(RadCalendar calendar)
            : this(null, calendar, null)
        {
        }

        public CalendarVisualElement(RadCalendar calendar, CalendarView view)
            : this(null, calendar, view)
        {
        }

        public CalendarVisualElement(CalendarVisualElement owner)
            : this(owner, null, null)
        {
        }

        public CalendarVisualElement(CalendarVisualElement owner, RadCalendar calendar, CalendarView view)
        {
            this.calendar = calendar;
            this.owner = owner;
            this.view = view;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = true;
            this.DrawBorder = true;
            this.Class = "CalendarVisualElement";
        }

        protected override void DisposeManagedResources()
        {
            this.view = null;
            this.calendar = null;
            this.owner = null;

            base.DisposeManagedResources();
        }

        #endregion

        #region Properties
        /// <summary>
        /// 	<para>Exposes the top instance of <strong>CalendarView</strong> or its derived
        ///     types.</para>v
        /// 	<para>Every <strong>CalendarView</strong> class handles the real calculation and
        ///     rendering of <strong>RadCalendar</strong>'s calendric information. The
        ///     <strong>CalendarView</strong> has the
        ///     <a href="RadCalendar~Telerik.WebControls.Base.Calendar.CalendarView~ChildViews.html">
        ///     ChildViews</a> collection which contains all the sub views in case of multi view
        ///     setup.</para>
        /// </summary>
        [Browsable(false)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CalendarView View
        {
            get
            {
                if (this.Owner != null)
                {
                    return this.Owner.View;
                }
                return this.view;
            }
            set
            {
                if (this.view != value)
                {
                    this.view = value;
                }
            }
        }

        /// <summary>
        /// Gets the parent calendar that the current view is assigned to.
        /// </summary>
        [Browsable(false),
        DefaultValue(null),
        Description("Gets the parent calendar that the current view is assigned to."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadCalendar Calendar
        {
            get
            {
                if (this.Owner != null)
                {
                    return this.Owner.Calendar;
                }
                return this.calendar;
            }
            internal set
            {
                if (this.calendar != value)
                {
                    this.calendar = value;
                }
            }
        }

        internal protected virtual CalendarVisualElement Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }
        #endregion

        #region Methods
        internal protected virtual void RenderVisuals()
        {
        }

        internal protected virtual void RefreshVisuals()
        {
            RefreshVisuals(false);
        }
        internal protected virtual void RefreshVisuals(bool unconditional)
        {
        } 
        #endregion
    }
}
