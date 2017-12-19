namespace GeoDo.RSS.UI.AddIn.CMA
{
    partial class frmMonitorShowSetting
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
            this.gbRasterLayer = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdCurrentResolution = new System.Windows.Forms.RadioButton();
            this.rdOrigResolution = new System.Windows.Forms.RadioButton();
            this.gbOutputRegion = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.rdAllView = new System.Windows.Forms.RadioButton();
            this.rdCurrentRegion = new System.Windows.Forms.RadioButton();
            this.rdSetRegion = new System.Windows.Forms.RadioButton();
            this.gbOtherLayerSet = new System.Windows.Forms.GroupBox();
            this.ckIsOutputGrid = new System.Windows.Forms.CheckBox();
            this.ckIsOutputVector = new System.Windows.Forms.CheckBox();
            this.ckIsOutputBin = new System.Windows.Forms.CheckBox();
            this.cbIsSaveSetting = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbRasterLayer.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gbOutputRegion.SuspendLayout();
            this.gbOtherLayerSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbRasterLayer
            // 
            this.gbRasterLayer.Controls.Add(this.groupBox4);
            this.gbRasterLayer.Controls.Add(this.gbOutputRegion);
            this.gbRasterLayer.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.gbRasterLayer.Location = new System.Drawing.Point(12, 12);
            this.gbRasterLayer.Name = "gbRasterLayer";
            this.gbRasterLayer.Size = new System.Drawing.Size(386, 215);
            this.gbRasterLayer.TabIndex = 1;
            this.gbRasterLayer.TabStop = false;
            this.gbRasterLayer.Text = "影像图层设置:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdCurrentResolution);
            this.groupBox4.Controls.Add(this.rdOrigResolution);
            this.groupBox4.Location = new System.Drawing.Point(11, 138);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(364, 61);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "分辨率";
            // 
            // rdCurrentResolution
            // 
            this.rdCurrentResolution.AutoSize = true;
            this.rdCurrentResolution.Location = new System.Drawing.Point(146, 26);
            this.rdCurrentResolution.Name = "rdCurrentResolution";
            this.rdCurrentResolution.Size = new System.Drawing.Size(125, 24);
            this.rdCurrentResolution.TabIndex = 1;
            this.rdCurrentResolution.TabStop = true;
            this.rdCurrentResolution.Text = "当前分辨率输出";
            this.rdCurrentResolution.UseVisualStyleBackColor = true;
            // 
            // rdOrigResolution
            // 
            this.rdOrigResolution.AutoSize = true;
            this.rdOrigResolution.Checked = true;
            this.rdOrigResolution.Location = new System.Drawing.Point(18, 25);
            this.rdOrigResolution.Name = "rdOrigResolution";
            this.rdOrigResolution.Size = new System.Drawing.Size(74, 24);
            this.rdOrigResolution.TabIndex = 0;
            this.rdOrigResolution.TabStop = true;
            this.rdOrigResolution.Text = "1:1输出";
            this.rdOrigResolution.UseVisualStyleBackColor = true;
            // 
            // gbOutputRegion
            // 
            this.gbOutputRegion.Controls.Add(this.comboBox1);
            this.gbOutputRegion.Controls.Add(this.rdAllView);
            this.gbOutputRegion.Controls.Add(this.rdCurrentRegion);
            this.gbOutputRegion.Controls.Add(this.rdSetRegion);
            this.gbOutputRegion.Location = new System.Drawing.Point(11, 25);
            this.gbOutputRegion.Name = "gbOutputRegion";
            this.gbOutputRegion.Size = new System.Drawing.Size(364, 107);
            this.gbOutputRegion.TabIndex = 5;
            this.gbOutputRegion.TabStop = false;
            this.gbOutputRegion.Text = "输出范围";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(146, 66);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(200, 28);
            this.comboBox1.TabIndex = 3;
            // 
            // rdAllView
            // 
            this.rdAllView.AutoSize = true;
            this.rdAllView.Checked = true;
            this.rdAllView.Location = new System.Drawing.Point(18, 31);
            this.rdAllView.Name = "rdAllView";
            this.rdAllView.Size = new System.Drawing.Size(83, 24);
            this.rdAllView.TabIndex = 0;
            this.rdAllView.TabStop = true;
            this.rdAllView.Text = "全图输出";
            this.rdAllView.UseVisualStyleBackColor = true;
            // 
            // rdCurrentRegion
            // 
            this.rdCurrentRegion.AutoSize = true;
            this.rdCurrentRegion.Location = new System.Drawing.Point(146, 30);
            this.rdCurrentRegion.Name = "rdCurrentRegion";
            this.rdCurrentRegion.Size = new System.Drawing.Size(139, 24);
            this.rdCurrentRegion.TabIndex = 1;
            this.rdCurrentRegion.TabStop = true;
            this.rdCurrentRegion.Text = "当前显示区域输出";
            this.rdCurrentRegion.UseVisualStyleBackColor = true;
            // 
            // rdSetRegion
            // 
            this.rdSetRegion.AutoSize = true;
            this.rdSetRegion.Location = new System.Drawing.Point(18, 66);
            this.rdSetRegion.Name = "rdSetRegion";
            this.rdSetRegion.Size = new System.Drawing.Size(125, 24);
            this.rdSetRegion.TabIndex = 2;
            this.rdSetRegion.TabStop = true;
            this.rdSetRegion.Text = "指定区域输出：";
            this.rdSetRegion.UseVisualStyleBackColor = true;
            this.rdSetRegion.CheckedChanged += new System.EventHandler(this.rdSetRegion_CheckedChanged);
            // 
            // gbOtherLayerSet
            // 
            this.gbOtherLayerSet.Controls.Add(this.ckIsOutputGrid);
            this.gbOtherLayerSet.Controls.Add(this.ckIsOutputVector);
            this.gbOtherLayerSet.Controls.Add(this.ckIsOutputBin);
            this.gbOtherLayerSet.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.gbOtherLayerSet.Location = new System.Drawing.Point(12, 233);
            this.gbOtherLayerSet.Name = "gbOtherLayerSet";
            this.gbOtherLayerSet.Size = new System.Drawing.Size(386, 78);
            this.gbOtherLayerSet.TabIndex = 2;
            this.gbOtherLayerSet.TabStop = false;
            this.gbOtherLayerSet.Text = "其他数据层设置：";
            // 
            // ckIsOutputGrid
            // 
            this.ckIsOutputGrid.AutoSize = true;
            this.ckIsOutputGrid.Location = new System.Drawing.Point(263, 35);
            this.ckIsOutputGrid.Name = "ckIsOutputGrid";
            this.ckIsOutputGrid.Size = new System.Drawing.Size(112, 24);
            this.ckIsOutputGrid.TabIndex = 2;
            this.ckIsOutputGrid.Text = "输出经纬网格";
            this.ckIsOutputGrid.UseVisualStyleBackColor = true;
            // 
            // ckIsOutputVector
            // 
            this.ckIsOutputVector.AutoSize = true;
            this.ckIsOutputVector.Checked = true;
            this.ckIsOutputVector.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsOutputVector.Location = new System.Drawing.Point(140, 35);
            this.ckIsOutputVector.Name = "ckIsOutputVector";
            this.ckIsOutputVector.Size = new System.Drawing.Size(98, 24);
            this.ckIsOutputVector.TabIndex = 1;
            this.ckIsOutputVector.Text = "输出矢量层";
            this.ckIsOutputVector.UseVisualStyleBackColor = true;
            // 
            // ckIsOutputBin
            // 
            this.ckIsOutputBin.AutoSize = true;
            this.ckIsOutputBin.Checked = true;
            this.ckIsOutputBin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsOutputBin.Location = new System.Drawing.Point(19, 35);
            this.ckIsOutputBin.Name = "ckIsOutputBin";
            this.ckIsOutputBin.Size = new System.Drawing.Size(98, 24);
            this.ckIsOutputBin.TabIndex = 0;
            this.ckIsOutputBin.Text = "输出二值图";
            this.ckIsOutputBin.UseVisualStyleBackColor = true;
            // 
            // cbIsSaveSetting
            // 
            this.cbIsSaveSetting.AutoSize = true;
            this.cbIsSaveSetting.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.cbIsSaveSetting.Location = new System.Drawing.Point(12, 328);
            this.cbIsSaveSetting.Name = "cbIsSaveSetting";
            this.cbIsSaveSetting.Size = new System.Drawing.Size(84, 24);
            this.cbIsSaveSetting.TabIndex = 3;
            this.cbIsSaveSetting.Text = "保存设置";
            this.cbIsSaveSetting.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.btnOK.Location = new System.Drawing.Point(209, 324);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.btnCancel.Location = new System.Drawing.Point(308, 324);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmMonitorShowSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 364);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbIsSaveSetting);
            this.Controls.Add(this.gbOtherLayerSet);
            this.Controls.Add(this.gbRasterLayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmMonitorShowSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "多通道合成图生成设置";
            this.gbRasterLayer.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gbOutputRegion.ResumeLayout(false);
            this.gbOutputRegion.PerformLayout();
            this.gbOtherLayerSet.ResumeLayout(false);
            this.gbOtherLayerSet.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRasterLayer;
        private System.Windows.Forms.RadioButton rdCurrentRegion;
        private System.Windows.Forms.RadioButton rdAllView;
        private System.Windows.Forms.RadioButton rdSetRegion;
        private System.Windows.Forms.GroupBox gbOtherLayerSet;
        private System.Windows.Forms.CheckBox ckIsOutputGrid;
        private System.Windows.Forms.CheckBox ckIsOutputVector;
        private System.Windows.Forms.CheckBox ckIsOutputBin;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdCurrentResolution;
        private System.Windows.Forms.RadioButton rdOrigResolution;
        private System.Windows.Forms.GroupBox gbOutputRegion;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cbIsSaveSetting;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}