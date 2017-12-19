namespace GeoDo.RSS.UI.AddIn.MaxCsr
{
    partial class frmMaxCsr
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
            this.labFName = new System.Windows.Forms.Label();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBandNos = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labFName
            // 
            this.labFName.AutoSize = true;
            this.labFName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFName.Location = new System.Drawing.Point(12, 18);
            this.labFName.Name = "labFName";
            this.labFName.Size = new System.Drawing.Size(104, 17);
            this.labFName.TabIndex = 0;
            this.labFName.Text = "待计算文件路径：";
            // 
            // txtFileDir
            // 
            this.txtFileDir.Location = new System.Drawing.Point(122, 17);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.Size = new System.Drawing.Size(271, 21);
            this.txtFileDir.TabIndex = 1;
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Image = global::GeoDo.RSS.UI.AddIn.MaxCsr.Properties.Resources.folder_open16;
            this.btnOpenDir.Location = new System.Drawing.Point(402, 14);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(25, 25);
            this.btnOpenDir.TabIndex = 2;
            this.btnOpenDir.UseVisualStyleBackColor = true;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(218, 98);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "计算";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(318, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "NDVI计算波段：";
            // 
            // txtBandNos
            // 
            this.txtBandNos.Location = new System.Drawing.Point(122, 46);
            this.txtBandNos.Name = "txtBandNos";
            this.txtBandNos.Size = new System.Drawing.Size(271, 21);
            this.txtBandNos.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(12, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(299, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "eg：波段用【，】间隔，依次输入可见光、近红外波段";
            // 
            // frmMaxCsr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 133);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBandNos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnOpenDir);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.labFName);
            this.Name = "frmMaxCsr";
            this.ShowIcon = false;
            this.Text = "计算晴空反射率...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labFName;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBandNos;
        private System.Windows.Forms.Label label4;
    }
}