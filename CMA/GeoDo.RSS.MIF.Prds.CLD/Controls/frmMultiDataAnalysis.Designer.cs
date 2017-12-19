namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class frmMultiDataAnalysis
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstInputFiles = new System.Windows.Forms.ListBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.labOutDir = new System.Windows.Forms.Label();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.chkVtneeded = new System.Windows.Forms.CheckBox();
            this.chkUneeded = new System.Windows.Forms.CheckBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "待统计分析文件列表:";
            // 
            // lstInputFiles
            // 
            this.lstInputFiles.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstInputFiles.FormattingEnabled = true;
            this.lstInputFiles.ItemHeight = 17;
            this.lstInputFiles.Location = new System.Drawing.Point(136, 20);
            this.lstInputFiles.Name = "lstInputFiles";
            this.lstInputFiles.Size = new System.Drawing.Size(261, 123);
            this.lstInputFiles.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpen.Location = new System.Drawing.Point(407, 20);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(25, 27);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "+";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // labOutDir
            // 
            this.labOutDir.AutoSize = true;
            this.labOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labOutDir.Location = new System.Drawing.Point(12, 157);
            this.labOutDir.Name = "labOutDir";
            this.labOutDir.Size = new System.Drawing.Size(71, 17);
            this.labOutDir.TabIndex = 3;
            this.labOutDir.Text = "输出文件夹:";
            // 
            // txtOutDir
            // 
            this.txtOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOutDir.Location = new System.Drawing.Point(136, 154);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(261, 23);
            this.txtOutDir.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.chkVtneeded);
            this.panel1.Controls.Add(this.chkUneeded);
            this.panel1.Controls.Add(this.btnOutDir);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lstInputFiles);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.labOutDir);
            this.panel1.Controls.Add(this.txtOutDir);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 257);
            this.panel1.TabIndex = 5;
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRemove.Location = new System.Drawing.Point(407, 53);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 27);
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "--";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(354, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Location = new System.Drawing.Point(260, 220);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // chkVtneeded
            // 
            this.chkVtneeded.AutoSize = true;
            this.chkVtneeded.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkVtneeded.Location = new System.Drawing.Point(156, 190);
            this.chkVtneeded.Name = "chkVtneeded";
            this.chkVtneeded.Size = new System.Drawing.Size(135, 21);
            this.chkVtneeded.TabIndex = 7;
            this.chkVtneeded.Text = "Vt矩阵大小是否可变";
            this.chkVtneeded.UseVisualStyleBackColor = true;
            // 
            // chkUneeded
            // 
            this.chkUneeded.AutoSize = true;
            this.chkUneeded.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkUneeded.Location = new System.Drawing.Point(15, 190);
            this.chkUneeded.Name = "chkUneeded";
            this.chkUneeded.Size = new System.Drawing.Size(132, 21);
            this.chkUneeded.TabIndex = 6;
            this.chkUneeded.Text = "U矩阵大小是否可变";
            this.chkUneeded.UseVisualStyleBackColor = true;
            // 
            // btnOutDir
            // 
            this.btnOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOutDir.Location = new System.Drawing.Point(407, 154);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(25, 23);
            this.btnOutDir.TabIndex = 5;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // frmMultiDataAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 257);
            this.Controls.Add(this.panel1);
            this.Name = "frmMultiDataAnalysis";
            this.ShowIcon = false;
            this.Text = "SVD分解...";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstInputFiles;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label labOutDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox chkVtneeded;
        private System.Windows.Forms.CheckBox chkUneeded;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Button btnRemove;

    }
}