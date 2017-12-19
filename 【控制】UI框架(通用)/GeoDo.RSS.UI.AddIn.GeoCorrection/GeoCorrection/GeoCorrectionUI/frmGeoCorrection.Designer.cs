namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    partial class frmGeoCorrection
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ColErrorXY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarpPanel = new System.Windows.Forms.Panel();
            this.ColNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColWarpX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GCPdataView = new System.Windows.Forms.DataGridView();
            this.ColWarpY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBaseX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBaseY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColErrorX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColErrorY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Browse = new System.Windows.Forms.Button();
            this.AddPoint = new System.Windows.Forms.Button();
            this.DelPoint = new System.Windows.Forms.Button();
            this.GeoCorrect = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.OutPath = new System.Windows.Forms.TextBox();
            this.BasePanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.Export = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.TotalError = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.GCPdataView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ColErrorXY
            // 
            this.ColErrorXY.HeaderText = "XY误差";
            this.ColErrorXY.Name = "ColErrorXY";
            this.ColErrorXY.ReadOnly = true;
            this.ColErrorXY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // WarpPanel
            // 
            this.WarpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WarpPanel.Location = new System.Drawing.Point(3, 38);
            this.WarpPanel.Name = "WarpPanel";
            this.WarpPanel.Size = new System.Drawing.Size(694, 594);
            this.WarpPanel.TabIndex = 49;
            // 
            // ColNo
            // 
            this.ColNo.HeaderText = "序号";
            this.ColNo.Name = "ColNo";
            this.ColNo.ReadOnly = true;
            this.ColNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColWarpX
            // 
            this.ColWarpX.HeaderText = "待校正影像X坐标";
            this.ColWarpX.Name = "ColWarpX";
            this.ColWarpX.ReadOnly = true;
            this.ColWarpX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GCPdataView
            // 
            this.GCPdataView.AllowUserToAddRows = false;
            this.GCPdataView.AllowUserToDeleteRows = false;
            this.GCPdataView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GCPdataView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.GCPdataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GCPdataView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColNo,
            this.ColWarpX,
            this.ColWarpY,
            this.ColBaseX,
            this.ColBaseY,
            this.ColErrorX,
            this.ColErrorY,
            this.ColErrorXY});
            this.tableLayoutPanel1.SetColumnSpan(this.GCPdataView, 3);
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GCPdataView.DefaultCellStyle = dataGridViewCellStyle2;
            this.GCPdataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GCPdataView.Location = new System.Drawing.Point(3, 643);
            this.GCPdataView.Name = "GCPdataView";
            this.GCPdataView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GCPdataView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.GCPdataView.RowTemplate.Height = 23;
            this.GCPdataView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GCPdataView.Size = new System.Drawing.Size(1404, 145);
            this.GCPdataView.TabIndex = 45;
            this.GCPdataView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GCPdataView_RowHeaderMouseClick);
            // 
            // ColWarpY
            // 
            this.ColWarpY.HeaderText = "待校正影像Y坐标";
            this.ColWarpY.Name = "ColWarpY";
            this.ColWarpY.ReadOnly = true;
            this.ColWarpY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColBaseX
            // 
            this.ColBaseX.HeaderText = "基准数据X坐标";
            this.ColBaseX.Name = "ColBaseX";
            this.ColBaseX.ReadOnly = true;
            this.ColBaseX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColBaseY
            // 
            this.ColBaseY.HeaderText = "基准数据Y坐标";
            this.ColBaseY.Name = "ColBaseY";
            this.ColBaseY.ReadOnly = true;
            this.ColBaseY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColErrorX
            // 
            this.ColErrorX.HeaderText = "X误差";
            this.ColErrorX.Name = "ColErrorX";
            this.ColErrorX.ReadOnly = true;
            this.ColErrorX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColErrorY
            // 
            this.ColErrorY.HeaderText = "Y误差";
            this.ColErrorY.Name = "ColErrorY";
            this.ColErrorY.ReadOnly = true;
            this.ColErrorY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Browse
            // 
            this.Browse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Browse.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Browse.Location = new System.Drawing.Point(622, 3);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(64, 23);
            this.Browse.TabIndex = 28;
            this.Browse.Text = "浏览";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // AddPoint
            // 
            this.AddPoint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.AddPoint.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddPoint.Location = new System.Drawing.Point(3, 3);
            this.AddPoint.Name = "AddPoint";
            this.AddPoint.Size = new System.Drawing.Size(69, 24);
            this.AddPoint.TabIndex = 34;
            this.AddPoint.Text = "增加点";
            this.AddPoint.UseVisualStyleBackColor = true;
            this.AddPoint.Click += new System.EventHandler(this.AddPoint_Click);
            // 
            // DelPoint
            // 
            this.DelPoint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DelPoint.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DelPoint.Location = new System.Drawing.Point(100, 3);
            this.DelPoint.Name = "DelPoint";
            this.DelPoint.Size = new System.Drawing.Size(69, 24);
            this.DelPoint.TabIndex = 35;
            this.DelPoint.Text = "删除点";
            this.DelPoint.UseVisualStyleBackColor = true;
            this.DelPoint.Click += new System.EventHandler(this.DelPoint_Click);
            // 
            // GeoCorrect
            // 
            this.GeoCorrect.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeoCorrect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GeoCorrect.Location = new System.Drawing.Point(592, 3);
            this.GeoCorrect.Name = "GeoCorrect";
            this.GeoCorrect.Size = new System.Drawing.Size(73, 24);
            this.GeoCorrect.TabIndex = 37;
            this.GeoCorrect.Text = "几何校正";
            this.GeoCorrect.UseVisualStyleBackColor = true;
            this.GeoCorrect.Click += new System.EventHandler(this.GeoCorrect_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.Browse, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.OutPath, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(694, 29);
            this.tableLayoutPanel2.TabIndex = 51;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 29;
            this.label1.Text = "输出影像：";
            // 
            // OutPath
            // 
            this.OutPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutPath.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.OutPath.Location = new System.Drawing.Point(83, 3);
            this.OutPath.Name = "OutPath";
            this.OutPath.ReadOnly = true;
            this.OutPath.Size = new System.Drawing.Size(528, 21);
            this.OutPath.TabIndex = 34;
            // 
            // BasePanel
            // 
            this.BasePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BasePanel.CausesValidation = false;
            this.BasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BasePanel.Location = new System.Drawing.Point(713, 38);
            this.BasePanel.Name = "BasePanel";
            this.BasePanel.Size = new System.Drawing.Size(694, 594);
            this.BasePanel.TabIndex = 50;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(309, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 38;
            this.label3.Text = "总误差：";
            // 
            // Export
            // 
            this.Export.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Export.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Export.Location = new System.Drawing.Point(197, 3);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(69, 24);
            this.Export.TabIndex = 36;
            this.Export.Text = "导 出";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.WarpPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.GCPdataView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BasePanel, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1410, 791);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 6;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel4.Controls.Add(this.AddPoint, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.GeoCorrect, 5, 0);
            this.tableLayoutPanel4.Controls.Add(this.DelPoint, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.Export, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.TotalError, 4, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(713, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(694, 29);
            this.tableLayoutPanel4.TabIndex = 53;
            // 
            // TotalError
            // 
            this.TotalError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TotalError.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TotalError.Location = new System.Drawing.Point(384, 4);
            this.TotalError.Name = "TotalError";
            this.TotalError.ReadOnly = true;
            this.TotalError.Size = new System.Drawing.Size(145, 21);
            this.TotalError.TabIndex = 39;
            // 
            // frmGeoCorrection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmGeoCorrection";
            this.Size = new System.Drawing.Size(1410, 791);
            ((System.ComponentModel.ISupportInitialize)(this.GCPdataView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn ColErrorXY;
        private System.Windows.Forms.Panel WarpPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColWarpX;
        private System.Windows.Forms.DataGridView GCPdataView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColWarpY;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBaseX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBaseY;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColErrorX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColErrorY;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button AddPoint;
        private System.Windows.Forms.Button GeoCorrect;
        private System.Windows.Forms.Button DelPoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.TextBox TotalError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OutPath;
        private System.Windows.Forms.Panel BasePanel;

    }
}
