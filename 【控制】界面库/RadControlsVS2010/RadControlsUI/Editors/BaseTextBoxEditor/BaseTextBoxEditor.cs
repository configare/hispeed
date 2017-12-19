using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a text editor.
    /// </summary> 
    [ToolboxItem(false)]
    public class BaseTextBoxEditor : BaseInputEditor
    {
        #region Fields

        protected string nullValue = string.Empty;
        protected int selectionStart;
        protected int selectionLength;
        protected bool isAtFirstLine;
        protected bool isAtLastLine;
        protected bool isModified;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RadTextBoxEditor class.
        /// </summary>
        public BaseTextBoxEditor()
        {

        }

        #endregion

        #region Properties

        public override object Value
        {
            get
            {
                RadTextBoxElement editor = (RadTextBoxElement)this.EditorElement;
                return editor.TextBoxItem.Text;
            }
            set
            {
                string text = string.Empty;
                if (value != null)
                {
                    text = value.ToString();
                }
                RadTextBoxElement editor = (RadTextBoxElement)this.EditorElement;
                editor.TextBoxItem.Text = text;
                if (IsInitalizing && editor.TextBoxItem.Text != text)
                {
                    isModified = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the null value for the editor.
        /// </summary>
        public string NullValue
        {
            get { return this.nullValue; }
            set { this.nullValue = value; }
        }

        /// <summary>
        /// Indicates if all charactes should be left alone or converted
        /// to upper or lower case
        /// </summary>
        public CharacterCasing CharacterCasing
        {
            get { return ((RadTextBoxElement)this.EditorElement).TextBoxItem.CharacterCasing; }
            set { ((RadTextBoxElement)this.EditorElement).TextBoxItem.CharacterCasing = value; }
        }

        /// <summary>
        /// The text could span more than a line when the value is true
        /// </summary>
        public bool Multiline
        {
            get { return ((RadTextBoxElement)this.EditorElement).TextBoxItem.Multiline; }
            set { ((RadTextBoxElement)this.EditorElement).TextBoxItem.Multiline = value; }
        }

        /// <summary>
        /// Specifies the maximum length of characters which could be entered
        /// </summary>
        public int MaxLength
        {
            get { return ((RadTextBoxElement)this.EditorElement).TextBoxItem.MaxLength; }
            set { ((RadTextBoxElement)this.EditorElement).TextBoxItem.MaxLength = value; }
        }

        /// <summary>
        /// Gets or sets wheather the editor accepts tha tab key in multiline mode
        /// </summary>
        public bool AcceptsTab
        {
            get { return ((RadTextBoxElement)this.EditorElement).TextBoxItem.AcceptsTab; }
            set { ((RadTextBoxElement)this.EditorElement).TextBoxItem.AcceptsTab = value; }
        }

        /// <summary>
        /// Gets or sets wheather the editor accepts tha tab key in multiline mode
        /// </summary>
        public bool AcceptsReturn
        {
            get { return ((RadTextBoxElement)this.EditorElement).TextBoxItem.AcceptsReturn; }
            set { ((RadTextBoxElement)this.EditorElement).TextBoxItem.AcceptsReturn = value; }
        }

        protected BaseTextBoxEditorElement TextBoxEditorElement
        {
            get
            {
                return this.EditorElement as BaseTextBoxEditorElement;
            }
        }

        public override Type DataType
        {
            get
            {
                return typeof(string);
            }
        }

        public override bool IsModified
        {
            get
            {
                return base.IsModified || isModified;
            }
        }

        #endregion

        #region Initialization

        protected override RadElement CreateEditorElement()
        {
            return new BaseTextBoxEditorElement();
        }

        #endregion

        #region Public methods

        public override void Initialize(object owner, object value)
        {
            isModified = false;
            base.Initialize(owner, value);
        }

        public override void BeginEdit()
        {
            base.BeginEdit();

            RadTextBoxItem textBoxItem = TextBoxEditorElement.TextBoxItem;

            TextBoxEditorElement.BackColor = Color.White;
            textBoxItem.StretchVertically = textBoxItem.Multiline;
            textBoxItem.SelectAll();
            textBoxItem.TextBoxControl.Focus();

            textBoxItem.TextChanging += new TextChangingEventHandler(TextBoxItem_TextChanging);
            textBoxItem.TextChanged += new EventHandler(TextBoxItem_TextChanged);
            textBoxItem.KeyDown += new KeyEventHandler(TextBoxItem_KeyDown);
            textBoxItem.HostedControl.LostFocus += new EventHandler(HostedControl_LostFocus);
        }


        public override bool EndEdit()
        {
            RadTextBoxItem textBoxItem = TextBoxEditorElement.TextBoxItem;

            textBoxItem.TextChanging -= new TextChangingEventHandler(TextBoxItem_TextChanging);
            textBoxItem.TextChanged -= new EventHandler(TextBoxItem_TextChanged);
            textBoxItem.KeyDown -= new KeyEventHandler(TextBoxItem_KeyDown);
            textBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);

            textBoxItem.SelectionStart = 0;
            textBoxItem.SelectionLength = 0;
            textBoxItem.Text = "";

            return base.EndEdit();
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

        private void TextBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.TextBoxEditorElement == null || !this.TextBoxEditorElement.IsInValidState(true))
            {
                return;
            }

            this.selectionStart = this.TextBoxEditorElement.TextBoxItem.SelectionStart;
            this.selectionLength = this.TextBoxEditorElement.TextBoxItem.SelectionLength;
            this.isAtFirstLine = this.TextBoxEditorElement.IsCaretAtFirstLine;
            this.isAtLastLine = this.TextBoxEditorElement.IsCaretAtLastLine;

            this.OnKeyDown(e);
        }

        private void TextBoxItem_TextChanging(object sender, TextChangingEventArgs e)
        {
            if (!IsInitalizing)
            {
                ValueChangingEventArgs changingArgs = new ValueChangingEventArgs(e.NewValue);
                OnValueChanging(changingArgs);
                e.Cancel = changingArgs.Cancel;
            }
        }

        private void TextBoxItem_TextChanged(object sender, EventArgs e)
        {
            if (!IsInitalizing)
            {
                OnValueChanged();
            }
        }

        void HostedControl_LostFocus(object sender, EventArgs e)
        {
            this.OnLostFocus();
        }

        #endregion
    }
}
