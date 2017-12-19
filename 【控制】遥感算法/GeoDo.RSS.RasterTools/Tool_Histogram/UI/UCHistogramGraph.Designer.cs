namespace GeoDo.RSS.RasterTools
{
    partial class UCHistogramGraph
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCHistogramGraph));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStatItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSaveToFile = new System.Windows.Forms.ToolStripButton();
            this.ucHistogramGrapCanvas1 = new GeoDo.RSS.RasterTools.UCHistogramGrapCanvas();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStatItems,
            this.btnSaveToFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(695, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStatItems
            // 
            this.btnStatItems.Image = ((System.Drawing.Image)(resources.GetObject("btnStatItems.Image")));
            this.btnStatItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStatItems.Name = "btnStatItems";
            this.btnStatItems.Size = new System.Drawing.Size(73, 22);
            this.btnStatItems.Text = "直方图";
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveToFile.Image")));
            this.btnSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(52, 22);
            this.btnSaveToFile.Text = "导出";
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // ucHistogramGrapCanvas1
            // 
            this.ucHistogramGrapCanvas1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucHistogramGrapCanvas1.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucHistogramGrapCanvas1.Location = new System.Drawing.Point(0, 25);
            this.ucHistogramGrapCanvas1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucHistogramGrapCanvas1.Name = "ucHistogramGrapCanvas1";
            this.ucHistogramGrapCanvas1.Size = new System.Drawing.Size(695, 351);
            this.ucHistogramGrapCanvas1.TabIndex = 2;
            // 
            // UCHistogramGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucHistogramGrapCanvas1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCHistogramGraph";
            this.Size = new System.Drawing.Size(695, 376);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnStatItems;
        private UCHistogramGrapCanvas ucHistogramGrapCanvas1;
        private System.Windows.Forms.ToolStripButton btnSaveToFile;
    }
}
