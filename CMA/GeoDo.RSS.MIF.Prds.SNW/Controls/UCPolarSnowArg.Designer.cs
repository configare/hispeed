namespace GeoDo.RSS.MIF.Prds.SNW
{
    partial class UCPolarSnowArg
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCPolarSnowArg));
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbRange = new System.Windows.Forms.ComboBox();
            this.labRange = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.labFile = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBands = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(344, 85);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbRange
            // 
            this.cmbRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRange.FormattingEnabled = true;
            this.cmbRange.Items.AddRange(new object[] {
            "北极",
            "南极"});
            this.cmbRange.Location = new System.Drawing.Point(127, 32);
            this.cmbRange.Name = "cmbRange";
            this.cmbRange.Size = new System.Drawing.Size(292, 20);
            this.cmbRange.TabIndex = 13;
            // 
            // labRange
            // 
            this.labRange.AutoSize = true;
            this.labRange.Location = new System.Drawing.Point(3, 36);
            this.labRange.Name = "labRange";
            this.labRange.Size = new System.Drawing.Size(59, 12);
            this.labRange.TabIndex = 11;
            this.labRange.Text = "区域选择:";
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(127, 2);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(292, 21);
            this.txtFileName.TabIndex = 9;
            // 
            // labFile
            // 
            this.labFile.AutoSize = true;
            this.labFile.Location = new System.Drawing.Point(3, 6);
            this.labFile.Name = "labFile";
            this.labFile.Size = new System.Drawing.Size(119, 12);
            this.labFile.TabIndex = 8;
            this.labFile.Text = "雪深雪水当量日产品:";
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(425, 1);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(26, 23);
            this.btnOpen.TabIndex = 10;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "产品及波段选择:";
            // 
            // cmbBands
            // 
            this.cmbBands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbBands.FormattingEnabled = true;
            this.cmbBands.Items.AddRange(new object[] {
            "积雪深度_升轨",
            "积雪深度_降轨",
            "雪水当量_升轨",
            "雪水当量_降轨"});
            this.cmbBands.Location = new System.Drawing.Point(127, 61);
            this.cmbBands.Name = "cmbBands";
            this.cmbBands.Size = new System.Drawing.Size(292, 20);
            this.cmbBands.TabIndex = 17;
            // 
            // UCPolarSnowArg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbBands);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbRange);
            this.Controls.Add(this.labRange);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.labFile);
            this.Name = "UCPolarSnowArg";
            this.Size = new System.Drawing.Size(454, 115);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbRange;
        private System.Windows.Forms.Label labRange;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label labFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBands;
    }
}
