namespace GeoDo.RSS.UI.AddIn.CMA
{
    partial class frmChangeWater
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExportText = new System.Windows.Forms.ToolStripMenuItem();
            this.btnChooseRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.vectorFeatureListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete,
            this.btnExportText,
            this.btnChooseRegion});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(547, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(41, 20);
            this.btnDelete.Text = "删除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnExportText
            // 
            this.btnExportText.Name = "btnExportText";
            this.btnExportText.Size = new System.Drawing.Size(65, 20);
            this.btnExportText.Text = "导出文本";
            // 
            // btnChooseRegion
            // 
            this.btnChooseRegion.Name = "btnChooseRegion";
            this.btnChooseRegion.Size = new System.Drawing.Size(65, 20);
            this.btnChooseRegion.Text = "选择区域";
            this.btnChooseRegion.Click += new System.EventHandler(this.btnChooseRegion_Click);
            // 
            // vectorFeatureListView
            // 
            this.vectorFeatureListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.vectorFeatureListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vectorFeatureListView.FullRowSelect = true;
            this.vectorFeatureListView.Location = new System.Drawing.Point(0, 24);
            this.vectorFeatureListView.Name = "vectorFeatureListView";
            this.vectorFeatureListView.Size = new System.Drawing.Size(547, 152);
            this.vectorFeatureListView.TabIndex = 2;
            this.vectorFeatureListView.UseCompatibleStateImageBehavior = false;
            this.vectorFeatureListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名称";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "判识水体面积(Km²)";
            this.columnHeader2.Width = 118;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "泛滥水体面积(Km²)";
            this.columnHeader3.Width = 118;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "未变化水体面积(Km²)";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "缩小水体面积(Km²)";
            this.columnHeader5.Width = 118;
            // 
            // frmChangeWater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 176);
            this.Controls.Add(this.vectorFeatureListView);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmChangeWater";
            this.ShowIcon = false;
            this.Text = "统计矢量要素";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnDelete;
        private System.Windows.Forms.ToolStripMenuItem btnExportText;
        private System.Windows.Forms.ToolStripMenuItem btnChooseRegion;
        private System.Windows.Forms.ListView vectorFeatureListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}
