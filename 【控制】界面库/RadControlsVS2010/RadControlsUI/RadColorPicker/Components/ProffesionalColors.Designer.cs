namespace Telerik.WinControls.UI.RadColorPicker
{
	partial class ProfessionalColors
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.radioL = new System.Windows.Forms.RadioButton();
            this.radioH = new System.Windows.Forms.RadioButton();
            this.radioB = new System.Windows.Forms.RadioButton();
            this.radioG = new System.Windows.Forms.RadioButton();
            this.radioS = new System.Windows.Forms.RadioButton();
            this.radioR = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.numHue = new Telerik.WinControls.UI.RadSpinEditor();
            this.numSaturation = new Telerik.WinControls.UI.RadSpinEditor();
            this.numLuminance = new Telerik.WinControls.UI.RadSpinEditor();
            this.label1 = new Telerik.WinControls.UI.RadLabel();
            this.numAlpha = new Telerik.WinControls.UI.RadSpinEditor();
            this.numRed = new Telerik.WinControls.UI.RadSpinEditor();
            this.numGreen = new Telerik.WinControls.UI.RadSpinEditor();
            this.numBlue = new Telerik.WinControls.UI.RadSpinEditor();
            this.proColors2DBox1 = new Telerik.WinControls.UI.ProfessionalColors2DBox();
            this.proColorsSlider1 = new Telerik.WinControls.UI.ProfessionalColorsSlider();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLuminance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).BeginInit();
            this.SuspendLayout();
            // 
            // radioL
            // 
            this.radioL.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioL.AutoSize = true;
            this.radioL.Location = new System.Drawing.Point(3, 59);
            this.radioL.MinimumSize = new System.Drawing.Size(36, 0);
            this.radioL.Name = "radioL";
            this.radioL.Size = new System.Drawing.Size(36, 17);
            this.radioL.TabIndex = 39;
            this.radioL.Text = "L:";
            this.radioL.UseVisualStyleBackColor = true;
            this.radioL.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // radioH
            // 
            this.radioH.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioH.AutoSize = true;
            this.radioH.Checked = true;
            this.radioH.Location = new System.Drawing.Point(3, 5);
            this.radioH.Name = "radioH";
            this.radioH.Size = new System.Drawing.Size(36, 17);
            this.radioH.TabIndex = 36;
            this.radioH.TabStop = true;
            this.radioH.Text = "H:";
            this.radioH.UseVisualStyleBackColor = true;
            this.radioH.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // radioB
            // 
            this.radioB.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioB.AutoSize = true;
            this.radioB.Location = new System.Drawing.Point(3, 167);
            this.radioB.MinimumSize = new System.Drawing.Size(36, 0);
            this.radioB.Name = "radioB";
            this.radioB.Size = new System.Drawing.Size(36, 17);
            this.radioB.TabIndex = 41;
            this.radioB.TabStop = true;
            this.radioB.Text = "B:";
            this.radioB.UseVisualStyleBackColor = true;
            this.radioB.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // radioG
            // 
            this.radioG.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioG.AutoSize = true;
            this.radioG.Location = new System.Drawing.Point(3, 140);
            this.radioG.MinimumSize = new System.Drawing.Size(36, 0);
            this.radioG.Name = "radioG";
            this.radioG.Size = new System.Drawing.Size(36, 17);
            this.radioG.TabIndex = 40;
            this.radioG.TabStop = true;
            this.radioG.Text = "G:";
            this.radioG.UseVisualStyleBackColor = true;
            this.radioG.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // radioS
            // 
            this.radioS.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioS.AutoSize = true;
            this.radioS.Location = new System.Drawing.Point(3, 32);
            this.radioS.MinimumSize = new System.Drawing.Size(36, 0);
            this.radioS.Name = "radioS";
            this.radioS.Size = new System.Drawing.Size(36, 17);
            this.radioS.TabIndex = 37;
            this.radioS.Text = "S:";
            this.radioS.UseVisualStyleBackColor = true;
            this.radioS.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // radioR
            // 
            this.radioR.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.radioR.AutoSize = true;
            this.radioR.Location = new System.Drawing.Point(3, 113);
            this.radioR.MinimumSize = new System.Drawing.Size(36, 0);
            this.radioR.Name = "radioR";
            this.radioR.Size = new System.Drawing.Size(36, 17);
            this.radioR.TabIndex = 38;
            this.radioR.TabStop = true;
            this.radioR.Text = "R:";
            this.radioR.UseVisualStyleBackColor = true;
            this.radioR.CheckedChanged += new System.EventHandler(this.colorModeChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.radioH);
            this.flowLayoutPanel1.Controls.Add(this.numHue);
            this.flowLayoutPanel1.Controls.Add(this.radioS);
            this.flowLayoutPanel1.Controls.Add(this.numSaturation);
            this.flowLayoutPanel1.Controls.Add(this.radioL);
            this.flowLayoutPanel1.Controls.Add(this.numLuminance);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.numAlpha);
            this.flowLayoutPanel1.Controls.Add(this.radioR);
            this.flowLayoutPanel1.Controls.Add(this.numRed);
            this.flowLayoutPanel1.Controls.Add(this.radioG);
            this.flowLayoutPanel1.Controls.Add(this.numGreen);
            this.flowLayoutPanel1.Controls.Add(this.radioB);
            this.flowLayoutPanel1.Controls.Add(this.numBlue);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(261, 13);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(106, 197);
            this.flowLayoutPanel1.TabIndex = 52;
            // 
            // numHue
            // 
            this.numHue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numHue.Location = new System.Drawing.Point(45, 3);
            this.numHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numHue.Name = "numHue";
            // 
            // 
            // 
            this.numHue.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numHue.ShowBorder = true;
            this.numHue.Size = new System.Drawing.Size(56, 21);
            this.numHue.TabIndex = 42;
            this.numHue.TabStop = false;
            this.numHue.ThemeName = "Vista";
            this.numHue.ValueChanged += new System.EventHandler(this.numHue_ValueChanged);
            // 
            // numSaturation
            // 
            this.numSaturation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numSaturation.Location = new System.Drawing.Point(45, 30);
            this.numSaturation.Name = "numSaturation";
            // 
            // 
            // 
            this.numSaturation.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numSaturation.ShowBorder = true;
            this.numSaturation.Size = new System.Drawing.Size(56, 21);
            this.numSaturation.TabIndex = 44;
            this.numSaturation.TabStop = false;
            this.numSaturation.ThemeName = "Vista";
            this.numSaturation.ValueChanged += new System.EventHandler(this.numSaturation_ValueChanged);
            // 
            // numLuminance
            // 
            this.numLuminance.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numLuminance.Location = new System.Drawing.Point(45, 57);
            this.numLuminance.Name = "numLuminance";
            // 
            // 
            // 
            this.numLuminance.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numLuminance.ShowBorder = true;
            this.numLuminance.Size = new System.Drawing.Size(56, 21);
            this.numLuminance.TabIndex = 46;
            this.numLuminance.TabStop = false;
            this.numLuminance.ThemeName = "Vista";
            this.numLuminance.ValueChanged += new System.EventHandler(this.numLuminance_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.Location = new System.Drawing.Point(3, 86);
            this.label1.MinimumSize = new System.Drawing.Size(36, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            // 
            // 
            // 
            this.label1.RootElement.MinSize = new System.Drawing.Size(36, 0);
            this.label1.RootElement.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 50;
            this.label1.TabStop = true;
            this.label1.Text = "A:";
            // 
            // numAlpha
            // 
            this.numAlpha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numAlpha.Location = new System.Drawing.Point(45, 84);
            this.numAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numAlpha.Name = "numAlpha";
            // 
            // 
            // 
            this.numAlpha.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numAlpha.ShowBorder = true;
            this.numAlpha.Size = new System.Drawing.Size(56, 21);
            this.numAlpha.TabIndex = 51;
            this.numAlpha.TabStop = false;
            this.numAlpha.ThemeName = "Vista";
            this.numAlpha.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numAlpha.ValueChanged += new System.EventHandler(this.numAlpha_ValueChanged);
            // 
            // numRed
            // 
            this.numRed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numRed.Location = new System.Drawing.Point(45, 111);
            this.numRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numRed.Name = "numRed";
            // 
            // 
            // 
            this.numRed.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numRed.ShowBorder = true;
            this.numRed.Size = new System.Drawing.Size(56, 21);
            this.numRed.TabIndex = 43;
            this.numRed.TabStop = false;
            this.numRed.ThemeName = "Vista";
            this.numRed.ValueChanged += new System.EventHandler(this.numRed_ValueChanged);
            // 
            // numGreen
            // 
            this.numGreen.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numGreen.Location = new System.Drawing.Point(45, 138);
            this.numGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numGreen.Name = "numGreen";
            // 
            // 
            // 
            this.numGreen.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numGreen.ShowBorder = true;
            this.numGreen.Size = new System.Drawing.Size(56, 21);
            this.numGreen.TabIndex = 45;
            this.numGreen.TabStop = false;
            this.numGreen.ThemeName = "Vista";
            this.numGreen.ValueChanged += new System.EventHandler(this.numGreen_ValueChanged);
            // 
            // numBlue
            // 
            this.numBlue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.numBlue.Location = new System.Drawing.Point(45, 165);
            this.numBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlue.Name = "numBlue";
            // 
            // 
            // 
            this.numBlue.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.numBlue.ShowBorder = true;
            this.numBlue.Size = new System.Drawing.Size(56, 21);
            this.numBlue.TabIndex = 47;
            this.numBlue.TabStop = false;
            this.numBlue.ThemeName = "Vista";
            this.numBlue.ValueChanged += new System.EventHandler(this.numBlue_ValueChanged);
            // 
            // proColors2DBox1
            // 
            this.proColors2DBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.proColors2DBox1.ColorHSL = Telerik.WinControls.HslColor.Empty;
            this.proColors2DBox1.ColorMode = Telerik.WinControls.UI.ColorModes.Red;
            this.proColors2DBox1.ColorRGB = System.Drawing.Color.Empty;
            this.proColors2DBox1.Location = new System.Drawing.Point(3, 3);
            this.proColors2DBox1.Name = "proColors2DBox1";
            this.proColors2DBox1.Size = new System.Drawing.Size(216, 216);
            this.proColors2DBox1.TabIndex = 49;
            // 
            // proColorsSlider1
            // 
            this.proColorsSlider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.proColorsSlider1.ColorHSL = Telerik.WinControls.HslColor.FromAhsl(0D, 1D, 1D);
            this.proColorsSlider1.ColorMode = Telerik.WinControls.UI.ColorModes.Red;
            this.proColorsSlider1.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.proColorsSlider1.Location = new System.Drawing.Point(219, 3);
            this.proColorsSlider1.Name = "proColorsSlider1";
            this.proColorsSlider1.Position = 0;
            this.proColorsSlider1.Size = new System.Drawing.Size(41, 216);
            this.proColorsSlider1.TabIndex = 48;
            // 
            // ProfessionalColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.proColors2DBox1);
            this.Controls.Add(this.proColorsSlider1);
            this.Name = "ProfessionalColors";
            this.Size = new System.Drawing.Size(370, 223);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLuminance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Telerik.WinControls.UI.RadSpinEditor numRed;
        private Telerik.WinControls.UI.RadSpinEditor numGreen;
        private Telerik.WinControls.UI.RadSpinEditor numHue;
        private Telerik.WinControls.UI.RadSpinEditor numSaturation;
        private Telerik.WinControls.UI.RadSpinEditor numBlue;
        private Telerik.WinControls.UI.RadSpinEditor numLuminance;
		private System.Windows.Forms.RadioButton radioL;
		private System.Windows.Forms.RadioButton radioH;
		private System.Windows.Forms.RadioButton radioB;
		private System.Windows.Forms.RadioButton radioG;
		private System.Windows.Forms.RadioButton radioS;
		private System.Windows.Forms.RadioButton radioR;
		private ProfessionalColorsSlider proColorsSlider1;
		private ProfessionalColors2DBox proColors2DBox1;
        private Telerik.WinControls.UI.RadSpinEditor numAlpha;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private RadLabel label1;
	}
}
