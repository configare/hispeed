namespace GeoDo.RSS.CA
{
    partial class frmReversalColorEditor
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
            this.btnAll = new System.Windows.Forms.Button();
            this.pleRgb = new System.Windows.Forms.Panel();
            this.rdiBlue = new System.Windows.Forms.CheckBox();
            this.rdiGreen = new System.Windows.Forms.CheckBox();
            this.rdiRed = new System.Windows.Forms.CheckBox();
            this.ck3BandView = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.pleRgb.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(255, 41);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(255, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(255, 70);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckPreviewing.Location = new System.Drawing.Point(255, 109);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnAll);
            this.groupBox2.Controls.Add(this.pleRgb);
            this.groupBox2.Location = new System.Drawing.Point(12, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 182);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数";
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(23, 139);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(75, 23);
            this.btnAll.TabIndex = 31;
            this.btnAll.Text = "全选";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // pleRgb
            // 
            this.pleRgb.Controls.Add(this.rdiBlue);
            this.pleRgb.Controls.Add(this.rdiGreen);
            this.pleRgb.Controls.Add(this.rdiRed);
            this.pleRgb.Location = new System.Drawing.Point(23, 18);
            this.pleRgb.Name = "pleRgb";
            this.pleRgb.Size = new System.Drawing.Size(183, 103);
            this.pleRgb.TabIndex = 30;
            // 
            // rdiBlue
            // 
            this.rdiBlue.AutoSize = true;
            this.rdiBlue.Checked = true;
            this.rdiBlue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rdiBlue.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.rdiBlue.ForeColor = System.Drawing.Color.Blue;
            this.rdiBlue.Location = new System.Drawing.Point(4, 73);
            this.rdiBlue.Name = "rdiBlue";
            this.rdiBlue.Size = new System.Drawing.Size(70, 29);
            this.rdiBlue.TabIndex = 2;
            this.rdiBlue.Text = "Blue";
            this.rdiBlue.UseVisualStyleBackColor = true;
            this.rdiBlue.CheckedChanged += new System.EventHandler(this.rdiBlue_CheckedChanged);
            // 
            // rdiGreen
            // 
            this.rdiGreen.AutoSize = true;
            this.rdiGreen.Checked = true;
            this.rdiGreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rdiGreen.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.rdiGreen.ForeColor = System.Drawing.Color.Green;
            this.rdiGreen.Location = new System.Drawing.Point(4, 38);
            this.rdiGreen.Name = "rdiGreen";
            this.rdiGreen.Size = new System.Drawing.Size(85, 29);
            this.rdiGreen.TabIndex = 1;
            this.rdiGreen.Text = "Green";
            this.rdiGreen.UseVisualStyleBackColor = true;
            this.rdiGreen.CheckedChanged += new System.EventHandler(this.rdiGreen_CheckedChanged);
            // 
            // rdiRed
            // 
            this.rdiRed.AutoSize = true;
            this.rdiRed.Checked = true;
            this.rdiRed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rdiRed.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.rdiRed.ForeColor = System.Drawing.Color.Red;
            this.rdiRed.Location = new System.Drawing.Point(4, 3);
            this.rdiRed.Name = "rdiRed";
            this.rdiRed.Size = new System.Drawing.Size(66, 29);
            this.rdiRed.TabIndex = 0;
            this.rdiRed.Text = "Red";
            this.rdiRed.UseVisualStyleBackColor = true;
            this.rdiRed.CheckedChanged += new System.EventHandler(this.rdiRed_CheckedChanged);
            // 
            // ck3BandView
            // 
            this.ck3BandView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ck3BandView.AutoSize = true;
            this.ck3BandView.Location = new System.Drawing.Point(255, 131);
            this.ck3BandView.Name = "ck3BandView";
            this.ck3BandView.Size = new System.Drawing.Size(84, 16);
            this.ck3BandView.TabIndex = 11;
            this.ck3BandView.Text = "分波段预览";
            this.ck3BandView.UseVisualStyleBackColor = true;
            this.ck3BandView.CheckedChanged += new System.EventHandler(this.ck3BandView_CheckedChanged);
            // 
            // frmReversalColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 191);
            this.Controls.Add(this.ck3BandView);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmReversalColorEditor";
            this.ShowIcon = false;
            this.Text = "反相";
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.ck3BandView, 0);
            this.groupBox2.ResumeLayout(false);
            this.pleRgb.ResumeLayout(false);
            this.pleRgb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pleRgb;
        private System.Windows.Forms.CheckBox rdiBlue;
        private System.Windows.Forms.CheckBox rdiGreen;
        private System.Windows.Forms.CheckBox rdiRed;
        private System.Windows.Forms.CheckBox ck3BandView;
        private System.Windows.Forms.Button btnAll;

    }
}