namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class FeatureListContent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatureListContent));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStatArea = new System.Windows.Forms.ToolStripButton();
            this.btnStatBinaryArea = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRemoveFeatures = new System.Windows.Forms.ToolStripButton();
            this.btnSaveToFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.labBand = new System.Windows.Forms.ToolStripLabel();
            this.cmbBand = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.labPencilType = new System.Windows.Forms.ToolStripLabel();
            this.cbPencilType = new System.Windows.Forms.ToolStripComboBox();
            this.btnColor = new System.Windows.Forms.ToolStripButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnColor_old = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStatArea,
            this.btnStatBinaryArea,
            this.toolStripSeparator3,
            this.btnRemoveFeatures,
            this.btnSaveToFile,
            this.toolStripSeparator2,
            this.labBand,
            this.cmbBand,
            this.toolStripSeparator1,
            this.labPencilType,
            this.cbPencilType,
            this.btnColor});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(982, 29);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStatArea
            // 
            this.btnStatArea.Image = ((System.Drawing.Image)(resources.GetObject("btnStatArea.Image")));
            this.btnStatArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStatArea.Name = "btnStatArea";
            this.btnStatArea.Size = new System.Drawing.Size(76, 26);
            this.btnStatArea.Text = "面积统计";
            this.btnStatArea.Click += new System.EventHandler(this.btnStatArea_Click);
            // 
            // btnStatBinaryArea
            // 
            this.btnStatBinaryArea.Image = ((System.Drawing.Image)(resources.GetObject("btnStatBinaryArea.Image")));
            this.btnStatBinaryArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStatBinaryArea.Name = "btnStatBinaryArea";
            this.btnStatBinaryArea.Size = new System.Drawing.Size(112, 26);
            this.btnStatBinaryArea.Text = "二值图面积统计";
            this.btnStatBinaryArea.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // btnRemoveFeatures
            // 
            this.btnRemoveFeatures.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveFeatures.Image")));
            this.btnRemoveFeatures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveFeatures.Name = "btnRemoveFeatures";
            this.btnRemoveFeatures.Size = new System.Drawing.Size(52, 26);
            this.btnRemoveFeatures.Text = "移除";
            this.btnRemoveFeatures.Click += new System.EventHandler(this.btnRemoveFeatures_Click);
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveToFile.Image")));
            this.btnSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(112, 26);
            this.btnSaveToFile.Text = "导出为矢量文件";
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 29);
            // 
            // labBand
            // 
            this.labBand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.labBand.Name = "labBand";
            this.labBand.Size = new System.Drawing.Size(32, 26);
            this.labBand.Text = "波段";
            this.labBand.Visible = false;
            // 
            // cmbBand
            // 
            this.cmbBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBand.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cmbBand.Name = "cmbBand";
            this.cmbBand.Size = new System.Drawing.Size(80, 29);
            this.cmbBand.Visible = false;
            this.cmbBand.SelectedIndexChanged += new System.EventHandler(this.cmbBand_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // labPencilType
            // 
            this.labPencilType.Name = "labPencilType";
            this.labPencilType.Size = new System.Drawing.Size(56, 26);
            this.labPencilType.Text = "铅笔类型";
            // 
            // cbPencilType
            // 
            this.cbPencilType.BackColor = System.Drawing.Color.White;
            this.cbPencilType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPencilType.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cbPencilType.Items.AddRange(new object[] {
            "自由多边形",
            "线段多边形",
            "矩形"});
            this.cbPencilType.Name = "cbPencilType";
            this.cbPencilType.Size = new System.Drawing.Size(121, 29);
            this.cbPencilType.Visible = false;
            this.cbPencilType.Click += new System.EventHandler(this.cbPencilType_Click);
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColor.Margin = new System.Windows.Forms.Padding(2);
            this.btnColor.Name = "btnColor";
            this.btnColor.Padding = new System.Windows.Forms.Padding(2);
            this.btnColor.Size = new System.Drawing.Size(88, 25);
            this.btnColor.Text = "                  ";
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 29);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(982, 330);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // btnColor_old
            // 
            this.btnColor_old.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnColor_old.Location = new System.Drawing.Point(875, 0);
            this.btnColor_old.Name = "btnColor_old";
            this.btnColor_old.Size = new System.Drawing.Size(94, 28);
            this.btnColor_old.TabIndex = 2;
            this.btnColor_old.UseVisualStyleBackColor = false;
            this.btnColor_old.Visible = false;
            this.btnColor_old.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // FeatureListContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnColor_old);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FeatureListContent";
            this.Size = new System.Drawing.Size(982, 359);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnStatArea;
        private System.Windows.Forms.ToolStripButton btnStatBinaryArea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRemoveFeatures;
        private System.Windows.Forms.ToolStripButton btnSaveToFile;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStripLabel labPencilType;
        private System.Windows.Forms.ToolStripComboBox cbPencilType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Button btnColor_old;
        private System.Windows.Forms.ToolStripLabel labBand;
        private System.Windows.Forms.ToolStripComboBox cmbBand;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnColor;
    }
}
