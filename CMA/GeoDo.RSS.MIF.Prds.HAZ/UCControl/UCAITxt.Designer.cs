namespace GeoDo.RSS.MIF.Prds.HAZ
{
    partial class UCAITxt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCAITxt));
            this.label1 = new System.Windows.Forms.Label();
            this.txtAITxtFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAITxtDir = new System.Windows.Forms.Button();
            this.txtAITxtDir = new System.Windows.Forms.TextBox();
            this.btnAITxtFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AI指数文件";
            // 
            // txtAITxtFile
            // 
            this.txtAITxtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAITxtFile.Location = new System.Drawing.Point(74, 31);
            this.txtAITxtFile.Name = "txtAITxtFile";
            this.txtAITxtFile.ReadOnly = true;
            this.txtAITxtFile.Size = new System.Drawing.Size(197, 21);
            this.txtAITxtFile.TabIndex = 14;
            this.txtAITxtFile.TextChanged += new System.EventHandler(this.txtObservationData_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "AI指数路径";
            // 
            // btnAITxtDir
            // 
            this.btnAITxtDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAITxtDir.Image = ((System.Drawing.Image)(resources.GetObject("btnAITxtDir.Image")));
            this.btnAITxtDir.Location = new System.Drawing.Point(279, 2);
            this.btnAITxtDir.Name = "btnAITxtDir";
            this.btnAITxtDir.Size = new System.Drawing.Size(28, 23);
            this.btnAITxtDir.TabIndex = 3;
            this.btnAITxtDir.UseVisualStyleBackColor = true;
            this.btnAITxtDir.Click += new System.EventHandler(this.btnNatrueColorDir_Click);
            // 
            // txtAITxtDir
            // 
            this.txtAITxtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAITxtDir.Location = new System.Drawing.Point(74, 3);
            this.txtAITxtDir.Name = "txtAITxtDir";
            this.txtAITxtDir.Size = new System.Drawing.Size(197, 21);
            this.txtAITxtDir.TabIndex = 14;
            this.txtAITxtDir.TextChanged += new System.EventHandler(this.txtAITxtDir_TextChanged);
            // 
            // btnAITxtFile
            // 
            this.btnAITxtFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAITxtFile.Image = ((System.Drawing.Image)(resources.GetObject("btnAITxtFile.Image")));
            this.btnAITxtFile.Location = new System.Drawing.Point(277, 29);
            this.btnAITxtFile.Name = "btnAITxtFile";
            this.btnAITxtFile.Size = new System.Drawing.Size(28, 23);
            this.btnAITxtFile.TabIndex = 15;
            this.btnAITxtFile.UseVisualStyleBackColor = true;
            this.btnAITxtFile.Click += new System.EventHandler(this.btnAITxtFile_Click);
            // 
            // UCAITxt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAITxtFile);
            this.Controls.Add(this.txtAITxtFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAITxtDir);
            this.Controls.Add(this.btnAITxtDir);
            this.Controls.Add(this.label6);
            this.Name = "UCAITxt";
            this.Size = new System.Drawing.Size(313, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAITxtFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAITxtDir;
        private System.Windows.Forms.TextBox txtAITxtDir;
        private System.Windows.Forms.Button btnAITxtFile;
    }
}
