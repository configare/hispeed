namespace GeoDo.RSS.MIF.Prds.HAZ
{
    partial class UCCoverageFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCoverageFile));
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNatrueColorDir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOutDir
            // 
            this.txtOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutDir.Location = new System.Drawing.Point(64, 6);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(252, 21);
            this.txtOutDir.TabIndex = 1;
            this.txtOutDir.TextChanged += new System.EventHandler(this.txtOutDir_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "工作目录";
            // 
            // btnNatrueColorDir
            // 
            this.btnNatrueColorDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNatrueColorDir.Image = ((System.Drawing.Image)(resources.GetObject("btnNatrueColorDir.Image")));
            this.btnNatrueColorDir.Location = new System.Drawing.Point(322, 5);
            this.btnNatrueColorDir.Name = "btnNatrueColorDir";
            this.btnNatrueColorDir.Size = new System.Drawing.Size(28, 23);
            this.btnNatrueColorDir.TabIndex = 4;
            this.btnNatrueColorDir.UseVisualStyleBackColor = true;
            this.btnNatrueColorDir.Click += new System.EventHandler(this.btnNatrueColorDir_Click);
            // 
            // UCCoverageFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNatrueColorDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutDir);
            this.Name = "UCCoverageFile";
            this.Size = new System.Drawing.Size(353, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labFileName;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNatrueColorDir;
    }
}
