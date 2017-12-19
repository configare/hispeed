namespace GeoDo.ProjectUI
{
    partial class UCSetOutRange
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtResolutionY = new System.Windows.Forms.TextBox();
            this.txtResolutionX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCenterY = new System.Windows.Forms.TextBox();
            this.txtCenterX = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numImgHeight = new System.Windows.Forms.NumericUpDown();
            this.numImgWidth = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtMaxY = new System.Windows.Forms.TextBox();
            this.txtMinY = new System.Windows.Forms.TextBox();
            this.txtMaxX = new System.Windows.Forms.TextBox();
            this.txtMinX = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tsbWhole = new System.Windows.Forms.ToolStripButton();
            this.tsbCenter = new System.Windows.Forms.ToolStripButton();
            this.tsbCorners = new System.Windows.Forms.ToolStripButton();
            this.tsbDefined = new System.Windows.Forms.ToolStripButton();
            this.tsOutRange = new System.Windows.Forms.ToolStrip();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numImgHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImgWidth)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tsOutRange.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtResolutionY);
            this.groupBox1.Controls.Add(this.txtResolutionX);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "中心分辨率";
            // 
            // txtResolutionY
            // 
            this.txtResolutionY.Location = new System.Drawing.Point(82, 62);
            this.txtResolutionY.Name = "txtResolutionY";
            this.txtResolutionY.Size = new System.Drawing.Size(89, 21);
            this.txtResolutionY.TabIndex = 11;
            this.txtResolutionY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtResolutionY_KeyUp);
            // 
            // txtResolutionX
            // 
            this.txtResolutionX.Location = new System.Drawing.Point(82, 29);
            this.txtResolutionX.Name = "txtResolutionX";
            this.txtResolutionX.Size = new System.Drawing.Size(89, 21);
            this.txtResolutionX.TabIndex = 10;
            this.txtResolutionX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtResolutionX_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "度/像素";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(180, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "度/像素";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "南北";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "东西";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtCenterY);
            this.groupBox2.Controls.Add(this.txtCenterX);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "中心经纬度";
            this.groupBox2.Visible = false;
            // 
            // txtCenterY
            // 
            this.txtCenterY.Location = new System.Drawing.Point(82, 60);
            this.txtCenterY.Name = "txtCenterY";
            this.txtCenterY.Size = new System.Drawing.Size(89, 21);
            this.txtCenterY.TabIndex = 9;
            this.txtCenterY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCenterY_KeyUp);
            // 
            // txtCenterX
            // 
            this.txtCenterX.Location = new System.Drawing.Point(82, 24);
            this.txtCenterX.Name = "txtCenterX";
            this.txtCenterX.Size = new System.Drawing.Size(89, 21);
            this.txtCenterX.TabIndex = 8;
            this.txtCenterX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCenterX_KeyUp);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 63);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 12);
            this.label17.TabIndex = 7;
            this.label17.Text = "中心纬度";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 27);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 12);
            this.label18.TabIndex = 6;
            this.label18.Text = "中心经度";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numImgHeight);
            this.groupBox3.Controls.Add(this.numImgWidth);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 225);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 100);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "图像尺寸(点数)";
            this.groupBox3.Visible = false;
            // 
            // numImgHeight
            // 
            this.numImgHeight.Location = new System.Drawing.Point(82, 63);
            this.numImgHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numImgHeight.Name = "numImgHeight";
            this.numImgHeight.Size = new System.Drawing.Size(89, 21);
            this.numImgHeight.TabIndex = 9;
            this.numImgHeight.ValueChanged += new System.EventHandler(this.numImgHeight_ValueChanged);
            // 
            // numImgWidth
            // 
            this.numImgWidth.Location = new System.Drawing.Point(82, 27);
            this.numImgWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numImgWidth.Name = "numImgWidth";
            this.numImgWidth.Size = new System.Drawing.Size(89, 21);
            this.numImgWidth.TabIndex = 8;
            this.numImgWidth.ValueChanged += new System.EventHandler(this.numImgWidth_ValueChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(180, 65);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(29, 12);
            this.label22.TabIndex = 7;
            this.label22.Text = "像素";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(180, 29);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(29, 12);
            this.label21.TabIndex = 6;
            this.label21.Text = "像素";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(35, 65);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(29, 12);
            this.label20.TabIndex = 2;
            this.label20.Text = "高度";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(35, 29);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 12);
            this.label19.TabIndex = 1;
            this.label19.Text = "宽度";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtMaxY);
            this.groupBox5.Controls.Add(this.txtMinY);
            this.groupBox5.Controls.Add(this.txtMaxX);
            this.groupBox5.Controls.Add(this.txtMinX);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(0, 325);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(250, 200);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "四角经纬度";
            this.groupBox5.Visible = false;
            // 
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(82, 146);
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.Size = new System.Drawing.Size(89, 21);
            this.txtMaxY.TabIndex = 9;
            this.txtMaxY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMaxY_KeyUp);
            // 
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(82, 115);
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.Size = new System.Drawing.Size(89, 21);
            this.txtMinY.TabIndex = 8;
            this.txtMinY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMinY_KeyUp);
            // 
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(82, 73);
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.Size = new System.Drawing.Size(89, 21);
            this.txtMaxX.TabIndex = 7;
            this.txtMaxX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMaxX_KeyUp);
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(82, 38);
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.Size = new System.Drawing.Size(89, 21);
            this.txtMinX.TabIndex = 6;
            this.txtMinX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMinX_KeyUp);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 149);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 12);
            this.label16.TabIndex = 4;
            this.label16.Text = "底端纬度";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 118);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 3;
            this.label15.Text = "顶端纬度";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 76);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 12);
            this.label14.TabIndex = 2;
            this.label14.Text = "右边经度";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 1;
            this.label13.Text = "左边经度";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.button1);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox7.Location = new System.Drawing.Point(0, 525);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(250, 200);
            this.groupBox7.TabIndex = 7;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "分幅定义";
            this.groupBox7.Visible = false;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(152, 177);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "自由分幅编辑";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tsbWhole
            // 
            this.tsbWhole.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbWhole.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWhole.Name = "tsbWhole";
            this.tsbWhole.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.tsbWhole.Size = new System.Drawing.Size(24, 22);
            this.tsbWhole.Text = "整轨投影";
            this.tsbWhole.Click += new System.EventHandler(this.tsbWhole_Click);
            // 
            // tsbCenter
            // 
            this.tsbCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCenter.Name = "tsbCenter";
            this.tsbCenter.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.tsbCenter.Size = new System.Drawing.Size(24, 22);
            this.tsbCenter.Text = "按中心点";
            this.tsbCenter.Click += new System.EventHandler(this.tsbCenter_Click);
            // 
            // tsbCorners
            // 
            this.tsbCorners.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCorners.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCorners.Name = "tsbCorners";
            this.tsbCorners.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.tsbCorners.Size = new System.Drawing.Size(24, 22);
            this.tsbCorners.Text = "四角经纬度";
            this.tsbCorners.Click += new System.EventHandler(this.tsbCorners_Click);
            // 
            // tsbDefined
            // 
            this.tsbDefined.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDefined.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDefined.Name = "tsbDefined";
            this.tsbDefined.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.tsbDefined.Size = new System.Drawing.Size(24, 22);
            this.tsbDefined.Text = "已定义分幅";
            this.tsbDefined.Click += new System.EventHandler(this.tsbDefined_Click);
            // 
            // tsOutRange
            // 
            this.tsOutRange.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsOutRange.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsOutRange.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbWhole,
            this.tsbCenter,
            this.tsbCorners,
            this.tsbDefined});
            this.tsOutRange.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsOutRange.Location = new System.Drawing.Point(0, 0);
            this.tsOutRange.Name = "tsOutRange";
            this.tsOutRange.Padding = new System.Windows.Forms.Padding(0);
            this.tsOutRange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tsOutRange.Size = new System.Drawing.Size(250, 25);
            this.tsOutRange.Stretch = true;
            this.tsOutRange.TabIndex = 0;
            // 
            // UCSetOutRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tsOutRange);
            this.Name = "UCSetOutRange";
            this.Size = new System.Drawing.Size(250, 704);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numImgHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImgWidth)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.tsOutRange.ResumeLayout(false);
            this.tsOutRange.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtResolutionY;
        private System.Windows.Forms.TextBox txtResolutionX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtCenterY;
        private System.Windows.Forms.TextBox txtCenterX;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numImgHeight;
        private System.Windows.Forms.NumericUpDown numImgWidth;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtMaxY;
        private System.Windows.Forms.TextBox txtMinY;
        private System.Windows.Forms.TextBox txtMaxX;
        private System.Windows.Forms.TextBox txtMinX;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripButton tsbWhole;
        private System.Windows.Forms.ToolStripButton tsbCenter;
        private System.Windows.Forms.ToolStripButton tsbCorners;
        private System.Windows.Forms.ToolStripButton tsbDefined;
        private System.Windows.Forms.ToolStrip tsOutRange;

    }
}
