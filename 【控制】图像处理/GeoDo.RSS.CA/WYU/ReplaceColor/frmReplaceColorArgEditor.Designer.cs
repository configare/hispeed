namespace GeoDo.RSS.CA
{
    partial class frmReplaceColorArgEditor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpSelectErea = new System.Windows.Forms.GroupBox();
            this.txtTolerance = new System.Windows.Forms.NumericUpDown();
            this.lblTolerance = new System.Windows.Forms.Label();
            this.trackTolerance = new System.Windows.Forms.TrackBar();
            this.panelTargetColor = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPickColor = new System.Windows.Forms.Button();
            this.grpReplace = new System.Windows.Forms.GroupBox();
            this.panelResultColor = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLightness = new System.Windows.Forms.NumericUpDown();
            this.trackLightness = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSaturation = new System.Windows.Forms.NumericUpDown();
            this.trackSaturation = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHue = new System.Windows.Forms.NumericUpDown();
            this.trackHue = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.grpSelectErea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTolerance)).BeginInit();
            this.grpReplace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHue)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(232, 53);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(232, 24);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(232, 82);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckPreviewing.Location = new System.Drawing.Point(232, 121);
            // 
            // grpSelectErea
            // 
            this.grpSelectErea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSelectErea.Controls.Add(this.txtTolerance);
            this.grpSelectErea.Controls.Add(this.lblTolerance);
            this.grpSelectErea.Controls.Add(this.trackTolerance);
            this.grpSelectErea.Controls.Add(this.panelTargetColor);
            this.grpSelectErea.Controls.Add(this.label1);
            this.grpSelectErea.Controls.Add(this.btnPickColor);
            this.grpSelectErea.Location = new System.Drawing.Point(9, 5);
            this.grpSelectErea.Name = "grpSelectErea";
            this.grpSelectErea.Size = new System.Drawing.Size(208, 146);
            this.grpSelectErea.TabIndex = 20;
            this.grpSelectErea.TabStop = false;
            this.grpSelectErea.Text = "选区";
            // 
            // txtTolerance
            // 
            this.txtTolerance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTolerance.Location = new System.Drawing.Point(143, 71);
            this.txtTolerance.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.txtTolerance.Name = "txtTolerance";
            this.txtTolerance.Size = new System.Drawing.Size(48, 21);
            this.txtTolerance.TabIndex = 5;
            this.txtTolerance.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.txtTolerance.ValueChanged += new System.EventHandler(this.txtTolerance_ValueChanged);
            // 
            // lblTolerance
            // 
            this.lblTolerance.AutoSize = true;
            this.lblTolerance.Location = new System.Drawing.Point(7, 76);
            this.lblTolerance.Name = "lblTolerance";
            this.lblTolerance.Size = new System.Drawing.Size(59, 12);
            this.lblTolerance.TabIndex = 4;
            this.lblTolerance.Text = "颜色容差:";
            // 
            // trackTolerance
            // 
            this.trackTolerance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTolerance.Location = new System.Drawing.Point(7, 94);
            this.trackTolerance.Maximum = 250;
            this.trackTolerance.Name = "trackTolerance";
            this.trackTolerance.Size = new System.Drawing.Size(189, 45);
            this.trackTolerance.TabIndex = 3;
            this.trackTolerance.TickFrequency = 14;
            this.trackTolerance.Value = 40;
            this.trackTolerance.Scroll += new System.EventHandler(this.trackTolerance_SelectedIndexChanged);
            // 
            // panelTargetColor
            // 
            this.panelTargetColor.BackColor = System.Drawing.Color.Red;
            this.panelTargetColor.Location = new System.Drawing.Point(132, 12);
            this.panelTargetColor.Name = "panelTargetColor";
            this.panelTargetColor.Size = new System.Drawing.Size(64, 44);
            this.panelTargetColor.TabIndex = 2;
            this.panelTargetColor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTargetColor_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "颜色:";
            // 
            // btnPickColor
            // 
            this.btnPickColor.Image = global::GeoDo.RSS.CA.Properties.Resources.PaintDotNet_Icons_ColorPickerToolIcon;
            this.btnPickColor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPickColor.Location = new System.Drawing.Point(7, 21);
            this.btnPickColor.Name = "btnPickColor";
            this.btnPickColor.Size = new System.Drawing.Size(78, 23);
            this.btnPickColor.TabIndex = 0;
            this.btnPickColor.Text = "拾取颜色";
            this.btnPickColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPickColor.UseVisualStyleBackColor = true;
            this.btnPickColor.Click += new System.EventHandler(this.btnPickColor_Click);
            // 
            // grpReplace
            // 
            this.grpReplace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.grpReplace.Controls.Add(this.panelResultColor);
            this.grpReplace.Controls.Add(this.label6);
            this.grpReplace.Controls.Add(this.txtLightness);
            this.grpReplace.Controls.Add(this.trackLightness);
            this.grpReplace.Controls.Add(this.label5);
            this.grpReplace.Controls.Add(this.txtSaturation);
            this.grpReplace.Controls.Add(this.trackSaturation);
            this.grpReplace.Controls.Add(this.label4);
            this.grpReplace.Controls.Add(this.txtHue);
            this.grpReplace.Controls.Add(this.trackHue);
            this.grpReplace.Controls.Add(this.label3);
            this.grpReplace.Location = new System.Drawing.Point(9, 159);
            this.grpReplace.Name = "grpReplace";
            this.grpReplace.Size = new System.Drawing.Size(301, 261);
            this.grpReplace.TabIndex = 21;
            this.grpReplace.TabStop = false;
            this.grpReplace.Text = "替换";
            // 
            // panelResultColor
            // 
            this.panelResultColor.BackColor = System.Drawing.Color.Red;
            this.panelResultColor.Location = new System.Drawing.Point(114, 216);
            this.panelResultColor.Name = "panelResultColor";
            this.panelResultColor.Size = new System.Drawing.Size(64, 44);
            this.panelResultColor.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(64, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "结果:";
            // 
            // txtLightness
            // 
            this.txtLightness.Location = new System.Drawing.Point(232, 146);
            this.txtLightness.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtLightness.Name = "txtLightness";
            this.txtLightness.Size = new System.Drawing.Size(45, 21);
            this.txtLightness.TabIndex = 14;
            this.txtLightness.ValueChanged += new System.EventHandler(this.txtBrightness_ValueChanged);
            // 
            // trackLightness
            // 
            this.trackLightness.LargeChange = 1;
            this.trackLightness.Location = new System.Drawing.Point(9, 173);
            this.trackLightness.Maximum = 100;
            this.trackLightness.Minimum = -100;
            this.trackLightness.Name = "trackLightness";
            this.trackLightness.Size = new System.Drawing.Size(279, 45);
            this.trackLightness.TabIndex = 13;
            this.trackLightness.TickFrequency = 10;
            this.trackLightness.Scroll += new System.EventHandler(this.trackBrightness_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "明度:";
            // 
            // txtSaturation
            // 
            this.txtSaturation.Location = new System.Drawing.Point(232, 81);
            this.txtSaturation.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtSaturation.Name = "txtSaturation";
            this.txtSaturation.Size = new System.Drawing.Size(45, 21);
            this.txtSaturation.TabIndex = 11;
            this.txtSaturation.ValueChanged += new System.EventHandler(this.txtSaturation_ValueChanged);
            // 
            // trackSaturation
            // 
            this.trackSaturation.LargeChange = 1;
            this.trackSaturation.Location = new System.Drawing.Point(7, 105);
            this.trackSaturation.Maximum = 100;
            this.trackSaturation.Minimum = -100;
            this.trackSaturation.Name = "trackSaturation";
            this.trackSaturation.Size = new System.Drawing.Size(279, 45);
            this.trackSaturation.TabIndex = 10;
            this.trackSaturation.TickFrequency = 10;
            this.trackSaturation.Scroll += new System.EventHandler(this.trackSaturation_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "饱和度";
            // 
            // txtHue
            // 
            this.txtHue.Location = new System.Drawing.Point(232, 17);
            this.txtHue.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.txtHue.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.txtHue.Name = "txtHue";
            this.txtHue.Size = new System.Drawing.Size(45, 21);
            this.txtHue.TabIndex = 8;
            this.txtHue.ValueChanged += new System.EventHandler(this.txtHue_ValueChanged);
            // 
            // trackHue
            // 
            this.trackHue.LargeChange = 1;
            this.trackHue.Location = new System.Drawing.Point(7, 41);
            this.trackHue.Maximum = 180;
            this.trackHue.Minimum = -180;
            this.trackHue.Name = "trackHue";
            this.trackHue.Size = new System.Drawing.Size(279, 45);
            this.trackHue.TabIndex = 7;
            this.trackHue.TickFrequency = 18;
            this.trackHue.Scroll += new System.EventHandler(this.trackHue_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "色相:";
            // 
            // frmReplaceColorArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 429);
            this.Controls.Add(this.grpReplace);
            this.Controls.Add(this.grpSelectErea);
            this.Name = "frmReplaceColorArgEditor";
            this.Text = "颜色替换";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.grpSelectErea, 0);
            this.Controls.SetChildIndex(this.grpReplace, 0);
            this.grpSelectErea.ResumeLayout(false);
            this.grpSelectErea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTolerance)).EndInit();
            this.grpReplace.ResumeLayout(false);
            this.grpReplace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSelectErea;
        private System.Windows.Forms.NumericUpDown txtTolerance;
        private System.Windows.Forms.Label lblTolerance;
        private System.Windows.Forms.TrackBar trackTolerance;
        private System.Windows.Forms.Panel panelTargetColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPickColor;
        private System.Windows.Forms.GroupBox grpReplace;
        private System.Windows.Forms.Panel panelResultColor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtLightness;
        private System.Windows.Forms.TrackBar trackLightness;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown txtSaturation;
        private System.Windows.Forms.TrackBar trackSaturation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtHue;
        private System.Windows.Forms.TrackBar trackHue;
        private System.Windows.Forms.Label label3;

    }
}