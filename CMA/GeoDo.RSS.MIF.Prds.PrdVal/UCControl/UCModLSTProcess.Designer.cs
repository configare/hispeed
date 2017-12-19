namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    partial class UCModLSTProcess
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
            this.txtinput = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.grbLatLon = new System.Windows.Forms.GroupBox();
            this.txtRes = new System.Windows.Forms.TextBox();
            this.labRes = new System.Windows.Forms.Label();
            this.btnOpenData = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMinX = new System.Windows.Forms.TextBox();
            this.txtMinY = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtMaxX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMaxY = new System.Windows.Forms.TextBox();
            this.grbMethods = new System.Windows.Forms.GroupBox();
            this.chbHistogram = new System.Windows.Forms.CheckBox();
            this.chbRmse = new System.Windows.Forms.CheckBox();
            this.chbTimeSeq = new System.Windows.Forms.CheckBox();
            this.chbScatter = new System.Windows.Forms.CheckBox();
            this.txtToVal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rdioData = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutdir = new System.Windows.Forms.TextBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.grbLatLon.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grbMethods.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据路径";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtinput
            // 
            this.txtinput.Location = new System.Drawing.Point(68, 10);
            this.txtinput.Name = "txtinput";
            this.txtinput.Size = new System.Drawing.Size(170, 21);
            this.txtinput.TabIndex = 1;
            this.txtinput.Text = "E:\\临时工作\\产品精度评估\\MODLST\\500836287";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(248, 10);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(33, 21);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // grbLatLon
            // 
            this.grbLatLon.Controls.Add(this.txtRegion);
            this.grbLatLon.Controls.Add(this.label4);
            this.grbLatLon.Controls.Add(this.txtRes);
            this.grbLatLon.Controls.Add(this.labRes);
            this.grbLatLon.Controls.Add(this.btnOpenData);
            this.grbLatLon.Controls.Add(this.groupBox1);
            this.grbLatLon.Controls.Add(this.txtToVal);
            this.grbLatLon.Controls.Add(this.label2);
            this.grbLatLon.Controls.Add(this.rdioData);
            this.grbLatLon.Controls.Add(this.radioButton1);
            this.grbLatLon.Location = new System.Drawing.Point(5, 72);
            this.grbLatLon.Name = "grbLatLon";
            this.grbLatLon.Size = new System.Drawing.Size(273, 227);
            this.grbLatLon.TabIndex = 28;
            this.grbLatLon.TabStop = false;
            this.grbLatLon.Text = "输出参数";
            this.grbLatLon.Enter += new System.EventHandler(this.grbLatLon_Enter);
            // 
            // txtRes
            // 
            this.txtRes.Location = new System.Drawing.Point(77, 196);
            this.txtRes.Name = "txtRes";
            this.txtRes.Size = new System.Drawing.Size(43, 21);
            this.txtRes.TabIndex = 32;
            this.txtRes.Text = "0.01";
            // 
            // labRes
            // 
            this.labRes.AutoSize = true;
            this.labRes.Location = new System.Drawing.Point(9, 199);
            this.labRes.Name = "labRes";
            this.labRes.Size = new System.Drawing.Size(65, 12);
            this.labRes.TabIndex = 31;
            this.labRes.Text = "输出分辨率";
            // 
            // btnOpenData
            // 
            this.btnOpenData.Enabled = false;
            this.btnOpenData.Location = new System.Drawing.Point(234, 47);
            this.btnOpenData.Name = "btnOpenData";
            this.btnOpenData.Size = new System.Drawing.Size(33, 21);
            this.btnOpenData.TabIndex = 30;
            this.btnOpenData.Text = "...";
            this.btnOpenData.UseVisualStyleBackColor = true;
            this.btnOpenData.Click += new System.EventHandler(this.btnOpenData_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMinX);
            this.groupBox1.Controls.Add(this.txtMinY);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtMaxX);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtMaxY);
            this.groupBox1.Controls.Add(this.grbMethods);
            this.groupBox1.Location = new System.Drawing.Point(6, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 102);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "经纬度范围";
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(42, 45);
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.Size = new System.Drawing.Size(54, 21);
            this.txtMinX.TabIndex = 12;
            this.txtMinX.Text = "70";
            // 
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(116, 69);
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.Size = new System.Drawing.Size(54, 21);
            this.txtMinY.TabIndex = 15;
            this.txtMinY.Text = "10";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(81, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "MinY";
            // 
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(189, 45);
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.Size = new System.Drawing.Size(54, 21);
            this.txtMaxX.TabIndex = 14;
            this.txtMaxX.Text = "142";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(157, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 13;
            this.label8.Text = "MaxX";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "MinX";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(81, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "MaxY";
            // 
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(115, 19);
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.Size = new System.Drawing.Size(54, 21);
            this.txtMaxY.TabIndex = 10;
            this.txtMaxY.Text = "56";
            // 
            // grbMethods
            // 
            this.grbMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grbMethods.Controls.Add(this.chbHistogram);
            this.grbMethods.Controls.Add(this.chbRmse);
            this.grbMethods.Controls.Add(this.chbTimeSeq);
            this.grbMethods.Controls.Add(this.chbScatter);
            this.grbMethods.Location = new System.Drawing.Point(152, 102);
            this.grbMethods.Name = "grbMethods";
            this.grbMethods.Size = new System.Drawing.Size(255, 70);
            this.grbMethods.TabIndex = 17;
            this.grbMethods.TabStop = false;
            this.grbMethods.Text = "评估方式";
            // 
            // chbHistogram
            // 
            this.chbHistogram.AutoSize = true;
            this.chbHistogram.Location = new System.Drawing.Point(19, 42);
            this.chbHistogram.Name = "chbHistogram";
            this.chbHistogram.Size = new System.Drawing.Size(84, 16);
            this.chbHistogram.TabIndex = 3;
            this.chbHistogram.Text = "偏差直方图";
            this.chbHistogram.UseVisualStyleBackColor = true;
            // 
            // chbRmse
            // 
            this.chbRmse.AutoSize = true;
            this.chbRmse.Location = new System.Drawing.Point(133, 42);
            this.chbRmse.Name = "chbRmse";
            this.chbRmse.Size = new System.Drawing.Size(120, 16);
            this.chbRmse.TabIndex = 2;
            this.chbRmse.Text = "均方根误差(RMSE)";
            this.chbRmse.UseVisualStyleBackColor = true;
            // 
            // chbTimeSeq
            // 
            this.chbTimeSeq.AutoSize = true;
            this.chbTimeSeq.Location = new System.Drawing.Point(133, 20);
            this.chbTimeSeq.Name = "chbTimeSeq";
            this.chbTimeSeq.Size = new System.Drawing.Size(84, 16);
            this.chbTimeSeq.TabIndex = 1;
            this.chbTimeSeq.Text = "时间序列图";
            this.chbTimeSeq.UseVisualStyleBackColor = true;
            // 
            // chbScatter
            // 
            this.chbScatter.AutoSize = true;
            this.chbScatter.Location = new System.Drawing.Point(19, 20);
            this.chbScatter.Name = "chbScatter";
            this.chbScatter.Size = new System.Drawing.Size(60, 16);
            this.chbScatter.TabIndex = 0;
            this.chbScatter.Text = "散点图";
            this.chbScatter.UseVisualStyleBackColor = true;
            // 
            // txtToVal
            // 
            this.txtToVal.Enabled = false;
            this.txtToVal.Location = new System.Drawing.Point(79, 47);
            this.txtToVal.Name = "txtToVal";
            this.txtToVal.Size = new System.Drawing.Size(149, 21);
            this.txtToVal.TabIndex = 19;
            this.txtToVal.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "数据文件";
            // 
            // rdioData
            // 
            this.rdioData.AutoSize = true;
            this.rdioData.Location = new System.Drawing.Point(15, 20);
            this.rdioData.Name = "rdioData";
            this.rdioData.Size = new System.Drawing.Size(71, 16);
            this.rdioData.TabIndex = 17;
            this.rdioData.TabStop = true;
            this.rdioData.Text = "根据文件";
            this.rdioData.UseVisualStyleBackColor = true;
            this.rdioData.CheckedChanged += new System.EventHandler(this.rdioData_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(146, 20);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(59, 16);
            this.radioButton1.TabIndex = 16;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "自定义";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 29;
            this.label3.Text = "输出路径";
            // 
            // txtOutdir
            // 
            this.txtOutdir.Location = new System.Drawing.Point(68, 44);
            this.txtOutdir.Name = "txtOutdir";
            this.txtOutdir.Size = new System.Drawing.Size(169, 21);
            this.txtOutdir.TabIndex = 30;
            this.txtOutdir.Text = "E:\\临时工作\\产品精度评估\\MODISLST225days";
            this.txtOutdir.TextChanged += new System.EventHandler(this.txtOutdir_TextChanged);
            // 
            // btnOutDir
            // 
            this.btnOutDir.Location = new System.Drawing.Point(248, 43);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(33, 21);
            this.btnOutDir.TabIndex = 31;
            this.btnOutDir.Text = "...";
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(200, 305);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 32;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 33;
            this.label4.Text = "区域名称";
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(191, 196);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(72, 21);
            this.txtRegion.TabIndex = 34;
            // 
            // UCModLSTProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.txtOutdir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.grbLatLon);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtinput);
            this.Controls.Add(this.label1);
            this.Name = "UCModLSTProcess";
            this.Size = new System.Drawing.Size(287, 337);
            this.grbLatLon.ResumeLayout(false);
            this.grbLatLon.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbMethods.ResumeLayout(false);
            this.grbMethods.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtinput;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.GroupBox grbLatLon;
        private System.Windows.Forms.Button btnOpenData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMinX;
        private System.Windows.Forms.TextBox txtMinY;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtMaxX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMaxY;
        private System.Windows.Forms.GroupBox grbMethods;
        private System.Windows.Forms.CheckBox chbHistogram;
        private System.Windows.Forms.CheckBox chbRmse;
        private System.Windows.Forms.CheckBox chbTimeSeq;
        private System.Windows.Forms.CheckBox chbScatter;
        private System.Windows.Forms.TextBox txtToVal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdioData;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutdir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.TextBox txtRes;
        private System.Windows.Forms.Label labRes;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.Label label4;
    }
}
