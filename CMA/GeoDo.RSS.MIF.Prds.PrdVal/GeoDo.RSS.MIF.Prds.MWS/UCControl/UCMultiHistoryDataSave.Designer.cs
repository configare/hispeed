namespace GeoDo.RSS.MIF.Prds.MWS
{
    partial class UCMultiHistoryDataSave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCMultiHistoryDataSave));
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btnsavefile = new System.Windows.Forms.Button();
            this.labBackWater = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtFileDir
            // 
            this.txtFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtFileDir.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtFileDir.Location = new System.Drawing.Point(120, 21);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.Size = new System.Drawing.Size(144, 21);
            this.txtFileDir.TabIndex = 5;
            this.txtFileDir.TextChanged += new System.EventHandler(this.txtFileDir_TextChanged);
            // 
            // btnsavefile
            // 
            this.btnsavefile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnsavefile.Image = ((System.Drawing.Image)(resources.GetObject("btnsavefile.Image")));
            this.btnsavefile.Location = new System.Drawing.Point(270, 20);
            this.btnsavefile.Name = "btnsavefile";
            this.btnsavefile.Size = new System.Drawing.Size(25, 22);
            this.btnsavefile.TabIndex = 4;
            this.btnsavefile.UseVisualStyleBackColor = true;
            this.btnsavefile.Click += new System.EventHandler(this.btnsavefile_Click_1);
            // 
            // labBackWater
            // 
            this.labBackWater.AutoSize = true;
            this.labBackWater.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.labBackWater.Location = new System.Drawing.Point(7, 21);
            this.labBackWater.Name = "labBackWater";
            this.labBackWater.Size = new System.Drawing.Size(116, 17);
            this.labBackWater.TabIndex = 3;
            this.labBackWater.Text = "生成数据保存路径：";
            this.labBackWater.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labBackWater.Click += new System.EventHandler(this.labBackWater_Click);
            // 
            // UCMultiHistoryDataSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.btnsavefile);
            this.Controls.Add(this.labBackWater);
            this.Name = "UCMultiHistoryDataSave";
            this.Size = new System.Drawing.Size(307, 54);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btnsavefile;
        private System.Windows.Forms.Label labBackWater;
    }
}
