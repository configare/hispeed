namespace GeoDo.RSS.CA
{
    partial class frmExponentEnhanceII
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dblBarB = new GeoDo.RSS.CA.DblBarTrackWithBoxs();
            this.dblBarG = new GeoDo.RSS.CA.DblBarTrackWithBoxs();
            this.dblBarR = new GeoDo.RSS.CA.DblBarTrackWithBoxs();
            this.rdSingleChannel = new System.Windows.Forms.RadioButton();
            this.rdFullChannel = new System.Windows.Forms.RadioButton();
            this.labelBlue = new System.Windows.Forms.Label();
            this.labelGreen = new System.Windows.Forms.Label();
            this.labelRed = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(370, 39);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(370, 10);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(370, 68);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(370, 107);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dblBarB);
            this.groupBox2.Controls.Add(this.dblBarG);
            this.groupBox2.Controls.Add(this.dblBarR);
            this.groupBox2.Controls.Add(this.rdSingleChannel);
            this.groupBox2.Controls.Add(this.rdFullChannel);
            this.groupBox2.Controls.Add(this.labelBlue);
            this.groupBox2.Controls.Add(this.labelGreen);
            this.groupBox2.Controls.Add(this.labelRed);
            this.groupBox2.Location = new System.Drawing.Point(12, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 201);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数";
            // 
            // dblBarB
            // 
            this.dblBarB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dblBarB.Caption = "";
            this.dblBarB.Location = new System.Drawing.Point(43, 162);
            this.dblBarB.MaxEndPointValue = 255D;
            this.dblBarB.MaxLengthMaxValue = 5;
            this.dblBarB.MaxLengthMinValue = 5;
            this.dblBarB.MaxValue = 54.762D;
            this.dblBarB.MinEndPointValue = 0D;
            this.dblBarB.MinSpan = 0;
            this.dblBarB.MinValue = 21.429D;
            this.dblBarB.Name = "dblBarB";
            this.dblBarB.Size = new System.Drawing.Size(303, 33);
            this.dblBarB.TabIndex = 30;
            this.dblBarB.TrackLineFillColor = System.Drawing.Color.Gray;
            this.dblBarB.EndValueChanged += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedHandler(this.dblBarB_EndValueChanged);
            this.dblBarB.BarValueChangedFinished += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedFinishedHandler(this.dblBarB_BarValueChangedFinished);
            // 
            // dblBarG
            // 
            this.dblBarG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dblBarG.Caption = "";
            this.dblBarG.Location = new System.Drawing.Point(43, 113);
            this.dblBarG.MaxEndPointValue = 255D;
            this.dblBarG.MaxLengthMaxValue = 5;
            this.dblBarG.MaxLengthMinValue = 5;
            this.dblBarG.MaxValue = 54.762D;
            this.dblBarG.MinEndPointValue = 0D;
            this.dblBarG.MinSpan = 0;
            this.dblBarG.MinValue = 21.429D;
            this.dblBarG.Name = "dblBarG";
            this.dblBarG.Size = new System.Drawing.Size(303, 33);
            this.dblBarG.TabIndex = 30;
            this.dblBarG.TrackLineFillColor = System.Drawing.Color.Gray;
            this.dblBarG.EndValueChanged += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedHandler(this.dblBarG_EndValueChanged);
            this.dblBarG.BarValueChangedFinished += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedFinishedHandler(this.dblBarG_BarValueChangedFinished);
            // 
            // dblBarR
            // 
            this.dblBarR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dblBarR.Caption = "";
            this.dblBarR.Location = new System.Drawing.Point(43, 61);
            this.dblBarR.MaxEndPointValue = 255D;
            this.dblBarR.MaxLengthMaxValue = 5;
            this.dblBarR.MaxLengthMinValue = 5;
            this.dblBarR.MaxValue = 60.819D;
            this.dblBarR.MinEndPointValue = 0D;
            this.dblBarR.MinSpan = 0;
            this.dblBarR.MinValue = 27.485D;
            this.dblBarR.Name = "dblBarR";
            this.dblBarR.Size = new System.Drawing.Size(303, 33);
            this.dblBarR.TabIndex = 30;
            this.dblBarR.TrackLineFillColor = System.Drawing.Color.Gray;
            this.dblBarR.EndValueChanged += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedHandler(this.dblBarR_EndValueChanged);
            this.dblBarR.BarValueChangedFinished += new GeoDo.RSS.CA.DblBarTrackWithBoxs.BarValueChangedFinishedHandler(this.dblBarR_BarValueChangedFinished);
            // 
            // rdSingleChannel
            // 
            this.rdSingleChannel.AutoSize = true;
            this.rdSingleChannel.Location = new System.Drawing.Point(163, 21);
            this.rdSingleChannel.Name = "rdSingleChannel";
            this.rdSingleChannel.Size = new System.Drawing.Size(131, 16);
            this.rdSingleChannel.TabIndex = 29;
            this.rdSingleChannel.Text = "各通道分别设置参数";
            this.rdSingleChannel.UseVisualStyleBackColor = true;
            this.rdSingleChannel.CheckedChanged += new System.EventHandler(this.rdSingleChannel_CheckedChanged);
            // 
            // rdFullChannel
            // 
            this.rdFullChannel.AutoSize = true;
            this.rdFullChannel.Checked = true;
            this.rdFullChannel.Location = new System.Drawing.Point(24, 21);
            this.rdFullChannel.Name = "rdFullChannel";
            this.rdFullChannel.Size = new System.Drawing.Size(131, 16);
            this.rdFullChannel.TabIndex = 28;
            this.rdFullChannel.TabStop = true;
            this.rdFullChannel.Text = "各通道使用相同参数";
            this.rdFullChannel.UseVisualStyleBackColor = true;
            this.rdFullChannel.CheckedChanged += new System.EventHandler(this.rdFullChannel_CheckedChanged);
            // 
            // labelBlue
            // 
            this.labelBlue.AutoSize = true;
            this.labelBlue.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelBlue.ForeColor = System.Drawing.Color.Blue;
            this.labelBlue.Location = new System.Drawing.Point(19, 141);
            this.labelBlue.Name = "labelBlue";
            this.labelBlue.Size = new System.Drawing.Size(29, 25);
            this.labelBlue.TabIndex = 23;
            this.labelBlue.Text = "B:";
            // 
            // labelGreen
            // 
            this.labelGreen.AutoSize = true;
            this.labelGreen.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelGreen.ForeColor = System.Drawing.Color.Green;
            this.labelGreen.Location = new System.Drawing.Point(19, 97);
            this.labelGreen.Name = "labelGreen";
            this.labelGreen.Size = new System.Drawing.Size(31, 25);
            this.labelGreen.TabIndex = 18;
            this.labelGreen.Text = "G:";
            // 
            // labelRed
            // 
            this.labelRed.AutoSize = true;
            this.labelRed.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelRed.ForeColor = System.Drawing.Color.Red;
            this.labelRed.Location = new System.Drawing.Point(19, 51);
            this.labelRed.Name = "labelRed";
            this.labelRed.Size = new System.Drawing.Size(29, 25);
            this.labelRed.TabIndex = 6;
            this.labelRed.Text = "R:";
            // 
            // frmExponentEnhanceII
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 217);
            this.Controls.Add(this.groupBox2);
            this.KeyPreview = true;
            this.Name = "frmExponentEnhanceII";
            this.ShowIcon = false;
            this.Text = "指数增强II";
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelRed;
        private System.Windows.Forms.Label labelBlue;
        private System.Windows.Forms.Label labelGreen;
        private System.Windows.Forms.RadioButton rdSingleChannel;
        private System.Windows.Forms.RadioButton rdFullChannel;
        private DblBarTrackWithBoxs dblBarR;
        private DblBarTrackWithBoxs dblBarB;
        private DblBarTrackWithBoxs dblBarG;
    }
}