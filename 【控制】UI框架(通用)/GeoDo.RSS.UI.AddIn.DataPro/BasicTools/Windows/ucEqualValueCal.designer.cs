namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class ucEqualValueCal
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucEqualValueCal));
            this.btnGetShapeFile = new System.Windows.Forms.Button();
            this.btnResFile = new System.Windows.Forms.Button();
            this.txtResFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbBoundCount = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnOutputShapeFile = new System.Windows.Forms.Button();
            this.txtOutputtShapeFile = new System.Windows.Forms.TextBox();
            this.brnFromMemory = new System.Windows.Forms.RadioButton();
            this.btnFromFile = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIgnore = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIgnore)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetShapeFile
            // 
            this.btnGetShapeFile.Location = new System.Drawing.Point(280, 264);
            this.btnGetShapeFile.Name = "btnGetShapeFile";
            this.btnGetShapeFile.Size = new System.Drawing.Size(78, 23);
            this.btnGetShapeFile.TabIndex = 13;
            this.btnGetShapeFile.Text = "确定";
            this.btnGetShapeFile.UseVisualStyleBackColor = true;
            this.btnGetShapeFile.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnResFile
            // 
            this.btnResFile.Enabled = false;
            this.btnResFile.Image = ((System.Drawing.Image)(resources.GetObject("btnResFile.Image")));
            this.btnResFile.Location = new System.Drawing.Point(401, 16);
            this.btnResFile.Name = "btnResFile";
            this.btnResFile.Size = new System.Drawing.Size(23, 23);
            this.btnResFile.TabIndex = 11;
            this.btnResFile.UseVisualStyleBackColor = true;
            this.btnResFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtResFile
            // 
            this.txtResFile.Enabled = false;
            this.txtResFile.Location = new System.Drawing.Point(55, 17);
            this.txtResFile.Name = "txtResFile";
            this.txtResFile.ReadOnly = true;
            this.txtResFile.Size = new System.Drawing.Size(342, 21);
            this.txtResFile.TabIndex = 8;
            this.txtResFile.TextChanged += new System.EventHandler(this.txtResFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "源图像";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnResFile);
            this.groupBox1.Controls.Add(this.txtResFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 53);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图像路径";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbBoundCount);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(12, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(431, 44);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "像素范围";
            // 
            // cmbBoundCount
            // 
            this.cmbBoundCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoundCount.FormattingEnabled = true;
            this.cmbBoundCount.Location = new System.Drawing.Point(89, 17);
            this.cmbBoundCount.Name = "cmbBoundCount";
            this.cmbBoundCount.Size = new System.Drawing.Size(49, 20);
            this.cmbBoundCount.TabIndex = 27;
            this.cmbBoundCount.SelectedValueChanged += new System.EventHandler(this.cmbBoundCount_SelectedValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 25;
            this.label9.Text = "选择通道";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDown3);
            this.groupBox3.Controls.Add(this.numericUpDown2);
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 156);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(432, 46);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "等值线设置";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(368, 14);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(49, 21);
            this.numericUpDown3.TabIndex = 21;
            this.numericUpDown3.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(235, 14);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            30000,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(49, 21);
            this.numericUpDown2.TabIndex = 18;
            this.numericUpDown2.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(89, 13);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            30000,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(49, 21);
            this.numericUpDown1.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(300, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "等值线间隔";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(155, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "等值线最大值";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "等值线最小值";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(364, 264);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(78, 23);
            this.button5.TabIndex = 17;
            this.button5.Text = "取消";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnOutputShapeFile);
            this.groupBox4.Controls.Add(this.txtOutputtShapeFile);
            this.groupBox4.Location = new System.Drawing.Point(12, 212);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(432, 45);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "输出ShapeFiles矢量文件名";
            // 
            // btnOutputShapeFile
            // 
            this.btnOutputShapeFile.Image = ((System.Drawing.Image)(resources.GetObject("btnOutputShapeFile.Image")));
            this.btnOutputShapeFile.Location = new System.Drawing.Point(401, 16);
            this.btnOutputShapeFile.Name = "btnOutputShapeFile";
            this.btnOutputShapeFile.Size = new System.Drawing.Size(23, 23);
            this.btnOutputShapeFile.TabIndex = 14;
            this.btnOutputShapeFile.UseVisualStyleBackColor = true;
            this.btnOutputShapeFile.Click += new System.EventHandler(this.btnOutputShapeFile_Click);
            // 
            // txtOutputtShapeFile
            // 
            this.txtOutputtShapeFile.Location = new System.Drawing.Point(9, 17);
            this.txtOutputtShapeFile.Name = "txtOutputtShapeFile";
            this.txtOutputtShapeFile.Size = new System.Drawing.Size(388, 21);
            this.txtOutputtShapeFile.TabIndex = 13;
            this.txtOutputtShapeFile.DoubleClick += new System.EventHandler(this.txtOutputtShapeFile_DoubleClick);
            // 
            // brnFromMemory
            // 
            this.brnFromMemory.AutoSize = true;
            this.brnFromMemory.Checked = true;
            this.brnFromMemory.Location = new System.Drawing.Point(13, 13);
            this.brnFromMemory.Name = "brnFromMemory";
            this.brnFromMemory.Size = new System.Drawing.Size(155, 16);
            this.brnFromMemory.TabIndex = 19;
            this.brnFromMemory.TabStop = true;
            this.brnFromMemory.Text = "使用当前显示图像作为源";
            this.brnFromMemory.UseVisualStyleBackColor = true;
            this.brnFromMemory.CheckedChanged += new System.EventHandler(this.brnFromMemory_CheckedChanged);
            // 
            // btnFromFile
            // 
            this.btnFromFile.AutoSize = true;
            this.btnFromFile.Location = new System.Drawing.Point(216, 13);
            this.btnFromFile.Name = "btnFromFile";
            this.btnFromFile.Size = new System.Drawing.Size(95, 16);
            this.btnFromFile.TabIndex = 20;
            this.btnFromFile.Text = "新打开源图像";
            this.btnFromFile.UseVisualStyleBackColor = true;
            this.btnFromFile.CheckedChanged += new System.EventHandler(this.btnFromFile_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 268);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "忽略少于";
            // 
            // txtIgnore
            // 
            this.txtIgnore.Location = new System.Drawing.Point(70, 264);
            this.txtIgnore.Name = "txtIgnore";
            this.txtIgnore.Size = new System.Drawing.Size(52, 21);
            this.txtIgnore.TabIndex = 22;
            this.txtIgnore.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(128, 268);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "个点的线";
            // 
            // ucEqualValueCal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtIgnore);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnFromFile);
            this.Controls.Add(this.brnFromMemory);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnGetShapeFile);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "ucEqualValueCal";
            this.Size = new System.Drawing.Size(454, 293);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtIgnore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetShapeFile;
        private System.Windows.Forms.Button btnResFile;
        private System.Windows.Forms.TextBox txtResFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnOutputShapeFile;
        private System.Windows.Forms.TextBox txtOutputtShapeFile;
        private System.Windows.Forms.RadioButton brnFromMemory;
        private System.Windows.Forms.RadioButton btnFromFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtIgnore;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbBoundCount;
    }
}