namespace GeoDo.RSS.MIF.Prds.DRT
{
    partial class UCTVDIPanel
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
            this.gb = new System.Windows.Forms.GroupBox();
            this.btNH = new System.Windows.Forms.Button();
            this.txtMinB = new System.Windows.Forms.TextBox();
            this.txtMinA = new System.Windows.Forms.TextBox();
            this.txtMaxB = new System.Windows.Forms.TextBox();
            this.txtMaxA = new System.Windows.Forms.TextBox();
            this.B = new System.Windows.Forms.Label();
            this.lbb = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.A = new System.Windows.Forms.Label();
            this.lb = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.demFileSelect = new GeoDo.RSS.MIF.Prds.DRT.UCFileSelectBase();
            this.LstFileSelect = new GeoDo.RSS.MIF.Prds.DRT.UCFileSelectBase();
            this.ndviFileSelect = new GeoDo.RSS.MIF.Prds.DRT.UCFileSelectBase();
            this.ucExpCoefficientBase1 = new GeoDo.RSS.MIF.Prds.DRT.UCExpCoefficientBase();
            this.gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb
            // 
            this.gb.Controls.Add(this.btNH);
            this.gb.Controls.Add(this.txtMinB);
            this.gb.Controls.Add(this.txtMinA);
            this.gb.Controls.Add(this.txtMaxB);
            this.gb.Controls.Add(this.txtMaxA);
            this.gb.Controls.Add(this.B);
            this.gb.Controls.Add(this.lbb);
            this.gb.Controls.Add(this.label4);
            this.gb.Controls.Add(this.A);
            this.gb.Controls.Add(this.lb);
            this.gb.Controls.Add(this.label1);
            this.gb.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb.Location = new System.Drawing.Point(0, 90);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(271, 106);
            this.gb.TabIndex = 3;
            this.gb.TabStop = false;
            this.gb.Text = "干湿边拟合";
            // 
            // btNH
            // 
            this.btNH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btNH.Location = new System.Drawing.Point(201, 76);
            this.btNH.Name = "btNH";
            this.btNH.Size = new System.Drawing.Size(65, 23);
            this.btNH.TabIndex = 2;
            this.btNH.Text = "拟合";
            this.btNH.UseVisualStyleBackColor = true;
            this.btNH.Click += new System.EventHandler(this.btNH_Click);
            // 
            // txtMinB
            // 
            this.txtMinB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinB.Enabled = false;
            this.txtMinB.Location = new System.Drawing.Point(183, 48);
            this.txtMinB.Name = "txtMinB";
            this.txtMinB.Size = new System.Drawing.Size(82, 21);
            this.txtMinB.TabIndex = 1;
            // 
            // txtMinA
            // 
            this.txtMinA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinA.Enabled = false;
            this.txtMinA.Location = new System.Drawing.Point(80, 49);
            this.txtMinA.Name = "txtMinA";
            this.txtMinA.Size = new System.Drawing.Size(82, 21);
            this.txtMinA.TabIndex = 1;
            // 
            // txtMaxB
            // 
            this.txtMaxB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxB.Enabled = false;
            this.txtMaxB.Location = new System.Drawing.Point(183, 23);
            this.txtMaxB.Name = "txtMaxB";
            this.txtMaxB.Size = new System.Drawing.Size(82, 21);
            this.txtMaxB.TabIndex = 1;
            // 
            // txtMaxA
            // 
            this.txtMaxA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxA.Enabled = false;
            this.txtMaxA.Location = new System.Drawing.Point(80, 23);
            this.txtMaxA.Name = "txtMaxA";
            this.txtMaxA.Size = new System.Drawing.Size(82, 21);
            this.txtMaxA.TabIndex = 1;
            // 
            // B
            // 
            this.B.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B.AutoSize = true;
            this.B.Location = new System.Drawing.Point(166, 53);
            this.B.Name = "B";
            this.B.Size = new System.Drawing.Size(11, 12);
            this.B.TabIndex = 0;
            this.B.Text = "B";
            // 
            // lbb
            // 
            this.lbb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbb.AutoSize = true;
            this.lbb.Location = new System.Drawing.Point(166, 27);
            this.lbb.Name = "lbb";
            this.lbb.Size = new System.Drawing.Size(11, 12);
            this.lbb.TabIndex = 0;
            this.lbb.Text = "B";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "最小端";
            // 
            // A
            // 
            this.A.AutoSize = true;
            this.A.Location = new System.Drawing.Point(61, 53);
            this.A.Name = "A";
            this.A.Size = new System.Drawing.Size(11, 12);
            this.A.TabIndex = 0;
            this.A.Text = "A";
            // 
            // lb
            // 
            this.lb.AutoSize = true;
            this.lb.Location = new System.Drawing.Point(61, 27);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(11, 12);
            this.lb.TabIndex = 0;
            this.lb.Text = "A";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "最大端";
            // 
            // demFileSelect
            // 
            this.demFileSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.demFileSelect.Location = new System.Drawing.Point(0, 60);
            this.demFileSelect.Name = "demFileSelect";
            this.demFileSelect.Size = new System.Drawing.Size(271, 30);
            this.demFileSelect.TabIndex = 2;
            // 
            // LstFileSelect
            // 
            this.LstFileSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.LstFileSelect.Location = new System.Drawing.Point(0, 30);
            this.LstFileSelect.Name = "LstFileSelect";
            this.LstFileSelect.Size = new System.Drawing.Size(271, 30);
            this.LstFileSelect.TabIndex = 1;
            // 
            // ndviFileSelect
            // 
            this.ndviFileSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.ndviFileSelect.Location = new System.Drawing.Point(0, 0);
            this.ndviFileSelect.Name = "ndviFileSelect";
            this.ndviFileSelect.Size = new System.Drawing.Size(271, 30);
            this.ndviFileSelect.TabIndex = 0;
            // 
            // ucExpCoefficientBase1
            // 
            this.ucExpCoefficientBase1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucExpCoefficientBase1.Location = new System.Drawing.Point(0, 196);
            this.ucExpCoefficientBase1.Name = "ucExpCoefficientBase1";
            this.ucExpCoefficientBase1.Size = new System.Drawing.Size(271, 30);
            this.ucExpCoefficientBase1.TabIndex = 3;
            // 
            // UCTVDIPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucExpCoefficientBase1);
            this.Controls.Add(this.gb);
            this.Controls.Add(this.demFileSelect);
            this.Controls.Add(this.LstFileSelect);
            this.Controls.Add(this.ndviFileSelect);
            this.Name = "UCTVDIPanel";
            this.Size = new System.Drawing.Size(271, 231);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UCFileSelectBase ndviFileSelect;
        private UCFileSelectBase LstFileSelect;
        private UCFileSelectBase demFileSelect;
        private System.Windows.Forms.GroupBox gb;
        private System.Windows.Forms.TextBox txtMaxA;
        private System.Windows.Forms.Label B;
        private System.Windows.Forms.Label lbb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label A;
        private System.Windows.Forms.Label lb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMinB;
        private System.Windows.Forms.TextBox txtMinA;
        private System.Windows.Forms.TextBox txtMaxB;
        private System.Windows.Forms.Button btNH;
        private UCExpCoefficientBase ucExpCoefficientBase1;

    }
}
