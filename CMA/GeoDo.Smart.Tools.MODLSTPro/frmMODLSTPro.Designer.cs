namespace GeoDo.Smart.Tools.MODLSTPro
{
    partial class frmMODLSTPro
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
            this.lstFileInfoList = new System.Windows.Forms.ListView();
            this.Filename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbFileAttr = new System.Windows.Forms.GroupBox();
            this.lbRightDownPoint = new System.Windows.Forms.Label();
            this.lbLeftUpPoint = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbProjectAgrs = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbProject = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRightDownPoint = new System.Windows.Forms.ComboBox();
            this.cbLeftUpPoint = new System.Windows.Forms.ComboBox();
            this.cbProjectAgrs = new System.Windows.Forms.ComboBox();
            this.cbProject = new System.Windows.Forms.ComboBox();
            this.gbDataSetAttr = new System.Windows.Forms.GroupBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbBandName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbOffsetValue = new System.Windows.Forms.Label();
            this.lbScaleValue = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbValidRegion = new System.Windows.Forms.Label();
            this.cbBandName = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbValidRegion = new System.Windows.Forms.ComboBox();
            this.cbOffsetValue = new System.Windows.Forms.ComboBox();
            this.cbScaleValue = new System.Windows.Forms.ComboBox();
            this.lstDataSets = new System.Windows.Forms.ListView();
            this.dataset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.desc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbDataSets = new System.Windows.Forms.GroupBox();
            this.btOK = new System.Windows.Forms.Button();
            this.ckAutoSX = new System.Windows.Forms.CheckBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.ckTimeMosaic = new System.Windows.Forms.CheckBox();
            this.btChooseDir = new System.Windows.Forms.Button();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btClear = new System.Windows.Forms.Button();
            this.btDelFiles = new System.Windows.Forms.Button();
            this.btAddFiles = new System.Windows.Forms.Button();
            this.dpBegin = new System.Windows.Forms.DateTimePicker();
            this.dpEnd = new System.Windows.Forms.DateTimePicker();
            this.ckTimeRegion = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChooseOutDir = new System.Windows.Forms.Button();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbFileCount = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.ckGCTPChina = new System.Windows.Forms.CheckBox();
            this.ckGLL = new System.Windows.Forms.CheckBox();
            this.gbFileAttr.SuspendLayout();
            this.gbDataSetAttr.SuspendLayout();
            this.gbDataSets.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstFileInfoList
            // 
            this.lstFileInfoList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Filename,
            this.Time});
            this.lstFileInfoList.FullRowSelect = true;
            this.lstFileInfoList.HideSelection = false;
            this.lstFileInfoList.Location = new System.Drawing.Point(13, 68);
            this.lstFileInfoList.MultiSelect = false;
            this.lstFileInfoList.Name = "lstFileInfoList";
            this.lstFileInfoList.Size = new System.Drawing.Size(390, 507);
            this.lstFileInfoList.TabIndex = 0;
            this.lstFileInfoList.UseCompatibleStateImageBehavior = false;
            this.lstFileInfoList.View = System.Windows.Forms.View.Details;
            this.lstFileInfoList.SelectedIndexChanged += new System.EventHandler(this.lstFileInfoList_SelectedIndexChanged);
            // 
            // Filename
            // 
            this.Filename.Text = "文件名";
            this.Filename.Width = 234;
            // 
            // Time
            // 
            this.Time.Text = "时间";
            this.Time.Width = 132;
            // 
            // gbFileAttr
            // 
            this.gbFileAttr.Controls.Add(this.lbRightDownPoint);
            this.gbFileAttr.Controls.Add(this.lbLeftUpPoint);
            this.gbFileAttr.Controls.Add(this.label5);
            this.gbFileAttr.Controls.Add(this.lbProjectAgrs);
            this.gbFileAttr.Controls.Add(this.label4);
            this.gbFileAttr.Controls.Add(this.lbProject);
            this.gbFileAttr.Controls.Add(this.label3);
            this.gbFileAttr.Controls.Add(this.label2);
            this.gbFileAttr.Controls.Add(this.cbRightDownPoint);
            this.gbFileAttr.Controls.Add(this.cbLeftUpPoint);
            this.gbFileAttr.Controls.Add(this.cbProjectAgrs);
            this.gbFileAttr.Controls.Add(this.cbProject);
            this.gbFileAttr.Enabled = false;
            this.gbFileAttr.Location = new System.Drawing.Point(420, 14);
            this.gbFileAttr.Name = "gbFileAttr";
            this.gbFileAttr.Size = new System.Drawing.Size(365, 135);
            this.gbFileAttr.TabIndex = 1;
            this.gbFileAttr.TabStop = false;
            this.gbFileAttr.Text = "文件属性";
            // 
            // lbRightDownPoint
            // 
            this.lbRightDownPoint.AutoSize = true;
            this.lbRightDownPoint.Location = new System.Drawing.Point(191, 106);
            this.lbRightDownPoint.Name = "lbRightDownPoint";
            this.lbRightDownPoint.Size = new System.Drawing.Size(0, 12);
            this.lbRightDownPoint.TabIndex = 2;
            // 
            // lbLeftUpPoint
            // 
            this.lbLeftUpPoint.AutoSize = true;
            this.lbLeftUpPoint.Location = new System.Drawing.Point(191, 80);
            this.lbLeftUpPoint.Name = "lbLeftUpPoint";
            this.lbLeftUpPoint.Size = new System.Drawing.Size(0, 12);
            this.lbLeftUpPoint.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "右下坐标";
            // 
            // lbProjectAgrs
            // 
            this.lbProjectAgrs.AutoSize = true;
            this.lbProjectAgrs.Location = new System.Drawing.Point(191, 54);
            this.lbProjectAgrs.Name = "lbProjectAgrs";
            this.lbProjectAgrs.Size = new System.Drawing.Size(0, 12);
            this.lbProjectAgrs.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "左上坐标";
            // 
            // lbProject
            // 
            this.lbProject.AutoSize = true;
            this.lbProject.Location = new System.Drawing.Point(191, 28);
            this.lbProject.Name = "lbProject";
            this.lbProject.Size = new System.Drawing.Size(0, 12);
            this.lbProject.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "投影参数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "投影类型";
            // 
            // cbRightDownPoint
            // 
            this.cbRightDownPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRightDownPoint.FormattingEnabled = true;
            this.cbRightDownPoint.Location = new System.Drawing.Point(64, 103);
            this.cbRightDownPoint.Name = "cbRightDownPoint";
            this.cbRightDownPoint.Size = new System.Drawing.Size(121, 20);
            this.cbRightDownPoint.TabIndex = 0;
            this.cbRightDownPoint.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cbLeftUpPoint
            // 
            this.cbLeftUpPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLeftUpPoint.FormattingEnabled = true;
            this.cbLeftUpPoint.Location = new System.Drawing.Point(64, 77);
            this.cbLeftUpPoint.Name = "cbLeftUpPoint";
            this.cbLeftUpPoint.Size = new System.Drawing.Size(121, 20);
            this.cbLeftUpPoint.TabIndex = 0;
            this.cbLeftUpPoint.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cbProjectAgrs
            // 
            this.cbProjectAgrs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProjectAgrs.FormattingEnabled = true;
            this.cbProjectAgrs.Location = new System.Drawing.Point(64, 51);
            this.cbProjectAgrs.Name = "cbProjectAgrs";
            this.cbProjectAgrs.Size = new System.Drawing.Size(121, 20);
            this.cbProjectAgrs.TabIndex = 0;
            this.cbProjectAgrs.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cbProject
            // 
            this.cbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProject.FormattingEnabled = true;
            this.cbProject.Location = new System.Drawing.Point(64, 25);
            this.cbProject.Name = "cbProject";
            this.cbProject.Size = new System.Drawing.Size(121, 20);
            this.cbProject.TabIndex = 0;
            this.cbProject.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // gbDataSetAttr
            // 
            this.gbDataSetAttr.Controls.Add(this.txtWidth);
            this.gbDataSetAttr.Controls.Add(this.txtHeight);
            this.gbDataSetAttr.Controls.Add(this.label10);
            this.gbDataSetAttr.Controls.Add(this.label9);
            this.gbDataSetAttr.Controls.Add(this.lbBandName);
            this.gbDataSetAttr.Controls.Add(this.label6);
            this.gbDataSetAttr.Controls.Add(this.lbOffsetValue);
            this.gbDataSetAttr.Controls.Add(this.lbScaleValue);
            this.gbDataSetAttr.Controls.Add(this.label11);
            this.gbDataSetAttr.Controls.Add(this.label8);
            this.gbDataSetAttr.Controls.Add(this.lbValidRegion);
            this.gbDataSetAttr.Controls.Add(this.cbBandName);
            this.gbDataSetAttr.Controls.Add(this.label7);
            this.gbDataSetAttr.Controls.Add(this.cbValidRegion);
            this.gbDataSetAttr.Controls.Add(this.cbOffsetValue);
            this.gbDataSetAttr.Controls.Add(this.cbScaleValue);
            this.gbDataSetAttr.Enabled = false;
            this.gbDataSetAttr.Location = new System.Drawing.Point(420, 375);
            this.gbDataSetAttr.Name = "gbDataSetAttr";
            this.gbDataSetAttr.Size = new System.Drawing.Size(365, 163);
            this.gbDataSetAttr.TabIndex = 1;
            this.gbDataSetAttr.TabStop = false;
            this.gbDataSetAttr.Text = "数据集属性";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(244, 134);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(121, 21);
            this.txtWidth.TabIndex = 3;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(64, 134);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(121, 21);
            this.txtHeight.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(191, 140);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "数据列数";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 137);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "数据行数";
            // 
            // lbBandName
            // 
            this.lbBandName.AutoSize = true;
            this.lbBandName.Location = new System.Drawing.Point(191, 27);
            this.lbBandName.Name = "lbBandName";
            this.lbBandName.Size = new System.Drawing.Size(0, 12);
            this.lbBandName.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "通道细节";
            // 
            // lbOffsetValue
            // 
            this.lbOffsetValue.AutoSize = true;
            this.lbOffsetValue.Location = new System.Drawing.Point(191, 108);
            this.lbOffsetValue.Name = "lbOffsetValue";
            this.lbOffsetValue.Size = new System.Drawing.Size(0, 12);
            this.lbOffsetValue.TabIndex = 2;
            // 
            // lbScaleValue
            // 
            this.lbScaleValue.AutoSize = true;
            this.lbScaleValue.Location = new System.Drawing.Point(191, 79);
            this.lbScaleValue.Name = "lbScaleValue";
            this.lbScaleValue.Size = new System.Drawing.Size(0, 12);
            this.lbScaleValue.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 2;
            this.label11.Text = "计算截距";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "计算斜率";
            // 
            // lbValidRegion
            // 
            this.lbValidRegion.AutoSize = true;
            this.lbValidRegion.Location = new System.Drawing.Point(191, 53);
            this.lbValidRegion.Name = "lbValidRegion";
            this.lbValidRegion.Size = new System.Drawing.Size(0, 12);
            this.lbValidRegion.TabIndex = 2;
            // 
            // cbBandName
            // 
            this.cbBandName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBandName.FormattingEnabled = true;
            this.cbBandName.Location = new System.Drawing.Point(64, 24);
            this.cbBandName.Name = "cbBandName";
            this.cbBandName.Size = new System.Drawing.Size(121, 20);
            this.cbBandName.TabIndex = 0;
            this.cbBandName.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "值域范围";
            // 
            // cbValidRegion
            // 
            this.cbValidRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbValidRegion.FormattingEnabled = true;
            this.cbValidRegion.Location = new System.Drawing.Point(64, 50);
            this.cbValidRegion.Name = "cbValidRegion";
            this.cbValidRegion.Size = new System.Drawing.Size(121, 20);
            this.cbValidRegion.TabIndex = 0;
            this.cbValidRegion.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cbOffsetValue
            // 
            this.cbOffsetValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOffsetValue.FormattingEnabled = true;
            this.cbOffsetValue.Location = new System.Drawing.Point(64, 105);
            this.cbOffsetValue.Name = "cbOffsetValue";
            this.cbOffsetValue.Size = new System.Drawing.Size(121, 20);
            this.cbOffsetValue.TabIndex = 0;
            this.cbOffsetValue.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // cbScaleValue
            // 
            this.cbScaleValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScaleValue.FormattingEnabled = true;
            this.cbScaleValue.Location = new System.Drawing.Point(64, 76);
            this.cbScaleValue.Name = "cbScaleValue";
            this.cbScaleValue.Size = new System.Drawing.Size(121, 20);
            this.cbScaleValue.TabIndex = 0;
            this.cbScaleValue.SelectedIndexChanged += new System.EventHandler(this.cb_SelectedIndexChanged);
            // 
            // lstDataSets
            // 
            this.lstDataSets.CheckBoxes = true;
            this.lstDataSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dataset,
            this.desc});
            this.lstDataSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDataSets.FullRowSelect = true;
            this.lstDataSets.HideSelection = false;
            this.lstDataSets.Location = new System.Drawing.Point(3, 17);
            this.lstDataSets.MultiSelect = false;
            this.lstDataSets.Name = "lstDataSets";
            this.lstDataSets.Size = new System.Drawing.Size(359, 198);
            this.lstDataSets.TabIndex = 0;
            this.lstDataSets.UseCompatibleStateImageBehavior = false;
            this.lstDataSets.View = System.Windows.Forms.View.Details;
            this.lstDataSets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstDataSets_ItemChecked);
            this.lstDataSets.SelectedIndexChanged += new System.EventHandler(this.lstDataSets_SelectedIndexChanged);
            // 
            // dataset
            // 
            this.dataset.Text = "数据集";
            this.dataset.Width = 111;
            // 
            // desc
            // 
            this.desc.Text = "描述";
            this.desc.Width = 229;
            // 
            // gbDataSets
            // 
            this.gbDataSets.Controls.Add(this.lstDataSets);
            this.gbDataSets.Enabled = false;
            this.gbDataSets.Location = new System.Drawing.Point(420, 153);
            this.gbDataSets.Name = "gbDataSets";
            this.gbDataSets.Size = new System.Drawing.Size(365, 218);
            this.gbDataSets.TabIndex = 1;
            this.gbDataSets.TabStop = false;
            this.gbDataSets.Text = "镶嵌数据集";
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(631, 581);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 3;
            this.btOK.Text = "镶嵌";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ckAutoSX
            // 
            this.ckAutoSX.AutoSize = true;
            this.ckAutoSX.Location = new System.Drawing.Point(436, 607);
            this.ckAutoSX.Name = "ckAutoSX";
            this.ckAutoSX.Size = new System.Drawing.Size(72, 16);
            this.ckAutoSX.TabIndex = 4;
            this.ckAutoSX.Text = "通道顺序";
            this.ckAutoSX.UseVisualStyleBackColor = true;
            this.ckAutoSX.Visible = false;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(710, 581);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // ckTimeMosaic
            // 
            this.ckTimeMosaic.AutoSize = true;
            this.ckTimeMosaic.Checked = true;
            this.ckTimeMosaic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckTimeMosaic.Location = new System.Drawing.Point(427, 585);
            this.ckTimeMosaic.Name = "ckTimeMosaic";
            this.ckTimeMosaic.Size = new System.Drawing.Size(60, 16);
            this.ckTimeMosaic.TabIndex = 4;
            this.ckTimeMosaic.Text = "按时间";
            this.ckTimeMosaic.UseVisualStyleBackColor = true;
            // 
            // btChooseDir
            // 
            this.btChooseDir.Location = new System.Drawing.Point(328, 38);
            this.btChooseDir.Name = "btChooseDir";
            this.btChooseDir.Size = new System.Drawing.Size(75, 23);
            this.btChooseDir.TabIndex = 5;
            this.btChooseDir.Text = "选择";
            this.btChooseDir.UseVisualStyleBackColor = true;
            this.btChooseDir.Click += new System.EventHandler(this.btChooseDir_Click);
            // 
            // txtFileDir
            // 
            this.txtFileDir.Location = new System.Drawing.Point(12, 40);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.Size = new System.Drawing.Size(310, 21);
            this.txtFileDir.TabIndex = 6;
            this.txtFileDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFileDir_KeyDown);
            // 
            // btClear
            // 
            this.btClear.Location = new System.Drawing.Point(328, 581);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(75, 23);
            this.btClear.TabIndex = 9;
            this.btClear.Text = "清空";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btDelFiles
            // 
            this.btDelFiles.Location = new System.Drawing.Point(242, 581);
            this.btDelFiles.Name = "btDelFiles";
            this.btDelFiles.Size = new System.Drawing.Size(75, 23);
            this.btDelFiles.TabIndex = 8;
            this.btDelFiles.Text = "删除";
            this.btDelFiles.UseVisualStyleBackColor = true;
            this.btDelFiles.Click += new System.EventHandler(this.btDelFiles_Click);
            // 
            // btAddFiles
            // 
            this.btAddFiles.Location = new System.Drawing.Point(156, 581);
            this.btAddFiles.Name = "btAddFiles";
            this.btAddFiles.Size = new System.Drawing.Size(75, 23);
            this.btAddFiles.TabIndex = 7;
            this.btAddFiles.Text = "添加";
            this.btAddFiles.UseVisualStyleBackColor = true;
            this.btAddFiles.Click += new System.EventHandler(this.btAddFiles_Click);
            // 
            // dpBegin
            // 
            this.dpBegin.Enabled = false;
            this.dpBegin.Location = new System.Drawing.Point(79, 9);
            this.dpBegin.Name = "dpBegin";
            this.dpBegin.Size = new System.Drawing.Size(144, 21);
            this.dpBegin.TabIndex = 10;
            // 
            // dpEnd
            // 
            this.dpEnd.Enabled = false;
            this.dpEnd.Location = new System.Drawing.Point(260, 11);
            this.dpEnd.Name = "dpEnd";
            this.dpEnd.Size = new System.Drawing.Size(143, 21);
            this.dpEnd.TabIndex = 10;
            // 
            // ckTimeRegion
            // 
            this.ckTimeRegion.AutoSize = true;
            this.ckTimeRegion.Location = new System.Drawing.Point(13, 14);
            this.ckTimeRegion.Name = "ckTimeRegion";
            this.ckTimeRegion.Size = new System.Drawing.Size(60, 16);
            this.ckTimeRegion.TabIndex = 11;
            this.ckTimeRegion.Text = "时间段";
            this.ckTimeRegion.UseVisualStyleBackColor = true;
            this.ckTimeRegion.CheckedChanged += new System.EventHandler(this.ckTimeRegion_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "~";
            // 
            // btnChooseOutDir
            // 
            this.btnChooseOutDir.Location = new System.Drawing.Point(710, 548);
            this.btnChooseOutDir.Name = "btnChooseOutDir";
            this.btnChooseOutDir.Size = new System.Drawing.Size(75, 23);
            this.btnChooseOutDir.TabIndex = 5;
            this.btnChooseOutDir.Text = "选择";
            this.btnChooseOutDir.UseVisualStyleBackColor = true;
            this.btnChooseOutDir.Click += new System.EventHandler(this.btnChooseOutDir_Click);
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(485, 550);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(219, 21);
            this.txtOutDir.TabIndex = 6;
            this.txtOutDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFileDir_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(425, 553);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "输出路径";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 588);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 13;
            this.label13.Text = "共";
            // 
            // lbFileCount
            // 
            this.lbFileCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileCount.AutoSize = true;
            this.lbFileCount.ForeColor = System.Drawing.Color.Red;
            this.lbFileCount.Location = new System.Drawing.Point(36, 589);
            this.lbFileCount.Name = "lbFileCount";
            this.lbFileCount.Size = new System.Drawing.Size(11, 12);
            this.lbFileCount.TabIndex = 13;
            this.lbFileCount.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(79, 589);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 13;
            this.label15.Text = "个文件";
            // 
            // ckGCTPChina
            // 
            this.ckGCTPChina.AutoSize = true;
            this.ckGCTPChina.Checked = true;
            this.ckGCTPChina.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckGCTPChina.Location = new System.Drawing.Point(485, 585);
            this.ckGCTPChina.Name = "ckGCTPChina";
            this.ckGCTPChina.Size = new System.Drawing.Size(72, 16);
            this.ckGCTPChina.TabIndex = 4;
            this.ckGCTPChina.Text = "正弦中国";
            this.ckGCTPChina.UseVisualStyleBackColor = true;
            // 
            // ckGLL
            // 
            this.ckGLL.AutoSize = true;
            this.ckGLL.Checked = true;
            this.ckGLL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckGLL.Location = new System.Drawing.Point(555, 585);
            this.ckGLL.Name = "ckGLL";
            this.ckGLL.Size = new System.Drawing.Size(72, 16);
            this.ckGLL.TabIndex = 4;
            this.ckGLL.Text = "等经纬度";
            this.ckGLL.UseVisualStyleBackColor = true;
            // 
            // frmMODLSTPro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 614);
            this.Controls.Add(this.ckGLL);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lbFileCount);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.gbFileAttr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckTimeRegion);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.dpEnd);
            this.Controls.Add(this.dpBegin);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.btDelFiles);
            this.Controls.Add(this.btAddFiles);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.btnChooseOutDir);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.btChooseDir);
            this.Controls.Add(this.ckGCTPChina);
            this.Controls.Add(this.ckTimeMosaic);
            this.Controls.Add(this.ckAutoSX);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.gbDataSetAttr);
            this.Controls.Add(this.gbDataSets);
            this.Controls.Add(this.lstFileInfoList);
            this.Name = "frmMODLSTPro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据镶嵌";
            this.gbFileAttr.ResumeLayout(false);
            this.gbFileAttr.PerformLayout();
            this.gbDataSetAttr.ResumeLayout(false);
            this.gbDataSetAttr.PerformLayout();
            this.gbDataSets.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstFileInfoList;
        private System.Windows.Forms.GroupBox gbFileAttr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbProjectAgrs;
        private System.Windows.Forms.ComboBox cbProject;
        private System.Windows.Forms.GroupBox gbDataSetAttr;
        private System.Windows.Forms.ListView lstDataSets;
        private System.Windows.Forms.GroupBox gbDataSets;
        private System.Windows.Forms.ColumnHeader Filename;
        private System.Windows.Forms.ColumnHeader Time;
        private System.Windows.Forms.Label lbRightDownPoint;
        private System.Windows.Forms.Label lbLeftUpPoint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbProjectAgrs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbProject;
        private System.Windows.Forms.ComboBox cbRightDownPoint;
        private System.Windows.Forms.ComboBox cbLeftUpPoint;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbBandName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbOffsetValue;
        private System.Windows.Forms.Label lbScaleValue;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbValidRegion;
        private System.Windows.Forms.ComboBox cbBandName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbValidRegion;
        private System.Windows.Forms.ComboBox cbOffsetValue;
        private System.Windows.Forms.ComboBox cbScaleValue;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.CheckBox ckAutoSX;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.CheckBox ckTimeMosaic;
        private System.Windows.Forms.Button btChooseDir;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Button btDelFiles;
        private System.Windows.Forms.Button btAddFiles;
        private System.Windows.Forms.DateTimePicker dpBegin;
        private System.Windows.Forms.DateTimePicker dpEnd;
        private System.Windows.Forms.CheckBox ckTimeRegion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader dataset;
        private System.Windows.Forms.ColumnHeader desc;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Button btnChooseOutDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lbFileCount;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox ckGCTPChina;
        private System.Windows.Forms.CheckBox ckGLL;
    }
}