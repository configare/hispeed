namespace GeoDo.RSS.CA
{
    partial class frmMiddleFilterArgEditor
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
            this.lblName = new System.Windows.Forms.Label();
            this.cbxWindowRadius = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(232, 37);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(232, 8);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(232, 66);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(232, 100);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(14, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(65, 12);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "窗口大小：";
            // 
            // cbxWindowRadius
            // 
            this.cbxWindowRadius.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxWindowRadius.FormattingEnabled = true;
            this.cbxWindowRadius.Location = new System.Drawing.Point(73, 17);
            this.cbxWindowRadius.Name = "cbxWindowRadius";
            this.cbxWindowRadius.Size = new System.Drawing.Size(129, 20);
            this.cbxWindowRadius.TabIndex = 8;
            this.cbxWindowRadius.SelectedIndexChanged += new System.EventHandler(this.collectedWindowRadius_SelectedIndexChanged);
            // 
            // frmMiddleFilterArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 119);
            this.Controls.Add(this.cbxWindowRadius);
            this.Controls.Add(this.lblName);
            this.Name = "frmMiddleFilterArgEditor";
            this.Text = "中值滤波";
            this.Controls.SetChildIndex(this.lblName, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.cbxWindowRadius, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ComboBox cbxWindowRadius;
    }
}