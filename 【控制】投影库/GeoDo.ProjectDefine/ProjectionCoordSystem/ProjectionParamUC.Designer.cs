namespace GeoDo.ProjectDefine
{
    partial class ProjectionParamUI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.grpProj = new System.Windows.Forms.GroupBox();
            this.dgvParams = new System.Windows.Forms.DataGridView();
            this.cmbPrjName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpProj.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).BeginInit();
            this.SuspendLayout();
            // 
            // grpProj
            // 
            this.grpProj.Controls.Add(this.dgvParams);
            this.grpProj.Controls.Add(this.cmbPrjName);
            this.grpProj.Controls.Add(this.label1);
            this.grpProj.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProj.Location = new System.Drawing.Point(0, 0);
            this.grpProj.Name = "grpProj";
            this.grpProj.Size = new System.Drawing.Size(311, 222);
            this.grpProj.TabIndex = 0;
            this.grpProj.TabStop = false;
            this.grpProj.Text = "投影";
            // 
            // dgvParams
            // 
            this.dgvParams.AllowUserToAddRows = false;
            this.dgvParams.AllowUserToDeleteRows = false;
            this.dgvParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvParams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParams.Location = new System.Drawing.Point(6, 55);
            this.dgvParams.Name = "dgvParams";
            this.dgvParams.RowHeadersVisible = false;
            this.dgvParams.RowTemplate.Height = 23;
            this.dgvParams.Size = new System.Drawing.Size(299, 150);
            this.dgvParams.TabIndex = 2;
            // 
            // cmbPrjName
            // 
            this.cmbPrjName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPrjName.FormattingEnabled = true;
            this.cmbPrjName.Location = new System.Drawing.Point(68, 21);
            this.cmbPrjName.Name = "cmbPrjName";
            this.cmbPrjName.Size = new System.Drawing.Size(237, 20);
            this.cmbPrjName.TabIndex = 1;
            this.cmbPrjName.SelectedIndexChanged += new System.EventHandler(this.cmbPrjName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // ProjectionParamUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpProj);
            this.Name = "ProjectionParamUI";
            this.Size = new System.Drawing.Size(311, 222);
            this.grpProj.ResumeLayout(false);
            this.grpProj.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProj;
        private System.Windows.Forms.ComboBox cmbPrjName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvParams;
    }
}
