namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    partial class frmFY3L2ProDataSelect
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstDataDef = new System.Windows.Forms.ListView();
            this.chProName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chProDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chProDatasets = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(410, 174);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(173, 37);
            this.panel1.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Location = new System.Drawing.Point(6, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(87, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstDataDef
            // 
            this.lstDataDef.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDataDef.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chProName,
            this.chProDesc,
            this.chProDatasets});
            this.lstDataDef.FullRowSelect = true;
            this.lstDataDef.Location = new System.Drawing.Point(12, 12);
            this.lstDataDef.MultiSelect = false;
            this.lstDataDef.Name = "lstDataDef";
            this.lstDataDef.Size = new System.Drawing.Size(571, 156);
            this.lstDataDef.TabIndex = 4;
            this.lstDataDef.UseCompatibleStateImageBehavior = false;
            this.lstDataDef.View = System.Windows.Forms.View.Details;
            // 
            // chProName
            // 
            this.chProName.Text = "产品名称";
            this.chProName.Width = 73;
            // 
            // chProDesc
            // 
            this.chProDesc.Text = "产品描述";
            this.chProDesc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chProDesc.Width = 175;
            // 
            // chProDatasets
            // 
            this.chProDatasets.Text = "数据集";
            this.chProDatasets.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chProDatasets.Width = 306;
            // 
            // frmFY3L2ProDataSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 215);
            this.Controls.Add(this.lstDataDef);
            this.Controls.Add(this.panel1);
            this.Name = "frmFY3L2ProDataSelect";
            this.Text = "FY-3二级及以上产品数据定义";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lstDataDef;
        private System.Windows.Forms.ColumnHeader chProName;
        private System.Windows.Forms.ColumnHeader chProDesc;
        private System.Windows.Forms.ColumnHeader chProDatasets;
    }
}