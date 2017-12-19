namespace GeoDo.RSS.CA
{
    partial class frmWeightedAveFilterArgEditor
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
            this.cbxWindowRadius = new System.Windows.Forms.ComboBox();
            this.lblWindowSize = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(293, 37);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(293, 8);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(293, 66);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckPreviewing.Location = new System.Drawing.Point(293, 105);
            // 
            // cbxWindowRadius
            // 
            this.cbxWindowRadius.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxWindowRadius.FormattingEnabled = true;
            this.cbxWindowRadius.Location = new System.Drawing.Point(70, 10);
            this.cbxWindowRadius.Name = "cbxWindowRadius";
            this.cbxWindowRadius.Size = new System.Drawing.Size(129, 20);
            this.cbxWindowRadius.TabIndex = 10;
            // 
            // lblWindowSize
            // 
            this.lblWindowSize.AutoSize = true;
            this.lblWindowSize.Location = new System.Drawing.Point(11, 15);
            this.lblWindowSize.Name = "lblWindowSize";
            this.lblWindowSize.Size = new System.Drawing.Size(65, 12);
            this.lblWindowSize.TabIndex = 9;
            this.lblWindowSize.Text = "窗口大小：";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(13, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 158);
            this.panel1.TabIndex = 11;
            // 
            // frmWeightedAveFilterArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 200);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbxWindowRadius);
            this.Controls.Add(this.lblWindowSize);
            this.Name = "frmWeightedAveFilterArgEditor";
            this.Text = "自定义滤波";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.lblWindowSize, 0);
            this.Controls.SetChildIndex(this.cbxWindowRadius, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxWindowRadius;
        private System.Windows.Forms.Label lblWindowSize;
        private System.Windows.Forms.Panel panel1;
    }
}