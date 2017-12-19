namespace GeoDo.RSS.MIF.Prds.UHI
{
    partial class UCDensitySetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDensitySetting));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.labDensity = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.btDensity = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btDensity);
            this.panel1.Controls.Add(this.numInterval);
            this.panel1.Controls.Add(this.txtFileDir);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.labDensity);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 60);
            this.panel1.TabIndex = 0;
            // 
            // txtFileDir
            // 
            this.txtFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileDir.Location = new System.Drawing.Point(67, 4);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.ReadOnly = true;
            this.txtFileDir.Size = new System.Drawing.Size(194, 21);
            this.txtFileDir.TabIndex = 2;
            this.txtFileDir.TextChanged += new System.EventHandler(this.txtFileDir_TextChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(265, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(40, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // labDensity
            // 
            this.labDensity.AutoSize = true;
            this.labDensity.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.labDensity.Location = new System.Drawing.Point(3, 5);
            this.labDensity.Name = "labDensity";
            this.labDensity.Size = new System.Drawing.Size(68, 17);
            this.labDensity.TabIndex = 0;
            this.labDensity.Text = "密度文件：";
            this.labDensity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "密度间隔：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numInterval
            // 
            this.numInterval.Location = new System.Drawing.Point(67, 32);
            this.numInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(104, 21);
            this.numInterval.TabIndex = 3;
            this.numInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // btDensity
            // 
            this.btDensity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDensity.Location = new System.Drawing.Point(230, 33);
            this.btDensity.Name = "btDensity";
            this.btDensity.Size = new System.Drawing.Size(75, 23);
            this.btDensity.TabIndex = 4;
            this.btDensity.Text = "密度分割";
            this.btDensity.UseVisualStyleBackColor = true;
            this.btDensity.Click += new System.EventHandler(this.btDensity_Click);
            // 
            // UCDensitySetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "UCDensitySetting";
            this.Size = new System.Drawing.Size(308, 60);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label labDensity;
        private System.Windows.Forms.Button btDensity;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Label label1;
    }
}
