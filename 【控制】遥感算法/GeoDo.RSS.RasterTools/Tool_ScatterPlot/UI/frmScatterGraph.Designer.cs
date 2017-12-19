namespace GeoDo.RSS.RasterTools
{
    partial class frmScatterGraph
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
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExport = new System.Windows.Forms.ToolStripMenuItem();
            this.分类CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选项OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtFitResult = new System.Windows.Forms.TextBox();
            this.ucScatterGraph1 = new GeoDo.RSS.RasterTools.UCScatterGraph();
            this.btnSetEndPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem,
            this.分类CToolStripMenuItem,
            this.选项OToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(523, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExport});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // btnExport
            // 
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(115, 22);
            this.btnExport.Text = "导出(&E)";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // 分类CToolStripMenuItem
            // 
            this.分类CToolStripMenuItem.Name = "分类CToolStripMenuItem";
            this.分类CToolStripMenuItem.Size = new System.Drawing.Size(60, 21);
            this.分类CToolStripMenuItem.Text = "分类(&C)";
            // 
            // 选项OToolStripMenuItem
            // 
            this.选项OToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSetEndPoints});
            this.选项OToolStripMenuItem.Name = "选项OToolStripMenuItem";
            this.选项OToolStripMenuItem.Size = new System.Drawing.Size(62, 21);
            this.选项OToolStripMenuItem.Text = "选项(&O)";
            // 
            // txtFitResult
            // 
            this.txtFitResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtFitResult.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFitResult.ForeColor = System.Drawing.Color.Blue;
            this.txtFitResult.Location = new System.Drawing.Point(0, 431);
            this.txtFitResult.Name = "txtFitResult";
            this.txtFitResult.Size = new System.Drawing.Size(523, 23);
            this.txtFitResult.TabIndex = 2;
            this.txtFitResult.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFitResult_KeyPress);
            // 
            // ucScatterGraph1
            // 
            this.ucScatterGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucScatterGraph1.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucScatterGraph1.Location = new System.Drawing.Point(0, 25);
            this.ucScatterGraph1.Name = "ucScatterGraph1";
            this.ucScatterGraph1.Size = new System.Drawing.Size(523, 406);
            this.ucScatterGraph1.TabIndex = 1;
            this.ucScatterGraph1.LinearFitFinished += new System.EventHandler(this.ucScatterGraph1_LinearFitFinished);
            // 
            // btnSetEndPoints
            // 
            this.btnSetEndPoints.Name = "btnSetEndPoints";
            this.btnSetEndPoints.Size = new System.Drawing.Size(152, 22);
            this.btnSetEndPoints.Text = "设置坐标范围";
            this.btnSetEndPoints.Click += new System.EventHandler(this.btnSetEndPoints_Click);
            // 
            // frmScatterGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 454);
            this.Controls.Add(this.ucScatterGraph1);
            this.Controls.Add(this.txtFitResult);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmScatterGraph";
            this.Text = "波段散点图...";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmScatterGraph_KeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmScatterGraph_PreviewKeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 分类CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选项OToolStripMenuItem;
        private UCScatterGraph ucScatterGraph1;
        private System.Windows.Forms.ToolStripMenuItem btnExport;
        private System.Windows.Forms.TextBox txtFitResult;
        private System.Windows.Forms.ToolStripMenuItem btnSetEndPoints;
    }
}