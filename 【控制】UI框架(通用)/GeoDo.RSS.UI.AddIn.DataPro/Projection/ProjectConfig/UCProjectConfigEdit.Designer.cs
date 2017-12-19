namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class UCProjectConfigEdit
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
            this.labProjectDir = new System.Windows.Forms.Label();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.ckbIsUsed = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labProjectDir
            // 
            this.labProjectDir.AutoSize = true;
            this.labProjectDir.Location = new System.Drawing.Point(8, 44);
            this.labProjectDir.Name = "labProjectDir";
            this.labProjectDir.Size = new System.Drawing.Size(107, 12);
            this.labProjectDir.TabIndex = 0;
            this.labProjectDir.Text = "投影默认输出目录:";
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Enabled = false;
            this.txtDir.Location = new System.Drawing.Point(8, 64);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(491, 21);
            this.txtDir.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Enabled = false;
            this.btnOpen.Font = new System.Drawing.Font("宋体", 3.75F);
            this.btnOpen.Location = new System.Drawing.Point(505, 62);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(28, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // ckbIsUsed
            // 
            this.ckbIsUsed.AutoSize = true;
            this.ckbIsUsed.Location = new System.Drawing.Point(10, 13);
            this.ckbIsUsed.Name = "ckbIsUsed";
            this.ckbIsUsed.Size = new System.Drawing.Size(144, 16);
            this.ckbIsUsed.TabIndex = 3;
            this.ckbIsUsed.Text = "启用投影默认输出目录";
            this.ckbIsUsed.UseVisualStyleBackColor = true;
            this.ckbIsUsed.CheckedChanged += new System.EventHandler(this.ckbIsUsed_CheckedChanged);
            // 
            // UCProjectConfigEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbIsUsed);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.labProjectDir);
            this.Name = "UCProjectConfigEdit";
            this.Size = new System.Drawing.Size(539, 97);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labProjectDir;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.CheckBox ckbIsUsed;
    }
}
