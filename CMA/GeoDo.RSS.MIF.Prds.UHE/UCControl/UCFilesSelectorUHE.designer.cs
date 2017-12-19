namespace GeoDo.RSS.MIF.Prds.UHE
{
    partial class UCFilesSelectorUHE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCFilesSelectorUHE));
            this.robCurrent = new System.Windows.Forms.RadioButton();
            this.robFiles = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lsbFileNames = new System.Windows.Forms.ListBox();
            this.btnOpenFiles = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.cmbDirPath = new System.Windows.Forms.ComboBox();
            this.robDirectory = new System.Windows.Forms.RadioButton();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // robCurrent
            // 
            this.robCurrent.AutoSize = true;
            this.robCurrent.Checked = true;
            this.robCurrent.Location = new System.Drawing.Point(3, 24);
            this.robCurrent.Name = "robCurrent";
            this.robCurrent.Size = new System.Drawing.Size(95, 16);
            this.robCurrent.TabIndex = 0;
            this.robCurrent.TabStop = true;
            this.robCurrent.Text = "使用当前影像";
            this.robCurrent.UseVisualStyleBackColor = true;
            // 
            // robFiles
            // 
            this.robFiles.AutoSize = true;
            this.robFiles.Location = new System.Drawing.Point(3, 98);
            this.robFiles.Name = "robFiles";
            this.robFiles.Size = new System.Drawing.Size(125, 16);
            this.robFiles.TabIndex = 1;
            this.robFiles.Text = "选择多个局地文件:";
            this.robFiles.UseVisualStyleBackColor = true;
            this.robFiles.CheckedChanged += new System.EventHandler(this.robFiles_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "请选择待计算文件：";
            // 
            // lsbFileNames
            // 
            this.lsbFileNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lsbFileNames.Enabled = false;
            this.lsbFileNames.FormattingEnabled = true;
            this.lsbFileNames.ItemHeight = 12;
            this.lsbFileNames.Location = new System.Drawing.Point(0, 118);
            this.lsbFileNames.Name = "lsbFileNames";
            this.lsbFileNames.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbFileNames.Size = new System.Drawing.Size(219, 100);
            this.lsbFileNames.TabIndex = 4;
            // 
            // btnOpenFiles
            // 
            this.btnOpenFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenFiles.Enabled = false;
            this.btnOpenFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFiles.Image")));
            this.btnOpenFiles.Location = new System.Drawing.Point(225, 117);
            this.btnOpenFiles.Name = "btnOpenFiles";
            this.btnOpenFiles.Size = new System.Drawing.Size(24, 24);
            this.btnOpenFiles.TabIndex = 9;
            this.btnOpenFiles.UseVisualStyleBackColor = true;
            this.btnOpenFiles.Click += new System.EventHandler(this.btnOpenFiles_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Enabled = false;
            this.btnDelete.Image = global::GeoDo.RSS.MIF.Prds.UHE.Properties.Resources.deleteFile;
            this.btnDelete.Location = new System.Drawing.Point(225, 149);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(24, 24);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnDelete);
            this.panel4.Controls.Add(this.lsbFileNames);
            this.panel4.Controls.Add(this.btnOpenFiles);
            this.panel4.Controls.Add(this.btnOpenDir);
            this.panel4.Controls.Add(this.cmbDirPath);
            this.panel4.Controls.Add(this.btnOK);
            this.panel4.Controls.Add(this.robDirectory);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.robFiles);
            this.panel4.Controls.Add(this.robCurrent);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(252, 253);
            this.panel4.TabIndex = 14;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(164, 224);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(86, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDir.Enabled = false;
            this.btnOpenDir.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenDir.Image")));
            this.btnOpenDir.Location = new System.Drawing.Point(225, 64);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(24, 24);
            this.btnOpenDir.TabIndex = 10;
            this.btnOpenDir.UseVisualStyleBackColor = true;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // cmbDirPath
            // 
            this.cmbDirPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDirPath.Enabled = false;
            this.cmbDirPath.FormattingEnabled = true;
            this.cmbDirPath.Location = new System.Drawing.Point(1, 67);
            this.cmbDirPath.Name = "cmbDirPath";
            this.cmbDirPath.Size = new System.Drawing.Size(219, 20);
            this.cmbDirPath.TabIndex = 6;
            // 
            // robDirectory
            // 
            this.robDirectory.AutoSize = true;
            this.robDirectory.Location = new System.Drawing.Point(3, 45);
            this.robDirectory.Name = "robDirectory";
            this.robDirectory.Size = new System.Drawing.Size(137, 16);
            this.robDirectory.TabIndex = 2;
            this.robDirectory.Text = "选择局地文件夹路径:";
            this.robDirectory.UseVisualStyleBackColor = true;
            this.robDirectory.CheckedChanged += new System.EventHandler(this.robDirectory_CheckedChanged);
            // 
            // UCFilesSelectorLST
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Name = "UCFilesSelectorLST";
            this.Size = new System.Drawing.Size(252, 253);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton robCurrent;
        private System.Windows.Forms.RadioButton robFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lsbFileNames;
        private System.Windows.Forms.Button btnOpenFiles;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.ComboBox cmbDirPath;
        private System.Windows.Forms.RadioButton robDirectory;
    }
}
