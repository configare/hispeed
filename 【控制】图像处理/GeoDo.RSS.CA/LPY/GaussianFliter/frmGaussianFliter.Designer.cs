namespace GeoDo.RSS.CA
{
    partial class frmGaussianFliter
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
            this.trackSigma = new System.Windows.Forms.TrackBar();
            this.txtnSigma = new System.Windows.Forms.NumericUpDown();
            this.trackKernel = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtKernelSize = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackKernel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(529, 41);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(529, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(529, 70);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(529, 111);
            // 
            // trackSigma
            // 
            this.trackSigma.Location = new System.Drawing.Point(86, 12);
            this.trackSigma.Maximum = 2500;
            this.trackSigma.Minimum = 5;
            this.trackSigma.Name = "trackSigma";
            this.trackSigma.Size = new System.Drawing.Size(295, 45);
            this.trackSigma.TabIndex = 4;
            this.trackSigma.TickFrequency = 250;
            this.trackSigma.Value = 15;
            this.trackSigma.Scroll += new System.EventHandler(this.trackSigma_Scroll_1);
            // 
            // txtnSigma
            // 
            this.txtnSigma.DecimalPlaces = 1;
            this.txtnSigma.Location = new System.Drawing.Point(400, 15);
            this.txtnSigma.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            65536});
            this.txtnSigma.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.txtnSigma.Name = "txtnSigma";
            this.txtnSigma.Size = new System.Drawing.Size(62, 21);
            this.txtnSigma.TabIndex = 5;
            this.txtnSigma.Value = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            this.txtnSigma.ValueChanged += new System.EventHandler(this.numericSigma_ValueChanged);
            // 
            // trackKernel
            // 
            this.trackKernel.Cursor = System.Windows.Forms.Cursors.Default;
            this.trackKernel.LargeChange = 1;
            this.trackKernel.Location = new System.Drawing.Point(86, 63);
            this.trackKernel.Maximum = 9;
            this.trackKernel.Minimum = 1;
            this.trackKernel.Name = "trackKernel";
            this.trackKernel.Size = new System.Drawing.Size(295, 45);
            this.trackKernel.TabIndex = 6;
            this.trackKernel.Value = 1;
            this.trackKernel.Scroll += new System.EventHandler(this.trackKernel_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "半径大小";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "卷积核";
            // 
            // txtKernelSize
            // 
            this.txtKernelSize.Location = new System.Drawing.Point(400, 61);
            this.txtKernelSize.Name = "txtKernelSize";
            this.txtKernelSize.ReadOnly = true;
            this.txtKernelSize.Size = new System.Drawing.Size(62, 21);
            this.txtKernelSize.TabIndex = 10;
            this.txtKernelSize.Text = "3×3";
            // 
            // frmGaussianFliter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 136);
            this.Controls.Add(this.txtKernelSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackSigma);
            this.Controls.Add(this.trackKernel);
            this.Controls.Add(this.txtnSigma);
            this.Name = "frmGaussianFliter";
            this.Text = "\"高斯滤波\"";
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.txtnSigma, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.trackKernel, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.trackSigma, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.txtKernelSize, 0);
            ((System.ComponentModel.ISupportInitialize)(this.trackSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackKernel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackSigma;
        private System.Windows.Forms.NumericUpDown txtnSigma;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TrackBar trackKernel;
        private System.Windows.Forms.TextBox txtKernelSize;
    }
}

