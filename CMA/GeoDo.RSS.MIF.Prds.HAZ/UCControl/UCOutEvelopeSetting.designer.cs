namespace GeoDo.RSS.MIF.Prds.HAZ
{
    partial class UCOutEvelopeSetting
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
            this.NDSIPanel = new System.Windows.Forms.Panel();
            this.lonMaxTextBox = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.OutLonMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.label4 = new System.Windows.Forms.Label();
            this.lonMinTextBox = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.VisiableMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.nearVisiablePanel = new System.Windows.Forms.Panel();
            this.latMaxTextBox = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.OutLatMultiBar = new GeoDo.RSS.MIF.UI.MultiBarTrack();
            this.label5 = new System.Windows.Forms.Label();
            this.latMinTextBox = new GeoDo.RSS.MIF.UI.DoubleTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.NDSIPanel.SuspendLayout();
            this.nearVisiablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NDSIPanel
            // 
            this.NDSIPanel.Controls.Add(this.lonMaxTextBox);
            this.NDSIPanel.Controls.Add(this.OutLonMultiBar);
            this.NDSIPanel.Controls.Add(this.label4);
            this.NDSIPanel.Controls.Add(this.lonMinTextBox);
            this.NDSIPanel.Controls.Add(this.label8);
            this.NDSIPanel.Controls.Add(this.label3);
            this.NDSIPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NDSIPanel.Location = new System.Drawing.Point(0, 0);
            this.NDSIPanel.Name = "NDSIPanel";
            this.NDSIPanel.Size = new System.Drawing.Size(290, 62);
            this.NDSIPanel.TabIndex = 12;
            // 
            // lonMaxTextBox
            // 
            this.lonMaxTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lonMaxTextBox.DefaultValue = 0D;
            this.lonMaxTextBox.FineTuning = 1D;
            this.lonMaxTextBox.Location = new System.Drawing.Point(213, 9);
            this.lonMaxTextBox.Name = "lonMaxTextBox";
            this.lonMaxTextBox.Size = new System.Drawing.Size(66, 21);
            this.lonMaxTextBox.TabIndex = 3;
            this.lonMaxTextBox.Text = "0";
            this.lonMaxTextBox.Value = 0D;
            this.lonMaxTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LonMaxTextBox_KeyUp);
            // 
            // OutLonMultiBar
            // 
            this.OutLonMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutLonMultiBar.BarItemCount = 2;
            this.OutLonMultiBar.Location = new System.Drawing.Point(0, 28);
            this.OutLonMultiBar.MaxEndPointValue = 180D;
            this.OutLonMultiBar.MinEndPointValue = -180D;
            this.OutLonMultiBar.MinSpan = 6;
            this.OutLonMultiBar.Name = "OutLonMultiBar";
            this.OutLonMultiBar.Size = new System.Drawing.Size(290, 40);
            this.OutLonMultiBar.TabIndex = 10;
            this.OutLonMultiBar.TrackLineBorderColor = System.Drawing.Color.Black;
            this.OutLonMultiBar.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.OutLonMultiBar.TrackLineHeight = 8;
            this.OutLonMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.MiddleSegment;
            this.OutLonMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.OutLonMultiBar.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.OutLonMultiBar_BarValueChanged);
            this.OutLonMultiBar.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.MultiBar_BarValueChangedFinished);
            this.OutLonMultiBar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pairTrack_MouseClick);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(193, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "<";
            // 
            // lonMinTextBox
            // 
            this.lonMinTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lonMinTextBox.DefaultValue = 0D;
            this.lonMinTextBox.FineTuning = 1D;
            this.lonMinTextBox.Location = new System.Drawing.Point(122, 9);
            this.lonMinTextBox.Name = "lonMinTextBox";
            this.lonMinTextBox.Size = new System.Drawing.Size(66, 21);
            this.lonMinTextBox.TabIndex = 1;
            this.lonMinTextBox.Text = "0";
            this.lonMinTextBox.Value = 0D;
            this.lonMinTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LonMinTextBox_KeyUp);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(99, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = ">=";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "输出经度范围";
            // 
            // VisiableMultiBar
            // 
            this.VisiableMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VisiableMultiBar.BarItemCount = 1;
            this.VisiableMultiBar.Location = new System.Drawing.Point(0, 28);
            this.VisiableMultiBar.MaxEndPointValue = 100D;
            this.VisiableMultiBar.MinEndPointValue = 0D;
            this.VisiableMultiBar.MinSpan = 6;
            this.VisiableMultiBar.Name = "VisiableMultiBar";
            this.VisiableMultiBar.Size = new System.Drawing.Size(290, 48);
            this.VisiableMultiBar.TabIndex = 8;
            this.VisiableMultiBar.TrackLineBorderColor = System.Drawing.Color.Black;
            this.VisiableMultiBar.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.VisiableMultiBar.TrackLineHeight = 8;
            this.VisiableMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.RightSegment;
            this.VisiableMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.VisiableMultiBar.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.OutLatMultiBar_BarValueChanged);
            this.VisiableMultiBar.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.MultiBar_BarValueChangedFinished);
            this.VisiableMultiBar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pairTrack_MouseClick);
            // 
            // nearVisiablePanel
            // 
            this.nearVisiablePanel.Controls.Add(this.latMaxTextBox);
            this.nearVisiablePanel.Controls.Add(this.OutLatMultiBar);
            this.nearVisiablePanel.Controls.Add(this.label5);
            this.nearVisiablePanel.Controls.Add(this.latMinTextBox);
            this.nearVisiablePanel.Controls.Add(this.label7);
            this.nearVisiablePanel.Controls.Add(this.label6);
            this.nearVisiablePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.nearVisiablePanel.Location = new System.Drawing.Point(0, 62);
            this.nearVisiablePanel.Name = "nearVisiablePanel";
            this.nearVisiablePanel.Size = new System.Drawing.Size(290, 66);
            this.nearVisiablePanel.TabIndex = 13;
            // 
            // latMaxTextBox
            // 
            this.latMaxTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.latMaxTextBox.DefaultValue = 0D;
            this.latMaxTextBox.FineTuning = 1D;
            this.latMaxTextBox.Location = new System.Drawing.Point(213, 9);
            this.latMaxTextBox.Name = "latMaxTextBox";
            this.latMaxTextBox.Size = new System.Drawing.Size(66, 21);
            this.latMaxTextBox.TabIndex = 14;
            this.latMaxTextBox.Text = "0";
            this.latMaxTextBox.Value = 0D;
            this.latMaxTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LatMaxTextBox_KeyUp);
            // 
            // OutLatMultiBar
            // 
            this.OutLatMultiBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutLatMultiBar.BarItemCount = 2;
            this.OutLatMultiBar.Location = new System.Drawing.Point(0, 29);
            this.OutLatMultiBar.MaxEndPointValue = 90D;
            this.OutLatMultiBar.MinEndPointValue = -90D;
            this.OutLatMultiBar.MinSpan = 6;
            this.OutLatMultiBar.Name = "OutLatMultiBar";
            this.OutLatMultiBar.Size = new System.Drawing.Size(290, 40);
            this.OutLatMultiBar.TabIndex = 15;
            this.OutLatMultiBar.TrackLineBorderColor = System.Drawing.Color.Black;
            this.OutLatMultiBar.TrackLineColor = System.Drawing.SystemColors.ActiveCaption;
            this.OutLatMultiBar.TrackLineHeight = 8;
            this.OutLatMultiBar.ValidPartion = GeoDo.RSS.MIF.UI.MultiBarTrack.enumValidPartion.MiddleSegment;
            this.OutLatMultiBar.ValidSegmentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.OutLatMultiBar.BarValueChanged += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedHandler(this.OutLatMultiBar_BarValueChanged);
            this.OutLatMultiBar.BarValueChangedFinished += new GeoDo.RSS.MIF.UI.MultiBarTrack.BarValueChangedFinishedHandler(this.nearVisiableMultiBar_BarValueChangedFinished);
            this.OutLatMultiBar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pairTrack_MouseClick);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(193, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "<";
            // 
            // latMinTextBox
            // 
            this.latMinTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.latMinTextBox.DefaultValue = 0D;
            this.latMinTextBox.FineTuning = 1D;
            this.latMinTextBox.Location = new System.Drawing.Point(122, 9);
            this.latMinTextBox.Name = "latMinTextBox";
            this.latMinTextBox.Size = new System.Drawing.Size(66, 21);
            this.latMinTextBox.TabIndex = 12;
            this.latMinTextBox.Text = "0";
            this.latMinTextBox.Value = 0D;
            this.latMinTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LatMinTextBox_KeyUp);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(99, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = ">=";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "输出纬度范围";
            // 
            // UCOutEvelopeSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nearVisiablePanel);
            this.Controls.Add(this.NDSIPanel);
            this.Controls.Add(this.VisiableMultiBar);
            this.Name = "UCOutEvelopeSetting";
            this.Size = new System.Drawing.Size(290, 130);
            this.NDSIPanel.ResumeLayout(false);
            this.NDSIPanel.PerformLayout();
            this.nearVisiablePanel.ResumeLayout(false);
            this.nearVisiablePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UI.MultiBarTrack VisiableMultiBar;
        private UI.MultiBarTrack OutLonMultiBar;
        private System.Windows.Forms.Panel NDSIPanel;
        private UI.DoubleTextBox lonMaxTextBox;
        private System.Windows.Forms.Label label4;
        private UI.DoubleTextBox lonMinTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel nearVisiablePanel;
        private UI.DoubleTextBox latMaxTextBox;
        private UI.MultiBarTrack OutLatMultiBar;
        private System.Windows.Forms.Label label5;
        private UI.DoubleTextBox latMinTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
    }
}