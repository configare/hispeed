namespace Geodo.Smart.ProDataCollect
{
    partial class FrmProDataCollect
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
            this.btWorkspace = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbProduct = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbsubProduct = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWorksapce = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpBegin = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.ckCycTime = new System.Windows.Forms.CheckBox();
            this.cbCycTime = new System.Windows.Forms.ComboBox();
            this.btOutdir = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutdir = new System.Windows.Forms.TextBox();
            this.ckRemove = new System.Windows.Forms.CheckBox();
            this.btSaveArgs = new System.Windows.Forms.Button();
            this.btReadArgs = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btWorkspace
            // 
            this.btWorkspace.Location = new System.Drawing.Point(421, 49);
            this.btWorkspace.Name = "btWorkspace";
            this.btWorkspace.Size = new System.Drawing.Size(75, 23);
            this.btWorkspace.TabIndex = 0;
            this.btWorkspace.Text = "选择..";
            this.btWorkspace.UseVisualStyleBackColor = true;
            this.btWorkspace.Click += new System.EventHandler(this.btWorkspace_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "监测产品";
            // 
            // cbProduct
            // 
            this.cbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProduct.FormattingEnabled = true;
            this.cbProduct.Location = new System.Drawing.Point(70, 13);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Size = new System.Drawing.Size(121, 20);
            this.cbProduct.TabIndex = 2;
            this.cbProduct.SelectedIndexChanged += new System.EventHandler(this.cbProduct_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "监测子产品";
            // 
            // cbsubProduct
            // 
            this.cbsubProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbsubProduct.FormattingEnabled = true;
            this.cbsubProduct.Location = new System.Drawing.Point(268, 13);
            this.cbsubProduct.Name = "cbsubProduct";
            this.cbsubProduct.Size = new System.Drawing.Size(228, 20);
            this.cbsubProduct.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "工作空间";
            // 
            // txtWorksapce
            // 
            this.txtWorksapce.Location = new System.Drawing.Point(70, 51);
            this.txtWorksapce.Name = "txtWorksapce";
            this.txtWorksapce.Size = new System.Drawing.Size(345, 21);
            this.txtWorksapce.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "时间范围";
            // 
            // dtpBegin
            // 
            this.dtpBegin.Location = new System.Drawing.Point(70, 93);
            this.dtpBegin.Name = "dtpBegin";
            this.dtpBegin.Size = new System.Drawing.Size(121, 21);
            this.dtpBegin.TabIndex = 4;
            this.dtpBegin.ValueChanged += new System.EventHandler(this.dtpBegin_ValueChanged);
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(214, 93);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(122, 21);
            this.dtpEnd.TabIndex = 4;
            // 
            // ckCycTime
            // 
            this.ckCycTime.AutoSize = true;
            this.ckCycTime.Location = new System.Drawing.Point(367, 94);
            this.ckCycTime.Name = "ckCycTime";
            this.ckCycTime.Size = new System.Drawing.Size(48, 16);
            this.ckCycTime.TabIndex = 5;
            this.ckCycTime.Text = "周期";
            this.ckCycTime.UseVisualStyleBackColor = true;
            this.ckCycTime.CheckedChanged += new System.EventHandler(this.ckCycTime_CheckedChanged);
            // 
            // cbCycTime
            // 
            this.cbCycTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCycTime.Enabled = false;
            this.cbCycTime.FormattingEnabled = true;
            this.cbCycTime.Items.AddRange(new object[] {
            "周",
            "旬",
            "月"});
            this.cbCycTime.Location = new System.Drawing.Point(421, 90);
            this.cbCycTime.Name = "cbCycTime";
            this.cbCycTime.Size = new System.Drawing.Size(75, 20);
            this.cbCycTime.TabIndex = 2;
            this.cbCycTime.SelectedIndexChanged += new System.EventHandler(this.cbCycTime_SelectedIndexChanged);
            // 
            // btOutdir
            // 
            this.btOutdir.Location = new System.Drawing.Point(421, 134);
            this.btOutdir.Name = "btOutdir";
            this.btOutdir.Size = new System.Drawing.Size(75, 23);
            this.btOutdir.TabIndex = 0;
            this.btOutdir.Text = "选择..";
            this.btOutdir.UseVisualStyleBackColor = true;
            this.btOutdir.Click += new System.EventHandler(this.btOutdir_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "输出目录";
            // 
            // txtOutdir
            // 
            this.txtOutdir.Location = new System.Drawing.Point(70, 134);
            this.txtOutdir.Name = "txtOutdir";
            this.txtOutdir.Size = new System.Drawing.Size(345, 21);
            this.txtOutdir.TabIndex = 3;
            // 
            // ckRemove
            // 
            this.ckRemove.AutoSize = true;
            this.ckRemove.Location = new System.Drawing.Point(15, 175);
            this.ckRemove.Name = "ckRemove";
            this.ckRemove.Size = new System.Drawing.Size(48, 16);
            this.ckRemove.TabIndex = 5;
            this.ckRemove.Text = "剪切";
            this.ckRemove.UseVisualStyleBackColor = true;
            // 
            // btSaveArgs
            // 
            this.btSaveArgs.Location = new System.Drawing.Point(70, 171);
            this.btSaveArgs.Name = "btSaveArgs";
            this.btSaveArgs.Size = new System.Drawing.Size(75, 23);
            this.btSaveArgs.TabIndex = 0;
            this.btSaveArgs.Text = " 保存参数";
            this.btSaveArgs.UseVisualStyleBackColor = true;
            this.btSaveArgs.Click += new System.EventHandler(this.btSaveArgs_Click);
            // 
            // btReadArgs
            // 
            this.btReadArgs.Location = new System.Drawing.Point(161, 171);
            this.btReadArgs.Name = "btReadArgs";
            this.btReadArgs.Size = new System.Drawing.Size(75, 23);
            this.btReadArgs.TabIndex = 0;
            this.btReadArgs.Text = "读取参数";
            this.btReadArgs.UseVisualStyleBackColor = true;
            this.btReadArgs.Click += new System.EventHandler(this.btReadArgs_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(340, 171);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(421, 171);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 0;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // FrmProDataCollect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 202);
            this.Controls.Add(this.ckRemove);
            this.Controls.Add(this.ckCycTime);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.dtpBegin);
            this.Controls.Add(this.txtOutdir);
            this.Controls.Add(this.txtWorksapce);
            this.Controls.Add(this.cbCycTime);
            this.Controls.Add(this.cbsubProduct);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbProduct);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btReadArgs);
            this.Controls.Add(this.btSaveArgs);
            this.Controls.Add(this.btOutdir);
            this.Controls.Add(this.btWorkspace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmProDataCollect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMART产品数据收集与整理";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btWorkspace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProduct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbsubProduct;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWorksapce;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpBegin;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.CheckBox ckCycTime;
        private System.Windows.Forms.ComboBox cbCycTime;
        private System.Windows.Forms.Button btOutdir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutdir;
        private System.Windows.Forms.CheckBox ckRemove;
        private System.Windows.Forms.Button btSaveArgs;
        private System.Windows.Forms.Button btReadArgs;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
    }
}

