using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a numeric up/down editor.
    /// </summary> 
    [ToolboxItem(false)]
    public class BaseSpinEditor : BaseInputEditor
    {
        #region Fields

        protected Type valueType = typeof(decimal);
        protected int selectionStart;
        protected int selectionLength;

        #endregion

        #region Properties

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

        /// <summary>
        /// Gets or sets the minimum value that could be set in the editor.
        /// </summary>
        public Decimal MinValue
        {
            get { return ((RadSpinElement)this.EditorElement).MinValue; }
            set { ((RadSpinElement)this.EditorElement).MinValue = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value that could be set in the editor.
        /// </summary>
        public Decimal MaxValue
        {
            get { return ((RadSpinElement)this.EditorElement).MaxValue; }
            set { ((RadSpinElement)this.EditorElement).MaxValue = value; }
        }

        /// <summary>
        /// Gets or sets the value which is added to/subtracted from the current value of the editor.
        /// </summary>
        public Decimal Step
        {
            get { return ((RadSpinElement)EditorElement).Step; }
            set { ((RadSpinElement)EditorElement).Step = value; }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to display in the editor.
        /// </summary>
        public int DecimalPlaces
        {
            get { return ((RadSpinElement)EditorElement).DecimalPlaces; }
            set { ((RadSpinElement)EditorElement).DecimalPlaces = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a thousands separator is displayed in the editor.
        /// </summary>
        public bool ThousandsSeparator
        {
            get { return ((RadSpinElement)EditorElement).ThousandsSeparator; }
            set { ((RadSpinElement)EditorElement).ThousandsSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the type of the value to use in the editor.
        /// </summary>
        public Type ValueType
        {
            get { return this.valueType; }
            set { this.valueType = value; }
        }

        public override Type DataType
        {
            get
            {
                return typeof(decimal);
            }
        }

        #endregion

        #region Public methods

        public override void BeginEdit()
        {
            base.BeginEdit();
            BaseSpinEditorElement spinElement = this.EditorElement as BaseSpinEditorElement;
            spinElement.ValueChanging += new ValueChangingEventHandler(spinElement_ValueChanging);
            spinElement.ValueChanged += new EventHandler(spinElement_ValueChanged);
            spinElement.KeyDown += new KeyEventHandler(spinElement_KeyDown);
            spinElement.KeyUp += new KeyEventHandler(spinElement_KeyUp);
            spinElement.BackColor = Color.White;
            spinElement.TextBoxItem.SelectAll();
            spinElement.TextBoxItem.TextBoxControl.Focus();
            spinElement.RightToLeft = RightToLeft;
            spinElement.TextBoxItem.HostedControl.LostFocus += new EventHandler(HostedControl_LostFocus);
        }

        public override bool EndEdit()
        {
            base.EndEdit();
            BaseSpinEditorElement spinElement = this.EditorElement as BaseSpinEditorElement;
            spinElement.ValueChanging -= new ValueChangingEventHandler(spinElement_ValueChanging);
            spinElement.ValueChanged -= new EventHandler(spinElement_ValueChanged);
            spinElement.KeyDown -= new KeyEventHandler(spinElement_KeyDown);
            spinElement.KeyUp -= new KeyEventHandler(spinElement_KeyUp);
            spinElement.TextBoxItem.HostedControl.LostFocus -= new EventHandler(HostedControl_LostFocus);
            spinElement.EnableValueChangingOnTextChanging = false;
            spinElement.Value = spinElement.MinValue;
            return true;
        }

        public override bool Validate()
        {
            (this.EditorElement as BaseSpinEditorElement).Validate();
            return base.Validate();
        }

        #endregion

        #region Virtual Methods

        public virtual void OnLostFocus()
        {

        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
             
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
             
        }

        #endregion

        #region Event Handlers

        private void spinElement_KeyDown(object sender, KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;
            if (editorElement == null || !editorElement.IsInValidState(true))
            {
                return;
            }

            selectionStart = editorElement.TextBoxItem.SelectionStart;
            selectionLength = editorElement.TextBoxItem.SelectionLength;

            OnKeyDown(e);
        }

        private void spinElement_KeyUp(object sender, KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;
            if (editorElement == null || !editorElement.IsInValidState(true))
            {
                return;
            }

            OnKeyUp(e);
        }

        private void spinElement_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged();
        }

        private void spinElement_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            OnValueChanging(e);
        }

        void HostedControl_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus();
        }

        #endregion

        #region Overrides

        protected override RadElement CreateEditorElement()
        {
            return new BaseSpinEditorElement();
        }

        #endregion

        #region Private Methods

        private object GetEditorValue()
        {
            RadSpinElement editorElement = this.EditorElement as RadSpinElement;
            editorElement.Validate();
            string text = editorElement.Text;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            return editorElement.Value;
        }

        private void SetEditorValue(object value)
        {
            RadSpinElement spinEditor = this.EditorElement as RadSpinElement;
            if (value == null)
            {
                spinEditor.Text = String.Empty;
                return;
            }
            try
            {
                Decimal convertedValue = Convert.ToDecimal(value);
                if (convertedValue < spinEditor.MinValue)
                {
                    convertedValue = spinEditor.MinValue;
                }
                else if (convertedValue > spinEditor.MaxValue)
                {
                    convertedValue = spinEditor.MaxValue;
                }
                spinEditor.Value = convertedValue;
            }
            catch
            {
                try
                {
                    spinEditor.Value = decimal.Parse(value.ToString());
                }
                catch
                {
                    spinEditor.Value = spinEditor.MinValue;
                }
            }
        }

        #endregion
    }
}
