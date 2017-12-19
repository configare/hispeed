namespace GeoDo.RSS.MIF.Prds.HAZ
{
    partial class UCOAIM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCOAIM));
            this.btnNatrueColorDir = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNatrueColorDir = new System.Windows.Forms.TextBox();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.txtNatrueColorFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnNatrueColorDir
            // 
            this.btnNatrueColorDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNatrueColorDir.Image = ((System.Drawing.Image)(resources.GetObject("btnNatrueColorDir.Image")));
            this.btnNatrueColorDir.Location = new System.Drawing.Point(279, 2);
            this.btnNatrueColorDir.Name = "btnNatrueColorDir";
            this.btnNatrueColorDir.Size = new System.Drawing.Size(28, 23);
            this.btnNatrueColorDir.TabIndex = 3;
            this.btnNatrueColorDir.UseVisualStyleBackColor = true;
            this.btnNatrueColorDir.Click += new System.EventHandler(this.btnNatrueColorDir_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "真彩图路径";
            // 
            // txtNatrueColorDir
            // 
            this.txtNatrueColorDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNatrueColorDir.Location = new System.Drawing.Point(74, 3);
            this.txtNatrueColorDir.Name = "txtNatrueColorDir";
            this.txtNatrueColorDir.Size = new System.Drawing.Size(197, 21);
            this.txtNatrueColorDir.TabIndex = 14;
            this.txtNatrueColorDir.TextChanged += new System.EventHandler(this.txtNatrueColorDir_TextChanged);
            // 
            // lstFiles
            // 
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.HorizontalScrollbar = true;
            this.lstFiles.ItemHeight = 12;
            this.lstFiles.Items.AddRange(new object[] {
            "dasda",
            "asda",
            "sdasdasdsd",
            "啥地方色法娃儿娃儿啊温柔啊温柔娃儿娃儿温柔"});
            this.lstFiles.Location = new System.Drawing.Point(74, 58);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(233, 136);
            this.lstFiles.TabIndex = 16;
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_SelectedIndexChanged_1);
            // 
            // txtNatrueColorFile
            // 
            this.txtNatrueColorFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNatrueColorFile.Location = new System.Drawing.Point(74, 31);
            this.txtNatrueColorFile.Name = "txtNatrueColorFile";
            this.txtNatrueColorFile.ReadOnly = true;
            this.txtNatrueColorFile.Size = new System.Drawing.Size(233, 21);
            this.txtNatrueColorFile.TabIndex = 14;
            this.txtNatrueColorFile.TextChanged += new System.EventHandler(this.txtObservationData_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "真彩图文件";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 202);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(302, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // UCOAIM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.txtNatrueColorFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNatrueColorDir);
            this.Controls.Add(this.btnNatrueColorDir);
            this.Controls.Add(this.label6);
            this.Name = "UCOAIM";
            this.Size = new System.Drawing.Size(313, 447);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNatrueColorDir;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNatrueColorDir;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.TextBox txtNatrueColorFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;

    }
}
