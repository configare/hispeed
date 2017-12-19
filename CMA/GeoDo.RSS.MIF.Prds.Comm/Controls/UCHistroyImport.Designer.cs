namespace GeoDo.RSS.MIF.Prds.Comm
{
    partial class UCHistroyImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCHistroyImport));
            this.labFireDataSource = new System.Windows.Forms.Label();
            this.txtHistroyFile = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labFireDataSource
            // 
            this.labFireDataSource.AutoSize = true;
            this.labFireDataSource.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFireDataSource.Location = new System.Drawing.Point(4, 4);
            this.labFireDataSource.Name = "labFireDataSource";
            this.labFireDataSource.Size = new System.Drawing.Size(92, 17);
            this.labFireDataSource.TabIndex = 0;
            this.labFireDataSource.Text = "历史数据文件：";
            // 
            // txtHistroyFile
            // 
            this.txtHistroyFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHistroyFile.Location = new System.Drawing.Point(94, 4);
            this.txtHistroyFile.Name = "txtHistroyFile";
            this.txtHistroyFile.ReadOnly = true;
            this.txtHistroyFile.Size = new System.Drawing.Size(184, 21);
            this.txtHistroyFile.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(284, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(25, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // UCHistroyImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtHistroyFile);
            this.Controls.Add(this.labFireDataSource);
            this.Name = "UCHistroyImport";
            this.Size = new System.Drawing.Size(313, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labFireDataSource;
        private System.Windows.Forms.TextBox txtHistroyFile;
        private System.Windows.Forms.Button btnOpen;
    }
}
