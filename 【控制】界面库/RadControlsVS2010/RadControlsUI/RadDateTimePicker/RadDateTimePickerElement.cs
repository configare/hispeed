using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the RadDateTimePickerElement class
    /// </summary>
    public class RadDateTimePickerElement : RadEditorElement
    {
        #region BitState Keys
        internal const ulong ShowCheckBoxStateKey = RadItemLastStateKey << 1;
        internal const ulong ShowUpDownStateKey = ShowCheckBoxStateKey << 1;
        internal const ulong CheckStateStateKey = ShowUpDownStateKey << 1;
        internal const ulong ValidTimeStateKey = CheckStateStateKey << 1;
        internal const ulong UserHasSetValueStateKey = ValidTimeStateKey << 1;
        internal const ulong ShowCurrentTimeStateKey = UserHasSetValueStateKey << 1;

        #endregion

        #region Fields
        private DateTime creationTime;
        private string customFormat;
        private DateTime min;
        private DateTime max;
        private DateTime? _value;
        private RadDateTimePickerBehaviorDirector defaultDirector;
        internal RadCheckBoxElement checkBox;
        private Size calendarSize = new Size(100, 156);
        private CultureInfo cultureInfo = null;
        private RadDateTimePickerBehaviorDirector currentBehavior;
        private string nullText = string.Empty;
        private DateTimePickerFormat format = DateTimePickerFormat.Long;
        private Point calendarLocation;

        public RadDateTimePickerBehaviorDirector CurrentBehavior
        {
            get
            {
                return currentBehavior;
            }
            set
            {
                currentBehavior = value;
            }
        }

        /// <summary>
        /// Occurs when the drop down is opened
        /// </summary>
        [Description("Occurs when the drop down is opened")]
        [Category("Action")]
        public event EventHandler Opened;

        /// <summary>
        /// Occurs when the drop down is opening
        /// </summary>
        [Description("Occurs when the drop down is opening")]
        [Category("Action")]
        public event CancelEventHandler Opening;

        /// <summary>
        /// Occurs when the drop down is closing
        /// </summary>
        [Description("Occurs when the drop down is closing")]
        [Category("Action")]
        public event RadPopupClosingEventHandler Closing;

        /// <summary>
        /// Occurs when the drop down is closed
        /// </summary>
        [Description("Occurs when the drop down is closed")]
        [Category("Action")]
        public event RadPopupClosedEventHandler Closed;

        /// <summary>
        /// Gets the maximum date value allowed for the DateTimePicker control.
        /// </summary>
        [Description("Gets the maximum date value allowed for the DateTimePicker control.")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MaxDateTime;

        /// <summary>
        /// Gets the minimum date value allowed for the DateTimePicker control.
        /// </summary>
        [Description("Gets the minimum date value allowed for the DateTimePicker control.")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MinDateTime;

        private DateTime _nullDate;


        /// <summary>
        /// Occurs when the value of the control has changed
        /// </summary>
        [Description("Occurs when the value of the control has changed"), Category("Action")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when the format of the control has changed
        /// </summary>
        [Description("Occurs when the value of the control has changed"), Category("Action")]
        public event EventHandler FormatChanged;

        /// <summary>
        /// Occurs when the value of the control is changing
        /// </summary>
        [Description("Occurs when the value of the control is changing"), Category("Action")]
        public event ValueChangingEventHandler ValueChanging;

        #endregion

        #region Constructors
        /// <summary>
        /// Represents RadDateTimePickerElement's constructor
        /// </summary>
        public RadDateTimePickerElement()
        {
        }

        static RadDateTimePickerElement()
        {
            MinDateTime = new DateTime(0x6d9, 1, 1);
            MaxDateTime = new DateTime(0x270e, 12, 0x1f);
            new Themes.ControlDefault.DateTimePicker().DeserializeTheme();
        }

        /// <summary>
        /// Represents RadDateTimePickerElement's constructor
        /// </summary>
        /// <param name="behaviorDirector"></param>
        public RadDateTimePickerElement(RadDateTimePickerBehaviorDirector behaviorDirector)
        {
            this.defaultDirector = behaviorDirector;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether a spin box rather than a drop down calendar is displayed for editing the control's value
        /// </summary>
        [Description("Indicates whether a spin box rather than a drop down calendar is displayed for editing the control's value")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowUpDown
        {
            get
            {
                return this.GetBitState(ShowUpDownStateKey);
            }
            set
            {
                this.SetBitState(ShowUpDownStateKey, value);
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
        [RefreshProperties(RefreshProperties.Repaint),
        Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        /// <summary>
        /// Gets the default behavior of the control
        /// </summary>
        [Browsable(false)]
        [Category("Behavior")]
        [Description("Gets the default behavior of the control")]
        [DefaultValue(null)]
        public RadDateTimePickerBehaviorDirector DefaultBehavior
        {
            get
            {
                if (this.defaultDirector != null)
                {
                    return this.defaultDirector;
                }

                this.currentBehavior = new RadDateTimePickerCalendar(this);
                return this.currentBehavior;
            }
        }

        /// <summary>
        /// Gets the default null date
        /// </summary>
        protected virtual DateTime DefaultNullDate
        {
            get
            {
                return DateTime.MinValue; //new DateTime(1901,7,6);
            }
        }

        /// <summary>
        /// The DateTime value assigned to the date picker when the Value is null
        /// </summary>
        [Category("Data"), Description("The DateTime value assigned to the date picker when the Value is null"), Bindable(false)]
        public DateTime NullDate
        {
            get
            {
                if (this._nullDate != this.DefaultNullDate)
                {
                    return this._nullDate;
                }

                return this.DefaultNullDate;
            }
            set
            {
                if (!DateTime.Equals(this._nullDate, value))
                {
                    this._nullDate = value;
                }
               
            }
        }

        /// <summary>
        /// Gets an instance of RadTextBoxElement
        /// </summary>
        [Browsable(false)]
        public RadMaskedEditBoxElement TextBoxElement
        {
            get
            {
                if ((this.GetCurrentBehavior() as RadDateTimePickerCalendar) != null)
                {
                    return (this.GetCurrentBehavior() as RadDateTimePickerCalendar).TextBoxElement;
                }

                if ((this.GetCurrentBehavior() as RadDateTimePickerSpinEdit) != null)
                {
                    return (this.GetCurrentBehavior() as RadDateTimePickerSpinEdit).TextBoxElement;
                }

                return null;
            }
        }

        internal RadArrowButtonElement ArrowButton
        {
            get
            {
                if ((this.GetCurrentBehavior() as RadDateTimePickerCalendar) != null)
                {
                    return (this.GetCurrentBehavior() as RadDateTimePickerCalendar).ArrowButton;
                }

                return null;
            }
        }

        internal bool IsDropDownShown
        {
            get
            {
                return (bool)this.GetValue(RadDateTimePickerElement.IsDropDownShownProperty);
            }
            set
            {
                this.SetValue(RadDateTimePickerElement.IsDropDownShownProperty, value);
            }
        }

        /// <summary>
        /// When ShowCheckBox is true, determines that the user has selected a value
        /// </summary>
        [Description("When ShowCheckBox is true, determines that the user has selected a value")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return this.GetBitState(CheckStateStateKey);
            }
            set
            {
                this.SetBitState(CheckStateStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom date/time format string.
        /// </summary>
        [DefaultValue((string)null), RefreshProperties(RefreshProperties.Repaint), Description("Gets or sets the custom date/time format string."), Localizable(true), Category("Behavior")]
        public string CustomFormat
        {
            get
            {
                return this.customFormat;
            }
            set
            {
                if (((value != null) && !value.Equals(this.customFormat)) || ((value == null) && (this.customFormat != null)))
                {
                    this.customFormat = value;
                    this.OnNotifyPropertyChanged("CustomFormat");
                }
            }
        }

        /// <summary>
        /// Gets or sets the format of the date and time displayed in the control.
        /// </summary>
        [DefaultValue(typeof(DateTimePickerFormat), "DateTimePickerFormat.Long")]
        [Description("Gets or sets the format of the date and time displayed in the control."), RefreshProperties(RefreshProperties.Repaint), Category("Appearance")]
        public DateTimePickerFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, 1, 8, 1))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DateTimePickerFormat));
                }
                if (this.format != value)
                {
                    this.format = value;
                    this.OnFormatChanged(EventArgs.Empty);
                    this.OnNotifyPropertyChanged("Format");
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the drop down showing the calendar
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Browsable(false)]
        [Description("Gets or sets the location of the drop down showing the calendar")]
        [Category("Behavior")]
        public Point CalendarLocation
        {
            get
            {
                return this.calendarLocation;
            }
            set
            {
                if (this.calendarLocation != value)
                {
                    this.calendarLocation = value;
                    this.OnNotifyPropertyChanged("CalendarLocation");
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the calendar in the drop down
        /// </summary>
        [DefaultValue(typeof(Size), "100, 156")]
        [Browsable(false)]
        [Description("Gets or sets the size of the calendar in the drop down")]
        [Category("Behavior")]
        public Size CalendarSize
        {
            get
            {
                return this.calendarSize;
            }
            set
            {
                if (this.calendarSize != value)
                {
                    this.calendarSize = value;
                    this.OnNotifyPropertyChanged("CalendarSize");
                }
            }
        }

        /// <summary>
        /// Indicates whether a check box is displayed in the control. When the check box is unchecked no value is selected
        /// </summary>
        [Description("Indicates whether a check box is displayed in the control. When the check box is unchecked no value is selected")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get
            {
                return this.GetBitState(ShowCheckBoxStateKey);
            }
            set
            {
                this.SetBitState(ShowCheckBoxStateKey, value);
            }
        }

        /// <summary>
        ///Gets or sets whether the current time is shown.
        /// </summary>
        [Bindable(true), RefreshProperties(RefreshProperties.All),
        Description("Gets or sets whether the current time is shown"),
        Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCurrentTime
        {
            get
            {
                return this.GetBitState(ShowCurrentTimeStateKey);
            }
            set
            {
                this.SetBitState(ShowCurrentTimeStateKey, value);
            }
        }

        private bool internalValueSet = false;
        /// <summary>
        /// Gets or sets the date/time value assigned to the control.
        /// </summary>
        [Bindable(true),
        RefreshProperties(RefreshProperties.All),
        Description("Gets or sets the date/time value assigned to the control."),
        Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? Value
        {
            get
            {
                if (!this.GetBitState(UserHasSetValueStateKey) && this.GetBitState(ValidTimeStateKey))
                {
                    return this.creationTime;
                }

                return this._value;
            }
            set
            {
                if (this.internalValueSet)
                {
                    return;
                }

                bool flag = !DateTime.Equals(this.Value, value);

                if ( value.HasValue && (value.Value.Date.Equals(this.NullDate.Date)))// || value.Value.Date.Equals(this.MinDate)))
                {
                    //this.internalValueSet = true;
                    //this.CurrentBehavior.DateTimePickerElement.TextBoxElement.Value = null;
                    //this.CurrentBehavior.DateTimePickerElement.Text = "";
                    //this.internalValueSet = false;
                    this.CurrentBehavior.DateTimePickerElement.TextBoxElement.TextBoxItem.Text = "";
                    this._value = NullDate.Date;

                    return;
                }
              
                
                if (flag || value == this.NullDate)
                {
                    if (value != NullDate )
                    {
                        if (value < this.MinDate)
                        {
                            value = this.MinDate;
                        }
                        else if (value > this.MaxDate)
                        {
                            value = this.MaxDate;
                        }
                    }
                     
                    ValueChangingEventArgs args = new ValueChangingEventArgs(value, this._value);
                    this.OnValueChanging(args);
                    if (args.Cancel)
                    {
                        this.CurrentBehavior.SetDateByValue(this._value, String.IsNullOrEmpty(this.customFormat) ? this.format : DateTimePickerFormat.Custom);
                        return;
                    }
                    
                    string text = this.Text;
                    this._value = value;
                    this.CurrentBehavior.SetDateByValue(value, String.IsNullOrEmpty(this.customFormat) ? this.format : DateTimePickerFormat.Custom);

                    this.BitState[UserHasSetValueStateKey] = true;

                    if (flag)
                    {
                        this.OnValueChanged(EventArgs.Empty);
                    }
                    if (!text.Equals(this.Text))
                    {
                        this.OnTextChanged(EventArgs.Empty);
                    }          
                    

                    this.OnNotifyPropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the DateTimePicker contains a null 
        /// reference.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the text that is displayed when the DateTimePicker contains a null reference")]
        public string NullText
        {
            get
            {
                return this.nullText;
            }

            set
            {
                if (this.nullText != value)
                {
                    this.nullText = value;
                    this.OnNotifyPropertyChanged("NullText");
                }
            }
        }

        /// <summary>
        /// Gets the maximum date value allowed for the DateTimePicker control.
        /// </summary>
        [Description("Gets the maximum date value allowed for the DateTimePicker control.")]
        public static DateTime MaximumDateTime
        {
            get
            {
                return MaxDateTime;
            }
        }

        /// <summary>
        /// Gets or sets the delay of auto updating when typing in RadDateTimePicker
        /// </summary>
        [Description("Gets or sets the delay of auto updating when typing in RadDateTimePicker")]
        [Obsolete]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AutoUpdateDelay
        {
            get
            {               
                return 0;
            }
            set
            {
                
            }
        }

        /// <summary>
        /// Gets the minimum date value allowed for the DateTimePicker control.
        /// </summary>
        [Description("Gets the minimum date value allowed for the DateTimePicker control.")]
        public static DateTime MinimumDateTime
        {
            get
            {
                DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
                //if (minSupportedDateTime.Year < 1753)
                //{
                //    return new DateTime(1753, 1, 1);
                //}
                return minSupportedDateTime;
            }
        }

        /// <summary>
        /// Gets or sets the minimum date and time that can be selected in the control.
        /// </summary>
        [Description("Gets or sets the minimum date and time that can be selected in the control."), Category("Behavior")]
        public DateTime MinDate
        {
            get
            {
                return EffectiveMinDate(this.min);
            }
            set
            {
                if (value != this.min)
                {
                    if (value > EffectiveMaxDate(this.max))
                    {
                        throw new Exception("value is higher than the maximum available value");
                    }
                    if (value < MinimumDateTime)
                    {
                        throw new Exception("value is lower than the minimum available value");
                    }
                    this.min = value;
                    if (this.Value < this.min)
                    {
                        this.Value = this.min;
                    }

                    //this.NullDate = value;
                    this.OnNotifyPropertyChanged("MinDate");
                    this.currentBehavior.DateTimePickerElement.TextBoxElement.minDate = this.min;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum date and time that can be selected in the control.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets the maximum date and time that can be selected in the control.")]
        public DateTime MaxDate
        {
            get
            {
                return EffectiveMaxDate(this.max);
            }
            set
            {
                if (value != this.max)
                {
                    if (value < EffectiveMinDate(this.min))
                    {
                        throw new ArgumentOutOfRangeException("MaxDate cannot be lower than the min date");
                    }
                    if (value > MaximumDateTime)
                    {
                        throw new ArgumentOutOfRangeException("MaxDate cannot be higher than the max date");
                    }
                    this.max = value;
                    if (this.Value > this.max)
                    {
                        this.Value = this.max;
                    }

                    this.OnNotifyPropertyChanged("MaxDate");
                    this.currentBehavior.DateTimePickerElement.TextBoxElement.maxDate = this.max;
                }
            }
        }

        #endregion

        #region RadProperties
        /// <summary>
        /// Represents the IsDropDownShown dependancy property
        /// </summary>
        public static RadProperty IsDropDownShownProperty = RadProperty.Register(
            "IsDropDownShown", typeof(bool), typeof(RadDateTimePickerElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        #endregion

        #region Methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
            //fixed very strange bug with text 
            //TFS ID# 106558

            string oldText = this.TextBoxElement.Text;
            this.TextBoxElement.TextBoxItem.Text = string.Empty;
            this.TextBoxElement.TextBoxItem.Text = oldText;            
        }

        protected virtual void OnClosed(RadPopupClosedEventArgs args)
        {
            if (this.Closed != null)
            {
                this.Closed(this, args);
            }
        }

        protected virtual void OnOpened(EventArgs args)
        {
            if (this.Opened != null)
            {
                this.Opened(this, args);
            }
        }

        protected virtual void OnOpening(CancelEventArgs args)
        {
            if (this.Opening != null)
            {
                this.Opening(this, args);
            }
        }

        protected virtual void OnClosing(RadPopupClosingEventArgs args)
        {
            if (this.Closing != null)
            {
                this.Closing(this, args);
            }
        }

        internal static DateTime EffectiveMaxDate(DateTime maxDate)
        {
            DateTime maximumDateTime = MaximumDateTime;
            if (maxDate > maximumDateTime)
            {
                return maximumDateTime;
            }
            return maxDate;
        }

        internal static DateTime EffectiveMinDate(DateTime minDate)
        {
            DateTime minimumDateTime = MinimumDateTime;
            if (minDate < minimumDateTime)
            {
                return minimumDateTime;
            }
            return minDate;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;

            this.BitState[ValidTimeStateKey] = true;
            this._value = DateTime.Now;
            this.creationTime = DateTime.Now;
            this.max = DateTime.MaxValue;
            this.min = DateTime.MinValue;
            this.format = DateTimePickerFormat.Long;
            this._nullDate = this.DefaultNullDate;

            this.CanFocus = false;
        }

        protected override void DisposeManagedResources()
        {
            if (this.currentBehavior as RadDateTimePickerCalendar != null)
            {
                RadDateTimePickerCalendar calendarBehavior = this.currentBehavior as RadDateTimePickerCalendar;
                if (calendarBehavior.PopupControl != null)
                {
                    calendarBehavior.PopupControl.Opened -= new EventHandler(PopupControl_Opened);
                    calendarBehavior.PopupControl.Opening -= new CancelEventHandler(PopupControl_Opening);
                    calendarBehavior.PopupControl.Closing -= new RadPopupClosingEventHandler(PopupControl_Closing);
                    calendarBehavior.PopupControl.Closed -= new RadPopupClosedEventHandler(PopupControl_Closed);
                }

                RadTextBoxElement textBoxElement = this.TextBoxElement;
                if (textBoxElement != null)
                {
                    RadTextBoxItem textBoxItem = textBoxElement.TextBoxItem;
                    if (textBoxItem != null && textBoxItem.HostedControl != null)
                    {
                         (this.currentBehavior as RadDateTimePickerCalendar).MaskBoxElement.ValueChanged += new EventHandler(RadDateTimePickerElement_ValueChanged);
          
                    }
                }
            }

            if (this.checkBox != null)
                this.checkBox.ToggleStateChanged -= new StateChangedEventHandler(checkBox_ToggleStateChanged);

            IDisposable disposable = this.defaultDirector as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }

            disposable = this.currentBehavior as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }

            this.currentBehavior = null;
            this.defaultDirector = null;
            base.DisposeManagedResources();
        }

        /// <summary>
        /// Raises the FormatChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFormatChanged(EventArgs e)
        {
            if (this.FormatChanged != null)
            {
                this.FormatChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the ValueChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the ValueChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        public void CallOnValueChanged(EventArgs e)
        {
            this.OnValueChanged(e);
        }

        public void CallOnValueChanging(ValueChangingEventArgs e)
        {
            this.OnValueChanging(e);
        }

        public bool ShouldSerializeNullText()
        {
            if (String.IsNullOrEmpty(nullText))
                return false;

            return true;
        }

        public void ResetNullText()
        {
            nullText = "";
        }

        /// <summary>
        /// Gets the date as a string
        /// </summary>
        /// <returns>string value</returns>
        public override string ToString()
        {
            if (this.Value.HasValue)
            {
                return base.ToString() + ", Value: " + FormatDateTime(this.Value.Value);
            }

            return base.ToString() + ", Value: NULL";
        }

        private static string FormatDateTime(DateTime value)
        {
            return value.ToString("G", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Resets the current value
        /// </summary>
        public void ResetValue()
        {
            this._value = DateTime.Now;
            this.BitState[UserHasSetValueStateKey] = false;
            this.Checked = false;

            this.GetCurrentBehavior().SetDateByValue(this._value, this.Format);
            this.OnValueChanged(EventArgs.Empty);
            this.OnTextChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets the current behavior of the control. By default it is showing a calendar in the drop down
        /// </summary>
        /// <returns></returns>
        public RadDateTimePickerBehaviorDirector GetCurrentBehavior()
        {
            return this.currentBehavior;
        }

        /// <summary>
        /// Sets the behavior of the date picker
        /// </summary>
        /// <param name="childrenDirector"></param>
        protected internal virtual void SetBehavior(RadDateTimePickerBehaviorDirector childrenDirector)
        {
            RadDateTimePickerBehaviorDirector current = this.GetCurrentBehavior();

            if (current != null && current as RadDateTimePickerCalendar != null)
            {
                (current as RadDateTimePickerCalendar).PopupControl.Opened -= new EventHandler(PopupControl_Opened);
                (current as RadDateTimePickerCalendar).PopupControl.Opening -= new CancelEventHandler(PopupControl_Opening);
                (current as RadDateTimePickerCalendar).PopupControl.Closing -= new RadPopupClosingEventHandler(PopupControl_Closing);
                (current as RadDateTimePickerCalendar).PopupControl.Closed -= new RadPopupClosedEventHandler(PopupControl_Closed);

                if (this.TextBoxElement != null)
                {
                    (this.TextBoxElement as RadMaskedEditBoxElement).ValueChanged -= new EventHandler(RadDateTimePickerElement_ValueChanged);
                }
            }

            if (this.ElementTree != null)
            {
                this.ElementTree.Control.Controls.Clear();
            }

            if (this.checkBox != null)
                this.checkBox.ToggleStateChanged -= new StateChangedEventHandler(checkBox_ToggleStateChanged);

            this.Children.Clear();
            this.currentBehavior = childrenDirector;
            childrenDirector.CreateChildren();

            if (this.currentBehavior as RadDateTimePickerCalendar != null)
            {
                (this.currentBehavior as RadDateTimePickerCalendar).PopupControl.Opened += new EventHandler(PopupControl_Opened);
                (this.currentBehavior as RadDateTimePickerCalendar).PopupControl.Opening += new CancelEventHandler(PopupControl_Opening);
                (this.currentBehavior as RadDateTimePickerCalendar).PopupControl.Closing += new RadPopupClosingEventHandler(PopupControl_Closing);
                (this.currentBehavior as RadDateTimePickerCalendar).PopupControl.Closed += new RadPopupClosedEventHandler(PopupControl_Closed);
                (this.currentBehavior as RadDateTimePickerCalendar).MaskBoxElement.ValueChanged += new EventHandler(RadDateTimePickerElement_ValueChanged);
            }

            if (this.checkBox != null)
                this.checkBox.ToggleStateChanged += new StateChangedEventHandler(checkBox_ToggleStateChanged);

            currentBehavior.DateTimePickerElement.TextBoxElement.maxDate = this.MaxDate;
            currentBehavior.DateTimePickerElement.TextBoxElement.minDate = this.MinDate;
        }

        /// <summary>
        /// initializes the children
        /// </summary>
        protected override void CreateChildElements()
        {
            SetBehavior(this.DefaultBehavior);

            this._value = DateTime.Now;
            this.creationTime = DateTime.Now;

            this.GetCurrentBehavior().SetDateByValue(_value, String.IsNullOrEmpty(this.customFormat) ? this.format : DateTimePickerFormat.Custom);
        }

        internal void CallKeyDown(KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        internal void CallKeyUp(KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        internal void CallKeyPress(KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void checkBox_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            this.Checked = this.checkBox.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On;
        }

        /// <summary>
        /// Sets the current value to behave as a null value
        /// </summary>
        public void SetToNullValue()
        {
            this.Value = this.NullDate;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MinDate":
                    if ((this.GetCurrentBehavior() as RadDateTimePickerCalendar) != null)
                    {
                        (this.GetCurrentBehavior() as RadDateTimePickerCalendar).Calendar.RangeMinDate = this.MinDate;
                    }

                    break;
                case "MaxDate":
                    if ((this.GetCurrentBehavior() as RadDateTimePickerCalendar) != null)
                    {
                        (this.GetCurrentBehavior() as RadDateTimePickerCalendar).Calendar.RangeMaxDate = this.MaxDate;
                    }

                    break;
                case "Checked":
                    if (!Checked)
                    {
                        this.TextBoxElement.TextBoxItem.HostedControl.Enabled = false;
                    }
                    else
                    {
                        this.TextBoxElement.TextBoxItem.HostedControl.Enabled = true;
                    }

                    this.checkBox.ToggleState = this.Checked ? ToggleState.On : ToggleState.Off;

                    break;
                case "Format":
                case "CustomFormat":
                    this.GetCurrentBehavior().SetDateByValue(_value, String.IsNullOrEmpty(this.customFormat) ? this.format : DateTimePickerFormat.Custom);
                    break;
                case "ShowCheckBox":

                    if (this.ShowCheckBox)
                    {
                        this.checkBox.Visibility = ElementVisibility.Visible;
                        this.checkBox.MaxSize = new Size(15, 15);
                    }
                    else
                    {
                        this.checkBox.Visibility = ElementVisibility.Collapsed;
                        this.checkBox.MaxSize = new Size(15, 15);
                    }

                    break;
                case "Culture":
                    if (this.GetCurrentBehavior() is RadDateTimePickerCalendar)
                    {
                        ((RadDateTimePickerCalendar)this.GetCurrentBehavior()).Calendar.Culture = this.Culture;
                    }

                    this.GetCurrentBehavior().SetDateByValue(_value, String.IsNullOrEmpty(this.customFormat) ? this.format : DateTimePickerFormat.Custom);

                    break;
                case "ShowUpDown":
                    if (!this.ShowUpDown)
                    {
                        this.SetBehavior(new RadDateTimePickerCalendar(this));
                    }
                    else
                    {
                        this.SetBehavior(new RadDateTimePickerSpinEdit(this));
                    }

                    break;
                case "NullText":
                    if (this.GetCurrentBehavior() is RadDateTimePickerCalendar)
                    {
                        ((RadDateTimePickerCalendar)this.GetCurrentBehavior()).TextBoxElement.TextBoxItem.NullText =
                        this.nullText;
                    }

                    break;
            }

            base.OnNotifyPropertyChanged(e);
        }

        #region INotifyPropertyChanged Members

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == ShowCheckBoxStateKey)
            {
                this.OnNotifyPropertyChanged("ShowCheckBox");
            }
            else if (key == ShowUpDownStateKey)
            {
                this.OnNotifyPropertyChanged("ShowUpDown");
            }
            else if (key == CheckStateStateKey)
            {
                this.OnNotifyPropertyChanged("Checked");
            }
            else if (key == ShowCurrentTimeStateKey)
            {
                this.OnNotifyPropertyChanged("ShowCurrentTime");
            }
        }

        #endregion

        public bool ShouldSerializeCulture()
        {
            if (this.cultureInfo == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(this.cultureInfo.ToString()))
                return false;

            if (this.cultureInfo.ToString() == "en-US")
                return false;

            return true;
        }

        public void ResetCulture()
        {
            cultureInfo = new CultureInfo("en-US");
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RightToLeftProperty)
            {
                if ((bool)e.OldValue != (bool)e.NewValue)
                {
                    this.SetBehavior(this.GetCurrentBehavior());
                }
            }

            if (e.Property == RadDateTimePickerElement.IsDropDownShownProperty)
            {
                ((RadDateTimePickerCalendar)this.GetCurrentBehavior()).ArrowButton.SetValue(RadDateTimePickerElement.IsDropDownShownProperty, e.NewValue);
            }
        }

        #endregion

        #region Events
        private void RadDateTimePickerElement_ValueChanged(object sender, EventArgs e)
        {
            RadMaskedEditBoxElement maskBox = this.TextBoxElement;
            if( maskBox.IsKeyBoard )
            {
                return;	
            }

            if (maskBox.Value != null && DateTime.Compare((DateTime)maskBox.Value, this.MaxDate) > 0)
            {
                if (!this.Value.Equals(this.MaxDate))
                {
                    this.Value = this.MaxDate;
                }
                else
                {
                    maskBox.Value = this.MaxDate;
                }

                return;
            }

            if (maskBox.Value != null && DateTime.Compare((DateTime)maskBox.Value, this.MinDate) < 0)
            {                
                if (this.Value != NullDate)
                    maskBox.Value = this.MinDate;                
            }
        }

        private void PopupControl_Closed(object sender, RadPopupClosedEventArgs args)
        {
            this.OnClosed(args);
        }

        private void PopupControl_Closing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnClosing(args);
        }

        private void PopupControl_Opening(object sender, CancelEventArgs args)
        {
            if (this.IsInValidState(true))
            {
                string themeName = this.ElementTree.ThemeName;

                RadCalendar currentCalendar = (this.GetCurrentBehavior() as RadDateTimePickerCalendar).Calendar;

                if (currentCalendar.ThemeName != themeName)
                {
                    currentCalendar.ThemeName = themeName;
                }

                RadSizablePopupControl popupControl = (this.GetCurrentBehavior() as RadDateTimePickerCalendar).PopupControl;

                if (popupControl.ThemeName != themeName)
                {
                    popupControl.ThemeName = themeName;
                }
            }
            this.OnOpening(args);
        }

        private void PopupControl_Opened(object sender, EventArgs args)
        {
            this.OnOpened(args);
        }
        #endregion
    }
}
