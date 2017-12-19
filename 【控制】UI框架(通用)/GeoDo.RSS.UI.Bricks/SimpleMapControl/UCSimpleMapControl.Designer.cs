namespace GeoDo.RSS.UI.Bricks
{
    partial class UCSimpleMapControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSimpleMapControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnToChina = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnToWorld = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAOI = new System.Windows.Forms.ToolStripButton();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.canvasHost1 = new GeoDo.RSS.Core.View.CanvasHost();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.btnToChina,
            this.toolStripSeparator2,
            this.btnToWorld,
            this.toolStripSeparator3,
            this.btnAOI,
            this.btnSelect});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(648, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton1.Text = "刷新";
            this.toolStripButton1.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnToChina
            // 
            this.btnToChina.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnToChina.Image = ((System.Drawing.Image)(resources.GetObject("btnToChina.Image")));
            this.btnToChina.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnToChina.Name = "btnToChina";
            this.btnToChina.Size = new System.Drawing.Size(60, 22);
            this.btnToChina.Text = "中国视图";
            this.btnToChina.ToolTipText = "切换到=>中国视图";
            this.btnToChina.Click += new System.EventHandler(this.btnToChina_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnToWorld
            // 
            this.btnToWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnToWorld.Image = ((System.Drawing.Image)(resources.GetObject("btnToWorld.Image")));
            this.btnToWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnToWorld.Name = "btnToWorld";
            this.btnToWorld.Size = new System.Drawing.Size(60, 22);
            this.btnToWorld.Text = "全球视图";
            this.btnToWorld.Click += new System.EventHandler(this.btnToWorld_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAOI
            // 
            this.btnAOI.Image = ((System.Drawing.Image)(resources.GetObject("btnAOI.Image")));
            this.btnAOI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAOI.Name = "btnAOI";
            this.btnAOI.Size = new System.Drawing.Size(50, 22);
            this.btnAOI.Text = "AOI";
            this.btnAOI.Click += new System.EventHandler(this.btnAOI_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(76, 22);
            this.btnSelect.Text = "交互选择";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // canvasHost1
            // 
            this.canvasHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasHost1.Location = new System.Drawing.Point(0, 25);
            this.canvasHost1.Name = "canvasHost1";
            this.canvasHost1.Size = new System.Drawing.Size(648, 455);
            this.canvasHost1.TabIndex = 1;
            // 
            // UCSimpleMapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.canvasHost1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCSimpleMapControl";
            this.Size = new System.Drawing.Size(648, 480);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GeoDo.RSS.Core.View.CanvasHost canvasHost1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnToChina;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnToWorld;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnAOI;
        private System.Windows.Forms.ToolStripButton btnSelect;
    }
}
