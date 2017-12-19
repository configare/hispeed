using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class BaseBrowseEditorElement : LightVisualElement
    {
        #region Fields

        private RadTextBoxElement filePathTextBox;
        private RadButtonElement openFileDialogButton;
        private StackLayoutElement stack;
        private OpenFileDialog openFileDialog;
        private string value;
        private string oldValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the editor.
        /// </summary>
        public virtual string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.SetFileValue(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="RadTextBoxElement"/> which displays the file path.
        /// </summary>
        public RadTextBoxElement FilePathTextBox
        {
            get
            {
                return this.filePathTextBox;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadButtonElement"/> that opens the <see cref="OpenFileDialog"/>.
        /// </summary>
        public RadButtonElement OpenFileDialogButton
        {
            get
            {
                return this.openFileDialogButton;
            }
        }

        /// <summary>
        /// Gets the <see cref="OpenFileDialog"/> that will open upon pressing the browse button.
        /// </summary>
        public OpenFileDialog OpenFileDialog
        {
            get
            {
                return this.openFileDialog;
            }
        }

        #endregion

        #region Initialization & Disposal

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = false;
            this.DrawBorder = false;
            this.value = null;
            this.oldValue = null;

            this.ShouldHandleMouseInput = true;
            this.NotifyParentOnMouseInput = false;

            this.Padding = new Padding(2);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.openFileDialog = new OpenFileDialog();
                        
            this.stack = new StackLayoutElement();
            this.stack.Orientation = Orientation.Horizontal;
            this.stack.StretchVertically = true;
            this.stack.FitInAvailableSize = true;
            this.stack.StretchHorizontally = true;
            this.stack.ShouldHandleMouseInput = false;
            this.stack.NotifyParentOnMouseInput = true;
            this.stack.ElementSpacing = 1;

            this.filePathTextBox = this.CreateFilePathTextBoxElement();
            this.openFileDialogButton = this.CreateOpenFileDialogButtonElement();

            this.stack.Children.Add(this.filePathTextBox);
            this.stack.Children.Add(this.openFileDialogButton);

            this.Children.Add(this.stack);

            this.WireEvents();
        }

        protected virtual RadTextBoxElement CreateFilePathTextBoxElement()
        {
            RadTextBoxElement element = new RadTextBoxElement();
            element.StretchHorizontally = true;
            element.StretchVertically = true;

            return element;
        }

        protected virtual RadButtonElement CreateOpenFileDialogButtonElement()
        {
            RadButtonElement element = new RadButtonElement();
            element.StretchHorizontally = false;
            element.StretchVertically = true;
            element.Text = ". . .";
            element.Click += new EventHandler(ButtonElement_Click);

            return element;
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
            if (this.filePathTextBox.Text == "(none)" || this.filePathTextBox.Text == "System.Drawing.Bitmap" || File.Exists(this.filePathTextBox.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Implementation

        protected virtual void WireEvents()
        {
            this.filePathTextBox.KeyDown += new KeyEventHandler(filePathTextBox_KeyDown);
        }

        protected virtual void UnwireEvents()
        {
            this.filePathTextBox.KeyDown -= filePathTextBox_KeyDown;
            this.openFileDialogButton.Click -= ButtonElement_Click;
        }

        protected virtual void SetFileValue(string newValue)
        {
            ValueChangingEventArgs args = new ValueChangingEventArgs(newValue, this.oldValue);
            this.OnValueChanging(args);

            if (args.Cancel)
            {
                return;
            }

            if (string.IsNullOrEmpty(newValue) || newValue == "(none)")
            {
                this.value = "(none)";
                this.filePathTextBox.Text = "(none)";
            }
            else if (File.Exists(newValue) && this.value != newValue)
            {
                this.oldValue = this.value;
                this.value = newValue;
                this.filePathTextBox.Text = newValue;

                EventArgs eventArgs = new EventArgs();
                this.OnValueChanged(eventArgs);
            }
            else
            {
                if (this.value == this.oldValue)
                {
                    return;
                }

                DialogResult result = RadMessageBox.Show("File not found", "Alert", MessageBoxButtons.OKCancel, RadMessageIcon.Exclamation);

                if (result == DialogResult.OK)
                {
                    this.filePathTextBox.Focus();
                    this.filePathTextBox.TextBoxItem.SelectAll();
                }
                else if (result == DialogResult.Cancel)
                {
                    this.SetFileValue(this.oldValue);
                    return;
                }
            }
        }
                
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the dialog window is closed.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the dialog window is closed.")]
        public event DialogClosedEventHandler DialogClosed;
                
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

        protected virtual void OnDialogClosed(DialogClosedEventArgs e)
        {
            if (this.DialogClosed != null)
            {
                this.DialogClosed(this, e);
            }
        }

        #endregion

        #region Event handlers
        
        private void ButtonElement_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.Value = this.openFileDialog.FileName;
            }

            this.OnDialogClosed(new DialogClosedEventArgs(result));
        }

        private void filePathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        #endregion
    }
}
