namespace GeoDo.RSS.RasterTools
{
    partial class frmScatterTwoVarSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvXBands = new System.Windows.Forms.TreeView();
            this.btnSelectXFile = new System.Windows.Forms.Button();
            this.txtXFileName = new System.Windows.Forms.TextBox();
            this.ckNeedFit = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdFullImage = new System.Windows.Forms.RadioButton();
            this.rdAOI = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvYBands = new System.Windows.Forms.TreeView();
            this.btnSelectYFile = new System.Windows.Forms.Button();
            this.txtYFileName = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(607, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(512, 350);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.btnSelectXFile);
            this.groupBox3.Controls.Add(this.txtXFileName);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(5, 3);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(350, 290);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "X 轴文件";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvXBands);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(3, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 228);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X轴波段 - [?]";
            // 
            // tvXBands
            // 
            this.tvXBands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvXBands.FullRowSelect = true;
            this.tvXBands.HideSelection = false;
            this.tvXBands.Location = new System.Drawing.Point(16, 23);
            this.tvXBands.Name = "tvXBands";
            this.tvXBands.Size = new System.Drawing.Size(310, 187);
            this.tvXBands.TabIndex = 0;
            // 
            // btnSelectXFile
            // 
            this.btnSelectXFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectXFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectXFile.Location = new System.Drawing.Point(318, 24);
            this.btnSelectXFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectXFile.Name = "btnSelectXFile";
            this.btnSelectXFile.Size = new System.Drawing.Size(29, 24);
            this.btnSelectXFile.TabIndex = 8;
            this.btnSelectXFile.Text = "...";
            this.btnSelectXFile.UseVisualStyleBackColor = true;
            this.btnSelectXFile.Click += new System.EventHandler(this.btnSelectXFile_Click);
            // 
            // txtXFileName
            // 
            this.txtXFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXFileName.Enabled = false;
            this.txtXFileName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtXFileName.Location = new System.Drawing.Point(12, 25);
            this.txtXFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtXFileName.Name = "txtXFileName";
            this.txtXFileName.ReadOnly = true;
            this.txtXFileName.Size = new System.Drawing.Size(300, 23);
            this.txtXFileName.TabIndex = 7;
            // 
            // ckNeedFit
            // 
            this.ckNeedFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckNeedFit.AutoSize = true;
            this.ckNeedFit.Checked = true;
            this.ckNeedFit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckNeedFit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedFit.Location = new System.Drawing.Point(13, 354);
            this.ckNeedFit.Name = "ckNeedFit";
            this.ckNeedFit.Size = new System.Drawing.Size(75, 21);
            this.ckNeedFit.TabIndex = 7;
            this.ckNeedFit.Text = "线性拟合";
            this.ckNeedFit.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.rdFullImage);
            this.groupBox4.Controls.Add(this.rdAOI);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(6, 294);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(702, 54);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "像元范围";
            // 
            // rdFullImage
            // 
            this.rdFullImage.AutoSize = true;
            this.rdFullImage.Location = new System.Drawing.Point(454, 27);
            this.rdFullImage.Name = "rdFullImage";
            this.rdFullImage.Size = new System.Drawing.Size(50, 21);
            this.rdFullImage.TabIndex = 1;
            this.rdFullImage.TabStop = true;
            this.rdFullImage.Text = "全图";
            this.rdFullImage.UseVisualStyleBackColor = true;
            // 
            // rdAOI
            // 
            this.rdAOI.AutoSize = true;
            this.rdAOI.Location = new System.Drawing.Point(16, 22);
            this.rdAOI.Name = "rdAOI";
            this.rdAOI.Size = new System.Drawing.Size(116, 21);
            this.rdAOI.TabIndex = 0;
            this.rdAOI.TabStop = true;
            this.rdAOI.Text = "感兴趣区域(AOI)";
            this.rdAOI.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox2);
            this.groupBox5.Controls.Add(this.btnSelectYFile);
            this.groupBox5.Controls.Add(this.txtYFileName);
            this.groupBox5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(358, 3);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox5.Size = new System.Drawing.Size(350, 290);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Y 轴文件";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tvYBands);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(3, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 228);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Y 轴波段 - [?]";
            // 
            // tvYBands
            // 
            this.tvYBands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvYBands.HideSelection = false;
            this.tvYBands.Location = new System.Drawing.Point(16, 23);
            this.tvYBands.Name = "tvYBands";
            this.tvYBands.Size = new System.Drawing.Size(310, 187);
            this.tvYBands.TabIndex = 1;
            // 
            // btnSelectYFile
            // 
            this.btnSelectYFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectYFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectYFile.Location = new System.Drawing.Point(318, 24);
            this.btnSelectYFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectYFile.Name = "btnSelectYFile";
            this.btnSelectYFile.Size = new System.Drawing.Size(29, 24);
            this.btnSelectYFile.TabIndex = 8;
            this.btnSelectYFile.Text = "...";
            this.btnSelectYFile.UseVisualStyleBackColor = true;
            this.btnSelectYFile.Click += new System.EventHandler(this.btnSelectYFile_Click);
            // 
            // txtYFileName
            // 
            this.txtYFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtYFileName.Enabled = false;
            this.txtYFileName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtYFileName.Location = new System.Drawing.Point(12, 25);
            this.txtYFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtYFileName.Name = "txtYFileName";
            this.txtYFileName.ReadOnly = true;
            this.txtYFileName.Size = new System.Drawing.Size(300, 23);
            this.txtYFileName.TabIndex = 7;
            // 
            // frmScatterTwoVarSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 382);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.ckNeedFit);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmScatterTwoVarSelector";
            this.ShowInTaskbar = false;
            this.Text = "散点图波段选择...";
            this.Load += new System.EventHandler(this.frmScatterTwoVarSelector_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSelectXFile;
        private System.Windows.Forms.TextBox txtXFileName;
        private System.Windows.Forms.CheckBox ckNeedFit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdFullImage;
        private System.Windows.Forms.RadioButton rdAOI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView tvXBands;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView tvYBands;
        private System.Windows.Forms.Button btnSelectYFile;
        private System.Windows.Forms.TextBox txtYFileName;
    }
}