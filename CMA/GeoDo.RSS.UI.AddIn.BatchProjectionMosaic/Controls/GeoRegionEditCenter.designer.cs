namespace GeoDo.RSS.UI.AddIn.BatchProjectionMosaic
{
    partial class GeoRegionEditCenter
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gpBoxOutputSize = new System.Windows.Forms.GroupBox();
            this.linkSize = new System.Windows.Forms.Label();
            this.cmbSizeY = new System.Windows.Forms.ComboBox();
            this.cmbSizeX = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.gbEnvelope = new System.Windows.Forms.GroupBox();
            this.txtCenterLatitude = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCenterLongitude = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gpBoxOutputResolution = new System.Windows.Forms.GroupBox();
            this.linkResolution = new System.Windows.Forms.Label();
            this.cmbResolutionY = new System.Windows.Forms.ComboBox();
            this.cmbResolutionX = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lbOutputResolutionY = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbOutputResolutionX = new System.Windows.Forms.Label();
            this.txtPrjenvelope = new System.Windows.Forms.TextBox();
            this.gpBoxOutputSize.SuspendLayout();
            this.gbEnvelope.SuspendLayout();
            this.gpBoxOutputResolution.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpBoxOutputSize
            // 
            this.gpBoxOutputSize.Controls.Add(this.linkSize);
            this.gpBoxOutputSize.Controls.Add(this.cmbSizeY);
            this.gpBoxOutputSize.Controls.Add(this.cmbSizeX);
            this.gpBoxOutputSize.Controls.Add(this.label14);
            this.gpBoxOutputSize.Controls.Add(this.label19);
            this.gpBoxOutputSize.Controls.Add(this.label13);
            this.gpBoxOutputSize.Controls.Add(this.label18);
            this.gpBoxOutputSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpBoxOutputSize.Location = new System.Drawing.Point(0, 136);
            this.gpBoxOutputSize.Name = "gpBoxOutputSize";
            this.gpBoxOutputSize.Size = new System.Drawing.Size(194, 68);
            this.gpBoxOutputSize.TabIndex = 41;
            this.gpBoxOutputSize.TabStop = false;
            this.gpBoxOutputSize.Text = "图像尺寸(像素)";
            // 
            // linkSize
            // 
            this.linkSize.AutoSize = true;
            this.linkSize.Location = new System.Drawing.Point(6, 34);
            this.linkSize.Name = "linkSize";
            this.linkSize.Size = new System.Drawing.Size(11, 12);
            this.linkSize.TabIndex = 44;
            this.linkSize.Text = "+";
            this.linkSize.Click += new System.EventHandler(this.linkSize_Click);
            // 
            // cmbSizeY
            // 
            this.cmbSizeY.Enabled = false;
            this.cmbSizeY.FormattingEnabled = true;
            this.cmbSizeY.Items.AddRange(new object[] {
            "512",
            "1024",
            "2048",
            "4096"});
            this.cmbSizeY.Location = new System.Drawing.Point(55, 43);
            this.cmbSizeY.Name = "cmbSizeY";
            this.cmbSizeY.Size = new System.Drawing.Size(72, 20);
            this.cmbSizeY.TabIndex = 45;
            this.cmbSizeY.Text = "1024";
            this.cmbSizeY.TextChanged += new System.EventHandler(this.cmbSizeY_TextChanged);
            // 
            // cmbSizeX
            // 
            this.cmbSizeX.FormattingEnabled = true;
            this.cmbSizeX.Items.AddRange(new object[] {
            "512",
            "1024",
            "2048",
            "4096"});
            this.cmbSizeX.Location = new System.Drawing.Point(55, 17);
            this.cmbSizeX.Name = "cmbSizeX";
            this.cmbSizeX.Size = new System.Drawing.Size(72, 20);
            this.cmbSizeX.TabIndex = 44;
            this.cmbSizeX.Text = "1024";
            this.cmbSizeX.TextChanged += new System.EventHandler(this.cmbSizeX_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 12);
            this.label14.TabIndex = 16;
            this.label14.Text = "宽度";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(133, 46);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 12);
            this.label19.TabIndex = 23;
            this.label19.Text = "像素";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 45);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 12);
            this.label13.TabIndex = 17;
            this.label13.Text = "高度";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(133, 21);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(29, 12);
            this.label18.TabIndex = 22;
            this.label18.Text = "像素";
            // 
            // gbEnvelope
            // 
            this.gbEnvelope.Controls.Add(this.txtCenterLatitude);
            this.gbEnvelope.Controls.Add(this.label9);
            this.gbEnvelope.Controls.Add(this.txtCenterLongitude);
            this.gbEnvelope.Controls.Add(this.label4);
            this.gbEnvelope.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbEnvelope.Location = new System.Drawing.Point(0, 68);
            this.gbEnvelope.Name = "gbEnvelope";
            this.gbEnvelope.Size = new System.Drawing.Size(194, 68);
            this.gbEnvelope.TabIndex = 42;
            this.gbEnvelope.TabStop = false;
            this.gbEnvelope.Text = "中心点经纬度";
            // 
            // txtCenterLatitude
            // 
            this.txtCenterLatitude.Location = new System.Drawing.Point(55, 42);
            this.txtCenterLatitude.Name = "txtCenterLatitude";
            this.txtCenterLatitude.Size = new System.Drawing.Size(72, 21);
            this.txtCenterLatitude.TabIndex = 1;
            this.txtCenterLatitude.Text = "0.0";
            this.txtCenterLatitude.TextChanged += new System.EventHandler(this.txtCenterLatitude_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 30;
            this.label9.Text = "纬度";
            // 
            // txtCenterLongitude
            // 
            this.txtCenterLongitude.Location = new System.Drawing.Point(55, 15);
            this.txtCenterLongitude.Name = "txtCenterLongitude";
            this.txtCenterLongitude.Size = new System.Drawing.Size(72, 21);
            this.txtCenterLongitude.TabIndex = 0;
            this.txtCenterLongitude.Text = "0.0";
            this.txtCenterLongitude.TextChanged += new System.EventHandler(this.txtCenterLongitude_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "经度";
            // 
            // gpBoxOutputResolution
            // 
            this.gpBoxOutputResolution.Controls.Add(this.linkResolution);
            this.gpBoxOutputResolution.Controls.Add(this.cmbResolutionY);
            this.gpBoxOutputResolution.Controls.Add(this.cmbResolutionX);
            this.gpBoxOutputResolution.Controls.Add(this.label11);
            this.gpBoxOutputResolution.Controls.Add(this.lbOutputResolutionY);
            this.gpBoxOutputResolution.Controls.Add(this.label12);
            this.gpBoxOutputResolution.Controls.Add(this.lbOutputResolutionX);
            this.gpBoxOutputResolution.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpBoxOutputResolution.Location = new System.Drawing.Point(0, 0);
            this.gpBoxOutputResolution.Name = "gpBoxOutputResolution";
            this.gpBoxOutputResolution.Size = new System.Drawing.Size(194, 68);
            this.gpBoxOutputResolution.TabIndex = 40;
            this.gpBoxOutputResolution.TabStop = false;
            this.gpBoxOutputResolution.Text = "分辨率";
            // 
            // linkResolution
            // 
            this.linkResolution.AutoSize = true;
            this.linkResolution.Location = new System.Drawing.Point(6, 32);
            this.linkResolution.Name = "linkResolution";
            this.linkResolution.Size = new System.Drawing.Size(11, 12);
            this.linkResolution.TabIndex = 43;
            this.linkResolution.Text = "+";
            this.linkResolution.Click += new System.EventHandler(this.linkResolution_Click);
            // 
            // cmbResolutionY
            // 
            this.cmbResolutionY.Enabled = false;
            this.cmbResolutionY.FormattingEnabled = true;
            this.cmbResolutionY.Items.AddRange(new object[] {
            "0.01",
            "0.005",
            "0.0025",
            "0.05"});
            this.cmbResolutionY.Location = new System.Drawing.Point(55, 42);
            this.cmbResolutionY.Name = "cmbResolutionY";
            this.cmbResolutionY.Size = new System.Drawing.Size(72, 20);
            this.cmbResolutionY.TabIndex = 43;
            this.cmbResolutionY.Text = "0.01";
            this.cmbResolutionY.TextChanged += new System.EventHandler(this.cmbResolutionY_TextChanged);
            // 
            // cmbResolutionX
            // 
            this.cmbResolutionX.FormattingEnabled = true;
            this.cmbResolutionX.Items.AddRange(new object[] {
            "0.01",
            "0.005",
            "0.0025",
            "0.05"});
            this.cmbResolutionX.Location = new System.Drawing.Point(55, 16);
            this.cmbResolutionX.Name = "cmbResolutionX";
            this.cmbResolutionX.Size = new System.Drawing.Size(72, 20);
            this.cmbResolutionX.TabIndex = 43;
            this.cmbResolutionX.Text = "0.01";
            this.cmbResolutionX.TextChanged += new System.EventHandler(this.cmbResolutionX_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "X方向";
            // 
            // lbOutputResolutionY
            // 
            this.lbOutputResolutionY.AutoSize = true;
            this.lbOutputResolutionY.Location = new System.Drawing.Point(133, 45);
            this.lbOutputResolutionY.Name = "lbOutputResolutionY";
            this.lbOutputResolutionY.Size = new System.Drawing.Size(47, 12);
            this.lbOutputResolutionY.TabIndex = 21;
            this.lbOutputResolutionY.Text = "度/像素";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 12);
            this.label12.TabIndex = 12;
            this.label12.Text = "Y方向";
            // 
            // lbOutputResolutionX
            // 
            this.lbOutputResolutionX.AutoSize = true;
            this.lbOutputResolutionX.Location = new System.Drawing.Point(133, 17);
            this.lbOutputResolutionX.Name = "lbOutputResolutionX";
            this.lbOutputResolutionX.Size = new System.Drawing.Size(47, 12);
            this.lbOutputResolutionX.TabIndex = 20;
            this.lbOutputResolutionX.Text = "度/像素";
            // 
            // txtPrjenvelope
            // 
            this.txtPrjenvelope.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrjenvelope.Location = new System.Drawing.Point(0, 204);
            this.txtPrjenvelope.Multiline = true;
            this.txtPrjenvelope.Name = "txtPrjenvelope";
            this.txtPrjenvelope.ReadOnly = true;
            this.txtPrjenvelope.Size = new System.Drawing.Size(194, 48);
            this.txtPrjenvelope.TabIndex = 43;
            this.txtPrjenvelope.Text = "坐标范围\r\nX：12.213123,24.2313（10.222）\r\nY：23.8999,46.112222（10.222）";
            // 
            // GeoRegionEditCenter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtPrjenvelope);
            this.Controls.Add(this.gpBoxOutputSize);
            this.Controls.Add(this.gbEnvelope);
            this.Controls.Add(this.gpBoxOutputResolution);
            this.Name = "GeoRegionEditCenter";
            this.Size = new System.Drawing.Size(194, 252);
            this.gpBoxOutputSize.ResumeLayout(false);
            this.gpBoxOutputSize.PerformLayout();
            this.gbEnvelope.ResumeLayout(false);
            this.gbEnvelope.PerformLayout();
            this.gpBoxOutputResolution.ResumeLayout(false);
            this.gpBoxOutputResolution.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gpBoxOutputSize;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox gbEnvelope;
        private System.Windows.Forms.TextBox txtCenterLatitude;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCenterLongitude;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gpBoxOutputResolution;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbOutputResolutionY;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbOutputResolutionX;
        private System.Windows.Forms.ComboBox cmbResolutionX;
        private System.Windows.Forms.ComboBox cmbResolutionY;
        private System.Windows.Forms.ComboBox cmbSizeY;
        private System.Windows.Forms.ComboBox cmbSizeX;
        private System.Windows.Forms.Label linkResolution;
        private System.Windows.Forms.Label linkSize;
        private System.Windows.Forms.TextBox txtPrjenvelope;

    }
}
