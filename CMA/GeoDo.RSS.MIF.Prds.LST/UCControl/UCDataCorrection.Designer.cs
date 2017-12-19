namespace GeoDo.RSS.MIF.Prds.LST
{
    partial class UCDataCorrection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDataCorrection));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOpenObsvt = new System.Windows.Forms.Button();
            this.btnOpenNDVI = new System.Windows.Forms.Button();
            this.btnOpentxt = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.txtCorrectValue = new System.Windows.Forms.TextBox();
            this.txtObservationData = new System.Windows.Forms.TextBox();
            this.txtNDVIFile = new System.Windows.Forms.TextBox();
            this.txtParaFile = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDir = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "观测数据文件";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "旬NDVI文件：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "NDVI分段系数文件：";
            // 
            // btnOpenObsvt
            // 
            this.btnOpenObsvt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenObsvt.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenObsvt.Image")));
            this.btnOpenObsvt.Location = new System.Drawing.Point(279, 30);
            this.btnOpenObsvt.Name = "btnOpenObsvt";
            this.btnOpenObsvt.Size = new System.Drawing.Size(28, 22);
            this.btnOpenObsvt.TabIndex = 3;
            this.btnOpenObsvt.UseVisualStyleBackColor = true;
            this.btnOpenObsvt.Click += new System.EventHandler(this.btnOpenObsvt_Click);
            // 
            // btnOpenNDVI
            // 
            this.btnOpenNDVI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenNDVI.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenNDVI.Image")));
            this.btnOpenNDVI.Location = new System.Drawing.Point(279, 56);
            this.btnOpenNDVI.Name = "btnOpenNDVI";
            this.btnOpenNDVI.Size = new System.Drawing.Size(28, 23);
            this.btnOpenNDVI.TabIndex = 4;
            this.btnOpenNDVI.UseVisualStyleBackColor = true;
            this.btnOpenNDVI.Click += new System.EventHandler(this.btnOpenNDVI_Click);
            // 
            // btnOpentxt
            // 
            this.btnOpentxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpentxt.Image = ((System.Drawing.Image)(resources.GetObject("btnOpentxt.Image")));
            this.btnOpentxt.Location = new System.Drawing.Point(279, 83);
            this.btnOpentxt.Name = "btnOpentxt";
            this.btnOpentxt.Size = new System.Drawing.Size(28, 23);
            this.btnOpentxt.TabIndex = 5;
            this.btnOpentxt.UseVisualStyleBackColor = true;
            this.btnOpentxt.Click += new System.EventHandler(this.btnOpentxt_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "参考点数据输出路径：";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "LST最低修正值：";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.Location = new System.Drawing.Point(131, 111);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(176, 21);
            this.txtOutputDir.TabIndex = 12;
            this.txtOutputDir.Visible = false;
            // 
            // txtCorrectValue
            // 
            this.txtCorrectValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCorrectValue.Location = new System.Drawing.Point(96, 111);
            this.txtCorrectValue.Name = "txtCorrectValue";
            this.txtCorrectValue.Size = new System.Drawing.Size(211, 21);
            this.txtCorrectValue.TabIndex = 13;
            this.txtCorrectValue.TextChanged += new System.EventHandler(this.txtCorrectValue_TextChanged);
            // 
            // txtObservationData
            // 
            this.txtObservationData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservationData.Enabled = false;
            this.txtObservationData.Location = new System.Drawing.Point(96, 31);
            this.txtObservationData.Name = "txtObservationData";
            this.txtObservationData.Size = new System.Drawing.Size(175, 21);
            this.txtObservationData.TabIndex = 14;
            this.txtObservationData.TextChanged += new System.EventHandler(this.txtObservationData_TextChanged);
            // 
            // txtNDVIFile
            // 
            this.txtNDVIFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNDVIFile.Location = new System.Drawing.Point(96, 57);
            this.txtNDVIFile.Name = "txtNDVIFile";
            this.txtNDVIFile.Size = new System.Drawing.Size(175, 21);
            this.txtNDVIFile.TabIndex = 15;
            this.txtNDVIFile.TextChanged += new System.EventHandler(this.txtNDVIFile_TextChanged);
            // 
            // txtParaFile
            // 
            this.txtParaFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParaFile.Location = new System.Drawing.Point(131, 84);
            this.txtParaFile.Name = "txtParaFile";
            this.txtParaFile.Size = new System.Drawing.Size(141, 21);
            this.txtParaFile.TabIndex = 16;
            this.txtParaFile.TextChanged += new System.EventHandler(this.txtParaFile_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(235, 110);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "观测数据路径";
            // 
            // btnDir
            // 
            this.btnDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDir.Image = ((System.Drawing.Image)(resources.GetObject("btnDir.Image")));
            this.btnDir.Location = new System.Drawing.Point(279, 2);
            this.btnDir.Name = "btnDir";
            this.btnDir.Size = new System.Drawing.Size(28, 23);
            this.btnDir.TabIndex = 3;
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Location = new System.Drawing.Point(96, 3);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(175, 21);
            this.txtDir.TabIndex = 14;
            // 
            // UCDataCorrection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtParaFile);
            this.Controls.Add(this.txtNDVIFile);
            this.Controls.Add(this.txtObservationData);
            this.Controls.Add(this.txtCorrectValue);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnOpentxt);
            this.Controls.Add(this.btnOpenNDVI);
            this.Controls.Add(this.btnOpenObsvt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.btnDir);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnOK);
            this.Name = "UCDataCorrection";
            this.Size = new System.Drawing.Size(313, 140);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenObsvt;
        private System.Windows.Forms.Button btnOpenNDVI;
        private System.Windows.Forms.Button btnOpentxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.TextBox txtCorrectValue;
        private System.Windows.Forms.TextBox txtObservationData;
        private System.Windows.Forms.TextBox txtNDVIFile;
        private System.Windows.Forms.TextBox txtParaFile;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.TextBox txtDir;
    }
}
