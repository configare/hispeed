using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class RadDropDownListEditableAreaElement : LightVisualElement
    {
        #region Fields
        private RadDropDownListElement owner;
        private RadDropDownTextBoxElement textBox;
        private RadDropDownStyle dropDownStyle = RadDropDownStyle.DropDown;
        private bool onTextBoxCaptureChanged = false;
        private bool entering = false;

        #endregion

        #region Cstors

        static RadDropDownListEditableAreaElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new GroupBoxVisualElementStateManagerFactory(), typeof(RadDropDownListEditableAreaElement));
        }

        public RadDropDownListEditableAreaElement(RadDropDownListElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties
        /// <summary>
        /// TextBox Property
        /// </summary>
        public RadDropDownTextBoxElement TextBox
        {
            get
            {
                return textBox;
            }
            set
            {
                textBox = value;
            }
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.textBox = new RadDropDownTextBoxElement();
            this.textBox.TextBoxItem.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.textBox.Text = "";
            this.Children.Add(this.textBox);
            this.TextAlignment = this.textBox.TextBoxItem.Alignment;
            this.BackColor = this.textBox.BackColor;
            this.DrawFill = true;
            this.DrawText = false;
            this.NumberOfColors = 1;
            this.BindProperty(RadItem.TextProperty, this.textBox, RadItem.TextProperty, PropertyBindingOptions.TwoWay);                        
            this.WireEvents();
        }

        protected override void DisposeManagedResources()
        {
            this.UnWireEvents();
            base.DisposeManagedResources();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.dropDownStyle == RadDropDownStyle.DropDown)
            {
                return;
            }

            PopupEditorNotificationData notificationData = new PopupEditorNotificationData(e);
            notificationData.context = PopupEditorNotificationData.Context.MouseUpOnEditorElement;
            this.owner.NotifyOwner(notificationData);
        }

        #endregion

        #region API

        internal bool Entering
        {
            get
            {
                return this.entering;
            }

            set
            {
                this.entering = value;
            }
        }

        public RadDropDownStyle DropDownStyle
        {
            get
            {
                return this.dropDownStyle;
            }
            set
            {
                if (this.dropDownStyle != value)
                {
                    this.UnWireEditorKeysEvets();
                    this.dropDownStyle = value;
                    this.WireEditorKeysEvets();
                    this.SetDropDownStyle();
                }
            }
        }

        private void SetTabStop(bool tabStop)
        {
            if (this.ElementTree == null || this.ElementTree.Control == null)
            {
                return;
            }

            Control control = this.ElementTree.Control;
            if (control is RadDropDownList)
            {
                control.TabStop = tabStop;
            }
        }

        protected virtual void SetDropDownStyle()
        {
            switch (dropDownStyle)
            {
                case RadDropDownStyle.DropDown:
                    this.textBox.Visibility = ElementVisibility.Visible;
                    this.DrawText = false;
                    this.SetTabStop(false);
                    break;
                case RadDropDownStyle.DropDownList:                    
                    //this.CanFocus = true;
                    //this.Focus();
                    this.textBox.Visibility = ElementVisibility.Hidden;
                    this.DrawText = true;
                    this.SetTabStop(true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Selects a range of text in the editable portion of the combo box
        /// </summary>
        /// <param name="start">The position of the first character in the current text selection within the text box.</param>
        /// <param name="length">The number of characters to select.</param>
        public void Select(int start, int length)
        {
            this.textBox.TextBoxItem.Select(start, length);
        }

        /// <summary>
        /// Selects all the text in the editable portion of the combo box.
        /// </summary>
        public void SelectAll()
        {
            this.textBox.TextBoxItem.SelectAll();
        }

        public override bool Focus()
        {
            switch (this.dropDownStyle)
            {
                case RadDropDownStyle.DropDown:
                    return this.textBox.TextBoxItem.TextBoxControl.Focus();
                case RadDropDownStyle.DropDownList:
                    return base.Focus();
                default:
                    return false;                   
            }
        }

        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of the ComboBox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the text that is selected in the editable portion of the ComboBox."),
        Browsable(false)]
        public string SelectedText
        {
            get
            {
                if (this.DropDownStyle != RadDropDownStyle.DropDownList)
                {
                    return this.textBox.TextBoxItem.SelectedText;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (this.DropDownStyle != RadDropDownStyle.DropDownList)
                {
                    this.textBox.TextBoxItem.SelectedText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of characters selected in the editable portion of the combo box.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the number of characters selected in the editable portion of the combo box.")]
        public int SelectionLength
        {
            get
            {
                return this.textBox.TextBoxItem.SelectionLength;
            }

            set
            {
                this.textBox.TextBoxItem.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the starting index of text selected in the combo box.
        /// </summary>
        [Description("Gets or sets the starting index of text selected in the combo box."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)]
        public int SelectionStart
        {
            get
            {
                return this.textBox.TextBoxItem.SelectionStart;
            }

            set
            {
                this.textBox.TextBoxItem.SelectionStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the ComboBox contains a null 
        /// reference.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        RadDefaultValue("NullText", typeof(RadTextBoxItem)),
        RadDescription("NullText", typeof(RadTextBoxItem))]
        public string NullText
        {
            get
            {
                return this.textBox.TextBoxItem.NullText;
            }

            set
            {
                this.textBox.TextBoxItem.NullText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text that is displayed when the ComboBox contains a null 
        /// reference.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public int MaxLength
        {
            get
            {
                return this.textBox.TextBoxItem.MaxLength;
            }

            set
            {
                this.textBox.TextBoxItem.MaxLength = value;
            }
        }

        #endregion

        #region Helpers

        protected virtual void WireEvents()
        {
            //TODO implement
            this.WireEditorKeysEvets();
            this.DoubleClick += textBox_DoubleClick;
            this.textBox.TextBoxItem.DoubleClick += textBox_DoubleClick;
            this.textBox.TextChanged += textBox_TextChanged;
            this.textBox.TextBoxItem.TextBoxControl.LostFocus += TextBoxItem_LostFocus;
            this.TextBox.TextBoxItem.TextBoxControl.MouseCaptureChanged += TextBoxControl_MouseCaptureChanged;
            this.textBox.TextBoxItem.MouseWheel += textBox_MouseWheel;
        }

        private void WireEditorKeysEvets()
        {
            switch (this.dropDownStyle)
            {
                case RadDropDownStyle.DropDown:
                    this.textBox.KeyDown += textBox_KeyDown;
                    this.textBox.KeyPress += textBox_KeyPress;
                    this.textBox.KeyUp += textBox_KeyUp;
                    this.textBox.MouseUp += textBox_MouseUp;
                    break;
                case RadDropDownStyle.DropDownList:
                    this.KeyDown += textBox_KeyDown;
                    this.KeyPress += textBox_KeyPress;
                    this.KeyUp += textBox_KeyUp;
                    break;
                default:
                    break;
            }
        }

        private void UnWireEditorKeysEvets()
        {
            switch (this.dropDownStyle)
            {
                case RadDropDownStyle.DropDown:
                    this.textBox.KeyDown-= textBox_KeyDown;
                    this.textBox.KeyPress -= textBox_KeyPress;
                    this.textBox.KeyUp -= textBox_KeyUp;
                    this.textBox.MouseUp -= textBox_MouseUp;
                    break;
                case RadDropDownStyle.DropDownList:
                    this.KeyDown -= textBox_KeyDown;
                    this.KeyPress -= textBox_KeyPress;
                    this.KeyUp -= textBox_KeyUp;
                    break;
                default:
                    break;
            }
        }

        protected virtual void UnWireEvents()
        {
            //TODO
            this.UnWireEditorKeysEvets();          
            this.DoubleClick -= textBox_DoubleClick;
            this.textBox.TextBoxItem.DoubleClick -= textBox_DoubleClick;
            this.textBox.TextChanged -= textBox_TextChanged;
            this.textBox.TextBoxItem.TextBoxControl.LostFocus -= TextBoxItem_LostFocus;
            this.TextBox.TextBoxItem.TextBoxControl.MouseCaptureChanged -= TextBoxControl_MouseCaptureChanged;
            this.textBox.TextBoxItem.MouseWheel -= textBox_MouseWheel;
        }

        void textBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.DropDownStyle == RadDropDownStyle.DropDown && this.Entering)
            {
                this.SelectAll();
                this.Entering = false;
            }
        }

        void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            PopupEditorNotificationData notificationData = new PopupEditorNotificationData(e);
            notificationData.context = PopupEditorNotificationData.Context.KeyUp;
            this.owner.NotifyOwner(notificationData);
            if (e.Handled)
            {
                return;
            }

            if (this.owner.InternalKeyUp != null)
            {
                this.owner.InternalKeyUp(sender, e);
            }

            if (this.dropDownStyle == RadDropDownStyle.DropDown)
            {
                base.OnKeyUp(e);
            }
        }

        void TextBoxControl_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (this.onTextBoxCaptureChanged)
            {
                return;
            }

            this.onTextBoxCaptureChanged = true;
            if (this.textBox.TextBoxItem.HostedControl.Capture && !this.SelectAllText(this.Text))
            {
                this.textBox.TextBoxItem.TextBoxControl.DeselectAll();
            }
        }

        private bool SelectAllText(string text)
        {
            if (this.textBox.TextBoxItem.TextBoxControl.Focused)
            {
                this.SelectionStart = 0;
                this.SelectionLength = string.IsNullOrEmpty(text) ? 0 : text.Length;
                return true;
            }
            return false;
        }

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            PopupEditorNotificationData notificationData = new PopupEditorNotificationData(e);
            notificationData.context = PopupEditorNotificationData.Context.KeyDown;
            this.owner.NotifyOwner(notificationData);
            if (e.Handled)
            {
                return;
            }

            if (this.owner.InternalKeyDown != null)
            {
                this.owner.InternalKeyDown(sender, e);
            }

            if (e.KeyCode == Keys.F4)
            {
                this.HandleF4Down();
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                this.HandleKeyUpKeyDown(e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.HandleEsc();
                e.Handled = true;
            }
        }

        private void HandleEsc()
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData();
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.Esc;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }

        private void HandleKeyUpKeyDown(KeyEventArgs e)
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData(e);
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.KeyUpKeyDownPress;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }

        private void HandleF4Down()
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData();
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.F4Press;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }

        void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.owner.InternalKeyPress != null)
            {
                this.owner.InternalKeyPress(sender, e);
            }

            if (e.KeyChar == (char)Keys.Escape)
            {
                this.HandleEsc();
                e.Handled = true;
                return;
            }

            this.owner.NotifyOwner(new PopupEditorNotificationData(e));
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData();
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.TextChanged;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }

        void TextBoxItem_LostFocus(object sender, EventArgs e)
        {
            this.onTextBoxCaptureChanged = false;
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData();
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.MouseEvent;
            this.owner.NotifyOwner(popupEditorNotificationData);            
        }

        void textBox_MouseWheel(object sender, MouseEventArgs e)
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData(e);
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.MouseWheel;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }

        void textBox_DoubleClick(object sender, EventArgs e)
        {
            PopupEditorNotificationData popupEditorNotificationData = new PopupEditorNotificationData();
            popupEditorNotificationData.context = PopupEditorNotificationData.Context.TextBoxDoubleClick;
            this.owner.NotifyOwner(popupEditorNotificationData);
        }    
        #endregion
    }

    public class RadDropDownTextBoxElement : RadTextBoxElement
    {
    }
}
