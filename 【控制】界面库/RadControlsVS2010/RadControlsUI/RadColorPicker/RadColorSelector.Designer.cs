using System.Drawing;

namespace Telerik.WinControls.UI
{
	partial class RadColorSelector
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                ColorDialogLocalizationProvider.CurrentProviderChanged -= ColorDialogLocalizationProvider_CurrentProviderChanged;

                if(components != null)
                {
				components.Dispose();
                }
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.newLabel = new System.Windows.Forms.Label();
            this.labelOldColor = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelColor = new Telerik.WinControls.UI.RadColorPicker.TransparentColorBox();
            this.hexHeadingLabel = new System.Windows.Forms.Label();
            this.currentLabel = new System.Windows.Forms.Label();
            this.customColors = new Telerik.WinControls.UI.RadColorPicker.CustomColors();
            this.textBoxColor = new Telerik.WinControls.UI.RadTextBox();
            this.btnAddNewColor = new Telerik.WinControls.UI.RadButton();
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.radPageViewPage1 = new Telerik.WinControls.UI.RadPageViewPage();
            this.discreteColorHexagon = new Telerik.WinControls.UI.RadColorPicker.DiscreteColorHexagon();
            this.radPageViewPage2 = new Telerik.WinControls.UI.RadPageViewPage();
            this.listBox1 = new Telerik.WinControls.UI.RadColorPicker.ColorListBox();
            this.radPageViewPage3 = new Telerik.WinControls.UI.RadPageViewPage();
            this.listBox2 = new Telerik.WinControls.UI.RadColorPicker.ColorListBox();
            this.radPageViewPage4 = new Telerik.WinControls.UI.RadPageViewPage();
            this.professionalColorsControl = new Telerik.WinControls.UI.RadColorPicker.ProfessionalColors();
            this.radButton3 = new Telerik.WinControls.UI.RadButton();
            this.btnScreenColorPick = new Telerik.WinControls.UI.RadButton();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.labelColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddNewColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageViewPage1.SuspendLayout();
            this.radPageViewPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listBox1)).BeginInit();
            this.radPageViewPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listBox2)).BeginInit();
            this.radPageViewPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnScreenColorPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            this.SuspendLayout();
            // 
            // newLabel
            // 
            this.newLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newLabel.AutoSize = true;
            this.newLabel.Location = new System.Drawing.Point(441, 38);
            this.newLabel.Name = "newLabel";
            this.newLabel.Size = new System.Drawing.Size(29, 13);
            this.newLabel.TabIndex = 4;
            this.newLabel.Text = "New";
            // 
            // labelOldColor
            // 
            this.labelOldColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelOldColor.BackColor = System.Drawing.Color.LightGray;
            this.labelOldColor.Location = new System.Drawing.Point(-6, 25);
            this.labelOldColor.Name = "labelOldColor";
            this.labelOldColor.Size = new System.Drawing.Size(67, 25);
            this.labelOldColor.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.labelOldColor);
            this.panel1.Controls.Add(this.labelColor);
            this.panel1.Location = new System.Drawing.Point(428, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(57, 50);
            this.panel1.TabIndex = 7;
            // 
            // labelColor
            // 
            this.labelColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelColor.Location = new System.Drawing.Point(-6, 0);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(67, 25);
            this.labelColor.TabIndex = 0;
            // 
            // hexHeadingLabel
            // 
            this.hexHeadingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hexHeadingLabel.AutoSize = true;
            this.hexHeadingLabel.Location = new System.Drawing.Point(416, 135);
            this.hexHeadingLabel.Name = "hexHeadingLabel";
            this.hexHeadingLabel.Size = new System.Drawing.Size(14, 13);
            this.hexHeadingLabel.TabIndex = 19;
            this.hexHeadingLabel.Text = "#";
            // 
            // currentLabel
            // 
            this.currentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentLabel.AutoSize = true;
            this.currentLabel.Location = new System.Drawing.Point(438, 108);
            this.currentLabel.Name = "currentLabel";
            this.currentLabel.Size = new System.Drawing.Size(41, 13);
            this.currentLabel.TabIndex = 27;
            this.currentLabel.Text = "Current";
            // 
            // customColors
            // 
            this.customColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.customColors.BackColor = System.Drawing.Color.Transparent;
            this.customColors.Location = new System.Drawing.Point(8, 315);
            this.customColors.Name = "customColors";
            this.customColors.Padding = new System.Windows.Forms.Padding(5);
            this.customColors.SelectedColorIndex = -1;
            this.customColors.Size = new System.Drawing.Size(290, 37);
            this.customColors.TabIndex = 29;
            // 
            // textBoxColor
            // 
            this.textBoxColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxColor.Location = new System.Drawing.Point(428, 131);
            this.textBoxColor.Name = "textBoxColor";
            this.textBoxColor.Padding = new System.Windows.Forms.Padding(2, 2, 2, 3);
            // 
            // 
            // 
            this.textBoxColor.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.textBoxColor.RootElement.Padding = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.textBoxColor.Size = new System.Drawing.Size(57, 20);
            this.textBoxColor.TabIndex = 28;
            this.textBoxColor.TabStop = false;
            this.textBoxColor.TextChanged += new System.EventHandler(this.textBoxColor_TextChanged);
            // 
            // btnAddNewColor
            // 
            this.btnAddNewColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddNewColor.Location = new System.Drawing.Point(304, 322);
            this.btnAddNewColor.Name = "btnAddNewColor";
            this.btnAddNewColor.Size = new System.Drawing.Size(111, 23);
            this.btnAddNewColor.TabIndex = 26;
            this.btnAddNewColor.Text = "Add Custom Color";
            this.btnAddNewColor.Click += new System.EventHandler(this.radButton4_Click);
            // 
            // radPageView1
            // 
            this.radPageView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radPageView1.Controls.Add(this.radPageViewPage1);
            this.radPageView1.Controls.Add(this.radPageViewPage2);
            this.radPageView1.Controls.Add(this.radPageViewPage3);
            this.radPageView1.Controls.Add(this.radPageViewPage4);
            this.radPageView1.Location = new System.Drawing.Point(8, 8);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.radPageViewPage4;
            this.radPageView1.Size = new System.Drawing.Size(407, 301);
            this.radPageView1.TabIndex = 24;
            this.radPageView1.Text = "radPageView1";
            // 
            // radPageViewPage1
            // 
            this.radPageViewPage1.Controls.Add(this.discreteColorHexagon);
            this.radPageViewPage1.Location = new System.Drawing.Point(10, 37);
            this.radPageViewPage1.Name = "radPageViewPage1";
            this.radPageViewPage1.Size = new System.Drawing.Size(386, 253);
            this.radPageViewPage1.Text = "Basic";
            // 
            // discreteColorHexagon
            // 
            this.discreteColorHexagon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.discreteColorHexagon.Location = new System.Drawing.Point(0, 0);
            this.discreteColorHexagon.Name = "discreteColorHexagon";
            this.discreteColorHexagon.Size = new System.Drawing.Size(386, 253);
            this.discreteColorHexagon.TabIndex = 0;
            // 
            // radPageViewPage2
            // 
            this.radPageViewPage2.Controls.Add(this.listBox1);
            this.radPageViewPage2.Location = new System.Drawing.Point(10, 37);
            this.radPageViewPage2.Name = "radPageViewPage2";
            this.radPageViewPage2.Size = new System.Drawing.Size(405, 271);
            this.radPageViewPage2.Text = "System";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(405, 271);
            this.listBox1.TabIndex = 0;
            // 
            // radPageViewPage3
            // 
            this.radPageViewPage3.Controls.Add(this.listBox2);
            this.radPageViewPage3.Location = new System.Drawing.Point(10, 37);
            this.radPageViewPage3.Name = "radPageViewPage3";
            this.radPageViewPage3.Text = "Web";
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(0, 0);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(405, 271);
            this.listBox2.TabIndex = 1;
            // 
            // radPageViewPage4
            // 
            this.radPageViewPage4.Controls.Add(this.professionalColorsControl);
            this.radPageViewPage4.Location = new System.Drawing.Point(5, 27);
            this.radPageViewPage4.Name = "radPageViewPage4";
            this.radPageViewPage4.Size = new System.Drawing.Size(397, 269);
            this.radPageViewPage4.Text = "Professional";
            // 
            // professionalColorsControl
            // 
            this.professionalColorsControl.ColorRgb = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.professionalColorsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.professionalColorsControl.Location = new System.Drawing.Point(0, 0);
            this.professionalColorsControl.Name = "professionalColorsControl";
            this.professionalColorsControl.Size = new System.Drawing.Size(397, 269);
            this.professionalColorsControl.TabIndex = 0;
            // 
            // radButton3
            // 
            this.radButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radButton3.Location = new System.Drawing.Point(422, 360);
            this.radButton3.Name = "radButton3";
            this.radButton3.Size = new System.Drawing.Size(75, 23);
            this.radButton3.TabIndex = 23;
            this.radButton3.Text = "Cancel";
            this.radButton3.Click += new System.EventHandler(this.radButton3_Click);
            // 
            // btnScreenColorPick
            // 
            this.btnScreenColorPick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScreenColorPick.BackColor = System.Drawing.Color.Transparent;
            this.btnScreenColorPick.DisplayStyle = Telerik.WinControls.DisplayStyle.Image;
            this.btnScreenColorPick.Image = global::Telerik.WinControls.UI.Properties.Resources.eyedropper;
            this.btnScreenColorPick.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnScreenColorPick.Location = new System.Drawing.Point(437, 159);
            this.btnScreenColorPick.Name = "btnScreenColorPick";
            this.btnScreenColorPick.Size = new System.Drawing.Size(38, 32);
            this.btnScreenColorPick.TabIndex = 22;
            this.btnScreenColorPick.Click += new System.EventHandler(this.buttonColorPick_Click);
            // 
            // radButton1
            // 
            this.radButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radButton1.Location = new System.Drawing.Point(340, 360);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(75, 23);
            this.radButton1.TabIndex = 21;
            this.radButton1.Text = "OK";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // RadColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.customColors);
            this.Controls.Add(this.textBoxColor);
            this.Controls.Add(this.currentLabel);
            this.Controls.Add(this.btnAddNewColor);
            this.Controls.Add(this.radPageView1);
            this.Controls.Add(this.radButton3);
            this.Controls.Add(this.btnScreenColorPick);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.hexHeadingLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.newLabel);
            this.MinimumSize = new System.Drawing.Size(508, 395);
            this.Name = "RadColorSelector";
            this.Size = new System.Drawing.Size(508, 395);
            this.Load += new System.EventHandler(this.RadColorDialog_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.labelColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddNewColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageViewPage1.ResumeLayout(false);
            this.radPageViewPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listBox1)).EndInit();
            this.radPageViewPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listBox2)).EndInit();
            this.radPageViewPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnScreenColorPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private RadColorPicker.DiscreteColorHexagon discreteColorHexagon;
		private RadColorPicker.TransparentColorBox labelColor;
		private System.Windows.Forms.Label newLabel;
		private System.Windows.Forms.Label labelOldColor;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label hexHeadingLabel;
		private RadColorPicker.ProfessionalColors professionalColorsControl;
		private Telerik.WinControls.UI.RadButton radButton1;
		private Telerik.WinControls.UI.RadButton btnScreenColorPick;
		private Telerik.WinControls.UI.RadButton radButton3;
		private Telerik.WinControls.UI.RadPageView radPageView1;
		private Telerik.WinControls.UI.RadPageViewPage radPageViewPage1;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage2;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage3;
        private Telerik.WinControls.UI.RadPageViewPage radPageViewPage4;
		private RadColorPicker.ColorListBox listBox2;
		private RadColorPicker.ColorListBox listBox1;
		private Telerik.WinControls.UI.RadButton btnAddNewColor;
		private System.Windows.Forms.Label currentLabel;
		private RadTextBox textBoxColor;
		private RadColorPicker.CustomColors customColors;
	}
}
