namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCSetNDVITool
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
            this.btnNDVI = new System.Windows.Forms.Button();
            this.txtNDVI = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txta = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtminndvi = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NDSIPanel = new System.Windows.Forms.Panel();
            this.txtndvimax = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.NDVIMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.label5 = new System.Windows.Forms.Label();
            this.txtndvimin = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ckbaoi = new System.Windows.Forms.CheckBox();
            this.NDSIPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNDVI
            // 
            this.btnNDVI.Location = new System.Drawing.Point(247, 9);
            this.btnNDVI.Name = "btnNDVI";
            this.btnNDVI.Size = new System.Drawing.Size(40, 23);
            this.btnNDVI.TabIndex = 7;
            this.btnNDVI.Text = "获取";
            this.btnNDVI.UseVisualStyleBackColor = true;
            this.btnNDVI.Click += new System.EventHandler(this.btnGetAOIIndex);
            // 
            // txtNDVI
            // 
            this.txtNDVI.Location = new System.Drawing.Point(102, 9);
            this.txtNDVI.Name = "txtNDVI";
            this.txtNDVI.Size = new System.Drawing.Size(139, 21);
            this.txtNDVI.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "NDVI水体背景值：";
            // 
            // txta
            // 
            this.txta.Location = new System.Drawing.Point(102, 42);
            this.txta.Name = "txta";
            this.txta.Size = new System.Drawing.Size(139, 21);
            this.txta.TabIndex = 9;
            this.txta.Text = "0.3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "判识阈值参数 a：";
            // 
            // txtb
            // 
            this.txtb.Location = new System.Drawing.Point(102, 78);
            this.txtb.Name = "txtb";
            this.txtb.Size = new System.Drawing.Size(139, 21);
            this.txtb.TabIndex = 11;
            this.txtb.Text = "-0.04";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "判识阈值参数 b：";
            // 
            // txtminndvi
            // 
            this.txtminndvi.Location = new System.Drawing.Point(101, 111);
            this.txtminndvi.Name = "txtminndvi";
            this.txtminndvi.Size = new System.Drawing.Size(139, 21);
            this.txtminndvi.TabIndex = 13;
            this.txtminndvi.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "NDVI阈值低端值：";
            // 
            // NDSIPanel
            // 
            this.NDSIPanel.Controls.Add(this.txtndvimax);
            this.NDSIPanel.Controls.Add(this.NDVIMultiBar);
            this.NDSIPanel.Controls.Add(this.label5);
            this.NDSIPanel.Controls.Add(this.txtndvimin);
            this.NDSIPanel.Controls.Add(this.label8);
            this.NDSIPanel.Controls.Add(this.label6);
            this.NDSIPanel.Location = new System.Drawing.Point(4, 140);
            this.NDSIPanel.Name = "NDSIPanel";
            this.NDSIPanel.Size = new System.Drawing.Size(281, 62);
            this.NDSIPanel.TabIndex = 14;
            // 
            // txtndvimax
            // 
            this.txtndvimax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtndvimax.DefaultValue = 0D;
            this.txtndvimax.FineTuning = 1D;
            this.txtndvimax.Location = new System.Drawing.Point(204, 9);
            this.txtndvimax.Name = "txtndvimax";
            this.txtndvimax.Size = new System.Drawing.Size(66, 21);
            this.txtndvimax.TabIndex = 3;
            this.txtndvimax.Text = "0.81";
            this.txtndvimax.Value = 0.81D;
            // 
            // NDVIMultiBar
            // 
            this.NDVIMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NDVIMultiBar.BarItemCount = 2;
            this.NDVIMultiBar.Location = new System.Drawing.Point(0, 29);
            this.NDVIMultiBar.MaxEndPointValue = 1D;
            this.NDVIMultiBar.MinEndPointValue = -1D;
            this.NDVIMultiBar.MinSpan = 6;
            this.NDVIMultiBar.Name = "NDVIMultiBar";
            this.NDVIMultiBar.Size = new System.Drawing.Size(278, 33);
            this.NDVIMultiBar.TabIndex = 10;
            this.NDVIMultiBar.TrackLineBorderColor = System.Drawing.Color.Black;
            this.NDVIMultiBar.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.NDVIMultiBar.TrackLineHeight = 8;
            this.NDVIMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.MiddleSegment;
            this.NDVIMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.NDVIMultiBar.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.NDVIMultiBar_BarValueChanged);
            this.NDVIMultiBar.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.NDVIMultiBar_BarValueChangedFinished);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(184, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "<";
            // 
            // txtndvimin
            // 
            this.txtndvimin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtndvimin.DefaultValue = 0D;
            this.txtndvimin.FineTuning = 1D;
            this.txtndvimin.Location = new System.Drawing.Point(113, 9);
            this.txtndvimin.Name = "txtndvimin";
            this.txtndvimin.Size = new System.Drawing.Size(66, 21);
            this.txtndvimin.TabIndex = 1;
            this.txtndvimin.Text = "0";
            this.txtndvimin.Value = 0D;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(90, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = ">=";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "NDVI阈值范围 ：";
            // 
            // ckbaoi
            // 
            this.ckbaoi.AutoSize = true;
            this.ckbaoi.Location = new System.Drawing.Point(247, 44);
            this.ckbaoi.Name = "ckbaoi";
            this.ckbaoi.Size = new System.Drawing.Size(15, 14);
            this.ckbaoi.TabIndex = 15;
            this.ckbaoi.UseVisualStyleBackColor = true;
            this.ckbaoi.Visible = false;
            // 
            // UCSetNDVITool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbaoi);
            this.Controls.Add(this.NDSIPanel);
            this.Controls.Add(this.txtminndvi);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txta);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnNDVI);
            this.Controls.Add(this.txtNDVI);
            this.Controls.Add(this.label1);
            this.Name = "UCSetNDVITool";
            this.Size = new System.Drawing.Size(291, 210);
            this.NDSIPanel.ResumeLayout(false);
            this.NDSIPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNDVI;
        private System.Windows.Forms.TextBox txtNDVI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtminndvi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel NDSIPanel;
        public UI.DoubleTextBox txtndvimax;
        private UI.MultiBarTrack NDVIMultiBar;
        private System.Windows.Forms.Label label5;
        public  UI.DoubleTextBox txtndvimin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox ckbaoi;
    }
}
