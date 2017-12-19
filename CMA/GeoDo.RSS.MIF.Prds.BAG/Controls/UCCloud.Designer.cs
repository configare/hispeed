namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCCloud
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
            this.mbnsvi = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.txtnsvi = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.pane_NSVI = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.txtmaxndvi = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.mbkndvi = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.VisiableMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.NDSIPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtnearinfrared = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.near = new System.Windows.Forms.Label();
            this.bmknearinfrared = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.farInfraredPanel = new System.Windows.Forms.Panel();
            this.txtvisiable = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.VisiablePanel = new System.Windows.Forms.Panel();
            this.mbkvisiable = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.pane_NSVI.SuspendLayout();
            this.NDSIPanel.SuspendLayout();
            this.farInfraredPanel.SuspendLayout();
            this.VisiablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbnsvi
            // 
            this.mbnsvi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbnsvi.BarItemCount = 1;
            this.mbnsvi.Location = new System.Drawing.Point(3, 34);
            this.mbnsvi.MaxEndPointValue = 10D;
            this.mbnsvi.MinEndPointValue = 0D;
            this.mbnsvi.MinSpan = 6;
            this.mbnsvi.Name = "mbnsvi";
            this.mbnsvi.Size = new System.Drawing.Size(284, 29);
            this.mbnsvi.TabIndex = 15;
            this.mbnsvi.TrackLineBorderColor = System.Drawing.Color.Black;
            this.mbnsvi.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.mbnsvi.TrackLineHeight = 8;
            this.mbnsvi.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.LeftSegment;
            this.mbnsvi.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.mbnsvi.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.mbnsvi_BarValueChanged_1);
            this.mbnsvi.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.mbnsvi_BarValueChangedFinished);
            // 
            // txtnsvi
            // 
            this.txtnsvi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtnsvi.DefaultValue = 0D;
            this.txtnsvi.FineTuning = 1D;
            this.txtnsvi.Location = new System.Drawing.Point(213, 9);
            this.txtnsvi.Name = "txtnsvi";
            this.txtnsvi.Size = new System.Drawing.Size(66, 21);
            this.txtnsvi.TabIndex = 12;
            this.txtnsvi.Text = "2";
            this.txtnsvi.Value = 2D;
            this.txtnsvi.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtnsvi_KeyUp_1);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 12);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 12);
            this.label13.TabIndex = 11;
            this.label13.Text = "近红外/短波红外";
            // 
            // pane_NSVI
            // 
            this.pane_NSVI.Controls.Add(this.mbnsvi);
            this.pane_NSVI.Controls.Add(this.txtnsvi);
            this.pane_NSVI.Controls.Add(this.label12);
            this.pane_NSVI.Controls.Add(this.label13);
            this.pane_NSVI.Dock = System.Windows.Forms.DockStyle.Top;
            this.pane_NSVI.Location = new System.Drawing.Point(0, 137);
            this.pane_NSVI.Name = "pane_NSVI";
            this.pane_NSVI.Size = new System.Drawing.Size(290, 66);
            this.pane_NSVI.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(190, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 11;
            this.label12.Text = "<=";
            // 
            // txtmaxndvi
            // 
            this.txtmaxndvi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtmaxndvi.DefaultValue = 0D;
            this.txtmaxndvi.FineTuning = 1D;
            this.txtmaxndvi.Location = new System.Drawing.Point(213, 9);
            this.txtmaxndvi.Name = "txtmaxndvi";
            this.txtmaxndvi.Size = new System.Drawing.Size(66, 21);
            this.txtmaxndvi.TabIndex = 3;
            this.txtmaxndvi.Text = "-0.05";
            this.txtmaxndvi.Value = -0.05D;
            this.txtmaxndvi.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtmaxndvi_KeyUp);
            // 
            // mbkndvi
            // 
            this.mbkndvi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbkndvi.BarItemCount = 1;
            this.mbkndvi.Location = new System.Drawing.Point(0, 28);
            this.mbkndvi.MaxEndPointValue = 1D;
            this.mbkndvi.MinEndPointValue = -1D;
            this.mbkndvi.MinSpan = 6;
            this.mbkndvi.Name = "mbkndvi";
            this.mbkndvi.Size = new System.Drawing.Size(290, 40);
            this.mbkndvi.TabIndex = 10;
            this.mbkndvi.TrackLineBorderColor = System.Drawing.Color.Black;
            this.mbkndvi.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.mbkndvi.TrackLineHeight = 8;
            this.mbkndvi.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.mbkndvi.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.mbkndvi.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.mbkndvi_BarValueChanged);
            this.mbkndvi.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.mbkndvi_BarValueChangedFinished);
            // 
            // VisiableMultiBar
            // 
            this.VisiableMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VisiableMultiBar.BarItemCount = 1;
            this.VisiableMultiBar.Location = new System.Drawing.Point(-70, -59);
            this.VisiableMultiBar.MaxEndPointValue = 100D;
            this.VisiableMultiBar.MinEndPointValue = 0D;
            this.VisiableMultiBar.MinSpan = 6;
            this.VisiableMultiBar.Name = "VisiableMultiBar";
            this.VisiableMultiBar.Size = new System.Drawing.Size(430, 29);
            this.VisiableMultiBar.TabIndex = 15;
            this.VisiableMultiBar.TrackLineBorderColor = System.Drawing.Color.Black;
            this.VisiableMultiBar.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.VisiableMultiBar.TrackLineHeight = 8;
            this.VisiableMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.VisiableMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            // 
            // NDSIPanel
            // 
            this.NDSIPanel.Controls.Add(this.label2);
            this.NDSIPanel.Controls.Add(this.txtmaxndvi);
            this.NDSIPanel.Controls.Add(this.mbkndvi);
            this.NDSIPanel.Controls.Add(this.label3);
            this.NDSIPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.NDSIPanel.Location = new System.Drawing.Point(0, 206);
            this.NDSIPanel.Name = "NDSIPanel";
            this.NDSIPanel.Size = new System.Drawing.Size(290, 62);
            this.NDSIPanel.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = ">=";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = " NDVI  ";
            // 
            // txtnearinfrared
            // 
            this.txtnearinfrared.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtnearinfrared.DefaultValue = 0D;
            this.txtnearinfrared.FineTuning = 1D;
            this.txtnearinfrared.Location = new System.Drawing.Point(213, 9);
            this.txtnearinfrared.Name = "txtnearinfrared";
            this.txtnearinfrared.Size = new System.Drawing.Size(66, 21);
            this.txtnearinfrared.TabIndex = 1;
            this.txtnearinfrared.Text = "15";
            this.txtnearinfrared.Value = 15D;
            this.txtnearinfrared.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtnearinfrared_KeyUp);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(193, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = ">=";
            // 
            // near
            // 
            this.near.AutoSize = true;
            this.near.Location = new System.Drawing.Point(3, 12);
            this.near.Name = "near";
            this.near.Size = new System.Drawing.Size(47, 12);
            this.near.TabIndex = 0;
            this.near.Text = "近红外 ";
            // 
            // bmknearinfrared
            // 
            this.bmknearinfrared.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bmknearinfrared.BarItemCount = 1;
            this.bmknearinfrared.Location = new System.Drawing.Point(0, 29);
            this.bmknearinfrared.MaxEndPointValue = 100D;
            this.bmknearinfrared.MinEndPointValue = 0D;
            this.bmknearinfrared.MinSpan = 6;
            this.bmknearinfrared.Name = "bmknearinfrared";
            this.bmknearinfrared.Size = new System.Drawing.Size(290, 48);
            this.bmknearinfrared.TabIndex = 9;
            this.bmknearinfrared.TrackLineBorderColor = System.Drawing.Color.Black;
            this.bmknearinfrared.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.bmknearinfrared.TrackLineHeight = 8;
            this.bmknearinfrared.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.bmknearinfrared.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.bmknearinfrared.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.bmknearinfrared_BarValueChanged);
            this.bmknearinfrared.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.bmknearinfrared_BarValueChangedFinished);
            // 
            // farInfraredPanel
            // 
            this.farInfraredPanel.Controls.Add(this.txtnearinfrared);
            this.farInfraredPanel.Controls.Add(this.label9);
            this.farInfraredPanel.Controls.Add(this.near);
            this.farInfraredPanel.Controls.Add(this.bmknearinfrared);
            this.farInfraredPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.farInfraredPanel.Location = new System.Drawing.Point(0, 69);
            this.farInfraredPanel.Name = "farInfraredPanel";
            this.farInfraredPanel.Size = new System.Drawing.Size(290, 68);
            this.farInfraredPanel.TabIndex = 18;
            // 
            // txtvisiable
            // 
            this.txtvisiable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtvisiable.DefaultValue = 0D;
            this.txtvisiable.FineTuning = 1D;
            this.txtvisiable.Location = new System.Drawing.Point(213, 9);
            this.txtvisiable.Name = "txtvisiable";
            this.txtvisiable.Size = new System.Drawing.Size(66, 21);
            this.txtvisiable.TabIndex = 1;
            this.txtvisiable.Text = "30";
            this.txtvisiable.Value = 30D;
            this.txtvisiable.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtvisiable_KeyUp);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(193, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = ">=";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "可见光 ";
            // 
            // VisiablePanel
            // 
            this.VisiablePanel.Controls.Add(this.mbkvisiable);
            this.VisiablePanel.Controls.Add(this.txtvisiable);
            this.VisiablePanel.Controls.Add(this.label10);
            this.VisiablePanel.Controls.Add(this.label1);
            this.VisiablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.VisiablePanel.Location = new System.Drawing.Point(0, 0);
            this.VisiablePanel.Name = "VisiablePanel";
            this.VisiablePanel.Size = new System.Drawing.Size(290, 69);
            this.VisiablePanel.TabIndex = 16;
            // 
            // mbkvisiable
            // 
            this.mbkvisiable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mbkvisiable.BarItemCount = 1;
            this.mbkvisiable.Location = new System.Drawing.Point(0, 36);
            this.mbkvisiable.MaxEndPointValue = 100D;
            this.mbkvisiable.MinEndPointValue = 0D;
            this.mbkvisiable.MinSpan = 6;
            this.mbkvisiable.Name = "mbkvisiable";
            this.mbkvisiable.Size = new System.Drawing.Size(287, 33);
            this.mbkvisiable.TabIndex = 9;
            this.mbkvisiable.TrackLineBorderColor = System.Drawing.Color.Black;
            this.mbkvisiable.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.mbkvisiable.TrackLineHeight = 8;
            this.mbkvisiable.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.mbkvisiable.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.mbkvisiable.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.mbkvisiable_BarValueChanged);
            this.mbkvisiable.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.mbkvisiable_BarValueChangedFinished);
            // 
            // UCCloud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pane_NSVI);
            this.Controls.Add(this.VisiableMultiBar);
            this.Controls.Add(this.NDSIPanel);
            this.Controls.Add(this.farInfraredPanel);
            this.Controls.Add(this.VisiablePanel);
            this.Name = "UCCloud";
            this.Size = new System.Drawing.Size(290, 268);
            this.pane_NSVI.ResumeLayout(false);
            this.pane_NSVI.PerformLayout();
            this.NDSIPanel.ResumeLayout(false);
            this.NDSIPanel.PerformLayout();
            this.farInfraredPanel.ResumeLayout(false);
            this.farInfraredPanel.PerformLayout();
            this.VisiablePanel.ResumeLayout(false);
            this.VisiablePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UI.MultiBarTrack mbnsvi;
        private UI.DoubleTextBox txtnsvi;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel pane_NSVI;
        private System.Windows.Forms.Label label12;
        private UI.DoubleTextBox txtmaxndvi;
        private UI.MultiBarTrack mbkndvi;
        private UI.MultiBarTrack VisiableMultiBar;
        private System.Windows.Forms.Panel NDSIPanel;
        private System.Windows.Forms.Label label3;
        private UI.DoubleTextBox txtnearinfrared;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label near;
        private UI.MultiBarTrack bmknearinfrared;
        private System.Windows.Forms.Panel farInfraredPanel;
        private UI.DoubleTextBox txtvisiable;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel VisiablePanel;
        private UI.MultiBarTrack mbkvisiable;
        private System.Windows.Forms.Label label2;
    }
}
