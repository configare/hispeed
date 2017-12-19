namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class CLDDataRetrieval
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelGridView = new System.Windows.Forms.Panel();
            this.dataGridViewPeriod = new System.Windows.Forms.DataGridView();
            this.yearPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonthPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegionPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resolutionPeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imageNamePeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statTypePeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDay = new System.Windows.Forms.DataGridView();
            this.observeDateDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegionDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resolutionDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.validpercentDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GranuleCountsDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GranuleTimesDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imageNameDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelRetriOptions = new System.Windows.Forms.Panel();
            this.groupBoxDayNight = new System.Windows.Forms.GroupBox();
            this.radioButtonNight = new System.Windows.Forms.RadioButton();
            this.radioButtonDay = new System.Windows.Forms.RadioButton();
            this.groupBoxDataSet = new System.Windows.Forms.GroupBox();
            this.ucCheckBoxListDataSet = new GeoDo.RSS.MIF.Prds.CLD.ucCheckBoxList();
            this.textBoxQueryConditions = new System.Windows.Forms.TextBox();
            this.groupBoxPrdsTime = new System.Windows.Forms.GroupBox();
            this.panelPrdsTimeTen = new System.Windows.Forms.Panel();
            this.checkBoxTen3 = new System.Windows.Forms.CheckBox();
            this.checkBoxTen2 = new System.Windows.Forms.CheckBox();
            this.checkBoxTen1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelPrdsTimeMonth = new System.Windows.Forms.Panel();
            this.ucMonths = new GeoDo.RSS.MIF.Prds.CLD.ucMonths();
            this.label1 = new System.Windows.Forms.Label();
            this.labelEnd = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.labelBegin = new System.Windows.Forms.Label();
            this.btnAddQueryCondtions = new System.Windows.Forms.Button();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerBegin = new System.Windows.Forms.DateTimePicker();
            this.radibtnSameTime = new System.Windows.Forms.RadioButton();
            this.radibtnContinueTime = new System.Windows.Forms.RadioButton();
            this.groupBoxPrdsPeriod = new System.Windows.Forms.GroupBox();
            this.radibtnPrdYear = new System.Windows.Forms.RadioButton();
            this.radibtnPrdMonth = new System.Windows.Forms.RadioButton();
            this.radibtnPrdTen = new System.Windows.Forms.RadioButton();
            this.radibtnPrdDay = new System.Windows.Forms.RadioButton();
            this.groupBoxProducts = new System.Windows.Forms.GroupBox();
            this.ucRadioBoxListProducts = new GeoDo.RSS.MIF.Prds.CLD.ucRadioBoxList();
            this.groupBoxDataSource = new System.Windows.Forms.GroupBox();
            this.radibtnISCCPD2 = new System.Windows.Forms.RadioButton();
            this.radibtnAIRS = new System.Windows.Forms.RadioButton();
            this.radibtnMOD06 = new System.Windows.Forms.RadioButton();
            this.panelDown = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.panelGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDay)).BeginInit();
            this.panelRetriOptions.SuspendLayout();
            this.groupBoxDayNight.SuspendLayout();
            this.groupBoxDataSet.SuspendLayout();
            this.groupBoxPrdsTime.SuspendLayout();
            this.panelPrdsTimeTen.SuspendLayout();
            this.panelPrdsTimeMonth.SuspendLayout();
            this.groupBoxPrdsPeriod.SuspendLayout();
            this.groupBoxProducts.SuspendLayout();
            this.groupBoxDataSource.SuspendLayout();
            this.panelDown.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelGridView);
            this.panelMain.Controls.Add(this.splitter1);
            this.panelMain.Controls.Add(this.panelRetriOptions);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1262, 578);
            this.panelMain.TabIndex = 2;
            // 
            // panelGridView
            // 
            this.panelGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGridView.Controls.Add(this.dataGridViewPeriod);
            this.panelGridView.Controls.Add(this.dataGridViewDay);
            this.panelGridView.Location = new System.Drawing.Point(0, 464);
            this.panelGridView.Name = "panelGridView";
            this.panelGridView.Size = new System.Drawing.Size(1262, 79);
            this.panelGridView.TabIndex = 1;
            // 
            // dataGridViewPeriod
            // 
            this.dataGridViewPeriod.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewPeriod.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPeriod.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yearPeriod,
            this.MonthPeriod,
            this.tenPeriod,
            this.RegionPeriod,
            this.resolutionPeriod,
            this.imageNamePeriod,
            this.statTypePeriod});
            this.dataGridViewPeriod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPeriod.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPeriod.Name = "dataGridViewPeriod";
            this.dataGridViewPeriod.RowTemplate.Height = 23;
            this.dataGridViewPeriod.Size = new System.Drawing.Size(1262, 79);
            this.dataGridViewPeriod.TabIndex = 20;
            this.dataGridViewPeriod.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPeriod_CellContentDoubleClick);
            this.dataGridViewPeriod.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPeriod_RowHeaderMouseDoubleClick);
            // 
            // yearPeriod
            // 
            this.yearPeriod.HeaderText = "年";
            this.yearPeriod.Name = "yearPeriod";
            this.yearPeriod.Width = 60;
            // 
            // MonthPeriod
            // 
            this.MonthPeriod.HeaderText = "月";
            this.MonthPeriod.Name = "MonthPeriod";
            this.MonthPeriod.Width = 60;
            // 
            // tenPeriod
            // 
            this.tenPeriod.HeaderText = "旬";
            this.tenPeriod.Name = "tenPeriod";
            this.tenPeriod.Width = 60;
            // 
            // RegionPeriod
            // 
            this.RegionPeriod.HeaderText = "区域";
            this.RegionPeriod.Name = "RegionPeriod";
            // 
            // resolutionPeriod
            // 
            this.resolutionPeriod.HeaderText = "分辨率";
            this.resolutionPeriod.Name = "resolutionPeriod";
            // 
            // imageNamePeriod
            // 
            this.imageNamePeriod.HeaderText = "文件路径";
            this.imageNamePeriod.Name = "imageNamePeriod";
            this.imageNamePeriod.Width = 400;
            // 
            // statTypePeriod
            // 
            this.statTypePeriod.HeaderText = "统计类型";
            this.statTypePeriod.Name = "statTypePeriod";
            // 
            // dataGridViewDay
            // 
            this.dataGridViewDay.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewDay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.observeDateDay,
            this.RegionDay,
            this.resolutionDay,
            this.validpercentDay,
            this.GranuleCountsDay,
            this.GranuleTimesDay,
            this.imageNameDay});
            this.dataGridViewDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDay.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewDay.Name = "dataGridViewDay";
            this.dataGridViewDay.RowTemplate.Height = 23;
            this.dataGridViewDay.Size = new System.Drawing.Size(1262, 79);
            this.dataGridViewDay.TabIndex = 13;
            this.dataGridViewDay.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDay_CellContentDoubleClick);
            this.dataGridViewDay.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewDay_RowHeaderMouseDoubleClick);
            // 
            // observeDateDay
            // 
            this.observeDateDay.HeaderText = "观测日期";
            this.observeDateDay.Name = "observeDateDay";
            this.observeDateDay.Width = 140;
            // 
            // RegionDay
            // 
            this.RegionDay.HeaderText = "区域";
            this.RegionDay.Name = "RegionDay";
            // 
            // resolutionDay
            // 
            this.resolutionDay.HeaderText = "分辨率";
            this.resolutionDay.Name = "resolutionDay";
            // 
            // validpercentDay
            // 
            this.validpercentDay.HeaderText = "数据有效性(%)";
            this.validpercentDay.Name = "validpercentDay";
            this.validpercentDay.Width = 85;
            // 
            // GranuleCountsDay
            // 
            this.GranuleCountsDay.HeaderText = "分钟段数量(个)";
            this.GranuleCountsDay.Name = "GranuleCountsDay";
            this.GranuleCountsDay.Width = 85;
            // 
            // GranuleTimesDay
            // 
            this.GranuleTimesDay.HeaderText = "分钟段观测时间";
            this.GranuleTimesDay.Name = "GranuleTimesDay";
            this.GranuleTimesDay.Width = 150;
            // 
            // imageNameDay
            // 
            this.imageNameDay.HeaderText = "文件路径";
            this.imageNameDay.Name = "imageNameDay";
            this.imageNameDay.Width = 350;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 458);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1262, 2);
            this.splitter1.TabIndex = 21;
            this.splitter1.TabStop = false;
            // 
            // panelRetriOptions
            // 
            this.panelRetriOptions.Controls.Add(this.groupBoxDayNight);
            this.panelRetriOptions.Controls.Add(this.groupBoxDataSet);
            this.panelRetriOptions.Controls.Add(this.textBoxQueryConditions);
            this.panelRetriOptions.Controls.Add(this.groupBoxPrdsTime);
            this.panelRetriOptions.Controls.Add(this.groupBoxPrdsPeriod);
            this.panelRetriOptions.Controls.Add(this.groupBoxProducts);
            this.panelRetriOptions.Controls.Add(this.groupBoxDataSource);
            this.panelRetriOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRetriOptions.Location = new System.Drawing.Point(0, 0);
            this.panelRetriOptions.Name = "panelRetriOptions";
            this.panelRetriOptions.Size = new System.Drawing.Size(1262, 458);
            this.panelRetriOptions.TabIndex = 0;
            // 
            // groupBoxDayNight
            // 
            this.groupBoxDayNight.Controls.Add(this.radioButtonNight);
            this.groupBoxDayNight.Controls.Add(this.radioButtonDay);
            this.groupBoxDayNight.Location = new System.Drawing.Point(512, 220);
            this.groupBoxDayNight.Name = "groupBoxDayNight";
            this.groupBoxDayNight.Size = new System.Drawing.Size(264, 45);
            this.groupBoxDayNight.TabIndex = 22;
            this.groupBoxDayNight.TabStop = false;
            this.groupBoxDayNight.Text = "昼夜";
            // 
            // radioButtonNight
            // 
            this.radioButtonNight.AutoSize = true;
            this.radioButtonNight.Location = new System.Drawing.Point(144, 20);
            this.radioButtonNight.Name = "radioButtonNight";
            this.radioButtonNight.Size = new System.Drawing.Size(47, 16);
            this.radioButtonNight.TabIndex = 1;
            this.radioButtonNight.Text = "晚上";
            this.radioButtonNight.UseVisualStyleBackColor = true;
            // 
            // radioButtonDay
            // 
            this.radioButtonDay.AutoSize = true;
            this.radioButtonDay.Checked = true;
            this.radioButtonDay.Location = new System.Drawing.Point(24, 20);
            this.radioButtonDay.Name = "radioButtonDay";
            this.radioButtonDay.Size = new System.Drawing.Size(47, 16);
            this.radioButtonDay.TabIndex = 0;
            this.radioButtonDay.TabStop = true;
            this.radioButtonDay.Text = "白天";
            this.radioButtonDay.UseVisualStyleBackColor = true;
            // 
            // groupBoxDataSet
            // 
            this.groupBoxDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDataSet.AutoSize = true;
            this.groupBoxDataSet.Controls.Add(this.ucCheckBoxListDataSet);
            this.groupBoxDataSet.Location = new System.Drawing.Point(8, 125);
            this.groupBoxDataSet.Name = "groupBoxDataSet";
            this.groupBoxDataSet.Size = new System.Drawing.Size(1251, 93);
            this.groupBoxDataSet.TabIndex = 21;
            this.groupBoxDataSet.TabStop = false;
            this.groupBoxDataSet.Text = "数据集";
            // 
            // ucCheckBoxListDataSet
            // 
            this.ucCheckBoxListDataSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCheckBoxListDataSet.Location = new System.Drawing.Point(3, 17);
            this.ucCheckBoxListDataSet.Margin = new System.Windows.Forms.Padding(4);
            this.ucCheckBoxListDataSet.Name = "ucCheckBoxListDataSet";
            this.ucCheckBoxListDataSet.Size = new System.Drawing.Size(1245, 73);
            this.ucCheckBoxListDataSet.TabIndex = 0;
            // 
            // textBoxQueryConditions
            // 
            this.textBoxQueryConditions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxQueryConditions.Location = new System.Drawing.Point(7, 418);
            this.textBoxQueryConditions.Multiline = true;
            this.textBoxQueryConditions.Name = "textBoxQueryConditions";
            this.textBoxQueryConditions.Size = new System.Drawing.Size(1251, 35);
            this.textBoxQueryConditions.TabIndex = 20;
            // 
            // groupBoxPrdsTime
            // 
            this.groupBoxPrdsTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPrdsTime.Controls.Add(this.panelPrdsTimeTen);
            this.groupBoxPrdsTime.Controls.Add(this.panelPrdsTimeMonth);
            this.groupBoxPrdsTime.Controls.Add(this.labelEnd);
            this.groupBoxPrdsTime.Controls.Add(this.btnQuery);
            this.groupBoxPrdsTime.Controls.Add(this.labelBegin);
            this.groupBoxPrdsTime.Controls.Add(this.btnAddQueryCondtions);
            this.groupBoxPrdsTime.Controls.Add(this.dateTimePickerEnd);
            this.groupBoxPrdsTime.Controls.Add(this.dateTimePickerBegin);
            this.groupBoxPrdsTime.Controls.Add(this.radibtnSameTime);
            this.groupBoxPrdsTime.Controls.Add(this.radibtnContinueTime);
            this.groupBoxPrdsTime.Location = new System.Drawing.Point(7, 271);
            this.groupBoxPrdsTime.Name = "groupBoxPrdsTime";
            this.groupBoxPrdsTime.Size = new System.Drawing.Size(1252, 144);
            this.groupBoxPrdsTime.TabIndex = 17;
            this.groupBoxPrdsTime.TabStop = false;
            this.groupBoxPrdsTime.Text = "产品时间";
            // 
            // panelPrdsTimeTen
            // 
            this.panelPrdsTimeTen.Controls.Add(this.checkBoxTen3);
            this.panelPrdsTimeTen.Controls.Add(this.checkBoxTen2);
            this.panelPrdsTimeTen.Controls.Add(this.checkBoxTen1);
            this.panelPrdsTimeTen.Controls.Add(this.label2);
            this.panelPrdsTimeTen.Location = new System.Drawing.Point(19, 105);
            this.panelPrdsTimeTen.Name = "panelPrdsTimeTen";
            this.panelPrdsTimeTen.Size = new System.Drawing.Size(348, 34);
            this.panelPrdsTimeTen.TabIndex = 54;
            this.panelPrdsTimeTen.Visible = false;
            // 
            // checkBoxTen3
            // 
            this.checkBoxTen3.AutoSize = true;
            this.checkBoxTen3.Location = new System.Drawing.Point(216, 10);
            this.checkBoxTen3.Name = "checkBoxTen3";
            this.checkBoxTen3.Size = new System.Drawing.Size(48, 16);
            this.checkBoxTen3.TabIndex = 56;
            this.checkBoxTen3.Text = "下旬";
            this.checkBoxTen3.UseVisualStyleBackColor = true;
            // 
            // checkBoxTen2
            // 
            this.checkBoxTen2.AutoSize = true;
            this.checkBoxTen2.Location = new System.Drawing.Point(142, 10);
            this.checkBoxTen2.Name = "checkBoxTen2";
            this.checkBoxTen2.Size = new System.Drawing.Size(48, 16);
            this.checkBoxTen2.TabIndex = 55;
            this.checkBoxTen2.Text = "中旬";
            this.checkBoxTen2.UseVisualStyleBackColor = true;
            // 
            // checkBoxTen1
            // 
            this.checkBoxTen1.AutoSize = true;
            this.checkBoxTen1.Location = new System.Drawing.Point(72, 10);
            this.checkBoxTen1.Name = "checkBoxTen1";
            this.checkBoxTen1.Size = new System.Drawing.Size(48, 16);
            this.checkBoxTen1.TabIndex = 54;
            this.checkBoxTen1.Text = "上旬";
            this.checkBoxTen1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 53;
            this.label2.Text = " 旬选择：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelPrdsTimeMonth
            // 
            this.panelPrdsTimeMonth.Controls.Add(this.ucMonths);
            this.panelPrdsTimeMonth.Controls.Add(this.label1);
            this.panelPrdsTimeMonth.Location = new System.Drawing.Point(19, 71);
            this.panelPrdsTimeMonth.Name = "panelPrdsTimeMonth";
            this.panelPrdsTimeMonth.Size = new System.Drawing.Size(831, 34);
            this.panelPrdsTimeMonth.TabIndex = 53;
            this.panelPrdsTimeMonth.Visible = false;
            // 
            // ucMonths
            // 
            this.ucMonths.Location = new System.Drawing.Point(72, 3);
            this.ucMonths.Margin = new System.Windows.Forms.Padding(4);
            this.ucMonths.Name = "ucMonths";
            this.ucMonths.Size = new System.Drawing.Size(678, 31);
            this.ucMonths.TabIndex = 51;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 50;
            this.label1.Text = "月份选择：";
            // 
            // labelEnd
            // 
            this.labelEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(247, 49);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(65, 12);
            this.labelEnd.TabIndex = 35;
            this.labelEnd.Text = "结束日期：";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Location = new System.Drawing.Point(1138, 116);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 19;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // labelBegin
            // 
            this.labelBegin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBegin.AutoSize = true;
            this.labelBegin.Location = new System.Drawing.Point(18, 49);
            this.labelBegin.Name = "labelBegin";
            this.labelBegin.Size = new System.Drawing.Size(65, 12);
            this.labelBegin.TabIndex = 34;
            this.labelBegin.Text = "开始日期：";
            // 
            // btnAddQueryCondtions
            // 
            this.btnAddQueryCondtions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddQueryCondtions.Location = new System.Drawing.Point(1028, 116);
            this.btnAddQueryCondtions.Name = "btnAddQueryCondtions";
            this.btnAddQueryCondtions.Size = new System.Drawing.Size(75, 23);
            this.btnAddQueryCondtions.TabIndex = 18;
            this.btnAddQueryCondtions.Text = "添加查询";
            this.btnAddQueryCondtions.UseVisualStyleBackColor = true;
            this.btnAddQueryCondtions.Visible = false;
            this.btnAddQueryCondtions.Click += new System.EventHandler(this.btnAddQueryCondtions_Click);
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(324, 44);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(106, 21);
            this.dateTimePickerEnd.TabIndex = 33;
            // 
            // dateTimePickerBegin
            // 
            this.dateTimePickerBegin.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerBegin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerBegin.Location = new System.Drawing.Point(83, 44);
            this.dateTimePickerBegin.Name = "dateTimePickerBegin";
            this.dateTimePickerBegin.Size = new System.Drawing.Size(106, 21);
            this.dateTimePickerBegin.TabIndex = 32;
            // 
            // radibtnSameTime
            // 
            this.radibtnSameTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radibtnSameTime.AutoSize = true;
            this.radibtnSameTime.Location = new System.Drawing.Point(130, 20);
            this.radibtnSameTime.Name = "radibtnSameTime";
            this.radibtnSameTime.Size = new System.Drawing.Size(71, 16);
            this.radibtnSameTime.TabIndex = 3;
            this.radibtnSameTime.Text = "同期查询";
            this.radibtnSameTime.UseVisualStyleBackColor = true;
            this.radibtnSameTime.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsTime_click);
            // 
            // radibtnContinueTime
            // 
            this.radibtnContinueTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radibtnContinueTime.AutoSize = true;
            this.radibtnContinueTime.Location = new System.Drawing.Point(23, 20);
            this.radibtnContinueTime.Name = "radibtnContinueTime";
            this.radibtnContinueTime.Size = new System.Drawing.Size(71, 16);
            this.radibtnContinueTime.TabIndex = 2;
            this.radibtnContinueTime.Text = "连续查询";
            this.radibtnContinueTime.UseVisualStyleBackColor = true;
            this.radibtnContinueTime.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsTime_click);
            // 
            // groupBoxPrdsPeriod
            // 
            this.groupBoxPrdsPeriod.Controls.Add(this.radibtnPrdYear);
            this.groupBoxPrdsPeriod.Controls.Add(this.radibtnPrdMonth);
            this.groupBoxPrdsPeriod.Controls.Add(this.radibtnPrdTen);
            this.groupBoxPrdsPeriod.Controls.Add(this.radibtnPrdDay);
            this.groupBoxPrdsPeriod.Location = new System.Drawing.Point(7, 220);
            this.groupBoxPrdsPeriod.Name = "groupBoxPrdsPeriod";
            this.groupBoxPrdsPeriod.Size = new System.Drawing.Size(451, 45);
            this.groupBoxPrdsPeriod.TabIndex = 16;
            this.groupBoxPrdsPeriod.TabStop = false;
            this.groupBoxPrdsPeriod.Text = "产品周期";
            // 
            // radibtnPrdYear
            // 
            this.radibtnPrdYear.AutoSize = true;
            this.radibtnPrdYear.Location = new System.Drawing.Point(249, 20);
            this.radibtnPrdYear.Name = "radibtnPrdYear";
            this.radibtnPrdYear.Size = new System.Drawing.Size(59, 16);
            this.radibtnPrdYear.TabIndex = 5;
            this.radibtnPrdYear.Text = "年产品";
            this.radibtnPrdYear.UseVisualStyleBackColor = true;
            this.radibtnPrdYear.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsPeriod_click);
            // 
            // radibtnPrdMonth
            // 
            this.radibtnPrdMonth.AutoSize = true;
            this.radibtnPrdMonth.Location = new System.Drawing.Point(142, 20);
            this.radibtnPrdMonth.Name = "radibtnPrdMonth";
            this.radibtnPrdMonth.Size = new System.Drawing.Size(59, 16);
            this.radibtnPrdMonth.TabIndex = 4;
            this.radibtnPrdMonth.Text = "月产品";
            this.radibtnPrdMonth.UseVisualStyleBackColor = true;
            this.radibtnPrdMonth.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsPeriod_click);
            // 
            // radibtnPrdTen
            // 
            this.radibtnPrdTen.AutoSize = true;
            this.radibtnPrdTen.Location = new System.Drawing.Point(371, 20);
            this.radibtnPrdTen.Name = "radibtnPrdTen";
            this.radibtnPrdTen.Size = new System.Drawing.Size(59, 16);
            this.radibtnPrdTen.TabIndex = 3;
            this.radibtnPrdTen.Text = "旬产品";
            this.radibtnPrdTen.UseVisualStyleBackColor = true;
            this.radibtnPrdTen.Visible = false;
            this.radibtnPrdTen.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsPeriod_click);
            // 
            // radibtnPrdDay
            // 
            this.radibtnPrdDay.AutoSize = true;
            this.radibtnPrdDay.Location = new System.Drawing.Point(23, 20);
            this.radibtnPrdDay.Name = "radibtnPrdDay";
            this.radibtnPrdDay.Size = new System.Drawing.Size(59, 16);
            this.radibtnPrdDay.TabIndex = 2;
            this.radibtnPrdDay.Text = "日产品";
            this.radibtnPrdDay.UseVisualStyleBackColor = true;
            this.radibtnPrdDay.CheckedChanged += new System.EventHandler(this.RadiBtnPrdsPeriod_click);
            // 
            // groupBoxProducts
            // 
            this.groupBoxProducts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProducts.Controls.Add(this.ucRadioBoxListProducts);
            this.groupBoxProducts.Location = new System.Drawing.Point(7, 54);
            this.groupBoxProducts.Name = "groupBoxProducts";
            this.groupBoxProducts.Size = new System.Drawing.Size(1252, 68);
            this.groupBoxProducts.TabIndex = 15;
            this.groupBoxProducts.TabStop = false;
            this.groupBoxProducts.Text = "产品类型";
            // 
            // ucRadioBoxListProducts
            // 
            this.ucRadioBoxListProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucRadioBoxListProducts.Location = new System.Drawing.Point(3, 17);
            this.ucRadioBoxListProducts.Margin = new System.Windows.Forms.Padding(4);
            this.ucRadioBoxListProducts.Name = "ucRadioBoxListProducts";
            this.ucRadioBoxListProducts.Size = new System.Drawing.Size(1246, 48);
            this.ucRadioBoxListProducts.TabIndex = 23;
            // 
            // groupBoxDataSource
            // 
            this.groupBoxDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDataSource.Controls.Add(this.radibtnISCCPD2);
            this.groupBoxDataSource.Controls.Add(this.radibtnAIRS);
            this.groupBoxDataSource.Controls.Add(this.radibtnMOD06);
            this.groupBoxDataSource.Location = new System.Drawing.Point(7, 6);
            this.groupBoxDataSource.Name = "groupBoxDataSource";
            this.groupBoxDataSource.Size = new System.Drawing.Size(1252, 45);
            this.groupBoxDataSource.TabIndex = 13;
            this.groupBoxDataSource.TabStop = false;
            this.groupBoxDataSource.Text = "产品数据源";
            // 
            // radibtnISCCPD2
            // 
            this.radibtnISCCPD2.AutoSize = true;
            this.radibtnISCCPD2.Location = new System.Drawing.Point(242, 20);
            this.radibtnISCCPD2.Name = "radibtnISCCPD2";
            this.radibtnISCCPD2.Size = new System.Drawing.Size(71, 16);
            this.radibtnISCCPD2.TabIndex = 4;
            this.radibtnISCCPD2.Text = "ISCCP D2";
            this.radibtnISCCPD2.UseVisualStyleBackColor = true;
            this.radibtnISCCPD2.CheckedChanged += new System.EventHandler(this.RadiBtnDataSource_click);
            // 
            // radibtnAIRS
            // 
            this.radibtnAIRS.AutoSize = true;
            this.radibtnAIRS.Location = new System.Drawing.Point(130, 20);
            this.radibtnAIRS.Name = "radibtnAIRS";
            this.radibtnAIRS.Size = new System.Drawing.Size(47, 16);
            this.radibtnAIRS.TabIndex = 3;
            this.radibtnAIRS.Text = "AIRS";
            this.radibtnAIRS.UseVisualStyleBackColor = true;
            this.radibtnAIRS.CheckedChanged += new System.EventHandler(this.RadiBtnDataSource_click);
            // 
            // radibtnMOD06
            // 
            this.radibtnMOD06.AutoSize = true;
            this.radibtnMOD06.Location = new System.Drawing.Point(23, 20);
            this.radibtnMOD06.Name = "radibtnMOD06";
            this.radibtnMOD06.Size = new System.Drawing.Size(53, 16);
            this.radibtnMOD06.TabIndex = 2;
            this.radibtnMOD06.Text = "MOD06";
            this.radibtnMOD06.UseVisualStyleBackColor = true;
            this.radibtnMOD06.CheckedChanged += new System.EventHandler(this.RadiBtnDataSource_click);
            // 
            // panelDown
            // 
            this.panelDown.Controls.Add(this.btnCancel);
            this.panelDown.Controls.Add(this.btnOK);
            this.panelDown.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDown.Location = new System.Drawing.Point(0, 542);
            this.panelDown.Margin = new System.Windows.Forms.Padding(2);
            this.panelDown.Name = "panelDown";
            this.panelDown.Size = new System.Drawing.Size(1262, 36);
            this.panelDown.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(1145, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(1035, 7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // CLDDataRetrieval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 578);
            this.Controls.Add(this.panelDown);
            this.Controls.Add(this.panelMain);
            this.Name = "CLDDataRetrieval";
            this.ShowIcon = false;
            this.Text = "云参数产品数据库查询";
            this.Load += new System.EventHandler(this.CLDDataRetrieval_Load);
            this.panelMain.ResumeLayout(false);
            this.panelGridView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDay)).EndInit();
            this.panelRetriOptions.ResumeLayout(false);
            this.panelRetriOptions.PerformLayout();
            this.groupBoxDayNight.ResumeLayout(false);
            this.groupBoxDayNight.PerformLayout();
            this.groupBoxDataSet.ResumeLayout(false);
            this.groupBoxPrdsTime.ResumeLayout(false);
            this.groupBoxPrdsTime.PerformLayout();
            this.panelPrdsTimeTen.ResumeLayout(false);
            this.panelPrdsTimeTen.PerformLayout();
            this.panelPrdsTimeMonth.ResumeLayout(false);
            this.panelPrdsTimeMonth.PerformLayout();
            this.groupBoxPrdsPeriod.ResumeLayout(false);
            this.groupBoxPrdsPeriod.PerformLayout();
            this.groupBoxProducts.ResumeLayout(false);
            this.groupBoxDataSource.ResumeLayout(false);
            this.groupBoxDataSource.PerformLayout();
            this.panelDown.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelGridView;
        private System.Windows.Forms.Panel panelRetriOptions;
        private System.Windows.Forms.GroupBox groupBoxProducts;
        private System.Windows.Forms.GroupBox groupBoxDataSource;
        private System.Windows.Forms.RadioButton radibtnISCCPD2;
        private System.Windows.Forms.RadioButton radibtnAIRS;
        private System.Windows.Forms.RadioButton radibtnMOD06;
        private System.Windows.Forms.GroupBox groupBoxPrdsPeriod;
        private System.Windows.Forms.RadioButton radibtnPrdYear;
        private System.Windows.Forms.RadioButton radibtnPrdMonth;
        private System.Windows.Forms.RadioButton radibtnPrdTen;
        private System.Windows.Forms.RadioButton radibtnPrdDay;
        private System.Windows.Forms.GroupBox groupBoxPrdsTime;
        private System.Windows.Forms.RadioButton radibtnSameTime;
        private System.Windows.Forms.RadioButton radibtnContinueTime;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.Label labelBegin;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.TextBox textBoxQueryConditions;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnAddQueryCondtions;
        private System.Windows.Forms.DataGridView dataGridViewDay;
        private System.Windows.Forms.DataGridView dataGridViewPeriod;
        private System.Windows.Forms.Panel panelPrdsTimeTen;
        private System.Windows.Forms.CheckBox checkBoxTen3;
        private System.Windows.Forms.CheckBox checkBoxTen2;
        private System.Windows.Forms.CheckBox checkBoxTen1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelPrdsTimeMonth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxDataSet;
        private ucCheckBoxList ucCheckBoxListDataSet;
        private ucMonths ucMonths;
        private ucRadioBoxList ucRadioBoxListProducts;
        private System.Windows.Forms.GroupBox groupBoxDayNight;
        private System.Windows.Forms.RadioButton radioButtonNight;
        private System.Windows.Forms.RadioButton radioButtonDay;
        private System.Windows.Forms.Panel panelDown;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridViewTextBoxColumn yearPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn MonthPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegionPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn resolutionPeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn imageNamePeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn statTypePeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn observeDateDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegionDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn resolutionDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn validpercentDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn GranuleCountsDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn GranuleTimesDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn imageNameDay;
    }
}

