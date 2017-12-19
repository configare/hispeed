namespace GeoDo.ProjectDefine
{
    partial class PrimeMeridianUC
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
            this.grpPrimeMeridian = new System.Windows.Forms.GroupBox();
            this.cmbPrimeMeridianName = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpPrimeMeridian.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPrimeMeridian
            // 
            this.grpPrimeMeridian.Controls.Add(this.cmbPrimeMeridianName);
            this.grpPrimeMeridian.Controls.Add(this.txtValue);
            this.grpPrimeMeridian.Controls.Add(this.label2);
            this.grpPrimeMeridian.Controls.Add(this.label1);
            this.grpPrimeMeridian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPrimeMeridian.Location = new System.Drawing.Point(0, 0);
            this.grpPrimeMeridian.Name = "grpPrimeMeridian";
            this.grpPrimeMeridian.Size = new System.Drawing.Size(300, 93);
            this.grpPrimeMeridian.TabIndex = 0;
            this.grpPrimeMeridian.TabStop = false;
            this.grpPrimeMeridian.Text = "本初子午线";
            // 
            // cmbPrimeMeridianName
            // 
            this.cmbPrimeMeridianName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPrimeMeridianName.FormattingEnabled = true;
            this.cmbPrimeMeridianName.Location = new System.Drawing.Point(89, 21);
            this.cmbPrimeMeridianName.Name = "cmbPrimeMeridianName";
            this.cmbPrimeMeridianName.Size = new System.Drawing.Size(175, 20);
            this.cmbPrimeMeridianName.TabIndex = 3;
            this.cmbPrimeMeridianName.Text = "<自定义>";
            this.cmbPrimeMeridianName.SelectedIndexChanged += new System.EventHandler(this.cmbPrimeMeridianName_SelectedIndexChanged);
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(131, 52);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(133, 21);
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
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "经度:";
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
            // PrimeMeridianUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPrimeMeridian);
            this.Name = "PrimeMeridianUC";
            this.Size = new System.Drawing.Size(300, 93);
            this.grpPrimeMeridian.ResumeLayout(false);
            this.grpPrimeMeridian.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPrimeMeridian;
        private System.Windows.Forms.ComboBox cmbPrimeMeridianName;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
