namespace GeoDo.RSS.DI
{
    partial class frmDataImport
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
            this.lvFiles = new System.Windows.Forms.ListView();
            this.hedaerCheck = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerPro = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerSubPro = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerDir = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.ckFullSelect = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ckCommSetting = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbCommSubpro = new System.Windows.Forms.ComboBox();
            this.cbCommPro = new System.Windows.Forms.ComboBox();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.btSelectDir = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbFileCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvFiles
            // 
            this.lvFiles.CheckBoxes = true;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hedaerCheck,
            this.headerPro,
            this.headerSubPro,
            this.headerFilename,
            this.headerDir});
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.Location = new System.Drawing.Point(12, 34);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(514, 374);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvFiles_ItemChecked);
            // 
            // hedaerCheck
            // 
            this.hedaerCheck.Text = "";
            this.hedaerCheck.Width = 20;
            // 
            // headerPro
            // 
            this.headerPro.Text = "产品";
            this.headerPro.Width = 72;
            // 
            // headerSubPro
            // 
            this.headerSubPro.Text = "子产品";
            this.headerSubPro.Width = 82;
            // 
            // headerFilename
            // 
            this.headerFilename.Text = "文件名";
            this.headerFilename.Width = 140;
            // 
            // headerDir
            // 
            this.headerDir.Text = "路径";
            this.headerDir.Width = 171;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(451, 448);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(370, 448);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ckFullSelect
            // 
            this.ckFullSelect.AutoSize = true;
            this.ckFullSelect.Checked = true;
            this.ckFullSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckFullSelect.Location = new System.Drawing.Point(12, 10);
            this.ckFullSelect.Name = "ckFullSelect";
            this.ckFullSelect.Size = new System.Drawing.Size(48, 16);
            this.ckFullSelect.TabIndex = 2;
            this.ckFullSelect.Text = "全选";
            this.ckFullSelect.UseVisualStyleBackColor = true;
            this.ckFullSelect.CheckedChanged += new System.EventHandler(this.ckFullSelect_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(286, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "产品";
            // 
            // ckCommSetting
            // 
            this.ckCommSetting.AutoSize = true;
            this.ckCommSetting.Location = new System.Drawing.Point(208, 10);
            this.ckCommSetting.Name = "ckCommSetting";
            this.ckCommSetting.Size = new System.Drawing.Size(72, 16);
            this.ckCommSetting.TabIndex = 4;
            this.ckCommSetting.Text = "统一设置";
            this.ckCommSetting.UseVisualStyleBackColor = true;
            this.ckCommSetting.CheckedChanged += new System.EventHandler(this.ckCommSetting_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(402, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "子产品";
            // 
            // cbCommSubpro
            // 
            this.cbCommSubpro.Enabled = false;
            this.cbCommSubpro.FormattingEnabled = true;
            this.cbCommSubpro.Location = new System.Drawing.Point(449, 8);
            this.cbCommSubpro.Name = "cbCommSubpro";
            this.cbCommSubpro.Size = new System.Drawing.Size(74, 20);
            this.cbCommSubpro.TabIndex = 5;
            this.cbCommSubpro.SelectedIndexChanged += new System.EventHandler(this.cbCommSubpro_SelectedIndexChanged);
            // 
            // cbCommPro
            // 
            this.cbCommPro.Enabled = false;
            this.cbCommPro.FormattingEnabled = true;
            this.cbCommPro.Location = new System.Drawing.Point(321, 8);
            this.cbCommPro.Name = "cbCommPro";
            this.cbCommPro.Size = new System.Drawing.Size(74, 20);
            this.cbCommPro.TabIndex = 5;
            this.cbCommPro.SelectedIndexChanged += new System.EventHandler(this.cbCommPro_SelectedIndexChanged);
            // 
            // txtDir
            // 
            this.txtDir.Location = new System.Drawing.Point(105, 417);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(340, 21);
            this.txtDir.TabIndex = 6;
            this.txtDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDir_KeyDown);
            // 
            // btSelectDir
            // 
            this.btSelectDir.Location = new System.Drawing.Point(451, 415);
            this.btSelectDir.Name = "btSelectDir";
            this.btSelectDir.Size = new System.Drawing.Size(75, 23);
            this.btSelectDir.TabIndex = 7;
            this.btSelectDir.Text = "选择..";
            this.btSelectDir.UseVisualStyleBackColor = true;
            this.btSelectDir.Click += new System.EventHandler(this.btSelectDir_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 420);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "待导入文件路径";
            // 
            // lbFileCount
            // 
            this.lbFileCount.AutoSize = true;
            this.lbFileCount.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFileCount.ForeColor = System.Drawing.Color.MediumBlue;
            this.lbFileCount.Location = new System.Drawing.Point(10, 453);
            this.lbFileCount.Name = "lbFileCount";
            this.lbFileCount.Size = new System.Drawing.Size(113, 12);
            this.lbFileCount.TabIndex = 3;
            this.lbFileCount.Text = "共有： 0  选中： 0";
            // 
            // frmDataImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 476);
            this.Controls.Add(this.btSelectDir);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.cbCommPro);
            this.Controls.Add(this.cbCommSubpro);
            this.Controls.Add(this.ckCommSetting);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbFileCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckFullSelect);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.lvFiles);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDataImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据导入窗口";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader headerPro;
        private System.Windows.Forms.ColumnHeader headerSubPro;
        private System.Windows.Forms.ColumnHeader headerFilename;
        private System.Windows.Forms.ColumnHeader headerDir;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.ColumnHeader hedaerCheck;
        private System.Windows.Forms.CheckBox ckFullSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckCommSetting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbCommSubpro;
        private System.Windows.Forms.ComboBox cbCommPro;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Button btSelectDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbFileCount;
    }
}