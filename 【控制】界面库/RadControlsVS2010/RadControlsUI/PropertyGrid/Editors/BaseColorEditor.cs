using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class BaseColorEditor : BaseInputEditor
    {
        #region Fileds

        protected Type valueType = typeof(Color);

        #endregion

        #region Initialization

        protected override RadElement CreateEditorElement()
        {
            return new BaseColorEditorElement();
        }

        #endregion

        #region Properties

        public override Type DataType
        {
            get
            {
                return typeof(Color);
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

            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;
            editorElement.ValueChanging += new ValueChangingEventHandler(editorElement_ValueChanging);
            editorElement.ValueChanged += new EventHandler(editorElement_ValueChanged);
            editorElement.KeyDown += new KeyEventHandler(editorElement_KeyDown);
            editorElement.KeyUp += new KeyEventHandler(editorElement_KeyUp);
            editorElement.ColorTextBox.TextBoxItem.SelectAll();
            editorElement.ColorTextBox.TextBoxItem.TextBoxControl.Focus();
            editorElement.RightToLeft = this.RightToLeft;
            editorElement.ColorTextBox.TextBoxItem.HostedControl.LostFocus += new EventHandler(HostedControl_LostFocus);
        }

        public override bool EndEdit()
        {
            base.EndEdit();
            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;
            editorElement.ValueChanging -= editorElement_ValueChanging;
            editorElement.ValueChanged -= editorElement_ValueChanged;
            editorElement.KeyDown -= editorElement_KeyDown;
            editorElement.KeyUp -= editorElement_KeyUp;
            editorElement.ColorTextBox.TextBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);

            return true;
        }

        public override bool Validate()
        {
            return (((BaseColorEditorElement)this.EditorElement).Validate() && base.Validate());
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

        void HostedControl_LostFocus(object sender, EventArgs e)
        {
            this.OnLostFocus();
        }

        void editorElement_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        void editorElement_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
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

        private Color GetEditorValue()
        {
            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;
            editorElement.Validate();
            return editorElement.Value;
        }

        private void SetEditorValue(Object value)
        {
            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;
            TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));

            if (!colorConverter.IsValid(value))
            {
                editorElement.Value = Color.Empty;
            } 
            else
            {
                editorElement.Value = (Color)colorConverter.ConvertFromString(value.ToString());
            }
        }

        #endregion
    }
}
