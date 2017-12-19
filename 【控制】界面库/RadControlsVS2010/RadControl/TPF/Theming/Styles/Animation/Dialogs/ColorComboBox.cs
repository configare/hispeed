using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Telerik.WinControls
{
	
	internal sealed class ColorComboBox : UserControl
	{
		// Methods
		static ColorComboBox()
		{
            
		}

		public ColorComboBox()
		{
            this.InitializeComponent();
            this.SelectedColor = Color.FromKnownColor(KnownColor.Control);
		}

	    private TextBox tbColor;
        private Button btnSelectColor;
        private Label label1;

        private TypeConverter converter = null;

        private Color ColorFromString(string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                return Color.FromKnownColor(KnownColor.Control);
            }

            if (converter == null)
            {
                converter = TypeDescriptor.GetConverter(typeof(Color));
            }

            return (Color)converter.ConvertFrom(null, AnimationValueCalculatorFactory.SerializationCulture, color);            
        }

        private string ColorToString(Color color)
        {
            if (converter == null)
            {
                converter = TypeDescriptor.GetConverter(typeof(Color));
            }
            return (string)converter.ConvertTo(null, AnimationValueCalculatorFactory.SerializationCulture, color, typeof(string));
        }

		/// <summary>
		/// Gets or sets the selected color
		/// </summary>
		public Color SelectedColor
		{
			get
			{
				return ColorFromString(this.tbColor.Text.Trim());
			}
			set
			{
				this.tbColor.Text = ColorToString(value);
                this.label1.BackColor = value;
			}
		}

        private void InitializeComponent()
        {
			this.colorDialog1 = RadColorEditor.CreateColorDialogInstance();
			colorSelector = RadColorEditor.CreateColorSelectorInstance() as UserControl;
			colorSelector.Dock = DockStyle.Fill;
			((Form)colorDialog1).Controls.Add(colorSelector);
			
			this.tbColor = new System.Windows.Forms.TextBox();
			this.btnSelectColor = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbColor
			// 
			this.tbColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbColor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbColor.Location = new System.Drawing.Point(20, 2);
			this.tbColor.MinimumSize = new System.Drawing.Size(60, 13);
			this.tbColor.Name = "tbColor";
			this.tbColor.Size = new System.Drawing.Size(80, 13);
			this.tbColor.TabIndex = 0;
			this.tbColor.Validating += new System.ComponentModel.CancelEventHandler(this.tbColor_Validating);
			// 
			// btnSelectColor
			// 
			this.btnSelectColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectColor.Location = new System.Drawing.Point(101, -1);
			this.btnSelectColor.Name = "btnSelectColor";
			this.btnSelectColor.Size = new System.Drawing.Size(20, 18);
			this.btnSelectColor.TabIndex = 1;
			this.btnSelectColor.Text = "...";
			this.btnSelectColor.UseVisualStyleBackColor = true;
			this.btnSelectColor.Click += new System.EventHandler(this.btnSelectColor_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(-1, -1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(20, 20);
			this.label1.TabIndex = 2;
			// 
			// ColorComboBox
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSelectColor);
			this.Controls.Add(this.tbColor);
			this.MinimumSize = new System.Drawing.Size(100, 17);
			this.Name = "ColorComboBox";
			this.Size = new System.Drawing.Size(123, 18);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        private IRadColorDialog colorDialog1;
		private UserControl colorSelector;

        private void btnSelectColor_Click(object sender, EventArgs e)
        {			
			this.colorDialog1.SelectedColor = (Color)this.SelectedColor;
			this.colorDialog1.OldColor = (Color)this.SelectedColor;
            if (((Form)this.colorDialog1).ShowDialog() == DialogResult.OK)
				this.SelectedColor = this.colorDialog1.SelectedColor;
        }

        private void tbColor_Validating(object sender, CancelEventArgs e)
        {
			try
			{
				this.SelectedColor = ColorFromString(tbColor.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Color value entered is not valid.", "Telerik Visual Style Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Cancel = true;
			}
        }
	}
}
