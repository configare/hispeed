using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a date time editor.
    /// </summary>
    [RadToolboxItem(false)]
    public class BaseDateTimeEditor : BaseInputEditor
    {
        #region Fields
        private MaskDateTimeProvider dateTimeMaskHandler;
        private int selectedItemIndex = -1;
        private bool isRightmostMaskItemSelected;
        private bool isLeftmostMaskItemSelected;
        private object initialValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RadDateTimeEditor class.
        /// </summary>
        public BaseDateTimeEditor()
        {
        }

        #endregion

        #region Properties

        public override object Value
        {
            get
            {
                RadDateTimePickerElement editor = (RadDateTimePickerElement)this.EditorElement;
                if (editor.Value.Equals(this.NullValue))
                {
                    return null;
                }
                return editor.Value;
            }
            set
            {
                RadDateTimePickerElement editor = (RadDateTimePickerElement)this.EditorElement;

                if (value == null)
                {
                    editor.SetToNullValue();
                    return;
                }

                try
                {
                    editor.Value = Convert.ToDateTime(value);
                }
                catch
                {
                    editor.SetToNullValue();
                }
            }
        }

        /// <summary>
        /// The DateTime value assigned to the date picker when the Value is null
        /// </summary>
        public DateTime NullValue
        {
            get
            {
                return ((RadDateTimePickerElement)this.EditorElement).NullDate;
            }
            set
            {
                ((RadDateTimePickerElement)this.EditorElement).NullDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum date and time that can be selected in the editor.
        /// </summary>
        public DateTime MinValue
        {
            get
            {
                return ((RadDateTimePickerElement)this.EditorElement).MinDate;
            }
            set
            {
                ((RadDateTimePickerElement)this.EditorElement).MinDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum date and time that can be selected in the editor.
        /// </summary>
        public DateTime MaxValue
        {
            get
            {
                return ((RadDateTimePickerElement)this.EditorElement).MaxDate;
            }
            set
            {
                ((RadDateTimePickerElement)this.EditorElement).MaxDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom date/time format string.
        /// </summary>
        public string CustomFormat
        {
            get
            {
                return ((RadDateTimePickerElement)EditorElement).CustomFormat;
            }
            set
            {
                ((RadDateTimePickerElement)EditorElement).CustomFormat = value;
            }
        }

        public override Type DataType
        {
            get
            {
                return typeof(DateTime);
            }
        }

        #endregion

        #region Public methods

        public override void BeginEdit()
        {
            this.Value = this.initialValue;

            base.BeginEdit();

            //This is need to always make selected the first part ot date mask when the editor is opened
            RadDateTimePickerElement editor = (RadDateTimePickerElement)this.EditorElement;
            RadTextBoxElement textBoxElement = editor.TextBoxElement;
            RadDateTimePickerCalendar dateTimeBehavior = editor.CurrentBehavior as RadDateTimePickerCalendar;           
            editor.TextBoxElement.TextBoxItem.HostedControl.Focus();
            editor.TextBoxElement.TextBoxItem.SelectionStart = 0;
            editor.TextBoxElement.TextBoxItem.SelectionLength = 1;

            if (dateTimeBehavior != null)
            {
                dateTimeBehavior.DropDownMinSize = new Size(editor.Size.Width, 200);
                RadMaskedEditBoxElement maskTextBox = editor.TextBoxElement as RadMaskedEditBoxElement;
                MaskDateTimeProvider maskHandler = (MaskDateTimeProvider)maskTextBox.Provider;
                maskHandler.SelectFirstEditableItem();
                this.selectedItemIndex = maskHandler.SelectedItemIndex;
                //int lastCaretPos = maskHandler.;
                //int lastSelectionPos = maskHandler.SelectionPosition;
                //while (maskHandler.SelectionPosition >= 0)
                //{
                //    lastSelectionPos = maskHandler.SelectionPosition;
                //    maskHandler.HandleLeftKey();
                //    if (lastSelectionPos == maskHandler.SelectionPosition)
                //        break;
                //}
                //this.selectedItemIndex = maskHandler.SelectedFormatInfoIndex;
                //maskTextBox.SelectionLength = maskHandler.SelectionLength;

                dateTimeBehavior.PopupControl.Opened += new EventHandler(PopupControl_Opened);
                dateTimeBehavior.PopupControl.Closed += new RadPopupClosedEventHandler(PopupControl_Closed);
                dateTimeBehavior.Calendar.CalendarElement.CalendarStatusElement.ClearButton.Click += new EventHandler(ClearButton_Click);
                dateTimeBehavior.Calendar.CalendarElement.CalendarStatusElement.TodayButton.Click += new EventHandler(TodayButton_Click);

                if (editor.Value == editor.NullDate)
                {
                    maskTextBox.Text = String.Empty;
                }
            }

            editor.NotifyParentOnMouseInput = false;
            editor.BackColor = Color.White;
            editor.ValueChanging += new ValueChangingEventHandler(RadDateTimeEditor_ValueChanging);
            editor.ValueChanged += new EventHandler(RadDateTimeEditor_ValueChanged);

            if (textBoxElement != null)
            {
                textBoxElement.KeyDown += new KeyEventHandler(TextBoxElement_KeyDown);             
            }
        }

        public override bool EndEdit()
        {
            base.EndEdit();
            RadDateTimePickerElement editor = (RadDateTimePickerElement)this.EditorElement;
            RadTextBoxElement textBoxElement = editor.TextBoxElement;
            editor.ValueChanging -= new ValueChangingEventHandler(RadDateTimeEditor_ValueChanging);
            editor.ValueChanged -= new EventHandler(RadDateTimeEditor_ValueChanged);
            RadDateTimePickerCalendar dateTimeBehavior = editor.GetCurrentBehavior() as RadDateTimePickerCalendar;
            if (dateTimeBehavior != null)
            {
                dateTimeBehavior.PopupControl.Opened -= new EventHandler(PopupControl_Opened);
                dateTimeBehavior.PopupControl.Closed -= new RadPopupClosedEventHandler(PopupControl_Closed);
                dateTimeBehavior.Calendar.CalendarElement.CalendarStatusElement.ClearButton.Click -= new EventHandler(ClearButton_Click);
                dateTimeBehavior.Calendar.CalendarElement.CalendarStatusElement.TodayButton.Click -= new EventHandler(TodayButton_Click);
            }
            if (textBoxElement != null)
            {
                textBoxElement.KeyDown -= new KeyEventHandler(TextBoxElement_KeyDown);
                textBoxElement.TextBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);
            }
            RadDateTimePickerCalendar calendar = editor.GetCurrentBehavior() as RadDateTimePickerCalendar;
            if (calendar != null)
            {
                calendar.PopupControl.Hide();
            }
            return true;
        }
        
        protected override RadElement CreateEditorElement()
        {
            BaseDateTimeEditorElement editor = new BaseDateTimeEditorElement();
            RadDateTimePickerCalendar calendarBehavior = editor.GetCurrentBehavior() as RadDateTimePickerCalendar;

            if (calendarBehavior != null)
            {
                calendarBehavior.Calendar.ShowFooter = true;
                editor.CalendarSize = new Size(200, 200);
            }

            return editor;
        }

        public override void Initialize(object owner, object value)
        {
            this.ownerElement = owner as RadElement;
            this.originalValue = null;

            try
            {
                this.initialValue = Convert.ToDateTime(value);
            }
            catch
            {
            }
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            RadDateTimePickerElement dateTimePickerElement = this.EditorElement as RadDateTimePickerElement;
            if (dateTimePickerElement == null || !dateTimePickerElement.IsInValidState(true))
            {
                return;
            }

            if (this.OwnerElement != null)
            {
                if (e.KeyCode == Keys.Right)
                {
                    isRightmostMaskItemSelected = this.IsRightmostMaskItemSelected();
                }
                else if (e.KeyCode == Keys.Left)
                {
                    isLeftmostMaskItemSelected = this.IsLeftmostMaskItemSelected();
                }
                else
                {
                    dateTimePickerElement.NullDate = DateTime.MinValue;
                }
            }
        }

        protected virtual void OnLostFocus()
        {
        }

        #endregion

        #region Event handlers
        private bool IsLeftmostMaskItemSelected()
        {
            if (!EnsureDateTimeMaskHandler())
            {
                return false;
            }
            int tempItemIndex = this.dateTimeMaskHandler.SelectedItemIndex;
            if (tempItemIndex == selectedItemIndex)
            {
                selectedItemIndex = -1;
                return true;
            }
            // selectedItemIndex = tempItemIndex;
            return false;
        }

        private bool IsRightmostMaskItemSelected()
        {
            if (!EnsureDateTimeMaskHandler())
            {
                return false;
            }
            int tempItemIndex = this.dateTimeMaskHandler.List.Count - 1;
            if (this.dateTimeMaskHandler.SelectedItemIndex == tempItemIndex)
            {
                selectedItemIndex = -1;
                return true;
            }
            //  selectedItemIndex = tempItemIndex;
            return false;
        }

        private bool EnsureDateTimeMaskHandler()
        {
            RadDateTimePickerElement editorElement = this.EditorElement as RadDateTimePickerElement;
            if (editorElement == null)
            {
                return false;
            }
            RadDateTimePickerCalendar dateTimeBehavior = editorElement.CurrentBehavior as RadDateTimePickerCalendar;
            if (dateTimeBehavior == null)
            {
                return false;
            }
            RadMaskedEditBoxElement maskTextBox = editorElement.TextBoxElement as RadMaskedEditBoxElement;
            if (maskTextBox == null)
            {
                return false;
            }
            if (maskTextBox.TextBoxItem.SelectionLength == maskTextBox.Text.Length)
            {
                return false;
            }
            this.dateTimeMaskHandler = maskTextBox.Provider as MaskDateTimeProvider;
            if (this.dateTimeMaskHandler == null)
            {
                return false;
            }
            return true;
        }

        private void PopupControl_Opened(object sender, EventArgs e)
        {
            RadDateTimePickerElement dateTimePicker = this.EditorElement as RadDateTimePickerElement;
            RadDateTimePickerCalendar dateTimePickerCalendar = dateTimePicker.GetCurrentBehavior() as RadDateTimePickerCalendar;

            if (dateTimePickerCalendar != null)
            {
                dateTimePickerCalendar.Calendar.SelectedDates.Clear();
                dateTimePickerCalendar.Calendar.InvalidateCalendar();
            }
        }

        private void PopupControl_Closed(object sender, RadPopupClosedEventArgs args)
        {
            RadDateTimePickerElement editor = this.EditorElement as RadDateTimePickerElement;
            editor.TextBoxElement.TextBoxItem.HostedControl.Focus();
        }

        private void TextBoxElement_KeyDown(object sender, KeyEventArgs e)
        { 
            OnKeyDown(e);
        }

        private void RadDateTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged();
        }

        private void RadDateTimeEditor_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            if (!DateTime.Equals(e.NewValue, e.OldValue))
            {
                OnValueChanging(e);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            (this.EditorElement as BaseDateTimeEditorElement).SetToNullValue();
        }

        private void TodayButton_Click(object sender, EventArgs e)
        {
            (this.EditorElement as BaseDateTimeEditorElement).Value = DateTime.Now;
        }

        private void HostedControl_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus();
        }
        #endregion
    }
}