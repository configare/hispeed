namespace GeoDo.ProjectDefine
{
    partial class LinearUnitUC
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
            this.grpLinearUnit = new System.Windows.Forms.GroupBox();
            this.cmbLinearUnitName = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpLinearUnit.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpLinearUnit
            // 
            this.grpLinearUnit.Controls.Add(this.cmbLinearUnitName);
            this.grpLinearUnit.Controls.Add(this.txtValue);
            this.grpLinearUnit.Controls.Add(this.label2);
            this.grpLinearUnit.Controls.Add(this.label1);
            this.grpLinearUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLinearUnit.Location = new System.Drawing.Point(0, 0);
            this.grpLinearUnit.Name = "grpLinearUnit";
            this.grpLinearUnit.Size = new System.Drawing.Size(300, 93);
            this.grpLinearUnit.TabIndex = 0;
            this.grpLinearUnit.TabStop = false;
            this.grpLinearUnit.Text = "线性单位";
            // 
            // cmbLinearUnitName
            // 
            this.cmbLinearUnitName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLinearUnitName.FormattingEnabled = true;
            this.cmbLinearUnitName.Location = new System.Drawing.Point(89, 21);
            this.cmbLinearUnitName.Name = "cmbLinearUnitName";
            this.cmbLinearUnitName.Size = new System.Drawing.Size(175, 20);
            this.cmbLinearUnitName.TabIndex = 3;
            this.cmbLinearUnitName.Text = "<自定义>";
            this.cmbLinearUnitName.SelectedIndexChanged += new System.EventHandler(this.cmbLinearUnitName_SelectedIndexChanged);
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(134, 52);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(130, 21);
            this.txtValue.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "每单位米数:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // LinearUnitUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpLinearUnit);
            this.Name = "LinearUnitUC";
            this.Size = new System.Drawing.Size(300, 93);
            this.grpLinearUnit.ResumeLayout(false);
            this.grpLinearUnit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpLinearUnit;
        private System.Windows.Forms.ComboBox cmbLinearUnitName;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
