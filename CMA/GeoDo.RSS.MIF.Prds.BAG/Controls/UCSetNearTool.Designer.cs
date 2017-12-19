namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCSetNearTool
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
            this.txtNear = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtpa = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtpb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NDSIPanel = new System.Windows.Forms.Panel();
            this.NDVIMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.txtnearmin = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ckbone = new System.Windows.Forms.CheckBox();
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
            this.btnNDVI.Click += new System.EventHandler(this.btnNear_Click);
            // 
            // txtNear
            // 
            this.txtNear.Location = new System.Drawing.Point(102, 9);
            this.txtNear.Name = "txtNear";
            this.txtNear.Size = new System.Drawing.Size(139, 21);
            this.txtNear.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "近红外水体阈值a：";
            // 
            // txtpa
            // 
            this.txtpa.Location = new System.Drawing.Point(102, 42);
            this.txtpa.Name = "txtpa";
            this.txtpa.Size = new System.Drawing.Size(139, 21);
            this.txtpa.TabIndex = 9;
            this.txtpa.Text = "0.5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "蓝藻阈值参数 b：";
            // 
            // txtpb
            // 
            this.txtpb.Location = new System.Drawing.Point(102, 78);
            this.txtpb.Name = "txtpb";
            this.txtpb.Size = new System.Drawing.Size(139, 21);
            this.txtpb.TabIndex = 11;
            this.txtpb.Text = "-0.03";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "蓝藻阈值参数 c：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "近红外阈值低端值：";
            // 
            // NDSIPanel
            // 
            this.NDSIPanel.Controls.Add(this.NDVIMultiBar);
            this.NDSIPanel.Controls.Add(this.txtnearmin);
            this.NDSIPanel.Controls.Add(this.label8);
            this.NDSIPanel.Controls.Add(this.label6);
            this.NDSIPanel.Location = new System.Drawing.Point(4, 140);
            this.NDSIPanel.Name = "NDSIPanel";
            this.NDSIPanel.Size = new System.Drawing.Size(281, 62);
            this.NDSIPanel.TabIndex = 14;
            // 
            // NDVIMultiBar
            // 
            this.NDVIMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NDVIMultiBar.BarItemCount = 1;
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
            this.NDVIMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.NDVIMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.NDVIMultiBar.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.NDVIMultiBar_BarValueChanged);
            this.NDVIMultiBar.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.NDVIMultiBar_BarValueChangedFinished);
            // 
            // txtnearmin
            // 
            this.txtnearmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtnearmin.DefaultValue = 0D;
            this.txtnearmin.FineTuning = 1D;
            this.txtnearmin.Location = new System.Drawing.Point(113, 9);
            this.txtnearmin.Name = "txtnearmin";
            this.txtnearmin.Size = new System.Drawing.Size(124, 21);
            this.txtnearmin.TabIndex = 1;
            this.txtnearmin.Text = "0";
            this.txtnearmin.Value = 0D;
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
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "近红外反射率：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label7.Location = new System.Drawing.Point(122, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "a+（b+c）/10";
            // 
            // ckbone
            // 
            this.ckbone.AutoSize = true;
            this.ckbone.Location = new System.Drawing.Point(247, 60);
            this.ckbone.Name = "ckbone";
            this.ckbone.Size = new System.Drawing.Size(15, 14);
            this.ckbone.TabIndex = 17;
            this.ckbone.UseVisualStyleBackColor = true;
            this.ckbone.Visible = false;
            // 
            // UCSetNearTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbone);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.NDSIPanel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtpb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtpa);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnNDVI);
            this.Controls.Add(this.txtNear);
            this.Controls.Add(this.label1);
            this.Name = "UCSetNearTool";
            this.Size = new System.Drawing.Size(291, 216);
            this.NDSIPanel.ResumeLayout(false);
            this.NDSIPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNDVI;
        private System.Windows.Forms.TextBox txtNear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtpa;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtpb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel NDSIPanel;
        private UI.MultiBarTrack NDVIMultiBar;
        public  UI.DoubleTextBox txtnearmin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.CheckBox ckbone;
    }
}
