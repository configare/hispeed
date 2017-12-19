namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class RasterPropertyWndContent
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
            this.btnApply = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucRasterInfoTree1 = new GeoDo.RSS.UI.AddIn.Windows.UCRasterInfoTree();
            this.ucSelectBandForRgb1 = new GeoDo.RSS.UI.AddIn.Windows.UCSelectBandForRgb();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(151, 186);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(101, 27);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "应用(Load)";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 275);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(255, 5);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.ucSelectBandForRgb1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 280);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(255, 220);
            this.panel1.TabIndex = 5;
            // 
            // ucRasterInfoTree1
            // 
            this.ucRasterInfoTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucRasterInfoTree1.Location = new System.Drawing.Point(0, 0);
            this.ucRasterInfoTree1.Name = "ucRasterInfoTree1";
            this.ucRasterInfoTree1.Size = new System.Drawing.Size(255, 275);
            this.ucRasterInfoTree1.TabIndex = 2;
            // 
            // ucSelectBandForRgb1
            // 
            this.ucSelectBandForRgb1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucSelectBandForRgb1.Location = new System.Drawing.Point(0, 0);
            this.ucSelectBandForRgb1.Name = "ucSelectBandForRgb1";
            this.ucSelectBandForRgb1.Size = new System.Drawing.Size(255, 180);
            this.ucSelectBandForRgb1.TabIndex = 4;
            this.ucSelectBandForRgb1.Load += new System.EventHandler(this.ucSelectBandForRgb1_Load);
            // 
            // RasterPropertyWndContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucRasterInfoTree1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "RasterPropertyWndContent";
            this.Size = new System.Drawing.Size(255, 500);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private UCRasterInfoTree ucRasterInfoTree1;
        private UCSelectBandForRgb ucSelectBandForRgb1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
    }
}
