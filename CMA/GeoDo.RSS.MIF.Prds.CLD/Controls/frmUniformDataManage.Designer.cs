namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class frmUniformDataManage
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
            this.txtErrorLog = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ucCheckBoxListDate = new GeoDo.RSS.MIF.Prds.CLD.ucCheckBoxList();
            this.ucCheckBoxListSets = new GeoDo.RSS.MIF.Prds.CLD.ucCheckBoxList();
            this.ucRadioBoxList1 = new GeoDo.RSS.MIF.Prds.CLD.ucRadioBoxList();
            this.SuspendLayout();
            // 
            // txtErrorLog
            // 
            this.txtErrorLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorLog.Location = new System.Drawing.Point(3, 227);
            this.txtErrorLog.Multiline = true;
            this.txtErrorLog.Name = "txtErrorLog";
            this.txtErrorLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorLog.Size = new System.Drawing.Size(652, 193);
            this.txtErrorLog.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(473, 426);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(571, 426);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ucCheckBoxListDate
            // 
            this.ucCheckBoxListDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucCheckBoxListDate.Location = new System.Drawing.Point(3, 227);
            this.ucCheckBoxListDate.Name = "ucCheckBoxListDate";
            this.ucCheckBoxListDate.Size = new System.Drawing.Size(652, 24);
            this.ucCheckBoxListDate.TabIndex = 6;
            // 
            // ucCheckBoxListSets
            // 
            this.ucCheckBoxListSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucCheckBoxListSets.Location = new System.Drawing.Point(3, 35);
            this.ucCheckBoxListSets.Name = "ucCheckBoxListSets";
            this.ucCheckBoxListSets.Size = new System.Drawing.Size(652, 186);
            this.ucCheckBoxListSets.TabIndex = 5;
            // 
            // ucRadioBoxList1
            // 
            this.ucRadioBoxList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucRadioBoxList1.Location = new System.Drawing.Point(3, 3);
            this.ucRadioBoxList1.Name = "ucRadioBoxList1";
            this.ucRadioBoxList1.Size = new System.Drawing.Size(652, 29);
            this.ucRadioBoxList1.TabIndex = 4;
            this.ucRadioBoxList1.CheckedChanged += new System.EventHandler(this.ucRadioBoxList1_CheckedChanged);
            // 
            // frmUniformDataManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 454);
            this.Controls.Add(this.ucCheckBoxListSets);
            this.Controls.Add(this.ucRadioBoxList1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtErrorLog);
            this.Controls.Add(this.ucCheckBoxListDate);
            this.Name = "frmUniformDataManage";
            this.Text = "归档数据清理";
            this.Load += new System.EventHandler(this.frmUniformDataManage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtErrorLog;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private ucRadioBoxList ucRadioBoxList1;
        private ucCheckBoxList ucCheckBoxListSets;
        private ucCheckBoxList ucCheckBoxListDate;
    }
}