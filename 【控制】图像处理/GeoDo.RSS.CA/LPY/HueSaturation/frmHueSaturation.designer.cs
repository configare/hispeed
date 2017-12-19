namespace GeoDo.RSS.CA
{
    partial class frmHueSaturation
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSaturation = new System.Windows.Forms.Label();
            this.lableHue = new System.Windows.Forms.Label();
            this.numericLum = new System.Windows.Forms.NumericUpDown();
            this.numericSaturation = new System.Windows.Forms.NumericUpDown();
            this.labelLum = new System.Windows.Forms.Label();
            this.numericHue = new System.Windows.Forms.NumericUpDown();
            this.trackBarSaturation = new System.Windows.Forms.TrackBar();
            this.trackBarHue = new System.Windows.Forms.TrackBar();
            this.trackBarLum = new System.Windows.Forms.TrackBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLum)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(475, 41);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(475, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(475, 70);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(475, 109);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSaturation);
            this.groupBox1.Controls.Add(this.lableHue);
            this.groupBox1.Controls.Add(this.numericLum);
            this.groupBox1.Controls.Add(this.numericSaturation);
            this.groupBox1.Controls.Add(this.labelLum);
            this.groupBox1.Controls.Add(this.numericHue);
            this.groupBox1.Controls.Add(this.trackBarSaturation);
            this.groupBox1.Controls.Add(this.trackBarHue);
            this.groupBox1.Controls.Add(this.trackBarLum);
            this.groupBox1.Location = new System.Drawing.Point(12, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 182);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数";
            // 
            // labelSaturation
            // 
            this.labelSaturation.AutoSize = true;
            this.labelSaturation.Location = new System.Drawing.Point(6, 82);
            this.labelSaturation.Name = "labelSaturation";
            this.labelSaturation.Size = new System.Drawing.Size(41, 12);
            this.labelSaturation.TabIndex = 10;
            this.labelSaturation.Text = "饱和度";
            // 
            // lableHue
            // 
            this.lableHue.AutoSize = true;
            this.lableHue.Location = new System.Drawing.Point(10, 34);
            this.lableHue.Name = "lableHue";
            this.lableHue.Size = new System.Drawing.Size(29, 12);
            this.lableHue.TabIndex = 9;
            this.lableHue.Text = "色相";
            // 
            // numericLum
            // 
            this.numericLum.Location = new System.Drawing.Point(390, 133);
            this.numericLum.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericLum.Name = "numericLum";
            this.numericLum.Size = new System.Drawing.Size(42, 21);
            this.numericLum.TabIndex = 8;
            this.numericLum.ValueChanged += new System.EventHandler(this.numericLum_ValueChanged);
            // 
            // numericSaturation
            // 
            this.numericSaturation.Location = new System.Drawing.Point(390, 82);
            this.numericSaturation.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericSaturation.Name = "numericSaturation";
            this.numericSaturation.Size = new System.Drawing.Size(43, 21);
            this.numericSaturation.TabIndex = 7;
            this.numericSaturation.ValueChanged += new System.EventHandler(this.numericSaturation_ValueChanged);
            // 
            // labelLum
            // 
            this.labelLum.AutoSize = true;
            this.labelLum.Location = new System.Drawing.Point(9, 133);
            this.labelLum.Name = "labelLum";
            this.labelLum.Size = new System.Drawing.Size(29, 12);
            this.labelLum.TabIndex = 5;
            this.labelLum.Text = "亮度";
            // 
            // numericHue
            // 
            this.numericHue.Location = new System.Drawing.Point(391, 29);
            this.numericHue.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.numericHue.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.numericHue.Name = "numericHue";
            this.numericHue.Size = new System.Drawing.Size(43, 21);
            this.numericHue.TabIndex = 6;
            this.numericHue.ValueChanged += new System.EventHandler(this.numericHue_ValueChanged);
            // 
            // trackBarSaturation
            // 
            this.trackBarSaturation.Location = new System.Drawing.Point(61, 80);
            this.trackBarSaturation.Maximum = 100;
            this.trackBarSaturation.Minimum = -100;
            this.trackBarSaturation.Name = "trackBarSaturation";
            this.trackBarSaturation.Size = new System.Drawing.Size(319, 45);
            this.trackBarSaturation.TabIndex = 1;
            this.trackBarSaturation.TickFrequency = 10;
            this.trackBarSaturation.Scroll += new System.EventHandler(this.trackBarSaturation_Scroll);
            // 
            // trackBarHue
            // 
            this.trackBarHue.Location = new System.Drawing.Point(61, 29);
            this.trackBarHue.Maximum = 180;
            this.trackBarHue.Minimum = -180;
            this.trackBarHue.Name = "trackBarHue";
            this.trackBarHue.Size = new System.Drawing.Size(319, 45);
            this.trackBarHue.TabIndex = 0;
            this.trackBarHue.TickFrequency = 18;
            this.trackBarHue.Scroll += new System.EventHandler(this.trackBarHue_Scroll);
            // 
            // trackBarLum
            // 
            this.trackBarLum.Location = new System.Drawing.Point(61, 133);
            this.trackBarLum.Maximum = 100;
            this.trackBarLum.Minimum = -100;
            this.trackBarLum.Name = "trackBarLum";
            this.trackBarLum.Size = new System.Drawing.Size(319, 45);
            this.trackBarLum.TabIndex = 5;
            this.trackBarLum.TickFrequency = 10;
            this.trackBarLum.Scroll += new System.EventHandler(this.trackBarLum_Scroll);
            // 
            // frmHueSaturation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 202);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmHueSaturation";
            this.Text = "色相、饱和度";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericLum;
        private System.Windows.Forms.NumericUpDown numericSaturation;
        private System.Windows.Forms.NumericUpDown numericHue;
        private System.Windows.Forms.TrackBar trackBarSaturation;
        private System.Windows.Forms.TrackBar trackBarHue;
        private System.Windows.Forms.TrackBar trackBarLum;
        private System.Windows.Forms.Label lableHue;
        private System.Windows.Forms.Label labelSaturation;
        private System.Windows.Forms.Label labelLum;
    }
}

