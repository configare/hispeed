namespace GeoDo.RSS.MIF.Prds.ICE
{
    partial class UCDBLVImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDBLVImport));
            this.labDBLV = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labDBLV
            // 
            this.labDBLV.AutoSize = true;
            this.labDBLV.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.labDBLV.Location = new System.Drawing.Point(3, 9);
            this.labDBLV.Name = "labDBLV";
            this.labDBLV.Size = new System.Drawing.Size(92, 17);
            this.labDBLV.TabIndex = 2;
            this.labDBLV.Text = "判识结果文件：";
            this.labDBLV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(276, 7);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(25, 22);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.UseVisualStyleBackColor = true;
            // 
            // txtFileDir
            // 
            this.txtFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileDir.Location = new System.Drawing.Point(91, 8);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.ReadOnly = true;
            this.txtFileDir.Size = new System.Drawing.Size(179, 21);
            this.txtFileDir.TabIndex = 5;
            // 
            // UCDBLVImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.labDBLV);
            this.Name = "UCDBLVImport";
            this.Size = new System.Drawing.Size(304, 34);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labDBLV;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtFileDir;
    }
}
