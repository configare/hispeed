namespace GeoDo.RSS.MIF.Prds.DRT
{
    partial class frmHistograms
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
            this.label1 = new System.Windows.Forms.Label();
            this.nudLength = new System.Windows.Forms.NumericUpDown();
            this.btCancel = new System.Windows.Forms.Button();
            this.txtNdviMax = new System.Windows.Forms.TextBox();
            this.txtNdviMin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLstMin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLstMax = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtBMin = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBMax = new System.Windows.Forms.TextBox();
            this.txtAMin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAMax = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btGetAB = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.nudHGYZ = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nudLimit = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nudLstPC = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHGYZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLstPC)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "回归阈值";
            // 
            // nudLength
            // 
            this.nudLength.Location = new System.Drawing.Point(58, 18);
            this.nudLength.Name = "nudLength";
            this.nudLength.Size = new System.Drawing.Size(96, 21);
            this.nudLength.TabIndex = 1;
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(163, 193);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "取消";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // txtNdviMax
            // 
            this.txtNdviMax.Location = new System.Drawing.Point(209, 18);
            this.txtNdviMax.Name = "txtNdviMax";
            this.txtNdviMax.Size = new System.Drawing.Size(96, 21);
            this.txtNdviMax.TabIndex = 3;
            // 
            // txtNdviMin
            // 
            this.txtNdviMin.Location = new System.Drawing.Point(58, 18);
            this.txtNdviMin.Name = "txtNdviMin";
            this.txtNdviMin.Size = new System.Drawing.Size(96, 21);
            this.txtNdviMin.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "最大值";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "最小值";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtNdviMin);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtNdviMax);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(4, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 50);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NDVI";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtLstMin);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtLstMax);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(4, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(314, 50);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LST";
            // 
            // txtLstMin
            // 
            this.txtLstMin.Location = new System.Drawing.Point(58, 18);
            this.txtLstMin.Name = "txtLstMin";
            this.txtLstMin.Size = new System.Drawing.Size(96, 21);
            this.txtLstMin.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "最大值";
            // 
            // txtLstMax
            // 
            this.txtLstMax.Location = new System.Drawing.Point(209, 18);
            this.txtLstMax.Name = "txtLstMax";
            this.txtLstMax.Size = new System.Drawing.Size(96, 21);
            this.txtLstMax.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "最小值";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtBMin);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.txtBMax);
            this.groupBox3.Controls.Add(this.txtAMin);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtAMax);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(4, 218);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 77);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "系数";
            // 
            // txtBMin
            // 
            this.txtBMin.Location = new System.Drawing.Point(209, 47);
            this.txtBMin.Name = "txtBMin";
            this.txtBMin.ReadOnly = true;
            this.txtBMin.Size = new System.Drawing.Size(96, 21);
            this.txtBMin.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "最小端";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "最大端";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(52, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(11, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "A";
            // 
            // txtBMax
            // 
            this.txtBMax.Location = new System.Drawing.Point(209, 18);
            this.txtBMax.Name = "txtBMax";
            this.txtBMax.ReadOnly = true;
            this.txtBMax.Size = new System.Drawing.Size(96, 21);
            this.txtBMax.TabIndex = 3;
            // 
            // txtAMin
            // 
            this.txtAMin.Location = new System.Drawing.Point(74, 47);
            this.txtAMin.Name = "txtAMin";
            this.txtAMin.ReadOnly = true;
            this.txtAMin.Size = new System.Drawing.Size(96, 21);
            this.txtAMin.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(52, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "A";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(188, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "B";
            // 
            // txtAMax
            // 
            this.txtAMax.Location = new System.Drawing.Point(74, 18);
            this.txtAMax.Name = "txtAMax";
            this.txtAMax.ReadOnly = true;
            this.txtAMax.Size = new System.Drawing.Size(96, 21);
            this.txtAMax.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(188, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "B";
            // 
            // btGetAB
            // 
            this.btGetAB.Location = new System.Drawing.Point(243, 193);
            this.btGetAB.Name = "btGetAB";
            this.btGetAB.Size = new System.Drawing.Size(75, 23);
            this.btGetAB.TabIndex = 2;
            this.btGetAB.Text = "分析";
            this.btGetAB.UseVisualStyleBackColor = true;
            this.btGetAB.Click += new System.EventHandler(this.btGetAB_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(83, 193);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "步长";
            // 
            // nudHGYZ
            // 
            this.nudHGYZ.DecimalPlaces = 1;
            this.nudHGYZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudHGYZ.Location = new System.Drawing.Point(209, 53);
            this.nudHGYZ.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHGYZ.Name = "nudHGYZ";
            this.nudHGYZ.Size = new System.Drawing.Size(96, 21);
            this.nudHGYZ.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "相对系数";
            // 
            // nudLimit
            // 
            this.nudLimit.DecimalPlaces = 1;
            this.nudLimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudLimit.Location = new System.Drawing.Point(58, 53);
            this.nudLimit.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLimit.Name = "nudLimit";
            this.nudLimit.Size = new System.Drawing.Size(96, 21);
            this.nudLimit.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(157, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "LST频次";
            // 
            // nudLstPC
            // 
            this.nudLstPC.Location = new System.Drawing.Point(209, 18);
            this.nudLstPC.Name = "nudLstPC";
            this.nudLstPC.Size = new System.Drawing.Size(96, 21);
            this.nudLstPC.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.nudHGYZ);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.nudLength);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.nudLimit);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.nudLstPC);
            this.groupBox4.Location = new System.Drawing.Point(4, 107);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(314, 79);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "参数设置 ";
            // 
            // frmHistograms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 296);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btGetAB);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmHistograms";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "干湿边拟合";
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHGYZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLstPC)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudLength;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.TextBox txtNdviMax;
        private System.Windows.Forms.TextBox txtNdviMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLstMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLstMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtBMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAMax;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btGetAB;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.TextBox txtBMin;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAMin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudHGYZ;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudLimit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudLstPC;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}