namespace GeoDo.RSS.MIF.Prds.FIR
{
    partial class UCGbalFirRevise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCGbalFirRevise));
            this.sbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtPointCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtDelPtCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtReCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddfile = new System.Windows.Forms.ToolStripDropDownButton();
            this.ItemAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ItemAddPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDeleteFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtOutFile = new System.Windows.Forms.ToolStripTextBox();
            this.btnOpenOutDir = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.hDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dATToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cvPanel = new System.Windows.Forms.Panel();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.btnToWorld = new System.Windows.Forms.ToolStripButton();
            this.btnGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnImgPan = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAOIDraw = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.btnDelSelectPt = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.filelistLabel = new System.Windows.Forms.ToolStripLabel();
            this.lstbFilelist = new System.Windows.Forms.ListBox();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.cvPanel.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // sbInfo
            // 
            this.sbInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.sbInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.sbInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbInfo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sbInfo.Name = "sbInfo";
            this.sbInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sbInfo.Size = new System.Drawing.Size(550, 17);
            this.sbInfo.Spring = true;
            this.sbInfo.Text = "消息栏";
            this.sbInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbInfo,
            this.txtPointCount,
            this.txtDelPtCount,
            this.txtReCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 478);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(849, 22);
            this.statusStrip1.TabIndex = 63;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtPointCount
            // 
            this.txtPointCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtPointCount.Name = "txtPointCount";
            this.txtPointCount.Size = new System.Drawing.Size(57, 17);
            this.txtPointCount.Text = "火点数：";
            // 
            // txtDelPtCount
            // 
            this.txtDelPtCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txtDelPtCount.Name = "txtDelPtCount";
            this.txtDelPtCount.Size = new System.Drawing.Size(81, 17);
            this.txtDelPtCount.Text = "删除火点数：";
            // 
            // txtReCount
            // 
            this.txtReCount.Name = "txtReCount";
            this.txtReCount.Size = new System.Drawing.Size(89, 17);
            this.txtReCount.Text = "剩余火点数据：";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddfile,
            this.toolStripSeparator3,
            this.btnDeleteFile,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.txtOutFile,
            this.btnOpenOutDir,
            this.toolStripSeparator4,
            this.btnSave,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(849, 27);
            this.toolStrip1.TabIndex = 60;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddfile
            // 
            this.btnAddfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ItemAddFile,
            this.ItemAddPath});
            this.btnAddfile.Image = ((System.Drawing.Image)(resources.GetObject("btnAddfile.Image")));
            this.btnAddfile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddfile.Name = "btnAddfile";
            this.btnAddfile.Size = new System.Drawing.Size(66, 24);
            this.btnAddfile.Text = "添加";
            // 
            // ItemAddFile
            // 
            this.ItemAddFile.Image = global::GeoDo.RSS.MIF.Prds.FIR.Properties.Resources.cmdOpen;
            this.ItemAddFile.Name = "ItemAddFile";
            this.ItemAddFile.Size = new System.Drawing.Size(134, 24);
            this.ItemAddFile.Text = "单个文件";
            this.ItemAddFile.Click += new System.EventHandler(this.ItemAddFile_Click);
            // 
            // ItemAddPath
            // 
            this.ItemAddPath.Image = ((System.Drawing.Image)(resources.GetObject("ItemAddPath.Image")));
            this.ItemAddPath.Name = "ItemAddPath";
            this.ItemAddPath.Size = new System.Drawing.Size(134, 24);
            this.ItemAddPath.Text = "目录";
            this.ItemAddPath.Click += new System.EventHandler(this.ItemAddPath_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteFile.Image")));
            this.btnDeleteFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(57, 24);
            this.btnDeleteFile.Text = "移除";
            this.btnDeleteFile.ToolTipText = "移除待修正的文件";
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 24);
            this.toolStripLabel1.Text = "输出目录:";
            // 
            // txtOutFile
            // 
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(280, 27);
            // 
            // btnOpenOutDir
            // 
            this.btnOpenOutDir.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenOutDir.Image")));
            this.btnOpenOutDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenOutDir.Name = "btnOpenOutDir";
            this.btnOpenOutDir.Size = new System.Drawing.Size(57, 24);
            this.btnOpenOutDir.Text = "打开";
            this.btnOpenOutDir.Click += new System.EventHandler(this.btnOpenOutDir_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(57, 24);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hDFToolStripMenuItem,
            this.dATToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(66, 24);
            this.toolStripDropDownButton1.Text = "另存";
            // 
            // hDFToolStripMenuItem
            // 
            this.hDFToolStripMenuItem.Name = "hDFToolStripMenuItem";
            this.hDFToolStripMenuItem.Size = new System.Drawing.Size(107, 24);
            this.hDFToolStripMenuItem.Text = "HDF";
            this.hDFToolStripMenuItem.Click += new System.EventHandler(this.hDFToolStripMenuItem_Click);
            // 
            // dATToolStripMenuItem
            // 
            this.dATToolStripMenuItem.Name = "dATToolStripMenuItem";
            this.dATToolStripMenuItem.Size = new System.Drawing.Size(107, 24);
            this.dATToolStripMenuItem.Text = "DAT";
            this.dATToolStripMenuItem.Click += new System.EventHandler(this.dATToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "20130911121324903_easyicon_net_16.png");
            this.imageList1.Images.SetKeyName(1, "20130911121320750_easyicon_net_16.ico");
            // 
            // cvPanel
            // 
            this.cvPanel.Controls.Add(this.toolStrip3);
            this.cvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cvPanel.Location = new System.Drawing.Point(214, 27);
            this.cvPanel.Name = "cvPanel";
            this.cvPanel.Size = new System.Drawing.Size(635, 451);
            this.cvPanel.TabIndex = 62;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnToWorld,
            this.btnGrid,
            this.toolStripSeparator1,
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnImgPan,
            this.toolStripSeparator5,
            this.btnAOIDraw,
            this.toolStripLabel2,
            this.btnDelSelectPt,
            this.btnUndo});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(635, 27);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // btnToWorld
            // 
            this.btnToWorld.Image = ((System.Drawing.Image)(resources.GetObject("btnToWorld.Image")));
            this.btnToWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnToWorld.Name = "btnToWorld";
            this.btnToWorld.Size = new System.Drawing.Size(85, 24);
            this.btnToWorld.Text = "全球视图";
            this.btnToWorld.Click += new System.EventHandler(this.btnToWorld_Click);
            // 
            // btnGrid
            // 
            this.btnGrid.Image = ((System.Drawing.Image)(resources.GetObject("btnGrid.Image")));
            this.btnGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGrid.Name = "btnGrid";
            this.btnGrid.Size = new System.Drawing.Size(57, 24);
            this.btnGrid.Text = "格网";
            this.btnGrid.Click += new System.EventHandler(this.btnGrid_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.Image")));
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(57, 24);
            this.btnZoomIn.Text = "放大";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.Image")));
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(57, 24);
            this.btnZoomOut.Text = "缩小";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnImgPan
            // 
            this.btnImgPan.Image = ((System.Drawing.Image)(resources.GetObject("btnImgPan.Image")));
            this.btnImgPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImgPan.Name = "btnImgPan";
            this.btnImgPan.Size = new System.Drawing.Size(57, 24);
            this.btnImgPan.Text = "漫游";
            this.btnImgPan.Click += new System.EventHandler(this.btnImgPan_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // btnAOIDraw
            // 
            this.btnAOIDraw.Image = ((System.Drawing.Image)(resources.GetObject("btnAOIDraw.Image")));
            this.btnAOIDraw.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAOIDraw.Name = "btnAOIDraw";
            this.btnAOIDraw.Size = new System.Drawing.Size(54, 24);
            this.btnAOIDraw.Text = "AOI";
            this.btnAOIDraw.Click += new System.EventHandler(this.btnAOIDraw_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(79, 24);
            this.toolStripLabel2.Text = "当前动作：";
            // 
            // btnDelSelectPt
            // 
            this.btnDelSelectPt.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.btnDelSelectPt.CheckOnClick = true;
            this.btnDelSelectPt.Image = ((System.Drawing.Image)(resources.GetObject("btnDelSelectPt.Image")));
            this.btnDelSelectPt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelSelectPt.Name = "btnDelSelectPt";
            this.btnDelSelectPt.Size = new System.Drawing.Size(85, 24);
            this.btnDelSelectPt.Text = "删除选中";
            this.btnDelSelectPt.Click += new System.EventHandler(this.btnDelSelectPt_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(85, 24);
            this.btnUndo.Text = "取消删除";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Controls.Add(this.lstbFilelist);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 451);
            this.panel1.TabIndex = 64;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filelistLabel});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(214, 25);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // filelistLabel
            // 
            this.filelistLabel.AutoSize = false;
            this.filelistLabel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.filelistLabel.Image = ((System.Drawing.Image)(resources.GetObject("filelistLabel.Image")));
            this.filelistLabel.Name = "filelistLabel";
            this.filelistLabel.Size = new System.Drawing.Size(100, 22);
            this.filelistLabel.Text = "文件列表";
            // 
            // lstbFilelist
            // 
            this.lstbFilelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstbFilelist.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstbFilelist.FormattingEnabled = true;
            this.lstbFilelist.HorizontalScrollbar = true;
            this.lstbFilelist.ItemHeight = 17;
            this.lstbFilelist.Location = new System.Drawing.Point(0, 27);
            this.lstbFilelist.Name = "lstbFilelist";
            this.lstbFilelist.Size = new System.Drawing.Size(214, 429);
            this.lstbFilelist.TabIndex = 2;
            this.lstbFilelist.Tag = "";
            // 
            // UCGbalFirRevise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cvPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCGbalFirRevise";
            this.Size = new System.Drawing.Size(849, 500);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cvPanel.ResumeLayout(false);
            this.cvPanel.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel sbInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel txtPointCount;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnDeleteFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtOutFile;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripStatusLabel txtDelPtCount;
        private System.Windows.Forms.ToolStripStatusLabel txtReCount;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripDropDownButton btnAddfile;
        private System.Windows.Forms.ToolStripMenuItem ItemAddFile;
        private System.Windows.Forms.ToolStripMenuItem ItemAddPath;
        private System.Windows.Forms.Panel cvPanel;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton btnToWorld;
        private System.Windows.Forms.ToolStripButton btnGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnImgPan;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnAOIDraw;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton btnDelSelectPt;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lstbFilelist;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel filelistLabel;
        private System.Windows.Forms.ToolStripButton btnOpenOutDir;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem hDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dATToolStripMenuItem;


    }
}
