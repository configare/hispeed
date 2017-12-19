namespace GeoDo.RSS.UI.AddIn.CMA.UIProvider.FIR
{
    partial class UCGbalFireCorrect
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
            this.cvPanel = new System.Windows.Forms.Panel();
            this.txtDataSet = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPath = new System.Windows.Forms.Button();
            this.lstFileList = new System.Windows.Forms.ListView();
            this.argPanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.fileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.argPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cvPanel
            // 
            this.cvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cvPanel.Location = new System.Drawing.Point(200, 0);
            this.cvPanel.Name = "cvPanel";
            this.cvPanel.Size = new System.Drawing.Size(524, 588);
            this.cvPanel.TabIndex = 0;
            // 
            // txtDataSet
            // 
            this.txtDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataSet.Location = new System.Drawing.Point(49, 62);
            this.txtDataSet.Name = "txtDataSet";
            this.txtDataSet.Size = new System.Drawing.Size(142, 21);
            this.txtDataSet.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "数据集";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(49, 25);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(106, 21);
            this.txtPath.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "目录";
            // 
            // btnPath
            // 
            this.btnPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPath.Image = global::GeoDo.RSS.UI.AddIn.CMA.Properties.Resources.cmdOpen;
            this.btnPath.Location = new System.Drawing.Point(158, 23);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(33, 23);
            this.btnPath.TabIndex = 4;
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // lstFileList
            // 
            this.lstFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileName});
            this.lstFileList.Location = new System.Drawing.Point(9, 103);
            this.lstFileList.Name = "lstFileList";
            this.lstFileList.Size = new System.Drawing.Size(182, 433);
            this.lstFileList.TabIndex = 5;
            this.lstFileList.UseCompatibleStateImageBehavior = false;
            this.lstFileList.View = System.Windows.Forms.View.Details;
            // 
            // argPanel
            // 
            this.argPanel.Controls.Add(this.txtDataSet);
            this.argPanel.Controls.Add(this.txtPath);
            this.argPanel.Controls.Add(this.label1);
            this.argPanel.Controls.Add(this.label2);
            this.argPanel.Controls.Add(this.btOK);
            this.argPanel.Controls.Add(this.btCancel);
            this.argPanel.Controls.Add(this.btnPath);
            this.argPanel.Controls.Add(this.lstFileList);
            this.argPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.argPanel.Location = new System.Drawing.Point(0, 0);
            this.argPanel.Name = "argPanel";
            this.argPanel.Size = new System.Drawing.Size(200, 588);
            this.argPanel.TabIndex = 6;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(200, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 588);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCancel.Location = new System.Drawing.Point(9, 553);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(83, 23);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(108, 553);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(83, 23);
            this.btOK.TabIndex = 4;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // fileName
            // 
            this.fileName.Text = "文件名";
            this.fileName.Width = 136;
            // 
            // UCGbalFireCorrect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.cvPanel);
            this.Controls.Add(this.argPanel);
            this.Name = "UCGbalFireCorrect";
            this.Size = new System.Drawing.Size(724, 588);
            this.argPanel.ResumeLayout(false);
            this.argPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cvPanel;
        private System.Windows.Forms.TextBox txtDataSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.ListView lstFileList;
        private System.Windows.Forms.Panel argPanel;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ColumnHeader fileName;
    }
}
