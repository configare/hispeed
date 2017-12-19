using System;
using System.Drawing;

namespace Telerik.WinControls.UI
{ 
    /// <summary>
    /// Represents a numeric up/down editor element.
    /// </summary>
    public class BaseSpinEditorElement : RadSpinElement
    {
         #region Fields

        private bool setSpinValue;
        private bool enableValueChangingOnTextChanging = false;
        private bool valueChangingCancel = false;
        private int oldSelectionStart = 0;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the GridSpinEditorElement class.
        /// </summary>
        public BaseSpinEditorElement()
        {
            this.DefaultSize = new System.Drawing.Size(50, 20);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = true;
            this.NotifyParentOnMouseInput = false;
            this.Alignment = ContentAlignment.MiddleCenter;
        }

        #endregion

        #region Properties

        internal bool EnableValueChangingOnTextChanging
        {
            get { return this.enableValueChangingOnTextChanging; }
            set
            {
                if (value != this.enableValueChangingOnTextChanging)
                {
                    this.UnbindTextBoxItem();

                    if (value)
                    {
                        this.BindTextBoxItem();
                    }

                    this.enableValueChangingOnTextChanging = value;
                }
            }
        }

        #endregion

        #region Methods

        private void BindTextBoxItem()
        {
            this.TextBoxItem.TextChanging += new TextChangingEventHandler(TextBoxItem_TextChanging);
            this.TextBoxItem.HostedControl.TextChanged += new EventHandler(HostedControl_TextChanged);
        }

        private void UnbindTextBoxItem()
        {
            this.TextBoxItem.TextChanging -= new TextChangingEventHandler(TextBoxItem_TextChanging);
            this.TextBoxItem.HostedControl.TextChanged -= new EventHandler(HostedControl_TextChanged);
        }

        #endregion

        #region Event Handlers

        private void TextBoxItem_TextChanging(object sender, TextChangingEventArgs e)
        {
            this.oldSelectionStart = this.TextBoxItem.SelectionStart - 1;
            this.BitState[RadSpinElement.TextValueChangedStateKey] = true;
            this.EndTextEdit();
            e.Cancel = this.valueChangingCancel;
        }

        private void HostedControl_TextChanged(object sender, EventArgs e)
        {
            if (this.valueChangingCancel && this.oldSelectionStart >= 0)
            {
                this.TextBoxItem.SelectionStart = this.oldSelectionStart;
            }

            this.valueChangingCancel = false;

            if (string.IsNullOrEmpty(this.TextBoxItem.HostedControl.Text))
            {
                this.value = Decimal.MinValue;
                OnValueChanged(e);
            }
        }

        protected override void OnValueChanging(ValueChangingEventArgs e)
        {
            base.OnValueChanging(e);
            this.valueChangingCancel = e.Cancel;
        }

        public override void PerformStep(decimal step)
        {
            if (this.value == Decimal.MinValue)
            {
                this.value = 0m;
            }

            base.PerformStep(step);
        }

        #endregion

        #region Overrides

        protected override Type ThemeEffectiveType
        {
            get { return typeof(RadSpinElement); }
        }

        protected override void SetSpinValue(decimal value, bool fromValue)
        {
            if (setSpinValue)
            {
                return;
            }
            setSpinValue = true;

            decimal newValue = this.Constrain(value);
            bool valueChanged = this.value != newValue;

            if (this.value != newValue)
            {
                ValueChangingEventArgs args = new ValueChangingEventArgs(newValue, this.value);
                this.OnValueChanging(args);
                if (args.Cancel)
                {
                    setSpinValue = false;
                    return;
                }

                this.value = newValue;
            }

            if (fromValue)
            {
                this.BitState[SuppressEditorStateKey] = true;
                this.BitState[TextValueChangedStateKey] = false;

                if (this.TextBoxItem.HostedControl != null)
                {
                    this.Text = GetNumberText(this.value);
                }

                this.BitState[SuppressEditorStateKey] = false;
            }

            if (valueChanged)
            {
                this.OnNotifyPropertyChanged("Value");
                this.OnValueChanged(EventArgs.Empty);
            }

            setSpinValue = false;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            availableSize.Width = Math.Min(availableSize.Width, this.DefaultSize.Width);
            SizeF desiredSize = base.MeasureOverride(availableSize);
            if (desiredSize.Width < availableSize.Width &&
                desiredSize.Width < this.DefaultSize.Width)
            {
                desiredSize.Width = availableSize.Width;
            }
            return desiredSize;
        }

        #endregion
    }
}
