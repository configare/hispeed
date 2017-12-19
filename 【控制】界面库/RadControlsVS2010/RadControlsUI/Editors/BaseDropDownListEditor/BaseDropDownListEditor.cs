using System;
using Telerik.WinControls.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a DropDownList editor.
    /// </summary>
    [RadToolboxItem(false)]
    public class BaseDropDownListEditor : BaseInputEditor
    {
        #region Fields

        protected int selectionStart;
        protected bool cancelValueChanging;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RadDropDownListEditor class.
        /// </summary>
        public BaseDropDownListEditor()
        {
        }

        #endregion

        #region Properties

        public override object Value
        {
            get
            {
                RadDropDownListElement editor = this.EditorElement as RadDropDownListElement;
                if (!string.IsNullOrEmpty(editor.ValueMember))
                {
                    return editor.SelectedValue;
                }
                if (editor.SelectedItem != null)
                {
                    return editor.SelectedItem.Text;
                }
                return null;
            }
            set
            {
                RadDropDownListElement editor = this.EditorElement as RadDropDownListElement;
                if (value == null)
                {
                    editor.SelectedItem = null;
                }
                else if (editor.ValueMember == null)
                {
                    editor.SelectedItem = editor.ListElement.Items[editor.FindStringExact(value.ToString())];
                }
                else
                {
                    editor.SelectedValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the style of the DropDownList.
        /// </summary>
        public RadDropDownStyle DropDownStyle
        {
            get
            {
                return (this.EditorElement as RadDropDownListElement).DropDownStyle;
            }
            set
            {
                (this.EditorElement as RadDropDownListElement).DropDownStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.
        /// </summary>
        public SizingMode DropDownSizingMode
        {
            get
            {
                return (this.EditorElement as RadDropDownListElement).DropDownSizingMode;
            }
            set
            {
                (this.EditorElement as RadDropDownListElement).DropDownSizingMode = value;
            }
        }

        public override Type DataType
        {
            get
            {
                return typeof(string);
            }
        }

        #endregion

        #region Public methods

        public override void BeginEdit()
        {
            base.BeginEdit();

            BaseDropDownListEditorElement dropDownListElement = (BaseDropDownListEditorElement)this.EditorElement;
            dropDownListElement.TextBox.TextBoxItem.TextChanged += new EventHandler(TextBoxItem_TextChanged);
            dropDownListElement.TextBox.TextBoxItem.HostedControl.LostFocus += new EventHandler(HostedControl_LostFocus);
            dropDownListElement.SelectedIndexChanging += new Telerik.WinControls.UI.Data.PositionChangingEventHandler(DropDownListElement_SelectedIndexChanging);
            dropDownListElement.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(DropDownListElement_SelectedIndexChanged);
            dropDownListElement.HandleKeyDown += new KeyEventHandler(DropDownListElement_HandleKeyDown);
            dropDownListElement.SelectionStart = 0;
            dropDownListElement.SelectionLength = dropDownListElement.Text.Length;
            dropDownListElement.Focus();
        }

        public override bool EndEdit()
        {
            base.EndEdit();

            BaseDropDownListEditorElement dropDownListElement = (BaseDropDownListEditorElement)this.EditorElement;

            if (dropDownListElement.IsPopupOpen)
            {
                dropDownListElement.ClosePopup();
            }

            dropDownListElement.SelectedIndexChanging -= new Telerik.WinControls.UI.Data.PositionChangingEventHandler(DropDownListElement_SelectedIndexChanging);
            dropDownListElement.SelectedIndexChanged -= new Telerik.WinControls.UI.Data.PositionChangedEventHandler(DropDownListElement_SelectedIndexChanged);
            dropDownListElement.HandleKeyDown -= new KeyEventHandler(DropDownListElement_HandleKeyDown);
            dropDownListElement.TextBox.TextBoxItem.TextChanged -= new EventHandler(TextBoxItem_TextChanged);
            dropDownListElement.TextBox.TextBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);
            dropDownListElement.DataSource = null;
            dropDownListElement.Items.Clear();
            dropDownListElement.ListElement.BindingContext = null;
            return true;
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
        }

        protected virtual void OnLostFocus()
        {
        }

        #endregion

        #region Event handlers

        private void DropDownListElement_HandleKeyDown(object sender, KeyEventArgs e)
        {
            BaseDropDownListEditorElement editorElement = this.EditorElement as BaseDropDownListEditorElement;
            if (editorElement == null || !editorElement.IsInValidState(true))
            {
                return;
            }

            OnKeyDown(e);
        }

        private void DropDownListElement_SelectedIndexChanging(object sender, Telerik.WinControls.UI.Data.PositionChangingCancelEventArgs e)
        {
            if (e.Position >= 0)
            {
                RadDropDownListElement dropDownListElement = (RadDropDownListElement)this.EditorElement;
                if (!this.IsInitalizing && this.EditorElement.IsInValidState(true))
                {
                    object newValue = dropDownListElement.Items[e.Position].Value;
                    object oldValue = null;
                    if (dropDownListElement.SelectedItem != null)
                    {
                        oldValue = dropDownListElement.SelectedItem.Value;
                    }
                    ValueChangingEventArgs args = new ValueChangingEventArgs(newValue, oldValue);
                    this.OnValueChanging(args);
                    e.Cancel = args.Cancel;
                    cancelValueChanging = args.Cancel;
                }
            }
        }

        private void DropDownListElement_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (!cancelValueChanging)
            {
                OnValueChanged();
            }
        }

        private void TextBoxItem_TextChanged(object sender, EventArgs e)
        {
            BaseDropDownListEditorElement dropDownListElement = (BaseDropDownListEditorElement)this.EditorElement;

            if (dropDownListElement.DropDownStyle != RadDropDownStyle.DropDown)
            {
                return;
            }

            RadTextBoxItem textBox = sender as RadTextBoxItem;
            string text = textBox.Text;
            StringComparison stringComparison = dropDownListElement.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            foreach (RadListDataItem item in dropDownListElement.Items)
            {
                if (item.Text.Equals(text, stringComparison))//ticket id 370361  
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        void HostedControl_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus();
        }

        #endregion

        #region Protected Methods

        protected override RadElement CreateEditorElement()
        {
            return new BaseDropDownListEditorElement();
        }
        #endregion
    }
}