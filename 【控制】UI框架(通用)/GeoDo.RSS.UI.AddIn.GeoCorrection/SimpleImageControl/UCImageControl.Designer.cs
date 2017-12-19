namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    partial class UCImageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCImageControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.DataType = new System.Windows.Forms.ToolStripLabel();
            this.DataOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ImgZoomIn = new System.Windows.Forms.ToolStripButton();
            this.ImgZoomOut = new System.Windows.Forms.ToolStripButton();
            this.ImgPan = new System.Windows.Forms.ToolStripButton();
            this.ImgFullMap = new System.Windows.Forms.ToolStripButton();
            this.SelPoint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ImgHistEven = new System.Windows.Forms.ToolStripButton();
            this.ImgRecover = new System.Windows.Forms.ToolStripButton();
            this.canvasHost1 = new GeoDo.RSS.Core.View.CanvasHost();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DataType,
            this.DataOpen,
            this.toolStripSeparator1,
            this.ImgZoomIn,
            this.ImgZoomOut,
            this.ImgPan,
            this.ImgFullMap,
            this.SelPoint,
            this.toolStripSeparator2,
            this.ImgHistEven,
            this.ImgRecover});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(632, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // DataType
            // 
            this.DataType.Name = "DataType";
            this.DataType.Size = new System.Drawing.Size(96, 28);
            this.DataType.Text = "toolStripLabel1";
            // 
            // DataOpen
            // 
            this.DataOpen.Image = ((System.Drawing.Image)(resources.GetObject("DataOpen.Image")));
            this.DataOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DataOpen.Name = "DataOpen";
            this.DataOpen.Size = new System.Drawing.Size(60, 28);
            this.DataOpen.Text = "加载";
            this.DataOpen.Click += new System.EventHandler(this.DataOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // ImgZoomIn
            // 
            this.ImgZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("ImgZoomIn.Image")));
            this.ImgZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgZoomIn.Name = "ImgZoomIn";
            this.ImgZoomIn.Size = new System.Drawing.Size(60, 28);
            this.ImgZoomIn.Text = "放大";
            this.ImgZoomIn.Click += new System.EventHandler(this.ImgZoomIn_Click);
            // 
            // ImgZoomOut
            // 
            this.ImgZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("ImgZoomOut.Image")));
            this.ImgZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgZoomOut.Name = "ImgZoomOut";
            this.ImgZoomOut.Size = new System.Drawing.Size(60, 28);
            this.ImgZoomOut.Text = "缩小";
            this.ImgZoomOut.Click += new System.EventHandler(this.ImgZoomOut_Click);
            // 
            // ImgPan
            // 
            this.ImgPan.Image = ((System.Drawing.Image)(resources.GetObject("ImgPan.Image")));
            this.ImgPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgPan.Name = "ImgPan";
            this.ImgPan.Size = new System.Drawing.Size(60, 28);
            this.ImgPan.Text = "漫游";
            this.ImgPan.Click += new System.EventHandler(this.ImgPan_Click);
            // 
            // ImgFullMap
            // 
            this.ImgFullMap.Image = ((System.Drawing.Image)(resources.GetObject("ImgFullMap.Image")));
            this.ImgFullMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgFullMap.Name = "ImgFullMap";
            this.ImgFullMap.Size = new System.Drawing.Size(60, 28);
            this.ImgFullMap.Text = "全图";
            this.ImgFullMap.Click += new System.EventHandler(this.ImgFullMap_Click);
            // 
            // SelPoint
            // 
            this.SelPoint.Image = ((System.Drawing.Image)(resources.GetObject("SelPoint.Image")));
            this.SelPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SelPoint.Name = "SelPoint";
            this.SelPoint.Size = new System.Drawing.Size(60, 28);
            this.SelPoint.Text = "选点";
            this.SelPoint.Click += new System.EventHandler(this.SelPoint_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // ImgHistEven
            // 
            this.ImgHistEven.Image = ((System.Drawing.Image)(resources.GetObject("ImgHistEven.Image")));
            this.ImgHistEven.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgHistEven.Name = "ImgHistEven";
            this.ImgHistEven.Size = new System.Drawing.Size(96, 28);
            this.ImgHistEven.Text = "直方图均衡";
            this.ImgHistEven.Click += new System.EventHandler(this.ImgHistEven_Click);
            // 
            // ImgRecover
            // 
            this.ImgRecover.Image = ((System.Drawing.Image)(resources.GetObject("ImgRecover.Image")));
            this.ImgRecover.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ImgRecover.Name = "ImgRecover";
            this.ImgRecover.Size = new System.Drawing.Size(60, 28);
            this.ImgRecover.Text = "恢复";
            this.ImgRecover.Click += new System.EventHandler(this.ImgRecover_Click);
            // 
            // canvasHost1
            // 
            this.canvasHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasHost1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.canvasHost1.Location = new System.Drawing.Point(0, 31);
            this.canvasHost1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.canvasHost1.Name = "canvasHost1";
            this.canvasHost1.Size = new System.Drawing.Size(632, 439);
            this.canvasHost1.TabIndex = 2;
            // 
            // UCImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.canvasHost1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCImageControl";
            this.Size = new System.Drawing.Size(632, 470);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ImgZoomIn;
        private System.Windows.Forms.ToolStripButton ImgZoomOut;
        private System.Windows.Forms.ToolStripButton ImgPan;
        private System.Windows.Forms.ToolStripButton ImgFullMap;
        private Core.View.CanvasHost canvasHost1;
        private System.Windows.Forms.ToolStripLabel DataType;
        private System.Windows.Forms.ToolStripButton SelPoint;
        private System.Windows.Forms.ToolStripButton DataOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton ImgHistEven;
        private System.Windows.Forms.ToolStripButton ImgRecover;
    }
}
