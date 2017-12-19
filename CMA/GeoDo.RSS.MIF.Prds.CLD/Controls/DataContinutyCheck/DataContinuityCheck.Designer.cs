namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class DataContinuityCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataContinuityCheck));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbISCCP = new System.Windows.Forms.RadioButton();
            this.rdbAIRS = new System.Windows.Forms.RadioButton();
            this.rdbMODIS = new System.Windows.Forms.RadioButton();
            this.btnOpenInDir = new System.Windows.Forms.Button();
            this.txtInDir = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxPrdsLevl = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpenOutputLog = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rdbISCCP);
            this.groupBox2.Controls.Add(this.rdbAIRS);
            this.groupBox2.Controls.Add(this.rdbMODIS);
            this.groupBox2.Controls.Add(this.btnOpenInDir);
            this.groupBox2.Controls.Add(this.txtInDir);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbxPrdsLevl);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(413, 147);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据选择";
            // 
            // rdbISCCP
            // 
            this.rdbISCCP.AutoSize = true;
            this.rdbISCCP.Location = new System.Drawing.Point(321, 72);
            this.rdbISCCP.Name = "rdbISCCP";
            this.rdbISCCP.Size = new System.Drawing.Size(66, 20);
            this.rdbISCCP.TabIndex = 30;
            this.rdbISCCP.TabStop = true;
            this.rdbISCCP.Text = "ISCCP";
            this.rdbISCCP.UseVisualStyleBackColor = true;
            // 
            // rdbAIRS
            // 
            this.rdbAIRS.AutoSize = true;
            this.rdbAIRS.Location = new System.Drawing.Point(206, 70);
            this.rdbAIRS.Name = "rdbAIRS";
            this.rdbAIRS.Size = new System.Drawing.Size(58, 20);
            this.rdbAIRS.TabIndex = 29;
            this.rdbAIRS.TabStop = true;
            this.rdbAIRS.Text = "AIRS";
            this.rdbAIRS.UseVisualStyleBackColor = true;
            // 
            // rdbMODIS
            // 
            this.rdbMODIS.AutoSize = true;
            this.rdbMODIS.Location = new System.Drawing.Point(99, 70);
            this.rdbMODIS.Name = "rdbMODIS";
            this.rdbMODIS.Size = new System.Drawing.Size(66, 20);
            this.rdbMODIS.TabIndex = 28;
            this.rdbMODIS.TabStop = true;
            this.rdbMODIS.Text = "MODIS";
            this.rdbMODIS.UseVisualStyleBackColor = true;
            // 
            // btnOpenInDir
            // 
            this.btnOpenInDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenInDir.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenInDir.Image")));
            this.btnOpenInDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenInDir.Location = new System.Drawing.Point(380, 31);
            this.btnOpenInDir.Name = "btnOpenInDir";
            this.btnOpenInDir.Size = new System.Drawing.Size(26, 23);
            this.btnOpenInDir.TabIndex = 26;
            this.btnOpenInDir.UseVisualStyleBackColor = true;
            this.btnOpenInDir.Click += new System.EventHandler(this.btnOpenInDir_Click);
            // 
            // txtInDir
            // 
            this.txtInDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInDir.Location = new System.Drawing.Point(92, 31);
            this.txtInDir.Name = "txtInDir";
            this.txtInDir.Size = new System.Drawing.Size(282, 26);
            this.txtInDir.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 24;
            this.label4.Text = "输入目录:";
            // 
            // cbxPrdsLevl
            // 
            this.cbxPrdsLevl.FormattingEnabled = true;
            this.cbxPrdsLevl.Location = new System.Drawing.Point(93, 102);
            this.cbxPrdsLevl.Name = "cbxPrdsLevl";
            this.cbxPrdsLevl.Size = new System.Drawing.Size(180, 24);
            this.cbxPrdsLevl.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(9, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "数据类型:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 16);
            this.label9.TabIndex = 3;
            this.label9.Text = "数据级别:";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.txtErrorLog);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 12F);
            this.groupBox4.Location = new System.Drawing.Point(12, 156);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(412, 201);
            this.groupBox4.TabIndex = 33;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "提示";
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorLog.Location = new System.Drawing.Point(3, 22);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorLog.Size = new System.Drawing.Size(406, 176);
            this.txtErrorLog.TabIndex = 29;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(295, 363);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(58, 23);
            this.btnOK.TabIndex = 34;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(359, 363);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 23);
            this.btnCancel.TabIndex = 35;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenOutputLog
            // 
            this.btnOpenOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenOutputLog.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenOutputLog.Image")));
            this.btnOpenOutputLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenOutputLog.Location = new System.Drawing.Point(232, 363);
            this.btnOpenOutputLog.Name = "btnOpenOutputLog";
            this.btnOpenOutputLog.Size = new System.Drawing.Size(55, 23);
            this.btnOpenOutputLog.TabIndex = 36;
            this.btnOpenOutputLog.Text = "日志";
            this.btnOpenOutputLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenOutputLog.UseVisualStyleBackColor = true;
            this.btnOpenOutputLog.Click += new System.EventHandler(this.btnOpenOutputLog_Click);
            // 
            // DataContinuityCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 391);
            this.Controls.Add(this.btnOpenOutputLog);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Name = "DataContinuityCheck";
            this.ShowIcon = false;
            this.Text = "数据完整性检验";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbxPrdsLevl;
        private System.Windows.Forms.Button btnOpenInDir;
        private System.Windows.Forms.TextBox txtInDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdbISCCP;
        private System.Windows.Forms.RadioButton rdbAIRS;
        private System.Windows.Forms.RadioButton rdbMODIS;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpenOutputLog;
    }
}