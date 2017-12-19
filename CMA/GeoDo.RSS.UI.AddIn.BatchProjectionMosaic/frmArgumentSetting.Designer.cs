namespace GeoDo.RSS.UI.AddIn.BatchProjectionMosaic
{
    partial class frmArgumentSetting
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
            this.labPrjOutDir = new System.Windows.Forms.Label();
            this.txtInputDir = new System.Windows.Forms.TextBox();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnOutputDir = new System.Windows.Forms.Button();
            this.btnExcute = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnInputDir = new System.Windows.Forms.Button();
            this.labProjectID = new System.Windows.Forms.Label();
            this.cmbProjectID = new System.Windows.Forms.ComboBox();
            this.rdoOResolution = new System.Windows.Forms.RadioButton();
            this.rdoCResolution = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grbBands = new System.Windows.Forms.GroupBox();
            this.txtBands = new System.Windows.Forms.TextBox();
            this.rdoCustomBands = new System.Windows.Forms.RadioButton();
            this.rdoAllBands = new System.Windows.Forms.RadioButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.labSatellite = new System.Windows.Forms.Label();
            this.cboSatellite = new System.Windows.Forms.ComboBox();
            this.labSensor = new System.Windows.Forms.Label();
            this.cboSensor = new System.Windows.Forms.ComboBox();
            this.labFilter = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.labRegionName = new System.Windows.Forms.Label();
            this.grpRegion = new System.Windows.Forms.GroupBox();
            this.radRect = new System.Windows.Forms.RadioButton();
            this.radCenter = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddEvp = new System.Windows.Forms.Button();
            this.lstRegions = new System.Windows.Forms.ListBox();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.grbInputArg = new System.Windows.Forms.GroupBox();
            this.rdoInputDir = new System.Windows.Forms.RadioButton();
            this.rdbInputFiles = new System.Windows.Forms.RadioButton();
            this.btnInputFiles = new System.Windows.Forms.Button();
            this.txtInputFiles = new System.Windows.Forms.TextBox();
            this.grbPrjArgs = new System.Windows.Forms.GroupBox();
            this.ckbMoasic = new System.Windows.Forms.CheckBox();
            this.ckbOnlyMoasicFile = new System.Windows.Forms.CheckBox();
            this.btnMoasicOutDir = new System.Windows.Forms.Button();
            this.labMoasicOutDir = new System.Windows.Forms.Label();
            this.txtMoasicOutDir = new System.Windows.Forms.TextBox();
            this.labMoasicArg = new System.Windows.Forms.Label();
            this.ckbSameOutDir = new System.Windows.Forms.CheckBox();
            this.ucOutputRegion = new GeoDo.RSS.UI.AddIn.BatchProjectionMosaic.UCGeoRangeControl();
            this.geoRegionEditCenter1 = new GeoDo.RSS.UI.AddIn.BatchProjectionMosaic.GeoRegionEditCenter();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grbBands.SuspendLayout();
            this.grpRegion.SuspendLayout();
            this.grbInputArg.SuspendLayout();
            this.grbPrjArgs.SuspendLayout();
            this.SuspendLayout();
            // 
            // labPrjOutDir
            // 
            this.labPrjOutDir.AutoSize = true;
            this.labPrjOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labPrjOutDir.Location = new System.Drawing.Point(19, 26);
            this.labPrjOutDir.Name = "labPrjOutDir";
            this.labPrjOutDir.Size = new System.Drawing.Size(92, 17);
            this.labPrjOutDir.TabIndex = 1;
            this.labPrjOutDir.Text = "输出文件路径：";
            // 
            // txtInputDir
            // 
            this.txtInputDir.Location = new System.Drawing.Point(118, 55);
            this.txtInputDir.Name = "txtInputDir";
            this.txtInputDir.Size = new System.Drawing.Size(220, 23);
            this.txtInputDir.TabIndex = 2;
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Location = new System.Drawing.Point(118, 26);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(220, 23);
            this.txtOutputDir.TabIndex = 3;
            // 
            // btnOutputDir
            // 
            this.btnOutputDir.Location = new System.Drawing.Point(343, 24);
            this.btnOutputDir.Name = "btnOutputDir";
            this.btnOutputDir.Size = new System.Drawing.Size(25, 25);
            this.btnOutputDir.TabIndex = 5;
            this.btnOutputDir.UseVisualStyleBackColor = true;
            this.btnOutputDir.Click += new System.EventHandler(this.btnOutputDir_Click);
            // 
            // btnExcute
            // 
            this.btnExcute.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExcute.Location = new System.Drawing.Point(3, 3);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(75, 23);
            this.btnExcute.TabIndex = 9;
            this.btnExcute.Text = "执行";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(100, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnInputDir
            // 
            this.btnInputDir.Location = new System.Drawing.Point(343, 53);
            this.btnInputDir.Name = "btnInputDir";
            this.btnInputDir.Size = new System.Drawing.Size(25, 25);
            this.btnInputDir.TabIndex = 4;
            this.btnInputDir.UseVisualStyleBackColor = true;
            this.btnInputDir.Click += new System.EventHandler(this.btnInputDir_Click);
            // 
            // labProjectID
            // 
            this.labProjectID.AutoSize = true;
            this.labProjectID.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labProjectID.Location = new System.Drawing.Point(19, 55);
            this.labProjectID.Name = "labProjectID";
            this.labProjectID.Size = new System.Drawing.Size(68, 17);
            this.labProjectID.TabIndex = 11;
            this.labProjectID.Text = "投影方式：";
            // 
            // cmbProjectID
            // 
            this.cmbProjectID.FormattingEnabled = true;
            this.cmbProjectID.Items.AddRange(new object[] {
            "等经纬度投影"});
            this.cmbProjectID.Location = new System.Drawing.Point(118, 55);
            this.cmbProjectID.Name = "cmbProjectID";
            this.cmbProjectID.Size = new System.Drawing.Size(220, 25);
            this.cmbProjectID.TabIndex = 12;
            // 
            // rdoOResolution
            // 
            this.rdoOResolution.AutoSize = true;
            this.rdoOResolution.Checked = true;
            this.rdoOResolution.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdoOResolution.Location = new System.Drawing.Point(6, 20);
            this.rdoOResolution.Name = "rdoOResolution";
            this.rdoOResolution.Size = new System.Drawing.Size(86, 21);
            this.rdoOResolution.TabIndex = 14;
            this.rdoOResolution.TabStop = true;
            this.rdoOResolution.Text = "原始分辨率";
            this.rdoOResolution.UseVisualStyleBackColor = true;
            // 
            // rdoCResolution
            // 
            this.rdoCResolution.AutoSize = true;
            this.rdoCResolution.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdoCResolution.Location = new System.Drawing.Point(6, 47);
            this.rdoCResolution.Name = "rdoCResolution";
            this.rdoCResolution.Size = new System.Drawing.Size(110, 21);
            this.rdoCResolution.TabIndex = 15;
            this.rdoCResolution.Text = "自定义分辨率：";
            this.rdoCResolution.UseVisualStyleBackColor = true;
            this.rdoCResolution.CheckedChanged += new System.EventHandler(this.rdoCResolution_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtResolution);
            this.groupBox1.Controls.Add(this.rdoOResolution);
            this.groupBox1.Controls.Add(this.rdoCResolution);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(19, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 78);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "输出分辨率";
            // 
            // txtResolution
            // 
            this.txtResolution.Enabled = false;
            this.txtResolution.Location = new System.Drawing.Point(122, 46);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(197, 23);
            this.txtResolution.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnExcute);
            this.panel1.Location = new System.Drawing.Point(696, 512);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 32);
            this.panel1.TabIndex = 18;
            // 
            // grbBands
            // 
            this.grbBands.Controls.Add(this.txtBands);
            this.grbBands.Controls.Add(this.rdoCustomBands);
            this.grbBands.Controls.Add(this.rdoAllBands);
            this.grbBands.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grbBands.Location = new System.Drawing.Point(19, 171);
            this.grbBands.Name = "grbBands";
            this.grbBands.Size = new System.Drawing.Size(349, 114);
            this.grbBands.TabIndex = 19;
            this.grbBands.TabStop = false;
            this.grbBands.Text = "投影通道";
            // 
            // txtBands
            // 
            this.txtBands.Enabled = false;
            this.txtBands.Location = new System.Drawing.Point(26, 78);
            this.txtBands.Name = "txtBands";
            this.txtBands.Size = new System.Drawing.Size(293, 23);
            this.txtBands.TabIndex = 2;
            this.txtBands.Visible = false;
            // 
            // rdoCustomBands
            // 
            this.rdoCustomBands.AutoSize = true;
            this.rdoCustomBands.Location = new System.Drawing.Point(7, 51);
            this.rdoCustomBands.Name = "rdoCustomBands";
            this.rdoCustomBands.Size = new System.Drawing.Size(246, 21);
            this.rdoCustomBands.TabIndex = 1;
            this.rdoCustomBands.Text = "自定义(通道值从1开始，通道间以\",\"分隔)";
            this.rdoCustomBands.UseVisualStyleBackColor = true;
            this.rdoCustomBands.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rdoAllBands
            // 
            this.rdoAllBands.AutoSize = true;
            this.rdoAllBands.Checked = true;
            this.rdoAllBands.Location = new System.Drawing.Point(7, 23);
            this.rdoAllBands.Name = "rdoAllBands";
            this.rdoAllBands.Size = new System.Drawing.Size(62, 21);
            this.rdoAllBands.TabIndex = 0;
            this.rdoAllBands.TabStop = true;
            this.rdoAllBands.Text = "全通道";
            this.rdoAllBands.UseVisualStyleBackColor = true;
            this.rdoAllBands.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // labSatellite
            // 
            this.labSatellite.AutoSize = true;
            this.labSatellite.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSatellite.Location = new System.Drawing.Point(35, 88);
            this.labSatellite.Name = "labSatellite";
            this.labSatellite.Size = new System.Drawing.Size(44, 17);
            this.labSatellite.TabIndex = 20;
            this.labSatellite.Text = "卫星：";
            // 
            // cboSatellite
            // 
            this.cboSatellite.FormattingEnabled = true;
            this.cboSatellite.Items.AddRange(new object[] {
            "FY3A",
            "FY3B",
            "NOAA16",
            "NOAA18"});
            this.cboSatellite.Location = new System.Drawing.Point(82, 84);
            this.cboSatellite.Name = "cboSatellite";
            this.cboSatellite.Size = new System.Drawing.Size(98, 25);
            this.cboSatellite.TabIndex = 21;
            this.cboSatellite.SelectedValueChanged += new System.EventHandler(this.cboSatellite_SelectedValueChanged);
            // 
            // labSensor
            // 
            this.labSensor.AutoSize = true;
            this.labSensor.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSensor.Location = new System.Drawing.Point(183, 87);
            this.labSensor.Name = "labSensor";
            this.labSensor.Size = new System.Drawing.Size(56, 17);
            this.labSensor.TabIndex = 22;
            this.labSensor.Text = "传感器：";
            // 
            // cboSensor
            // 
            this.cboSensor.FormattingEnabled = true;
            this.cboSensor.Items.AddRange(new object[] {
            "MERSI",
            "VIRR",
            "VIRRX",
            "AVHRR"});
            this.cboSensor.Location = new System.Drawing.Point(240, 84);
            this.cboSensor.Name = "cboSensor";
            this.cboSensor.Size = new System.Drawing.Size(98, 25);
            this.cboSensor.TabIndex = 23;
            this.cboSensor.SelectedValueChanged += new System.EventHandler(this.cboSensor_SelectedValueChanged);
            // 
            // labFilter
            // 
            this.labFilter.AutoSize = true;
            this.labFilter.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFilter.Location = new System.Drawing.Point(35, 118);
            this.labFilter.Name = "labFilter";
            this.labFilter.Size = new System.Drawing.Size(68, 17);
            this.labFilter.TabIndex = 24;
            this.labFilter.Text = "文件筛选：";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(118, 115);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(220, 23);
            this.txtFilter.TabIndex = 25;
            // 
            // labRegionName
            // 
            this.labRegionName.AutoSize = true;
            this.labRegionName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labRegionName.Location = new System.Drawing.Point(174, 22);
            this.labRegionName.Name = "labRegionName";
            this.labRegionName.Size = new System.Drawing.Size(59, 17);
            this.labRegionName.TabIndex = 26;
            this.labRegionName.Text = "区域名称:";
            // 
            // grpRegion
            // 
            this.grpRegion.Controls.Add(this.ucOutputRegion);
            this.grpRegion.Controls.Add(this.radRect);
            this.grpRegion.Controls.Add(this.radCenter);
            this.grpRegion.Controls.Add(this.btnDelete);
            this.grpRegion.Controls.Add(this.btnAddEvp);
            this.grpRegion.Controls.Add(this.lstRegions);
            this.grpRegion.Controls.Add(this.txtRegionName);
            this.grpRegion.Controls.Add(this.labRegionName);
            this.grpRegion.Controls.Add(this.geoRegionEditCenter1);
            this.grpRegion.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpRegion.Location = new System.Drawing.Point(390, 20);
            this.grpRegion.Name = "grpRegion";
            this.grpRegion.Size = new System.Drawing.Size(460, 297);
            this.grpRegion.TabIndex = 27;
            this.grpRegion.TabStop = false;
            this.grpRegion.Text = "输出范围";
            // 
            // radRect
            // 
            this.radRect.AutoSize = true;
            this.radRect.Checked = true;
            this.radRect.Location = new System.Drawing.Point(243, 46);
            this.radRect.Name = "radRect";
            this.radRect.Size = new System.Drawing.Size(83, 16);
            this.radRect.TabIndex = 33;
            this.radRect.TabStop = true;
            this.radRect.Text = "经纬度范围";
            this.radRect.UseVisualStyleBackColor = true;
            this.radRect.CheckedChanged += new System.EventHandler(this.radRect_CheckedChanged);
            // 
            // radCenter
            // 
            this.radCenter.AutoSize = true;
            this.radCenter.Location = new System.Drawing.Point(177, 46);
            this.radCenter.Name = "radCenter";
            this.radCenter.Size = new System.Drawing.Size(59, 16);
            this.radCenter.TabIndex = 33;
            this.radCenter.Text = "中心点";
            this.radCenter.UseVisualStyleBackColor = true;
            this.radCenter.CheckedChanged += new System.EventHandler(this.radCenter_CheckedChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(370, 114);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 31;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddEvp
            // 
            this.btnAddEvp.Location = new System.Drawing.Point(370, 85);
            this.btnAddEvp.Name = "btnAddEvp";
            this.btnAddEvp.Size = new System.Drawing.Size(75, 23);
            this.btnAddEvp.TabIndex = 30;
            this.btnAddEvp.Text = "添加";
            this.btnAddEvp.UseVisualStyleBackColor = true;
            this.btnAddEvp.Click += new System.EventHandler(this.btnAddEvp_Click);
            // 
            // lstRegions
            // 
            this.lstRegions.FormattingEnabled = true;
            this.lstRegions.ItemHeight = 12;
            this.lstRegions.Location = new System.Drawing.Point(13, 20);
            this.lstRegions.Name = "lstRegions";
            this.lstRegions.Size = new System.Drawing.Size(155, 256);
            this.lstRegions.TabIndex = 29;
            this.lstRegions.SelectedIndexChanged += new System.EventHandler(this.lstRegions_SelectedIndexChanged);
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(243, 20);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(191, 21);
            this.txtRegionName.TabIndex = 28;
            // 
            // grbInputArg
            // 
            this.grbInputArg.Controls.Add(this.rdoInputDir);
            this.grbInputArg.Controls.Add(this.rdbInputFiles);
            this.grbInputArg.Controls.Add(this.txtFilter);
            this.grbInputArg.Controls.Add(this.btnInputFiles);
            this.grbInputArg.Controls.Add(this.labFilter);
            this.grbInputArg.Controls.Add(this.txtInputFiles);
            this.grbInputArg.Controls.Add(this.cboSensor);
            this.grbInputArg.Controls.Add(this.txtInputDir);
            this.grbInputArg.Controls.Add(this.labSensor);
            this.grbInputArg.Controls.Add(this.btnInputDir);
            this.grbInputArg.Controls.Add(this.cboSatellite);
            this.grbInputArg.Controls.Add(this.labSatellite);
            this.grbInputArg.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grbInputArg.Location = new System.Drawing.Point(12, 12);
            this.grbInputArg.Name = "grbInputArg";
            this.grbInputArg.Size = new System.Drawing.Size(382, 154);
            this.grbInputArg.TabIndex = 28;
            this.grbInputArg.TabStop = false;
            this.grbInputArg.Text = "输入文件参数";
            // 
            // rdoInputDir
            // 
            this.rdoInputDir.AutoSize = true;
            this.rdoInputDir.Checked = true;
            this.rdoInputDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdoInputDir.Location = new System.Drawing.Point(19, 55);
            this.rdoInputDir.Name = "rdoInputDir";
            this.rdoInputDir.Size = new System.Drawing.Size(98, 21);
            this.rdoInputDir.TabIndex = 4;
            this.rdoInputDir.TabStop = true;
            this.rdoInputDir.Text = "输入文件夹：";
            this.rdoInputDir.UseVisualStyleBackColor = true;
            // 
            // rdbInputFiles
            // 
            this.rdbInputFiles.AutoSize = true;
            this.rdbInputFiles.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdbInputFiles.Location = new System.Drawing.Point(19, 26);
            this.rdbInputFiles.Name = "rdbInputFiles";
            this.rdbInputFiles.Size = new System.Drawing.Size(86, 21);
            this.rdbInputFiles.TabIndex = 3;
            this.rdbInputFiles.Text = "输入文件：";
            this.rdbInputFiles.UseVisualStyleBackColor = true;
            this.rdbInputFiles.CheckedChanged += new System.EventHandler(this.rdbInputFiles_CheckedChanged);
            // 
            // btnInputFiles
            // 
            this.btnInputFiles.Enabled = false;
            this.btnInputFiles.Location = new System.Drawing.Point(343, 24);
            this.btnInputFiles.Name = "btnInputFiles";
            this.btnInputFiles.Size = new System.Drawing.Size(25, 25);
            this.btnInputFiles.TabIndex = 2;
            this.btnInputFiles.UseVisualStyleBackColor = true;
            this.btnInputFiles.Click += new System.EventHandler(this.btnInputFiles_Click);
            // 
            // txtInputFiles
            // 
            this.txtInputFiles.Enabled = false;
            this.txtInputFiles.Location = new System.Drawing.Point(118, 26);
            this.txtInputFiles.Name = "txtInputFiles";
            this.txtInputFiles.Size = new System.Drawing.Size(220, 23);
            this.txtInputFiles.TabIndex = 1;
            // 
            // grbPrjArgs
            // 
            this.grbPrjArgs.Controls.Add(this.txtOutputDir);
            this.grbPrjArgs.Controls.Add(this.labPrjOutDir);
            this.grbPrjArgs.Controls.Add(this.grpRegion);
            this.grbPrjArgs.Controls.Add(this.groupBox1);
            this.grbPrjArgs.Controls.Add(this.btnOutputDir);
            this.grbPrjArgs.Controls.Add(this.grbBands);
            this.grbPrjArgs.Controls.Add(this.labProjectID);
            this.grbPrjArgs.Controls.Add(this.cmbProjectID);
            this.grbPrjArgs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grbPrjArgs.Location = new System.Drawing.Point(12, 181);
            this.grbPrjArgs.Name = "grbPrjArgs";
            this.grbPrjArgs.Size = new System.Drawing.Size(864, 325);
            this.grbPrjArgs.TabIndex = 29;
            this.grbPrjArgs.TabStop = false;
            this.grbPrjArgs.Text = "投影参数";
            // 
            // ckbMoasic
            // 
            this.ckbMoasic.AutoSize = true;
            this.ckbMoasic.Checked = true;
            this.ckbMoasic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbMoasic.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbMoasic.Location = new System.Drawing.Point(416, 39);
            this.ckbMoasic.Name = "ckbMoasic";
            this.ckbMoasic.Size = new System.Drawing.Size(75, 21);
            this.ckbMoasic.TabIndex = 17;
            this.ckbMoasic.Text = "执行拼接";
            this.ckbMoasic.UseVisualStyleBackColor = true;
            this.ckbMoasic.CheckedChanged += new System.EventHandler(this.ckbMoasic_CheckedChanged);
            // 
            // ckbOnlyMoasicFile
            // 
            this.ckbOnlyMoasicFile.AutoSize = true;
            this.ckbOnlyMoasicFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbOnlyMoasicFile.Location = new System.Drawing.Point(541, 39);
            this.ckbOnlyMoasicFile.Name = "ckbOnlyMoasicFile";
            this.ckbOnlyMoasicFile.Size = new System.Drawing.Size(111, 21);
            this.ckbOnlyMoasicFile.TabIndex = 0;
            this.ckbOnlyMoasicFile.Text = "仅保存拼接文件";
            this.ckbOnlyMoasicFile.UseVisualStyleBackColor = true;
            // 
            // btnMoasicOutDir
            // 
            this.btnMoasicOutDir.Location = new System.Drawing.Point(761, 99);
            this.btnMoasicOutDir.Name = "btnMoasicOutDir";
            this.btnMoasicOutDir.Size = new System.Drawing.Size(25, 25);
            this.btnMoasicOutDir.TabIndex = 34;
            this.btnMoasicOutDir.UseVisualStyleBackColor = true;
            this.btnMoasicOutDir.Click += new System.EventHandler(this.btnMoasicOutDir_Click);
            // 
            // labMoasicOutDir
            // 
            this.labMoasicOutDir.AutoSize = true;
            this.labMoasicOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labMoasicOutDir.Location = new System.Drawing.Point(416, 104);
            this.labMoasicOutDir.Name = "labMoasicOutDir";
            this.labMoasicOutDir.Size = new System.Drawing.Size(116, 17);
            this.labMoasicOutDir.TabIndex = 32;
            this.labMoasicOutDir.Text = "拼接输出文件路径：";
            // 
            // txtMoasicOutDir
            // 
            this.txtMoasicOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMoasicOutDir.Location = new System.Drawing.Point(541, 101);
            this.txtMoasicOutDir.Name = "txtMoasicOutDir";
            this.txtMoasicOutDir.Size = new System.Drawing.Size(214, 23);
            this.txtMoasicOutDir.TabIndex = 33;
            // 
            // labMoasicArg
            // 
            this.labMoasicArg.AutoSize = true;
            this.labMoasicArg.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labMoasicArg.Location = new System.Drawing.Point(416, 15);
            this.labMoasicArg.Name = "labMoasicArg";
            this.labMoasicArg.Size = new System.Drawing.Size(68, 17);
            this.labMoasicArg.TabIndex = 35;
            this.labMoasicArg.Text = "拼接参数：";
            // 
            // ckbSameOutDir
            // 
            this.ckbSameOutDir.AutoSize = true;
            this.ckbSameOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbSameOutDir.Location = new System.Drawing.Point(416, 69);
            this.ckbSameOutDir.Name = "ckbSameOutDir";
            this.ckbSameOutDir.Size = new System.Drawing.Size(171, 21);
            this.ckbSameOutDir.TabIndex = 36;
            this.ckbSameOutDir.Text = "与投影文件存放在相同目录";
            this.ckbSameOutDir.UseVisualStyleBackColor = true;
            this.ckbSameOutDir.CheckedChanged += new System.EventHandler(this.ckbSameOutDir_CheckedChanged);
            // 
            // ucOutputRegion
            // 
            this.ucOutputRegion.Location = new System.Drawing.Point(177, 69);
            this.ucOutputRegion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucOutputRegion.MaxX = 0D;
            this.ucOutputRegion.MaxY = 0D;
            this.ucOutputRegion.MinX = 0D;
            this.ucOutputRegion.MinY = 0D;
            this.ucOutputRegion.Name = "ucOutputRegion";
            this.ucOutputRegion.Size = new System.Drawing.Size(165, 156);
            this.ucOutputRegion.TabIndex = 32;
            // 
            // geoRegionEditCenter1
            // 
            this.geoRegionEditCenter1.Location = new System.Drawing.Point(174, 71);
            this.geoRegionEditCenter1.Name = "geoRegionEditCenter1";
            this.geoRegionEditCenter1.Size = new System.Drawing.Size(184, 205);
            this.geoRegionEditCenter1.TabIndex = 34;
            this.geoRegionEditCenter1.Visible = false;
            // 
            // frmArgumentSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 556);
            this.Controls.Add(this.ckbSameOutDir);
            this.Controls.Add(this.labMoasicArg);
            this.Controls.Add(this.txtMoasicOutDir);
            this.Controls.Add(this.labMoasicOutDir);
            this.Controls.Add(this.grbPrjArgs);
            this.Controls.Add(this.btnMoasicOutDir);
            this.Controls.Add(this.grbInputArg);
            this.Controls.Add(this.ckbOnlyMoasicFile);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ckbMoasic);
            this.Name = "frmArgumentSetting";
            this.ShowIcon = false;
            this.Text = "批量投影拼接参数设置...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.grbBands.ResumeLayout(false);
            this.grbBands.PerformLayout();
            this.grpRegion.ResumeLayout(false);
            this.grpRegion.PerformLayout();
            this.grbInputArg.ResumeLayout(false);
            this.grbInputArg.PerformLayout();
            this.grbPrjArgs.ResumeLayout(false);
            this.grbPrjArgs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labPrjOutDir;
        private System.Windows.Forms.TextBox txtInputDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnInputDir;
        private System.Windows.Forms.Button btnOutputDir;
        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labProjectID;
        private System.Windows.Forms.ComboBox cmbProjectID;
        private System.Windows.Forms.RadioButton rdoOResolution;
        private System.Windows.Forms.RadioButton rdoCResolution;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grbBands;
        private System.Windows.Forms.TextBox txtBands;
        private System.Windows.Forms.RadioButton rdoCustomBands;
        private System.Windows.Forms.RadioButton rdoAllBands;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label labSatellite;
        private System.Windows.Forms.ComboBox cboSatellite;
        private System.Windows.Forms.Label labSensor;
        private System.Windows.Forms.ComboBox cboSensor;
        private System.Windows.Forms.Label labFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label labRegionName;
        private System.Windows.Forms.GroupBox grpRegion;
        private System.Windows.Forms.TextBox txtRegionName;
        private System.Windows.Forms.GroupBox grbInputArg;
        private System.Windows.Forms.RadioButton rdoInputDir;
        private System.Windows.Forms.RadioButton rdbInputFiles;
        private System.Windows.Forms.Button btnInputFiles;
        private System.Windows.Forms.TextBox txtInputFiles;
        private System.Windows.Forms.GroupBox grbPrjArgs;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddEvp;
        private System.Windows.Forms.ListBox lstRegions;
        private System.Windows.Forms.CheckBox ckbMoasic;
        private System.Windows.Forms.CheckBox ckbOnlyMoasicFile;
        private System.Windows.Forms.Button btnMoasicOutDir;
        private System.Windows.Forms.Label labMoasicOutDir;
        private System.Windows.Forms.TextBox txtMoasicOutDir;
        private System.Windows.Forms.Label labMoasicArg;
        private System.Windows.Forms.CheckBox ckbSameOutDir;
        private UCGeoRangeControl ucOutputRegion;
        private System.Windows.Forms.RadioButton radCenter;
        private System.Windows.Forms.RadioButton radRect;
        private GeoRegionEditCenter geoRegionEditCenter1;
    }
}