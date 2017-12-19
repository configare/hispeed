using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an editor for items of type Color.
    /// </summary>
    public class BaseColorEditorElement : LightVisualElement
    {
        #region Fields

        private BaseColorEditorColorBox colorBox;
        private RadTextBoxElement colorTextBox;
        private RadButtonElement colorPickerButton;
        private StackLayoutElement stack;
        private Color value;
        private Color oldValue;
        private TypeConverter converter;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the editor.
        /// </summary>
        public virtual Color Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.SetColorValue(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="BaseColorEditorColorBox"/> that shows the color in the editor.
        /// </summary>
        public BaseColorEditorColorBox ColorBox
        {
        	get
            {
                return this.colorBox;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadTextBoxElement"/> which displays the color as a string.
        /// </summary>
        public RadTextBoxElement ColorTextBox
        {
            get
            {
                return this.colorTextBox;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadButtonElement"/> that opens the <see cref="RadColorDialog"/>.
        /// </summary>
        public RadButtonElement ColorPickerButton
        {
        	get
            {
                return this.colorPickerButton;
            }
        }
                        
        #endregion

        #region Initialization & Disposal

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = false;
            this.DrawBorder = false;
            this.value = Color.Empty;
            
            this.ShouldHandleMouseInput = true;        
            this.NotifyParentOnMouseInput = false;

            this.Padding = new Padding(2);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.converter = TypeDescriptor.GetConverter(typeof(Color));

            this.stack = new StackLayoutElement();
            this.stack.Orientation = Orientation.Horizontal;
            this.stack.StretchVertically = true;
            this.stack.FitInAvailableSize = true;
            this.stack.StretchHorizontally = true;
            this.stack.ShouldHandleMouseInput = false;
            this.stack.NotifyParentOnMouseInput = true;
            this.stack.ElementSpacing = 1;

            this.colorBox = this.CreateColorBoxElement();
            this.colorTextBox = this.CreateColorTextBoxElement();
            this.colorPickerButton = this.CreateColorPickerButtonElement();
            
            this.stack.Children.Add(this.colorBox);
            this.stack.Children.Add(this.colorTextBox);
            this.stack.Children.Add(this.colorPickerButton);

            this.Children.Add(this.stack);
            
            this.WireEvents();
        }

        protected virtual RadTextBoxElement CreateColorTextBoxElement()
        {
            RadTextBoxElement element = new RadTextBoxElement();
            element.StretchHorizontally = true;
            element.StretchVertically = true;
            element.NotifyParentOnMouseInput = true;

            return element;
        }

        protected virtual RadButtonElement CreateColorPickerButtonElement()
        {
            RadButtonElement element = new RadButtonElement();
            element.StretchHorizontally = false;
            element.StretchVertically = true;
            element.Text = ". . .";
            element.Click += new EventHandler(ButtonElement_Click);

            return element;
        }

        protected virtual BaseColorEditorColorBox CreateColorBoxElement()
        {
            return new BaseColorEditorColorBox();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this.UnwireEvents();
        }

        #endregion

        #region Methods

        public virtual bool Validate()
        {
            try
            {
                this.converter.ConvertFromString(this.colorTextBox.Text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Implementation

        protected virtual void WireEvents()
        {
            this.colorTextBox.KeyDown += new KeyEventHandler(ColorTextBox_KeyDown);
        }

        protected virtual void UnwireEvents()
        {
            this.colorTextBox.KeyDown -= ColorTextBox_KeyDown;
        }
        
        protected virtual void SetColorValue(Color newValue)
        {
            ValueChangingEventArgs valueChangingEventArgs = new ValueChangingEventArgs(newValue, this.value);
            this.OnValueChanging(valueChangingEventArgs);

            if (valueChangingEventArgs.Cancel)
            {
                return;
            }
            
            this.oldValue = this.value;
            this.value = newValue;
            this.colorBox.BackColor = newValue;
            
            if (newValue.IsNamedColor)
            {
                this.colorTextBox.Text = newValue.Name;
            }
            else
            {
                this.colorTextBox.Text = this.converter.ConvertToString(newValue);
            }

            EventArgs valueChangedEventArgs = new EventArgs();
            this.OnValueChanged(valueChangedEventArgs);
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the value is being changed. Cancelable event.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the editor is changing the value during the editing proccess.")]
        public event ValueChangingEventHandler ValueChanging;

        /// <summary>
        /// Occurs after the editor has changed the value during the editing process.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs after the editor has changed the value during the editing process.")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when the dialog window is closed.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the dialog window is closed.")]
        public event DialogClosedEventHandler DialogClosed;

        protected virtual void OnDialogClosed(DialogClosedEventArgs e)
        {
            if (this.DialogClosed != null)
            {
                this.DialogClosed(this, e);
            }
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            ValueChangingEventHandler handler = this.ValueChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    Color newValue = (Color)converter.ConvertFromString(this.colorTextBox.Text);
                    this.Value = newValue;
                }
                catch (Exception)
                {
                    this.colorTextBox.Focus();
                    this.colorTextBox.TextBoxItem.SelectAll();
                    
                    return;
                }
            }
            else if (e.KeyData == Keys.Escape)
            {
                this.Value = this.oldValue;
            }

            base.OnKeyDown(e);
        }

        #endregion

        #region Event Handlers

        private void ColorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        protected virtual void ButtonElement_Click(object sender, EventArgs e)
        {
            RadColorDialog colorDialog = new RadColorDialog();
            colorDialog.SelectedColor = this.value;
            DialogResult result = colorDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string convertedColor = this.converter.ConvertToString(colorDialog.SelectedColor);
                this.Value = (Color)this.converter.ConvertFromString(convertedColor);
            }

            this.OnDialogClosed(new DialogClosedEventArgs(result));
        }

        #endregion
    }
}
