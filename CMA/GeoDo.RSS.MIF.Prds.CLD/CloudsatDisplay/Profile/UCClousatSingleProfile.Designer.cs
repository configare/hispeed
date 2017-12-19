namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class UCClousatSingleProfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCClousatSingleProfile));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnBrowseFile = new System.Windows.Forms.ToolStripButton();
            this.btnBrowseDir = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.grpDataSet = new System.Windows.Forms.GroupBox();
            this.tvdataset = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            this.grpDataSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBrowseFile,
            this.btnBrowseDir,
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(315, 27);
            this.toolStrip1.TabIndex = 61;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseFile.Image")));
            this.btnBrowseFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(57, 24);
            this.btnBrowseFile.Text = "打开";
            // 
            // btnBrowseDir
            // 
            this.btnBrowseDir.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseDir.Image")));
            this.btnBrowseDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBrowseDir.Name = "btnBrowseDir";
            this.btnBrowseDir.Size = new System.Drawing.Size(85, 24);
            this.btnBrowseDir.Text = "添加路径";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 27);
            this.toolStripComboBox1.ToolTipText = "数据集";
            // 
            // grpDataSet
            // 
            this.grpDataSet.Controls.Add(this.tvdataset);
            this.grpDataSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDataSet.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpDataSet.Location = new System.Drawing.Point(0, 27);
            this.grpDataSet.Name = "grpDataSet";
            this.grpDataSet.Size = new System.Drawing.Size(315, 479);
            this.grpDataSet.TabIndex = 62;
            this.grpDataSet.TabStop = false;
            this.grpDataSet.Text = "文件";
            // 
            // tvdataset
            // 
            this.tvdataset.CheckBoxes = true;
            this.tvdataset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvdataset.Location = new System.Drawing.Point(3, 19);
            this.tvdataset.Name = "tvdataset";
            this.tvdataset.Size = new System.Drawing.Size(309, 457);
            this.tvdataset.TabIndex = 3;
            // 
            // UCClousatSingleProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpDataSet);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCClousatSingleProfile";
            this.Size = new System.Drawing.Size(315, 506);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpDataSet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnBrowseDir;
        private System.Windows.Forms.ToolStripButton btnBrowseFile;
        private System.Windows.Forms.GroupBox grpDataSet;
        private System.Windows.Forms.TreeView tvdataset;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    }
}
