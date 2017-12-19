using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Telerik.WinControls.UI.Design
{
	internal class FormatControl : UserControl
	{
		// Fields
		private const int CurrencyIndex = 2;
		private const int CustomIndex = 5;
		private TextBox customStringTextBox;
		private ListBox dateTimeFormatsListBox;
		private static DateTime dateTimeFormatValue;
		private const int DateTimeIndex = 3;
		private NumericUpDown decimalPlacesUpDown;
		private bool dirty;
		private Label explanationLabel;
		private GroupBox formatGroupBox;
		private Label formatTypeLabel;
		private ListBox formatTypeListBox;
		private bool loaded;
		private const int NoFormattingIndex = 0;
		private Label nullValueLabel;
		private TextBox nullValueTextBox;
		private const int NumericIndex = 1;
		private GroupBox sampleGroupBox;
		private Label sampleLabel;
		private const int ScientificIndex = 4;
		private Label secondRowLabel;
		private TableLayoutPanel tableLayoutPanel1;
		private Label thirdRowLabel;

		// Methods
		static FormatControl()
		{
			FormatControl.dateTimeFormatValue = DateTime.Now;
		}

		public FormatControl()
		{
			this.customStringTextBox = new TextBox();
			this.InitializeComponent();
		}

		private void customStringTextBox_TextChanged(object sender, EventArgs e)
		{
			CustomFormatType type1 = this.formatTypeListBox.SelectedItem as CustomFormatType;
			this.sampleLabel.Text = type1.SampleString;
			this.dirty = true;
		}

		private void dateTimeFormatsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			FormatTypeClass class1 = this.formatTypeListBox.SelectedItem as FormatTypeClass;
			this.sampleLabel.Text = class1.SampleString;
			this.dirty = true;
		}

		private void decimalPlacesUpDown_ValueChanged(object sender, EventArgs e)
		{
			FormatTypeClass class1 = this.formatTypeListBox.SelectedItem as FormatTypeClass;
			this.sampleLabel.Text = class1.SampleString;
			this.dirty = true;
		}

		private void FormatControl_Load(object sender, EventArgs e)
		{
			if (!this.loaded)
			{
				this.nullValueLabel.Text = "&Null value:";
				int num1 = this.nullValueLabel.Width;
				int num2 = this.nullValueLabel.Height;
				this.secondRowLabel.Text = "&Decimal places";
				num1 = Math.Max(num1, this.secondRowLabel.Width);
				num2 = Math.Max(num2, this.secondRowLabel.Height);
				this.secondRowLabel.Text = "Custo&m format";
				num1 = Math.Max(num1, this.secondRowLabel.Width);
				num2 = Math.Max(num2, this.secondRowLabel.Height);
				this.nullValueLabel.MinimumSize = new Size(num1, num2);
				this.secondRowLabel.MinimumSize = new Size(num1, num2);
				this.formatTypeListBox.SelectedIndexChanged -= new EventHandler(this.formatTypeListBox_SelectedIndexChanged);
				this.formatTypeListBox.Items.Clear();
				this.formatTypeListBox.Items.Add(new NoFormattingFormatType());
				this.formatTypeListBox.Items.Add(new NumericFormatType(this));
				this.formatTypeListBox.Items.Add(new CurrencyFormatType(this));
				this.formatTypeListBox.Items.Add(new DateTimeFormatType(this));
				this.formatTypeListBox.Items.Add(new ScientificFormatType(this));
				this.formatTypeListBox.Items.Add(new CustomFormatType(this));
				this.formatTypeListBox.SelectedIndex = 0;
				this.formatTypeListBox.SelectedIndexChanged += new EventHandler(this.formatTypeListBox_SelectedIndexChanged);
				this.UpdateCustomStringTextBox();
				this.UpdateTBLHeight();
				this.UpdateFormatTypeListBoxHeight();
				this.UpdateFormatTypeListBoxItems();
				this.UpdateControlVisibility(this.formatTypeListBox.SelectedItem as FormatTypeClass);
				this.sampleLabel.Text = (this.formatTypeListBox.SelectedItem as FormatTypeClass).SampleString;
				this.explanationLabel.Size = new Size(this.formatGroupBox.Width - 10, 30);
				this.explanationLabel.Text = (this.formatTypeListBox.SelectedItem as FormatTypeClass).TopLabelString;
				this.dirty = false;
				this.FormatControlFinishedLoading();
				this.loaded = true;
			}
		}

		private void FormatControlFinishedLoading()
		{
			FormatStringDialog dialog2 = null;
			for (Control control1 = base.Parent; control1 != null; control1 = control1.Parent)
			{
				dialog2 = control1 as FormatStringDialog;
				if (dialog2 != null)
				{
					break;
				}
			}
			if (dialog2 != null)
			{
				dialog2.FormatControlFinishedLoading();
			}
		}

		private void formatGroupBox_Enter(object sender, EventArgs e)
		{
		}

		private void formatTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			FormatTypeClass class1 = this.formatTypeListBox.SelectedItem as FormatTypeClass;
			this.UpdateControlVisibility(class1);
			this.sampleLabel.Text = class1.SampleString;
			this.explanationLabel.Text = class1.TopLabelString;
			this.dirty = true;
		}

		public static string FormatTypeStringFromFormatString(string formatString)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				return "No Formatting";
			}
			if (NumericFormatType.ParseStatic(formatString))
			{
				return "Numeric";
			}
			if (CurrencyFormatType.ParseStatic(formatString))
			{
				return "Currency";
			}
			if (DateTimeFormatType.ParseStatic(formatString))
			{
				return "Date Time";
			}
			if (ScientificFormatType.ParseStatic(formatString))
			{
				return "Scientific";
			}
			return "Custom";
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatControl));
			this.formatGroupBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.nullValueLabel = new System.Windows.Forms.Label();
			this.nullValueTextBox = new System.Windows.Forms.TextBox();
			this.decimalPlacesUpDown = new System.Windows.Forms.NumericUpDown();
			this.secondRowLabel = new System.Windows.Forms.Label();
			this.thirdRowLabel = new System.Windows.Forms.Label();
			this.dateTimeFormatsListBox = new System.Windows.Forms.ListBox();
			this.sampleGroupBox = new System.Windows.Forms.GroupBox();
			this.sampleLabel = new System.Windows.Forms.Label();
			this.formatTypeListBox = new System.Windows.Forms.ListBox();
			this.formatTypeLabel = new System.Windows.Forms.Label();
			this.explanationLabel = new System.Windows.Forms.Label();
			this.formatGroupBox.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.decimalPlacesUpDown)).BeginInit();
			this.sampleGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// formatGroupBox
			// 
			this.formatGroupBox.Controls.Add(this.tableLayoutPanel1);
			this.formatGroupBox.Controls.Add(this.sampleGroupBox);
			this.formatGroupBox.Controls.Add(this.formatTypeListBox);
			this.formatGroupBox.Controls.Add(this.formatTypeLabel);
			this.formatGroupBox.Controls.Add(this.explanationLabel);
			this.formatGroupBox.Location = new System.Drawing.Point(0, 0);
			this.formatGroupBox.Margin = new System.Windows.Forms.Padding(0);
			this.formatGroupBox.Name = "formatGroupBox";
			this.formatGroupBox.Size = new System.Drawing.Size(391, 267);
			this.formatGroupBox.TabIndex = 0;
			this.formatGroupBox.TabStop = false;
			this.formatGroupBox.Text = "Format";
			this.formatGroupBox.Enter += new System.EventHandler(this.formatGroupBox_Enter);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.nullValueLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.nullValueTextBox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.decimalPlacesUpDown, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.secondRowLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.thirdRowLabel, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.dateTimeFormatsListBox, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(129, 109);
			this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(256, 145);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(256, 145);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// nullValueLabel
			// 
			this.nullValueLabel.Location = new System.Drawing.Point(3, 0);
			this.nullValueLabel.MinimumSize = new System.Drawing.Size(81, 14);
			this.nullValueLabel.Name = "nullValueLabel";
			this.nullValueLabel.Size = new System.Drawing.Size(101, 26);
			this.nullValueLabel.TabIndex = 1;
			this.nullValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nullValueTextBox
			// 
			this.nullValueTextBox.Location = new System.Drawing.Point(107, 3);
			this.nullValueTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.nullValueTextBox.Name = "nullValueTextBox";
			this.nullValueTextBox.Size = new System.Drawing.Size(144, 20);
			this.nullValueTextBox.TabIndex = 2;
			this.nullValueTextBox.TextChanged += new System.EventHandler(this.nullValueTextBox_TextChanged);
			// 
			// decimalPlacesUpDown
			// 
			this.decimalPlacesUpDown.Location = new System.Drawing.Point(107, 29);
			this.decimalPlacesUpDown.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.decimalPlacesUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this.decimalPlacesUpDown.Name = "decimalPlacesUpDown";
			this.decimalPlacesUpDown.Size = new System.Drawing.Size(144, 20);
			this.decimalPlacesUpDown.TabIndex = 3;
			this.decimalPlacesUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.decimalPlacesUpDown.ValueChanged += new System.EventHandler(this.decimalPlacesUpDown_ValueChanged);
			// 
			// secondRowLabel
			// 
			this.secondRowLabel.Location = new System.Drawing.Point(3, 26);
			this.secondRowLabel.MinimumSize = new System.Drawing.Size(81, 14);
			this.secondRowLabel.Name = "secondRowLabel";
			this.secondRowLabel.Size = new System.Drawing.Size(101, 26);
			this.secondRowLabel.TabIndex = 0;
			this.secondRowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// thirdRowLabel
			// 
			this.thirdRowLabel.Location = new System.Drawing.Point(110, 52);
			this.thirdRowLabel.MaximumSize = new System.Drawing.Size(250, 0);
			this.thirdRowLabel.Name = "thirdRowLabel";
			this.thirdRowLabel.Size = new System.Drawing.Size(0, 13);
			this.thirdRowLabel.TabIndex = 4;
			this.thirdRowLabel.Text = resources.GetString("thirdRowLabel.Text");
			// 
			// dateTimeFormatsListBox
			// 
			this.dateTimeFormatsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dateTimeFormatsListBox.FormattingEnabled = true;
			this.dateTimeFormatsListBox.Location = new System.Drawing.Point(3, 52);
			this.dateTimeFormatsListBox.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.dateTimeFormatsListBox.Name = "dateTimeFormatsListBox";
			this.dateTimeFormatsListBox.Size = new System.Drawing.Size(104, 82);
			this.dateTimeFormatsListBox.TabIndex = 5;
			// 
			// sampleGroupBox
			// 
			this.sampleGroupBox.Controls.Add(this.sampleLabel);
			this.sampleGroupBox.Location = new System.Drawing.Point(129, 58);
			this.sampleGroupBox.MinimumSize = new System.Drawing.Size(250, 38);
			this.sampleGroupBox.Name = "sampleGroupBox";
			this.sampleGroupBox.Padding = new System.Windows.Forms.Padding(0);
			this.sampleGroupBox.Size = new System.Drawing.Size(251, 43);
			this.sampleGroupBox.TabIndex = 1;
			this.sampleGroupBox.TabStop = false;
			// 
			// sampleLabel
			// 
			this.sampleLabel.AutoSize = true;
			this.sampleLabel.Location = new System.Drawing.Point(3, 16);
			this.sampleLabel.Name = "sampleLabel";
			this.sampleLabel.Size = new System.Drawing.Size(0, 13);
			this.sampleLabel.TabIndex = 0;
			// 
			// formatTypeListBox
			// 
			this.formatTypeListBox.FormattingEnabled = true;
			this.formatTypeListBox.Location = new System.Drawing.Point(3, 74);
			this.formatTypeListBox.Name = "formatTypeListBox";
			this.formatTypeListBox.Size = new System.Drawing.Size(120, 160);
			this.formatTypeListBox.TabIndex = 2;
			this.formatTypeListBox.SelectedIndexChanged += new System.EventHandler(this.formatTypeListBox_SelectedIndexChanged);
			// 
			// formatTypeLabel
			// 
			this.formatTypeLabel.AutoSize = true;
			this.formatTypeLabel.Location = new System.Drawing.Point(3, 58);
			this.formatTypeLabel.Name = "formatTypeLabel";
			this.formatTypeLabel.Size = new System.Drawing.Size(65, 13);
			this.formatTypeLabel.TabIndex = 3;
			this.formatTypeLabel.Text = "&Format type:";
			// 
			// explanationLabel
			// 
			this.explanationLabel.Location = new System.Drawing.Point(3, 19);
			this.explanationLabel.MinimumSize = new System.Drawing.Size(0, 30);
			this.explanationLabel.Name = "explanationLabel";
			this.explanationLabel.Size = new System.Drawing.Size(381, 30);
			this.explanationLabel.TabIndex = 4;
			// 
			// FormatControl
			// 
			this.Controls.Add(this.formatGroupBox);
			this.MinimumSize = new System.Drawing.Size(390, 237);
			this.Name = "FormatControl";
			this.Size = new System.Drawing.Size(391, 268);
			this.Load += new System.EventHandler(this.FormatControl_Load);
			this.formatGroupBox.ResumeLayout(false);
			this.formatGroupBox.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.decimalPlacesUpDown)).EndInit();
			this.sampleGroupBox.ResumeLayout(false);
			this.sampleGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		private void nullValueTextBox_TextChanged(object sender, EventArgs e)
		{
			this.dirty = true;
		}

		protected override bool ProcessMnemonic(char charCode)
		{
			if (Control.IsMnemonic(charCode, this.formatTypeLabel.Text))
			{
				this.formatTypeListBox.Focus();
				return true;
			}
			if (Control.IsMnemonic(charCode, this.nullValueLabel.Text))
			{
				this.nullValueTextBox.Focus();
				return true;
			}
			switch (this.formatTypeListBox.SelectedIndex)
			{
				case 0:
					{
						return false;
					}
				case 1:
				case 2:
				case 4:
					{
						if (!Control.IsMnemonic(charCode, this.secondRowLabel.Text))
						{
							return false;
						}
						this.decimalPlacesUpDown.Focus();
						return true;
					}
				case 3:
					{
						if (!Control.IsMnemonic(charCode, this.secondRowLabel.Text))
						{
							return false;
						}
						this.dateTimeFormatsListBox.Focus();
						return true;
					}
				case 5:
					{
						if (!Control.IsMnemonic(charCode, this.secondRowLabel.Text))
						{
							return false;
						}
						this.customStringTextBox.Focus();
						return true;
					}
			}
			return false;
		}

		public void ResetFormattingInfo()
		{
			this.decimalPlacesUpDown.ValueChanged -= new EventHandler(this.decimalPlacesUpDown_ValueChanged);
			this.customStringTextBox.TextChanged -= new EventHandler(this.customStringTextBox_TextChanged);
			this.dateTimeFormatsListBox.SelectedIndexChanged -= new EventHandler(this.dateTimeFormatsListBox_SelectedIndexChanged);
			this.formatTypeListBox.SelectedIndexChanged -= new EventHandler(this.formatTypeListBox_SelectedIndexChanged);
			this.decimalPlacesUpDown.Value = new decimal(2);
			this.nullValueTextBox.Text = string.Empty;
			this.dateTimeFormatsListBox.SelectedIndex = -1;
			this.formatTypeListBox.SelectedIndex = -1;
			this.customStringTextBox.Text = string.Empty;
			this.decimalPlacesUpDown.ValueChanged += new EventHandler(this.decimalPlacesUpDown_ValueChanged);
			this.customStringTextBox.TextChanged += new EventHandler(this.customStringTextBox_TextChanged);
			this.dateTimeFormatsListBox.SelectedIndexChanged += new EventHandler(this.dateTimeFormatsListBox_SelectedIndexChanged);
			this.formatTypeListBox.SelectedIndexChanged += new EventHandler(this.formatTypeListBox_SelectedIndexChanged);
		}

		private void UpdateControlVisibility(FormatTypeClass formatType)
		{
			if (formatType == null)
			{
				this.explanationLabel.Visible = false;
				this.sampleLabel.Visible = false;
				this.nullValueLabel.Visible = false;
				this.secondRowLabel.Visible = false;
				this.nullValueTextBox.Visible = false;
				this.thirdRowLabel.Visible = false;
				this.dateTimeFormatsListBox.Visible = false;
				this.customStringTextBox.Visible = false;
				this.decimalPlacesUpDown.Visible = false;
			}
			else
			{
				this.tableLayoutPanel1.SuspendLayout();
				this.secondRowLabel.Text = "";
				if (formatType.DropDownVisible)
				{
					this.secondRowLabel.Text = "&Decimal places";
					this.decimalPlacesUpDown.Visible = true;
				}
				else
				{
					this.decimalPlacesUpDown.Visible = false;
				}
				if (formatType.FormatStringTextBoxVisible)
				{
					this.secondRowLabel.Text = "Custo&m format";
					if (this.tableLayoutPanel1.Controls.Contains(this.dateTimeFormatsListBox))
					{
						this.tableLayoutPanel1.Controls.Remove(this.dateTimeFormatsListBox);
					}
					this.thirdRowLabel.Visible = true;
					this.tableLayoutPanel1.SetColumn(this.thirdRowLabel, 0);
					this.tableLayoutPanel1.SetRow(this.thirdRowLabel, 2);
					this.tableLayoutPanel1.SetColumnSpan(this.thirdRowLabel, 2);
					this.customStringTextBox.Visible = true;					
					this.tableLayoutPanel1.Controls.Add(this.customStringTextBox, 1, 1);
				}
				else
				{
					this.thirdRowLabel.Visible = false;
					this.customStringTextBox.Visible = false;
				}
				if (formatType.ListBoxVisible)
				{
					this.secondRowLabel.Text = "&Type:";
					if (this.tableLayoutPanel1.Controls.Contains(this.customStringTextBox))
					{
						this.tableLayoutPanel1.Controls.Remove(this.customStringTextBox);
					}
					this.dateTimeFormatsListBox.Visible = true;
					this.tableLayoutPanel1.Controls.Add(this.dateTimeFormatsListBox, 0, 2);
					this.tableLayoutPanel1.SetColumn(this.dateTimeFormatsListBox, 0);
					this.tableLayoutPanel1.SetColumnSpan(this.dateTimeFormatsListBox, 2);
				}
				else
				{
					this.dateTimeFormatsListBox.Visible = false;
				}
				this.tableLayoutPanel1.ResumeLayout(true);
			}
		}

		private void UpdateCustomStringTextBox()
		{
			this.customStringTextBox = new TextBox();
			this.customStringTextBox.AccessibleDescription = "Custom format";
			this.customStringTextBox.Margin = new Padding(0, 3, 0, 3);
			this.customStringTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
			this.customStringTextBox.TabIndex = 3;
			this.customStringTextBox.TextChanged += new EventHandler(this.customStringTextBox_TextChanged);
		}

		private void UpdateFormatTypeListBoxHeight()
		{
			this.formatTypeListBox.Height = this.tableLayoutPanel1.Bottom - this.formatTypeListBox.Top;
		}

		private void UpdateFormatTypeListBoxItems()
		{
			this.dateTimeFormatsListBox.SelectedIndexChanged -= new EventHandler(this.dateTimeFormatsListBox_SelectedIndexChanged);
			this.dateTimeFormatsListBox.Items.Clear();
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "d"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "D"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "f"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "F"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "g"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "G"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "t"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "T"));
			this.dateTimeFormatsListBox.Items.Add(new DateTimeFormatsListBoxItem(FormatControl.dateTimeFormatValue, "M"));
			this.dateTimeFormatsListBox.SelectedIndex = 0;
			this.dateTimeFormatsListBox.SelectedIndexChanged += new EventHandler(this.dateTimeFormatsListBox_SelectedIndexChanged);
		}

		private void UpdateTBLHeight()
		{
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.Controls.Add(this.customStringTextBox, 1, 1);
			this.customStringTextBox.Visible = false;
			this.thirdRowLabel.MaximumSize = new Size(this.tableLayoutPanel1.Width, 0);
			this.dateTimeFormatsListBox.Visible = false;
			this.tableLayoutPanel1.SetColumn(this.thirdRowLabel, 0);
			this.tableLayoutPanel1.SetColumnSpan(this.thirdRowLabel, 2);
			this.thirdRowLabel.AutoSize = true;
			this.tableLayoutPanel1.ResumeLayout(true);
			this.tableLayoutPanel1.MinimumSize = new Size(this.tableLayoutPanel1.Width, this.tableLayoutPanel1.Height);
		}


		// Properties
		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		public string FormatType
		{
			get
			{
				FormatTypeClass class1 = this.formatTypeListBox.SelectedItem as FormatTypeClass;
				if (class1 != null)
				{
					return class1.ToString();
				}
				return string.Empty;
			}
			set
			{
				this.formatTypeListBox.SelectedIndex = 0;
				for (int num1 = 0; num1 < this.formatTypeListBox.Items.Count; num1++)
				{
					FormatTypeClass class1 = this.formatTypeListBox.Items[num1] as FormatTypeClass;
					if (class1.ToString().Equals(value))
					{
						this.formatTypeListBox.SelectedIndex = num1;
					}
				}
			}
		}

		public FormatTypeClass FormatTypeItem
		{
			get
			{
				return this.formatTypeListBox.SelectedItem as FormatTypeClass;
			}
		}

		public string NullValue
		{
			get
			{
				string text1 = this.nullValueTextBox.Text.Trim();
				if (text1.Length != 0)
				{
					return text1;
				}
				return null;
			}
			set
			{
				this.nullValueTextBox.TextChanged -= new EventHandler(this.nullValueTextBox_TextChanged);
				this.nullValueTextBox.Text = value;
				this.nullValueTextBox.TextChanged += new EventHandler(this.nullValueTextBox_TextChanged);
			}
		}

		public bool NullValueTextBoxEnabled
		{
			set
			{
				this.nullValueTextBox.Enabled = value;
			}
		}	

		// Nested Types
		private class CurrencyFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public CurrencyFormatType(FormatControl owner)
			{
				this.owner = owner;
			}

			public override bool Parse(string formatString)
			{
				return FormatControl.CurrencyFormatType.ParseStatic(formatString);
			}

			public static bool ParseStatic(string formatString)
			{
				if (((!formatString.Equals("C0") && !formatString.Equals("C1")) && (!formatString.Equals("C2") && !formatString.Equals("C3"))) && (!formatString.Equals("C4") && !formatString.Equals("C5")))
				{
					return formatString.Equals("C6");
				}
				return true;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
				if (formatString.Equals("C0"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(0);
				}
				else if (formatString.Equals("C1"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(1);
				}
				else if (formatString.Equals("C2"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(2);
				}
				else if (formatString.Equals("C3"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(3);
				}
				else if (formatString.Equals("C4"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(4);
				}
				else if (formatString.Equals("C5"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(5);
				}
				else if (formatString.Equals("C6"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(6);
				}
			}

			public override string ToString()
			{
				return "Currency";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return true;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					switch ((int)this.owner.decimalPlacesUpDown.Value)
					{
						case 0:
							{
								return "C0";
							}
						case 1:
							{
								return "C1";
							}
						case 2:
							{
								return "C2";
							}
						case 3:
							{
								return "C3";
							}
						case 4:
							{
								return "C4";
							}
						case 5:
							{
								return "C5";
							}
						case 6:
							{
								return "C6";
							}
					}
					return "";
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override string SampleString
			{
				get
				{
					double num1 = -1234.5678;
					return num1.ToString(this.FormatString, CultureInfo.CurrentCulture);
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Specify the format for monetary values.";
				}
			}


			// Fields
			private FormatControl owner;
		}

		private class CustomFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public CustomFormatType(FormatControl owner)
			{
				this.owner = owner;
			}

			public override bool Parse(string formatString)
			{
				return FormatControl.CustomFormatType.ParseStatic(formatString);
			}

			public static bool ParseStatic(string formatString)
			{
				return true;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
				this.owner.customStringTextBox.Text = formatString;
			}

			public override string ToString()
			{
				return "Custom";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return false;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					return this.owner.customStringTextBox.Text;
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return true;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override string SampleString
			{
				get
				{
					string text1 = this.FormatString;
					if (string.IsNullOrEmpty(text1))
					{
						return "";
					}
					string text2 = "";
					if (FormatControl.DateTimeFormatType.ParseStatic(text1))
					{
						text2 = FormatControl.dateTimeFormatValue.ToString(text1, CultureInfo.CurrentCulture);
					}
					if (text2.Equals(""))
					{
						try
						{
							double num1 = -1234.5678;
							text2 = num1.ToString(text1, CultureInfo.CurrentCulture);
						}
						catch (FormatException)
						{
							text2 = "";
						}
					}
					if (text2.Equals(""))
					{
						try
						{
							int num2 = -1234;
							text2 = num2.ToString(text1, CultureInfo.CurrentCulture);
						}
						catch (FormatException)
						{
							text2 = "";
						}
					}
					if (text2.Equals(""))
					{
						try
						{
							text2 = FormatControl.dateTimeFormatValue.ToString(text1, CultureInfo.CurrentCulture);
						}
						catch (FormatException)
						{
							text2 = "";
						}
					}
					if (text2.Equals(""))
					{
						text2 = "Invalid format";
					}
					return text2;
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Type a custom format string. A custom string may require extra handling unless it is a read-only value.";
				}
			}


			// Fields
			private FormatControl owner;
		}

		private class DateTimeFormatsListBoxItem
		{
			// Methods
			public DateTimeFormatsListBoxItem(DateTime value, string formatString)
			{
				this.value = value;
				this.formatString = formatString;
			}

			public override string ToString()
			{
				return this.value.ToString(this.formatString, CultureInfo.CurrentCulture);
			}


			// Properties
			public string FormatString
			{
				get
				{
					return this.formatString;
				}
			}


			// Fields
			private string formatString;
			private DateTime value;
		}

		private class DateTimeFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public DateTimeFormatType(FormatControl owner)
			{
				this.owner = owner;
			}

			public override bool Parse(string formatString)
			{
				return FormatControl.DateTimeFormatType.ParseStatic(formatString);
			}

			public static bool ParseStatic(string formatString)
			{
				if (((!formatString.Equals("d") && !formatString.Equals("D")) && (!formatString.Equals("f") && !formatString.Equals("F"))) && ((!formatString.Equals("g") && !formatString.Equals("G")) && (!formatString.Equals("t") && !formatString.Equals("T"))))
				{
					return formatString.Equals("M");
				}
				return true;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
				int num1 = -1;
				if (formatString.Equals("d"))
				{
					num1 = 0;
				}
				else if (formatString.Equals("D"))
				{
					num1 = 1;
				}
				else if (formatString.Equals("f"))
				{
					num1 = 2;
				}
				else if (formatString.Equals("F"))
				{
					num1 = 3;
				}
				else if (formatString.Equals("g"))
				{
					num1 = 4;
				}
				else if (formatString.Equals("G"))
				{
					num1 = 5;
				}
				else if (formatString.Equals("t"))
				{
					num1 = 6;
				}
				else if (formatString.Equals("T"))
				{
					num1 = 7;
				}
				else if (formatString.Equals("M"))
				{
					num1 = 8;
				}
				this.owner.dateTimeFormatsListBox.SelectedIndex = num1;
			}

			public override string ToString()
			{
				return "Date Time";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return false;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					FormatControl.DateTimeFormatsListBoxItem item1 = this.owner.dateTimeFormatsListBox.SelectedItem as FormatControl.DateTimeFormatsListBoxItem;
					return item1.FormatString;
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return true;
				}
			}

			public override string SampleString
			{
				get
				{
					if (this.owner.dateTimeFormatsListBox.SelectedItem == null)
					{
						return "";
					}
					return FormatControl.dateTimeFormatValue.ToString(this.FormatString, CultureInfo.CurrentCulture);
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Specify the format for date and time values.";
				}
			}


			// Fields
			private FormatControl owner;
		}

		public abstract class FormatTypeClass
		{
			// Methods
			protected FormatTypeClass()
			{
			}

			public abstract bool Parse(string formatString);

			public abstract void PushFormatStringIntoFormatType(string formatString);


			// Properties
			public abstract bool DropDownVisible { get; }

			public abstract bool FormatLabelVisible { get; }

			public abstract string FormatString { get; }

			public abstract bool FormatStringTextBoxVisible { get; }

			public abstract bool ListBoxVisible { get; }

			public abstract string SampleString { get; }

			public abstract string TopLabelString { get; }

		}

		private class NoFormattingFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public NoFormattingFormatType()
			{
			}

			public override bool Parse(string formatString)
			{
				return false;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
			}

			public override string ToString()
			{
				return "No Formatting";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return false;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					return "";
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override string SampleString
			{
				get
				{
					return "-1234.5";
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Use no formatting to display the value from the source without adornment.";
				}
			}

		}

		private class NumericFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public NumericFormatType(FormatControl owner)
			{
				this.owner = owner;
			}

			public override bool Parse(string formatString)
			{
				return FormatControl.NumericFormatType.ParseStatic(formatString);
			}

			public static bool ParseStatic(string formatString)
			{
				if (((!formatString.Equals("N0") && !formatString.Equals("N1")) && (!formatString.Equals("N2") && !formatString.Equals("N3"))) && (!formatString.Equals("N4") && !formatString.Equals("N5")))
				{
					return formatString.Equals("N6");
				}
				return true;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
				if (formatString.Equals("N0"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(0);
				}
				else if (formatString.Equals("N1"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(1);
				}
				else if (formatString.Equals("N2"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(2);
				}
				else if (formatString.Equals("N3"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(3);
				}
				else if (formatString.Equals("N4"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(4);
				}
				else if (formatString.Equals("N5"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(5);
				}
				else if (formatString.Equals("N6"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(6);
				}
			}

			public override string ToString()
			{
				return "Numeric";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return true;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					switch ((int)this.owner.decimalPlacesUpDown.Value)
					{
						case 0:
							{
								return "N0";
							}
						case 1:
							{
								return "N1";
							}
						case 2:
							{
								return "N2";
							}
						case 3:
							{
								return "N3";
							}
						case 4:
							{
								return "N4";
							}
						case 5:
							{
								return "N5";
							}
						case 6:
							{
								return "N6";
							}
					}
					return "";
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override string SampleString
			{
				get
				{
					double num1 = -1234.5678;
					return num1.ToString(this.FormatString, CultureInfo.CurrentCulture);
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Specify the format for numbers. Note that the Currency format type offers specialized formatting for monetary values.";
				}
			}


			// Fields
			private FormatControl owner;
		}

		private class ScientificFormatType : FormatControl.FormatTypeClass
		{
			// Methods
			public ScientificFormatType(FormatControl owner)
			{
				this.owner = owner;
			}

			public override bool Parse(string formatString)
			{
				return FormatControl.ScientificFormatType.ParseStatic(formatString);
			}

			public static bool ParseStatic(string formatString)
			{
				if (((!formatString.Equals("E0") && !formatString.Equals("E1")) && (!formatString.Equals("E2") && !formatString.Equals("E3"))) && (!formatString.Equals("E4") && !formatString.Equals("E5")))
				{
					return formatString.Equals("E6");
				}
				return true;
			}

			public override void PushFormatStringIntoFormatType(string formatString)
			{
				if (formatString.Equals("E0"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(0);
				}
				else if (formatString.Equals("E1"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(1);
				}
				else if (formatString.Equals("E2"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(2);
				}
				else if (formatString.Equals("E3"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(3);
				}
				else if (formatString.Equals("E4"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(4);
				}
				else if (formatString.Equals("E5"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(5);
				}
				else if (formatString.Equals("E6"))
				{
					this.owner.decimalPlacesUpDown.Value = new decimal(6);
				}
			}

			public override string ToString()
			{
				return "Scientific";
			}


			// Properties
			public override bool DropDownVisible
			{
				get
				{
					return true;
				}
			}

			public override bool FormatLabelVisible
			{
				get
				{
					return false;
				}
			}

			public override string FormatString
			{
				get
				{
					switch ((int)this.owner.decimalPlacesUpDown.Value)
					{
						case 0:
							{
								return "E0";
							}
						case 1:
							{
								return "E1";
							}
						case 2:
							{
								return "E2";
							}
						case 3:
							{
								return "E3";
							}
						case 4:
							{
								return "E4";
							}
						case 5:
							{
								return "E5";
							}
						case 6:
							{
								return "E6";
							}
					}
					return "";
				}
			}

			public override bool FormatStringTextBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override bool ListBoxVisible
			{
				get
				{
					return false;
				}
			}

			public override string SampleString
			{
				get
				{
					double num1 = -1234.5678;
					return num1.ToString(this.FormatString, CultureInfo.CurrentCulture);
				}
			}

			public override string TopLabelString
			{
				get
				{
					return "Specify the format for values that use scientific notation.";
				}
			}


			// Fields
			private FormatControl owner;
		}
	}
}
