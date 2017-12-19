namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class frmVectorToRaster
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVectorToRaster));
            this.label1 = new System.Windows.Forms.Label();
            this.txtShpFile = new System.Windows.Forms.TextBox();
            this.btnOpenShpFile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labField = new System.Windows.Forms.Label();
            this.lvFeatures = new System.Windows.Forms.ListView();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.grb = new System.Windows.Forms.GroupBox();
            this.cmbOutputType = new System.Windows.Forms.ComboBox();
            this.txtHeight = new System.Windows.Forms.NumericUpDown();
            this.txtWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.labResolusion = new System.Windows.Forms.Label();
            this.labHeight = new System.Windows.Forms.Label();
            this.labWidth = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lsFields = new System.Windows.Forms.ListView();
            this.grbOutputFile = new System.Windows.Forms.GroupBox();
            this.btnOpenOutputFile = new System.Windows.Forms.Button();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtMaxLat = new GeoDo.RSS.UI.AddIn.Tools.DoubleTextBox();
            this.txtMinLon = new GeoDo.RSS.UI.AddIn.Tools.DoubleTextBox();
            this.txtResolution = new GeoDo.RSS.UI.AddIn.Tools.DoubleTextBox();
            this.groupBox1.SuspendLayout();
            this.grb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.grbOutputFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "矢量文件:";
            // 
            // txtShpFile
            // 
            this.txtShpFile.Enabled = false;
            this.txtShpFile.Location = new System.Drawing.Point(81, 13);
            this.txtShpFile.Name = "txtShpFile";
            this.txtShpFile.Size = new System.Drawing.Size(521, 21);
            this.txtShpFile.TabIndex = 1;
            // 
            // btnOpenShpFile
            // 
            this.btnOpenShpFile.Location = new System.Drawing.Point(611, 11);
            this.btnOpenShpFile.Name = "btnOpenShpFile";
            this.btnOpenShpFile.Size = new System.Drawing.Size(33, 25);
            this.btnOpenShpFile.TabIndex = 2;
            this.btnOpenShpFile.UseVisualStyleBackColor = true;
            this.btnOpenShpFile.Click += new System.EventHandler(this.btnOpenShpFile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labField);
            this.groupBox1.Controls.Add(this.lvFeatures);
            this.groupBox1.Controls.Add(this.cmbFields);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(15, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 210);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择待矢量化要素";
            // 
            // labField
            // 
            this.labField.AutoSize = true;
            this.labField.Location = new System.Drawing.Point(9, 22);
            this.labField.Name = "labField";
            this.labField.Size = new System.Drawing.Size(68, 17);
            this.labField.TabIndex = 3;
            this.labField.Text = "字段名称：";
            // 
            // lvFeatures
            // 
            this.lvFeatures.CheckBoxes = true;
            this.lvFeatures.Location = new System.Drawing.Point(9, 49);
            this.lvFeatures.Name = "lvFeatures";
            this.lvFeatures.Size = new System.Drawing.Size(261, 152);
            this.lvFeatures.TabIndex = 2;
            this.lvFeatures.UseCompatibleStateImageBehavior = false;
            this.lvFeatures.View = System.Windows.Forms.View.Details;
            // 
            // cmbFields
            // 
            this.cmbFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(83, 19);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(97, 25);
            this.cmbFields.TabIndex = 1;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.cmbFields_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(210, 18);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(60, 25);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "全不选";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // grb
            // 
            this.grb.Controls.Add(this.txtMaxLat);
            this.grb.Controls.Add(this.txtMinLon);
            this.grb.Controls.Add(this.cmbOutputType);
            this.grb.Controls.Add(this.txtResolution);
            this.grb.Controls.Add(this.txtHeight);
            this.grb.Controls.Add(this.txtWidth);
            this.grb.Controls.Add(this.label7);
            this.grb.Controls.Add(this.labResolusion);
            this.grb.Controls.Add(this.labHeight);
            this.grb.Controls.Add(this.labWidth);
            this.grb.Controls.Add(this.label3);
            this.grb.Controls.Add(this.label2);
            this.grb.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grb.Location = new System.Drawing.Point(302, 49);
            this.grb.Name = "grb";
            this.grb.Size = new System.Drawing.Size(349, 112);
            this.grb.TabIndex = 4;
            this.grb.TabStop = false;
            this.grb.Text = "栅格化参数";
            // 
            // cmbOutputType
            // 
            this.cmbOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputType.FormattingEnabled = true;
            this.cmbOutputType.Location = new System.Drawing.Point(260, 74);
            this.cmbOutputType.Name = "cmbOutputType";
            this.cmbOutputType.Size = new System.Drawing.Size(76, 25);
            this.cmbOutputType.TabIndex = 11;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(260, 48);
            this.txtHeight.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(76, 23);
            this.txtHeight.TabIndex = 9;
            this.txtHeight.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(100, 48);
            this.txtWidth.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(76, 23);
            this.txtWidth.TabIndex = 8;
            this.txtWidth.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(186, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 5;
            this.label7.Text = "输出类型：";
            // 
            // labResolusion
            // 
            this.labResolusion.AutoSize = true;
            this.labResolusion.Location = new System.Drawing.Point(15, 78);
            this.labResolusion.Name = "labResolusion";
            this.labResolusion.Size = new System.Drawing.Size(80, 17);
            this.labResolusion.TabIndex = 4;
            this.labResolusion.Text = "输出分辨率：";
            // 
            // labHeight
            // 
            this.labHeight.AutoSize = true;
            this.labHeight.Location = new System.Drawing.Point(186, 51);
            this.labHeight.Name = "labHeight";
            this.labHeight.Size = new System.Drawing.Size(68, 17);
            this.labHeight.TabIndex = 3;
            this.labHeight.Text = "输出高度：";
            // 
            // labWidth
            // 
            this.labWidth.AutoSize = true;
            this.labWidth.Location = new System.Drawing.Point(15, 51);
            this.labWidth.Name = "labWidth";
            this.labWidth.Size = new System.Drawing.Size(68, 17);
            this.labWidth.TabIndex = 2;
            this.labWidth.Text = "输出宽度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "最大纬度：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "最小经度：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lsFields);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(302, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 92);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "对照属性";
            // 
            // lsFields
            // 
            this.lsFields.CheckBoxes = true;
            this.lsFields.Location = new System.Drawing.Point(13, 22);
            this.lsFields.Name = "lsFields";
            this.lsFields.Size = new System.Drawing.Size(326, 61);
            this.lsFields.TabIndex = 0;
            this.lsFields.UseCompatibleStateImageBehavior = false;
            this.lsFields.View = System.Windows.Forms.View.List;
            // 
            // grbOutputFile
            // 
            this.grbOutputFile.Controls.Add(this.btnOpenOutputFile);
            this.grbOutputFile.Controls.Add(this.txtOutFile);
            this.grbOutputFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grbOutputFile.Location = new System.Drawing.Point(15, 265);
            this.grbOutputFile.Name = "grbOutputFile";
            this.grbOutputFile.Size = new System.Drawing.Size(636, 52);
            this.grbOutputFile.TabIndex = 6;
            this.grbOutputFile.TabStop = false;
            this.grbOutputFile.Text = "输出文件";
            // 
            // btnOpenOutputFile
            // 
            this.btnOpenOutputFile.Location = new System.Drawing.Point(586, 20);
            this.btnOpenOutputFile.Name = "btnOpenOutputFile";
            this.btnOpenOutputFile.Size = new System.Drawing.Size(33, 25);
            this.btnOpenOutputFile.TabIndex = 2;
            this.btnOpenOutputFile.UseVisualStyleBackColor = true;
            this.btnOpenOutputFile.Click += new System.EventHandler(this.btnOpenOutputFile_Click);
            // 
            // txtOutFile
            // 
            this.txtOutFile.Enabled = false;
            this.txtOutFile.Location = new System.Drawing.Point(9, 22);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(571, 23);
            this.txtOutFile.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 323);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(463, 23);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Location = new System.Drawing.Point(491, 323);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(76, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "执行";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(573, 323);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder-open16.png");
            // 
            // txtMaxLat
            // 
            this.txtMaxLat.DefaultValue = 0D;
            this.txtMaxLat.Location = new System.Drawing.Point(260, 19);
            this.txtMaxLat.Name = "txtMaxLat";
            this.txtMaxLat.Size = new System.Drawing.Size(76, 23);
            this.txtMaxLat.TabIndex = 13;
            this.txtMaxLat.Text = "0";
            this.txtMaxLat.Value = 0D;
            // 
            // txtMinLon
            // 
            this.txtMinLon.DefaultValue = 0D;
            this.txtMinLon.Location = new System.Drawing.Point(100, 18);
            this.txtMinLon.Name = "txtMinLon";
            this.txtMinLon.Size = new System.Drawing.Size(76, 23);
            this.txtMinLon.TabIndex = 12;
            this.txtMinLon.Text = "0";
            this.txtMinLon.Value = 0D;
            // 
            // txtResolution
            // 
            this.txtResolution.DefaultValue = 0D;
            this.txtResolution.Location = new System.Drawing.Point(100, 75);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(76, 23);
            this.txtResolution.TabIndex = 10;
            this.txtResolution.Text = "0.01";
            this.txtResolution.Value = 0.01D;
            // 
            // frmVectorToRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 356);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.grbOutputFile);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grb);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOpenShpFile);
            this.Controls.Add(this.txtShpFile);
            this.Controls.Add(this.label1);
            this.Name = "frmVectorToRaster";
            this.ShowIcon = false;
            this.Text = "矢量栅格化...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grb.ResumeLayout(false);
            this.grb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.grbOutputFile.ResumeLayout(false);
            this.grbOutputFile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtShpFile;
        private System.Windows.Forms.Button btnOpenShpFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvFeatures;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox grb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labResolusion;
        private System.Windows.Forms.Label labHeight;
        private System.Windows.Forms.Label labWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbOutputType;
        private GeoDo.RSS.UI.AddIn.Tools.DoubleTextBox txtResolution;
        private System.Windows.Forms.NumericUpDown txtHeight;
        private System.Windows.Forms.NumericUpDown txtWidth;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView lsFields;
        private System.Windows.Forms.GroupBox grbOutputFile;
        private System.Windows.Forms.Button btnOpenOutputFile;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private DoubleTextBox txtMaxLat;
        private DoubleTextBox txtMinLon;
        private System.Windows.Forms.Label labField;
        private System.Windows.Forms.ImageList imageList1;
    }
}