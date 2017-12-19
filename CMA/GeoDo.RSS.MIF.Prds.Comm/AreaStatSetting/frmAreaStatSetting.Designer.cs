namespace GeoDo.RSS.MIF.Prds.Comm
{
    partial class frmAreaStatSetting
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
            this.labStatItem = new System.Windows.Forms.Label();
            this.cmbStatItems = new System.Windows.Forms.ComboBox();
            this.grbStatItemInfo = new System.Windows.Forms.GroupBox();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.labFields = new System.Windows.Forms.Label();
            this.btnOpenInfoFile = new System.Windows.Forms.Button();
            this.txtInfoFile = new System.Windows.Forms.TextBox();
            this.labInfoFile = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.cmbStatFiles = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.labColumnNames = new System.Windows.Forms.Label();
            this.txtColumnNames = new System.Windows.Forms.TextBox();
            this.grbStatItemInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // labStatItem
            // 
            this.labStatItem.AutoSize = true;
            this.labStatItem.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labStatItem.Location = new System.Drawing.Point(18, 22);
            this.labStatItem.Name = "labStatItem";
            this.labStatItem.Size = new System.Drawing.Size(56, 17);
            this.labStatItem.TabIndex = 2;
            this.labStatItem.Text = "统计项：";
            // 
            // cmbStatItems
            // 
            this.cmbStatItems.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbStatItems.FormattingEnabled = true;
            this.cmbStatItems.Location = new System.Drawing.Point(85, 18);
            this.cmbStatItems.Name = "cmbStatItems";
            this.cmbStatItems.Size = new System.Drawing.Size(120, 25);
            this.cmbStatItems.TabIndex = 3;
            this.cmbStatItems.SelectedIndexChanged += new System.EventHandler(this.cmbStatItems_SelectedIndexChanged);
            // 
            // grbStatItemInfo
            // 
            this.grbStatItemInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbStatItemInfo.Controls.Add(this.cmbFields);
            this.grbStatItemInfo.Controls.Add(this.labFields);
            this.grbStatItemInfo.Controls.Add(this.btnOpenInfoFile);
            this.grbStatItemInfo.Controls.Add(this.txtInfoFile);
            this.grbStatItemInfo.Controls.Add(this.labInfoFile);
            this.grbStatItemInfo.Controls.Add(this.btnOpen);
            this.grbStatItemInfo.Controls.Add(this.cmbStatFiles);
            this.grbStatItemInfo.Controls.Add(this.txtName);
            this.grbStatItemInfo.Controls.Add(this.label3);
            this.grbStatItemInfo.Controls.Add(this.label2);
            this.grbStatItemInfo.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grbStatItemInfo.Location = new System.Drawing.Point(12, 89);
            this.grbStatItemInfo.Name = "grbStatItemInfo";
            this.grbStatItemInfo.Size = new System.Drawing.Size(401, 128);
            this.grbStatItemInfo.TabIndex = 4;
            this.grbStatItemInfo.TabStop = false;
            this.grbStatItemInfo.Text = "统计项信息";
            // 
            // cmbFields
            // 
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(73, 89);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(285, 25);
            this.cmbFields.TabIndex = 10;
            // 
            // labFields
            // 
            this.labFields.AutoSize = true;
            this.labFields.Location = new System.Drawing.Point(6, 92);
            this.labFields.Name = "labFields";
            this.labFields.Size = new System.Drawing.Size(68, 17);
            this.labFields.TabIndex = 9;
            this.labFields.Text = "统计字段：";
            // 
            // btnOpenInfoFile
            // 
            this.btnOpenInfoFile.Enabled = false;
            this.btnOpenInfoFile.Image = global::GeoDo.RSS.MIF.Prds.Comm.Properties.Resources.cmdOpen;
            this.btnOpenInfoFile.Location = new System.Drawing.Point(363, 89);
            this.btnOpenInfoFile.Name = "btnOpenInfoFile";
            this.btnOpenInfoFile.Size = new System.Drawing.Size(28, 23);
            this.btnOpenInfoFile.TabIndex = 8;
            this.btnOpenInfoFile.UseVisualStyleBackColor = true;
            this.btnOpenInfoFile.Click += new System.EventHandler(this.btnOpenInfoFile_Click);
            // 
            // txtInfoFile
            // 
            this.txtInfoFile.Enabled = false;
            this.txtInfoFile.Location = new System.Drawing.Point(73, 89);
            this.txtInfoFile.Name = "txtInfoFile";
            this.txtInfoFile.Size = new System.Drawing.Size(284, 23);
            this.txtInfoFile.TabIndex = 7;
            // 
            // labInfoFile
            // 
            this.labInfoFile.AutoSize = true;
            this.labInfoFile.Location = new System.Drawing.Point(6, 92);
            this.labInfoFile.Name = "labInfoFile";
            this.labInfoFile.Size = new System.Drawing.Size(68, 17);
            this.labInfoFile.TabIndex = 6;
            this.labInfoFile.Text = "对照文件：";
            // 
            // btnOpen
            // 
            this.btnOpen.Image = global::GeoDo.RSS.MIF.Prds.Comm.Properties.Resources.cmdOpen;
            this.btnOpen.Location = new System.Drawing.Point(363, 59);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(28, 23);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // cmbStatFiles
            // 
            this.cmbStatFiles.FormattingEnabled = true;
            this.cmbStatFiles.Location = new System.Drawing.Point(73, 58);
            this.cmbStatFiles.Name = "cmbStatFiles";
            this.cmbStatFiles.Size = new System.Drawing.Size(284, 25);
            this.cmbStatFiles.TabIndex = 4;
            this.cmbStatFiles.TextChanged += new System.EventHandler(this.cmbStatFiles_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(73, 29);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(285, 23);
            this.txtName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "统计文件：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "菜单名称：";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(130, 233);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(329, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(232, 233);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labColumnNames
            // 
            this.labColumnNames.AutoSize = true;
            this.labColumnNames.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labColumnNames.Location = new System.Drawing.Point(18, 56);
            this.labColumnNames.Name = "labColumnNames";
            this.labColumnNames.Size = new System.Drawing.Size(56, 17);
            this.labColumnNames.TabIndex = 11;
            this.labColumnNames.Text = "列名称：";
            // 
            // txtColumnNames
            // 
            this.txtColumnNames.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtColumnNames.Location = new System.Drawing.Point(85, 53);
            this.txtColumnNames.Name = "txtColumnNames";
            this.txtColumnNames.Size = new System.Drawing.Size(284, 23);
            this.txtColumnNames.TabIndex = 12;
            // 
            // frmAreaStatSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 270);
            this.Controls.Add(this.txtColumnNames);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labColumnNames);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grbStatItemInfo);
            this.Controls.Add(this.cmbStatItems);
            this.Controls.Add(this.labStatItem);
            this.Name = "frmAreaStatSetting";
            this.ShowIcon = false;
            this.Text = "统计分析设置";
            this.grbStatItemInfo.ResumeLayout(false);
            this.grbStatItemInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labStatItem;
        private System.Windows.Forms.ComboBox cmbStatItems;
        private System.Windows.Forms.GroupBox grbStatItemInfo;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ComboBox cmbStatFiles;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnOpenInfoFile;
        private System.Windows.Forms.TextBox txtInfoFile;
        private System.Windows.Forms.Label labInfoFile;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Label labFields;
        private System.Windows.Forms.Label labColumnNames;
        private System.Windows.Forms.TextBox txtColumnNames;
    }
}