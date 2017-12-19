namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class StatRegionSet
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
            this.grpRegion = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddEvp = new System.Windows.Forms.Button();
            this.lstRegions = new System.Windows.Forms.ListBox();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.labRegionName = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.radiVectorAOI = new System.Windows.Forms.RadioButton();
            this.radiRecg = new System.Windows.Forms.RadioButton();
            this.cbxUseRegion = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtAOIName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAOIStatRegion = new System.Windows.Forms.Button();
            this.ucGeoRangeControl1 = new GeoDo.RSS.MIF.Prds.CLD.UCGeoRangeControl();
            this.grpRegion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpRegion
            // 
            this.grpRegion.Controls.Add(this.groupBox1);
            this.grpRegion.Controls.Add(this.lstRegions);
            this.grpRegion.Controls.Add(this.txtRegionName);
            this.grpRegion.Controls.Add(this.labRegionName);
            this.grpRegion.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpRegion.Location = new System.Drawing.Point(11, 34);
            this.grpRegion.Name = "grpRegion";
            this.grpRegion.Size = new System.Drawing.Size(410, 209);
            this.grpRegion.TabIndex = 28;
            this.grpRegion.TabStop = false;
            this.grpRegion.Text = "矩形统计区域";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucGeoRangeControl1);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnAddEvp);
            this.groupBox1.Location = new System.Drawing.Point(179, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 158);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "经纬度范围";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(146, 119);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 31;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddEvp
            // 
            this.btnAddEvp.Location = new System.Drawing.Point(146, 90);
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
            this.lstRegions.Location = new System.Drawing.Point(13, 26);
            this.lstRegions.Name = "lstRegions";
            this.lstRegions.Size = new System.Drawing.Size(155, 172);
            this.lstRegions.TabIndex = 29;
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(243, 20);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(140, 21);
            this.txtRegionName.TabIndex = 28;
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
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(244, 305);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 29;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(331, 305);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // radiVectorAOI
            // 
            this.radiVectorAOI.AutoSize = true;
            this.radiVectorAOI.Enabled = false;
            this.radiVectorAOI.Location = new System.Drawing.Point(244, 12);
            this.radiVectorAOI.Name = "radiVectorAOI";
            this.radiVectorAOI.Size = new System.Drawing.Size(89, 16);
            this.radiVectorAOI.TabIndex = 44;
            this.radiVectorAOI.TabStop = true;
            this.radiVectorAOI.Text = "矢量AOI区域";
            this.radiVectorAOI.UseVisualStyleBackColor = true;
            this.radiVectorAOI.CheckedChanged += new System.EventHandler(this.radiRecg_CheckedChanged);
            // 
            // radiRecg
            // 
            this.radiRecg.AutoSize = true;
            this.radiRecg.Enabled = false;
            this.radiRecg.Location = new System.Drawing.Point(136, 12);
            this.radiRecg.Name = "radiRecg";
            this.radiRecg.Size = new System.Drawing.Size(71, 16);
            this.radiRecg.TabIndex = 43;
            this.radiRecg.TabStop = true;
            this.radiRecg.Text = "矩形区域";
            this.radiRecg.UseVisualStyleBackColor = true;
            this.radiRecg.CheckedChanged += new System.EventHandler(this.radiRecg_CheckedChanged);
            // 
            // cbxUseRegion
            // 
            this.cbxUseRegion.AutoSize = true;
            this.cbxUseRegion.Location = new System.Drawing.Point(12, 12);
            this.cbxUseRegion.Name = "cbxUseRegion";
            this.cbxUseRegion.Size = new System.Drawing.Size(108, 16);
            this.cbxUseRegion.TabIndex = 42;
            this.cbxUseRegion.Text = "使用自定义区域";
            this.cbxUseRegion.UseVisualStyleBackColor = true;
            this.cbxUseRegion.CheckedChanged += new System.EventHandler(this.cbxUseRegion_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtAOIName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnAOIStatRegion);
            this.groupBox2.Location = new System.Drawing.Point(13, 250);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(408, 49);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "矢量AOI统计区域";
            // 
            // txtAOIName
            // 
            this.txtAOIName.Location = new System.Drawing.Point(259, 19);
            this.txtAOIName.Name = "txtAOIName";
            this.txtAOIName.Size = new System.Drawing.Size(128, 21);
            this.txtAOIName.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(152, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 41;
            this.label2.Text = "矢量AOI区域简称：";
            // 
            // btnAOIStatRegion
            // 
            this.btnAOIStatRegion.Enabled = false;
            this.btnAOIStatRegion.Location = new System.Drawing.Point(21, 17);
            this.btnAOIStatRegion.Name = "btnAOIStatRegion";
            this.btnAOIStatRegion.Size = new System.Drawing.Size(113, 23);
            this.btnAOIStatRegion.TabIndex = 40;
            this.btnAOIStatRegion.Text = "矢量AOI区域选择";
            this.btnAOIStatRegion.UseVisualStyleBackColor = true;
            this.btnAOIStatRegion.Click += new System.EventHandler(this.btnAOIStatRegion_Click);
            // 
            // ucGeoRangeControl1
            // 
            this.ucGeoRangeControl1.Location = new System.Drawing.Point(6, 14);
            this.ucGeoRangeControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucGeoRangeControl1.MaxX = 0D;
            this.ucGeoRangeControl1.MaxY = 0D;
            this.ucGeoRangeControl1.MinX = 0D;
            this.ucGeoRangeControl1.MinY = 0D;
            this.ucGeoRangeControl1.Name = "ucGeoRangeControl1";
            this.ucGeoRangeControl1.Size = new System.Drawing.Size(141, 140);
            this.ucGeoRangeControl1.TabIndex = 35;
            // 
            // StatRegionSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 336);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.radiVectorAOI);
            this.Controls.Add(this.radiRecg);
            this.Controls.Add(this.cbxUseRegion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.grpRegion);
            this.Name = "StatRegionSet";
            this.ShowIcon = false;
            this.Text = "统计分析区域设置";
            this.grpRegion.ResumeLayout(false);
            this.grpRegion.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRegion;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddEvp;
        private System.Windows.Forms.ListBox lstRegions;
        private System.Windows.Forms.TextBox txtRegionName;
        private System.Windows.Forms.Label labRegionName;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox1;
        private UCGeoRangeControl ucGeoRangeControl1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtAOIName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAOIStatRegion;
        private System.Windows.Forms.RadioButton radiVectorAOI;
        private System.Windows.Forms.RadioButton radiRecg;
        private System.Windows.Forms.CheckBox cbxUseRegion;
    }
}

