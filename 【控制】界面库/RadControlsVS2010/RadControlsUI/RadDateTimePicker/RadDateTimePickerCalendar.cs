using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the RadDateTimePickerCalendar class
    /// </summary>
    public class RadDateTimePickerCalendar : RadDateTimePickerBehaviorDirector, IDisposable
    {
        private RadDateTimePickerElement dateTimePickerElement;

        private RadMaskedEditBoxElement textBoxElement;
        private DockLayoutPanel dockLayout;
        private BorderPrimitive border;
        private FillPrimitive backGround;
        private RadCheckBoxElement checkBox;
        private RadArrowButtonElement arrowButton;
        private RadDateTimePickerDropDown popupControl;
        private RadCalendar calendar;
        private bool isDropDownShown;
        private Size dropDownMinSize = Size.Empty;
        private Size dropDownMaxSize = Size.Empty;
        private int dropDownHeight = 156;

        /// <summary>
        /// Represents the RadDateTimePickerCalendar constructor
        /// </summary>
        /// <param name="dateTimePicker"></param>
        public RadDateTimePickerCalendar(RadDateTimePickerElement dateTimePicker)
        {
            this.dateTimePickerElement = dateTimePicker;

            this.calendar = new RadCalendar();
            this.calendar.Focusable = false;

            this.popupControl = new RadDateTimePickerDropDown(this.dateTimePickerElement);
            this.popupControl.SizingMode = SizingMode.UpDownAndRightBottom;
            this.popupControl.Opened += new EventHandler(controlPanel_Opened);
            this.popupControl.Closing += new RadPopupClosingEventHandler(controlPanel_Closing);
            this.popupControl.Closed += new RadPopupClosedEventHandler(controlPanel_Closed);

            this.popupControl.HostedControl = this.calendar;
            this.popupControl.LoadElementTree();
            this.calendar.AllowMultipleSelect = false;
            this.calendar.SelectionChanged += new EventHandler(calendar_SelectionChanged);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.textBoxElement.Value != null)
            {
                textBoxElement.ValueChanged -= new EventHandler(maskBox_ValueChanged);
                textBoxElement.TextBoxItem.LostFocus -= new EventHandler(MaskBox_LostFocus);
                textBoxElement.TextBoxItem.GotFocus -= new EventHandler(maskBox_GotFocus);
                textBoxElement.TextBoxItem.MouseDown -= new MouseEventHandler(maskBox_MouseDown);
                textBoxElement.TextBoxItem.TextChanged -= new EventHandler(maskBox_TextChanged);
            }

            if (this.textBoxElement != null)
            {
                this.textBoxElement.KeyDown -= new KeyEventHandler(textBoxElement_KeyDown);
                this.textBoxElement.KeyPress -= new KeyPressEventHandler(textBoxElement_KeyPress);
                this.textBoxElement.KeyUp -= new KeyEventHandler(textBoxElement_KeyUp);
            }

            if (this.popupControl != null)
            {
                this.popupControl.Opened -= new EventHandler(controlPanel_Opened);
                this.popupControl.Closing -= new RadPopupClosingEventHandler(controlPanel_Closing);
                this.popupControl.Closed -= new RadPopupClosedEventHandler(controlPanel_Closed);
                this.popupControl.Dispose();
                this.popupControl = null;
            }

            if (this.calendar != null)
            { 
                this.calendar.SelectionChanged -= new EventHandler(calendar_SelectionChanged);
                this.calendar.Dispose();
                this.calendar = null;
            }

            if (this.textBoxElement != null)
            {
                this.textBoxElement.Dispose();
                this.textBoxElement.DisposeChildren();
                this.textBoxElement = null;
            }

            if (this.checkBox != null)
            {
                this.checkBox.Dispose();
                this.checkBox = null;
            }

            if (this.dockLayout != null)
            {
                this.dockLayout.Dispose();
                this.dockLayout = null;
            }

            if (this.border != null)
            {
                this.border.Dispose();
                this.border = null;
            }

            if (this.backGround != null)
            {
                this.backGround.Dispose();
                this.backGround = null;
            }

            if (this.arrowButton != null)
            {
                this.arrowButton.Dispose();
                this.arrowButton = null;
            }
            //private RadArrowButtonElement arrowButton;
        }

        #endregion

        void maskBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate))
            {
                this.textBoxElement.TextBoxItem.Text = "";
            }
        }

        void maskBox_TextChanged(object sender, EventArgs e)
        {
            //if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate) && !this.DateTimePickerElement.TextBoxElement.IsKeyBoard)
            //{
            //    if (!String.IsNullOrEmpty(textBoxElement.TextBoxItem.Text))
            //    {
            //        textBoxElement.TextBoxItem.Text = "";
            //    }
            //}
        }

        private void maskBox_GotFocus(object sender, EventArgs e)
        {
            //if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate))
            //{
            //    this.maskBox.Text = "";
            //}
            //if (this.DateTimePickerElement != null && this.DateTimePickerElement.Value == this.DateTimePickerElement.NullDate)
            //{
            //    this.maskBox.AllowMouseDownNavigation = false;
            //    this.DateTimePickerElement.TextBoxElement.TextBoxItem.ReadOnly = true;
            //    this.maskBox.BypassUpdateOnLostFocus = true;
            //}
        }

        private void MaskBox_LostFocus(object sender, EventArgs e)
        {
            if (this.textBoxElement.Value==null)
            {
                return;
            }

            DateTime value = (DateTime)this.textBoxElement.Value;
            if (value.Equals(this.DateTimePickerElement.NullDate))
            {
                this.textBoxElement.TextBoxItem.Text = "";
                return;
            }

            if (value != this.DateTimePickerElement.NullDate && 
                ((value < this.DateTimePickerElement.MinDate) || (value > this.DateTimePickerElement.MaxDate)))
            {
                if (value < this.DateTimePickerElement.MinDate)
                {
                    value = this.DateTimePickerElement.MinDate;
                }
                else if (value > this.DateTimePickerElement.MaxDate)
                {
                    value = this.DateTimePickerElement.MaxDate;
                }
            }

            //ValueChangingEventArgs args = new ValueChangingEventArgs(value, this.DateTimePickerElement.Value);
            //this.DateTimePickerElement.CallOnValueChanging(args);

            //if (args.Cancel)
            //{
            //    return;
            //}

            this.textBoxElement.Value = value;
        }

        internal void SetDate()
        {
            if (this.Calendar.SelectedDates.Count > 0 && this.calendar.SelectedDate >= this.DateTimePickerElement.MinDate && this.calendar.SelectedDate <= this.dateTimePickerElement.MaxDate)
            {
                this.textBoxElement.IsKeyBoard = true;
                this.textBoxElement.Value = this.Calendar.SelectedDate;
                this.textBoxElement.IsKeyBoard = false;
                //RadMaskComplexEditBehavior behavior = this.maskBox.RadMaskEditBehavior as RadMaskComplexEditBehavior;
                //if (behavior != null)
                //{
                //    behavior.UpdateUI();
                //}
                // old implementation when the datetimepicker was with disabled textbox
                //		this.DateTimePickerElement.Value = this.calendar.SelectedDate;
            }
        }

        private bool maskEditValueChanged;

        /// <summary>
        /// Sets the date shown in the textbox by a given value and format type.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatType"></param>
        public override void SetDateByValue(DateTime? date, DateTimePickerFormat formatType)
        {
            CultureInfo info = Thread.CurrentThread.CurrentCulture;

            maskEditValueChanged = false;

            Thread.CurrentThread.CurrentCulture = this.Calendar.Culture;
            this.textBoxElement.Culture = this.Calendar.Culture;

            if (date != this.DateTimePickerElement.NullDate)
            {
                if (date != this.calendar.FocusedDate && date.HasValue)
                    this.calendar.FocusedDate = date.Value;

                switch (formatType)
                {
                    case DateTimePickerFormat.Time:
                        if (this.dateTimePickerElement.ShowCurrentTime)
                        {
                            if (!date.HasValue)
                            {
                                date = DateTime.Now;
                            }
                            else
                            {
                                date = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute,
                                                    DateTime.Now.Second, DateTime.Now.Millisecond, date.Value.Kind);
                            }
                        }
                        this.textBoxElement.Mask = "T";
                        break;
                    case DateTimePickerFormat.Short:
                        this.textBoxElement.Mask = "d";
                        break;
                    case DateTimePickerFormat.Long:
                        this.textBoxElement.Mask = "D";
                        break;
                    case DateTimePickerFormat.Custom:
                        this.textBoxElement.Mask = this.dateTimePickerElement.CustomFormat;
                        break;
                }

                if (this.textBoxElement.Value==null)
                {
                    this.textBoxElement.Value = date;
                }

                if (this.textBoxElement.Value!=null && !this.textBoxElement.Value.Equals(date) && !this.textBoxElement.IsKeyBoard)
                {
                    this.textBoxElement.Value = date;
                }
            }
            else
            {
                if (!this.textBoxElement.IsKeyBoard)
                {
                    if (!this.textBoxElement.Value.Equals(date))
                    {
                        this.textBoxElement.Value = date;
                    }

                    this.textBoxElement.Text = this.textBoxElement.TextBoxItem.NullText;
                }
            }

            Thread.CurrentThread.CurrentCulture = info;

            maskEditValueChanged = true;
        }

        private void calendar_SelectionChanged(object sender, EventArgs e)
        {
            SetDate();
            this.PopupControl.HideControl();
        }

        //public RadDateTimePickerElement DateTimePickerElement { get { return this.textBoxElement; } }

        /// <summary>
        /// Gets or sets the calendar control which is shown when the pop up control is shown
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the calendar control which is shown when the pop up control is shown")]
        [DefaultValue(SizingMode.None)]
        public RadCalendar Calendar
        {
            get
            {
                return this.calendar;
            }

            set
            {
                this.calendar = value;

                if (this.popupControl != null)
                {
                    this.popupControl.HostedControl = this.calendar;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drop down control which is shown when the user clicks on the arrow button
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down control which is shown when the user clicks on the arrow button")]
        [DefaultValue(SizingMode.None)]
        public RadDateTimePickerDropDown PopupControl
        {
            get
            {
                return this.popupControl;
            }

            set
            {
                this.popupControl = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
        [DefaultValue(SizingMode.None)]
        public SizingMode DropDownSizingMode
        {
            get
            {
                return popupControl.SizingMode;
            }
            set
            {
                popupControl.SizingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop down minimum size.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down minimum size.")]
        [DefaultValue(typeof(Size), "0,0")]
        public Size DropDownMinSize
        {
            get
            {
                return this.dropDownMinSize;
            }
            set
            {
                this.dropDownMinSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop down maximum size.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down maximum size.")]
        [DefaultValue(typeof(Size), "0,0")]
        public Size DropDownMaxSize
        {
            get
            {
                return this.dropDownMaxSize;
            }
            set
            {
                this.dropDownMaxSize = value;
            }
        }

        /// <summary>
        /// Gets a value representing whether the drop down is shown
        /// </summary>
        [Description("Gets a value representing whether the drop down is shown")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(false)]
        public bool IsDropDownShow
        {
            get
            {
                return PopupManager.Default.ContainsPopup(this.popupControl);
            }
        }

        private Size cachedSize = Size.Empty;

        private Size GetDropDownSize()
        {
            Size dropDownSize = Size.Empty;

            dropDownSize = new Size(Math.Max(185, this.dateTimePickerElement.ControlBoundingRectangle.Width), Math.Max(185, this.dropDownHeight));

            if (this.dateTimePickerElement.ElementTree != null)
            {
                if (dropDownSize.Width == -1)
                    dropDownSize.Width = this.dateTimePickerElement.ElementTree.Control.Width;

                dropDownSize.Height = this.dropDownHeight +
                                      (this.DropDownSizingMode != SizingMode.None ? 10 : 0);

                if (this.dateTimePickerElement.CalendarSize != new Size(100, 156))
                {
                    dropDownSize = this.dateTimePickerElement.CalendarSize;
                }

                if (this.cachedSize != Size.Empty)
                    dropDownSize = cachedSize;
            }

            return dropDownSize;
        }

        private void SetDropDownMinMaxSize()
        {
            if (this.DropDownSizingMode != SizingMode.None)
            {
                this.popupControl.MinimumSize = LayoutUtils.UnionSizes(this.dropDownMinSize, new Size(0, this.dropDownMaxSize.Height + 10));

                //this.popupControl.MinimumSize = new Size(185, 185);
                //this.popupControl.ResizeGrip.MinSize = new Size(185, 185);

                if (this.dropDownMaxSize != Size.Empty)
                    this.popupControl.MaximumSize = new Size(this.dropDownMaxSize.Width,
                                                             this.dropDownMaxSize.Height + 10);
            }
            else
            {
                this.popupControl.MinimumSize = Size.Empty;
                this.popupControl.MaximumSize = Size.Empty;
            }
        }

        /// <summary>
        /// Shows the drop-down window part of the combo box
        /// </summary>
        public virtual void ShowDropDown()
        {
            if (this.dateTimePickerElement.Visibility != ElementVisibility.Visible)
                return;
            RadControl c = this.dateTimePickerElement.ElementTree.Control as RadControl;

            if (this.DateTimePickerElement.Value.HasValue)
            {
                this.calendar.FocusedDate = this.DateTimePickerElement.Value.Value;
            }
            else
            {
                this.calendar.FocusedDate = DateTime.Now;
            }
            
            popupControl.OwnerControl = c;

            if (!isDropDownShown)
            {
                this.SetDropDownMinMaxSize();
                popupControl.Size = this.GetDropDownSize();

                if (this.dateTimePickerElement.ElementTree != null && this.dateTimePickerElement.ElementTree != null)
                {
                    if (this.popupControl.HostedControl != null)
                    {
                        int sizablePopupHeight = (int)popupControl.SizingGrip.DesiredSize.Height;
                        popupControl.HostedControl.Size = new Size(popupControl.Size.Width, popupControl.Size.Height - sizablePopupHeight);
                    }

                    Point pt = popupControl.ShowControl(RadDirection.Down, 0);
                    this.textBoxElement.TextBoxItem.Focus();

                    if (!this.dateTimePickerElement.RightToLeft)
                    {
                        popupControl.Location = pt;
                    }
                    else
                    {
                        int distance = this.PopupControl.Size.Width - this.dateTimePickerElement.ControlBoundingRectangle.Width;
                        pt = new Point(pt.X - distance, pt.Y);
                        popupControl.Location = pt;
                    }

                    if (this.DateTimePickerElement.CalendarLocation != Point.Empty)
                    {
                        popupControl.Location = this.DateTimePickerElement.CalendarLocation;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the instance of RadDateTimePickerElement associated to the control
        /// </summary>
        [Browsable(false)]
        [Description("Gets the instance of RadDateTimePickerElement associated to the control")]
        [Category("Behavior")]
        public override RadDateTimePickerElement DateTimePickerElement
        {
            get
            {
                return this.dateTimePickerElement;
            }
        }

        internal RadMaskedEditBoxElement TextBoxElement
        {
            get
            {
                return this.textBoxElement;
            }
        }

        internal RadMaskedEditBoxElement MaskBoxElement
        {
            get
            {
                return this.textBoxElement;
            }
        }

        //private RadMaskTextBox maskBox;

        /// <summary>
        /// Gets an instance of RadMaskBox
        /// </summary>
        //[Description("Gets an instance of RadMaskBox")]
        //[DefaultValue(null)]
        //internal RadMaskTextBox MaskBox
        //{
        //    get
        //    {
        //        return this.maskBox;
        //    }
        //}

        /// <summary>
        /// Gets the RadArrowButtonElement instance
        /// that represents the Date Time Picker's arrow
        /// button.
        /// </summary>
        internal RadArrowButtonElement ArrowButton
        {
            get
            {
                return this.arrowButton;
            }
        }
        
        /// <summary>
        /// Creates dateTimePicker's children 
        /// </summary>
        public override void CreateChildren()
        {
            this.textBoxElement = new RadMaskedEditBoxElement();
            // textBoxElement = this.textBoxElement.TextBoxItem.HostedControl as RadMaskTextBox;
            this.textBoxElement.Mask = "";
            textBoxElement.MaskType = MaskType.DateTime;
            textBoxElement.ValueChanged += new EventHandler(maskBox_ValueChanged);
            textBoxElement.TextBoxItem.LostFocus += new EventHandler(MaskBox_LostFocus);
            textBoxElement.TextBoxItem.GotFocus += new EventHandler(maskBox_GotFocus);
            textBoxElement.TextBoxItem.MouseDown += new MouseEventHandler(maskBox_MouseDown);
            textBoxElement.TextBoxItem.TextChanged += new EventHandler(maskBox_TextChanged);
            this.textBoxElement.KeyDown += new KeyEventHandler(textBoxElement_KeyDown);
            this.textBoxElement.KeyPress += new KeyPressEventHandler(textBoxElement_KeyPress);
            this.textBoxElement.KeyUp += new KeyEventHandler(textBoxElement_KeyUp);

            this.textBoxElement.ShowBorder = false;
            this.textBoxElement.Class = "textbox";
            this.textBoxElement.ThemeRole = "DateTimePickerMaskTextBoxElement";

            this.dockLayout = new DockLayoutPanel();
            this.border = new BorderPrimitive();
            this.backGround = new FillPrimitive();
            this.checkBox = new RadCheckBoxElement();
            this.arrowButton = new RadDateTimePickerArrowButtonElement();
            this.arrowButton.Class = "ArrowButton";
            this.border.Class = "DateTimePickerBorder";
            this.backGround.Class = "DateTimePickerBackGround";
            this.backGround.GradientStyle = GradientStyles.Solid;
           
            if (this.dateTimePickerElement.RightToLeft)
            {
                this.checkBox.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
                this.checkBox.Children[1].Alignment = ContentAlignment.MiddleLeft;
                this.arrowButton.SetValue(DockLayoutPanel.DockProperty, Dock.Left);
                this.calendar.RightToLeft = RightToLeft.Yes;
            }
            else
            {
                this.checkBox.SetValue(DockLayoutPanel.DockProperty, Dock.Left);
                this.checkBox.Children[1].Alignment = ContentAlignment.MiddleLeft;
                this.arrowButton.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
                this.calendar.RightToLeft = RightToLeft.No;
            }

            this.arrowButton.StretchHorizontally = false;
            this.checkBox.StretchHorizontally = false;
            this.textBoxElement.Alignment = ContentAlignment.MiddleLeft;
            this.arrowButton.Arrow.AutoSize = true;
            this.arrowButton.Arrow.Alignment = ContentAlignment.MiddleCenter;
            this.arrowButton.MinSize = new Size(17, 6);

            this.dockLayout.Children.Add(this.checkBox);
            this.dockLayout.Children.Add(this.arrowButton);
            this.dockLayout.Children.Add(this.textBoxElement);

            this.dateTimePickerElement.Children.Add(this.backGround);
            this.dateTimePickerElement.Children.Add(this.dockLayout);
            this.dateTimePickerElement.Children.Add(this.border);

            this.dateTimePickerElement.checkBox = this.checkBox;

            if (!this.dateTimePickerElement.ShowCheckBox)
            {
                this.checkBox.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            }

            this.SetDateByValue(this.dateTimePickerElement.Value, this.dateTimePickerElement.Format);
            this.arrowButton.MouseDown += new MouseEventHandler(arrowButton_MouseDown);

            // The BorderPrimitive of the RadTextBoxElement should be collapsed. This makes the size of the editors equal
            this.textBoxElement.Children[this.TextBoxElement.Children.Count - 1].Visibility = ElementVisibility.Collapsed;
            this.border.Visibility = ElementVisibility.Visible;

            this.calendar.RangeMinDate = this.DateTimePickerElement.MinDate;
            this.calendar.RangeMaxDate = this.DateTimePickerElement.MaxDate;	
        }

        private void textBoxElement_KeyUp(object sender, KeyEventArgs e)
        {
            this.DateTimePickerElement.CallKeyUp(e);
        }

        private void textBoxElement_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.DateTimePickerElement.CallKeyPress(e);
        }

        private void textBoxElement_KeyDown(object sender, KeyEventArgs e)
        {
            this.DateTimePickerElement.CallKeyDown(e);
        }

        private void arrowButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.popupControl.HideControl();
            }
            else
            {
                if (!this.IsDropDownShow)
                {
                    if (this.Calendar.SelectedDate != this.DateTimePickerElement.Value)
                    {                        
                        if (this.DateTimePickerElement.Value.HasValue)
                        {
                            this.calendar.SelectedDate = this.DateTimePickerElement.Value.Value;
                        }
                        else
                        {
                            this.calendar.SelectedDate = DateTime.Now;
                        }
                    }
                    this.Calendar.FocusedDate = this.Calendar.SelectedDate;
                }

                //this.Calendar.FocusedDate = DateTime.MinValue;
               
                if (!this.IsDropDownShow)
                {
                    ShowDropDown();
                }
                else
                {
                    this.popupControl.ClosePopup(RadPopupCloseReason.Keyboard);
                }
            }
        }

        private void maskBox_ValueChanged(object sender, EventArgs e)
        {
            if (maskEditValueChanged)
            {
                //if ((DateTime)this.textBoxElement.Value >= this.DateTimePickerElement.MinDate && (DateTime)this.textBoxElement.Value <= this.dateTimePickerElement.MaxDate)
                if (this.textBoxElement.Value != null && this.textBoxElement.TextBoxItem.Text!="")
                {
                    this.DateTimePickerElement.Value = (DateTime)this.textBoxElement.Value;
                }
            }
        }

        private void controlPanel_Closed(object sender, RadPopupClosedEventArgs args)
        {
            this.isDropDownShown = false;
            this.dateTimePickerElement.IsDropDownShown = false;
        }

        private void controlPanel_Closing(object sender, RadPopupClosingEventArgs args)
        {
            this.cachedSize = this.popupControl.Size;
            Form frm = this.dateTimePickerElement.ElementTree.Control.FindForm();
            if (frm != null)
            {
                frm.FormClosed -= new FormClosedEventHandler(frm_FormClosed);
            }
        }

        private void controlPanel_Opened(object sender, EventArgs e)
        {
            this.isDropDownShown = true;
            this.dateTimePickerElement.IsDropDownShown = true;
            Form frm = this.dateTimePickerElement.ElementTree.Control.FindForm();
            if (frm != null)
            {
                frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
            }
        }

        private void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.PopupControl == null)
                return;

            this.PopupControl.HideControl();
        }
    }

    public class RadDateTimePickerArrowButtonElement : RadArrowButtonElement
    {
        static RadDateTimePickerArrowButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadDropDownArrowButtonElementStateManagerFactory(RadDateTimePickerElement.IsDropDownShownProperty), typeof(RadDateTimePickerArrowButtonElement));
        }
    }
}