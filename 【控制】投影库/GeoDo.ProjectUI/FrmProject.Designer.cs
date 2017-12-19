namespace GeoDo.ProjectUI
{
	partial class FrmProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProject));
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.是否拼接 = new System.Windows.Forms.CheckBox();
            this.gBoxInput = new System.Windows.Forms.GroupBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.lbSelctFiles = new System.Windows.Forms.ListBox();
            this.gBoxOutput = new System.Windows.Forms.GroupBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.btnOutFile = new System.Windows.Forms.Button();
            this.panelButton = new System.Windows.Forms.Panel();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnDo = new System.Windows.Forms.Button();
            this.gBoxParas = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ucSelectBands1 = new Geodo.ProjectUI.UCSelectBands();
            this.ucSetOutRange21 = new GeoDo.ProjectUI.UCtools.UCSetOutRange2();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.gBoxInput.SuspendLayout();
            this.gBoxOutput.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.gBoxParas.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gBoxParas);
            this.panel1.Controls.Add(this.panelButton);
            this.panel1.Controls.Add(this.gBoxOutput);
            this.panel1.Controls.Add(this.gBoxInput);
            this.panel1.Controls.Add(this.是否拼接);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 690);
            this.panel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(484, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(68, 36);
            this.toolStripButton1.Text = "添加";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(68, 36);
            this.toolStripButton2.Text = "删除";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(68, 36);
            this.toolStripButton3.Text = "清空";
            // 
            // 是否拼接
            // 
            this.是否拼接.AutoSize = true;
            this.是否拼接.Location = new System.Drawing.Point(354, 12);
            this.是否拼接.Name = "是否拼接";
            this.是否拼接.Size = new System.Drawing.Size(72, 16);
            this.是否拼接.TabIndex = 1;
            this.是否拼接.Text = "是否拼接";
            this.是否拼接.UseVisualStyleBackColor = true;
            // 
            // gBoxInput
            // 
            this.gBoxInput.Controls.Add(this.btnDown);
            this.gBoxInput.Controls.Add(this.btnUp);
            this.gBoxInput.Controls.Add(this.lbSelctFiles);
            this.gBoxInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxInput.Location = new System.Drawing.Point(0, 39);
            this.gBoxInput.Name = "gBoxInput";
            this.gBoxInput.Size = new System.Drawing.Size(484, 126);
            this.gBoxInput.TabIndex = 2;
            this.gBoxInput.TabStop = false;
            // 
            // btnDown
            // 
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Location = new System.Drawing.Point(431, 69);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(47, 45);
            this.btnDown.TabIndex = 19;
            this.btnDown.Text = "下移";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Location = new System.Drawing.Point(431, 14);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 45);
            this.btnUp.TabIndex = 18;
            this.btnUp.Text = "上移";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // lbSelctFiles
            // 
            this.lbSelctFiles.FormattingEnabled = true;
            this.lbSelctFiles.HorizontalScrollbar = true;
            this.lbSelctFiles.ItemHeight = 12;
            this.lbSelctFiles.Location = new System.Drawing.Point(12, 14);
            this.lbSelctFiles.Name = "lbSelctFiles";
            this.lbSelctFiles.Size = new System.Drawing.Size(414, 100);
            this.lbSelctFiles.TabIndex = 17;
            // 
            // gBoxOutput
            // 
            this.gBoxOutput.Controls.Add(this.txtOutFile);
            this.gBoxOutput.Controls.Add(this.btnOutFile);
            this.gBoxOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxOutput.Location = new System.Drawing.Point(0, 165);
            this.gBoxOutput.Name = "gBoxOutput";
            this.gBoxOutput.Size = new System.Drawing.Size(484, 54);
            this.gBoxOutput.TabIndex = 3;
            this.gBoxOutput.TabStop = false;
            this.gBoxOutput.Text = "输出位置";
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(12, 21);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(414, 21);
            this.txtOutFile.TabIndex = 7;
            // 
            // btnOutFile
            // 
            this.btnOutFile.FlatAppearance.BorderSize = 0;
            this.btnOutFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOutFile.Location = new System.Drawing.Point(434, 12);
            this.btnOutFile.Name = "btnOutFile";
            this.btnOutFile.Size = new System.Drawing.Size(40, 34);
            this.btnOutFile.TabIndex = 8;
            this.btnOutFile.Text = "打开";
            this.btnOutFile.UseVisualStyleBackColor = true;
            // 
            // panelButton
            // 
            this.panelButton.Controls.Add(this.btnQuit);
            this.panelButton.Controls.Add(this.btnDo);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButton.Location = new System.Drawing.Point(0, 219);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(484, 40);
            this.panelButton.TabIndex = 4;
            // 
            // btnQuit
            // 
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Location = new System.Drawing.Point(403, -3);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 40);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "退出";
            this.btnQuit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnQuit.UseVisualStyleBackColor = true;
            // 
            // btnDo
            // 
            this.btnDo.FlatAppearance.BorderSize = 0;
            this.btnDo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDo.Location = new System.Drawing.Point(294, 0);
            this.btnDo.Name = "btnDo";
            this.btnDo.Size = new System.Drawing.Size(93, 40);
            this.btnDo.TabIndex = 2;
            this.btnDo.Text = "开始执行";
            this.btnDo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDo.UseVisualStyleBackColor = true;
            // 
            // gBoxParas
            // 
            this.gBoxParas.Controls.Add(this.groupBox1);
            this.gBoxParas.Controls.Add(this.panel2);
            this.gBoxParas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBoxParas.Location = new System.Drawing.Point(0, 259);
            this.gBoxParas.Name = "gBoxParas";
            this.gBoxParas.Size = new System.Drawing.Size(484, 431);
            this.gBoxParas.TabIndex = 5;
            this.gBoxParas.TabStop = false;
            this.gBoxParas.Text = "参数设置";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucSelectBands1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(305, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 411);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "波段选择";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ucSetOutRange21);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 59);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(302, 352);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输出范围";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(3, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(302, 411);
            this.panel2.TabIndex = 0;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(27, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(155, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(203, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "更多";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 59);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "输出类型";
            // 
            // ucSelectBands1
            // 
            this.ucSelectBands1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSelectBands1.IsShowButton = true;
            this.ucSelectBands1.Location = new System.Drawing.Point(3, 17);
            this.ucSelectBands1.Name = "ucSelectBands1";
            this.ucSelectBands1.Provider = null;
            this.ucSelectBands1.Size = new System.Drawing.Size(170, 391);
            this.ucSelectBands1.TabIndex = 0;
            // 
            // ucSetOutRange21
            // 
            this.ucSetOutRange21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSetOutRange21.Location = new System.Drawing.Point(3, 17);
            this.ucSetOutRange21.Name = "ucSetOutRange21";
            this.ucSetOutRange21.Size = new System.Drawing.Size(296, 332);
            this.ucSetOutRange21.TabIndex = 0;
            // 
            // FrmProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 690);
            this.Controls.Add(this.panel1);
            this.Name = "FrmProject";
            this.Text = "FrmProject";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.gBoxInput.ResumeLayout(false);
            this.gBoxOutput.ResumeLayout(false);
            this.gBoxOutput.PerformLayout();
            this.panelButton.ResumeLayout(false);
            this.gBoxParas.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox 是否拼接;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.GroupBox gBoxInput;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.ListBox lbSelctFiles;
        private System.Windows.Forms.GroupBox gBoxOutput;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Button btnOutFile;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnDo;
        private System.Windows.Forms.GroupBox gBoxParas;
        private System.Windows.Forms.GroupBox groupBox1;
        private Geodo.ProjectUI.UCSelectBands ucSelectBands1;
        private System.Windows.Forms.GroupBox groupBox3;
        private UCtools.UCSetOutRange2 ucSetOutRange21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
	}
}