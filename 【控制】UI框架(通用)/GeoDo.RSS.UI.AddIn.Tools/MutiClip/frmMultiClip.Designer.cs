namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class frmMultiClip
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
            this.btnmuticlip = new System.Windows.Forms.Button();
            this.btnclose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtinput = new System.Windows.Forms.TextBox();
            this.btninputselect = new System.Windows.Forms.Button();
            this.txtextend = new System.Windows.Forms.TextBox();
            this.btnoutputselect = new System.Windows.Forms.Button();
            this.txtoutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnedit = new System.Windows.Forms.Button();
            this.btndelcoord = new System.Windows.Forms.Button();
            this.btnAddcoord = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtoutputname = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtmaxy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtminy = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtmaxx = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtminx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbcoord = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnmuticlip
            // 
            this.btnmuticlip.Location = new System.Drawing.Point(508, 396);
            this.btnmuticlip.Name = "btnmuticlip";
            this.btnmuticlip.Size = new System.Drawing.Size(75, 23);
            this.btnmuticlip.TabIndex = 0;
            this.btnmuticlip.Text = "批量裁切";
            this.btnmuticlip.UseVisualStyleBackColor = true;
            this.btnmuticlip.Click += new System.EventHandler(this.btnmuticlip_Click);
            // 
            // btnclose
            // 
            this.btnclose.Location = new System.Drawing.Point(596, 396);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(75, 23);
            this.btnclose.TabIndex = 1;
            this.btnclose.Text = "关闭";
            this.btnclose.UseVisualStyleBackColor = true;
            this.btnclose.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "输入文件：";
            // 
            // txtinput
            // 
            this.txtinput.Location = new System.Drawing.Point(73, 28);
            this.txtinput.Name = "txtinput";
            this.txtinput.Size = new System.Drawing.Size(380, 21);
            this.txtinput.TabIndex = 3;
            // 
            // btninputselect
            // 
            this.btninputselect.Location = new System.Drawing.Point(459, 27);
            this.btninputselect.Name = "btninputselect";
            this.btninputselect.Size = new System.Drawing.Size(53, 23);
            this.btninputselect.TabIndex = 4;
            this.btninputselect.Text = "选择";
            this.btninputselect.UseVisualStyleBackColor = true;
            this.btninputselect.Click += new System.EventHandler(this.btninputselect_Click);
            // 
            // txtextend
            // 
            this.txtextend.Location = new System.Drawing.Point(518, 28);
            this.txtextend.Name = "txtextend";
            this.txtextend.Size = new System.Drawing.Size(79, 21);
            this.txtextend.TabIndex = 5;
            this.txtextend.Text = "*.*";
            // 
            // btnoutputselect
            // 
            this.btnoutputselect.Location = new System.Drawing.Point(459, 66);
            this.btnoutputselect.Name = "btnoutputselect";
            this.btnoutputselect.Size = new System.Drawing.Size(53, 23);
            this.btnoutputselect.TabIndex = 8;
            this.btnoutputselect.Text = "选择";
            this.btnoutputselect.UseVisualStyleBackColor = true;
            this.btnoutputselect.Click += new System.EventHandler(this.btnoutputselect_Click);
            // 
            // txtoutput
            // 
            this.txtoutput.Location = new System.Drawing.Point(73, 67);
            this.txtoutput.Name = "txtoutput";
            this.txtoutput.Size = new System.Drawing.Size(380, 21);
            this.txtoutput.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "输出位置：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnedit);
            this.panel1.Controls.Add(this.btndelcoord);
            this.panel1.Controls.Add(this.btnAddcoord);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lbcoord);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(666, 239);
            this.panel1.TabIndex = 17;
            // 
            // btnedit
            // 
            this.btnedit.Location = new System.Drawing.Point(320, 93);
            this.btnedit.Name = "btnedit";
            this.btnedit.Size = new System.Drawing.Size(74, 23);
            this.btnedit.TabIndex = 30;
            this.btnedit.Text = "编 辑";
            this.btnedit.UseVisualStyleBackColor = true;
            this.btnedit.Click += new System.EventHandler(this.btnedit_Click);
            // 
            // btndelcoord
            // 
            this.btndelcoord.Location = new System.Drawing.Point(320, 136);
            this.btndelcoord.Name = "btndelcoord";
            this.btndelcoord.Size = new System.Drawing.Size(74, 23);
            this.btndelcoord.TabIndex = 28;
            this.btndelcoord.Text = "删 除";
            this.btndelcoord.UseVisualStyleBackColor = true;
            this.btndelcoord.Click += new System.EventHandler(this.btndelcoord_Click);
            // 
            // btnAddcoord
            // 
            this.btnAddcoord.Location = new System.Drawing.Point(320, 47);
            this.btnAddcoord.Name = "btnAddcoord";
            this.btnAddcoord.Size = new System.Drawing.Size(74, 23);
            this.btnAddcoord.TabIndex = 29;
            this.btnAddcoord.Text = "添 加";
            this.btnAddcoord.UseVisualStyleBackColor = true;
            this.btnAddcoord.Click += new System.EventHandler(this.btnAddcoord_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtoutputname);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.txtmaxy);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtminy);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.txtmaxx);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtminx);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(423, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(243, 239);
            this.panel2.TabIndex = 26;
            // 
            // txtoutputname
            // 
            this.txtoutputname.Location = new System.Drawing.Point(79, 21);
            this.txtoutputname.Name = "txtoutputname";
            this.txtoutputname.Size = new System.Drawing.Size(137, 21);
            this.txtoutputname.TabIndex = 30;
            this.txtoutputname.Text = "DXX";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 29;
            this.label8.Text = "输出名称：";
            // 
            // txtmaxy
            // 
            this.txtmaxy.Location = new System.Drawing.Point(79, 165);
            this.txtmaxy.Name = "txtmaxy";
            this.txtmaxy.Size = new System.Drawing.Size(137, 21);
            this.txtmaxy.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 31;
            this.label6.Text = "MaxY：";
            // 
            // txtminy
            // 
            this.txtminy.Location = new System.Drawing.Point(79, 129);
            this.txtminy.Name = "txtminy";
            this.txtminy.Size = new System.Drawing.Size(137, 21);
            this.txtminy.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "MinY：";
            // 
            // txtmaxx
            // 
            this.txtmaxx.Location = new System.Drawing.Point(79, 93);
            this.txtmaxx.Name = "txtmaxx";
            this.txtmaxx.Size = new System.Drawing.Size(137, 21);
            this.txtmaxx.TabIndex = 28;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 27;
            this.label4.Text = "MaxX：";
            // 
            // txtminx
            // 
            this.txtminx.Location = new System.Drawing.Point(79, 57);
            this.txtminx.Name = "txtminx";
            this.txtminx.Size = new System.Drawing.Size(137, 21);
            this.txtminx.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "MinX：";
            // 
            // lbcoord
            // 
            this.lbcoord.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbcoord.FormattingEnabled = true;
            this.lbcoord.ItemHeight = 12;
            this.lbcoord.Location = new System.Drawing.Point(0, 0);
            this.lbcoord.Name = "lbcoord";
            this.lbcoord.Size = new System.Drawing.Size(295, 239);
            this.lbcoord.TabIndex = 25;
            this.lbcoord.SelectedIndexChanged += new System.EventHandler(this.lbcoord_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnoutputselect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtinput);
            this.groupBox1.Controls.Add(this.btninputselect);
            this.groupBox1.Controls.Add(this.txtoutput);
            this.groupBox1.Controls.Add(this.txtextend);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(659, 117);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "路径设置";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(2, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(672, 259);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "坐标范围";
            // 
            // frmMultiClip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 425);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnclose);
            this.Controls.Add(this.btnmuticlip);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMultiClip";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "批量裁切工具";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnmuticlip;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtinput;
        private System.Windows.Forms.Button btninputselect;
        private System.Windows.Forms.TextBox txtextend;
        private System.Windows.Forms.Button btnoutputselect;
        private System.Windows.Forms.TextBox txtoutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btndelcoord;
        private System.Windows.Forms.Button btnAddcoord;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtmaxy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtminy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtmaxx;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtminx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbcoord;
        private System.Windows.Forms.TextBox txtoutputname;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnedit;
    }
}

