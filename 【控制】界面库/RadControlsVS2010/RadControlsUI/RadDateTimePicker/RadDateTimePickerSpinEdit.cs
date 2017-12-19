using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents the RadDateTimePickerSpinEdit class
	/// </summary>
	public class RadDateTimePickerSpinEdit : RadDateTimePickerBehaviorDirector
	{
		private RadDateTimePickerElement dateTimePickerElement;

		/// <summary>
		/// Represents the RadDateTimePickerSpinEdit constructor
		/// </summary>
		/// <param name="dateTimePicker"></param>
		public RadDateTimePickerSpinEdit(RadDateTimePickerElement dateTimePicker)
		{
			this.dateTimePickerElement = dateTimePicker;
		}

        /// <summary>
        /// Gets the instance of RadDateTimePickerElement associated to the control
        /// </summary>
        [Browsable(false)]
        [Description("Gets the instance of RadDateTimePickerElement associated to the control")]
        [Category("Behavior")]
        public override RadDateTimePickerElement DateTimePickerElement
        {
            get
            {
                return this.dateTimePickerElement;
            }
        }
     
        /// <summary>
        /// Sets the date shown in the textbox by a given value and format type.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatType"></param>
        public override void SetDateByValue(DateTime? date, DateTimePickerFormat formatType)
        {
            CultureInfo info = Thread.CurrentThread.CurrentCulture;

            maskEditValueChanged = false;

            Thread.CurrentThread.CurrentCulture = this.dateTimePickerElement.Culture;
            this.textBoxElement.Culture = this.dateTimePickerElement.Culture;

            if (date != this.dateTimePickerElement.NullDate)
            {
              
                switch (formatType)
                {
                    case DateTimePickerFormat.Time:
                        if (this.dateTimePickerElement.ShowCurrentTime)
                        {
                            if (!date.HasValue)
                            {
                                date = DateTime.Now;
                            }
                            else
                            {
                                date = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute,
                                                    DateTime.Now.Second, DateTime.Now.Millisecond, date.Value.Kind);
                            }
                        }
                        this.textBoxElement.Mask = "T";
                        break;
                    case DateTimePickerFormat.Short:
                        this.textBoxElement.Mask = "d";
                        break;
                    case DateTimePickerFormat.Long:
                        this.textBoxElement.Mask = "D";
                        break;
                    case DateTimePickerFormat.Custom:
                        this.textBoxElement.Mask = this.dateTimePickerElement.CustomFormat;
                        break;
                }

                if (!this.textBoxElement.Value.Equals(date))
                {
                    this.textBoxElement.Value = date;
                }

            }
            else
            {
                if (!this.textBoxElement.Value.Equals(date))
                {
                    this.textBoxElement.Value = date;
                }

                this.textBoxElement.Text = this.textBoxElement.TextBoxItem.NullText;
            }

            Thread.CurrentThread.CurrentCulture = info;
            maskEditValueChanged = true;
	
        }

        private bool maskEditValueChanged;
        private RadMaskedEditBoxElement textBoxElement;
		private RadRepeatArrowElement upButton;
		private RadRepeatArrowElement downButton;		
		private DockLayoutPanel dockLayout;
		private BorderPrimitive border;
		private FillPrimitive backGround;
		private RadCheckBoxElement checkBox;

		/// <summary>
		/// Creates dateTimePicker's children
		/// </summary>
		public override void CreateChildren()
		{
			this.textBoxElement = new RadMaskedEditBoxElement();
            this.textBoxElement.KeyDown += new KeyEventHandler(textBoxElement_KeyDown);
            this.textBoxElement.KeyPress += new KeyPressEventHandler(textBoxElement_KeyPress);
            this.textBoxElement.KeyUp += new KeyEventHandler(textBoxElement_KeyUp);

            this.textBoxElement.Mask = "";
            this.textBoxElement.MaskType = MaskType.DateTime;
            this.textBoxElement.ValueChanged += new EventHandler(maskBox_ValueChanged);
            this.textBoxElement.TextBoxItem.LostFocus += new EventHandler(maskBox_LostFocus);
            this.textBoxElement.TextBoxItem.TextChanged += new EventHandler(maskBox_TextChanged);
            this.textBoxElement.MouseDown += new MouseEventHandler(maskBox_MouseDown);
            this.textBoxElement.ShowBorder = false;
			this.textBoxElement.Class = "textbox";
            this.textBoxElement.ThemeRole = "DateTimePickerMaskTextBoxElement";

			this.dockLayout = new DockLayoutPanel();
			this.border = new BorderPrimitive();
			this.backGround = new FillPrimitive();
			this.checkBox = new RadCheckBoxElement();
			this.border.Class = "DateTimePickerBorder";
			this.backGround.Class = "DateTimePickerBackGround";
			this.backGround.GradientStyle = GradientStyles.Solid;

			this.checkBox.SetValue(DockLayoutPanel.DockProperty, Dock.Left);
			this.checkBox.Children[1].Alignment = ContentAlignment.MiddleLeft;

			this.checkBox.StretchHorizontally = false;
			this.textBoxElement.Alignment = ContentAlignment.MiddleLeft;

			this.upButton = new RadRepeatArrowElement();
            this.upButton.ThemeRole = "UpButton";
			this.upButton.Padding = new Padding(3, 1, 3, 1);
			this.upButton.Border.Visibility = ElementVisibility.Visible;
			this.upButton.Click += new EventHandler(upButton_Click);
			this.upButton.Direction = ArrowDirection.Up;
			this.upButton.Arrow.AutoSize = true;
			this.upButton.CanFocus = false;

			this.downButton = new RadRepeatArrowElement();
            this.downButton.ThemeRole = "DownButton";
			this.downButton.Padding = new Padding(3, 1, 3, 0);
			this.downButton.Border.Visibility = ElementVisibility.Visible;
			this.downButton.Click += new EventHandler(downButton_Click);
			this.downButton.Arrow.AutoSize = true;
			this.downButton.Direction = ArrowDirection.Down;
			this.downButton.CanFocus = false;

			BoxLayout stackLayout = new BoxLayout();

			stackLayout.Orientation = Orientation.Vertical;
			BoxLayout.SetProportion(this.upButton, 1);
            BoxLayout.SetProportion(this.downButton, 1);
			stackLayout.Children.Add(this.upButton);
			stackLayout.Children.Add(this.downButton);

			if (this.dateTimePickerElement.RightToLeft)
			{
				this.checkBox.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
				this.checkBox.Children[1].Alignment = ContentAlignment.MiddleLeft;
				stackLayout.SetValue(DockLayoutPanel.DockProperty, Dock.Left);
				stackLayout.RightToLeft = false;
                this.textBoxElement.TextBoxItem.HostedControl.RightToLeft = RightToLeft.Yes;
			}
			else
			{
				this.checkBox.SetValue(DockLayoutPanel.DockProperty, Dock.Left);
				this.checkBox.Children[1].Alignment = ContentAlignment.MiddleLeft;
				stackLayout.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
                this.textBoxElement.TextBoxItem.HostedControl.RightToLeft = RightToLeft.No;
            }

			this.dockLayout.Children.Add(this.checkBox);
			this.dockLayout.Children.Add(stackLayout);
			this.dockLayout.Children.Add(this.textBoxElement);
			
			this.dateTimePickerElement.Children.Add(this.backGround);
			this.dateTimePickerElement.Children.Add(this.dockLayout);
			this.dateTimePickerElement.Children.Add(this.border);

			this.dateTimePickerElement.checkBox = this.checkBox;
            
			if (!this.dateTimePickerElement.ShowCheckBox)
			{
				this.checkBox.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
			}

			this.SetDateByValue(this.dateTimePickerElement.Value, this.dateTimePickerElement.Format);

            // The BorderPrimitive of the RadTextBoxElement should be collapsed. This makes the size of the editors equal
			this.textBoxElement.Children[this.textBoxElement.Children.Count - 1].Visibility = ElementVisibility.Collapsed;
			this.border.Visibility = ElementVisibility.Visible;

			this.SetDateByValue(this.dateTimePickerElement.Value, this.dateTimePickerElement.Format);
		
		}

        void maskBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate))
            {
                  this.textBoxElement.TextBoxItem.Text = "";
            }
        }

        void maskBox_TextChanged(object sender, EventArgs e)
        {
            if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate))
            {
                if (!String.IsNullOrEmpty(this.textBoxElement.Text))
                {
                    this.textBoxElement.Text = "";
                }
            }
        }


        private void textBoxElement_KeyUp(object sender, KeyEventArgs e)
        {
            this.DateTimePickerElement.CallRaiseKeyUp(e);
        }

        private void textBoxElement_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.DateTimePickerElement.CallRaiseKeyPress(e);
        }

        private void textBoxElement_KeyDown(object sender, KeyEventArgs e)
        {
            this.DateTimePickerElement.CallRaiseKeyDown(e);
        }

        //private void maskBox_LostFocus(object sender, EventArgs e)
        //{
        //    if (this.DateTimePickerElement.Value.Equals(this.DateTimePickerElement.NullDate))
        //    {
        //        this.textBoxElement.Text = "";
        //    }
        //}


        private void maskBox_LostFocus(object sender, EventArgs e)
        {
            DateTime value = (DateTime)this.textBoxElement.Value;
            if (value.Equals(this.DateTimePickerElement.NullDate))
            {
                this.textBoxElement.TextBoxItem.Text = "";
                return;
            }

            if (value != this.DateTimePickerElement.NullDate &&
                ((value < this.DateTimePickerElement.MinDate) || (value > this.DateTimePickerElement.MaxDate)))
            {
                if (value < this.DateTimePickerElement.MinDate)
                {
                    value = this.DateTimePickerElement.MinDate;
                }
                else if (value > this.DateTimePickerElement.MaxDate)
                {
                    value = this.DateTimePickerElement.MaxDate;
                }
            }

            ValueChangingEventArgs args = new ValueChangingEventArgs(value, this.DateTimePickerElement.Value);
            this.DateTimePickerElement.CallOnValueChanging(args);

            if (args.Cancel)
            {
                return;
            }

            this.textBoxElement.Value = value;
        }

        internal RadMaskedEditBoxElement TextBoxElement
		{
			get
			{
				return this.textBoxElement;
			}
		}

        private void downButton_Click(object sender, EventArgs e)
		{ 
            this.textBoxElement.Down();
		}

        private void upButton_Click(object sender, EventArgs e)
		{
            this.textBoxElement.Up();
		}

        private void maskBox_ValueChanged(object sender, EventArgs e)
		{
            if (maskEditValueChanged)
            {
                if ((DateTime)this.textBoxElement.Value >= this.DateTimePickerElement.MinDate && (DateTime)this.textBoxElement.Value <= this.dateTimePickerElement.MaxDate)
                    this.DateTimePickerElement.Value = (DateTime)this.textBoxElement.Value;
            }
        }
	}
}
