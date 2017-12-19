namespace GeoDo.RSS.MIF.Prds.ASL
{
    partial class frmDoAerosolReversion
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOutputDir = new System.Windows.Forms.Button();
            this.tbProductOutDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAncillaryDir = new System.Windows.Forms.Button();
            this.tbMODISAncillaryDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMODISDataDir = new System.Windows.Forms.Button();
            this.tbMODISDataDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenPrdDir = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 263);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(547, 18);
            this.progressBar1.TabIndex = 8;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(363, 230);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 20);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(141, 230);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(56, 20);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.btnOutputDir);
            this.groupBox1.Controls.Add(this.tbProductOutDir);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnAncillaryDir);
            this.groupBox1.Controls.Add(this.tbMODISAncillaryDir);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnMODISDataDir);
            this.groupBox1.Controls.Add(this.tbMODISDataDir);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(23, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(489, 177);
            this.groupBox1.TabIndex = 111;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "气溶胶数据提取参数设置（选择同一天的MODIS L2级数据和辅助数据）";
            // 
            // btnOutputDir
            // 
            this.btnOutputDir.Location = new System.Drawing.Point(448, 124);
            this.btnOutputDir.Margin = new System.Windows.Forms.Padding(2);
            this.btnOutputDir.Name = "btnOutputDir";
            this.btnOutputDir.Size = new System.Drawing.Size(30, 24);
            this.btnOutputDir.TabIndex = 6;
            this.btnOutputDir.Text = "...";
            this.btnOutputDir.UseVisualStyleBackColor = true;
            this.btnOutputDir.Click += new System.EventHandler(this.btnOutputDir_Click);
            // 
            // tbProductOutDir
            // 
            this.tbProductOutDir.Location = new System.Drawing.Point(118, 126);
            this.tbProductOutDir.Margin = new System.Windows.Forms.Padding(2);
            this.tbProductOutDir.Name = "tbProductOutDir";
            this.tbProductOutDir.Size = new System.Drawing.Size(322, 21);
            this.tbProductOutDir.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 129);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "气溶胶产品输出目录";
            // 
            // btnAncillaryDir
            // 
            this.btnAncillaryDir.Location = new System.Drawing.Point(448, 78);
            this.btnAncillaryDir.Margin = new System.Windows.Forms.Padding(2);
            this.btnAncillaryDir.Name = "btnAncillaryDir";
            this.btnAncillaryDir.Size = new System.Drawing.Size(30, 24);
            this.btnAncillaryDir.TabIndex = 4;
            this.btnAncillaryDir.Text = "...";
            this.btnAncillaryDir.UseVisualStyleBackColor = true;
            this.btnAncillaryDir.Click += new System.EventHandler(this.btnAncillaryDir_Click);
            // 
            // tbMODISAncillaryDir
            // 
            this.tbMODISAncillaryDir.Location = new System.Drawing.Point(118, 81);
            this.tbMODISAncillaryDir.Margin = new System.Windows.Forms.Padding(2);
            this.tbMODISAncillaryDir.Name = "tbMODISAncillaryDir";
            this.tbMODISAncillaryDir.Size = new System.Drawing.Size(322, 21);
            this.tbMODISAncillaryDir.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "MODIS辅助产品目录";
            // 
            // btnMODISDataDir
            // 
            this.btnMODISDataDir.Location = new System.Drawing.Point(448, 34);
            this.btnMODISDataDir.Margin = new System.Windows.Forms.Padding(2);
            this.btnMODISDataDir.Name = "btnMODISDataDir";
            this.btnMODISDataDir.Size = new System.Drawing.Size(30, 24);
            this.btnMODISDataDir.TabIndex = 2;
            this.btnMODISDataDir.Text = "...";
            this.btnMODISDataDir.UseVisualStyleBackColor = true;
            this.btnMODISDataDir.Click += new System.EventHandler(this.btnMODISDataDir_Click);
            // 
            // tbMODISDataDir
            // 
            this.tbMODISDataDir.Location = new System.Drawing.Point(118, 36);
            this.tbMODISDataDir.Margin = new System.Windows.Forms.Padding(2);
            this.tbMODISDataDir.Name = "tbMODISDataDir";
            this.tbMODISDataDir.Size = new System.Drawing.Size(322, 21);
            this.tbMODISDataDir.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "MODIS L2 产品目录";
            // 
            // btnOpenPrdDir
            // 
            this.btnOpenPrdDir.Location = new System.Drawing.Point(237, 230);
            this.btnOpenPrdDir.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenPrdDir.Name = "btnOpenPrdDir";
            this.btnOpenPrdDir.Size = new System.Drawing.Size(86, 20);
            this.btnOpenPrdDir.TabIndex = 8;
            this.btnOpenPrdDir.Text = "打开产品位置";
            this.btnOpenPrdDir.UseVisualStyleBackColor = true;
            this.btnOpenPrdDir.Click += new System.EventHandler(this.btnOpenPrdDir_Click);
            // 
            // frmDoAerosolReversion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 281);
            this.Controls.Add(this.btnOpenPrdDir);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmDoAerosolReversion";
            this.Text = "MODIS气溶胶数据反演";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutputDir;
        private System.Windows.Forms.TextBox tbProductOutDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAncillaryDir;
        private System.Windows.Forms.TextBox tbMODISAncillaryDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMODISDataDir;
        private System.Windows.Forms.TextBox tbMODISDataDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenPrdDir;
    }
}