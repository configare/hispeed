namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class frmImageClip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImageClip));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnInputFile = new System.Windows.Forms.Button();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ucGeoRangeControl1 = new GeoDo.RSS.UI.AddIn.DataPro.UCGeoRangeControl();
            this.txtWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnAddMask = new System.Windows.Forms.Button();
            this.txtMask = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnohters = new System.Windows.Forms.Button();
            this.txtothers = new System.Windows.Forms.TextBox();
            this.ckbothers = new System.Windows.Forms.CheckBox();
            this.ckbangle = new System.Windows.Forms.CheckBox();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(525, 374);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(439, 374);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 25;
            this.btnOK.Text = "开始裁切";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 374);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(420, 23);
            this.progressBar1.TabIndex = 32;
            this.progressBar1.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInputFile);
            this.groupBox1.Controls.Add(this.txtInputFile);
            this.groupBox1.Location = new System.Drawing.Point(13, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(592, 48);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "输入文件";
            // 
            // btnInputFile
            // 
            this.btnInputFile.Image = ((System.Drawing.Image)(resources.GetObject("btnInputFile.Image")));
            this.btnInputFile.Location = new System.Drawing.Point(561, 16);
            this.btnInputFile.Name = "btnInputFile";
            this.btnInputFile.Size = new System.Drawing.Size(25, 23);
            this.btnInputFile.TabIndex = 10;
            this.btnInputFile.UseVisualStyleBackColor = true;
            this.btnInputFile.Click += new System.EventHandler(this.btnInputFile_Click);
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(15, 18);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.ReadOnly = true;
            this.txtInputFile.Size = new System.Drawing.Size(540, 21);
            this.txtInputFile.TabIndex = 9;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtRegionName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.ucGeoRangeControl1);
            this.groupBox3.Controls.Add(this.txtWidth);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtHeight);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(9, 123);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(596, 139);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "裁切大小及位置";
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(109, 21);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(478, 21);
            this.txtRegionName.TabIndex = 25;
            this.txtRegionName.TextChanged += new System.EventHandler(this.txtRegionName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 24;
            this.label2.Text = "区域名称";
            // 
            // ucGeoRangeControl1
            // 
            this.ucGeoRangeControl1.Location = new System.Drawing.Point(298, 42);
            this.ucGeoRangeControl1.MaxX = 0D;
            this.ucGeoRangeControl1.MaxY = 0D;
            this.ucGeoRangeControl1.MinX = 0D;
            this.ucGeoRangeControl1.MinY = 0D;
            this.ucGeoRangeControl1.Name = "ucGeoRangeControl1";
            this.ucGeoRangeControl1.Size = new System.Drawing.Size(230, 93);
            this.ucGeoRangeControl1.TabIndex = 48;
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(109, 57);
            this.txtWidth.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(120, 21);
            this.txtWidth.TabIndex = 41;
            this.txtWidth.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.txtWidth.ValueChanged += new System.EventHandler(this.txtWidth_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(235, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 45;
            this.label7.Text = "(像素)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(235, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 44;
            this.label6.Text = "(像素)";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(109, 100);
            this.txtHeight.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(120, 21);
            this.txtHeight.TabIndex = 43;
            this.txtHeight.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.txtHeight.ValueChanged += new System.EventHandler(this.txtHeight_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 42;
            this.label5.Text = "输出高度:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 40;
            this.label4.Text = "输出宽度:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnAddMask);
            this.groupBox4.Controls.Add(this.txtMask);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(9, 63);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(596, 54);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "应用矢量模版";
            // 
            // btnAddMask
            // 
            this.btnAddMask.Image = ((System.Drawing.Image)(resources.GetObject("btnAddMask.Image")));
            this.btnAddMask.Location = new System.Drawing.Point(565, 19);
            this.btnAddMask.Name = "btnAddMask";
            this.btnAddMask.Size = new System.Drawing.Size(25, 23);
            this.btnAddMask.TabIndex = 23;
            this.btnAddMask.UseVisualStyleBackColor = true;
            this.btnAddMask.Click += new System.EventHandler(this.btnAddMask_Click);
            // 
            // txtMask
            // 
            this.txtMask.Location = new System.Drawing.Point(109, 20);
            this.txtMask.Name = "txtMask";
            this.txtMask.ReadOnly = true;
            this.txtMask.Size = new System.Drawing.Size(450, 21);
            this.txtMask.TabIndex = 22;
            this.txtMask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMask_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "矢量模版";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnohters);
            this.groupBox5.Controls.Add(this.txtothers);
            this.groupBox5.Controls.Add(this.ckbothers);
            this.groupBox5.Controls.Add(this.ckbangle);
            this.groupBox5.Controls.Add(this.btnSaveFile);
            this.groupBox5.Controls.Add(this.txtOutFile);
            this.groupBox5.Location = new System.Drawing.Point(9, 269);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(596, 99);
            this.groupBox5.TabIndex = 37;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "输出目录";
            // 
            // btnohters
            // 
            this.btnohters.Enabled = false;
            this.btnohters.Image = ((System.Drawing.Image)(resources.GetObject("btnohters.Image")));
            this.btnohters.Location = new System.Drawing.Point(322, 71);
            this.btnohters.Name = "btnohters";
            this.btnohters.Size = new System.Drawing.Size(25, 23);
            this.btnohters.TabIndex = 52;
            this.btnohters.UseVisualStyleBackColor = true;
            this.btnohters.Click += new System.EventHandler(this.btnohters_Click);
            // 
            // txtothers
            // 
            this.txtothers.Location = new System.Drawing.Point(97, 72);
            this.txtothers.Name = "txtothers";
            this.txtothers.ReadOnly = true;
            this.txtothers.Size = new System.Drawing.Size(219, 21);
            this.txtothers.TabIndex = 51;
            // 
            // ckbothers
            // 
            this.ckbothers.AutoSize = true;
            this.ckbothers.Location = new System.Drawing.Point(19, 77);
            this.ckbothers.Name = "ckbothers";
            this.ckbothers.Size = new System.Drawing.Size(72, 16);
            this.ckbothers.TabIndex = 50;
            this.ckbothers.Text = "其他文件";
            this.ckbothers.UseVisualStyleBackColor = true;
            this.ckbothers.CheckedChanged += new System.EventHandler(this.ckbothers_CheckedChanged);
            // 
            // ckbangle
            // 
            this.ckbangle.AutoSize = true;
            this.ckbangle.Location = new System.Drawing.Point(19, 45);
            this.ckbangle.Name = "ckbangle";
            this.ckbangle.Size = new System.Drawing.Size(72, 16);
            this.ckbangle.TabIndex = 49;
            this.ckbangle.Text = "角度文件";
            this.ckbangle.UseVisualStyleBackColor = true;
            this.ckbangle.CheckedChanged += new System.EventHandler(this.ckbangle_CheckedChanged);
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveFile.Image")));
            this.btnSaveFile.Location = new System.Drawing.Point(562, 16);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(25, 23);
            this.btnSaveFile.TabIndex = 12;
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(19, 18);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.ReadOnly = true;
            this.txtOutFile.Size = new System.Drawing.Size(540, 21);
            this.txtOutFile.TabIndex = 11;
            // 
            // frmImageClip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 408);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmImageClip";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据裁切...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown txtWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnAddMask;
        private System.Windows.Forms.TextBox txtMask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnInputFile;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.TextBox txtOutFile;
        private UCGeoRangeControl ucGeoRangeControl1;
        private System.Windows.Forms.TextBox txtRegionName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnohters;
        private System.Windows.Forms.TextBox txtothers;
        private System.Windows.Forms.CheckBox ckbothers;
        private System.Windows.Forms.CheckBox ckbangle;
    }
}