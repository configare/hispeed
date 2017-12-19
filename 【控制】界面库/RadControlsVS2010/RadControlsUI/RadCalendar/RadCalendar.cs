using System;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The RadCalendar main class.
    /// </summary>
    //[ToolboxBitmap(typeof(RadCalendar), "CalendarResources.RadCalendar.bmp")]
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [RadThemeDesignerData(typeof(RadCalendarDesignTimeData))]
    [DefaultProperty("SelectedDate"), DefaultEvent("SelectionChanged")]
    [ToolboxItem(true)]
    [Description("Enables the user to select a date from a highly customizable calendar")]
    public class RadCalendar : RadControl
    {
        #region Fields
        private RadCalendarElement calendarElement;

        private System.Drawing.ContentAlignment cellAlign = ContentAlignment.MiddleCenter;
        private CultureInfo cultureInfo = null;
        private static readonly object dayRenderEventKey;
        private static readonly object headerCellRenderEventKey;
        private static readonly object selectionChangedEventKey;
        private static readonly object viewChangedEventKey;
        private static readonly object selectionChangingEventKey;
        private static readonly object viewChangingEventKey;
        private DayNameFormat dayNameFormat = DayNameFormat.FirstLetter;
        private string cellDayFormat;
        private int? singleViewRows;
        private int? singleViewColumns;
        private int? headerHeight;
        private bool rightToLeft;
        private int? headerWidth;
        private MonthLayout? monthLayout;
        private bool allowToolTips = true;
        private int multiViewRows = 1;
        private int multiViewColumns = 1;
        private DateTime? rangeMaxDate;
        private DateTime? rangeMinDate;
        private FirstDayOfWeek? firstDayOfWeek;
        private DateTime? focusedDate;
        private bool readOnly;
        private DateTimeCollection selectedDates;
        private bool? enableNavigation;
        private bool showNavigationButtons = true;
        private bool showFastNavigationButtons = true;
        private bool showFooter = false;
        private string navigationPrevText;
        private string navigationNextText;
        private string fastNavigationPrevText;
        private string fastNavigationNextText;
        private string navigationPrevToolTip;
        private string navigationNextToolTip;
        private string fastNavigationPrevToolTip;
        private string fastNavigationNextToolTip;
        private string titleFormat;
        private string cellToolTipFormat;
        private string dateRangeSeparator;
        private bool showColumnHeaders = true;
        private bool showRowHeaders = false;
        private bool allowViewSelector;
        private bool showOtherMonthsDays = true;
        private string rowHeaderText = "";
        private Image rowHeaderImage = null;
        private string columnHeaderText = "";
        private Image columnHeaderImage;
        private string viewSelectorText = "x";
        private Image viewSelectorImage = null;
        private Orientation orientation = Orientation.Horizontal;
        private int fastNavigationStep = 3;
        private CalendarDayCollection specialDays;
        private bool allowMultiSelect = false;
        private bool allowMultiView = false;
        private CalendarView calendarView = null;
        private bool allowRowHeaderSelectors = false;
        private bool allowColumnHeaderSelectors = false;
        private bool showHeader = true;
        private bool showViewSelector = false;
        private int currentViewColumn = 0;
        private int currentViewRow = 0;
        private bool showViewHeader;
        private bool allowSelect = true;
        private Padding cellPadding = Padding.Empty;
        private Padding cellMargin = Padding.Empty;
        private int cellVerticalSpacing = 1;
        private int cellHorizontalSpacing = 1;
        private bool? allowFastNavigation;

        private bool updating = false;
        #endregion

        #region Constructors

        static RadCalendar()
        {
            dayRenderEventKey = new object();
            headerCellRenderEventKey = new object();
            selectionChangedEventKey = new object();
            viewChangedEventKey = new object();
            selectionChangingEventKey = new object();
            viewChangingEventKey = new object();

        }

        public RadCalendar()
        {
            this.AutoSize = true;
            this.CellAlign = ContentAlignment.MiddleCenter;
	
        }

        #endregion

        #region Properties
        protected override Size DefaultSize
        {
            get
            {
                return new Size(257, 227);
            }
        }

        /// <summary>Gets or set a value indicating whether automatic sizing is turned on.</summary>
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the row in the multi-view table where the focused date is positioned.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(0)]
        [Description("The row in the multi-view table where the focused date is positioned.")]
        public int CurrentViewRow
        {
            get
            {
                return this.currentViewRow;
            }
            set
            {
                if (this.currentViewRow != value)
                {
                    this.currentViewRow = value;


                    if (this.multiViewColumns > 1 || this.multiViewRows > 1)
                    {
                        this.calendarView = new MultiMonthView(this);
                    }
                    else
                    {
                        this.calendarView = new MonthView(this);
                    }

                    this.calendarView.Initialize();
                    ReInitializeCalendarElement();

                    this.OnNotifyPropertyChanged("CurrentViewRow");
                }
            }
        }

        /// <summary>
        /// The column in the multi-view table where the focused date is positioned.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(0)]
        [Description("The column in the multi-view table where the focused date is positioned.")]
        public int CurrentViewColumn
        {
            get
            {
                return currentViewColumn;
            }
            set
            {
                if (this.currentViewColumn != value)
                {
                    this.currentViewColumn = value;

                    if (this.multiViewColumns > 1 || this.multiViewRows > 1)
                    {
                        this.calendarView = new MultiMonthView(this);
                    }
                    else
                    {
                        this.calendarView = new MonthView(this);
                    }

                    this.calendarView.Initialize();
                    ReInitializeCalendarElement();

                    this.OnNotifyPropertyChanged("CurrentViewRowColumn");
                }
            }
        }

        /// <summary>
        /// Gets the instance of RadCalendarElement wrapped by this control. RadCalendarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadCalendar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadCalendarElement CalendarElement
        {
            get
            {
                return this.calendarElement;
            }
        }

        [Browsable(false)]
        public new Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// <em>SlectionChanged</em> event is fired when a new date is added or removed from
        /// the SelectedDates collection.
        /// </summary>
        public event SelectionEventHandler SelectionChanging
        {
            add
            {
                base.Events.AddHandler(selectionChangingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(selectionChangingEventKey, value);
            }
        }

        internal SelectionEventArgs CallOnSelectionChanging(DateTimeCollection dates)
        {
            return this.OnSelectionChanging(dates);
        }

        /// <summary>
        /// Raises the SelectionChanging event.
        /// </summary>
        /// <param name="dates">A DateTimeCollection collection used by SelectionEventArgs.</param>
        /// <returns>SelectionEventArgs instance.</returns>
        protected virtual SelectionEventArgs OnSelectionChanging(DateTimeCollection dates)
        {
            //		InvalidateCalendar();
            //	InvalidateCalendarSelection();


            SelectionEventHandler handler = (SelectionEventHandler)base.Events[selectionChangingEventKey];
            SelectionEventArgs args = new SelectionEventArgs(dates);
            if (handler != null)
            {
                handler(this, args);
            }
            return args;
        }

        /// <summary>
        /// <em>SlectionChanged</em> event is fired when a new date is added or removed from the
        /// SelectedDates collection.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { base.Events.AddHandler(selectionChangedEventKey, value); }
            remove { base.Events.RemoveHandler(selectionChangedEventKey, value); }
        }

        internal void CallOnSelectionChanged()
        {
            this.OnSelectionChanged();
        }

        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            //	InvalidateCalendar();
            InvalidateCalendarSelection();

            EventHandler handler = (EventHandler)base.Events[selectionChangedEventKey];
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// <em>ElementRender</em> event is fired after the generation of every calendar cell
        /// object and just before it gets rendered. It is the last place where
        /// changes to the already constructed calendar cells can be made.
        /// </summary>
        public event RenderElementEventHandler ElementRender
        {
            add { base.Events.AddHandler(dayRenderEventKey, value); }
            remove { base.Events.RemoveHandler(dayRenderEventKey, value); }
        }

        internal void CallOnElementRender(LightVisualElement cell, RadCalendarDay day, CalendarView currentView)
        {
            this.OnElementRender(cell, day, currentView);
        }

        /// <summary>
        /// Raises the <em>ElementRender</em> event of the RadCalendar control and allows you to provide a custom 
        /// handler for the <em>ElementRender</em> event. 
        /// </summary>
        /// <param name="cell">A LightVisualElement object that contains information about the cell to render.</param>
        /// <param name="day">A RadCalendarDay that contains information about the day to render.</param>
        /// <param name="view">A CalendarView that contains the day to render.</param>
        protected virtual void OnElementRender(LightVisualElement cell, RadCalendarDay day, CalendarView view)
        {
            RenderElementEventHandler handler1 = (RenderElementEventHandler)base.Events[dayRenderEventKey];
            if (handler1 != null)
            {
                handler1(this, new RenderElementEventArgs(cell, day, view));
            }
        }

        /// <summary>
        /// 	<em>ViewChanging</em> event is fired when a navigation to a different date range is required.
        /// </summary>
        public event ViewChangingEventHandler ViewChanging
        {
            add
            {
                base.Events.AddHandler(viewChangingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(viewChangingEventKey, value);
            }
        }

        internal ViewChangingEventArgs CallOnViewChanging(CalendarView view)
        {
            return this.OnViewChanging(view);
        }

        /// <summary>
        /// Raises the ViewChanging event.
        /// </summary>
        /// <param name="view">A CalendarView collection used by ViewChangingEventArgs.</param>
        /// <returns>ViewChangingEventArgs instance.</returns>
        protected virtual ViewChangingEventArgs OnViewChanging(CalendarView view)
        {
            ViewChangingEventHandler handler1 = (ViewChangingEventHandler)base.Events[viewChangingEventKey];
            ViewChangingEventArgs args = new ViewChangingEventArgs(view);
            if (handler1 != null)
            {
                handler1(this, args);
            }
            return args;
        }

        /// <summary>
        /// 	<em>ViewChanged</em> event is fired when a navigation to a different date
        /// range occurred. Generally this is done by using the normal navigation buttons.
        /// </summary>
        public event EventHandler ViewChanged
        {
            add
            {
                base.Events.AddHandler(viewChangedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(viewChangedEventKey, value);
            }
        }

        internal void CallOnViewChanged()
        {
            this.OnViewChanged();
        }

        /// <summary>
        /// Raises the ViewChanged event.
        /// </summary>
        protected virtual void OnViewChanged()
        {
            EventHandler handler1 = (EventHandler)base.Events[viewChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, new EventArgs());
            }
        }

        #endregion

        #region Theming

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadButtonElement))
            {
                return true;
            }

            if (elementType == typeof(RadRepeatButtonElement))
            {
                return true;
            }

            if (elementType == typeof(RadCalendarElement))
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion

        #region Localization

        /// <summary>
        /// Specifies the display formats for the days of the week used as selectors by <strong>RadCalendar</strong>.
        /// </summary>
        /// <remarks>
        /// 	<para>Use the <b>DayNameFormat</b> property to specify the name format for the days
        ///     of the week. This property is set with one of the <strong>DayNameFormat</strong>
        ///     enumeration values. You can specify whether the days of the week are displayed as
        ///     the full name, short (abbreviated) name, first letter of the day, or first two
        ///     letters of the day.</para>
        /// 	<para>The <b>DayNameFormat</b> enumeration represents the display formats for the
        ///     days of the week used as selectors by <strong>RadCalendar</strong>.</para>
        /// 	<list type="table">
        /// 		<listheader>
        /// 			<term>Member name</term>
        /// 			<description>Description</description>
        /// 		</listheader>
        /// 		<item>
        /// 			<term><b>FirstLetter</b></term>
        /// 			<description>The days of the week displayed with just the first letter. For
        ///             example, <strong>T</strong>.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>FirstTwoLetters</b></term>
        /// 			<description>The days of the week displayed with just the first two
        ///             letters. For example, <strong>Tu</strong>.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Full</b></term>
        /// 			<description>The days of the week displayed in full format. For example,
        ///             <strong>Tuesday</strong>.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Short</b></term>
        /// 			<description>The days of the week displayed in abbreviated format. For
        ///             example, <strong>Tues</strong>.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Shortest</b></term>
        /// 			<description>The shortest unique abbreviated day names associated with the current DateTimeFormatInfo 
        ///             object.</description>
        /// 		</item>
        /// 	</list>
        /// </remarks>
        [Category("Localization Settings")]
        [DefaultValue(DayNameFormat.FirstLetter)]
        [Description("Specifies the display format for the days of the week on RadCalendar.")]
        [NotifyParentProperty(true)]
        public DayNameFormat DayNameFormat
        {
            get
            {
                if (this.dayNameFormat != DayNameFormat.FirstLetter)
                {
                    return this.dayNameFormat;
                }
                return DayNameFormat.FirstLetter;
            }
            set
            {
                if (this.dayNameFormat != value)
                {
                    this.dayNameFormat = value;
                    this.OnNotifyPropertyChanged("DayNameFormat");
                }
            }
        }

        /// <summary>
        /// Gets or sets a <strong>DateTimeFormatInfo</strong> instance that defines the
        /// culturally appropriate format of displaying dates and times as specified by the default
        /// culture.
        /// </summary>
        /// <remarks>
        /// 	<para>A <strong>DateTimeFormatInfo</strong> can be created only for the invariant
        ///     culture or for specific cultures, not for neutral cultures.</para>
        /// 	<para>The cultures are generally grouped into three sets: the invariant culture,
        ///     the neutral cultures, and the specific cultures.</para>
        /// 	<para>The invariant culture is culture-insensitive. You can specify the invariant
        ///     culture by name using an empty string ("") or by its culture identifier 0x007F.
        ///     <strong>InvariantCulture</strong> retrieves an instance of the invariant culture.
        ///     It is associated with the English language but not with any country/region. It can
        ///     be used in almost any method in the Globalization namespace that requires a
        ///     culture. If a security decision depends on a string comparison or a case-change
        ///     operation, use the <b>InvariantCulture</b> to ensure that the behavior will be
        ///     consistent regardless of the culture settings of the system. However, the invariant
        ///     culture must be used only by processes that require culture-independent results,
        ///     such as system services; otherwise, it produces results that might be
        ///     linguistically incorrect or culturally inappropriate.</para>
        /// 	<para>A neutral culture is a culture that is associated with a language but not
        ///     with a country/region. A specific culture is a culture that is associated with a
        ///     language and a country/region. For example, "fr" is a neutral culture and "fr-FR"
        ///     is a specific culture. Note that "zh-CHS" (Simplified Chinese) and "zh-CHT"
        ///     (Traditional Chinese) are neutral cultures.</para>
        /// 	<para>The user might choose to override some of the values associated with the
        ///     current culture of Windows through Regional and Language Options (or Regional
        ///     Options or Regional Settings) in Control Panel. For example, the user might choose
        ///     to display the date in a different format or to use a currency other than the
        ///     default for the culture.</para>
        /// 	<para>If <strong>UseUserOverride</strong> is <b>true</b> and the specified culture
        ///     matches the current culture of Windows, the <strong>CultureInfo</strong> uses those
        ///     overrides, including user settings for the properties of the
        ///     <b>DateTimeFormatInfo</b> instance returned by the <b>DateTimeFormat</b> property,
        ///     the properties of the <strong>NumberFormatInfo</strong> instance returned by the
        ///     <strong>NumberFormat</strong> property, and the properties of the
        ///     <strong>CompareInfo</strong> instance returned by the <strong>CompareInfo</strong>
        ///     property. If the user settings are incompatible with the culture associated with
        ///     the <b>CultureInfo</b> (for example, if the selected calendar is not one of the
        ///     <strong>OptionalCalendars</strong> ), the results of the methods and the values of
        ///     the properties are undefined.<br/>
        /// 		<br/>
        /// 		<strong>Note:</strong> In this version of <strong>RadCalendar</strong> the
        ///     <strong>NumberFormatInfo</strong> instance returned by the
        ///     <strong>NumberFormat</strong> property is not taken into account.</para>
        /// </remarks>
        [Browsable(false)]
        [Category("Localization Settings")]
        [Description("Gets the default DateTimeFormatInfo instance as specified by the default culture.")]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                return this.Culture.DateTimeFormat;
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the <strong>CultureInfo</strong> supported by this <strong>RadCalendar</strong> object.</para>
        /// 	<para>Describes the names of the culture, the writing system, and
        ///     the calendar used, as well as access to culture-specific objects that provide
        ///     methods for common operations, such as formatting dates and sorting strings.</para>
        /// </summary>
        /// <remarks>
        /// 	<para>The culture names follow the RFC 1766 standard in the format
        ///     "&lt;languagecode2&gt;-&lt;country/regioncode2&gt;", where &lt;languagecode2&gt; is
        ///     a lowercase two-letter code derived from ISO 639-1 and &lt;country/regioncode2&gt;
        ///     is an uppercase two-letter code derived from ISO 3166. For example, U.S. English is
        ///     "en-US". In cases where a two-letter language code is not available, the
        ///     three-letter code derived from ISO 639-2 is used; for example, the three-letter
        ///     code "div" is used for cultures that use the Dhivehi language. Some culture names
        ///     have suffixes that specify the script; for example, "-Cyrl" specifies the Cyrillic
        ///     script, "-Latn" specifies the Latin script.</para>
        /// 	<para>The following predefined <b>CultureInfo</b> names and identifiers are
        ///     accepted and used by this class and other classes in the System.Globalization
        ///     namespace.</para>
        /// 	<table cellspacing="0">
        /// 		<tbody>
        /// 			<tr valign="top">
        /// 				<th width="32%">Culture Name</th>
        /// 				<th width="34%">Culture Identifier</th>
        /// 				<th width="34%">Language-Country/Region</th>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">"" (empty string)</td>
        /// 				<td width="34%">0x007F</td>
        /// 				<td width="34%">invariant culture</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">af</td>
        /// 				<td width="34%">0x0036</td>
        /// 				<td width="34%">Afrikaans</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">af-ZA</td>
        /// 				<td width="34%">0x0436</td>
        /// 				<td width="34%">Afrikaans - South Africa</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sq</td>
        /// 				<td width="34%">0x001C</td>
        /// 				<td width="34%">Albanian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sq-AL</td>
        /// 				<td width="34%">0x041C</td>
        /// 				<td width="34%">Albanian - Albania</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar</td>
        /// 				<td width="34%">0x0001</td>
        /// 				<td width="34%">Arabic</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-DZ</td>
        /// 				<td width="34%">0x1401</td>
        /// 				<td width="34%">Arabic - Algeria</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-BH</td>
        /// 				<td width="34%">0x3C01</td>
        /// 				<td width="34%">Arabic - Bahrain</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-EG</td>
        /// 				<td width="34%">0x0C01</td>
        /// 				<td width="34%">Arabic - Egypt</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-IQ</td>
        /// 				<td width="34%">0x0801</td>
        /// 				<td width="34%">Arabic - Iraq</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-JO</td>
        /// 				<td width="34%">0x2C01</td>
        /// 				<td width="34%">Arabic - Jordan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-KW</td>
        /// 				<td width="34%">0x3401</td>
        /// 				<td width="34%">Arabic - Kuwait</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-LB</td>
        /// 				<td width="34%">0x3001</td>
        /// 				<td width="34%">Arabic - Lebanon</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-LY</td>
        /// 				<td width="34%">0x1001</td>
        /// 				<td width="34%">Arabic - Libya</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-MA</td>
        /// 				<td width="34%">0x1801</td>
        /// 				<td width="34%">Arabic - Morocco</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-OM</td>
        /// 				<td width="34%">0x2001</td>
        /// 				<td width="34%">Arabic - Oman</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-QA</td>
        /// 				<td width="34%">0x4001</td>
        /// 				<td width="34%">Arabic - Qatar</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-SA</td>
        /// 				<td width="34%">0x0401</td>
        /// 				<td width="34%">Arabic - Saudi Arabia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-SY</td>
        /// 				<td width="34%">0x2801</td>
        /// 				<td width="34%">Arabic - Syria</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-TN</td>
        /// 				<td width="34%">0x1C01</td>
        /// 				<td width="34%">Arabic - Tunisia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-AE</td>
        /// 				<td width="34%">0x3801</td>
        /// 				<td width="34%">Arabic - United Arab Emirates</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ar-YE</td>
        /// 				<td width="34%">0x2401</td>
        /// 				<td width="34%">Arabic - Yemen</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hy</td>
        /// 				<td width="34%">0x002B</td>
        /// 				<td width="34%">Armenian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hy-AM</td>
        /// 				<td width="34%">0x042B</td>
        /// 				<td width="34%">Armenian - Armenia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">az</td>
        /// 				<td width="34%">0x002C</td>
        /// 				<td width="34%">Azeri</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">az-AZ-Cyrl</td>
        /// 				<td width="34%">0x082C</td>
        /// 				<td width="34%">Azeri (Cyrillic) - Azerbaijan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">az-AZ-Latn</td>
        /// 				<td width="34%">0x042C</td>
        /// 				<td width="34%">Azeri (Latin) - Azerbaijan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">eu</td>
        /// 				<td width="34%">0x002D</td>
        /// 				<td width="34%">Basque</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">eu-ES</td>
        /// 				<td width="34%">0x042D</td>
        /// 				<td width="34%">Basque - Basque</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">be</td>
        /// 				<td width="34%">0x0023</td>
        /// 				<td width="34%">Belarusian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">be-BY</td>
        /// 				<td width="34%">0x0423</td>
        /// 				<td width="34%">Belarusian - Belarus</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">bg</td>
        /// 				<td width="34%">0x0002</td>
        /// 				<td width="34%">Bulgarian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">bg-BG</td>
        /// 				<td width="34%">0x0402</td>
        /// 				<td width="34%">Bulgarian - Bulgaria</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ca</td>
        /// 				<td width="34%">0x0003</td>
        /// 				<td width="34%">Catalan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ca-ES</td>
        /// 				<td width="34%">0x0403</td>
        /// 				<td width="34%">Catalan - Catalan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-HK</td>
        /// 				<td width="34%">0x0C04</td>
        /// 				<td width="34%">Chinese - Hong Kong SAR</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-MO</td>
        /// 				<td width="34%">0x1404</td>
        /// 				<td width="34%">Chinese - Macau SAR</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-CN</td>
        /// 				<td width="34%">0x0804</td>
        /// 				<td width="34%">Chinese - China</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-CHS</td>
        /// 				<td width="34%">0x0004</td>
        /// 				<td width="34%">Chinese (Simplified)</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-SG</td>
        /// 				<td width="34%">0x1004</td>
        /// 				<td width="34%">Chinese - Singapore</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-TW</td>
        /// 				<td width="34%">0x0404</td>
        /// 				<td width="34%">Chinese - Taiwan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">zh-CHT</td>
        /// 				<td width="34%">0x7C04</td>
        /// 				<td width="34%">Chinese (Traditional)</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hr</td>
        /// 				<td width="34%">0x001A</td>
        /// 				<td width="34%">Croatian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hr-HR</td>
        /// 				<td width="34%">0x041A</td>
        /// 				<td width="34%">Croatian - Croatia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">cs</td>
        /// 				<td width="34%">0x0005</td>
        /// 				<td width="34%">Czech</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">cs-CZ</td>
        /// 				<td width="34%">0x0405</td>
        /// 				<td width="34%">Czech - Czech Republic</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">da</td>
        /// 				<td width="34%">0x0006</td>
        /// 				<td width="34%">Danish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">da-DK</td>
        /// 				<td width="34%">0x0406</td>
        /// 				<td width="34%">Danish - Denmark</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">div</td>
        /// 				<td width="34%">0x0065</td>
        /// 				<td width="34%">Dhivehi</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">div-MV</td>
        /// 				<td width="34%">0x0465</td>
        /// 				<td width="34%">Dhivehi - Maldives</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">nl</td>
        /// 				<td width="34%">0x0013</td>
        /// 				<td width="34%">Dutch</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">nl-BE</td>
        /// 				<td width="34%">0x0813</td>
        /// 				<td width="34%">Dutch - Belgium</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">nl-NL</td>
        /// 				<td width="34%">0x0413</td>
        /// 				<td width="34%">Dutch - The Netherlands</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en</td>
        /// 				<td width="34%">0x0009</td>
        /// 				<td width="34%">English</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-AU</td>
        /// 				<td width="34%">0x0C09</td>
        /// 				<td width="34%">English - Australia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-BZ</td>
        /// 				<td width="34%">0x2809</td>
        /// 				<td width="34%">English - Belize</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-CA</td>
        /// 				<td width="34%">0x1009</td>
        /// 				<td width="34%">English - Canada</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-CB</td>
        /// 				<td width="34%">0x2409</td>
        /// 				<td width="34%">English - Caribbean</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-IE</td>
        /// 				<td width="34%">0x1809</td>
        /// 				<td width="34%">English - Ireland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-JM</td>
        /// 				<td width="34%">0x2009</td>
        /// 				<td width="34%">English - Jamaica</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-NZ</td>
        /// 				<td width="34%">0x1409</td>
        /// 				<td width="34%">English - New Zealand</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-PH</td>
        /// 				<td width="34%">0x3409</td>
        /// 				<td width="34%">English - Philippines</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-ZA</td>
        /// 				<td width="34%">0x1C09</td>
        /// 				<td width="34%">English - South Africa</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-TT</td>
        /// 				<td width="34%">0x2C09</td>
        /// 				<td width="34%">English - Trinidad and Tobago</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-GB</td>
        /// 				<td width="34%">0x0809</td>
        /// 				<td width="34%">English - United Kingdom</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-US</td>
        /// 				<td width="34%">0x0409</td>
        /// 				<td width="34%">English - United States</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">en-ZW</td>
        /// 				<td width="34%">0x3009</td>
        /// 				<td width="34%">English - Zimbabwe</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">et</td>
        /// 				<td width="34%">0x0025</td>
        /// 				<td width="34%">Estonian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">et-EE</td>
        /// 				<td width="34%">0x0425</td>
        /// 				<td width="34%">Estonian - Estonia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fo</td>
        /// 				<td width="34%">0x0038</td>
        /// 				<td width="34%">Faroese</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fo-FO</td>
        /// 				<td width="34%">0x0438</td>
        /// 				<td width="34%">Faroese - Faroe Islands</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fa</td>
        /// 				<td width="34%">0x0029</td>
        /// 				<td width="34%">Farsi</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fa-IR</td>
        /// 				<td width="34%">0x0429</td>
        /// 				<td width="34%">Farsi - Iran</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fi</td>
        /// 				<td width="34%">0x000B</td>
        /// 				<td width="34%">Finnish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fi-FI</td>
        /// 				<td width="34%">0x040B</td>
        /// 				<td width="34%">Finnish - Finland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr</td>
        /// 				<td width="34%">0x000C</td>
        /// 				<td width="34%">French</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-BE</td>
        /// 				<td width="34%">0x080C</td>
        /// 				<td width="34%">French - Belgium</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-CA</td>
        /// 				<td width="34%">0x0C0C</td>
        /// 				<td width="34%">French - Canada</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-FR</td>
        /// 				<td width="34%">0x040C</td>
        /// 				<td width="34%">French - France</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-LU</td>
        /// 				<td width="34%">0x140C</td>
        /// 				<td width="34%">French - Luxembourg</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-MC</td>
        /// 				<td width="34%">0x180C</td>
        /// 				<td width="34%">French - Monaco</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">fr-CH</td>
        /// 				<td width="34%">0x100C</td>
        /// 				<td width="34%">French - Switzerland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">gl</td>
        /// 				<td width="34%">0x0056</td>
        /// 				<td width="34%">Galician</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">gl-ES</td>
        /// 				<td width="34%">0x0456</td>
        /// 				<td width="34%">Galician - Galician</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ka</td>
        /// 				<td width="34%">0x0037</td>
        /// 				<td width="34%">Georgian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ka-GE</td>
        /// 				<td width="34%">0x0437</td>
        /// 				<td width="34%">Georgian - Georgia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de</td>
        /// 				<td width="34%">0x0007</td>
        /// 				<td width="34%">German</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de-AT</td>
        /// 				<td width="34%">0x0C07</td>
        /// 				<td width="34%">German - Austria</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de-DE</td>
        /// 				<td width="34%">0x0407</td>
        /// 				<td width="34%">German - Germany</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de-LI</td>
        /// 				<td width="34%">0x1407</td>
        /// 				<td width="34%">German - Liechtenstein</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de-LU</td>
        /// 				<td width="34%">0x1007</td>
        /// 				<td width="34%">German - Luxembourg</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">de-CH</td>
        /// 				<td width="34%">0x0807</td>
        /// 				<td width="34%">German - Switzerland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">el</td>
        /// 				<td width="34%">0x0008</td>
        /// 				<td width="34%">Greek</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">el-GR</td>
        /// 				<td width="34%">0x0408</td>
        /// 				<td width="34%">Greek - Greece</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">gu</td>
        /// 				<td width="34%">0x0047</td>
        /// 				<td width="34%">Gujarati</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">gu-IN</td>
        /// 				<td width="34%">0x0447</td>
        /// 				<td width="34%">Gujarati - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">he</td>
        /// 				<td width="34%">0x000D</td>
        /// 				<td width="34%">Hebrew</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">he-IL</td>
        /// 				<td width="34%">0x040D</td>
        /// 				<td width="34%">Hebrew - Israel</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hi</td>
        /// 				<td width="34%">0x0039</td>
        /// 				<td width="34%">Hindi</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hi-IN</td>
        /// 				<td width="34%">0x0439</td>
        /// 				<td width="34%">Hindi - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hu</td>
        /// 				<td width="34%">0x000E</td>
        /// 				<td width="34%">Hungarian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">hu-HU</td>
        /// 				<td width="34%">0x040E</td>
        /// 				<td width="34%">Hungarian - Hungary</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">is</td>
        /// 				<td width="34%">0x000F</td>
        /// 				<td width="34%">Icelandic</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">is-IS</td>
        /// 				<td width="34%">0x040F</td>
        /// 				<td width="34%">Icelandic - Iceland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">id</td>
        /// 				<td width="34%">0x0021</td>
        /// 				<td width="34%">Indonesian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">id-ID</td>
        /// 				<td width="34%">0x0421</td>
        /// 				<td width="34%">Indonesian - Indonesia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">it</td>
        /// 				<td width="34%">0x0010</td>
        /// 				<td width="34%">Italian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">it-IT</td>
        /// 				<td width="34%">0x0410</td>
        /// 				<td width="34%">Italian - Italy</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">it-CH</td>
        /// 				<td width="34%">0x0810</td>
        /// 				<td width="34%">Italian - Switzerland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ja</td>
        /// 				<td width="34%">0x0011</td>
        /// 				<td width="34%">Japanese</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ja-JP</td>
        /// 				<td width="34%">0x0411</td>
        /// 				<td width="34%">Japanese - Japan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kn</td>
        /// 				<td width="34%">0x004B</td>
        /// 				<td width="34%">Kannada</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kn-IN</td>
        /// 				<td width="34%">0x044B</td>
        /// 				<td width="34%">Kannada - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kk</td>
        /// 				<td width="34%">0x003F</td>
        /// 				<td width="34%">Kazakh</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kk-KZ</td>
        /// 				<td width="34%">0x043F</td>
        /// 				<td width="34%">Kazakh - Kazakhstan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kok</td>
        /// 				<td width="34%">0x0057</td>
        /// 				<td width="34%">Konkani</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">kok-IN</td>
        /// 				<td width="34%">0x0457</td>
        /// 				<td width="34%">Konkani - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ko</td>
        /// 				<td width="34%">0x0012</td>
        /// 				<td width="34%">Korean</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ko-KR</td>
        /// 				<td width="34%">0x0412</td>
        /// 				<td width="34%">Korean - Korea</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ky</td>
        /// 				<td width="34%">0x0040</td>
        /// 				<td width="34%">Kyrgyz</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ky-KZ</td>
        /// 				<td width="34%">0x0440</td>
        /// 				<td width="34%">Kyrgyz - Kazakhstan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">lv</td>
        /// 				<td width="34%">0x0026</td>
        /// 				<td width="34%">Latvian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">lv-LV</td>
        /// 				<td width="34%">0x0426</td>
        /// 				<td width="34%">Latvian - Latvia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">lt</td>
        /// 				<td width="34%">0x0027</td>
        /// 				<td width="34%">Lithuanian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">lt-LT</td>
        /// 				<td width="34%">0x0427</td>
        /// 				<td width="34%">Lithuanian - Lithuania</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mk</td>
        /// 				<td width="34%">0x002F</td>
        /// 				<td width="34%">Macedonian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mk-MK</td>
        /// 				<td width="34%">0x042F</td>
        /// 				<td width="34%">Macedonian - FYROM</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ms</td>
        /// 				<td width="34%">0x003E</td>
        /// 				<td width="34%">Malay</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ms-BN</td>
        /// 				<td width="34%">0x083E</td>
        /// 				<td width="34%">Malay - Brunei</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ms-MY</td>
        /// 				<td width="34%">0x043E</td>
        /// 				<td width="34%">Malay - Malaysia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mr</td>
        /// 				<td width="34%">0x004E</td>
        /// 				<td width="34%">Marathi</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mr-IN</td>
        /// 				<td width="34%">0x044E</td>
        /// 				<td width="34%">Marathi - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mn</td>
        /// 				<td width="34%">0x0050</td>
        /// 				<td width="34%">Mongolian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">mn-MN</td>
        /// 				<td width="34%">0x0450</td>
        /// 				<td width="34%">Mongolian - Mongolia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">no</td>
        /// 				<td width="34%">0x0014</td>
        /// 				<td width="34%">Norwegian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">nb-NO</td>
        /// 				<td width="34%">0x0414</td>
        /// 				<td width="34%">Norwegian (Bokmål) - Norway</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">nn-NO</td>
        /// 				<td width="34%">0x0814</td>
        /// 				<td width="34%">Norwegian (Nynorsk) - Norway</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pl</td>
        /// 				<td width="34%">0x0015</td>
        /// 				<td width="34%">Polish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pl-PL</td>
        /// 				<td width="34%">0x0415</td>
        /// 				<td width="34%">Polish - Poland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pt</td>
        /// 				<td width="34%">0x0016</td>
        /// 				<td width="34%">Portuguese</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pt-BR</td>
        /// 				<td width="34%">0x0416</td>
        /// 				<td width="34%">Portuguese - Brazil</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pt-PT</td>
        /// 				<td width="34%">0x0816</td>
        /// 				<td width="34%">Portuguese - Portugal</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pa</td>
        /// 				<td width="34%">0x0046</td>
        /// 				<td width="34%">Punjabi</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">pa-IN</td>
        /// 				<td width="34%">0x0446</td>
        /// 				<td width="34%">Punjabi - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ro</td>
        /// 				<td width="34%">0x0018</td>
        /// 				<td width="34%">Romanian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ro-RO</td>
        /// 				<td width="34%">0x0418</td>
        /// 				<td width="34%">Romanian - Romania</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ru</td>
        /// 				<td width="34%">0x0019</td>
        /// 				<td width="34%">Russian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ru-RU</td>
        /// 				<td width="34%">0x0419</td>
        /// 				<td width="34%">Russian - Russia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sa</td>
        /// 				<td width="34%">0x004F</td>
        /// 				<td width="34%">Sanskrit</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sa-IN</td>
        /// 				<td width="34%">0x044F</td>
        /// 				<td width="34%">Sanskrit - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sr-SP-Cyrl</td>
        /// 				<td width="34%">0x0C1A</td>
        /// 				<td width="34%">Serbian (Cyrillic) - Serbia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sr-SP-Latn</td>
        /// 				<td width="34%">0x081A</td>
        /// 				<td width="34%">Serbian (Latin) - Serbia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sk</td>
        /// 				<td width="34%">0x001B</td>
        /// 				<td width="34%">Slovak</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sk-SK</td>
        /// 				<td width="34%">0x041B</td>
        /// 				<td width="34%">Slovak - Slovakia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sl</td>
        /// 				<td width="34%">0x0024</td>
        /// 				<td width="34%">Slovenian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sl-SI</td>
        /// 				<td width="34%">0x0424</td>
        /// 				<td width="34%">Slovenian - Slovenia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es</td>
        /// 				<td width="34%">0x000A</td>
        /// 				<td width="34%">Spanish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-AR</td>
        /// 				<td width="34%">0x2C0A</td>
        /// 				<td width="34%">Spanish - Argentina</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-BO</td>
        /// 				<td width="34%">0x400A</td>
        /// 				<td width="34%">Spanish - Bolivia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-CL</td>
        /// 				<td width="34%">0x340A</td>
        /// 				<td width="34%">Spanish - Chile</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-CO</td>
        /// 				<td width="34%">0x240A</td>
        /// 				<td width="34%">Spanish - Colombia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-CR</td>
        /// 				<td width="34%">0x140A</td>
        /// 				<td width="34%">Spanish - Costa Rica</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-DO</td>
        /// 				<td width="34%">0x1C0A</td>
        /// 				<td width="34%">Spanish - Dominican Republic</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-EC</td>
        /// 				<td width="34%">0x300A</td>
        /// 				<td width="34%">Spanish - Ecuador</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-SV</td>
        /// 				<td width="34%">0x440A</td>
        /// 				<td width="34%">Spanish - El Salvador</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-GT</td>
        /// 				<td width="34%">0x100A</td>
        /// 				<td width="34%">Spanish - Guatemala</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-HN</td>
        /// 				<td width="34%">0x480A</td>
        /// 				<td width="34%">Spanish - Honduras</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-MX</td>
        /// 				<td width="34%">0x080A</td>
        /// 				<td width="34%">Spanish - Mexico</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-NI</td>
        /// 				<td width="34%">0x4C0A</td>
        /// 				<td width="34%">Spanish - Nicaragua</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-PA</td>
        /// 				<td width="34%">0x180A</td>
        /// 				<td width="34%">Spanish - Panama</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-PY</td>
        /// 				<td width="34%">0x3C0A</td>
        /// 				<td width="34%">Spanish - Paraguay</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-PE</td>
        /// 				<td width="34%">0x280A</td>
        /// 				<td width="34%">Spanish - Peru</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-PR</td>
        /// 				<td width="34%">0x500A</td>
        /// 				<td width="34%">Spanish - Puerto Rico</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-ES</td>
        /// 				<td width="34%">0x0C0A</td>
        /// 				<td width="34%">Spanish - Spain</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-UY</td>
        /// 				<td width="34%">0x380A</td>
        /// 				<td width="34%">Spanish - Uruguay</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">es-VE</td>
        /// 				<td width="34%">0x200A</td>
        /// 				<td width="34%">Spanish - Venezuela</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sw</td>
        /// 				<td width="34%">0x0041</td>
        /// 				<td width="34%">Swahili</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sw-KE</td>
        /// 				<td width="34%">0x0441</td>
        /// 				<td width="34%">Swahili - Kenya</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sv</td>
        /// 				<td width="34%">0x001D</td>
        /// 				<td width="34%">Swedish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sv-FI</td>
        /// 				<td width="34%">0x081D</td>
        /// 				<td width="34%">Swedish - Finland</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">sv-SE</td>
        /// 				<td width="34%">0x041D</td>
        /// 				<td width="34%">Swedish - Sweden</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">syr</td>
        /// 				<td width="34%">0x005A</td>
        /// 				<td width="34%">Syriac</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">syr-SY</td>
        /// 				<td width="34%">0x045A</td>
        /// 				<td width="34%">Syriac - Syria</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ta</td>
        /// 				<td width="34%">0x0049</td>
        /// 				<td width="34%">Tamil</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ta-IN</td>
        /// 				<td width="34%">0x0449</td>
        /// 				<td width="34%">Tamil - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">tt</td>
        /// 				<td width="34%">0x0044</td>
        /// 				<td width="34%">Tatar</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">tt-RU</td>
        /// 				<td width="34%">0x0444</td>
        /// 				<td width="34%">Tatar - Russia</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">te</td>
        /// 				<td width="34%">0x004A</td>
        /// 				<td width="34%">Telugu</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">te-IN</td>
        /// 				<td width="34%">0x044A</td>
        /// 				<td width="34%">Telugu - India</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">th</td>
        /// 				<td width="34%">0x001E</td>
        /// 				<td width="34%">Thai</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">th-TH</td>
        /// 				<td width="34%">0x041E</td>
        /// 				<td width="34%">Thai - Thailand</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">tr</td>
        /// 				<td width="34%">0x001F</td>
        /// 				<td width="34%">Turkish</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">tr-TR</td>
        /// 				<td width="34%">0x041F</td>
        /// 				<td width="34%">Turkish - Turkey</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">uk</td>
        /// 				<td width="34%">0x0022</td>
        /// 				<td width="34%">Ukrainian</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">uk-UA</td>
        /// 				<td width="34%">0x0422</td>
        /// 				<td width="34%">Ukrainian - Ukraine</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ur</td>
        /// 				<td width="34%">0x0020</td>
        /// 				<td width="34%">Urdu</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">ur-PK</td>
        /// 				<td width="34%">0x0420</td>
        /// 				<td width="34%">Urdu - Pakistan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">uz</td>
        /// 				<td width="34%">0x0043</td>
        /// 				<td width="34%">Uzbek</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">uz-UZ-Cyrl</td>
        /// 				<td width="34%">0x0843</td>
        /// 				<td width="34%">Uzbek (Cyrillic) - Uzbekistan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">uz-UZ-Latn</td>
        /// 				<td width="34%">0x0443</td>
        /// 				<td width="34%">Uzbek (Latin) - Uzbekistan</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">vi</td>
        /// 				<td width="34%">0x002A</td>
        /// 				<td width="34%">Vietnamese</td>
        /// 			</tr>
        /// 			<tr valign="top">
        /// 				<td width="32%">vi-VN</td>
        /// 				<td width="34%">0x042A</td>
        /// 				<td width="34%">Vietnamese - Vietnam</td>
        /// 			</tr>
        /// 		</tbody>
        /// 	</table>
        /// </remarks>
        [Category("Localization Settings")]
        [Description("Gets or sets the culture supported by this calendar.")]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(CultureInfoConverter))]
        [DefaultValue(typeof(CultureInfo), "en-US")]
        public CultureInfo Culture
        {
            get
            {
                if (this.cultureInfo != null)
                {
                    return this.cultureInfo;
                }
                return CultureInfo.CurrentCulture;
            }
            set
            {
                if (this.cultureInfo != value)
                {
                    this.cultureInfo = value;
                    this.OnNotifyPropertyChanged("Culture");
                }
            }
        }

        ///// <summary>
        ///// Gets or sets the culture identifier for the current CultureInfo.
        ///// </summary>
        ///// <remarks>
        ///// 	<para>In most cases, the culture identifier is mapped to the corresponding National
        /////     Language Support (NLS) locale identifier.</para>
        ///// </remarks>
        //[Browsable(false)]
        //[Category("Localization Settings")]
        //[DefaultValue(1033)]
        //[Description("Gets or sets the culture identifier for the current CultureInfo.")]
        //[NotifyParentProperty(true)]
        //[Obsolete("Changing the culture should be done via the Culture property.", false)]
        //public int CultureID
        //{
        //    get
        //    {
        //        if (this.cultureID.HasValue)
        //        {
        //            return this.cultureID.Value;
        //        }
        //        return CultureInfo.CurrentCulture.LCID;
        //    }
        //    set
        //    {
        //        CultureInfo TempCultureInfo = new CultureInfo(value, true);
        //        if (this.cultureInfo != TempCultureInfo)
        //        {
        //            this.cultureInfo = TempCultureInfo;
        //            this.cultureID = TempCultureInfo.LCID;
        //            this.OnNotifyPropertyChanged("CultureID");
        //        }
        //    }
        //}

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
                if (this.Culture.DateTimeFormat.Calendar != null)
                {
                    return this.Culture.DateTimeFormat.Calendar;
                }
                return DateTimeFormatInfo.CurrentInfo.Calendar;
            }
        }

        /// <summary>
        /// Gets or sets the format string that will be applied to the dates presented in the
        /// calendar area.
        /// </summary>
        /// <remarks>
        /// For additional details see <a href="DateFormat.html">Date Format Pattern</a>
        /// topic
        /// </remarks>
        [Category("Localization Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue("%d")]
        [Description("Gets or sets the formatting string that will be applied to the days in the calendar.")]
        [Localizable(true)]
        public string DayCellFormat
        {
            get
            {
                if (string.IsNullOrEmpty(cellDayFormat))
                {
                    return "%d";
                }
                return cellDayFormat;
            }
            set
            {
                if (this.cellDayFormat != value)
                {
                    this.cellDayFormat = value;
                    this.OnNotifyPropertyChanged("DayCellFormat");
                }
            }
        }

        /// <summary>
        /// 	<para>Specifies the day to display as the first day of the week on the
        ///     RadCalendar control.</para>
        /// </summary>
        /// <remarks>
        /// 	<para>The <b>FirstDayOfWeek</b> enumeration represents the values that specify
        ///     which day to display as the first day of the week on the RadCalendar control.</para>
        /// 	<list type="table">
        /// 		<listheader>
        /// 			<term>Member name</term>
        /// 			<description>Description</description>
        /// 		</listheader>
        /// 		<item>
        /// 			<term><b>Default</b></term>
        /// 			<description>The first day of the week is specified by the system
        ///             settings.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Friday</b></term>
        /// 			<description>The first day of the week is Friday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Monday</b></term>
        /// 			<description>The first day of the week is Monday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Saturday</b></term>
        /// 			<description>The first day of the week is Saturday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Sunday</b></term>
        /// 			<description>The first day of the week is Sunday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Thursday</b></term>
        /// 			<description>The first day of the week is Thursday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Tuesday</b></term>
        /// 			<description>The first day of the week is Tuesday.</description>
        /// 		</item>
        /// 		<item>
        /// 			<term><b>Wednesday</b></term>
        /// 			<description>The first day of the week is Wednesday.</description>
        /// 		</item>
        /// 	</list>
        /// </remarks>
        [Category("Localization Settings")]
        [DefaultValue(FirstDayOfWeek.Default)]
        [Description("Specifies the day to display as the first day of the week.")]
        [NotifyParentProperty(true)]
        public FirstDayOfWeek FirstDayOfWeek
        {
            get
            {
                if (firstDayOfWeek.HasValue)
                {
                    return firstDayOfWeek.Value;
                }
                return FirstDayOfWeek.Default;
            }
            set
            {
                if ((value < FirstDayOfWeek.Sunday) || (value > FirstDayOfWeek.Default))
                {
                    throw new ArgumentOutOfRangeException("FirstDayOfWeek value");
                }
                if (this.firstDayOfWeek != value)
                {
                    this.firstDayOfWeek = value;
                    this.OnNotifyPropertyChanged("FirstDayOfWeek");
                }
            }
        }

        /// <summary>Gets or sets the format string that is applied to the calendar title.</summary>
        /// <remarks>
        /// 	<para>The <i>property</i> should contain either a format specifier character or a
        ///     custom format pattern. For more information, see the summary page for
        ///     <strong>System.Globalization.DateTimeFormatInfo</strong>.</para>
        /// 	<para>By default this <em>property</em> uses formatting string of
        ///     '<font size="2"><strong>MMMM yyyy</strong>'. Valid formats are all supported by the .NET
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
        [DefaultValue("MMMM yyyy")]
        [Description("Gets or sets the format string that is applied to the calendar title.")]
        [Localizable(true)]
        public string TitleFormat
        {
            get
            {
                if (string.IsNullOrEmpty(titleFormat))
                {
                    return "MMMM yyyy";
                }
                return titleFormat;
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
        /// Gets or sets whether tool tips are displayed for this speciffic control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Gets or sets whether tool tips are displayed for this speciffic control.")]
        public virtual bool AllowToolTips
        {
            get
            {
                return this.allowToolTips;
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

        /// <summary>
        /// Gets or sets the separator string that will be put between start and end months in a multi view title.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Title Settings")]
        [DefaultValue(" - ")]
        [Description("Gets or sets the separator string that will be put between start and end months in a multi view title.")]
        [Localizable(true)]
        public string DateRangeSeparator
        {
            get
            {
                if (string.IsNullOrEmpty(dateRangeSeparator))
                {
                    return " - ";
                }
                return dateRangeSeparator;
            }
            set
            {
                if (this.dateRangeSeparator != value)
                {
                    this.dateRangeSeparator = value;
                    this.OnNotifyPropertyChanged("DateRangeSeparator");
                }
            }
        }

        #endregion

        #region Single View Formatting

        /// <summary>
        /// Gets or sets the the count of rows to be displayed by a single CalendarView.
        /// </summary>
        /// <remarks>
        /// If the calendar represents a multi view, this property applies to the child views
        /// inside the multi view.
        /// </remarks>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(6)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The the count of rows to be displayed by a single CalendarView")]
        public int Rows
        {
            get
            {
                if (singleViewRows.HasValue)
                {
                    return singleViewRows.Value;
                }
                return 6;
            }
            set
            {
                if (this.singleViewRows != value)
                {
                    if (CalculateValidTableLayout(value, false))
                    {
                        this.singleViewRows = value;
                        this.OnNotifyPropertyChanged("Rows");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of columns to be displayed by a single CalendarView.
        /// </summary>
        /// <remarks>
        /// If the calendar represents a multi view, this property applies to the child views
        /// inside the multi view.
        /// </remarks>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(7)]
        [Description("The the count of columns to be displayed by a single CalendarView")]
        [RefreshProperties(RefreshProperties.All)]
        public int Columns
        {
            get
            {
                if (singleViewColumns.HasValue)
                {
                    return singleViewColumns.Value;
                }
                return 7;
            }
            set
            {
                if (this.singleViewColumns != value)
                {
                    if (CalculateValidTableLayout(value, true))
                    {
                        this.singleViewColumns = value;
                        this.OnNotifyPropertyChanged("Columns");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the today button of the footer element
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(7)]
        [Description("Gets the today button of the footer element")]
        public RadButtonElement TodayButton
        {
            get
            {
                return this.calendarElement.CalendarStatusElement.TodayButton;
            }
        }

        /// <summary>
        /// Gets the clear button of the footer element
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("General View Settings")]
        [DefaultValue(7)]
        [Description("Gets the clear button of the footer element")]
        public RadButtonElement ClearButton
        {
            get
            {
                return this.calendarElement.CalendarStatusElement.ClearButton;
            }
        }

        /// <summary>
        /// The Width applied to a Header
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("The Width applied to a Header")]
        public int HeaderWidth
        {
            get
            {
                if (this.headerWidth.HasValue)
                {
                    return headerWidth.Value;
                }
                return 17;
            }
            set
            {
                if (this.headerWidth != value)
                {
                    if (value >= 17)
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
        public int HeaderHeight
        {
            get
            {
                if (this.headerHeight.HasValue)
                {
                    return headerHeight.Value;
                }
                return 17;
            }
            set
            {
                if (this.headerHeight != value)
                {
                    if (value >= 17)
                    {
                        this.headerHeight = value;
                        this.OnNotifyPropertyChanged("HeaderHeight");
                    }
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the horizontal alignment of the date cells content inside the
        ///     calendar area.</para>
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
        [Category("General View Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue(System.Windows.Forms.VisualStyles.ContentAlignment.Center)]
        [Description("Specifies the horizonal alignment of the day cells in the calendar")]
        public System.Drawing.ContentAlignment CellAlign
        {
            get { return cellAlign; }
            set
            {
             
                if (this.cellAlign != value)
                {
                    this.cellAlign = value;
                    this.OnNotifyPropertyChanged("CellAlign");
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of rows to be displayed by a multi month CalendarView.
        /// </summary>
        [Category("General View Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of month rows in a multi view calendar.")]
        public int MultiViewRows
        {
            get { return multiViewRows; }
            set
            {
                if (this.multiViewRows != value && this.AllowMultipleView)
                {
                    this.multiViewRows = value;
                    this.OnNotifyPropertyChanged("MultiViewRows");

                    if (this.multiViewColumns > 1 || this.multiViewRows > 1)
                    {
                        this.calendarView = new MultiMonthView(this);
                    }
                    else
                    {
                        this.calendarView = new MonthView(this);
                    }

                    calendarView.Initialize();
                    ReInitializeCalendarElement();
                }
            }
        }

        /// <summary>
        /// Gets or sets the the count of columns to be displayed by a multi month CalendarView.
        /// </summary>
        [Category("General View Settings")]
        [NotifyParentProperty(true)]
        [DefaultValue(1)]
        [Description("Gets or sets the number of month columns in a multi view calendar.")]
        public int MultiViewColumns
        {
            get
            {
                return multiViewColumns;
            }
            set
            {
                if (this.multiViewColumns != value && this.AllowMultipleView)
                {
                    this.multiViewColumns = value;
                    this.OnNotifyPropertyChanged("MultiViewColumns");

                    if (this.multiViewColumns > 1 || this.multiViewRows > 1)
                    {
                        this.calendarView = new MultiMonthView(this);
                    }
                    else
                    {
                        this.calendarView = new MonthView(this);
                    }

                    this.calendarView.Initialize();
                    ReInitializeCalendarElement();

                }
            }
        }

        #endregion

        #region Date Management

        /// <summary>
        /// Gets or sets the maximum date valid for selection by
        /// Telerik RadCalendar. Must be interpreted as the higher bound of the valid
        /// dates range available for selection. Telerik RadCalendar will not allow
        /// navigation or selection past this date.
        /// </summary>
        /// <remarks>
        /// This property has a default value of <font size="1"><strong>12/30/2099</strong>
        /// (Gregorian calendar date).</font>
        /// </remarks>
        [Category("Dates Management")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(DateTime), "12/31/2099")]
        [Description(
            "Gets or sets the maximal date valid for selection by Telerik RadCalendar. Must be interpreted as the Higher bound of the valid dates range available for selection. Telerik RadCalendar will not allow navigation or selection past this date.")]
        public DateTime RangeMaxDate
        {
            get
            {
                if (this.rangeMaxDate.HasValue)
                {
                    return this.rangeMaxDate.Value;
                }
                return new DateTime(2099, 12, 30);
            }
            set
            {
                DateTime dateOnly = TruncateTimeComponent(value);
                if (this.rangeMaxDate != value)
                {
                    if (dateOnly > this.RangeMinDate)
                    {
                        this.rangeMaxDate = dateOnly;
                        this.OnNotifyPropertyChanged("RangeMaxDate");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimal date valid for selection by
        /// Telerik RadCalendar. Must be interpreted as the lower bound of the valid
        /// dates range available for selection. Telerik RadCalendar will not allow
        /// navigation or selection prior to this date.
        /// </summary>
        /// <remarks>
        /// This property has a default value of <font size="1"><strong>1/1/1980</strong>
        /// (Gregorian calendar date).</font>
        /// </remarks>
        [Category("Dates Management")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(DateTime), "1/1/1900")]
        [Description(
            "Gets or sets the minimal date valid for selection by Telerik RadCalendar. Must be interpreted as the Lower bound of the valid dates range available for selection. Telerik RadCalendar will not allow navigation or selection prior to this date.")]
        public DateTime RangeMinDate
        {
            get
            {
                if (this.rangeMinDate.HasValue)
                {
                    return this.rangeMinDate.Value;
                }

                return new DateTime(1900, 1, 1);
            }
            set
            {
                DateTime dateOnly = TruncateTimeComponent(value);
                if (this.rangeMinDate != value)
                {
                    if (dateOnly < this.RangeMaxDate)
                    {
                        this.rangeMinDate = dateOnly;
                        this.OnNotifyPropertyChanged("RangeMinDate");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is in read-only mode.
        /// </summary>
        [Description("Gets or sets a value indicating whether the calendar view is in read-only mode."),
         DefaultValue(false),
         Category("Behavior")]
        public virtual bool ReadOnly
        {
            get
            {
                return this.readOnly;
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
        /// Sets or returns the currently selected date. The default value is the value of
        /// <strong>System.DateTime.MinValue</strong>.
        /// </summary>
        /// <remarks>
        /// 	<para>Use the <b>SelectedDate</b> property to determine the selected date on the >RadCalendar control.</para>
        /// 	<para>The <b>SelectedDate</b> property and the SelectedDates collection are closely related. 
        ///     When the AllowMultipleSelect property is set to <b>false</b>, a mode that allows only a single date selection,
        ///     <b>SelectedDate</b> and <b>SelectedDates[0]</b> have the same value and <b>SelectedDates.Count</b> equals 1.
        ///     When the <b>AllowMultipleSelect</b> property is set to <b>true</b>, mode that allows multiple date 
        ///     selections, <b>SelectedDate</b> and <b>SelectedDates[0]</b> have the same value.</para>
        /// 	<para>The <b>SelectedDate</b> property is set using a System.DateTime object.</para>
        /// 	<para>When the user selects a date on the <strong>RadCalendar</strong> control, the SelectionChanged
        ///     event is raised. The <b>SelectedDate</b> property is updated to the selected date.
        ///     The <b>SelectedDates</b> collection is also updated to contain just this
        ///     date.</para>
        /// 	<blockquote class="dtBlock">
        /// 		<b class="le">Note</b> Both the <b>SelectedDate</b> property and the
        ///         <b>SelectedDates</b> collection are updated before the <b>SelectionChanged</b>
        ///         event is raised. You can override the date selection by using the
        ///         <strong>OnSelectionChanged</strong> event handler to manually set the
        ///         <b>SelectedDate</b> property. The <b>SelectionChanged</b> event does not get
        ///         raised when this property is programmatically set.
        ///     </blockquote>
        /// </remarks>
        [Category("Dates Management")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectedDate
        {
            get
            {
                if (this.SelectedDates.Count > 0)
                {
                    return this.SelectedDates[0];
                }

                return this.RangeMinDate;
            }
            set
            {
                if (!this.updating)
                {
                    SelectionEventArgs args = this.CallOnSelectionChanging(this.selectedDates);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                this.selectedDates.BeginUpdate();
                if (!this.AllowMultipleSelect)
                {
                    this.SelectedDates.Clear();
                }

                if (!this.SelectedDates.CanAdd(value))
                {
                    this.selectedDates.EndUpdate();
                    return;
                }

                if (!this.SelectedDates.Contains(value) && value <= this.RangeMaxDate && value >= this.RangeMinDate )
                {
                    this.SelectedDates.Insert(0, value);
                }

                this.selectedDates.EndUpdate();
                if (!this.updating)
                {
                    this.OnSelectionChanged();
                }

                this.OnNotifyPropertyChanged("SelectedDate");
            }
        }

        /// <summary>
        /// Gets or sets the value that is used by RadCalendar to determine
        /// the viewable area displayed .
        /// </summary>
        /// <remarks>
        /// 	<para>By default, the <strong>FocusedDate</strong> property returns the current
        ///     system date when in runtime, and in design mode defaults to
        ///     <strong>System.DateTime.MinValue.</strong> When the <strong>FocusedDate</strong> is
        ///     set, from that point, the value returned by the <strong>FocusedDate</strong>
        ///     property is the one the user sets.</para>
        /// </remarks>
        [Category("Dates Management")]
        [DefaultValue(typeof(DateTime), "1/1/1980")]
        [NotifyParentProperty(true)]
        [Description("The date used by RadCalendar to determine the viewable area displayed")]
        public DateTime FocusedDate
        {
            get
            {
                DateTime dateValue;
                dateValue = DateTime.Today;
                // DK_28_08_2007 design-time enhancement
                if (this.DesignMode)
                {
                    dateValue = new DateTime(1980, 1, 1);
                }
                if (focusedDate.HasValue)
                {
                    dateValue = focusedDate.Value;
                }
                if (dateValue > this.RangeMaxDate)
                {
                    dateValue = this.RangeMaxDate;
                }
                else if (dateValue < this.RangeMinDate)
                {
                    dateValue = this.RangeMinDate;
                }
                return dateValue;
            }
            set
            {
                if (!this.ReadOnly || this.DesignMode)
                {
                    DateTime dateOnly = TruncateTimeComponent(value);
                    if (this.focusedDate != value)
                    {
                        this.focusedDate = dateOnly;
                        this.OnNotifyPropertyChanged("FocusedDate");
                    }
                }
            }
        }

        /// <summary>
        /// Gets a collection of DateTime objects that represent the
        /// selected dates on the <strong>RadCalendar</strong> control.
        /// </summary>
        /// <value>
        /// A DateTimeCollection that contains a collection of <strong>System.DateTime</strong> objects representing the selected
        /// dates on the <strong>RadCalendar</strong> control. The default value is an empty <b>DateTimeCollection</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <b>SelectedDates</b> collection to determine the currently selected
        ///     dates on the control.</para>
        /// 	<para>The SelectedDate property and the <b>SelectedDates</b> collection are closely related. When the AllowMultipleSelect
        ///     property is set to <b>false</b>, a mode that allows only a single date selection,
        ///     <b>SelectedDate</b> and <b>SelectedDates[0]</b> have the same value and
        ///     <b>SelectedDates.Count</b> equals 1. When the AllowMultipleSelect
        ///     property is set to <b>true</b>, mode that allows multiple date selections,
        ///     <b>SelectedDate</b> and <b>SelectedDates[0]</b> have the same value.</para>
        /// 	<para>The <b>SelectedDates</b> property stores a collection of DateTime objects.</para>
        /// 	<para>When the user selects a date or date range (for example with the column or
        ///     rows selectors) on the <strong>RadCalendar</strong> control, the SelectionChanged
        ///     event is raised. The selected dates are added to the <b>SelectedDates</b>
        ///     collection, accumulating with previously selected dates. The range of dates are not
        ///     sorted by default. The <strong>SelectedDate</strong> property is also updated to
        ///     contain the first date in the <b>SelectedDates</b> collection.</para>
        /// 	<para>You can also use the <b>SelectedDates</b> collection to programmatically
        ///     select dates on the <b>Calendar</b> control. Use the Add, Remove, Clear, and SelectRange
        ///     methods to programmatically manipulate the selected dates in the <b>SelectedDates</b> collection.</para>
        /// 	<blockquote class="dtBlock">
        /// 		<b class="le">Note</b> Both the <b>SelectedDate</b> property and the
        ///         <b>SelectedDates</b> collection are updated before the <b>SelectionChanged</b>
        ///         event is raised.You can override the dates selection by using the
        ///         <strong>OnSelectionChanged</strong> event handler to manually set the
        ///         <b>SelectedDates</b> collection. The <b>SelectionChanged</b> event is not
        ///         raised when this collection is programmatically set.
        ///     </blockquote>
        /// </remarks>
        [Category("Dates Management")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [Description("System.DateTime objects collection that represent the selected dates.")]
        public DateTimeCollection SelectedDates
        {
            get
            {
                if (this.selectedDates == null)
                {
                    this.selectedDates = new DateTimeCollection(this);
                }

                return selectedDates;
            }
        }

        #endregion

        #region Navigation Bar Properties

        /// <summary>
        /// Gets or sets whether navigating RadCalendar is allowed.
        /// </summary>
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Category("Navigation")]
        [Description("Gets or sets whether navigating RadCalendar is allowed.")]
        public bool AllowNavigation
        {
            get
            {
                if (this.enableNavigation.HasValue)
                {
                    return this.enableNavigation.Value;
                }
                return true;
            }
            set
            {
                if (this.enableNavigation != value)
                {
                    this.enableNavigation = value;
                    this.OnNotifyPropertyChanged("AllowNavigation");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the fast navigation in RadCalendar is allowed.
        /// </summary>
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Category("Navigation")]
        [Description("Gets or sets whether the fast navigation in RadCalendar is allowed.")]
        public bool AllowFastNavigation
        {
            get
            {
                if (this.allowFastNavigation.HasValue)
                {
                    return this.allowFastNavigation.Value;
                }
                return true;
            }
            set
            {
                if (this.allowFastNavigation != value)
                {
                    this.allowFastNavigation = value;
                    this.OnNotifyPropertyChanged("AllowFastNavigation");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the text displayed for the previous month navigation control. Will be
        ///     applied only if there is no image set (see
        ///     <see cref="NavigationPrevImage">NavigationPrevImage</see>).
        /// </summary>
        /// <remarks>
        /// 	<para>Use the <em>NavigationPrevText</em> property to provide custom text for the
        ///     previous month navigation element in the title section of
        ///     <strong>RadCalendar</strong>.</para>
        /// </remarks>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> previous month
        /// navigation cell. The default value is <b>"&amp;lt;"</b>.
        /// </value>
        [DefaultValue("<")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed for the previous month navigation control.")]
        [Localizable(true)]
        public string NavigationPrevText
        {
            get
            {
                if (string.IsNullOrEmpty(this.navigationPrevText))
                {
                    return "<";
                }
                return this.navigationPrevText;
            }
            set
            {
                if (this.navigationPrevText != value)
                {
                    this.navigationPrevText = value;
                    this.OnNotifyPropertyChanged("NavigationPrevText");
                    this.CalendarElement.PreviousButton.Text = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the text displayed for the next month navigation control. Will be
        ///     applied if there is no image set (see
        ///     <see cref="NavigationNextImage">NavigationNextImage</see>).
        /// </summary>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> next month navigation
        /// cell. The default value is <b>"&amp;gt;"</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <em>NavigationNextText</em> property to provide custom text for the
        ///     next month navigation element in the title section of
        ///     <strong>RadCalendar</strong>.</para>
        /// </remarks>
        [DefaultValue(">")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed for the next month navigation control.")]
        [Localizable(true)]
        public string NavigationNextText
        {
            get
            {
                if (string.IsNullOrEmpty(this.navigationNextText))
                {
                    return ">";
                }
                return this.navigationNextText;
            }
            set
            {
                if (this.navigationNextText != value)
                {
                    this.navigationNextText = value;
                    this.OnNotifyPropertyChanged("NavigationNextText");
                    this.CalendarElement.NextButton.Text = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the text displayed for the fast navigation previous month control.
        /// </summary>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> selection element in the
        /// fast navigation previous month cell. The default value is
        /// <b>"&amp;lt;&amp;lt;"</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <em>FastNavigationPrevText</em> property to provide custom text for
        ///     the next month navigation element in the title section of
        ///     <strong>RadCalendar</strong>.</para>
        /// </remarks>
        [DefaultValue("<<")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed for the fast previous navigation control.")]
        [Localizable(true)]
        public string FastNavigationPrevText
        {
            get
            {
                if (string.IsNullOrEmpty(this.fastNavigationPrevText))
                {
                    return "<<";
                }
                return this.fastNavigationPrevText;
            }
            set
            {
                if (this.fastNavigationPrevText != value)
                {
                    this.fastNavigationPrevText = value;
                    this.OnNotifyPropertyChanged("FastNavigationPrevText");
                    this.CalendarElement.FastBackwardButton.Text = fastNavigationPrevText;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the text displayed for the fast navigation next month control.
        /// </summary>
        /// <value>
        /// The text displayed for the <strong>CalendarView</strong> selection element in the
        /// fast navigation next month cell. The default value is <b>"&amp;gt;&amp;gt;"</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <em>FastNavigationNextText</em> property to provide custom text for
        ///     the next month navigation element in the title section of <strong>RadCalendar</strong>.</para>
        /// </remarks>
        [DefaultValue(">>")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed for the fast next navigation control.")]
        [Localizable(true)]
        public string FastNavigationNextText
        {
            get
            {
                if (string.IsNullOrEmpty(this.fastNavigationNextText))
                {
                    return ">>";
                }
                return this.fastNavigationNextText;
            }
            set
            {
                if (this.fastNavigationNextText != value)
                {
                    this.fastNavigationNextText = value;
                    this.OnNotifyPropertyChanged("FastNavigationNextText");
                    this.CalendarElement.FastForwardButton.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the image that is displayed for the previous month navigation control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the image that is displayed for the previous month navigation control.")]
        [Localizable(true)]
        [DefaultValue(null)]
        public Image NavigationPrevImage
        {
            get
            {
                return this.CalendarElement.CalendarNavigationElement.LeftButtonImage;
            }
            set
            {
                    this.CalendarElement.CalendarNavigationElement.LeftButtonImage = value;
                    this.OnNotifyPropertyChanged("NavigationPrevImage");
            }
        }

        /// <summary>
        /// Gets or sets the image that is displayed for the next month navigation control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the image that is displayed for the next month navigation control.")]
        [Localizable(true)]
        [DefaultValue(null)]
        public Image NavigationNextImage
        {
            get
            {
                return this.CalendarElement.CalendarNavigationElement.RightButtonImage;
            }
            set
            {
                this.CalendarElement.CalendarNavigationElement.RightButtonImage = value;
                this.OnNotifyPropertyChanged("NavigationNextImage");
            }
        }

        /// <summary>
        /// Gets or sets the image that is displayed for the previous month fast navigation control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the image that is displayed for the fast previous navigation control.")]
        [Localizable(true)]
        [DefaultValue(null)]
        public Image FastNavigationPrevImage
        {
            get
            {
                return this.calendarElement.CalendarNavigationElement.FastLeftButtonImage;
            }
            set
            {
                this.calendarElement.CalendarNavigationElement.FastLeftButtonImage = value;
                this.OnNotifyPropertyChanged("FastNavigationPrevImage");
            }
        }

        /// <summary>
        /// Gets or sets the image that is displayed for the next month fast navigation control.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the image that is displayed for the fast next navigation control.")]
        [Localizable(true)]
        [DefaultValue(null)]
        public Image FastNavigationNextImage
        {
            get
            {
                return this.calendarElement.CalendarNavigationElement.FastRightButtonImage;
            }
            set
            {
                this.calendarElement.CalendarNavigationElement.FastRightButtonImage = value;
                this.OnNotifyPropertyChanged("FastNavigationNextImage");
            }
        }

        /// <summary>
        /// Gets or sets the text displayed as a tooltip for the previous month navigation control.
        /// </summary>
        /// <remarks>
        /// Use the <em>NavigationPrevToolTip</em> property to provide custom text for the
        /// tooltip of the previous month navigation element in the title section of
        /// <strong>RadCalendar</strong>.
        /// </remarks>
        /// <value>
        /// The tooltip text displayed for the <strong>CalendarView</strong> previous month
        /// navigation cell. The default value is <b>"&amp;lt;"</b>.
        /// </value>
        [DefaultValue("<")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed as a tooltip for the previous month navigation control.")]
        [Localizable(true)]
        public string NavigationPrevToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(this.navigationPrevToolTip))
                {
                    return "<";
                }
                return this.navigationPrevToolTip;
            }
            set
            {
                if (this.navigationPrevToolTip != value)
                {
                    this.navigationPrevToolTip = value;
                    this.OnNotifyPropertyChanged("NavigationPrevToolTip");
                    this.CalendarElement.PreviousButton.ToolTipText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text displayed as a tooltip for the next month navigation control.
        /// </summary>
        /// <value>
        /// The tooltip text displayed for the <strong>CalendarView</strong> next month
        /// navigation cell. The default value is <b>"&amp;gt;"</b>.
        /// </value>
        /// <remarks>
        /// Use the <em>NavigationNextToolTip</em> property to provide custom text for the
        /// tooltip of the next month navigation element in the title section of
        /// <strong>RadCalendar</strong>.
        /// </remarks>
        [DefaultValue(">")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed as a tooltip for the next month navigation control.")]
        [Localizable(true)]
        public string NavigationNextToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(this.navigationNextToolTip))
                {
                    return ">";
                }
                return this.navigationNextToolTip;
            }
            set
            {
                if (this.navigationNextToolTip != value)
                {
                    this.navigationNextToolTip = value;
                    this.OnNotifyPropertyChanged("NavigationNextToolTip");
                    this.CalendarElement.NextButton.ToolTipText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text displayed as a tooltip for the fast navigation previous
        /// month control.
        /// </summary>
        /// <remarks>
        /// Use the <em>FastNavigationPrevToolTip</em> property to provide custom text for
        /// the tooltip of the fast navigation previous month element in the title section of
        /// <strong>RadCalendar</strong>.
        /// </remarks>
        /// <value>
        /// The tooltip text displayed for the <strong>CalendarView</strong> fast navigation
        /// previous month cell. The default value is <b>"&amp;lt;&amp;lt;"</b>.
        /// </value>
        [DefaultValue("<<")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the text displayed as a tooltip for the fast navigation previous month control.")]
        [Localizable(true)]
        public string FastNavigationPrevToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(this.fastNavigationPrevToolTip))
                {
                    return "<<";
                }
                return this.fastNavigationPrevToolTip;
            }
            set
            {
                if (this.fastNavigationPrevToolTip != value)
                {
                    this.fastNavigationPrevToolTip = value;
                    this.OnNotifyPropertyChanged("FastNavigationPrevToolTip");
                    this.CalendarElement.FastBackwardButton.ToolTipText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text displayed as a tooltip for the fast navigation previous
        /// month control.
        /// </summary>
        /// <remarks>
        /// Use the <em>FastNavigationPrevToolTip</em> property to provide custom text for
        /// the tooltip of the fast navigation previous month element in the title section of
        /// <strong>RadCalendar</strong>.
        /// </remarks>
        /// <value>
        /// The tooltip text displayed for the <strong>CalendarView</strong> fast navigation
        /// previous month cell. The default value is <b>"&amp;lt;&amp;lt;"</b>.
        /// </value>
        [DefaultValue(">>")]
        [NotifyParentProperty(true)]
        [Category("Navigation Management")]
        [Description("Gets or sets the tooltip text displayed by the navigation button responsible for the fast forward navigation.")]
        [Localizable(true)]
        public string FastNavigationNextToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(this.fastNavigationNextToolTip))
                {
                    return ">>";
                }
                return this.fastNavigationNextToolTip;
            }
            set
            {
                if (this.fastNavigationNextToolTip != value)
                {
                    this.fastNavigationNextToolTip = value;
                    this.OnNotifyPropertyChanged("FastNavigationNextToolTip");
                    this.CalendarElement.FastForwardButton.ToolTipText = value;
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
        [Description("Gets or sets the horizontal alignment of the calendar title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Windows.Forms.VisualStyles.ContentAlignment TitleAlign
        {
            get
            {
                switch (this.CalendarElement.CalendarNavigationElement.TextAlignment)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.MiddleCenter:
                        return System.Windows.Forms.VisualStyles.ContentAlignment.Center;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.TopRight:
                    case ContentAlignment.MiddleRight:
                        return System.Windows.Forms.VisualStyles.ContentAlignment.Right;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.MiddleLeft:
                        return System.Windows.Forms.VisualStyles.ContentAlignment.Left;
                    default:
                        return System.Windows.Forms.VisualStyles.ContentAlignment.Center;
                }
            }
            set
            {
                switch (value)
                {
                    case System.Windows.Forms.VisualStyles.ContentAlignment.Right:
                        this.CalendarElement.CalendarNavigationElement.TextAlignment = ContentAlignment.MiddleRight;
                        break;
                    case System.Windows.Forms.VisualStyles.ContentAlignment.Left:
                        this.CalendarElement.CalendarNavigationElement.TextAlignment = ContentAlignment.MiddleLeft;
                        break;
                    case System.Windows.Forms.VisualStyles.ContentAlignment.Center:
                        this.CalendarElement.CalendarNavigationElement.TextAlignment = ContentAlignment.MiddleCenter;
                        break;
                }
                this.OnNotifyPropertyChanged("TitleAlign");
            }
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Allows RadCalendar to render multiple months in a single view.
        /// </summary>
        [Bindable(false)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [NotifyParentProperty(true)]
        [Description("Allows RadCalendar to render multiple months in a single view.")]
        public bool AllowMultipleView
        {
            get { return this.allowMultiView; }
            set
            {
                if (this.allowMultiView != value)
                {
                    if (this.allowMultiView)
                    {
                        this.MultiViewRows = 1;
                        this.MultiViewColumns = 1;
                    }

                    this.allowMultiView = value;
                    this.OnNotifyPropertyChanged("AllowMultiView");
                }
            }
        }

        /// <summary>
        /// Allows the selection of dates. If not set, selection is forbidden, and if any dates are all ready selected, they are cleared.
        /// </summary>
        [Bindable(false)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Description(
            "Allows the selection of dates. If not set, selection is forbidden, and if any dates are all ready selected, they are cleared.")]
        public bool AllowSelect
        {
            get { return this.allowSelect; }
            set
            {
                if (this.allowSelect != value)
                {
                    this.allowSelect = value;
                    this.OnNotifyPropertyChanged("AllowSelect");
                }
            }
        }

        /// <summary>
        /// Allows the selection of multiple dates. If not set, only a single date is selected, and if any dates
        /// are all ready selected, they are cleared.
        /// </summary>
        [Bindable(false)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [NotifyParentProperty(true)]
        [Description(
            "Allows the selection of multiple dates. If not set, only a single date is selected, and if any dates are all ready selected, they are cleared.")]
        public bool AllowMultipleSelect
        {
            get { return this.allowMultiSelect; }
            set
            {
                if (this.allowMultiSelect != value)
                {
                    this.allowMultiSelect = value;
                    this.OnNotifyPropertyChanged("AllowMultiSelect");
                }
            }
        }

        /// <summary>Gets or sets whether the navigation buttons should be visible.</summary>
        [Category("Header Settings")]
        [DefaultValue(true)]
        [Description("Gets or sets whether the navigation buttons should be visible.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowNavigationButtons
        {
            get
            {
                return this.showNavigationButtons;
            }
            set
            {
                if (this.showNavigationButtons != value)
                {
                    this.showNavigationButtons = value;
                    this.OnNotifyPropertyChanged("ShowNavigationButtons");
                }
            }
        }

        /// <summary>Gets or sets whether the fast navigation buttons should be visible.</summary>
        [Category("Header Settings")]
        [DefaultValue(true)]
        [Description("Gets or sets whether the fast navigation buttons should be visible.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowFastNavigationButtons
        {
            get
            {
                return this.showFastNavigationButtons;
            }
            set
            {
                if (this.showFastNavigationButtons != value)
                {
                    this.showFastNavigationButtons = value;
                    this.OnNotifyPropertyChanged("ShowFastNavigationButtons");
                }
            }
        }

        /// <summary>Gets or sets whether RadCalendar will display a footer row.</summary>
        [Category("Footer Settings")]
        [DefaultValue(false)]
        [Description("Gets or sets whether RadCalendar will display a footer row.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }
            set
            {
                if (this.showFooter != value)
                {
                    this.showFooter = value;
                    if (value)
                    {
                        this.calendarElement.CalendarStatusElement.StartTimer();
                        this.calendarElement.CalendarStatusElement.LabelElement.Text =
                            DateTime.Now.ToString(this.calendarElement.CalendarStatusElement.LableFormat, this.Culture);
                    }

                    this.OnNotifyPropertyChanged("ShowFooter");
                }
            }
        }

        /// <summary>Gets or sets whether RadCalendar will display a header/navigation row.</summary>
        [Category("Header Settings")]
        [DefaultValue(true)]
        [Description("Gets or sets whether RadCalendar will display a header/navigation row.")]
        [NotifyParentProperty(true)]
        public virtual bool ShowHeader
        {
            get
            {
                return this.showHeader;
            }
            set
            {
                if (this.showHeader != value)
                {
                    this.showHeader = value;
                    this.OnNotifyPropertyChanged("ShowHeader");
                }
            }
        }

        /// <summary>Gets or sets whether the column headers will appear on the calendar.</summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Determines whether the column headers will appear on the calendar.")]
        [NotifyParentProperty(true)]
        public bool ShowColumnHeaders
        {
            get { return showColumnHeaders; }
            set
            {
                if (this.showColumnHeaders != value)
                {
                    this.showColumnHeaders = value;
                    this.OnNotifyPropertyChanged("ShowColumnHeaders");
                }
            }
        }

        /// <summary>Gets or sets whether the row headers will appear on the calendar.</summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Determines whether the row headers will appear on the calendar.")]
        [NotifyParentProperty(true)]
        public bool ShowRowHeaders
        {
            get { return showRowHeaders; }
            set
            {
                if (this.showRowHeaders != value)
                {
                    this.showRowHeaders = value;
                    this.OnNotifyPropertyChanged("ShowRowHeaders");
                }
            }
        }

        /// <summary>Gets or sets whether a single CalendarView object will display a header .</summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a single CalendarView object will display a header.")]
        [NotifyParentProperty(true)]
        public bool ShowViewHeader
        {
            get { return this.showViewHeader; }
            set
            {
                if (this.showViewHeader != value)
                {
                    this.showViewHeader = value;
                    this.OnNotifyPropertyChanged("ShowViewHeader");
                }
            }
        }

        /// <summary>Gets or sets whether a single CalendarView object will display a selector.</summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a single CalendarView object will display a selector.")]
        [NotifyParentProperty(true)]
        public bool ShowViewSelector
        {
            get { return this.showViewSelector; }
            set
            {
                if (this.showViewSelector != value)
                {
                    this.showViewSelector = value;
                    this.OnNotifyPropertyChanged("ShowViewSelector");
                }
            }
        }

        /// <summary> 
        /// Gets or sets whether the view selector will be allowed to select all dates presented by the CalendarView.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description(
           "Gets or sets whether the view selector will be allowed to select all dates presented by the CalendarView.")]
        [NotifyParentProperty(true)]
        public bool AllowViewSelector
        {
            get { return this.allowViewSelector; }
            set
            {
                if (this.allowViewSelector != value)
                {
                    this.allowViewSelector = value;
                    this.OnNotifyPropertyChanged("AllowViewSelector");
                }
            }
        }

        /// <summary>Gets or sets the zooming factor of a cell which is handled by the zooming (fish eye) functionality.</summary>
        [Description("Gets or sets the zooming factor of a cell which is handled by the zooming (fish eye) functionality.")]
        [DefaultValue(1.3f)]
        [Category("Behavior")]
        [NotifyParentProperty(true)]
        public virtual float ZoomFactor
        {
            get
            {
                return this.calendarElement.ZoomFactor;
            }
            set
            {
                this.calendarElement.ZoomFactor = value;
                this.OnNotifyPropertyChanged("ZoomFactor");

            }
        }

        /// <summary>Gets or sets whether the zooming functionality is enabled.</summary>
        [Description("Gets or sets whether the zooming functionality is enabled.")]
        [DefaultValue(false)]
        [Category("Behavior")]
        [NotifyParentProperty(true)]
        public virtual bool AllowFishEye
        {
            get
            {
                return this.calendarElement.AllowFishEye;
            }
            set
            {
                this.calendarElement.AllowFishEye = value;
                this.OnNotifyPropertyChanged("AllowFishEye");

            }
        }

        /// <summary>
        /// Gets or sets whether row headers ( if displayed by a <strong>MonthView</strong> object)
        /// will act as row selectors.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or sets whether row headers ( if displayed by a MonthView object) will act as row selectors.")]
        [NotifyParentProperty(true)]
        public bool AllowRowHeaderSelectors
        {
            get
            {
                return this.allowRowHeaderSelectors;
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
        [Description("Gets or sets whether column headers ( if displayed by a MonthView object) will act as column selectors.")]
        [NotifyParentProperty(true)]
        public bool AllowColumnHeaderSelectors
        {
            get
            {
                return this.allowColumnHeaderSelectors;
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

        #endregion

        #region Month Specific Settings

        /// <summary>
        /// Gets or sets whether the month matrix, when rendered will show days from other (previous or next)
        /// months or will render only blank cells.
        /// </summary>
        [Category("MonthView Specific Settings")]
        [DefaultValue(true)]
        [Description("Gets or sets whether the month matrix, when rendered will show days from other (previous or next) months or will render only blank cells.")]
        [NotifyParentProperty(true)]
        public bool ShowOtherMonthsDays
        {
            get { return showOtherMonthsDays; }
            set
            {
                if (this.showOtherMonthsDays != value)
                {
                    this.showOtherMonthsDays = value;
                    this.OnNotifyPropertyChanged("ShowOtherMonthsDays");
                }
            }
        }



        private bool CalculateValidTableLayout(int value, bool isColumn)
        {
            if (isColumn)
            {
                if (value != 7 && value != 14 && value != 21 && value != 3 && value != 6 && value != 2)
                    return false;


                if (Orientation == Orientation.Vertical)
                {
                    switch (value)
                    {
                        case 3:
                            this.singleViewColumns = value;
                            this.Rows = 14;
                            this.monthLayout = MonthLayout.Layout_14rows_x_3columns;

                            return true;
                        case 2:
                            this.singleViewColumns = value;
                            this.Rows = 21;
                            this.monthLayout = MonthLayout.Layout_21rows_x_2columns;

                            return true;
                        case 6:
                            this.singleViewColumns = value;
                            this.Rows = 7;
                            this.monthLayout = MonthLayout.Layout_7rows_x_6columns;
                            return true;
                    }
                }
                else
                {
                    switch (value)
                    {
                        case 7:
                            this.singleViewColumns = value;
                            this.Rows = 6;
                            this.monthLayout = MonthLayout.Layout_7columns_x_6rows;
                            return true;
                        case 14:
                            this.singleViewColumns = value;
                            this.Rows = 3;
                            this.monthLayout = MonthLayout.Layout_14columns_x_3rows;
                            return true;
                        case 21:
                            this.singleViewColumns = value;
                            this.Rows = 2;
                            this.monthLayout = MonthLayout.Layout_21columns_x_2rows;
                            return true;
                    }
                }



            }
            else
            {
                if (value != 7 && value != 14 && value != 21 && value != 3 && value != 6 && value != 2)
                    return false;


                if (this.orientation == Orientation.Vertical)
                {
                    switch (value)
                    {
                        case 7:
                            this.singleViewRows = value;
                            this.Columns = 6;
                            return true;
                        case 14:
                            this.singleViewRows = value;
                            this.Columns = 3;
                            return true;
                        case 21:
                            this.singleViewRows = value;
                            this.Columns = 2;
                            return true;
                    }
                }
                else
                    switch (value)
                    {

                        case 3:
                            this.singleViewRows = value;
                            this.Columns = 14;
                            return true;
                        case 2:
                            this.singleViewRows = value;
                            this.Columns = 21;
                            return true;
                        case 6:
                            this.singleViewRows = value;
                            this.Columns = 7;
                            return true;
                    }

            }

            return false;
        }

        /// <summary>
        /// 	<para>Gets or sets the predefined pairs of rows and columns, so that the product of
        ///     the two values is exactly 42, which guarantees valid calendar layout. It is applied
        ///     on a single view level to every MonthView instance in the calendar.</para>
        /// </summary>
        /// <remarks>
        /// 	<para>The following values are applicable and defined in the MonthLayout
        ///     enumeration:<br/>
        /// 		<br/>
        ///     Layout_7columns_x_6rows - horizontal layout<br/>
        /// 		<br/>
        ///     Layout_14columns_x_3rows - horizontal layout<br/>
        /// 		<br/>
        ///     Layout_21columns_x_2rows - horizontal layout<br/>
        /// 		<br/>
        ///     Layout_7rows_x_6columns - vertical layout, required when AllowColumnHeaderSelectors is true and
        ///     Orientation is set to Vertical.<br/>
        /// 		<br/>
        ///     Layout_14rows_x_3columns - vertical layout, required when AllowColumnHeaderSelectors 
        ///     is true and Orientation is set to Vertical.<br/>
        /// 		<br/>
        ///     Layout_21rows_x_2columns - vertical layout, required when AllowColumnHeaderSelectors is true and Orientation
        ///     is set to Vertical.</para>
        /// </remarks>
        [NotifyParentProperty(true)]
        [Category("Month View Settings")]
        [DefaultValue(MonthLayout.Layout_7columns_x_6rows)]
        [Description("This property allows using presets, regarding the layout of the calendar area. Sets or gets predefined pairs of rows and columns, so that the product of the two values is exactly 42, which guarantees valid calendar layout.")]
        [RefreshProperties(RefreshProperties.All)]
        public MonthLayout MonthLayout
        {
            get
            {
                if (monthLayout.HasValue)
                {
                    return monthLayout.Value;
                }
                return MonthLayout.Layout_7columns_x_6rows;
            }
            set
            {

             //   this.MultiViewColumns == this.MultiViewRows
                if (this.monthLayout != value)
                {
                    this.monthLayout = value;
                    this.SynchronizeMonthView();
                    this.OnNotifyPropertyChanged("MonthLayout");
                }
            }
        }

        [Browsable(false)]
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Browsable(false)]
        public new Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        #endregion

        #region Headers/Selectors Settings

        /// <remarks>
        /// 	<para>Use the <strong>RowHeaderText</strong> property to provide custom text for
        ///     all row header elements.</para>
        /// </remarks>
        /// <value>
        /// The text displayed for all <strong>CalendarView</strong> row header elements. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the text displayed for all row header elements.
        /// </summary>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("Provides custom text for all row header elements.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string RowHeaderText
        {
            get { return this.rowHeaderText; }
            set
            {
                if (this.rowHeaderText != value)
                {
                    this.rowHeaderText = value;
                    this.OnNotifyPropertyChanged("RowHeaderText");
                }
            }
        }

        /// <value>
        /// The image displayed for all <strong>CalendarView</strong> row header elements. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the image displayed for all row header elements.
        /// </summary>
        [Category("Header Settings")]
        [DefaultValue(null)]
        [Description("The image displayed for all <strong>CalendarView</strong> row header elements.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image RowHeaderImage
        {
            get { return rowHeaderImage; }
            set
            {
                if (this.rowHeaderImage != value)
                {
                    this.rowHeaderImage = value;
                    this.OnNotifyPropertyChanged("RowHeaderImage");
                }

            }

        }


        [DefaultValue(false)]
        internal bool RTL
        {
            get
            {
                return
                     this.rightToLeft;
            }

            set
            {
                if (this.rightToLeft != value)
                {
                    this.rightToLeft = value;
                    this.OnNotifyPropertyChanged("RightToLeft");
                }
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            System.Windows.Forms.RightToLeft value = base.RightToLeft;
            if (value == RightToLeft.Inherit)
            {
                Control rtlSource = this.Parent;
                while (rtlSource != null && rtlSource.RightToLeft == RightToLeft.Inherit)
                {
                    rtlSource = rtlSource.Parent;
                }
                value = (rtlSource != null) ? rtlSource.RightToLeft : RightToLeft.No;

                if (value == RightToLeft.Yes)
                    RTL = true;
                else
                    RTL = false;
            }
            if (value == RightToLeft.No)
            {
                RTL = false;
            }
            else if (value == RightToLeft.Yes)
            {
                RTL = true;
            }

            base.OnRightToLeftChanged(e);
            this.InvalidateCalendar();
        }

        /// <remarks>
        /// 	<para>Use the <strong>ColumnHeaderText</strong> property to provide custom text
        ///     for all <strong>CalendarView</strong> column header elements.</para>
        /// </remarks>
        /// <value>
        /// The text displayed for all <strong>CalendarView</strong> column header elements. The default value is <b>""</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the text displayed for all column header elements.
        /// </summary>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("")]
        [Description("Provides custom text for all column header elements.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string ColumnHeaderText
        {
            get { return columnHeaderText; }
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
        /// The image displayed for all <strong>CalendarView</strong> column header elements. The default value is <b>null</b>.
        /// </value>
        /// <summary>
        /// Gets or sets the image displayed for all column header elements.
        /// </summary>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue(null)]
        [Description("The image displayed for all column header elements.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image ColumnHeaderImage
        {
            get { return columnHeaderImage; }
            set
            {
                if (this.columnHeaderImage != value)
                {
                    this.columnHeaderImage = value;
                    this.OnNotifyPropertyChanged("ColumnHeaderImage");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the text displayed for the view selector.</para>
        /// </summary>
        /// <value>
        /// The text displayed for the view selector. The default value is <b>"x"</b>.
        /// </value>
        /// <remarks>
        /// 	<para>Use the <strong>ViewSelectorText</strong> property to provide custom text for
        ///     the <strong>CalendarView</strong> selector element.</para>
        /// </remarks>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue("x")]
        [Description("The text displayed in the view selector element.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string ViewSelectorText
        {
            get { return viewSelectorText; }
            set
            {
                if (this.viewSelectorText != value)
                {
                    this.viewSelectorText = value;
                    this.OnNotifyPropertyChanged("ViewSelectorText");
                }
            }
        }

        /// <summary>
        /// 	<para>Gets or sets the image displayed for the view selector element.</para>
        /// </summary>
        /// <value>
        /// The image displayed for the <strong>CalendarView</strong> selector element. The default value is <b>null</b>.
        /// </value>
        [Bindable(false)]
        [Category("Header Settings")]
        [DefaultValue(null)]
        [Description("The image displayed in the view selector element.")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public Image ViewSelectorImage
        {
            get { return viewSelectorImage; }
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
            get { return this.orientation; }
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
        /// Gets or sets an integer value representing the number of CalendarView
        /// views that will be scrolled when the user clicks on a fast navigation button.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(3)]
        [Description("Gets or sets the number of views that will be scrolled when the user clicks on a fast navigation button.")]
        [NotifyParentProperty(true)]
        public int FastNavigationStep
        {
            get { return fastNavigationStep; }
            set
            {
                if (this.fastNavigationStep != value)
                {
                    this.fastNavigationStep = value;
                    this.OnNotifyPropertyChanged("FastNavigationStep");
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// A collection of special days in the calendar to which may be applied specific formatting.
        /// </summary>
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("A collection of special days in the calendar to which may be applied specific formatting.")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public CalendarDayCollection SpecialDays
        {
            get
            {
                if (specialDays == null)
                {
                    specialDays = new CalendarDayCollection(this);
                }
                return specialDays;
            }
        }

        /// <summary>
        /// Gets or sets the padding of the calendar cells.
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the padding of the calendar cells.")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Padding CellPadding
        {
            get
            {
                return this.cellPadding;
            }
            set
            {
                this.cellPadding = value;
                this.OnNotifyPropertyChanged("CellPadding");
            }
        }

        /// <summary>
        /// Gets or sets the vertical spacing between the calendar cells.
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the vertical spacing between the calendar cells.")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(1)]
        public int CellVerticalSpacing
        {
            get
            {
                return this.cellVerticalSpacing;
            }
            set
            {
                this.cellVerticalSpacing = value;
                this.OnNotifyPropertyChanged("CellVerticalSpacing");
            }
        }

        /// <summary>
        /// Gets or sets the horizontal spacing between the calendar cells.
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the horizontal spacing between the calendar cells.")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        [DefaultValue(1)]
        public int CellHorizontalSpacing
        {
            get
            {
                return this.cellHorizontalSpacing;
            }
            set
            {
                this.cellHorizontalSpacing = value;
                this.OnNotifyPropertyChanged("CellHorizontalSpacing");
            }
        }

        /// <summary>
        /// Gets or sets the margin of the calendar cells.
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the margin of the calendar cells.")]
        [NotifyParentProperty(true)]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public Padding CellMargin
        {
            get
            {
                return this.cellMargin;
            }
            set
            {
                this.cellMargin = value;
                this.OnNotifyPropertyChanged("CellMargin");
            }
        }

        /// <summary>
        /// 	<para>Exposes the top instance of <strong>CalendarView</strong> or its derived types.</para>
        /// 	<para>Every <strong>CalendarView</strong> class handles the real calculation and
        ///     rendering of <strong>RadCalendar</strong>'s calendric information. The
        ///     <strong>CalendarView</strong> has the ChildViews collection which contains all the sub views in case of a multi view
        ///     setup.</para>
        /// </summary>
        [Browsable(false)]
        [Category("Data")]
        [Description("Exposes the top instance of CalendarView or its derived types.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarView DefaultView
        {
            get
            {
                if (this.calendarView == null)
                {
                    return this.EnsureDefaultView();
                }
                return this.calendarView;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Removes the time component of a DateTime object, thus leaving only the date part.
        /// </summary>
        /// <param name="date">the DateTime object to be processed.</param>
        /// <returns>the DateTime object containing only the date part of the original DateTime object.</returns>
        protected static DateTime TruncateTimeComponent(DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// Ensures that a valid CalendarView object is instantiated and used by RadCalendar as default view.
        /// </summary>
        /// <returns>The CalendarView object to be used as default view.</returns>
        protected virtual CalendarView EnsureDefaultView()
        {
            if (this.multiViewColumns > 1 || this.multiViewRows > 1)
            {
                this.calendarView = new MultiMonthView(this);
            }
            else
            {
                this.calendarView = new MonthView(this);
            }

            this.calendarView.Initialize();
            return this.calendarView;
        }

        private void SynchronizeMonthView()
        {
            switch (this.monthLayout)
            {
                case MonthLayout.Layout_7columns_x_6rows:
                    this.Rows = 6;
                    this.Columns = 7;
                    break;
                case MonthLayout.Layout_14columns_x_3rows:
                    this.Rows = 3;
                    this.Columns = 14;
                    break;
                case MonthLayout.Layout_21columns_x_2rows:
                    this.Rows = 2;
                    this.Columns = 21;
                    break;
                case MonthLayout.Layout_7rows_x_6columns:
                    this.Rows = 7;
                    this.Columns = 6;
                    break;
                case MonthLayout.Layout_14rows_x_3columns:
                    this.Rows = 14;
                    this.Columns = 3;
                    break;
                case MonthLayout.Layout_21rows_x_2columns:
                    this.Rows = 21;
                    this.Columns = 2;
                    break;
                default:
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!this.DefaultView.HandleKeyDown(keyData))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        /// <summary>
        /// Explicitely invalidates RadCalendar layout. Can be used when batch updates to calendar properties are made
        /// outside of the control that require control invalidation.
        /// </summary>
        public void InvalidateCalendar()
        {
            this.CalendarElement.RefreshVisuals(true);
            this.Invalidate();
        }

        internal void InvalidateCalendarSelection()
        {
            if (this.CalendarElement.CalendarVisualElement as MultiMonthViewElement != null)
            {
                MultiMonthViewElement multiMonth = this.CalendarElement.CalendarVisualElement as MultiMonthViewElement;

                foreach (MonthViewElement currentMonth in multiMonth.Children[0].Children[1].Children)
                {
                    currentMonth.TableElement.RefreshSelectedDates();
                }
            }
            else if (this.CalendarElement.CalendarVisualElement as MonthViewElement != null)
            {
                MonthViewElement month = this.CalendarElement.CalendarVisualElement as MonthViewElement;
                month.TableElement.RefreshSelectedDates();
            }

            if (this.DesignMode && this.calendarElement != null)
            {
                this.calendarElement.RefreshVisuals(true);
                this.calendarElement.Invalidate();
            }
        }

        internal void SetFocusedDateView()
        {
            if (this.DefaultView.IsDateInView(this.FocusedDate))
            {
                this.CalendarElement.RefreshVisuals(true);
                this.Invalidate();
            }
            else
            {
                CalendarView newView = this.DefaultView.CreateView(this.FocusedDate);

                ViewChangingEventArgs args = this.OnViewChanging(newView);
                if (args.Cancel)
                {
                    return;
                }

                this.SetCalendarView(newView);
                this.CalendarElement.CalendarVisualElement.View = newView;

                this.OnViewChanged();

                this.CalendarElement.CalendarNavigationElement.Text = newView.GetTitleContent();
            }

            this.Invalidate();
        }

        internal void SetCalendarView(CalendarView inputView)
        {
            this.calendarView = inputView;
        }

        internal CalendarView GetNewViewFromStep(int navigationStep)
        {
            CalendarView newView = null;
            bool isNegative = false;
            if (navigationStep < 0)
            {
                navigationStep = -navigationStep;
                isNegative = true;
            }
            if (isNegative)
            {
                newView = this.DefaultView.GetPreviousView(navigationStep);

            }
            else
            {
                newView = this.DefaultView.GetNextView(navigationStep);
            }
            return newView;
        }

        private void ReInitializeCalendarElement()
        {
            this.calendarElement.ReInitializeChildren();
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FocusedDate":
                    this.SetFocusedDateView();
                    break;
                case "RowHeaderText":
                case "RowHeaderImage":
                case "ColumnHeaderText":
                case "ColumnHeaderImage":
                case "DayNameFormat":
                case "CellAlign":
                    this.CalendarElement.RefreshVisuals(true);
                    break;
                case "HostedControl":
                    break;
                case "HostedItem":
                    break;
                case "TreeView":
                    break;
                case "LoadedOnDemand":
                    break;
                case "FirstDayOfWeek":
                    this.InvalidateCalendar();
                    CalendarView newView = this.DefaultView.CreateView(this.FocusedDate);

                    ViewChangingEventArgs args = this.OnViewChanging(newView);
                    if (args.Cancel)
                    {
                        return;
                    }
                    this.SetCalendarView(newView);
                    this.CalendarElement.CalendarVisualElement.View = newView;

                    this.OnViewChanged();

                    this.CalendarElement.CalendarNavigationElement.Text = newView.GetTitleContent();
                    break;
                case "SelectedDate":
                    this.CalendarElement.RefreshVisuals(true);
                    break;
                case "MonthLayout":
                    this.DefaultView.Initialize();
                    this.InvalidateCalendar();
                    break;
                case "Culture":
                case "CultureID":
                    this.DefaultView.Initialize();
                    this.InvalidateCalendar();
                    break;
                case "ShowFooter":
                    if (this.ShowFooter)
                    {
                        this.CalendarElement.CalendarStatusElement.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        this.CalendarElement.CalendarStatusElement.Visibility = ElementVisibility.Collapsed;
                    }

                    break;
                case "TitleAlign":
                    this.CalendarElement.RefreshVisuals(true);
                    this.CalendarElement.InvalidateArrange();
                    this.CalendarElement.InvalidateMeasure();
                    this.CalendarElement.UpdateLayout();
                    break;
                case "AllowSelect":
                    this.ClearButton.Visibility = this.AllowSelect ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                    break;
                case "AllowMultiSelect":
                    if (!this.AllowMultipleSelect)
                    {
                        this.BeginUpdate();
                        DateTime time = this.SelectedDate;
                        this.SelectedDate = time;
                        this.EndUpdate();
                    }

                    break;
                case "Orientation":
                    if ((this.Rows == 6 && this.Columns == 7) ||
                        (this.singleViewColumns.HasValue && this.singleViewRows.HasValue))
                    {
                        if (this.singleViewColumns == null)
                        {
                            this.singleViewColumns = 7;
                        }

                        if (this.singleViewRows == null)
                        {
                            this.singleViewRows = 6;
                        }

                        int tmp = this.singleViewRows.Value;
                        this.singleViewRows = this.singleViewColumns.Value;
                        this.singleViewColumns = tmp;

                        switch (this.Rows)
                        {
                            case 6:
                                this.monthLayout = MonthLayout.Layout_7columns_x_6rows;
                                break;
                            case 7:
                                this.monthLayout = MonthLayout.Layout_7rows_x_6columns;
                                break;
                            case 14:
                                this.monthLayout = MonthLayout.Layout_14rows_x_3columns;
                                break;
                            case 3:
                                this.monthLayout = MonthLayout.Layout_14columns_x_3rows;
                                break;
                            case 21:
                                this.monthLayout = MonthLayout.Layout_21rows_x_2columns;
                                break;
                            case 2:
                                this.monthLayout = MonthLayout.Layout_21columns_x_2rows;
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            base.OnNotifyPropertyChanged(e);
        }

        internal void BeginUpdate()
        {
            this.updating = true;
        }

        internal void EndUpdate()
        {
            this.updating = false;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.calendarElement = new RadCalendarElement(this, this.DefaultView);
            parent.Children.Add(this.calendarElement);
            this.TitleAlign = System.Windows.Forms.VisualStyles.ContentAlignment.Center;
        } 
        #endregion 

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadCalendarAccessibleObject(this);
        }
    }
}
