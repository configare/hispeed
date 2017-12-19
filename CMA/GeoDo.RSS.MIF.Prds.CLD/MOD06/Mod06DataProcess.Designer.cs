namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class frmMod06DataPro
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
            this.components = new System.ComponentModel.Container();
            this.labDirName = new System.Windows.Forms.Label();
            this.txtDirName = new System.Windows.Forms.TextBox();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.grpDataSet = new System.Windows.Forms.GroupBox();
            this.tvdataset = new System.Windows.Forms.TreeView();
            this.grpRegion = new System.Windows.Forms.GroupBox();
            this.cbxLostAdded = new System.Windows.Forms.CheckBox();
            this.cbxOnlyPrj = new System.Windows.Forms.CheckBox();
            this.cbxDirectMosaic = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gpBoxOutputResolution = new System.Windows.Forms.GroupBox();
            this.cbxOriginResl = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbOutputResolutionX = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddEvp = new System.Windows.Forms.Button();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.labRegionName = new System.Windows.Forms.Label();
            this.cbxOverlapMosaic = new System.Windows.Forms.CheckBox();
            this.cbxOverlapPrj = new System.Windows.Forms.CheckBox();
            this.lstRegions = new System.Windows.Forms.ListBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnHistoryPrj = new System.Windows.Forms.Button();
            this.txtHistoryPrj = new System.Windows.Forms.TextBox();
            this.lblHistoryPrj = new System.Windows.Forms.Label();
            this.txtResl = new GeoDo.RSS.MIF.Prds.CLD.DoubleTextBox();
            this.ucGeoRangeControl1 = new GeoDo.RSS.MIF.Prds.CLD.UCGeoRangeControl();
            this.grpDataSet.SuspendLayout();
            this.grpRegion.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gpBoxOutputResolution.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labDirName
            // 
            this.labDirName.AutoSize = true;
            this.labDirName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labDirName.Location = new System.Drawing.Point(13, 13);
            this.labDirName.Name = "labDirName";
            this.labDirName.Size = new System.Drawing.Size(71, 17);
            this.labDirName.TabIndex = 0;
            this.labDirName.Text = "输入文件夹:";
            // 
            // txtDirName
            // 
            this.txtDirName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDirName.Location = new System.Drawing.Point(90, 10);
            this.txtDirName.Name = "txtDirName";
            this.txtDirName.Size = new System.Drawing.Size(339, 23);
            this.txtDirName.TabIndex = 1;
            this.txtDirName.TextChanged += new System.EventHandler(this.txtDirName_TextChanged);
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Location = new System.Drawing.Point(439, 10);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(30, 23);
            this.btnOpenDir.TabIndex = 2;
            this.btnOpenDir.UseVisualStyleBackColor = true;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // grpDataSet
            // 
            this.grpDataSet.Controls.Add(this.tvdataset);
            this.grpDataSet.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpDataSet.Location = new System.Drawing.Point(10, 62);
            this.grpDataSet.Name = "grpDataSet";
            this.grpDataSet.Size = new System.Drawing.Size(307, 300);
            this.grpDataSet.TabIndex = 5;
            this.grpDataSet.TabStop = false;
            this.grpDataSet.Text = "数据集选择";
            // 
            // tvdataset
            // 
            this.tvdataset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvdataset.CheckBoxes = true;
            this.tvdataset.Location = new System.Drawing.Point(5, 17);
            this.tvdataset.Name = "tvdataset";
            this.tvdataset.Size = new System.Drawing.Size(295, 275);
            this.tvdataset.TabIndex = 3;
            this.tvdataset.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvdataset_AfterCheck);
            // 
            // grpRegion
            // 
            this.grpRegion.Controls.Add(this.cbxLostAdded);
            this.grpRegion.Controls.Add(this.cbxOnlyPrj);
            this.grpRegion.Controls.Add(this.cbxDirectMosaic);
            this.grpRegion.Controls.Add(this.btnCancel);
            this.grpRegion.Controls.Add(this.btnOk);
            this.grpRegion.Controls.Add(this.groupBox2);
            this.grpRegion.Controls.Add(this.cbxOverlapMosaic);
            this.grpRegion.Controls.Add(this.cbxOverlapPrj);
            this.grpRegion.Controls.Add(this.lstRegions);
            this.grpRegion.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpRegion.Location = new System.Drawing.Point(322, 64);
            this.grpRegion.Name = "grpRegion";
            this.grpRegion.Size = new System.Drawing.Size(470, 298);
            this.grpRegion.TabIndex = 28;
            this.grpRegion.TabStop = false;
            this.grpRegion.Text = "输出范围";
            // 
            // cbxLostAdded
            // 
            this.cbxLostAdded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxLostAdded.AutoSize = true;
            this.cbxLostAdded.Location = new System.Drawing.Point(27, 216);
            this.cbxLostAdded.Name = "cbxLostAdded";
            this.cbxLostAdded.Size = new System.Drawing.Size(120, 16);
            this.cbxLostAdded.TabIndex = 47;
            this.cbxLostAdded.Text = "缺失文件投影拼接";
            this.cbxLostAdded.UseVisualStyleBackColor = true;
            this.cbxLostAdded.CheckedChanged += new System.EventHandler(this.cbxLostAdded_CheckedChanged);
            // 
            // cbxOnlyPrj
            // 
            this.cbxOnlyPrj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOnlyPrj.AutoSize = true;
            this.cbxOnlyPrj.Location = new System.Drawing.Point(27, 244);
            this.cbxOnlyPrj.Name = "cbxOnlyPrj";
            this.cbxOnlyPrj.Size = new System.Drawing.Size(96, 16);
            this.cbxOnlyPrj.TabIndex = 46;
            this.cbxOnlyPrj.Text = "只投影不拼接";
            this.cbxOnlyPrj.UseVisualStyleBackColor = true;
            this.cbxOnlyPrj.CheckedChanged += new System.EventHandler(this.cbxOnlyPrj_CheckedChanged);
            // 
            // cbxDirectMosaic
            // 
            this.cbxDirectMosaic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxDirectMosaic.AutoSize = true;
            this.cbxDirectMosaic.Location = new System.Drawing.Point(168, 244);
            this.cbxDirectMosaic.Name = "cbxDirectMosaic";
            this.cbxDirectMosaic.Size = new System.Drawing.Size(120, 16);
            this.cbxDirectMosaic.TabIndex = 45;
            this.cbxDirectMosaic.Text = "历史投影文件拼接";
            this.cbxDirectMosaic.UseVisualStyleBackColor = true;
            this.cbxDirectMosaic.CheckedChanged += new System.EventHandler(this.cbxDirectMosaic_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(393, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 23);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(307, 248);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(59, 23);
            this.btnOk.TabIndex = 43;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.gpBoxOutputResolution);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.btnAddEvp);
            this.groupBox2.Controls.Add(this.txtRegionName);
            this.groupBox2.Controls.Add(this.labRegionName);
            this.groupBox2.Location = new System.Drawing.Point(168, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(301, 215);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "区域设置";
            // 
            // gpBoxOutputResolution
            // 
            this.gpBoxOutputResolution.Controls.Add(this.cbxOriginResl);
            this.gpBoxOutputResolution.Controls.Add(this.txtResl);
            this.gpBoxOutputResolution.Controls.Add(this.label2);
            this.gpBoxOutputResolution.Controls.Add(this.lbOutputResolutionX);
            this.gpBoxOutputResolution.Location = new System.Drawing.Point(160, 47);
            this.gpBoxOutputResolution.Name = "gpBoxOutputResolution";
            this.gpBoxOutputResolution.Size = new System.Drawing.Size(128, 79);
            this.gpBoxOutputResolution.TabIndex = 47;
            this.gpBoxOutputResolution.TabStop = false;
            this.gpBoxOutputResolution.Text = "分辨率";
            // 
            // cbxOriginResl
            // 
            this.cbxOriginResl.AutoSize = true;
            this.cbxOriginResl.Checked = true;
            this.cbxOriginResl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxOriginResl.Location = new System.Drawing.Point(21, 54);
            this.cbxOriginResl.Name = "cbxOriginResl";
            this.cbxOriginResl.Size = new System.Drawing.Size(84, 16);
            this.cbxOriginResl.TabIndex = 44;
            this.cbxOriginResl.Text = "原始分辨率";
            this.cbxOriginResl.UseVisualStyleBackColor = true;
            this.cbxOriginResl.CheckedChanged += new System.EventHandler(this.cbxOriginResl_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 42;
            this.label2.Text = "X/Y:";
            // 
            // lbOutputResolutionX
            // 
            this.lbOutputResolutionX.AutoSize = true;
            this.lbOutputResolutionX.Location = new System.Drawing.Point(107, 29);
            this.lbOutputResolutionX.Name = "lbOutputResolutionX";
            this.lbOutputResolutionX.Size = new System.Drawing.Size(17, 12);
            this.lbOutputResolutionX.TabIndex = 20;
            this.lbOutputResolutionX.Text = "度";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucGeoRangeControl1);
            this.groupBox1.Location = new System.Drawing.Point(7, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 160);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "经纬度范围";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(197, 161);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 45;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddEvp
            // 
            this.btnAddEvp.Location = new System.Drawing.Point(197, 132);
            this.btnAddEvp.Name = "btnAddEvp";
            this.btnAddEvp.Size = new System.Drawing.Size(75, 23);
            this.btnAddEvp.TabIndex = 44;
            this.btnAddEvp.Text = "添加";
            this.btnAddEvp.UseVisualStyleBackColor = true;
            this.btnAddEvp.Click += new System.EventHandler(this.btnAddEvp_Click);
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(83, 17);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(191, 21);
            this.txtRegionName.TabIndex = 43;
            // 
            // labRegionName
            // 
            this.labRegionName.AutoSize = true;
            this.labRegionName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labRegionName.Location = new System.Drawing.Point(14, 19);
            this.labRegionName.Name = "labRegionName";
            this.labRegionName.Size = new System.Drawing.Size(59, 17);
            this.labRegionName.TabIndex = 42;
            this.labRegionName.Text = "区域名称:";
            // 
            // cbxOverlapMosaic
            // 
            this.cbxOverlapMosaic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOverlapMosaic.AutoSize = true;
            this.cbxOverlapMosaic.Location = new System.Drawing.Point(168, 270);
            this.cbxOverlapMosaic.Name = "cbxOverlapMosaic";
            this.cbxOverlapMosaic.Size = new System.Drawing.Size(120, 16);
            this.cbxOverlapMosaic.TabIndex = 37;
            this.cbxOverlapMosaic.Text = "覆盖历史拼接文件";
            this.cbxOverlapMosaic.UseVisualStyleBackColor = true;
            // 
            // cbxOverlapPrj
            // 
            this.cbxOverlapPrj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxOverlapPrj.AutoSize = true;
            this.cbxOverlapPrj.Location = new System.Drawing.Point(27, 270);
            this.cbxOverlapPrj.Name = "cbxOverlapPrj";
            this.cbxOverlapPrj.Size = new System.Drawing.Size(120, 16);
            this.cbxOverlapPrj.TabIndex = 36;
            this.cbxOverlapPrj.Text = "覆盖历史投影文件";
            this.cbxOverlapPrj.UseVisualStyleBackColor = true;
            this.cbxOverlapPrj.CheckedChanged += new System.EventHandler(this.cbxOverlapPrj_CheckedChanged);
            // 
            // lstRegions
            // 
            this.lstRegions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstRegions.FormattingEnabled = true;
            this.lstRegions.ItemHeight = 12;
            this.lstRegions.Location = new System.Drawing.Point(13, 20);
            this.lstRegions.Name = "lstRegions";
            this.lstRegions.Size = new System.Drawing.Size(150, 184);
            this.lstRegions.TabIndex = 29;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(14, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 31;
            this.label1.Text = "输出文件夹：";
            // 
            // txtOutDir
            // 
            this.txtOutDir.Enabled = false;
            this.txtOutDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOutDir.Location = new System.Drawing.Point(90, 39);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(339, 23);
            this.txtOutDir.TabIndex = 32;
            // 
            // btnOutDir
            // 
            this.btnOutDir.Enabled = false;
            this.btnOutDir.Location = new System.Drawing.Point(440, 39);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(29, 23);
            this.btnOutDir.TabIndex = 33;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 392);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(810, 23);
            this.progressBar.TabIndex = 34;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 370);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(810, 22);
            this.statusStrip1.TabIndex = 35;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(795, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // btnHistoryPrj
            // 
            this.btnHistoryPrj.Location = new System.Drawing.Point(770, 26);
            this.btnHistoryPrj.Name = "btnHistoryPrj";
            this.btnHistoryPrj.Size = new System.Drawing.Size(30, 23);
            this.btnHistoryPrj.TabIndex = 38;
            this.btnHistoryPrj.UseVisualStyleBackColor = true;
            this.btnHistoryPrj.Visible = false;
            this.btnHistoryPrj.Click += new System.EventHandler(this.btnHistoryPrj_Click);
            // 
            // txtHistoryPrj
            // 
            this.txtHistoryPrj.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtHistoryPrj.Location = new System.Drawing.Point(563, 26);
            this.txtHistoryPrj.Name = "txtHistoryPrj";
            this.txtHistoryPrj.Size = new System.Drawing.Size(201, 23);
            this.txtHistoryPrj.TabIndex = 37;
            this.txtHistoryPrj.Visible = false;
            // 
            // lblHistoryPrj
            // 
            this.lblHistoryPrj.AutoSize = true;
            this.lblHistoryPrj.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHistoryPrj.Location = new System.Drawing.Point(480, 29);
            this.lblHistoryPrj.Name = "lblHistoryPrj";
            this.lblHistoryPrj.Size = new System.Drawing.Size(83, 17);
            this.lblHistoryPrj.TabIndex = 36;
            this.lblHistoryPrj.Text = "历史投影目录:";
            this.lblHistoryPrj.Visible = false;
            // 
            // txtResl
            // 
            this.txtResl.Enabled = false;
            this.txtResl.Location = new System.Drawing.Point(40, 24);
            this.txtResl.Name = "txtResl";
            this.txtResl.Size = new System.Drawing.Size(64, 21);
            this.txtResl.TabIndex = 43;
            this.txtResl.Text = "0.05";
            this.txtResl.Value = 0.05D;
            this.txtResl.EnabledChanged += new System.EventHandler(this.txtResl_EnabledChanged);
            // 
            // ucGeoRangeControl1
            // 
            this.ucGeoRangeControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGeoRangeControl1.Location = new System.Drawing.Point(3, 17);
            this.ucGeoRangeControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucGeoRangeControl1.MaxX = 0D;
            this.ucGeoRangeControl1.MaxY = 0D;
            this.ucGeoRangeControl1.MinX = 0D;
            this.ucGeoRangeControl1.MinY = 0D;
            this.ucGeoRangeControl1.Name = "ucGeoRangeControl1";
            this.ucGeoRangeControl1.Size = new System.Drawing.Size(144, 140);
            this.ucGeoRangeControl1.TabIndex = 35;
            // 
            // frmMod06DataPro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 415);
            this.Controls.Add(this.btnHistoryPrj);
            this.Controls.Add(this.txtHistoryPrj);
            this.Controls.Add(this.lblHistoryPrj);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpRegion);
            this.Controls.Add(this.grpDataSet);
            this.Controls.Add(this.btnOpenDir);
            this.Controls.Add(this.txtDirName);
            this.Controls.Add(this.labDirName);
            this.Name = "frmMod06DataPro";
            this.ShowIcon = false;
            this.Text = "MOD06数据自动处理";
            this.grpDataSet.ResumeLayout(false);
            this.grpRegion.ResumeLayout(false);
            this.grpRegion.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gpBoxOutputResolution.ResumeLayout(false);
            this.gpBoxOutputResolution.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labDirName;
        private System.Windows.Forms.TextBox txtDirName;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.GroupBox grpDataSet;
        private System.Windows.Forms.TreeView tvdataset;
        private System.Windows.Forms.GroupBox grpRegion;
        private System.Windows.Forms.ListBox lstRegions;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox cbxOverlapMosaic;
        private System.Windows.Forms.CheckBox cbxOverlapPrj;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gpBoxOutputResolution;
        private DoubleTextBox txtResl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbOutputResolutionX;
        private System.Windows.Forms.GroupBox groupBox1;
        private UCGeoRangeControl ucGeoRangeControl1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddEvp;
        private System.Windows.Forms.TextBox txtRegionName;
        private System.Windows.Forms.Label labRegionName;
        private System.Windows.Forms.CheckBox cbxOriginResl;
        private System.Windows.Forms.CheckBox cbxDirectMosaic;
        private System.Windows.Forms.CheckBox cbxOnlyPrj;
        private System.Windows.Forms.Button btnHistoryPrj;
        private System.Windows.Forms.TextBox txtHistoryPrj;
        private System.Windows.Forms.Label lblHistoryPrj;
        private System.Windows.Forms.CheckBox cbxLostAdded;
    }
}

