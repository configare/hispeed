namespace Test
{
    partial class DataMove
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMove));
            this.label3 = new System.Windows.Forms.Label();
            this.txtToDir = new System.Windows.Forms.TextBox();
            this.btnOpenToDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDir2move = new System.Windows.Forms.TextBox();
            this.btnOpenDir2move = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTips = new System.Windows.Forms.TextBox();
            this.cbxDeleteRawData = new System.Windows.Forms.CheckBox();
            this.cbxOverLapExist = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "目标目录:";
            // 
            // txtToDir
            // 
            this.txtToDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToDir.Location = new System.Drawing.Point(99, 51);
            this.txtToDir.Name = "txtToDir";
            this.txtToDir.Size = new System.Drawing.Size(413, 21);
            this.txtToDir.TabIndex = 31;
            // 
            // btnOpenToDir
            // 
            this.btnOpenToDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenToDir.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenToDir.Image")));
            this.btnOpenToDir.Location = new System.Drawing.Point(520, 48);
            this.btnOpenToDir.Name = "btnOpenToDir";
            this.btnOpenToDir.Size = new System.Drawing.Size(24, 24);
            this.btnOpenToDir.TabIndex = 30;
            this.btnOpenToDir.UseVisualStyleBackColor = true;
            this.btnOpenToDir.Click += new System.EventHandler(this.btnOpenToDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(426, 89);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 23);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 29;
            this.label2.Text = "待迁移目录:";
            // 
            // txtDir2move
            // 
            this.txtDir2move.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir2move.Location = new System.Drawing.Point(99, 17);
            this.txtDir2move.Name = "txtDir2move";
            this.txtDir2move.Size = new System.Drawing.Size(413, 21);
            this.txtDir2move.TabIndex = 28;
            // 
            // btnOpenDir2move
            // 
            this.btnOpenDir2move.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDir2move.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenDir2move.Image")));
            this.btnOpenDir2move.Location = new System.Drawing.Point(520, 14);
            this.btnOpenDir2move.Name = "btnOpenDir2move";
            this.btnOpenDir2move.Size = new System.Drawing.Size(24, 24);
            this.btnOpenDir2move.TabIndex = 27;
            this.btnOpenDir2move.UseVisualStyleBackColor = true;
            this.btnOpenDir2move.Click += new System.EventHandler(this.btnOpenDir2move_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(488, 89);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtTips
            // 
            this.txtTips.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTips.BackColor = System.Drawing.SystemColors.Control;
            this.txtTips.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTips.Location = new System.Drawing.Point(12, 118);
            this.txtTips.Name = "txtTips";
            this.txtTips.Size = new System.Drawing.Size(532, 14);
            this.txtTips.TabIndex = 34;
            // 
            // cbxDeleteRawData
            // 
            this.cbxDeleteRawData.AutoSize = true;
            this.cbxDeleteRawData.Location = new System.Drawing.Point(99, 89);
            this.cbxDeleteRawData.Name = "cbxDeleteRawData";
            this.cbxDeleteRawData.Size = new System.Drawing.Size(108, 16);
            this.cbxDeleteRawData.TabIndex = 35;
            this.cbxDeleteRawData.Text = "迁移后删除文件";
            this.cbxDeleteRawData.UseVisualStyleBackColor = true;
            // 
            // cbxOverLapExist
            // 
            this.cbxOverLapExist.AutoSize = true;
            this.cbxOverLapExist.Location = new System.Drawing.Point(213, 89);
            this.cbxOverLapExist.Name = "cbxOverLapExist";
            this.cbxOverLapExist.Size = new System.Drawing.Size(96, 16);
            this.cbxOverLapExist.TabIndex = 36;
            this.cbxOverLapExist.Text = "覆盖现有文件";
            this.cbxOverLapExist.UseVisualStyleBackColor = true;
            // 
            // DataMove
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 136);
            this.Controls.Add(this.cbxOverLapExist);
            this.Controls.Add(this.cbxDeleteRawData);
            this.Controls.Add(this.txtTips);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtToDir);
            this.Controls.Add(this.btnOpenToDir);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDir2move);
            this.Controls.Add(this.btnOpenDir2move);
            this.Name = "DataMove";
            this.Text = "数据迁移工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtToDir;
        private System.Windows.Forms.Button btnOpenToDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDir2move;
        private System.Windows.Forms.Button btnOpenDir2move;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtTips;
        private System.Windows.Forms.CheckBox cbxDeleteRawData;
        private System.Windows.Forms.CheckBox cbxOverLapExist;
    }
}