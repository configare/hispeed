namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class DensitySliceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DensitySliceForm));
            this.tvFileInfo = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ckInterval = new System.Windows.Forms.CheckBox();
            this.numSliceRange = new System.Windows.Forms.NumericUpDown();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.labMax = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.labMin = new System.Windows.Forms.Label();
            this.btnComputeRange = new System.Windows.Forms.Button();
            this.labSliceRange = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lstDensityRange = new System.Windows.Forms.ListBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.默认ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置分割数目ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.imglist = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSliceRange)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFileInfo
            // 
            this.tvFileInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tvFileInfo.HideSelection = false;
            this.tvFileInfo.Location = new System.Drawing.Point(0, 25);
            this.tvFileInfo.Name = "tvFileInfo";
            this.tvFileInfo.Size = new System.Drawing.Size(235, 130);
            this.tvFileInfo.TabIndex = 8;
            this.tvFileInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFileInfo_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ckInterval);
            this.panel1.Controls.Add(this.numSliceRange);
            this.panel1.Controls.Add(this.txtMax);
            this.panel1.Controls.Add(this.labMax);
            this.panel1.Controls.Add(this.txtInterval);
            this.panel1.Controls.Add(this.txtMin);
            this.panel1.Controls.Add(this.labMin);
            this.panel1.Controls.Add(this.btnComputeRange);
            this.panel1.Controls.Add(this.labSliceRange);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(0, 154);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(235, 104);
            this.panel1.TabIndex = 9;
            // 
            // ckInterval
            // 
            this.ckInterval.AutoSize = true;
            this.ckInterval.Location = new System.Drawing.Point(6, 41);
            this.ckInterval.Name = "ckInterval";
            this.ckInterval.Size = new System.Drawing.Size(66, 16);
            this.ckInterval.TabIndex = 9;
            this.ckInterval.Text = "间隔值:";
            this.ckInterval.UseVisualStyleBackColor = true;
            // 
            // numSliceRange
            // 
            this.numSliceRange.Location = new System.Drawing.Point(55, 70);
            this.numSliceRange.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numSliceRange.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSliceRange.Name = "numSliceRange";
            this.numSliceRange.Size = new System.Drawing.Size(65, 21);
            this.numSliceRange.TabIndex = 6;
            this.numSliceRange.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numSliceRange.ValueChanged += new System.EventHandler(this.numSliceRange_ValueChanged);
            // 
            // txtMax
            // 
            this.txtMax.Location = new System.Drawing.Point(166, 6);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(55, 21);
            this.txtMax.TabIndex = 5;
            this.txtMax.TextChanged += new System.EventHandler(this.textBoxMax_TextChanged);
            this.txtMax.Leave += new System.EventHandler(this.txtMax_Leave);
            // 
            // labMax
            // 
            this.labMax.AutoSize = true;
            this.labMax.Location = new System.Drawing.Point(117, 10);
            this.labMax.Name = "labMax";
            this.labMax.Size = new System.Drawing.Size(47, 12);
            this.labMax.TabIndex = 4;
            this.labMax.Text = "最大值:";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(77, 39);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(144, 21);
            this.txtInterval.TabIndex = 3;
            this.txtInterval.Leave += new System.EventHandler(this.txtInterval_Leave);
            // 
            // txtMin
            // 
            this.txtMin.Location = new System.Drawing.Point(55, 6);
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(55, 21);
            this.txtMin.TabIndex = 3;
            this.txtMin.TextChanged += new System.EventHandler(this.textBoxMin_TextChanged);
            // 
            // labMin
            // 
            this.labMin.AutoSize = true;
            this.labMin.Location = new System.Drawing.Point(4, 10);
            this.labMin.Name = "labMin";
            this.labMin.Size = new System.Drawing.Size(47, 12);
            this.labMin.TabIndex = 2;
            this.labMin.Text = "最小值:";
            // 
            // btnComputeRange
            // 
            this.btnComputeRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnComputeRange.Location = new System.Drawing.Point(130, 70);
            this.btnComputeRange.Name = "btnComputeRange";
            this.btnComputeRange.Size = new System.Drawing.Size(92, 23);
            this.btnComputeRange.TabIndex = 1;
            this.btnComputeRange.Text = "计算范围";
            this.btnComputeRange.UseVisualStyleBackColor = true;
            this.btnComputeRange.Click += new System.EventHandler(this.btnComputeRange_Click);
            // 
            // labSliceRange
            // 
            this.labSliceRange.AutoSize = true;
            this.labSliceRange.Location = new System.Drawing.Point(4, 74);
            this.labSliceRange.Name = "labSliceRange";
            this.labSliceRange.Size = new System.Drawing.Size(47, 12);
            this.labSliceRange.TabIndex = 0;
            this.labSliceRange.Text = "级  数:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.lstDensityRange);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(0, 263);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 200);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "密度分割范围";
            // 
            // btnClear
            // 
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Location = new System.Drawing.Point(158, 169);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(65, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(82, 169);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(65, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "删除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnEdit.Location = new System.Drawing.Point(6, 169);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(65, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "编辑";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lstDensityRange
            // 
            this.lstDensityRange.BackColor = System.Drawing.SystemColors.Control;
            this.lstDensityRange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstDensityRange.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstDensityRange.FormattingEnabled = true;
            this.lstDensityRange.ItemHeight = 14;
            this.lstDensityRange.Location = new System.Drawing.Point(6, 20);
            this.lstDensityRange.Name = "lstDensityRange";
            this.lstDensityRange.Size = new System.Drawing.Size(217, 128);
            this.lstDensityRange.TabIndex = 2;
            this.lstDensityRange.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstDensityRange_MouseDoubleClick);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnApply.Location = new System.Drawing.Point(82, 468);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(72, 23);
            this.btnApply.TabIndex = 10;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.编辑ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(235, 25);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenFile,
            this.tsmiSaveFile});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(44, 21);
            this.tsmiFile.Text = "文件";
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.Size = new System.Drawing.Size(100, 22);
            this.tsmiOpenFile.Text = "打开";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiSaveFile
            // 
            this.tsmiSaveFile.Name = "tsmiSaveFile";
            this.tsmiSaveFile.Size = new System.Drawing.Size(100, 22);
            this.tsmiSaveFile.Text = "保存";
            this.tsmiSaveFile.Click += new System.EventHandler(this.tsmiSaveFile_Click);
            // 
            // 编辑ToolStripMenuItem
            // 
            this.编辑ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加项ToolStripMenuItem,
            this.默认ToolStripMenuItem,
            this.设置分割数目ToolStripMenuItem});
            this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            this.编辑ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.编辑ToolStripMenuItem.Text = "编辑";
            this.编辑ToolStripMenuItem.Visible = false;
            // 
            // 添加项ToolStripMenuItem
            // 
            this.添加项ToolStripMenuItem.Name = "添加项ToolStripMenuItem";
            this.添加项ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.添加项ToolStripMenuItem.Text = "添加项";
            this.添加项ToolStripMenuItem.Click += new System.EventHandler(this.AddDensityRange_Click);
            // 
            // 默认ToolStripMenuItem
            // 
            this.默认ToolStripMenuItem.Name = "默认ToolStripMenuItem";
            this.默认ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.默认ToolStripMenuItem.Text = "默认范围";
            this.默认ToolStripMenuItem.Click += new System.EventHandler(this.AddDefaultRange_Click);
            // 
            // 设置分割数目ToolStripMenuItem
            // 
            this.设置分割数目ToolStripMenuItem.Name = "设置分割数目ToolStripMenuItem";
            this.设置分割数目ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.设置分割数目ToolStripMenuItem.Text = "设置分割数目";
            this.设置分割数目ToolStripMenuItem.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(158, 468);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOk.Location = new System.Drawing.Point(7, 468);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 23);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // imglist
            // 
            this.imglist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglist.ImageStream")));
            this.imglist.TransparentColor = System.Drawing.Color.Transparent;
            this.imglist.Images.SetKeyName(0, "style.bmp");
            this.imglist.Images.SetKeyName(1, "rect2.png");
            // 
            // DensitySliceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 498);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tvFileInfo);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DensitySliceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "密度分割";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSliceRange)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvFileInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstDensityRange;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label labMax;
        private System.Windows.Forms.TextBox txtMin;
        private System.Windows.Forms.Label labMin;
        private System.Windows.Forms.Button btnComputeRange;
        private System.Windows.Forms.Label labSliceRange;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem 编辑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 默认ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置分割数目ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFile;
        private System.Windows.Forms.NumericUpDown numSliceRange;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.ImageList imglist;
        private System.Windows.Forms.CheckBox ckInterval;
    }
}