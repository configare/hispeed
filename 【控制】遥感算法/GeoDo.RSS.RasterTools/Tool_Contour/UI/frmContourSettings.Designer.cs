namespace GeoDo.RSS.RasterTools
{
    partial class frmContourSettings
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdFullImage = new System.Windows.Forms.RadioButton();
            this.rdAOI = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvValues = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnStat = new System.Windows.Forms.Button();
            this.btnAddValue = new System.Windows.Forms.Button();
            this.txtSpan = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.lbSpan = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.lbMinValue = new System.Windows.Forms.Label();
            this.rdSingleValue = new System.Windows.Forms.RadioButton();
            this.rdRanges = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.txtSaveAs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ckNeedFillColor = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.ckIsNeedDisplay = new System.Windows.Forms.CheckBox();
            this.ckNeedLabel = new System.Windows.Forms.CheckBox();
            this.ckNeedOutputShp = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtBandNos = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lbRasterSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSamples = new System.Windows.Forms.ComboBox();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdFullImage);
            this.groupBox4.Controls.Add(this.rdAOI);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(14, 69);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(197, 54);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "像元范围";
            // 
            // rdFullImage
            // 
            this.rdFullImage.AutoSize = true;
            this.rdFullImage.Location = new System.Drawing.Point(137, 22);
            this.rdFullImage.Name = "rdFullImage";
            this.rdFullImage.Size = new System.Drawing.Size(50, 21);
            this.rdFullImage.TabIndex = 1;
            this.rdFullImage.TabStop = true;
            this.rdFullImage.Text = "全图";
            this.rdFullImage.UseVisualStyleBackColor = true;
            this.rdFullImage.CheckedChanged += new System.EventHandler(this.rdFullImage_CheckedChanged);
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
            this.rdAOI.CheckedChanged += new System.EventHandler(this.rdAOI_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvValues);
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnRemoveAll);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.rdSingleValue);
            this.groupBox1.Controls.Add(this.rdRanges);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(13, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(521, 206);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "等值";
            // 
            // lvValues
            // 
            this.lvValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvValues.FullRowSelect = true;
            this.lvValues.Location = new System.Drawing.Point(10, 51);
            this.lvValues.Name = "lvValues";
            this.lvValues.Size = new System.Drawing.Size(309, 120);
            this.lvValues.TabIndex = 6;
            this.lvValues.UseCompatibleStateImageBehavior = false;
            this.lvValues.View = System.Windows.Forms.View.Details;
            this.lvValues.SelectedIndexChanged += new System.EventHandler(this.lvValues_SelectedIndexChanged);
            this.lvValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvValues_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "等值";
            this.columnHeader1.Width = 107;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "绘制颜色";
            this.columnHeader2.Width = 112;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(89, 177);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.Text = "移除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(8, 177);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAll.TabIndex = 4;
            this.btnRemoveAll.Text = "移除所有";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnStat);
            this.panel1.Controls.Add(this.btnAddValue);
            this.panel1.Controls.Add(this.txtSpan);
            this.panel1.Controls.Add(this.txtMaxValue);
            this.panel1.Controls.Add(this.txtMinValue);
            this.panel1.Controls.Add(this.lbSpan);
            this.panel1.Controls.Add(this.lbMaxValue);
            this.panel1.Controls.Add(this.lbMinValue);
            this.panel1.Location = new System.Drawing.Point(329, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(186, 140);
            this.panel1.TabIndex = 3;
            // 
            // btnStat
            // 
            this.btnStat.Location = new System.Drawing.Point(7, 99);
            this.btnStat.Name = "btnStat";
            this.btnStat.Size = new System.Drawing.Size(110, 24);
            this.btnStat.TabIndex = 14;
            this.btnStat.Text = "统计";
            this.btnStat.UseVisualStyleBackColor = true;
            this.btnStat.Click += new System.EventHandler(this.btnStat_Click);
            // 
            // btnAddValue
            // 
            this.btnAddValue.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAddValue.Location = new System.Drawing.Point(123, 98);
            this.btnAddValue.Name = "btnAddValue";
            this.btnAddValue.Size = new System.Drawing.Size(59, 34);
            this.btnAddValue.TabIndex = 13;
            this.btnAddValue.Text = "增加(+)";
            this.btnAddValue.UseVisualStyleBackColor = true;
            this.btnAddValue.Click += new System.EventHandler(this.btnAddValue_Click);
            // 
            // txtSpan
            // 
            this.txtSpan.Location = new System.Drawing.Point(64, 67);
            this.txtSpan.Name = "txtSpan";
            this.txtSpan.Size = new System.Drawing.Size(114, 23);
            this.txtSpan.TabIndex = 5;
            this.txtSpan.Text = "50";
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(64, 38);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(115, 23);
            this.txtMaxValue.TabIndex = 4;
            this.txtMaxValue.Text = "255";
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(64, 9);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(115, 23);
            this.txtMinValue.TabIndex = 3;
            this.txtMinValue.Text = "0";
            this.txtMinValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMinValue_KeyDown);
            this.txtMinValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinValue_KeyPress);
            // 
            // lbSpan
            // 
            this.lbSpan.AutoSize = true;
            this.lbSpan.Location = new System.Drawing.Point(14, 73);
            this.lbSpan.Name = "lbSpan";
            this.lbSpan.Size = new System.Drawing.Size(44, 17);
            this.lbSpan.TabIndex = 2;
            this.lbSpan.Text = "间隔值";
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.AutoSize = true;
            this.lbMaxValue.Location = new System.Drawing.Point(14, 44);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(44, 17);
            this.lbMaxValue.TabIndex = 1;
            this.lbMaxValue.Text = "最大值";
            // 
            // lbMinValue
            // 
            this.lbMinValue.AutoSize = true;
            this.lbMinValue.Location = new System.Drawing.Point(13, 14);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(44, 17);
            this.lbMinValue.TabIndex = 0;
            this.lbMinValue.Text = "最小值";
            // 
            // rdSingleValue
            // 
            this.rdSingleValue.AutoSize = true;
            this.rdSingleValue.Location = new System.Drawing.Point(224, 23);
            this.rdSingleValue.Name = "rdSingleValue";
            this.rdSingleValue.Size = new System.Drawing.Size(74, 21);
            this.rdSingleValue.TabIndex = 1;
            this.rdSingleValue.Text = "单值输入";
            this.rdSingleValue.UseVisualStyleBackColor = true;
            this.rdSingleValue.CheckedChanged += new System.EventHandler(this.rdSingleValue_CheckedChanged);
            // 
            // rdRanges
            // 
            this.rdRanges.AutoSize = true;
            this.rdRanges.Checked = true;
            this.rdRanges.Location = new System.Drawing.Point(14, 23);
            this.rdRanges.Name = "rdRanges";
            this.rdRanges.Size = new System.Drawing.Size(86, 21);
            this.rdRanges.TabIndex = 0;
            this.rdRanges.TabStop = true;
            this.rdRanges.Text = "按范围分割";
            this.rdRanges.UseVisualStyleBackColor = true;
            this.rdRanges.CheckedChanged += new System.EventHandler(this.rdRanges_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(540, 117);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(540, 77);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 34);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSaveAs);
            this.groupBox2.Controls.Add(this.txtSaveAs);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(13, 341);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(521, 54);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "输出为矢量";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSaveAs.Location = new System.Drawing.Point(471, 22);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(40, 23);
            this.btnSaveAs.TabIndex = 8;
            this.btnSaveAs.Text = "...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // txtSaveAs
            // 
            this.txtSaveAs.Enabled = false;
            this.txtSaveAs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSaveAs.Location = new System.Drawing.Point(57, 22);
            this.txtSaveAs.Name = "txtSaveAs";
            this.txtSaveAs.Size = new System.Drawing.Size(408, 23);
            this.txtSaveAs.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(9, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "文件名";
            // 
            // ckNeedFillColor
            // 
            this.ckNeedFillColor.AutoSize = true;
            this.ckNeedFillColor.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedFillColor.Location = new System.Drawing.Point(540, 234);
            this.ckNeedFillColor.Name = "ckNeedFillColor";
            this.ckNeedFillColor.Size = new System.Drawing.Size(75, 21);
            this.ckNeedFillColor.TabIndex = 13;
            this.ckNeedFillColor.Text = "是否填色";
            this.ckNeedFillColor.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnFile);
            this.groupBox3.Controls.Add(this.txtFileName);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(13, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(616, 60);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "文件";
            // 
            // btnFile
            // 
            this.btnFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFile.Location = new System.Drawing.Point(558, 24);
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
            this.txtFileName.Size = new System.Drawing.Size(540, 23);
            this.txtFileName.TabIndex = 7;
            // 
            // ckIsNeedDisplay
            // 
            this.ckIsNeedDisplay.AutoSize = true;
            this.ckIsNeedDisplay.Checked = true;
            this.ckIsNeedDisplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsNeedDisplay.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckIsNeedDisplay.Location = new System.Drawing.Point(540, 207);
            this.ckIsNeedDisplay.Name = "ckIsNeedDisplay";
            this.ckIsNeedDisplay.Size = new System.Drawing.Size(75, 21);
            this.ckIsNeedDisplay.TabIndex = 15;
            this.ckIsNeedDisplay.Text = "是否显示";
            this.ckIsNeedDisplay.UseVisualStyleBackColor = true;
            // 
            // ckNeedLabel
            // 
            this.ckNeedLabel.AutoSize = true;
            this.ckNeedLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedLabel.Location = new System.Drawing.Point(540, 263);
            this.ckNeedLabel.Name = "ckNeedLabel";
            this.ckNeedLabel.Size = new System.Drawing.Size(75, 21);
            this.ckNeedLabel.TabIndex = 16;
            this.ckNeedLabel.Text = "是否标注";
            this.ckNeedLabel.UseVisualStyleBackColor = true;
            // 
            // ckNeedOutputShp
            // 
            this.ckNeedOutputShp.AutoSize = true;
            this.ckNeedOutputShp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedOutputShp.Location = new System.Drawing.Point(539, 294);
            this.ckNeedOutputShp.Name = "ckNeedOutputShp";
            this.ckNeedOutputShp.Size = new System.Drawing.Size(99, 21);
            this.ckNeedOutputShp.TabIndex = 17;
            this.ckNeedOutputShp.Text = "是否输出矢量";
            this.ckNeedOutputShp.UseVisualStyleBackColor = true;
            this.ckNeedOutputShp.CheckedChanged += new System.EventHandler(this.ckNeedOutputShp_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtBandNos);
            this.groupBox5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(216, 71);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(85, 54);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "波段";
            // 
            // txtBandNos
            // 
            this.txtBandNos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtBandNos.FormattingEnabled = true;
            this.txtBandNos.Location = new System.Drawing.Point(16, 21);
            this.txtBandNos.Name = "txtBandNos";
            this.txtBandNos.Size = new System.Drawing.Size(59, 25);
            this.txtBandNos.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lbRasterSize);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.txtSamples);
            this.groupBox6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.Location = new System.Drawing.Point(307, 71);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(227, 54);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "采样";
            // 
            // lbRasterSize
            // 
            this.lbRasterSize.AutoSize = true;
            this.lbRasterSize.ForeColor = System.Drawing.Color.Blue;
            this.lbRasterSize.Location = new System.Drawing.Point(110, 28);
            this.lbRasterSize.Name = "lbRasterSize";
            this.lbRasterSize.Size = new System.Drawing.Size(0, 17);
            this.lbRasterSize.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "采样比";
            // 
            // txtSamples
            // 
            this.txtSamples.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtSamples.FormattingEnabled = true;
            this.txtSamples.Items.AddRange(new object[] {
            "1"});
            this.txtSamples.Location = new System.Drawing.Point(55, 21);
            this.txtSamples.Name = "txtSamples";
            this.txtSamples.Size = new System.Drawing.Size(50, 25);
            this.txtSamples.TabIndex = 0;
            this.txtSamples.SelectedIndexChanged += new System.EventHandler(this.txtSamples_SelectedIndexChanged);
            // 
            // frmContourSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 338);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.ckNeedOutputShp);
            this.Controls.Add(this.ckNeedLabel);
            this.Controls.Add(this.ckIsNeedDisplay);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ckNeedFillColor);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmContourSettings";
            this.Text = "等值线参数设置...";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdFullImage;
        private System.Windows.Forms.RadioButton rdAOI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddValue;
        private System.Windows.Forms.TextBox txtSpan;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.Label lbSpan;
        private System.Windows.Forms.Label lbMaxValue;
        private System.Windows.Forms.Label lbMinValue;
        private System.Windows.Forms.RadioButton rdSingleValue;
        private System.Windows.Forms.RadioButton rdRanges;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.TextBox txtSaveAs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckNeedFillColor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.CheckBox ckIsNeedDisplay;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnStat;
        private System.Windows.Forms.ListView lvValues;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox ckNeedLabel;
        private System.Windows.Forms.CheckBox ckNeedOutputShp;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox txtBandNos;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox txtSamples;
        private System.Windows.Forms.Label lbRasterSize;
    }
}