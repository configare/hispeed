namespace GeoDo.ProjectDefine
{
    partial class SpheroidUC
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
            this.grpSpheroid = new System.Windows.Forms.GroupBox();
            this.txtInverseFlattening = new System.Windows.Forms.TextBox();
            this.txtSemiminorAxis = new System.Windows.Forms.TextBox();
            this.rioInverseFlattening = new System.Windows.Forms.RadioButton();
            this.rioSemiminorAxis = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSemimajorAxis = new System.Windows.Forms.TextBox();
            this.cmbSpheroidName = new System.Windows.Forms.ComboBox();
            this.lblParam = new System.Windows.Forms.Label();
            this.grpSpheroid.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSpheroid
            // 
            this.grpSpheroid.Controls.Add(this.txtInverseFlattening);
            this.grpSpheroid.Controls.Add(this.txtSemiminorAxis);
            this.grpSpheroid.Controls.Add(this.rioInverseFlattening);
            this.grpSpheroid.Controls.Add(this.rioSemiminorAxis);
            this.grpSpheroid.Controls.Add(this.label1);
            this.grpSpheroid.Controls.Add(this.txtSemimajorAxis);
            this.grpSpheroid.Controls.Add(this.cmbSpheroidName);
            this.grpSpheroid.Controls.Add(this.lblParam);
            this.grpSpheroid.Location = new System.Drawing.Point(0, 0);
            this.grpSpheroid.Name = "grpSpheroid";
            this.grpSpheroid.Size = new System.Drawing.Size(329, 164);
            this.grpSpheroid.TabIndex = 3;
            this.grpSpheroid.TabStop = false;
            this.grpSpheroid.Text = "椭球体";
            // 
            // txtInverseFlattening
            // 
            this.txtInverseFlattening.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInverseFlattening.Location = new System.Drawing.Point(130, 124);
            this.txtInverseFlattening.Name = "txtInverseFlattening";
            this.txtInverseFlattening.Size = new System.Drawing.Size(174, 21);
            this.txtInverseFlattening.TabIndex = 8;
            // 
            // txtSemiminorAxis
            // 
            this.txtSemiminorAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSemiminorAxis.Location = new System.Drawing.Point(130, 90);
            this.txtSemiminorAxis.Name = "txtSemiminorAxis";
            this.txtSemiminorAxis.Size = new System.Drawing.Size(174, 21);
            this.txtSemiminorAxis.TabIndex = 7;
            // 
            // rioInverseFlattening
            // 
            this.rioInverseFlattening.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rioInverseFlattening.AutoSize = true;
            this.rioInverseFlattening.Checked = true;
            this.rioInverseFlattening.Location = new System.Drawing.Point(20, 129);
            this.rioInverseFlattening.Name = "rioInverseFlattening";
            this.rioInverseFlattening.Size = new System.Drawing.Size(65, 16);
            this.rioInverseFlattening.TabIndex = 0;
            this.rioInverseFlattening.TabStop = true;
            this.rioInverseFlattening.Text = "反扁率:";
            this.rioInverseFlattening.UseVisualStyleBackColor = true;
            this.rioInverseFlattening.CheckedChanged += new System.EventHandler(this.rioInverseFlattening_CheckedChanged);
            // 
            // rioSemiminorAxis
            // 
            this.rioSemiminorAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rioSemiminorAxis.AutoSize = true;
            this.rioSemiminorAxis.Location = new System.Drawing.Point(20, 95);
            this.rioSemiminorAxis.Name = "rioSemiminorAxis";
            this.rioSemiminorAxis.Size = new System.Drawing.Size(65, 16);
            this.rioSemiminorAxis.TabIndex = 0;
            this.rioSemiminorAxis.Text = "短半轴:";
            this.rioSemiminorAxis.UseVisualStyleBackColor = true;
            this.rioSemiminorAxis.CheckedChanged += new System.EventHandler(this.rioSemiminorAxis_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "长半轴:";
            // 
            // txtSemimajorAxis
            // 
            this.txtSemimajorAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSemimajorAxis.Location = new System.Drawing.Point(130, 50);
            this.txtSemimajorAxis.Name = "txtSemimajorAxis";
            this.txtSemimajorAxis.Size = new System.Drawing.Size(174, 21);
            this.txtSemimajorAxis.TabIndex = 3;
            // 
            // cmbSpheroidName
            // 
            this.cmbSpheroidName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSpheroidName.FormattingEnabled = true;
            this.cmbSpheroidName.Location = new System.Drawing.Point(81, 18);
            this.cmbSpheroidName.Name = "cmbSpheroidName";
            this.cmbSpheroidName.Size = new System.Drawing.Size(223, 20);
            this.cmbSpheroidName.TabIndex = 1;
            this.cmbSpheroidName.Text = "<自定义>";
            this.cmbSpheroidName.SelectedIndexChanged += new System.EventHandler(this.cmbSpheroidName_SelectedIndexChanged);
            // 
            // lblParam
            // 
            this.lblParam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblParam.AutoSize = true;
            this.lblParam.Location = new System.Drawing.Point(20, 26);
            this.lblParam.Name = "lblParam";
            this.lblParam.Size = new System.Drawing.Size(35, 12);
            this.lblParam.TabIndex = 0;
            this.lblParam.Text = "名称:";
            // 
            // SpheroidUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSpheroid);
            this.Name = "SpheroidUC";
            this.Size = new System.Drawing.Size(329, 164);
            this.grpSpheroid.ResumeLayout(false);
            this.grpSpheroid.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSpheroid;
        private System.Windows.Forms.TextBox txtSemimajorAxis;
        private System.Windows.Forms.ComboBox cmbSpheroidName;
        private System.Windows.Forms.Label lblParam;
        private System.Windows.Forms.TextBox txtInverseFlattening;
        private System.Windows.Forms.TextBox txtSemiminorAxis;
        private System.Windows.Forms.RadioButton rioInverseFlattening;
        private System.Windows.Forms.RadioButton rioSemiminorAxis;
        private System.Windows.Forms.Label label1;
    }
}
