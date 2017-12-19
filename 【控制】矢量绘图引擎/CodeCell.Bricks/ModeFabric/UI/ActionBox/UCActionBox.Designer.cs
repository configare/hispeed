namespace CodeCell.Bricks.ModelFabric
{
    partial class UCActionBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCActionBox));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lvResult = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtKeyWord = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabIndex = new System.Windows.Forms.TabPage();
            this.lstIndex = new System.Windows.Forms.ListBox();
            this.tabCatalog = new System.Windows.Forms.TabPage();
            this.tvActions = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabSearch = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            this.tabIndex.SuspendLayout();
            this.tabCatalog.SuspendLayout();
            this.tabSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lvResult);
            this.tabPage1.Controls.Add(this.btnSearch);
            this.tabPage1.Controls.Add(this.txtKeyWord);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(261, 382);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "查找";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvResult
            // 
            this.lvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvResult.FullRowSelect = true;
            this.lvResult.Location = new System.Drawing.Point(9, 50);
            this.lvResult.Name = "lvResult";
            this.lvResult.Size = new System.Drawing.Size(245, 327);
            this.lvResult.TabIndex = 3;
            this.lvResult.UseCompatibleStateImageBehavior = false;
            this.lvResult.View = System.Windows.Forms.View.Details;
            this.lvResult.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvResult_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "工具";
            this.columnHeader1.Width = 142;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "分类";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "描述";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(213, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(41, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyWord.FormattingEnabled = true;
            this.txtKeyWord.Location = new System.Drawing.Point(9, 23);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Size = new System.Drawing.Size(198, 20);
            this.txtKeyWord.TabIndex = 1;
            this.txtKeyWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyWord_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "关键字:";
            // 
            // tabIndex
            // 
            this.tabIndex.Controls.Add(this.lstIndex);
            this.tabIndex.Location = new System.Drawing.Point(4, 22);
            this.tabIndex.Name = "tabIndex";
            this.tabIndex.Padding = new System.Windows.Forms.Padding(3);
            this.tabIndex.Size = new System.Drawing.Size(261, 382);
            this.tabIndex.TabIndex = 1;
            this.tabIndex.Text = "索引";
            this.tabIndex.UseVisualStyleBackColor = true;
            // 
            // lstIndex
            // 
            this.lstIndex.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstIndex.FormattingEnabled = true;
            this.lstIndex.ItemHeight = 12;
            this.lstIndex.Location = new System.Drawing.Point(3, 3);
            this.lstIndex.Name = "lstIndex";
            this.lstIndex.Size = new System.Drawing.Size(255, 372);
            this.lstIndex.TabIndex = 0;
            this.lstIndex.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstIndex_MouseDoubleClick);
            // 
            // tabCatalog
            // 
            this.tabCatalog.Controls.Add(this.tvActions);
            this.tabCatalog.Location = new System.Drawing.Point(4, 22);
            this.tabCatalog.Name = "tabCatalog";
            this.tabCatalog.Padding = new System.Windows.Forms.Padding(3);
            this.tabCatalog.Size = new System.Drawing.Size(261, 382);
            this.tabCatalog.TabIndex = 0;
            this.tabCatalog.Text = "目录";
            this.tabCatalog.UseVisualStyleBackColor = true;
            // 
            // tvActions
            // 
            this.tvActions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvActions.ImageIndex = 0;
            this.tvActions.ImageList = this.imageList1;
            this.tvActions.Location = new System.Drawing.Point(3, 3);
            this.tvActions.Name = "tvActions";
            this.tvActions.SelectedImageIndex = 0;
            this.tvActions.Size = new System.Drawing.Size(255, 376);
            this.tvActions.TabIndex = 0;
            this.tvActions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvActions_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cmdThemeElementBrowser.png");
            this.imageList1.Images.SetKeyName(1, "ToolBox.png");
            // 
            // tabSearch
            // 
            this.tabSearch.Controls.Add(this.tabCatalog);
            this.tabSearch.Controls.Add(this.tabIndex);
            this.tabSearch.Controls.Add(this.tabPage1);
            this.tabSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSearch.Location = new System.Drawing.Point(0, 0);
            this.tabSearch.Name = "tabSearch";
            this.tabSearch.SelectedIndex = 0;
            this.tabSearch.Size = new System.Drawing.Size(269, 408);
            this.tabSearch.TabIndex = 0;
            // 
            // UCActionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabSearch);
            this.Name = "UCActionBox";
            this.Size = new System.Drawing.Size(269, 408);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabIndex.ResumeLayout(false);
            this.tabCatalog.ResumeLayout(false);
            this.tabSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabIndex;
        private System.Windows.Forms.ListBox lstIndex;
        private System.Windows.Forms.TabPage tabCatalog;
        private System.Windows.Forms.TreeView tvActions;
        private System.Windows.Forms.TabControl tabSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox txtKeyWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvResult;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
