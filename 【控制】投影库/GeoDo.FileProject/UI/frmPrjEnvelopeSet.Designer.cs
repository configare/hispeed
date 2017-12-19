namespace GeoDo.FileProject
{
    partial class frmPrjEnvelopeSet
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.prjArgsSelectBand1 = new GeoDo.FileProject.PrjArgsSelectBand();
            this.txtOutDirOrFile = new System.Windows.Forms.TextBox();
            this.btnOutDirOrFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.prjEnvelopeSet1 = new GeoDo.FileProject.PrjEnvelopeSet();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(410, 432);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(506, 432);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.prjArgsSelectBand1);
            this.groupBox1.Location = new System.Drawing.Point(295, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 357);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择波段";
            // 
            // prjArgsSelectBand1
            // 
            this.prjArgsSelectBand1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prjArgsSelectBand1.Location = new System.Drawing.Point(3, 17);
            this.prjArgsSelectBand1.Name = "prjArgsSelectBand1";
            this.prjArgsSelectBand1.Size = new System.Drawing.Size(281, 337);
            this.prjArgsSelectBand1.TabIndex = 4;
            // 
            // txtOutDirOrFile
            // 
            this.txtOutDirOrFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutDirOrFile.Location = new System.Drawing.Point(6, 20);
            this.txtOutDirOrFile.Name = "txtOutDirOrFile";
            this.txtOutDirOrFile.ReadOnly = true;
            this.txtOutDirOrFile.Size = new System.Drawing.Size(524, 21);
            this.txtOutDirOrFile.TabIndex = 7;
            // 
            // btnOutDirOrFile
            // 
            this.btnOutDirOrFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutDirOrFile.Location = new System.Drawing.Point(536, 18);
            this.btnOutDirOrFile.Name = "btnOutDirOrFile";
            this.btnOutDirOrFile.Size = new System.Drawing.Size(27, 23);
            this.btnOutDirOrFile.TabIndex = 8;
            this.btnOutDirOrFile.UseVisualStyleBackColor = true;
            this.btnOutDirOrFile.Click += new System.EventHandler(this.btnOutDirOrFile_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnOutDirOrFile);
            this.groupBox2.Controls.Add(this.txtOutDirOrFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(569, 49);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "投影输出位置";
            // 
            // prjEnvelopeSet1
            // 
            this.prjEnvelopeSet1.Location = new System.Drawing.Point(12, 60);
            this.prjEnvelopeSet1.Name = "prjEnvelopeSet1";
            this.prjEnvelopeSet1.Size = new System.Drawing.Size(262, 357);
            this.prjEnvelopeSet1.TabIndex = 0;
            // 
            // frmPrjEnvelopeSet
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(592, 470);
            this.Controls.Add(this.prjEnvelopeSet1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPrjEnvelopeSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "交互参数输入";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private PrjArgsSelectBand prjArgsSelectBand1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutDirOrFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtOutDirOrFile;
        private PrjEnvelopeSet prjEnvelopeSet1;
    }
}