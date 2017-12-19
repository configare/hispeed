namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    partial class frmMsgBoxInfos
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnShowDetail = new System.Windows.Forms.Button();
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            this.lblErrMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(327, 113);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnShowDetail
            // 
            this.btnShowDetail.Location = new System.Drawing.Point(28, 112);
            this.btnShowDetail.Name = "btnShowDetail";
            this.btnShowDetail.Size = new System.Drawing.Size(107, 24);
            this.btnShowDetail.TabIndex = 2;
            this.btnShowDetail.Text = "详细信息(&D)>>";
            this.btnShowDetail.Click += new System.EventHandler(this.btnShowDetail_Click);
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.Location = new System.Drawing.Point(28, 152);
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ReadOnly = true;
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStackTrace.Size = new System.Drawing.Size(374, 152);
            this.txtStackTrace.TabIndex = 3;
            // 
            // lblErrMessage
            // 
            this.lblErrMessage.BackColor = System.Drawing.SystemColors.Control;
            this.lblErrMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblErrMessage.Location = new System.Drawing.Point(28, 32);
            this.lblErrMessage.Multiline = true;
            this.lblErrMessage.Name = "lblErrMessage";
            this.lblErrMessage.ReadOnly = true;
            this.lblErrMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lblErrMessage.Size = new System.Drawing.Size(374, 67);
            this.lblErrMessage.TabIndex = 5;
            // 
            // frmMsgBoxInfos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 148);
            this.Controls.Add(this.lblErrMessage);
            this.Controls.Add(this.txtStackTrace);
            this.Controls.Add(this.btnShowDetail);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMsgBoxInfos";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "提示";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowDetail;
        private System.Windows.Forms.TextBox txtStackTrace;
        private System.Windows.Forms.TextBox lblErrMessage;
        private System.Windows.Forms.Button btnOK;
    }
}