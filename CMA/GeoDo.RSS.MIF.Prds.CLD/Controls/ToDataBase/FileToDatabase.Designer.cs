namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class FileToDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileToDatabase));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radiCloudSAT = new System.Windows.Forms.RadioButton();
            this.radiISCCP = new System.Windows.Forms.RadioButton();
            this.radiAIRS = new System.Windows.Forms.RadioButton();
            this.radiMODIS = new System.Windows.Forms.RadioButton();
            this.cbxOverrideRecord = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkbxAIRS = new System.Windows.Forms.CheckBox();
            this.checkbxCloudSAT = new System.Windows.Forms.CheckBox();
            this.checkbxMODIS = new System.Windows.Forms.CheckBox();
            this.checkbxISCCP = new System.Windows.Forms.CheckBox();
            this.cbxPrdsLevl = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpenOutputLog = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpenInDir = new System.Windows.Forms.Button();
            this.txtInDir = new System.Windows.Forms.TextBox();
            this.btnBrowseDocDir = new System.Windows.Forms.Button();
            this.cbxData2DocDir = new System.Windows.Forms.CheckBox();
            this.txtDocDir = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radiCloudSAT);
            this.groupBox2.Controls.Add(this.radiISCCP);
            this.groupBox2.Controls.Add(this.radiAIRS);
            this.groupBox2.Controls.Add(this.radiMODIS);
            this.groupBox2.Controls.Add(this.cbxOverrideRecord);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.checkbxAIRS);
            this.groupBox2.Controls.Add(this.checkbxCloudSAT);
            this.groupBox2.Controls.Add(this.checkbxMODIS);
            this.groupBox2.Controls.Add(this.checkbxISCCP);
            this.groupBox2.Controls.Add(this.cbxPrdsLevl);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(482, 126);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据选择";
            // 
            // radiCloudSAT
            // 
            this.radiCloudSAT.AutoSize = true;
            this.radiCloudSAT.Location = new System.Drawing.Point(366, 64);
            this.radiCloudSAT.Name = "radiCloudSAT";
            this.radiCloudSAT.Size = new System.Drawing.Size(90, 20);
            this.radiCloudSAT.TabIndex = 34;
            this.radiCloudSAT.Text = "CloudSAT";
            this.radiCloudSAT.UseVisualStyleBackColor = true;
            this.radiCloudSAT.CheckedChanged += new System.EventHandler(this.radiMODIS_CheckedChanged);
            // 
            // radiISCCP
            // 
            this.radiISCCP.AutoSize = true;
            this.radiISCCP.Location = new System.Drawing.Point(275, 64);
            this.radiISCCP.Name = "radiISCCP";
            this.radiISCCP.Size = new System.Drawing.Size(66, 20);
            this.radiISCCP.TabIndex = 33;
            this.radiISCCP.Text = "ISCCP";
            this.radiISCCP.UseVisualStyleBackColor = true;
            this.radiISCCP.CheckedChanged += new System.EventHandler(this.radiMODIS_CheckedChanged);
            // 
            // radiAIRS
            // 
            this.radiAIRS.AutoSize = true;
            this.radiAIRS.Location = new System.Drawing.Point(185, 64);
            this.radiAIRS.Name = "radiAIRS";
            this.radiAIRS.Size = new System.Drawing.Size(58, 20);
            this.radiAIRS.TabIndex = 32;
            this.radiAIRS.Text = "AIRS";
            this.radiAIRS.UseVisualStyleBackColor = true;
            this.radiAIRS.CheckedChanged += new System.EventHandler(this.radiMODIS_CheckedChanged);
            // 
            // radiMODIS
            // 
            this.radiMODIS.AutoSize = true;
            this.radiMODIS.Location = new System.Drawing.Point(98, 64);
            this.radiMODIS.Name = "radiMODIS";
            this.radiMODIS.Size = new System.Drawing.Size(66, 20);
            this.radiMODIS.TabIndex = 31;
            this.radiMODIS.Text = "MODIS";
            this.radiMODIS.UseVisualStyleBackColor = true;
            this.radiMODIS.CheckedChanged += new System.EventHandler(this.radiMODIS_CheckedChanged);
            // 
            // cbxOverrideRecord
            // 
            this.cbxOverrideRecord.AutoSize = true;
            this.cbxOverrideRecord.Checked = true;
            this.cbxOverrideRecord.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxOverrideRecord.Enabled = false;
            this.cbxOverrideRecord.Location = new System.Drawing.Point(16, 90);
            this.cbxOverrideRecord.Name = "cbxOverrideRecord";
            this.cbxOverrideRecord.Size = new System.Drawing.Size(123, 20);
            this.cbxOverrideRecord.TabIndex = 30;
            this.cbxOverrideRecord.Text = "更新历史记录";
            this.cbxOverrideRecord.UseVisualStyleBackColor = true;
            this.cbxOverrideRecord.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 16);
            this.label9.TabIndex = 28;
            this.label9.Text = "数据级别:";
            // 
            // checkbxAIRS
            // 
            this.checkbxAIRS.AutoSize = true;
            this.checkbxAIRS.Location = new System.Drawing.Point(185, 100);
            this.checkbxAIRS.Name = "checkbxAIRS";
            this.checkbxAIRS.Size = new System.Drawing.Size(59, 20);
            this.checkbxAIRS.TabIndex = 23;
            this.checkbxAIRS.Text = "AIRS";
            this.checkbxAIRS.UseVisualStyleBackColor = true;
            this.checkbxAIRS.Visible = false;
            // 
            // checkbxCloudSAT
            // 
            this.checkbxCloudSAT.AutoSize = true;
            this.checkbxCloudSAT.Location = new System.Drawing.Point(366, 99);
            this.checkbxCloudSAT.Name = "checkbxCloudSAT";
            this.checkbxCloudSAT.Size = new System.Drawing.Size(91, 20);
            this.checkbxCloudSAT.TabIndex = 22;
            this.checkbxCloudSAT.Text = "CloudSAT";
            this.checkbxCloudSAT.UseVisualStyleBackColor = true;
            this.checkbxCloudSAT.Visible = false;
            // 
            // checkbxMODIS
            // 
            this.checkbxMODIS.AutoSize = true;
            this.checkbxMODIS.Location = new System.Drawing.Point(91, 100);
            this.checkbxMODIS.Name = "checkbxMODIS";
            this.checkbxMODIS.Size = new System.Drawing.Size(67, 20);
            this.checkbxMODIS.TabIndex = 21;
            this.checkbxMODIS.Text = "MODIS";
            this.checkbxMODIS.UseVisualStyleBackColor = true;
            this.checkbxMODIS.Visible = false;
            this.checkbxMODIS.CheckedChanged += new System.EventHandler(this.checkbxMODIS_CheckedChanged);
            // 
            // checkbxISCCP
            // 
            this.checkbxISCCP.AutoSize = true;
            this.checkbxISCCP.Location = new System.Drawing.Point(274, 100);
            this.checkbxISCCP.Name = "checkbxISCCP";
            this.checkbxISCCP.Size = new System.Drawing.Size(67, 20);
            this.checkbxISCCP.TabIndex = 20;
            this.checkbxISCCP.Text = "ISCCP";
            this.checkbxISCCP.UseVisualStyleBackColor = true;
            this.checkbxISCCP.Visible = false;
            // 
            // cbxPrdsLevl
            // 
            this.cbxPrdsLevl.FormattingEnabled = true;
            this.cbxPrdsLevl.Location = new System.Drawing.Point(94, 27);
            this.cbxPrdsLevl.Name = "cbxPrdsLevl";
            this.cbxPrdsLevl.Size = new System.Drawing.Size(180, 24);
            this.cbxPrdsLevl.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(11, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "数据类型:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(351, 415);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(424, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 23);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenOutputLog
            // 
            this.btnOpenOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenOutputLog.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenOutputLog.Image")));
            this.btnOpenOutputLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenOutputLog.Location = new System.Drawing.Point(282, 415);
            this.btnOpenOutputLog.Name = "btnOpenOutputLog";
            this.btnOpenOutputLog.Size = new System.Drawing.Size(60, 23);
            this.btnOpenOutputLog.TabIndex = 26;
            this.btnOpenOutputLog.Text = "日志";
            this.btnOpenOutputLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenOutputLog.UseVisualStyleBackColor = true;
            this.btnOpenOutputLog.Click += new System.EventHandler(this.btnOpenOutputLog_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.txtErrorLog);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 12F);
            this.groupBox4.Location = new System.Drawing.Point(12, 280);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(477, 129);
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
            this.txtErrorLog.Size = new System.Drawing.Size(471, 104);
            this.txtErrorLog.TabIndex = 29;
            this.txtErrorLog.TextChanged += new System.EventHandler(this.txtErrorLog_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnOpenInDir);
            this.groupBox3.Controls.Add(this.txtInDir);
            this.groupBox3.Controls.Add(this.btnBrowseDocDir);
            this.groupBox3.Controls.Add(this.cbxData2DocDir);
            this.groupBox3.Controls.Add(this.txtDocDir);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(12, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(482, 142);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据归档";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 36;
            this.label4.Text = "输入目录:";
            // 
            // btnOpenInDir
            // 
            this.btnOpenInDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenInDir.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenInDir.Image")));
            this.btnOpenInDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenInDir.Location = new System.Drawing.Point(453, 28);
            this.btnOpenInDir.Name = "btnOpenInDir";
            this.btnOpenInDir.Size = new System.Drawing.Size(26, 23);
            this.btnOpenInDir.TabIndex = 35;
            this.btnOpenInDir.UseVisualStyleBackColor = true;
            this.btnOpenInDir.Click += new System.EventHandler(this.btnOpenInDir_Click);
            // 
            // txtInDir
            // 
            this.txtInDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInDir.Location = new System.Drawing.Point(96, 28);
            this.txtInDir.Name = "txtInDir";
            this.txtInDir.Size = new System.Drawing.Size(351, 26);
            this.txtInDir.TabIndex = 34;
            // 
            // btnBrowseDocDir
            // 
            this.btnBrowseDocDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDocDir.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseDocDir.Image")));
            this.btnBrowseDocDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseDocDir.Location = new System.Drawing.Point(453, 77);
            this.btnBrowseDocDir.Name = "btnBrowseDocDir";
            this.btnBrowseDocDir.Size = new System.Drawing.Size(26, 23);
            this.btnBrowseDocDir.TabIndex = 33;
            this.btnBrowseDocDir.UseVisualStyleBackColor = true;
            this.btnBrowseDocDir.Visible = false;
            this.btnBrowseDocDir.Click += new System.EventHandler(this.btnBrowseDocDir_Click);
            // 
            // cbxData2DocDir
            // 
            this.cbxData2DocDir.AutoSize = true;
            this.cbxData2DocDir.Checked = true;
            this.cbxData2DocDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxData2DocDir.Location = new System.Drawing.Point(33, 116);
            this.cbxData2DocDir.Name = "cbxData2DocDir";
            this.cbxData2DocDir.Size = new System.Drawing.Size(91, 20);
            this.cbxData2DocDir.TabIndex = 32;
            this.cbxData2DocDir.Text = "是否归档";
            this.cbxData2DocDir.UseVisualStyleBackColor = true;
            // 
            // txtDocDir
            // 
            this.txtDocDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDocDir.Enabled = false;
            this.txtDocDir.Location = new System.Drawing.Point(97, 74);
            this.txtDocDir.Name = "txtDocDir";
            this.txtDocDir.Size = new System.Drawing.Size(350, 26);
            this.txtDocDir.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(13, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "归档目录：";
            // 
            // FileToDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 445);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnOpenOutputLog);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "FileToDatabase";
            this.ShowIcon = false;
            this.Text = "云参数产品数据入库";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbxPrdsLevl;
        private System.Windows.Forms.CheckBox checkbxISCCP;
        private System.Windows.Forms.CheckBox checkbxMODIS;
        private System.Windows.Forms.CheckBox checkbxAIRS;
        private System.Windows.Forms.CheckBox checkbxCloudSAT;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpenOutputLog;
        private System.Windows.Forms.CheckBox cbxOverrideRecord;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpenInDir;
        private System.Windows.Forms.TextBox txtInDir;
        private System.Windows.Forms.Button btnBrowseDocDir;
        private System.Windows.Forms.CheckBox cbxData2DocDir;
        private System.Windows.Forms.TextBox txtDocDir;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton radiCloudSAT;
        private System.Windows.Forms.RadioButton radiISCCP;
        private System.Windows.Forms.RadioButton radiAIRS;
        private System.Windows.Forms.RadioButton radiMODIS;
    }
}