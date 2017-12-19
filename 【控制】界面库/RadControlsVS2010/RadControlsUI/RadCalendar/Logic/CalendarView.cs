using System;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Summary description for CalendarView.
	/// </summary>
    public abstract class CalendarView : INotifyPropertyChanged
	{
        // Fields
        private Padding? cellMargin;
        private Padding? cellPadding;
        private RadCalendar calendar = null;
        private CalendarView parent = null;
        protected CalendarViewCollection children = null;
        private System.Windows.Forms.VisualStyles.ContentAlignment titleAlign = System.Windows.Forms.VisualStyles.ContentAlignment.Center;
		private Orientation? orientation = null;
        private string titleContent = String.Empty;
        private bool visible = false;
        private string name = string.Empty;
        private int depth = -1;
        private bool? readOnly = null;
        private bool? showHeader;
		private bool? showSelector;
        private bool? showColumnHeaders;
        private bool? showRowHeaders;
        private bool? allowRowHeaderSelectors;
        private bool? allowColumnHeaderSelectors;
        private bool? allowViewSelector;
        private bool isMultiView = false;
        private string cellToolTipFormat;
        private bool? allowToolTips;
        private string conditionsErrorMessage = String.Empty;
        private DateTime viewRenderEndDate = DateTime.MinValue;
        private DateTime viewRenderStartDate = DateTime.MinValue;
        private DateTime viewEndDate = DateTime.MinValue;
        private DateTime viewStartDate = DateTime.MinValue;
		private Image viewSelectorImage;
        private string viewSelectorText = String.Empty;
        private Image columnHeaderImage;
		private string columnHeaderText = "";
		private Image rowHeaderImage;
        private string rowHeaderText = "";
        private string titleFormat = String.Empty;
        private int columns = 1;
        private int rows = 1;
        private DateTime currentViewEndDate = DateTime.MinValue;
        private DateTime currentViewBeginDate = DateTime.MinValue;
        private Rectangle bounds = Rectangle.Empty;
		private MonthLayout? monthLayout;
		private int? headerWidth;
		private int? headerHeight;
		private float? zoomFactor;
		private bool? allowFishEye;
		private bool? showOtherMonthDays;
        private int? cellVerticalSpacing;
        private int? cellHorizontalSpacing;
        private int? multiViewRows;
        private int? multiViewColumns;

        #region Constructors
        internal CalendarView(RadCalendar parent)
            : this(parent, null)
        {
        }

        internal CalendarView(RadCalendar parent, CalendarView parentView): this(parent, parentView, false, parent != null ? parent.Rows : 6, parent != null ? parent.Columns : 7)
        {
        }

        internal CalendarView(RadCalendar parent, CalendarView parentView, bool isMultiView, int rows, int columns)
        {
            //this.Calendar = parent;
            this.calendar = parent;
            this.parent = parentView;
            this.isMultiView = isMultiView;
            this.rows = rows;
            this.columns = columns;

			if ((this.calendar.MultiViewColumns > 1 || this.calendar.MultiViewRows > 1) && this.GetType() == typeof(MultiMonthView))
				this.Calendar.PropertyChanged += new PropertyChangedEventHandler(Calendar_PropertyChanged);
			else
			{
				if (this.calendar.MultiViewColumns == 1 && this.calendar.MultiViewRows == 1 )
					this.Calendar.PropertyChanged += new PropertyChangedEventHandler(Calendar_PropertyChanged);	
			}
		}


        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent calendar that the current view is assigned to.
        /// </summary>
        [Browsable(false),
        DefaultValue(null),
        Description("Gets the parent calendar that the current view is assigned to."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadCalendar Calendar
        {
            get
            {
                if (this.parent != null)
                {
                    return this.parent.Calendar;
                }
                return this.calendar;
            }
            internal set
            {
                if (this.calendar != value)
                {
                    this.calendar = value;
                    this.OnNotifyPropertyChanged("Calendar");
                }
            }
        }

        /// <summary>
        /// Gets the parent tree node of the current tree node. 
        /// </summary>
        [Browsable(false),
        DefaultValue(null),
        Description("Gets the parent tree node of the current tree node."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CalendarView Parent
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                if (this.parent != value)
                {
                    this.parent = value;
                    this.OnNotifyPropertyChanged("Parent");
                }
            }
        }

        /// <summary>
        /// Gets the collection of nodes that are assigned to the tree view control.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets the collection of nodes that are assigned to the tree view control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CalendarViewCollection Children
        {
            get
            {
                this.EnsureChildViews();
                return this.children;
            }
        }

        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        [Category("Appearance"),
        Description("Gets or sets the name of the node."),
        DefaultValue("")]
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.name))
                {
                    return string.Empty;
                }
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnNotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether tool tips are displayed for this speciffic control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Gets or sets whether tool tips are displayed for this speciffic control.")]
        [Localizable(true)]
        public virtual bool AllowToolTips
        {
            get
            {
                if (this.allowToolTips.HasValue)
                {
                    return this.allowToolTips.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.AllowToolTips;
                }
                return true;
            }
            set
            {
                if (this.allowToolTips != value)
                {
                    this.allowToolTips = value;
                    this.OnNotifyPropertyChanged("AllowToolTips");
                }
            }
        }

        /// <summary>Gets or sets the format string that is applied to the days cells tooltip.</summary>
        /// <remarks>
        /// 	<para>The <i>property</i> should contain either a format specifier character or a
        ///     custom format pattern. For more information, see the summary page for
        ///     <strong>System.Globalization.DateTimeFormatInfo</strong>.</para>
        /// 	<para>By default this <em>property</em> uses formatting string of
        ///     '<font size="2"><strong>dddd, MMMM dd, yyyy</strong>'. Valid formats are all supported by the .NET
        ///     Framework.</font></para>
        /// 	<para><font size="2">Example:</font></para>
        /// 	<ul class="noindent">
        /// 		<li>"d" is the standard short date pattern.</li>
        /// 		<li>"%d" returns the day of the month; "%d" is a custom pattern.</li>
        /// 		<li>"d " returns the day of the month followed by a white-space character; "d "
        ///         is a custom pattern.</li>
        /// 	</ul>
        /// </remarks>
        [NotifyParentProperty(true)]
        [Category("Title Settings")]
        [DefaultValue("dddd, MMMM dd, yyyy")]
        [Description("Gets or sets the format string that is applied to the days cells tooltip.")]
        [Localizable(true)]
        public string CellToolTipFormat
        {
            get
            {
                if (string.IsNullOrEmpty(cellToolTipFormat))
                {
                    return "dddd, MMMM dd, yyyy";
                }
                return cellToolTipFormat;
            }
            set
            {
                if (this.cellToolTipFormat != value)
                {
                    this.cellToolTipFormat = value;
                    this.OnNotifyPropertyChanged("CellToolTipFormat");
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation (rendering direction) of the calendar component.
        /// Default value is <strong>Horizontal</strong>.
        /// </summary>
        /// <remarks>
        /// 	<list type="table">
        /// 		<listheader>
        /// 			<term>Member</term>
        /// 			<description>Description</description>
        /// 		</listheader>
        /// 		<item>
        /// 			<term><strong>Horizontal</strong></term>
        /// 			<description>Renders the calendar data row after row.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><strong>Vertical</strong></term>
        /// 			<description>Renders the calendar data column after
        ///             column.</description>
        /// 		</item>
        /// 	</list>
        /// </remarks>
        [Category("Behavior")]
        [DefaultValue(Orientation.Horizontal)]
        [Description("Gets or sets the orientation (rendering direction) of the calendar component.")]
        [NotifyParentProperty(true)]
        public Orientation Orientation
        {
            get
            {
                if (this.orientation.HasValue)
                {
                    return this.orientation.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.Orientation;
                }
                return Orientation.Horizontal;
            }
            set
            {
                if (this.orientation != value)
                {
                    this.orientation = value;					
                    this.OnNotifyPropertyChanged("Orientation");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the horizontal alignment of the view title.</para>
        /// 	<para>The ContentAlignment enumeration is defined in
        ///     <strong>System.Windows.Forms.VisualStyles</strong></para>
        /// </summary>
        /// <remarks>
        /// 	<list type="table">
        /// 		<listheader>
        /// 			<term>
        /// 				<para align="left">Member name</para>
        /// 			</term>
        /// 			<description>
        /// 				<para align="left">Description</para>
        /// 			</description>
        /// 		</listheader>
        /// 		<item>
        /// 			<term>
        /// 				<para align="left"><b>Center</b></para>
        /// 			</term>
        /// 			<description>The contents of a container are centered.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Left</b></term>
        /// 			<description>The contents of a container are left justified.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Right</b></term>
        /// 			<description>The contents of a container are right justified.</description>
        /// 		</item>
        /// 	</list>
        /// </remarks>
        [NotifyParentProperty(true)]
        [Category("Title Settings")]
        [DefaultValue(System.Windows.Forms.VisualStyles.ContentAlignment.Center)]
        [Description("Gets or sets the horizontal alignment of the view title.")]
        public virtual System.Windows.Forms.VisualStyles.ContentAlignment TitleAlign
        {
            get
            {
                if (titleAlign != System.Windows.Forms.VisualStyles.ContentAlignment.Center)
                {
                    return this.titleAlign;
                }
                if (this.Parent != null)
                {
                    return this.Parent.TitleAlign;
                }
                if (this.Calendar != null)
                {
                    return this.Calendar.TitleAlign;
                }
                return this.titleAlign;
            }
            set
            {
                if (this.titleAlign != value)
                {
                    this.titleAlign = value;
                    this.OnNotifyPropertyChanged("TitleAlign");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree node is visible or partially visible. 
        /// </summary>
        [//Browsable(false),
        DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                if (this.visible != value)
                {
                    this.visible = value;
                    this.OnNotifyPropertyChanged("Visible");
                }
            }
        }

        /// <summary>
        /// Gets the root parent node for this instance.
        /// </summary>
        [Browsable(false), DefaultValue(null),
        Description("Gets the root parent node for this instance."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarView RootView
        {
            get
            {
                CalendarView view = this;
                while (view.Parent != null)
                {
                    view = view.Parent;
                }
                return view;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CalendarView is the top most view displayed by RadCalendar.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsRootView
        {
            get
            {
                if (this.Parent == null)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the zero-based depth of the tree node in the RadTreeView tree. 
        /// Returns -1 if the node is outside of a tree view.
        /// </summary>
        [Browsable(false),
        DefaultValue(-1),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                if (this.parent != null)
                {
                    return this.parent.depth + 1;
                }
                else if (this.calendar != null)
                {
                    return 0;
                }
                return -1;
            }
        }

        //PresentationType presentationType = PresentationType.Interactive;

        /// <summary>
        /// Gets or sets a value indicating whether the calendar view is in read-only mode.
        /// </summary>
        [Description("Gets or sets a value indicating whether the calendar view is in read-only mode."),
        DefaultValue(false),
        Category("Behavior")]
        public virtual bool ReadOnly
        {
            get
            {

                if (this.readOnly.HasValue)
                {
                    return this.readOnly.Value;
                }
                else
                {
                    if (this.Calendar != null)
                    {
                        return this.Calendar.ReadOnly;
                    }
                }

                return false;
            }

            set
            {
                if (this.readOnly != value)
                {
                    this.readOnly = value;
                    this.OnNotifyPropertyChanged("ReadOnly");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the text displayed for the complete
        ///     <a href="RadCalendar~Telerik.WebControls.Base.Calendar.CalendarView.html">CalendarView</a>
        ///     selection element in the view selector cell.</para>
        /// </summary>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> selection element in the
        /// selector cell. The default value is <b>""</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <strong>ViewSelectorText</strong> property to provide custom text for
        ///     the <strong>CalendarView</strong> complete selection element in the selector
        ///     cell.</para>
        /// 	<div>
        /// 		<list type="table">
        /// 			<item>
        /// 				<term><img src="images/hs-tip.gif"/></term>
        /// 				<description>
        /// 					<para>This property does not automatically encode to HTML. You need
        ///                     to convert special characters to the appropriate HTML value, unless
        ///                     you want the characters to be treated as HTML. For example, to
        ///                     explicitly display the greater than symbol (&gt;), you must use the
        ///                     value <strong>&amp;gt;</strong>.</para>
        /// 				</description>
        /// 			</item>
        /// 		</list>
        /// 	</div>
        /// 	<para>Because this property does not automatically encode to HTML, it is possible
        ///     to specify an HTML tag for the <strong>ViewSelectorText</strong> property. For
        ///     example, if you want to display an image for the next month navigation control, you
        ///     can set this property to an expression that contains an
        ///     <strong>&lt;img&gt;</strong> element.</para>
        /// 	<para>This property applies only if the <strong>EnableViewSelector</strong>
        ///     property is set to <strong>true</strong>.</para>
        /// </remarks>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("The text displayed in the view selector cell.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string ViewSelectorText
        {
            get
            {
                if (!String.IsNullOrEmpty(this.viewSelectorText))
                {
                    return this.viewSelectorText;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ViewSelectorText;
                }

                return "";
            }
            set
            {
                if (this.viewSelectorText != value)
                {
                    this.viewSelectorText = value;
                    this.OnNotifyPropertyChanged("ViewSelectorText");
                }
            }
        }

        /// <remarks>
        /// 	<para>Use the <strong>RowHeaderText</strong> property to provide custom text for
        ///     the <strong>CalendarView</strong> complete row header element.</para>
        /// 	<div>
        /// 		<list type="table">
        /// 			<item>
        /// 				<term><img src="images/hs-tip.gif"/></term>
        /// 				<description>
        /// 					<para>This property does not automatically encode to HTML. You need
        ///                     to convert special characters to the appropriate HTML value, unless
        ///                     you want the characters to be treated as HTML. For example, to
        ///                     explicitly display the greater than symbol (&gt;), you must use the
        ///                     value <strong>&amp;gt;</strong>.</para>
        /// 				</description>
        /// 			</item>
        /// 		</list>
        /// 	</div>
        /// 	<para>Because this property does not automatically encode to HTML, it is possible
        ///     to specify an HTML tag for the <strong>RowHeaderText</strong> property. For
        ///     example, if you want to display an image for the next month navigation control, you
        ///     can set this property to an expression that contains an
        ///     <strong>&lt;img&gt;</strong> element.</para>
        /// 	<para>This property applies only if the <strong>ShowRowsHeaders</strong>
        ///     property is set to <strong>true</strong>.</para>
        /// </remarks>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> header element. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the text displayed for the row header element.
        /// </summary>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("Provides custom text for the row header cells.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string RowHeaderText
        {
            get
            {
                if (this.rowHeaderText != "")
                {
                    return this.rowHeaderText;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.RowHeaderText;
                }

                return "";
            }
            set
            {
                if (this.rowHeaderText != value)
                {
                    this.rowHeaderText = value;
                    this.OnNotifyPropertyChanged("RowHeaderText");
                }
            }
        }

        /// <remarks>
        /// 	<para>Use the <strong>ColumnHeaderText</strong> property to provide custom text
        ///     for the <strong>CalendarView</strong> complete column header element.</para>
        /// 	<div>
        /// 		<list type="table">
        /// 			<item>
        /// 				<term><img src="images/hs-tip.gif"/></term>
        /// 				<description>
        /// 					<para>This property does not automatically encode to HTML. You need
        ///                     to convert special characters to the appropriate HTML value, unless
        ///                     you want the characters to be treated as HTML. For example, to
        ///                     explicitly display the greater than symbol (&gt;), you must use the
        ///                     value <strong>&amp;gt;</strong>.</para>
        /// 				</description>
        /// 			</item>
        /// 		</list>
        /// 	</div>
        /// 	<para>Because this property does not automatically encode to HTML, it is possible
        ///     to specify an HTML tag for the <strong>ColumnHeaderText</strong> property. For
        ///     example, if you want to display an image for the next month navigation control, you
        ///     can set this property to an expression that contains an
        ///     <strong>&lt;img&gt;</strong> element.</para>
        /// 	<para>This property applies only if the <strong>ShowColumnHeaders</strong>
        ///     property is set to <strong>true</strong>.</para>
        /// </remarks>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> column header element. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the text displayed for the column header element.
        /// </summary>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("Provides custom text for the column header cells.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string ColumnHeaderText
        {
            get
            {
                if (this.columnHeaderText != "")
                {
                    return this.columnHeaderText;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ColumnHeaderText;
                }

                return "";
            }
            set
            {
                if (this.columnHeaderText != value)
                {
                    this.columnHeaderText = value;
                    this.OnNotifyPropertyChanged("ColumnHeaderText");
                }
            }
        }
        /// <value>
        /// The image displayed for the <strong>CalendarView</strong> column header element in the
        /// header cells. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the image displayed for the column header element.
        /// </summary>
        /// <remarks>
        /// 	<para>This property applies only if the <strong>ShowColumnHeaders</strong> property
        ///     is set to <strong>true</strong>. If <strong>ColumnHeaderText</strong> is set too,
        ///     its value is set as an alternative text to the image of the column header.</para>
        /// 	<para>When using this property, the whole image URL is generated using also the
        ///     value.</para>
        /// 	<para>Example:</para>
        /// 	<para><strong>ShowColumnHeaders</strong>="true"<br/>
        /// 		<strong>ImagesBaseDir</strong> = "Img/"<br/>
        /// 		<strong>ColumnHeaderImage</strong> = "selector.gif"<br/>
        /// 		<strong>complete image URL</strong> : "Img/selector.gif"</para>
        /// </remarks>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("The image displayed for the column header cells.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image ColumnHeaderImage
        {
            get
            {
                if (this.columnHeaderImage != null)
                {
                    return this.columnHeaderImage;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ColumnHeaderImage;
                }

                return null;
            }
            set
            {
                if (this.columnHeaderImage != value)
                {
                    this.columnHeaderImage = value;
                    this.OnNotifyPropertyChanged("ColumnHeaderImage");
                }
            }
        }

        /// <value>
        /// The image displayed for the <strong>CalendarView</strong> row header element. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the image displayed for the row header element.
        /// </summary>
        /// <remarks>
        /// 	<para>This property applies only if the <strong>ShowRowHeaders</strong> property is
        ///     set to <strong>true</strong>. If <strong>RowHeaderText</strong> is set too, its
        ///     value is set as an alternative text to the image of the row header.</para>
        /// 	<para>When using this property, the whole image URL is generated using also the
        ///     value.</para>
        /// 	<para>Example:<br/>
        /// 		<strong>ShowRowHeaders</strong> = "true"<br/>
        /// 		<strong>ImagesBaseDir</strong> = "Img/"<br/>
        /// 		<strong>RowHeaderImage</strong> = "selector.gif"<br/>
        /// 		<strong>complete image URL</strong> : "Img/selector.gif"</para>
        /// </remarks>
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("The image displayed for the <strong>CalendarView</strong> row header element.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image RowHeaderImage
        {
            get
            {
                if (this.rowHeaderImage != null)
                {
                    return this.rowHeaderImage;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.RowHeaderImage;
                }

                return null;
            }
            set
            {
                if (this.rowHeaderImage != value)
                {
                    this.rowHeaderImage = value;
                    this.OnNotifyPropertyChanged("RowHeaderImage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the margin of the view cells
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the margin of the view cells")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Padding CellMargin
        {
            get
            {
                if (this.cellMargin != null)
                {
                    return this.cellMargin.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.CellMargin;
                }

                return Padding.Empty;
            }
            set
            {

                if (this.cellMargin != value)
                {
                    this.cellMargin = value;
                    this.OnNotifyPropertyChanged("CellMargin");
                }
            }
        }

        /// <summary>
        /// Gets or sets the margin of the view cells
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the margin of the view cells")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Padding CellPadding
        {
            get
            {
                if (this.cellPadding != null)
                {
                    return this.cellPadding.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.CellPadding;
                }

                return Padding.Empty;
            }
            set
            {

                if (this.cellPadding != value)
                {
                    this.cellPadding = value;
                    this.OnNotifyPropertyChanged("CellPadding");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the image displayed for the complete
        ///     selection element in the view selector cell.</para>
        /// </summary>
        /// <value>
        /// The image displayed for the <strong>CalendarView</strong> selection element in
        /// the selector cell. The default value is <b>""</b>.
        /// </value>
        /// <remarks>
        /// 	<para>When using this property, the whole image URL is generated using also the
        ///     value.</para>
        /// 	<para>Example:<br/>
        /// 		<strong>ImagesBaseDir</strong> = "Img/"<br/>
        /// 		<strong>ViewSelectorImage</strong> = "selector.gif"<br/>
        /// 		<strong>complete image URL</strong> : "Img/selector.gif"</para>
        /// </remarks>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("The image displayed in the view selector cell.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image ViewSelectorImage
        {
            get
            {
                if (this.viewSelectorImage != null)
                {
                    return this.viewSelectorImage;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ViewSelectorImage;
                }

                return null;
            }
            set
            {
                if (this.viewSelectorImage != value)
                {
                    this.viewSelectorImage = value;
                    this.OnNotifyPropertyChanged("ViewSelectorImage");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the month matrix, when rendered will show days from other (previous or next)
        /// months or will render only blank cells.
        /// </summary>
        [Category("MonthView Specific Settings")]
        [DefaultValue(true)]
        [Description(
            "Gets or sets whether the month matrix, when rendered will show days from other (previous or next) months or will render only blank cells.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowOtherMonthsDays
        {
            get
            {
                if (this.showOtherMonthDays.HasValue)
                {
                    return this.showOtherMonthDays.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ShowOtherMonthsDays;
                }

                return false;
            }
            set
            {
				if (this.showOtherMonthDays != value)
                {
					this.showOtherMonthDays = value;
                    this.OnNotifyPropertyChanged("ShowOtherMonthsDays");
                }
            }
        }

        /// <summary>Gets or sets whether the fish eye functionality is enabled.</summary>
        [Description("Gets or sets whether the fish eye functionality is enabled ")]
        [DefaultValue(false)]
        [Category("Behavior")]
        [NotifyParentProperty(true)]
        public virtual bool AllowFishEye
        {
            get
            {
                if (this.allowFishEye.HasValue)
                {
                    return this.allowFishEye.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.AllowFishEye;
                }

                return false;
            }
            set
            {
                if (this.allowFishEye != value)
                {
                    this.allowFishEye = value;
                    this.OnNotifyPropertyChanged("AllowFishEye");
                }
            }
        }

        /// <summary>Gets or sets the zooming factor of a cell which is handled by the fish eye functionality.</summary>
        [Description("Gets or sets the zooming factor of a cell which is handled by the fish eye functionality")]
        [DefaultValue(1.3)]
        [Category("Behavior")]
        [NotifyParentProperty(true)]
        public virtual float ZoomFactor
        {
            get
            {
                if (this.zoomFactor.HasValue)
                {
                    return this.zoomFactor.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ZoomFactor;
                }

                return 1.3f;
            }
            set
            {
                if (this.zoomFactor != value)
                {
                    this.zoomFactor = value;
                    this.OnNotifyPropertyChanged("ZoomFactor");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the predefined pairs of rows and columns, so that the product of
        ///     the two values is exactly 42, which guarantees valid calendar layout. It is applied
        ///     on a single view level to every
        ///</para>
        ///</summary>
        [NotifyParentProperty(true)]
        [Category("Month View Settings")]
        [DefaultValue(MonthLayout.Layout_7columns_x_6rows)]
        [Description(
            "This property allows using presets, regarding the layout of the view area. Sets or gets predefined pairs of rows and columns, so that the product of the two values is exactly 42, which guarantees valid view layout.")]
        public virtual MonthLayout MonthLayout
        {
            get
            {
                if (this.monthLayout.HasValue)
                {
                    return this.monthLayout.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.MonthLayout;
                }

                return MonthLayout.Layout_7columns_x_6rows;
            }
            set
            {
                if (this.monthLayout != value)
                {
                    this.monthLayout = value;
                    this.OnNotifyPropertyChanged("MonthLayout");
                }
            }
        }

        /// <summary>
        /// The Width applied to a Header
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("The Width applied to a Header")]
        public virtual int HeaderWidth
        {
            get
            {
                if (this.headerWidth.HasValue)
                {
                    return this.headerWidth.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.HeaderWidth;
                }

                return 15;
            }
            set
            {
                if (this.headerWidth != value)
                {
                    if (value >= 15)
                    {
                        this.headerWidth = value;
                        this.OnNotifyPropertyChanged("HeaderWidth");
                    }
                }
            }
        }

        /// <summary>
        /// The Height applied to a Header
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("The Height applied to a Header")]
        public virtual int HeaderHeight
        {
            get
            {
                if (this.headerHeight.HasValue)
                {
                    return this.headerHeight.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.HeaderHeight;
                }

                return 15;
            }
            set
            {
                if (this.headerHeight != value)
                {
                    if (value >= 15)
                    {
                        this.headerHeight = value;
                        this.OnNotifyPropertyChanged("HeaderHeight");
                    }
                }
            }
        }

        /// <summary>Gets or sets whether a single CalendarView object will display a selector.</summary>
        [Category("Header Settings")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a single CalendarView object will display a selector.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowSelector
        {
            get
            {
                if (this.showSelector.HasValue)
                {
                    return this.showSelector.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ShowViewSelector;
                }
                // enforces the rule that the top view never renders a title
                //if (this.IsRootView)
                //{
                //    return false;
                //}
                return false;
            }
            set
            {
                if (this.showSelector != value)
                {
                    this.showSelector = value;
                    this.OnNotifyPropertyChanged("ShowViewSelector");
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of rows to be displayed by a multi month
        /// <a href="RadCalendar~Telerik.WebControls.Base.Calendar.CalendarView.html">CalendarView</a>.
        /// </summary>
        [Category("General View Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of month rows in a multi view calendar.")]
        public int MultiViewRows
        {
            get
            {
                if (this.multiViewRows.HasValue)
                {
                    return multiViewRows.Value;
                }
                else
                    if (this.Calendar != null)
                    {
                        return this.Calendar.MultiViewRows;
                    }

                return 1;
            }
            set
            {
                if (this.multiViewRows != value)
                {
                    this.multiViewRows = value;
                    this.OnNotifyPropertyChanged("MultiViewRows");
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of columns to be displayed by a multi month
        /// <a href="RadCalendar~Telerik.WebControls.Base.Calendar.CalendarView.html">CalendarView</a>.
        /// </summary>
        [Category("General View Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of month columns in a multi view calendar.")]
        public int MultiViewColumns
        {
            get
            {
                if (this.multiViewColumns.HasValue)
                {
                    return multiViewColumns.Value;
                }
                else
                    if (this.Calendar != null)
                    {
                        return this.Calendar.MultiViewColumns;
                    }

                return 1;
            }
            set
            {
                if (this.multiViewColumns != value)
                {
                    this.multiViewColumns = value;
                    this.OnNotifyPropertyChanged("MultiViewColumns");
                }
            }
        }

        /// <summary>Gets or sets whether a single CalendarView object will display a title row.</summary>
        [Category("Header Settings")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a single CalendarView object will display a header row.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowHeader
        {
            get
            {
                if (this.showHeader.HasValue)
                {
                    return this.showHeader.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ShowViewHeader;
                }
                // enforces the rule that the top view never renders a title
                //if (this.IsRootView)
                //{
                //    return false;
                //}
                return false;
            }
            set
            {
                if (this.showHeader != value)
                {
                    this.showHeader = value;
                    this.OnNotifyPropertyChanged("ShowViewHeader");
                }
            }
        }

        /// <summary>Gets or sets the format string used to format the text inside the header row.</summary>
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("Gets or sets the format string used to format the text inside the header row.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public virtual string TitleFormat
        {
            get
            {
                if (!string.IsNullOrEmpty(this.titleFormat))
                {
                    return this.titleFormat;
                }
                if (this.Calendar != null &&
                    !string.IsNullOrEmpty(this.Calendar.TitleFormat))
                {
                    return this.Calendar.TitleFormat;
                }
                return null;
            }
            set
            {
                if (this.titleFormat != value)
                {
                    this.titleFormat = value;
                    this.OnNotifyPropertyChanged("TitleFormat");
                }
            }
        }

        /// <summary>Gets or sets whether a CalendarView object will display a header row.</summary>
        [Category("Header Settings")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a CalendarView object will display a header row.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowColumnHeaders
        {
            get
            {
                if (this.showColumnHeaders.HasValue)
                {
                    return this.showColumnHeaders.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ShowColumnHeaders;
                }
                return false;
            }
            set
            {
                if (this.showColumnHeaders != value)
                {
                    this.showColumnHeaders = value;
                    this.OnNotifyPropertyChanged("ShowColumnHeaders");
                }
            }
        }

        /// <summary>Gets or sets whether a CalendarView object will display a header column.</summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Gets or sets whether a CalendarView object will display a header column.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowRowHeaders
        {
            get
            {
                if (this.showRowHeaders.HasValue)
                {
                    return this.showRowHeaders.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.ShowRowHeaders;
                }
                return false;
            }
            set
            {
                if (this.showRowHeaders != value)
                {
                    this.showRowHeaders = value;
                    this.OnNotifyPropertyChanged("ShowRowHeaders");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether row headers ( if displayed by a <strong>MonthView</strong> object)
        /// will act as row selectors.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description(
           "Gets or sets whether row headers ( if displayed by a MonthView object) will act as row selectors.")]
        [NotifyParentProperty(true)]
        public virtual bool AllowRowHeaderSelectors
        {
            get
            {
                if (this.allowRowHeaderSelectors.HasValue)
                {
                    return this.allowRowHeaderSelectors.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.AllowRowHeaderSelectors;
                }
                return false;
            }
            set
            {
                if (this.allowRowHeaderSelectors != value)
                {
                    this.allowRowHeaderSelectors = value;
                    this.OnNotifyPropertyChanged("AllowRowHeaderSelectors");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether column headers ( if displayed by a <strong>MonthView</strong> object)
        /// will act as column selectors.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description(
         "Gets or sets whether column headers ( if displayed by a MonthView object) will act as column selectors.")]
        [NotifyParentProperty(true)]
        public virtual bool AllowColumnHeaderSelectors
        {
            get
            {
                if (this.allowColumnHeaderSelectors.HasValue)
                {
                    return this.allowColumnHeaderSelectors.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.AllowColumnHeaderSelectors;
                }
                return false;
            }
            set
            {
                if (this.allowColumnHeaderSelectors != value)
                {
                    this.allowColumnHeaderSelectors = value;
                    this.OnNotifyPropertyChanged("AllowColumnHeaderSelectors");
                }
            }
        }

        /// <summary> 
        /// Gets or sets whether a selector for the entire <strong>CalendarView</strong> (
        /// <strong>MonthView</strong> ) will appear on the calendar.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description(
            "Gets or sets whether a selector for the entire CalendarView (MonthView) will appear on the calendar.")]
        [NotifyParentProperty(true)]
        public virtual bool AllowViewSelector
        {
            get
            {
                if (this.allowViewSelector.HasValue)
                {
                    return this.allowViewSelector.Value;
                }
                else if (this.Calendar != null)
                {
                    return this.Calendar.AllowViewSelector;
                }
                return false;
            }
            set
            {
                if (this.allowViewSelector != value)
                {
                    this.allowViewSelector = value;
                    this.OnNotifyPropertyChanged("AllowViewSelector");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CalendarView has child views.
        /// </summary>
        public virtual bool IsMultipleView
        {
            get
            {
                return this.isMultiView;
            }
        }

        /// <summary>
        /// Gets the DateTime object that is the first date to be rendered by CalendarView.
        /// While ViewStartDate is the start date that is handled by a particular CalendarView instance,
        /// the ViewRenderStartDate might belong to a different (previous) CalendarView object.
        /// </summary>
        internal protected virtual DateTime ViewRenderStartDate
        {
            get
            {
                return this.viewRenderStartDate;
            }
            set
            {
                if (this.viewRenderStartDate != value)
                {
                    this.viewRenderStartDate = value;
                    this.OnNotifyPropertyChanged("ViewRenderStartDate");
                    //this.DirtyLayout = true;
                }
            }
        }

        /// <summary>
        /// Gets the DateTime object that is the last date to be rendered by CalendarView.
        /// While ViewEndDate is the start date that is handled by a particular CalendarView instance,
        /// the ViewRenderEndDate might belong to a different (next) CalendarView object.
        /// </summary>
        internal protected virtual DateTime ViewRenderEndDate
        {
            get
            {
                return this.viewRenderEndDate;
            }
            set
            {
                if (this.viewRenderEndDate != value)
                {
                    this.viewRenderEndDate = value;
                    this.OnNotifyPropertyChanged("ViewRenderEndDate");
                    //this.DirtyLayout = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a DateTime value specifying the starting date for the period handled by a CalendarView instance.
        /// </summary>
        [Category("Data")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(DateTime), "1/1/1980")]
        [Description("Gets or sets a DateTime value specifying the starting date for the period handled by a CalendarView instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DateTime ViewStartDate
        {
            get
            {
                return this.viewStartDate;
            }
            set
            {
                if (this.viewStartDate != value)
                {
                    this.viewStartDate = value;
                    this.OnNotifyPropertyChanged("ViewStartDate");
                    //this.DirtyLayout = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a DateTime value specifying the ending date for the period handled by a CalendarView instance.
        /// </summary>
        [Category("Data")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(DateTime), "2/1/1980")]
        [Description("Gets or sets a DateTime value specifying the ending date for the period handled by a CalendarView instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DateTime ViewEndDate
        {
            get
            {
                return this.viewEndDate;
            }
            set
            {
                if (this.viewEndDate != value)
                {
                    this.viewEndDate = value;
                    this.OnNotifyPropertyChanged("ViewEndDate");
                    //this.DirtyLayout = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size and location of the tree node in pixels, relative to the parent layout.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
            set
            {
                if (this.bounds != value)
                {
                    this.bounds = value;
                    this.OnNotifyPropertyChanged("Bounds");
                    //this.DirtyLayout = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of rows to be displayed by a CalendarView.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The the count of rows to be displayed by a CalendarView")]
        public int Rows
        {
            get
            {		

                return this.rows;
            }
			set
			{
				this.rows = value;
			}
        }

        /// <summary>
        /// Gets or sets the the count of columns to be displayed by a CalendarView.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The the count of columns to be displayed by a CalendarView")]
        public int Columns
        {
            get
            {
                return this.columns;
            }
			set
			{
				this.columns = value;
			}
        }

        /// <summary>
        /// Gets the previous available view. Used for traversal of the calendar.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CalendarView PreviousView
        {
            get
            {
                CalendarViewCollection parentCollection = null;
                if (this.Parent != null)
                {
                    parentCollection = this.Parent.Children;
                }
                //else if (this.Calendar != null)
                //{
                //    parentCollection = this.Calendar.Children;
                //}

                if (parentCollection != null)
                {
                    int prevIndex = parentCollection.IndexOf(this) - 1;
                    if (prevIndex > -1)
                    {
                        return parentCollection[prevIndex];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the next available view. Used for traversal of the calendar.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CalendarView NextView
        {
            get
            {
                CalendarViewCollection parentCollection = null;
                if (this.Parent != null)
                {
                    parentCollection = this.Parent.Children;
                }
                //else if (this.Calendar != null)
                //{
                //    parentCollection = this.Calendar.Children;
                //}

                if (parentCollection != null)
                {
                    int nextIndex = parentCollection.IndexOf(this) + 1;
                    if (nextIndex < parentCollection.Count)
                    {
                        return parentCollection[nextIndex];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the default <strong>System.Globalization.Calendar</strong> instance as
        /// specified by the default culture.
        /// </summary>
        /// <remarks>
        /// 	<para>A calendar divides time into measures, such as weeks, months, and years. The
        ///     number, length, and start of the divisions vary in each calendar.</para>
        /// 	<para>Any moment in time can be represented as a set of numeric values using a
        ///     particular calendar. For example, the last vernal equinox occurred at (0.0, 0, 46,
        ///     8, 20, 3, 1999) in the Gregorian calendar. An implementation of <b>Calendar</b> can
        ///     map any <strong>DateTime</strong> value to a similar set of numeric values, and
        ///     <b>DateTime</b> can map such sets of numeric values to a textual representation
        ///     using information from <b>Calendar</b> and <strong>DateTimeFormatInfo</strong>. The
        ///     textual representation can be culture-sensitive (for example, "8:46 AM March 20th
        ///     1999 AD" for the en-US culture) or culture-insensitive (for example,
        ///     "1999-03-20T08:46:00" in ISO 8601 format).</para>
        /// 	<para>A <b>Calendar</b> implementation can define one or more eras. The
        ///     <b>Calendar</b> class identifies the eras as enumerated integers where the current
        ///     era (<strong>CurrentEra</strong>) has the value 0.</para>
        /// 	<para>In order to make up for the difference between the calendar year and the
        ///     actual time that the earth rotates around the sun or the actual time that the moon
        ///     rotates around the earth, a leap year has a different number of days than a
        ///     standard calendar year. Each <b>Calendar</b> implementation defines leap years
        ///     differently.</para>
        /// 	<para>For consistency, the first unit in each interval (for example, the first
        ///     month) is assigned the value 1.</para>
        /// 	<para>The <strong>System.Globalization</strong> namespace includes the following
        ///     <b>Calendar</b> implementations: <strong>GregorianCalendar</strong>,
        ///     <strong>HebrewCalendar</strong>, <strong>HijriCalendar</strong>,
        ///     <strong>JapaneseCalendar</strong>, <strong>JulianCalendar</strong>,
        ///     <strong>KoreanCalendar</strong>, <strong>TaiwanCalendar</strong>, and
        ///     <strong>ThaiBuddhistCalendar</strong>.</para>
        /// </remarks>
        [Browsable(false)]
        [Category("Localization Settings")]
        [Description("Gets the default System.Globalization.Calendar instance as speified by the default culture.")]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Globalization.Calendar CurrentCalendar
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.CurrentCalendar;
                }
                else if (this.Calendar != null &&
                    this.Calendar.DateTimeFormat.Calendar != null)
                {
                    return this.Calendar.DateTimeFormat.Calendar;
                }
                return DateTimeFormatInfo.CurrentInfo.Calendar;
            }
        }

        /// <summary>
        /// Gets or sets the vertical spacing between the calendar cells
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the vertical spacing between the calendar cells")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(1)]
        public int CellVerticalSpacing
        {
            get
            {
                if (this.cellVerticalSpacing.HasValue)
                {
                    return this.cellVerticalSpacing.Value;
                }
                else
                    if (this.Calendar != null)
                    {
                        return this.Calendar.CellVerticalSpacing;
                    }

                return 1;
            }
            set
            {
                if (this.cellVerticalSpacing != value)
                {
                    this.cellVerticalSpacing = value;
                    this.OnNotifyPropertyChanged("CellVerticalSpacing");
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal spacing between the calendar cells
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or sets the horizontal spacing between the calendar cells")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(1)]
        public int CellHorizontalSpacing
        {
            get
            {
                if (this.cellHorizontalSpacing.HasValue)
                {
                    return this.cellHorizontalSpacing.Value;
                }
                else
                    if (this.Calendar != null)
                    {
                        return this.Calendar.CellHorizontalSpacing;
                    }

                return 1;
            }
            set
            {
                if (this.cellHorizontalSpacing != value)
                {
                    this.cellHorizontalSpacing = value;
                    this.OnNotifyPropertyChanged("CellHorizontalSpacing");
                }
            }
        } 
        #endregion

        /// <summary>
        /// Verifies CalendarView settings required for correct presentation of calendrical information.
        /// </summary>
        internal protected virtual void EnsureRenderSettings()
        {
        }

        /// <summary>
        /// Determines if a DateTime object belongs to the dates range managed by a particular CalendarView. 
        /// </summary>
        /// <param name="date">The DateTime object to be tested.</param>
        /// <returns>True if the DateTime object belongs to the dates range managed by a particular CalendarView; False otherwise.</returns>
        public bool IsDateInView(DateTime date)
        {
            return (date >= this.ViewStartDate) && (this.ViewEndDate >= date);
        }

        /// <summary>
        /// Gets a DateTime object that is part of the date range handled by the previous calendar view.
        /// Used for traversal of the calendar.
        /// </summary>
        /// <returns>The DateTime object</returns>
        protected virtual DateTime GetPreviousViewDate()
        {
            return this.AddViewPeriods(this.ViewStartDate, -1);
        }

        /// <summary>
        /// Gets a DateTime object that is part of the date range handled by the next calendar view.
        /// Used for traversal of the calendar.
        /// </summary>
        /// <returns>The DateTime object</returns>
        protected virtual DateTime GetNextViewDate()
        {
            return this.AddViewPeriods(this.ViewStartDate, 1);
        }

        internal virtual CalendarView GetPreviousView()
        {
            return CreateView(this.GetPreviousViewDate());
        }

        internal virtual CalendarView GetNextView()
        {
            return CreateView(this.GetNextViewDate());
        }

        internal virtual CalendarView GetPreviousView(int months)
        {
            return null;
        }
        
        internal virtual CalendarView GetNextView(int months)
        {
            return null;
        }

        public void Dispose()
        {
            this.calendar = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="periods"></param>
        /// <returns></returns>
        protected virtual DateTime AddViewPeriods(DateTime startDate, int periods)
        {
            return DateTime.MinValue;
        }

        /// <summary>
        /// Ensures that the child views collection is created.
        /// </summary>
        protected virtual void EnsureChildViews()
        {
            if (this.children == null)
            {
                this.children = new CalendarViewCollection(this);
            }
        }

        /// <summary>
        /// Initializes properly the ViewStartDate, ViewEndDate, ViewRenderStartDate, ViewRenderEndDate properties
        /// </summary>
        protected virtual void SetDateRange()
        {
        }

        /// <summary>
        /// Returns the DateTime object that is used by the CalendarView to initialize.
        /// </summary>
        /// <returns>DateTime object that is used by the CalendarView to initialize.</returns>
        internal protected virtual DateTime EffectiveVisibleDate()
        {
            if (this.Calendar.Site != null && this.Calendar.Site.DesignMode && (this.Calendar.FocusedDate == (new DateTime(1980, 1, 1))))
            {
                return DateTime.Today;
            }
            return this.Calendar.FocusedDate;
        }

        /// <summary>
        /// handles key events that require processing from CalendarView.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        internal protected virtual bool HandleKeyDown(Keys keys)
        {
            //Rectangle rectangle1 =
            //    new Rectangle(((int)this.ClientRectangle.X) - this.ScrollPosition.X, this.ClientRectangle.Y - this.ScrollPosition.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            //RadTreeNode nextVisible = null;
            //RadTreeNode selectedNode = this.SelectedNode;
            //bool flag1 = ((((e.KeyCode >= Keys.A) && (e.KeyCode <= Keys.Z)) || e.KeyCode == Keys.Space) ||
            //    ((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9))) && ((e.Modifiers == Keys.Shift) ||
            //    (e.Modifiers == Keys.None));

            //if (((e.Modifiers == Keys.Control) && ((e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End)))
            //    || ((e.Modifiers == Keys.None) || flag1))
            //{

                if (this.Calendar == null)
                    return false;
               
                if ((keys & Keys.Control) == Keys.Control)
                {
                    if ((keys & Keys.Right) == Keys.Right)
                    {
                        this.HandleRightKey(keys);
                        return true;
                    }
                    if ((keys & Keys.Left) == Keys.Left)
                    {
                        this.HandleLeftKey(keys);
                        return true;
                    }
                }

                switch (keys)
                {
                    // PgUp behavior
                    case Keys.Prior:
                        this.HandlePageUpKey(keys);
                        //data.Handled = true;
                        return true;
                    //PgDown behavior
                    case Keys.Next:
                        this.HandlePageDownKey(keys);
                        //data.Handled = true;
                        return true;
                    //End behavior
                    case Keys.End:
                        this.HandleEndKey(keys);
                        //data.Handled = true;
                        return true;
                    //Home behavior
                    case Keys.Home:
                        this.HandleHomeKey(keys);
                        //data.Handled = true;
                        return true;
                    case Keys.Left:
                        this.HandleLeftKey(keys);
                        return true;
                    case Keys.Right:
                        this.HandleRightKey(keys);
                        //data.Handled = true;
                        return true;
                    case Keys.Up:
                        this.HandleUpKey(keys);
                        //data.Handled = true;
                        return true;
                    case Keys.Down:
                        this.HandleDownKey(keys);
                        //data.Handled = true;
                        return true;
                    case Keys.Return:
                    case Keys.Space:
                        this.ToggleSelection(keys);
                        //data.Handled = true;
                        return true;
                    default:
                        break;
                }
            //}
            return false;
        }

        /// <summary>
        /// handles the page down key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandlePageDownKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the page up key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandlePageUpKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the down arrow key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleDownKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the up arrow key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleUpKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the End key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleEndKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the Home key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleHomeKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the left arrow key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleLeftKey(Keys keys)
        {
        }

        /// <summary>
        /// handles the right arrow key.
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void HandleRightKey(Keys keys)
        {
        }

        /// <summary>
        /// Toogles the date selection (Enter key).
        /// </summary>
        /// <param name="keys">The key data to be processed.</param>
        protected virtual void ToggleSelection(Keys keys)
        {
        }

        /// <summary>
        /// Creates a CalendarView object based on the logic implemented by the CalendarView instance 
        /// that implements the method.
        /// </summary>
        /// <param name="date">DateTime object that is used to create the CalendarView.</param>
        /// <returns>The created CalendarView object.</returns>
        internal protected virtual CalendarView CreateView(DateTime date) 
        {
            return null;
        }

        internal protected virtual CalendarView CreateView()
        {
            return null;
        }

        internal /*protected virtual*/ void GetViewRowsAndColumns(out int rows, out int columns)
        {
            int offsetRows = 0;
            int offsetColumns = 0;
            int contentRows = 0;
            int contentColumns = 0;
            this.GetContentOffset(out offsetRows, out offsetColumns);
            this.GetContentRowsAndColumns(out contentRows, out contentColumns);
			rows = contentRows + offsetColumns;
			columns = contentColumns + offsetRows;	
        }

        internal  virtual void GetContentRowsAndColumns(out int rows, out int columns)
        {
            rows = this.Rows;
            columns = this.Columns;
        }

        internal virtual void GetContentOffset(out int xShift, out int yShift)
        {
            int rows = 0;
            int columns = 0;
            if (this.ShowSelector)
            {
                columns += 1;
                rows += 1;
            }
            else
            {
				if (this.ShowRowHeaders)
                {
                    rows += 1;
                }

				if (this.ShowColumnHeaders)
                {
                    columns += 1;
                }
            }
            xShift = rows;
            yShift = columns;
        }

        /// <summary>
        /// Adds the specified date to the SelectedDates collection of RadCalendar.
        /// </summary>
        /// <param name="date"> The DateTime object to add.</param>
        public virtual void Select(DateTime date)
        {
            if (this.Calendar != null &&
                !this.Calendar.SelectedDates.Contains(date))
            {
                this.Calendar.SelectedDates.Add(date);
            }
        }

        /// <summary>
        /// Adds the specified range of dates to the SelectedDates collection of RadCalendar.
        /// </summary>
        /// <param name="dates">array of DateTime objects to add.</param>
        public virtual void SelectRange(DateTime[] dates)
        {
            for (int i = 0; i < dates.Length; i++)
            {
                this.Select(dates[i]);
            }
        }

        /// <summary>
        /// Adds the specified range of dates to the SelectedDates collection of RadCalendar.
        /// </summary>
        /// <param name="startDate">A System.DateTime that specifies the initial date to add to the SelectedDates collection.</param>
        /// <param name="endDate">A System.DateTime that specifies the end date to add to the SelectedDates collection.</param>
        public virtual void SelectRange(DateTime startDate, DateTime endDate)
        {
            DateTime counter = startDate;
            while (counter < endDate)
            {
                this.Select(counter);
                counter.AddDays(1);
            }
        }

        internal void SetRange(DateTime beginDate, DateTime endDate)
        {
            this.currentViewBeginDate = beginDate;
            this.currentViewEndDate = endDate;
        }

        internal virtual void Initialize()
        {
            //if (this.IsRootView && !this.IsMultipleView)
            //{
            //    this.ShowColumnHeaders = this.Parent.ShowColumnHeaders;
            //    this.ShowRowHeaders = this.Parent.ShowRowHeaders;
            //    this.AllowViewSelector = this.Parent.AllowViewSelector;
            //}
            //else
            //{
            //    this.ShowViewHeader = true;
            //}
            //this.Orientation = this.Parent.Orientation;
        }

        internal virtual void Initialize(CalendarView view)
        {
            //if (this.IsRootView && !this.IsMultipleView)
            //{
            //    this.ShowColumnHeaders = this.Parent.ShowColumnHeaders;
            //    this.ShowRowHeaders = this.Parent.ShowRowHeaders;
            //    this.AllowViewSelector = this.Parent.AllowViewSelector;
            //}
            //else
            //{
            //    this.ShowViewHeader = true;
            //}
            //this.Orientation = this.Parent.Orientation;
        }

        internal virtual void ReInitialize()
        {
            if (this.Calendar != null)
            {
				this.rows = this.Calendar.Rows;
				this.columns = this.Calendar.Columns;	
			}
        }

        internal abstract string GetTitleContent();

        internal virtual void Remove()
        {
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when when a property of an object changes change. 
        /// Calling the event is developer's responsibility.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

		private void Calendar_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Columns")
			{
				this.columns = this.calendar.Columns;
			}
            else if (e.PropertyName == "Rows")
			{
				this.rows = this.calendar.Rows;
			}

			this.OnNotifyPropertyChanged(e.PropertyName);
		} 

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    // properties that require additional handling special properties
            //    case "Selected":
            //        break;
            //    default:
            //        break;
            //}
	

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #endregion

    }
}


