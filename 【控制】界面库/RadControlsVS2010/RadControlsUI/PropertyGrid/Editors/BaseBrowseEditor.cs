using System;
using System.Windows.Forms;
using System.IO;

namespace Telerik.WinControls.UI
{
    public class BaseBrowseEditor : BaseInputEditor
    {
        #region Fileds

        protected Type valueType = typeof(string);

        #endregion

        #region Initialization

        protected override RadElement CreateEditorElement()
        {
            return new BaseBrowseEditorElement();
        }

        #endregion

        #region Properties

        public override Type DataType
        {
            get
            {
                return typeof(string);
            }
        }

        public override object Value
        {
            get
            {
                return this.GetEditorValue();
            }
            set
            {
                this.SetEditorValue(value);
            }
        }

        #endregion

        #region Methods

        public override void BeginEdit()
        {
            base.BeginEdit();

            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;
            editorElement.ValueChanging += new ValueChangingEventHandler(editorElement_ValueChanging);
            editorElement.ValueChanged += new EventHandler(editorElement_ValueChanged);
            editorElement.KeyDown += new KeyEventHandler(editorElement_KeyDown);
            editorElement.KeyUp += new KeyEventHandler(editorElement_KeyUp);
            editorElement.FilePathTextBox.TextBoxItem.SelectAll();
            editorElement.FilePathTextBox.TextBoxItem.TextBoxControl.Focus();
            editorElement.RightToLeft = this.RightToLeft;
            editorElement.FilePathTextBox.TextBoxItem.HostedControl.LostFocus += new EventHandler(HostedControl_LostFocus);
        }

        public override bool EndEdit()
        {
            base.EndEdit();
            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;
            editorElement.ValueChanging -= editorElement_ValueChanging;
            editorElement.ValueChanged -= editorElement_ValueChanged;
            editorElement.KeyDown -= editorElement_KeyDown;
            editorElement.KeyUp -= editorElement_KeyUp;
            editorElement.FilePathTextBox.TextBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);

            return true;
        }

        public override bool Validate()
        {
            return (base.Validate() && ((BaseBrowseEditorElement)this.EditorElement).Validate());
        }

        #endregion

        #region Event handlers

        protected virtual void OnLostFocus()
        {

        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {

        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {

        }

        protected virtual void OnOpenFileDialogOpening(OpenFileDialogOpeningEventArgs e)
        {
        	
        }

        void HostedControl_LostFocus(object sender, EventArgs e)
        {
            this.OnLostFocus();
        }

        void editorElement_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        void editorElement_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        void editorElement_ValueChanged(object sender, EventArgs e)
        {
            this.OnValueChanged();
        }

        void editorElement_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            this.OnValueChanging(e);
        }
        
        #endregion

        #region Implementation

        private string GetEditorValue()
        {
            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;
            editorElement.Validate();

            return editorElement.Value;
        }

        private void SetEditorValue(Object value)
        {
            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;

            if (string.IsNullOrEmpty(value.ToString()) || !File.Exists(value.ToString()))
            {
                editorElement.Value = String.Empty;
            }
            else
            {
                editorElement.Value = value.ToString();
            }
        }

        #endregion
    }
}
