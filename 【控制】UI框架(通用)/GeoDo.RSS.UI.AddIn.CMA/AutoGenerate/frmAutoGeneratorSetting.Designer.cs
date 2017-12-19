namespace GeoDo.RSS.UI.AddIn.CMA
{
    partial class frmAutoGeneratorSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAutoGeneratorSetting));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckIsOuputPng = new System.Windows.Forms.CheckBox();
            this.ckIsOuputGXD = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckIsCopyToFolder = new System.Windows.Forms.CheckBox();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.txtCopyFolder = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdNotOpenAfterFinished = new System.Windows.Forms.RadioButton();
            this.rdOpenAfteFinisehd = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmIsMemSettings = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdOveride = new System.Windows.Forms.RadioButton();
            this.rdRename = new System.Windows.Forms.RadioButton();
            this.rdSkip = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckIsOuputPng);
            this.groupBox1.Controls.Add(this.ckIsOuputGXD);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(14, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(575, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "生成参数";
            // 
            // ckIsOuputPng
            // 
            this.ckIsOuputPng.AutoSize = true;
            this.ckIsOuputPng.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckIsOuputPng.Location = new System.Drawing.Point(226, 26);
            this.ckIsOuputPng.Name = "ckIsOuputPng";
            this.ckIsOuputPng.Size = new System.Drawing.Size(169, 24);
            this.ckIsOuputPng.TabIndex = 1;
            this.ckIsOuputPng.Text = "生成专题图图片(.PNG)";
            this.ckIsOuputPng.UseVisualStyleBackColor = true;
            // 
            // ckIsOuputGXD
            // 
            this.ckIsOuputGXD.AutoSize = true;
            this.ckIsOuputGXD.Checked = true;
            this.ckIsOuputGXD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsOuputGXD.Enabled = false;
            this.ckIsOuputGXD.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckIsOuputGXD.Location = new System.Drawing.Point(19, 26);
            this.ckIsOuputGXD.Name = "ckIsOuputGXD";
            this.ckIsOuputGXD.Size = new System.Drawing.Size(169, 24);
            this.ckIsOuputGXD.TabIndex = 0;
            this.ckIsOuputGXD.Text = "生成专题图文档(.GXD)";
            this.ckIsOuputGXD.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckIsCopyToFolder);
            this.groupBox2.Controls.Add(this.btnOpenFolder);
            this.groupBox2.Controls.Add(this.txtCopyFolder);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(14, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(575, 58);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "将产品复制到?";
            // 
            // ckIsCopyToFolder
            // 
            this.ckIsCopyToFolder.AutoSize = true;
            this.ckIsCopyToFolder.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckIsCopyToFolder.Location = new System.Drawing.Point(19, 28);
            this.ckIsCopyToFolder.Name = "ckIsCopyToFolder";
            this.ckIsCopyToFolder.Size = new System.Drawing.Size(15, 14);
            this.ckIsCopyToFolder.TabIndex = 2;
            this.ckIsCopyToFolder.UseVisualStyleBackColor = true;
            this.ckIsCopyToFolder.CheckedChanged += new System.EventHandler(this.ckIsCopyToFolder_CheckedChanged);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Enabled = false;
            this.btnOpenFolder.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpenFolder.Image = global::GeoDo.RSS.UI.AddIn.CMA.Properties.Resources.cmdOpen;
            this.btnOpenFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenFolder.Location = new System.Drawing.Point(464, 22);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(105, 27);
            this.btnOpenFolder.TabIndex = 1;
            this.btnOpenFolder.Text = "选择文件夹";
            this.btnOpenFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // txtCopyFolder
            // 
            this.txtCopyFolder.Enabled = false;
            this.txtCopyFolder.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCopyFolder.Location = new System.Drawing.Point(40, 23);
            this.txtCopyFolder.Name = "txtCopyFolder";
            this.txtCopyFolder.Size = new System.Drawing.Size(418, 26);
            this.txtCopyFolder.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdNotOpenAfterFinished);
            this.groupBox3.Controls.Add(this.rdOpenAfteFinisehd);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(14, 203);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(575, 58);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "生成后";
            // 
            // rdNotOpenAfterFinished
            // 
            this.rdNotOpenAfterFinished.AutoSize = true;
            this.rdNotOpenAfterFinished.Checked = true;
            this.rdNotOpenAfterFinished.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdNotOpenAfterFinished.Location = new System.Drawing.Point(215, 23);
            this.rdNotOpenAfterFinished.Name = "rdNotOpenAfterFinished";
            this.rdNotOpenAfterFinished.Size = new System.Drawing.Size(219, 24);
            this.rdNotOpenAfterFinished.TabIndex = 1;
            this.rdNotOpenAfterFinished.TabStop = true;
            this.rdNotOpenAfterFinished.Text = "不打开文件(只保存在工作空间)";
            this.rdNotOpenAfterFinished.UseVisualStyleBackColor = true;
            // 
            // rdOpenAfteFinisehd
            // 
            this.rdOpenAfteFinisehd.AutoSize = true;
            this.rdOpenAfteFinisehd.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdOpenAfteFinisehd.Location = new System.Drawing.Point(19, 23);
            this.rdOpenAfteFinisehd.Name = "rdOpenAfteFinisehd";
            this.rdOpenAfteFinisehd.Size = new System.Drawing.Size(83, 24);
            this.rdOpenAfteFinisehd.TabIndex = 0;
            this.rdOpenAfteFinisehd.Text = "打开文件";
            this.rdOpenAfteFinisehd.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(403, 273);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(499, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmIsMemSettings
            // 
            this.cmIsMemSettings.AutoSize = true;
            this.cmIsMemSettings.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmIsMemSettings.Location = new System.Drawing.Point(14, 271);
            this.cmIsMemSettings.Name = "cmIsMemSettings";
            this.cmIsMemSettings.Size = new System.Drawing.Size(84, 24);
            this.cmIsMemSettings.TabIndex = 5;
            this.cmIsMemSettings.Text = "记住设置";
            this.cmIsMemSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdSkip);
            this.groupBox4.Controls.Add(this.rdRename);
            this.groupBox4.Controls.Add(this.rdOveride);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(14, 75);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(575, 59);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "文件存在时如何处理？";
            // 
            // rdOveride
            // 
            this.rdOveride.AutoSize = true;
            this.rdOveride.Checked = true;
            this.rdOveride.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdOveride.Location = new System.Drawing.Point(19, 23);
            this.rdOveride.Name = "rdOveride";
            this.rdOveride.Size = new System.Drawing.Size(55, 24);
            this.rdOveride.TabIndex = 1;
            this.rdOveride.TabStop = true;
            this.rdOveride.Text = "覆盖";
            this.rdOveride.UseVisualStyleBackColor = true;
            // 
            // rdRename
            // 
            this.rdRename.AutoSize = true;
            this.rdRename.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdRename.Location = new System.Drawing.Point(180, 23);
            this.rdRename.Name = "rdRename";
            this.rdRename.Size = new System.Drawing.Size(83, 24);
            this.rdRename.TabIndex = 2;
            this.rdRename.Text = "重命名为";
            this.rdRename.UseVisualStyleBackColor = true;
            // 
            // rdSkip
            // 
            this.rdSkip.AutoSize = true;
            this.rdSkip.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdSkip.Location = new System.Drawing.Point(347, 23);
            this.rdSkip.Name = "rdSkip";
            this.rdSkip.Size = new System.Drawing.Size(55, 24);
            this.rdSkip.TabIndex = 3;
            this.rdSkip.Text = "跳过";
            this.rdSkip.UseVisualStyleBackColor = true;
            // 
            // frmAutoGeneratorSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 311);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.cmIsMemSettings);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAutoGeneratorSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "快速生成参数设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckIsOuputPng;
        private System.Windows.Forms.CheckBox ckIsOuputGXD;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ckIsCopyToFolder;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.TextBox txtCopyFolder;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdNotOpenAfterFinished;
        private System.Windows.Forms.RadioButton rdOpenAfteFinisehd;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cmIsMemSettings;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdSkip;
        private System.Windows.Forms.RadioButton rdRename;
        private System.Windows.Forms.RadioButton rdOveride;
    }
}