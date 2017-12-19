namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    partial class frmHDF5DataRemoveLines
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
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.labFName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtBandNos = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOutDir
            // 
            this.btnOutDir.Image = global::GeoDo.RSS.UI.AddIn.RemoveLines.Properties.Resources.folder_open16;
            this.btnOutDir.Location = new System.Drawing.Point(399, 97);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(25, 25);
            this.btnOutDir.TabIndex = 7;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Image = global::GeoDo.RSS.UI.AddIn.RemoveLines.Properties.Resources.folder_open16;
            this.btnOpenDir.Location = new System.Drawing.Point(399, 20);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(25, 25);
            this.btnOpenDir.TabIndex = 8;
            this.btnOpenDir.UseVisualStyleBackColor = true;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // txtOutDir
            // 
            this.txtOutDir.Enabled = false;
            this.txtOutDir.Location = new System.Drawing.Point(98, 100);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.ReadOnly = true;
            this.txtOutDir.Size = new System.Drawing.Size(295, 21);
            this.txtOutDir.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(24, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "输出文件：";
            // 
            // txtFileDir
            // 
            this.txtFileDir.Enabled = false;
            this.txtFileDir.Location = new System.Drawing.Point(98, 23);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.Size = new System.Drawing.Size(295, 21);
            this.txtFileDir.TabIndex = 5;
            // 
            // labFName
            // 
            this.labFName.AutoSize = true;
            this.labFName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFName.Location = new System.Drawing.Point(24, 24);
            this.labFName.Name = "labFName";
            this.labFName.Size = new System.Drawing.Size(68, 17);
            this.labFName.TabIndex = 4;
            this.labFName.Text = "原始文件：";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(330, 144);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(229, 144);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "开始";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtBandNos
            // 
            this.txtBandNos.Location = new System.Drawing.Point(98, 60);
            this.txtBandNos.Name = "txtBandNos";
            this.txtBandNos.Size = new System.Drawing.Size(295, 21);
            this.txtBandNos.TabIndex = 12;
            this.txtBandNos.Text = "5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "待处理波段：";
            // 
            // frmHDF5DataRemoveLines
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 184);
            this.Controls.Add(this.txtBandNos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.btnOpenDir);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.labFName);
            this.Name = "frmHDF5DataRemoveLines";
            this.Text = "去条带";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Label labFName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtBandNos;
        private System.Windows.Forms.Label label2;
    }
}