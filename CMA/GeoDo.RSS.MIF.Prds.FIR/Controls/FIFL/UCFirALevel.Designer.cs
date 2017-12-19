namespace GeoDo.RSS.MIF.Prds.FIR
{
    partial class UCFirALevel
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
            this.btnMinNear = new System.Windows.Forms.Button();
            this.btnMaxNear = new System.Windows.Forms.Button();
            this.txtMinNear = new System.Windows.Forms.TextBox();
            this.txtMaxNear = new System.Windows.Forms.TextBox();
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
            this.label1.Text = "近红外最大(非火区)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "近红外最小(过火区)";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMinNear);
            this.panel1.Controls.Add(this.btnMaxNear);
            this.panel1.Controls.Add(this.txtMinNear);
            this.panel1.Controls.Add(this.txtMaxNear);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(353, 53);
            this.panel1.TabIndex = 2;
            // 
            // btnMinNear
            // 
            this.btnMinNear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinNear.Location = new System.Drawing.Point(310, 28);
            this.btnMinNear.Name = "btnMinNear";
            this.btnMinNear.Size = new System.Drawing.Size(40, 23);
            this.btnMinNear.TabIndex = 5;
            this.btnMinNear.Text = "获取";
            this.btnMinNear.UseVisualStyleBackColor = true;
            this.btnMinNear.Click += new System.EventHandler(this.btnMinNear_Click);
            // 
            // btnMaxNear
            // 
            this.btnMaxNear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaxNear.Location = new System.Drawing.Point(310, 3);
            this.btnMaxNear.Name = "btnMaxNear";
            this.btnMaxNear.Size = new System.Drawing.Size(40, 23);
            this.btnMaxNear.TabIndex = 4;
            this.btnMaxNear.Text = "获取";
            this.btnMaxNear.UseVisualStyleBackColor = true;
            this.btnMaxNear.Click += new System.EventHandler(this.btnMaxNear_Click);
            // 
            // txtMinNear
            // 
            this.txtMinNear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinNear.Location = new System.Drawing.Point(122, 29);
            this.txtMinNear.Name = "txtMinNear";
            this.txtMinNear.Size = new System.Drawing.Size(182, 21);
            this.txtMinNear.TabIndex = 3;
            // 
            // txtMaxNear
            // 
            this.txtMaxNear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxNear.Location = new System.Drawing.Point(122, 4);
            this.txtMaxNear.Name = "txtMaxNear";
            this.txtMaxNear.Size = new System.Drawing.Size(182, 21);
            this.txtMaxNear.TabIndex = 2;
            // 
            // UCFirALevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "UCFirALevel";
            this.Size = new System.Drawing.Size(353, 53);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnMinNear;
        private System.Windows.Forms.Button btnMaxNear;
        private System.Windows.Forms.TextBox txtMinNear;
        private System.Windows.Forms.TextBox txtMaxNear;
    }
}
