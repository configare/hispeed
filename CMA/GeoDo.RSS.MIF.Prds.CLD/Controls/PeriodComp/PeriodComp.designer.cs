namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class PeriodComp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeriodComp));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtTips = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblInputDir = new System.Windows.Forms.Label();
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.txtInputDir = new System.Windows.Forms.TextBox();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxOverlap = new System.Windows.Forms.CheckBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.cbxTen = new System.Windows.Forms.CheckBox();
            this.cbxMonth = new System.Windows.Forms.CheckBox();
            this.cbxYear = new System.Windows.Forms.CheckBox();
            this.lblDatasets = new System.Windows.Forms.Label();
            this.lblDataTime = new System.Windows.Forms.Label();
            this.combxYears = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxMax = new System.Windows.Forms.CheckBox();
            this.cbxMin = new System.Windows.Forms.CheckBox();
            this.cbxAVG = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutDir = new System.Windows.Forms.Button();
            this.btnBrowseInDir = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.radibtnAIRS = new System.Windows.Forms.RadioButton();
            this.radibtnMOD06 = new System.Windows.Forms.RadioButton();
            this.ucMonths = new GeoDo.RSS.MIF.Prds.CLD.ucMonths();
            this.ucCheckBoxListDataSet = new GeoDo.RSS.MIF.Prds.CLD.ucCheckBoxList();
            this.radibtnMYD06 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 357);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(775, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // txtTips
            // 
            this.txtTips.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTips.BackColor = System.Drawing.SystemColors.Control;
            this.txtTips.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTips.Location = new System.Drawing.Point(12, 334);
            this.txtTips.Name = "txtTips";
            this.txtTips.Size = new System.Drawing.Size(564, 14);
            this.txtTips.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(646, 328);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(58, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(711, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblInputDir
            // 
            this.lblInputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInputDir.AutoSize = true;
            this.lblInputDir.Location = new System.Drawing.Point(12, 9);
            this.lblInputDir.Name = "lblInputDir";
            this.lblInputDir.Size = new System.Drawing.Size(59, 12);
            this.lblInputDir.TabIndex = 4;
            this.lblInputDir.Text = "输入目录:";
            // 
            // lblOutputDir
            // 
            this.lblOutputDir.AutoSize = true;
            this.lblOutputDir.Location = new System.Drawing.Point(12, 41);
            this.lblOutputDir.Name = "lblOutputDir";
            this.lblOutputDir.Size = new System.Drawing.Size(59, 12);
            this.lblOutputDir.TabIndex = 5;
            this.lblOutputDir.Text = "输出目录:";
            // 
            // txtInputDir
            // 
            this.txtInputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputDir.BackColor = System.Drawing.Color.White;
            this.txtInputDir.Location = new System.Drawing.Point(78, 6);
            this.txtInputDir.Name = "txtInputDir";
            this.txtInputDir.Size = new System.Drawing.Size(626, 21);
            this.txtInputDir.TabIndex = 6;
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.BackColor = System.Drawing.Color.White;
            this.txtOutputDir.Location = new System.Drawing.Point(78, 36);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(626, 21);
            this.txtOutputDir.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "统计类型:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "周期类型:";
            // 
            // cbxOverlap
            // 
            this.cbxOverlap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOverlap.AutoSize = true;
            this.cbxOverlap.Location = new System.Drawing.Point(608, 75);
            this.cbxOverlap.Name = "cbxOverlap";
            this.cbxOverlap.Size = new System.Drawing.Size(96, 16);
            this.cbxOverlap.TabIndex = 12;
            this.cbxOverlap.Text = "覆盖历史结果";
            this.cbxOverlap.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(582, 328);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(58, 23);
            this.btnEdit.TabIndex = 13;
            this.btnEdit.Text = "配置";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // cbxTen
            // 
            this.cbxTen.AutoSize = true;
            this.cbxTen.Enabled = false;
            this.cbxTen.Location = new System.Drawing.Point(89, 105);
            this.cbxTen.Name = "cbxTen";
            this.cbxTen.Size = new System.Drawing.Size(36, 16);
            this.cbxTen.TabIndex = 14;
            this.cbxTen.Text = "旬";
            this.cbxTen.UseVisualStyleBackColor = true;
            // 
            // cbxMonth
            // 
            this.cbxMonth.AutoSize = true;
            this.cbxMonth.Location = new System.Drawing.Point(144, 105);
            this.cbxMonth.Name = "cbxMonth";
            this.cbxMonth.Size = new System.Drawing.Size(36, 16);
            this.cbxMonth.TabIndex = 15;
            this.cbxMonth.Text = "月";
            this.cbxMonth.UseVisualStyleBackColor = true;
            this.cbxMonth.CheckedChanged += new System.EventHandler(this.cbxMonth_CheckedChanged);
            // 
            // cbxYear
            // 
            this.cbxYear.AutoSize = true;
            this.cbxYear.Location = new System.Drawing.Point(198, 105);
            this.cbxYear.Name = "cbxYear";
            this.cbxYear.Size = new System.Drawing.Size(36, 16);
            this.cbxYear.TabIndex = 16;
            this.cbxYear.Text = "年";
            this.cbxYear.UseVisualStyleBackColor = true;
            this.cbxYear.CheckedChanged += new System.EventHandler(this.cbxYear_CheckedChanged);
            // 
            // lblDatasets
            // 
            this.lblDatasets.AutoSize = true;
            this.lblDatasets.Location = new System.Drawing.Point(24, 238);
            this.lblDatasets.Name = "lblDatasets";
            this.lblDatasets.Size = new System.Drawing.Size(47, 12);
            this.lblDatasets.TabIndex = 17;
            this.lblDatasets.Text = "数据集:";
            // 
            // lblDataTime
            // 
            this.lblDataTime.AutoSize = true;
            this.lblDataTime.Location = new System.Drawing.Point(24, 173);
            this.lblDataTime.Name = "lblDataTime";
            this.lblDataTime.Size = new System.Drawing.Size(47, 12);
            this.lblDataTime.TabIndex = 19;
            this.lblDataTime.Text = "数据年:";
            // 
            // combxYears
            // 
            this.combxYears.FormattingEnabled = true;
            this.combxYears.Location = new System.Drawing.Point(89, 170);
            this.combxYears.Name = "combxYears";
            this.combxYears.Size = new System.Drawing.Size(121, 20);
            this.combxYears.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 52;
            this.label3.Text = "月份:";
            this.label3.Visible = false;
            // 
            // cbxMax
            // 
            this.cbxMax.AutoSize = true;
            this.cbxMax.Location = new System.Drawing.Point(198, 74);
            this.cbxMax.Name = "cbxMax";
            this.cbxMax.Size = new System.Drawing.Size(42, 16);
            this.cbxMax.TabIndex = 56;
            this.cbxMax.Text = "Max";
            this.cbxMax.UseVisualStyleBackColor = true;
            this.cbxMax.Visible = false;
            // 
            // cbxMin
            // 
            this.cbxMin.AutoSize = true;
            this.cbxMin.Location = new System.Drawing.Point(144, 74);
            this.cbxMin.Name = "cbxMin";
            this.cbxMin.Size = new System.Drawing.Size(42, 16);
            this.cbxMin.TabIndex = 55;
            this.cbxMin.Text = "Min";
            this.cbxMin.UseVisualStyleBackColor = true;
            this.cbxMin.Visible = false;
            // 
            // cbxAVG
            // 
            this.cbxAVG.AutoSize = true;
            this.cbxAVG.Location = new System.Drawing.Point(89, 74);
            this.cbxAVG.Name = "cbxAVG";
            this.cbxAVG.Size = new System.Drawing.Size(42, 16);
            this.cbxAVG.TabIndex = 54;
            this.cbxAVG.Text = "Avg";
            this.cbxAVG.UseVisualStyleBackColor = true;
            // 
            // btnBrowseOutDir
            // 
            this.btnBrowseOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutDir.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseOutDir.Image")));
            this.btnBrowseOutDir.Location = new System.Drawing.Point(722, 32);
            this.btnBrowseOutDir.Name = "btnBrowseOutDir";
            this.btnBrowseOutDir.Size = new System.Drawing.Size(24, 24);
            this.btnBrowseOutDir.TabIndex = 58;
            this.btnBrowseOutDir.UseVisualStyleBackColor = true;
            this.btnBrowseOutDir.Click += new System.EventHandler(this.btnBrowseOutDir_Click);
            // 
            // btnBrowseInDir
            // 
            this.btnBrowseInDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInDir.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseInDir.Image")));
            this.btnBrowseInDir.Location = new System.Drawing.Point(722, 3);
            this.btnBrowseInDir.Name = "btnBrowseInDir";
            this.btnBrowseInDir.Size = new System.Drawing.Size(24, 24);
            this.btnBrowseInDir.TabIndex = 57;
            this.btnBrowseInDir.UseVisualStyleBackColor = true;
            this.btnBrowseInDir.Click += new System.EventHandler(this.btnBrowseInDir_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 59;
            this.label4.Text = "数据源:";
            // 
            // radibtnAIRS
            // 
            this.radibtnAIRS.AutoSize = true;
            this.radibtnAIRS.Location = new System.Drawing.Point(238, 136);
            this.radibtnAIRS.Name = "radibtnAIRS";
            this.radibtnAIRS.Size = new System.Drawing.Size(47, 16);
            this.radibtnAIRS.TabIndex = 61;
            this.radibtnAIRS.Text = "AIRS";
            this.radibtnAIRS.UseVisualStyleBackColor = true;
            this.radibtnAIRS.CheckedChanged += new System.EventHandler(this.radibtnAIRS_CheckedChanged);
            // 
            // radibtnMOD06
            // 
            this.radibtnMOD06.AutoSize = true;
            this.radibtnMOD06.Location = new System.Drawing.Point(89, 136);
            this.radibtnMOD06.Name = "radibtnMOD06";
            this.radibtnMOD06.Size = new System.Drawing.Size(53, 16);
            this.radibtnMOD06.TabIndex = 60;
            this.radibtnMOD06.Text = "MOD06";
            this.radibtnMOD06.UseVisualStyleBackColor = true;
            this.radibtnMOD06.CheckedChanged += new System.EventHandler(this.radibtnMOD06_CheckedChanged);
            // 
            // ucMonths
            // 
            this.ucMonths.Location = new System.Drawing.Point(78, 200);
            this.ucMonths.Margin = new System.Windows.Forms.Padding(4);
            this.ucMonths.Name = "ucMonths";
            this.ucMonths.Size = new System.Drawing.Size(661, 19);
            this.ucMonths.TabIndex = 53;
            this.ucMonths.Visible = false;
            // 
            // ucCheckBoxListDataSet
            // 
            this.ucCheckBoxListDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucCheckBoxListDataSet.Location = new System.Drawing.Point(78, 231);
            this.ucCheckBoxListDataSet.Name = "ucCheckBoxListDataSet";
            this.ucCheckBoxListDataSet.Size = new System.Drawing.Size(668, 93);
            this.ucCheckBoxListDataSet.TabIndex = 18;
            // 
            // radibtnMYD06
            // 
            this.radibtnMYD06.AutoSize = true;
            this.radibtnMYD06.Location = new System.Drawing.Point(160, 136);
            this.radibtnMYD06.Name = "radibtnMYD06";
            this.radibtnMYD06.Size = new System.Drawing.Size(53, 16);
            this.radibtnMYD06.TabIndex = 62;
            this.radibtnMYD06.Text = "MYD06";
            this.radibtnMYD06.UseVisualStyleBackColor = true;
            this.radibtnMYD06.CheckedChanged += new System.EventHandler(this.radibtnMYD06_CheckedChanged);
            // 
            // PeriodComp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 380);
            this.Controls.Add(this.radibtnMYD06);
            this.Controls.Add(this.radibtnAIRS);
            this.Controls.Add(this.radibtnMOD06);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnBrowseOutDir);
            this.Controls.Add(this.btnBrowseInDir);
            this.Controls.Add(this.cbxMax);
            this.Controls.Add(this.cbxMin);
            this.Controls.Add(this.cbxAVG);
            this.Controls.Add(this.ucMonths);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.combxYears);
            this.Controls.Add(this.lblDataTime);
            this.Controls.Add(this.ucCheckBoxListDataSet);
            this.Controls.Add(this.lblDatasets);
            this.Controls.Add(this.cbxYear);
            this.Controls.Add(this.cbxMonth);
            this.Controls.Add(this.cbxTen);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.cbxOverlap);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.txtInputDir);
            this.Controls.Add(this.lblOutputDir);
            this.Controls.Add(this.lblInputDir);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtTips);
            this.Controls.Add(this.progressBar1);
            this.Name = "PeriodComp";
            this.ShowIcon = false;
            this.Text = "周期合成提示";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtTips;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblInputDir;
        private System.Windows.Forms.Label lblOutputDir;
        private System.Windows.Forms.TextBox txtInputDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbxOverlap;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.CheckBox cbxTen;
        private System.Windows.Forms.CheckBox cbxMonth;
        private System.Windows.Forms.CheckBox cbxYear;
        private System.Windows.Forms.Label lblDatasets;
        private ucCheckBoxList ucCheckBoxListDataSet;
        private System.Windows.Forms.Label lblDataTime;
        private System.Windows.Forms.ComboBox combxYears;
        private ucMonths ucMonths;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbxMax;
        private System.Windows.Forms.CheckBox cbxMin;
        private System.Windows.Forms.CheckBox cbxAVG;
        private System.Windows.Forms.Button btnBrowseOutDir;
        private System.Windows.Forms.Button btnBrowseInDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radibtnAIRS;
        private System.Windows.Forms.RadioButton radibtnMOD06;
        private System.Windows.Forms.RadioButton radibtnMYD06;
    }
}