namespace  GeoDo.RSS.CA
{
    partial class DblBarTrackWithBoxs
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
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.multiBarTrack1 = new MultiBarTrack();
            this.txtCaption = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(3, 6);
            this.txtMinValue.MaxLength = 5;
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(50, 21);
            this.txtMinValue.TabIndex = 1;
            this.txtMinValue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtMinValue_MouseDoubleClick);
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(67, 6);
            this.txtMaxValue.MaxLength = 5;
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(50, 21);
            this.txtMaxValue.TabIndex = 2;
            this.txtMaxValue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtMaxValue_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "~";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            this.label1.DoubleClick += new System.EventHandler(this.label1_DoubleClick);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.pictureBox1);
            this.panelRight.Controls.Add(this.txtMinValue);
            this.panelRight.Controls.Add(this.label1);
            this.panelRight.Controls.Add(this.txtMaxValue);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(259, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(120, 33);
            this.panelRight.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(129, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(28, 20);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.txtCaption);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(68, 33);
            this.panelLeft.TabIndex = 5;
            this.panelLeft.DoubleClick += new System.EventHandler(this.panelLeft_DoubleClick);
            // 
            // txtCaption
            // 
            this.txtCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCaption.Location = new System.Drawing.Point(0, 0);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(68, 33);
            this.txtCaption.TabIndex = 0;
            this.txtCaption.Text = "...";
            this.txtCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.multiBarTrack1);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(68, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(191, 33);
            this.panelCenter.TabIndex = 6;
            // 
            // multiBarTrack1
            // 
            this.multiBarTrack1.BarItemCount = 2;
            this.multiBarTrack1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiBarTrack1.Location = new System.Drawing.Point(0, 0);
            this.multiBarTrack1.MaxEndPointValue = 100D;
            this.multiBarTrack1.MinEndPointValue = 0D;
            this.multiBarTrack1.MinSpan = 6;
            this.multiBarTrack1.Name = "multiBarTrack1";
            this.multiBarTrack1.Size = new System.Drawing.Size(191, 33);
            this.multiBarTrack1.TabIndex = 0;
            this.multiBarTrack1.TrackLineColor = System.Drawing.Color.Gray;
            this.multiBarTrack1.DoubleClick += new System.EventHandler(this.multiBarTrack1_DoubleClick);
            this.multiBarTrack1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.multiBarTrack1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // DblBarTrackWithBoxs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelRight);
            this.Name = "DblBarTrackWithBoxs";
            this.Size = new System.Drawing.Size(379, 33);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label txtCaption;
        private System.Windows.Forms.Panel panelCenter;
        private MultiBarTrack multiBarTrack1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
