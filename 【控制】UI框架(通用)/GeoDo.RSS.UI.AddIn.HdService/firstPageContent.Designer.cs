namespace GeoDo.RSS.UI.AddIn.HdService
{
    partial class firstPageContent
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
            if (stc != null)
                stc.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(firstPageContent));
            this.cvPanel = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ucDefinedRegion1 = new GeoDo.RSS.UI.AddIn.DataPro.ucDefinedRegion();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnMoni = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cvPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cvPanel
            // 
            this.cvPanel.Controls.Add(this.richTextBox1);
            this.cvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cvPanel.Location = new System.Drawing.Point(307, 39);
            this.cvPanel.Name = "cvPanel";
            this.cvPanel.Size = new System.Drawing.Size(441, 431);
            this.cvPanel.TabIndex = 6;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox1.Location = new System.Drawing.Point(0, 281);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(441, 150);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(304, 39);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 431);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // ucDefinedRegion1
            // 
            this.ucDefinedRegion1.CheckBoxes = true;
            this.ucDefinedRegion1.Dock = System.Windows.Forms.DockStyle.Left;
            this.ucDefinedRegion1.Location = new System.Drawing.Point(0, 39);
            this.ucDefinedRegion1.Name = "ucDefinedRegion1";
            this.ucDefinedRegion1.Size = new System.Drawing.Size(304, 431);
            this.ucDefinedRegion1.TabIndex = 4;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMoni,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(748, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnMoni
            // 
            this.btnMoni.Image = global::GeoDo.RSS.UI.AddIn.HdService.Properties.Resources.bullet_black;
            this.btnMoni.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMoni.Name = "btnMoni";
            this.btnMoni.Size = new System.Drawing.Size(116, 36);
            this.btnMoni.Text = "当天数据监控";
            this.btnMoni.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(116, 36);
            this.toolStripButton2.Text = "历史数据查询";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(92, 36);
            this.toolStripButton3.Text = "数据处理";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "bullet_green.png");
            this.imageList1.Images.SetKeyName(1, "bullet_orange.png");
            this.imageList1.Images.SetKeyName(2, "bullet_pink.png");
            this.imageList1.Images.SetKeyName(3, "bullet_purple.png");
            this.imageList1.Images.SetKeyName(4, "bullet_red.png");
            this.imageList1.Images.SetKeyName(5, "bullet_white.png");
            this.imageList1.Images.SetKeyName(6, "bullet_yellow.png");
            this.imageList1.Images.SetKeyName(7, "bullet-black (1).png");
            this.imageList1.Images.SetKeyName(8, "bullet-black.png");
            // 
            // firstPageContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cvPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.ucDefinedRegion1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "firstPageContent";
            this.Size = new System.Drawing.Size(748, 470);
            this.cvPanel.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel cvPanel;
        private System.Windows.Forms.Splitter splitter1;
        private DataPro.ucDefinedRegion ucDefinedRegion1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnMoni;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ImageList imageList1;
    }
}
