namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCCoverDegreeRegion
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMinConvertDegree = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaxConvertDegree = new System.Windows.Forms.TextBox();
            this.lsbDegreeRegion = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 126);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(94, 126);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "最小覆盖度";
            // 
            // txtMinConvertDegree
            // 
            this.txtMinConvertDegree.Location = new System.Drawing.Point(72, 6);
            this.txtMinConvertDegree.Name = "txtMinConvertDegree";
            this.txtMinConvertDegree.Size = new System.Drawing.Size(56, 21);
            this.txtMinConvertDegree.TabIndex = 8;
            this.txtMinConvertDegree.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "最大覆盖度";
            // 
            // txtMaxConvertDegree
            // 
            this.txtMaxConvertDegree.Location = new System.Drawing.Point(211, 6);
            this.txtMaxConvertDegree.Name = "txtMaxConvertDegree";
            this.txtMaxConvertDegree.Size = new System.Drawing.Size(56, 21);
            this.txtMaxConvertDegree.TabIndex = 10;
            this.txtMaxConvertDegree.Text = "1";
            // 
            // lsbDegreeRegion
            // 
            this.lsbDegreeRegion.FormattingEnabled = true;
            this.lsbDegreeRegion.ItemHeight = 12;
            this.lsbDegreeRegion.Location = new System.Drawing.Point(3, 37);
            this.lsbDegreeRegion.Name = "lsbDegreeRegion";
            this.lsbDegreeRegion.Size = new System.Drawing.Size(264, 76);
            this.lsbDegreeRegion.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(187, 125);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtMinConvertDegree);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtMaxConvertDegree);
            this.panel1.Controls.Add(this.lsbDegreeRegion);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(277, 177);
            this.panel1.TabIndex = 13;
            // 
            // UCCoverDegreeRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "UCCoverDegreeRegion";
            this.Size = new System.Drawing.Size(277, 177);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMinConvertDegree;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMaxConvertDegree;
        private System.Windows.Forms.ListBox lsbDegreeRegion;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
    }
}
