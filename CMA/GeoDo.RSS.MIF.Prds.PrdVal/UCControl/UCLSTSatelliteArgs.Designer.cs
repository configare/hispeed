namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    partial class UCLSTSatelliteArgs
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnOpen2 = new System.Windows.Forms.Button();
            this.txtForVal1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtToVal = new System.Windows.Forms.TextBox();
            this.btnOpen1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtforinvalid = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtinvalid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblmsg = new System.Windows.Forms.Label();
            this.txtColumn = new System.Windows.Forms.TextBox();
            this.showcolumn = new System.Windows.Forms.Label();
            this.chHistogram = new System.Windows.Forms.CheckBox();
            this.chRmse = new System.Windows.Forms.CheckBox();
            this.chScatter = new System.Windows.Forms.CheckBox();
            this.grbMethods = new System.Windows.Forms.GroupBox();
            this.chbHistogram = new System.Windows.Forms.CheckBox();
            this.chbRmse = new System.Windows.Forms.CheckBox();
            this.chbTimeSeq = new System.Windows.Forms.CheckBox();
            this.chbScatter = new System.Windows.Forms.CheckBox();
            this.txtMaxY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMaxX = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtMinY = new System.Windows.Forms.TextBox();
            this.txtMinX = new System.Windows.Forms.TextBox();
            this.grbLatLon = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grbMethods.SuspendLayout();
            this.grbLatLon.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(220, 364);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 23);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnOpen2
            // 
            this.btnOpen2.Location = new System.Drawing.Point(239, 86);
            this.btnOpen2.Name = "btnOpen2";
            this.btnOpen2.Size = new System.Drawing.Size(33, 23);
            this.btnOpen2.TabIndex = 23;
            this.btnOpen2.Text = "...";
            this.btnOpen2.UseVisualStyleBackColor = true;
            this.btnOpen2.Click += new System.EventHandler(this.btnOpen2_Click);
            // 
            // txtForVal1
            // 
            this.txtForVal1.Location = new System.Drawing.Point(101, 89);
            this.txtForVal1.Name = "txtForVal1";
            this.txtForVal1.Size = new System.Drawing.Size(132, 21);
            this.txtForVal1.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "MODIS LST数据";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtToVal
            // 
            this.txtToVal.Location = new System.Drawing.Point(101, 29);
            this.txtToVal.Name = "txtToVal";
            this.txtToVal.Size = new System.Drawing.Size(132, 21);
            this.txtToVal.TabIndex = 19;
            this.txtToVal.TextChanged += new System.EventHandler(this.txtToVal_TextChanged);
            // 
            // btnOpen1
            // 
            this.btnOpen1.Location = new System.Drawing.Point(239, 29);
            this.btnOpen1.Name = "btnOpen1";
            this.btnOpen1.Size = new System.Drawing.Size(33, 23);
            this.btnOpen1.TabIndex = 20;
            this.btnOpen1.Text = "...";
            this.btnOpen1.UseVisualStyleBackColor = true;
            this.btnOpen1.Click += new System.EventHandler(this.btnOpen1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "待验证LST数据";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtforinvalid);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtinvalid);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(0, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 140);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择数据";
            // 
            // txtforinvalid
            // 
            this.txtforinvalid.Location = new System.Drawing.Point(101, 107);
            this.txtforinvalid.Name = "txtforinvalid";
            this.txtforinvalid.Size = new System.Drawing.Size(98, 21);
            this.txtforinvalid.TabIndex = 32;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 31;
            this.label4.Text = "无效值( ,分隔)";
            // 
            // txtinvalid
            // 
            this.txtinvalid.Location = new System.Drawing.Point(101, 51);
            this.txtinvalid.Name = "txtinvalid";
            this.txtinvalid.Size = new System.Drawing.Size(98, 21);
            this.txtinvalid.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 29;
            this.label3.Text = "无效值( ,分隔)";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblmsg);
            this.groupBox2.Controls.Add(this.txtColumn);
            this.groupBox2.Controls.Add(this.showcolumn);
            this.groupBox2.Controls.Add(this.chHistogram);
            this.groupBox2.Controls.Add(this.chRmse);
            this.groupBox2.Controls.Add(this.chScatter);
            this.groupBox2.Location = new System.Drawing.Point(7, 261);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(269, 97);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "评估方式";
            // 
            // lblmsg
            // 
            this.lblmsg.AutoSize = true;
            this.lblmsg.Enabled = false;
            this.lblmsg.Location = new System.Drawing.Point(84, 21);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(149, 12);
            this.lblmsg.TabIndex = 31;
            this.lblmsg.Text = "正在生成散点图,请等待...";
            // 
            // txtColumn
            // 
            this.txtColumn.Enabled = false;
            this.txtColumn.Location = new System.Drawing.Point(175, 44);
            this.txtColumn.Name = "txtColumn";
            this.txtColumn.Size = new System.Drawing.Size(55, 21);
            this.txtColumn.TabIndex = 31;
            this.txtColumn.Text = "10";
            // 
            // showcolumn
            // 
            this.showcolumn.AutoSize = true;
            this.showcolumn.BackColor = System.Drawing.Color.Silver;
            this.showcolumn.Enabled = false;
            this.showcolumn.Location = new System.Drawing.Point(116, 48);
            this.showcolumn.Name = "showcolumn";
            this.showcolumn.Size = new System.Drawing.Size(59, 12);
            this.showcolumn.TabIndex = 4;
            this.showcolumn.Text = "最大组数:";
            // 
            // chHistogram
            // 
            this.chHistogram.AutoSize = true;
            this.chHistogram.Location = new System.Drawing.Point(26, 47);
            this.chHistogram.Name = "chHistogram";
            this.chHistogram.Size = new System.Drawing.Size(84, 16);
            this.chHistogram.TabIndex = 3;
            this.chHistogram.Text = "偏差直方图";
            this.chHistogram.UseVisualStyleBackColor = true;
            this.chHistogram.CheckedChanged += new System.EventHandler(this.chHistogram_CheckedChanged);
            // 
            // chRmse
            // 
            this.chRmse.AutoSize = true;
            this.chRmse.Location = new System.Drawing.Point(26, 75);
            this.chRmse.Name = "chRmse";
            this.chRmse.Size = new System.Drawing.Size(120, 16);
            this.chRmse.TabIndex = 2;
            this.chRmse.Text = "均方根误差(RMSE)";
            this.chRmse.UseVisualStyleBackColor = true;
            // 
            // chScatter
            // 
            this.chScatter.AutoSize = true;
            this.chScatter.Location = new System.Drawing.Point(26, 20);
            this.chScatter.Name = "chScatter";
            this.chScatter.Size = new System.Drawing.Size(60, 16);
            this.chScatter.TabIndex = 0;
            this.chScatter.Text = "散点图";
            this.chScatter.UseVisualStyleBackColor = true;
            this.chScatter.CheckedChanged += new System.EventHandler(this.chScatter_CheckedChanged);
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
            this.grbMethods.Size = new System.Drawing.Size(267, 70);
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
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(115, 19);
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.Size = new System.Drawing.Size(54, 21);
            this.txtMaxY.TabIndex = 10;
            this.txtMaxY.Text = "56";
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "MinX";
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
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(189, 45);
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.Size = new System.Drawing.Size(54, 21);
            this.txtMaxX.TabIndex = 14;
            this.txtMaxX.Text = "142";
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
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(116, 69);
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.Size = new System.Drawing.Size(54, 21);
            this.txtMinY.TabIndex = 15;
            this.txtMinY.Text = "10";
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(42, 45);
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.Size = new System.Drawing.Size(54, 21);
            this.txtMinX.TabIndex = 12;
            this.txtMinX.Text = "70";
            // 
            // grbLatLon
            // 
            this.grbLatLon.Controls.Add(this.txtMinX);
            this.grbLatLon.Controls.Add(this.txtMinY);
            this.grbLatLon.Controls.Add(this.label9);
            this.grbLatLon.Controls.Add(this.txtMaxX);
            this.grbLatLon.Controls.Add(this.label8);
            this.grbLatLon.Controls.Add(this.label7);
            this.grbLatLon.Controls.Add(this.label6);
            this.grbLatLon.Controls.Add(this.txtMaxY);
            this.grbLatLon.Controls.Add(this.grbMethods);
            this.grbLatLon.Location = new System.Drawing.Point(3, 153);
            this.grbLatLon.Name = "grbLatLon";
            this.grbLatLon.Size = new System.Drawing.Size(273, 102);
            this.grbLatLon.TabIndex = 27;
            this.grbLatLon.TabStop = false;
            this.grbLatLon.Text = "待验证数据经纬度";
            // 
            // UCLSTSatelliteArgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grbLatLon);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnOpen2);
            this.Controls.Add(this.txtForVal1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtToVal);
            this.Controls.Add(this.btnOpen1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "UCLSTSatelliteArgs";
            this.Size = new System.Drawing.Size(287, 393);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grbMethods.ResumeLayout(false);
            this.grbMethods.PerformLayout();
            this.grbLatLon.ResumeLayout(false);
            this.grbLatLon.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnOpen2;
        private System.Windows.Forms.TextBox txtForVal1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtToVal;
        private System.Windows.Forms.Button btnOpen1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chHistogram;
        private System.Windows.Forms.CheckBox chRmse;
        private System.Windows.Forms.CheckBox chScatter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtinvalid;
        private System.Windows.Forms.GroupBox grbMethods;
        private System.Windows.Forms.CheckBox chbHistogram;
        private System.Windows.Forms.CheckBox chbRmse;
        private System.Windows.Forms.CheckBox chbTimeSeq;
        private System.Windows.Forms.CheckBox chbScatter;
        private System.Windows.Forms.TextBox txtMaxY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMaxX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtMinY;
        private System.Windows.Forms.TextBox txtMinX;
        private System.Windows.Forms.GroupBox grbLatLon;
        private System.Windows.Forms.TextBox txtColumn;
        private System.Windows.Forms.Label showcolumn;
        private System.Windows.Forms.Label lblmsg;
        private System.Windows.Forms.TextBox txtforinvalid;
        private System.Windows.Forms.Label label4;
    }
}
