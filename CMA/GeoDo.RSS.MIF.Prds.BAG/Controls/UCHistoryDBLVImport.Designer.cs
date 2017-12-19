namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCHistoryDBLVImport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCHistoryDBLVImport));
            this.btnExcute = new System.Windows.Forms.Button();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.labBackWater = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExcute
            // 
            this.btnExcute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcute.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExcute.Location = new System.Drawing.Point(283, 31);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(57, 23);
            this.btnExcute.TabIndex = 7;
            this.btnExcute.Text = "替换";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // txtFileDir
            // 
            this.txtFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileDir.Location = new System.Drawing.Point(92, 5);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.ReadOnly = true;
            this.txtFileDir.Size = new System.Drawing.Size(248, 21);
            this.txtFileDir.TabIndex = 6;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(346, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(25, 22);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // labBackWater
            // 
            this.labBackWater.AutoSize = true;
            this.labBackWater.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.labBackWater.Location = new System.Drawing.Point(3, 7);
            this.labBackWater.Name = "labBackWater";
            this.labBackWater.Size = new System.Drawing.Size(92, 17);
            this.labBackWater.TabIndex = 4;
            this.labBackWater.Text = "历史判识文件：";
            this.labBackWater.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UCHistoryDBLVImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnExcute);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.labBackWater);
            this.Name = "UCHistoryDBLVImport";
            this.Size = new System.Drawing.Size(374, 59);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label labBackWater;
    }
}
