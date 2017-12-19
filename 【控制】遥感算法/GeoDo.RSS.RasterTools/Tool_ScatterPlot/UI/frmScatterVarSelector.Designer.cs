namespace GeoDo.RSS.RasterTools
{
    partial class frmScatterVarSelector
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvXBands = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvYBands = new System.Windows.Forms.TreeView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.ckNeedFit = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdAOI = new System.Windows.Forms.RadioButton();
            this.rdFullImage = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvXBands);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(12, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 177);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X 轴波段 - [?]";
            // 
            // tvXBands
            // 
            this.tvXBands.FullRowSelect = true;
            this.tvXBands.HideSelection = false;
            this.tvXBands.Location = new System.Drawing.Point(16, 23);
            this.tvXBands.Name = "tvXBands";
            this.tvXBands.Size = new System.Drawing.Size(266, 136);
            this.tvXBands.TabIndex = 0;
            this.tvXBands.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvXBands_MouseClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tvYBands);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(325, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 177);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Y 轴波段 - [?]";
            // 
            // tvYBands
            // 
            this.tvYBands.HideSelection = false;
            this.tvYBands.Location = new System.Drawing.Point(16, 23);
            this.tvYBands.Name = "tvYBands";
            this.tvYBands.Size = new System.Drawing.Size(266, 136);
            this.tvYBands.TabIndex = 1;
            this.tvYBands.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvYBands_MouseClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(534, 318);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(439, 318);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnFile);
            this.groupBox3.Controls.Add(this.txtFileName);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(12, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(611, 60);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "文件";
            // 
            // btnFile
            // 
            this.btnFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFile.Location = new System.Drawing.Point(548, 24);
            this.btnFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(47, 24);
            this.btnFile.TabIndex = 8;
            this.btnFile.Text = "...";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Enabled = false;
            this.txtFileName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFileName.Location = new System.Drawing.Point(12, 25);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(530, 23);
            this.txtFileName.TabIndex = 7;
            // 
            // ckNeedFit
            // 
            this.ckNeedFit.AutoSize = true;
            this.ckNeedFit.Checked = true;
            this.ckNeedFit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckNeedFit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedFit.Location = new System.Drawing.Point(13, 322);
            this.ckNeedFit.Name = "ckNeedFit";
            this.ckNeedFit.Size = new System.Drawing.Size(75, 21);
            this.ckNeedFit.TabIndex = 7;
            this.ckNeedFit.Text = "线性拟合";
            this.ckNeedFit.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdFullImage);
            this.groupBox4.Controls.Add(this.rdAOI);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(12, 72);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(613, 54);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "像元范围";
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
            // rdFullImage
            // 
            this.rdFullImage.AutoSize = true;
            this.rdFullImage.Location = new System.Drawing.Point(251, 22);
            this.rdFullImage.Name = "rdFullImage";
            this.rdFullImage.Size = new System.Drawing.Size(50, 21);
            this.rdFullImage.TabIndex = 1;
            this.rdFullImage.TabStop = true;
            this.rdFullImage.Text = "全图";
            this.rdFullImage.UseVisualStyleBackColor = true;
            // 
            // frmScatterVarSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 350);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.ckNeedFit);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmScatterVarSelector";
            this.ShowInTaskbar = false;
            this.Text = "散点图波段选择...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TreeView tvXBands;
        private System.Windows.Forms.TreeView tvYBands;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.CheckBox ckNeedFit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdFullImage;
        private System.Windows.Forms.RadioButton rdAOI;
    }
}