namespace GeoDo.RSS.MIF.Prds.FLD
{
    partial class UCComputeMixPixel
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLand = new System.Windows.Forms.Button();
            this.btnFld = new System.Windows.Forms.Button();
            this.txtLand = new System.Windows.Forms.TextBox();
            this.txtFld = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "近红外水体端元值：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "近红外陆地端元值：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnLand);
            this.panel1.Controls.Add(this.btnFld);
            this.panel1.Controls.Add(this.txtLand);
            this.panel1.Controls.Add(this.txtFld);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(263, 81);
            this.panel1.TabIndex = 2;
            // 
            // btnLand
            // 
            this.btnLand.Location = new System.Drawing.Point(219, 28);
            this.btnLand.Name = "btnLand";
            this.btnLand.Size = new System.Drawing.Size(40, 23);
            this.btnLand.TabIndex = 5;
            this.btnLand.Text = "获取";
            this.btnLand.UseVisualStyleBackColor = true;
            this.btnLand.Click += new System.EventHandler(this.btnLand_Click);
            // 
            // btnFld
            // 
            this.btnFld.Location = new System.Drawing.Point(219, 3);
            this.btnFld.Name = "btnFld";
            this.btnFld.Size = new System.Drawing.Size(40, 23);
            this.btnFld.TabIndex = 4;
            this.btnFld.Text = "获取";
            this.btnFld.UseVisualStyleBackColor = true;
            this.btnFld.Click += new System.EventHandler(this.btnFld_Click);
            // 
            // txtLand
            // 
            this.txtLand.Location = new System.Drawing.Point(113, 29);
            this.txtLand.Name = "txtLand";
            this.txtLand.Size = new System.Drawing.Size(100, 21);
            this.txtLand.TabIndex = 3;
            // 
            // txtFld
            // 
            this.txtFld.Location = new System.Drawing.Point(113, 4);
            this.txtFld.Name = "txtFld";
            this.txtFld.Size = new System.Drawing.Size(100, 21);
            this.txtFld.TabIndex = 2;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(45, 54);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(154, 54);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // UCComputeMixPixel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "UCComputeMixPixel";
            this.Size = new System.Drawing.Size(263, 81);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLand;
        private System.Windows.Forms.Button btnFld;
        private System.Windows.Forms.TextBox txtLand;
        private System.Windows.Forms.TextBox txtFld;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}
