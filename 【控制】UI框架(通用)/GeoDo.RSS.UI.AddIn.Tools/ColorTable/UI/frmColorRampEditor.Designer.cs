namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class frmColorRampEditor
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvalidColor = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnStatMinValueMaxValue = new System.Windows.Forms.Button();
            this.txtMaxValue = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMinValue = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnApplyMinValueMaxValue = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ucLinearColorRampEditor1 = new GeoDo.RSS.UI.AddIn.Tools.UCLinearColorRampEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinValue)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(483, 249);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(564, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(239, 244);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(6, 15);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(155, 160);
            this.listBox1.TabIndex = 7;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 206);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(70, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "保存";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "命名方案:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(74, 179);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(87, 21);
            this.textBox1.TabIndex = 10;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(92, 206);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(69, 23);
            this.btnDel.TabIndex = 11;
            this.btnDel.Text = "删除";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(258, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "在滑块上单击鼠标右键可删除颜色";
            // 
            // txtInvalidColor
            // 
            this.txtInvalidColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtInvalidColor.AutoSize = true;
            this.txtInvalidColor.BackColor = System.Drawing.Color.White;
            this.txtInvalidColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInvalidColor.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtInvalidColor.Location = new System.Drawing.Point(87, 203);
            this.txtInvalidColor.Name = "txtInvalidColor";
            this.txtInvalidColor.Size = new System.Drawing.Size(121, 21);
            this.txtInvalidColor.TabIndex = 13;
            this.txtInvalidColor.Text = "           ";
            this.txtInvalidColor.Click += new System.EventHandler(this.txtInvalidColor_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "无效值填充色";
            // 
            // btnStatMinValueMaxValue
            // 
            this.btnStatMinValueMaxValue.Location = new System.Drawing.Point(368, 18);
            this.btnStatMinValueMaxValue.Name = "btnStatMinValueMaxValue";
            this.btnStatMinValueMaxValue.Size = new System.Drawing.Size(76, 23);
            this.btnStatMinValueMaxValue.TabIndex = 25;
            this.btnStatMinValueMaxValue.Text = "统计";
            this.btnStatMinValueMaxValue.UseVisualStyleBackColor = true;
            this.btnStatMinValueMaxValue.Click += new System.EventHandler(this.btnStatMinValueMaxValue_Click);
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(231, 20);
            this.txtMaxValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txtMaxValue.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(71, 21);
            this.txtMaxValue.TabIndex = 24;
            this.txtMaxValue.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(160, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 23;
            this.label6.Text = "最大有效值";
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(83, 20);
            this.txtMinValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txtMinValue.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(71, 21);
            this.txtMinValue.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 21;
            this.label4.Text = "最小有效值";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDel);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 237);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "已保存的方案";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnApplyMinValueMaxValue);
            this.groupBox2.Controls.Add(this.btnStatMinValueMaxValue);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtMinValue);
            this.groupBox2.Controls.Add(this.txtMaxValue);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(189, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(450, 58);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "有效值范围";
            this.groupBox2.Visible = false;
            // 
            // btnApplyMinValueMaxValue
            // 
            this.btnApplyMinValueMaxValue.Location = new System.Drawing.Point(308, 18);
            this.btnApplyMinValueMaxValue.Name = "btnApplyMinValueMaxValue";
            this.btnApplyMinValueMaxValue.Size = new System.Drawing.Size(55, 23);
            this.btnApplyMinValueMaxValue.TabIndex = 26;
            this.btnApplyMinValueMaxValue.Text = "应用";
            this.btnApplyMinValueMaxValue.UseVisualStyleBackColor = true;
            this.btnApplyMinValueMaxValue.Click += new System.EventHandler(this.btnApplyMinValueMaxValue_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtInvalidColor);
            this.groupBox3.Controls.Add(this.ucLinearColorRampEditor1);
            this.groupBox3.Location = new System.Drawing.Point(190, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(449, 229);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "色表编辑";
            // 
            // ucLinearColorRampEditor1
            // 
            this.ucLinearColorRampEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucLinearColorRampEditor1.Location = new System.Drawing.Point(3, 17);
            this.ucLinearColorRampEditor1.MaxValue = 0;
            this.ucLinearColorRampEditor1.MinValue = 0;
            this.ucLinearColorRampEditor1.Name = "ucLinearColorRampEditor1";
            this.ucLinearColorRampEditor1.Size = new System.Drawing.Size(443, 209);
            this.ucLinearColorRampEditor1.TabIndex = 4;
            // 
            // frmColorRampEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 279);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmColorRampEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "线性梯度填色(单波段)...";
            this.Load += new System.EventHandler(this.frmColorRampEditor_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinValue)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private UCLinearColorRampEditor ucLinearColorRampEditor1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtInvalidColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnStatMinValueMaxValue;
        private System.Windows.Forms.NumericUpDown txtMaxValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtMinValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnApplyMinValueMaxValue;
    }
}