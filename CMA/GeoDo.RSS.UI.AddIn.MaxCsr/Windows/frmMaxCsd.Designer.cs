namespace GeoDo.RSS.UI.AddIn.MaxCsr
{
    partial class frmMaxCsd
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
            this.labFName = new System.Windows.Forms.Label();
            this.txtFileDir = new System.Windows.Forms.TextBox();
            this.btnOpenDir = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.txtCalcMothod = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDataSetBands = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rbNDVI = new System.Windows.Forms.RadioButton();
            this.rbSingleBand = new System.Windows.Forms.RadioButton();
            this.rbOther = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBands = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbangle = new System.Windows.Forms.CheckBox();
            this.txtextpars = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labFName
            // 
            this.labFName.AutoSize = true;
            this.labFName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFName.Location = new System.Drawing.Point(12, 11);
            this.labFName.Name = "labFName";
            this.labFName.Size = new System.Drawing.Size(92, 17);
            this.labFName.TabIndex = 0;
            this.labFName.Text = "待计算文件路径";
            // 
            // txtFileDir
            // 
            this.txtFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileDir.Location = new System.Drawing.Point(115, 10);
            this.txtFileDir.Name = "txtFileDir";
            this.txtFileDir.Size = new System.Drawing.Size(309, 21);
            this.txtFileDir.TabIndex = 1;
            // 
            // btnOpenDir
            // 
            this.btnOpenDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDir.Image = global::GeoDo.RSS.UI.AddIn.MaxCsr.Properties.Resources.folder_open16;
            this.btnOpenDir.Location = new System.Drawing.Point(425, 7);
            this.btnOpenDir.Name = "btnOpenDir";
            this.btnOpenDir.Size = new System.Drawing.Size(25, 25);
            this.btnOpenDir.TabIndex = 2;
            this.btnOpenDir.UseVisualStyleBackColor = true;
            this.btnOpenDir.Click += new System.EventHandler(this.btnOpenDir_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(288, 264);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "计算";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(374, 264);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "输出文件路径";
            // 
            // txtOutDir
            // 
            this.txtOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutDir.Location = new System.Drawing.Point(115, 67);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(309, 21);
            this.txtOutDir.TabIndex = 1;
            // 
            // btnOutDir
            // 
            this.btnOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutDir.Image = global::GeoDo.RSS.UI.AddIn.MaxCsr.Properties.Resources.folder_open16;
            this.btnOutDir.Location = new System.Drawing.Point(425, 64);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(25, 25);
            this.btnOutDir.TabIndex = 2;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // txtCalcMothod
            // 
            this.txtCalcMothod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCalcMothod.Location = new System.Drawing.Point(103, 46);
            this.txtCalcMothod.Name = "txtCalcMothod";
            this.txtCalcMothod.Size = new System.Drawing.Size(338, 21);
            this.txtCalcMothod.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "提取数据集波段";
            // 
            // txtDataSetBands
            // 
            this.txtDataSetBands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataSetBands.Location = new System.Drawing.Point(115, 95);
            this.txtDataSetBands.Name = "txtDataSetBands";
            this.txtDataSetBands.Size = new System.Drawing.Size(333, 21);
            this.txtDataSetBands.TabIndex = 1;
            this.txtDataSetBands.Text = "ALL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(112, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(271, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "eg：波段用【，】； 间隔  全波段用【ALL】标识";
            // 
            // rbNDVI
            // 
            this.rbNDVI.AutoSize = true;
            this.rbNDVI.Location = new System.Drawing.Point(58, 23);
            this.rbNDVI.Name = "rbNDVI";
            this.rbNDVI.Size = new System.Drawing.Size(71, 16);
            this.rbNDVI.TabIndex = 5;
            this.rbNDVI.Text = "NDVI最大";
            this.rbNDVI.UseVisualStyleBackColor = true;
            this.rbNDVI.CheckedChanged += new System.EventHandler(this.rbNDVI_CheckedChanged);
            // 
            // rbSingleBand
            // 
            this.rbSingleBand.AutoSize = true;
            this.rbSingleBand.Location = new System.Drawing.Point(172, 23);
            this.rbSingleBand.Name = "rbSingleBand";
            this.rbSingleBand.Size = new System.Drawing.Size(83, 16);
            this.rbSingleBand.TabIndex = 5;
            this.rbSingleBand.Text = "单波段最大";
            this.rbSingleBand.UseVisualStyleBackColor = true;
            this.rbSingleBand.CheckedChanged += new System.EventHandler(this.rbSingleBand_CheckedChanged);
            // 
            // rbOther
            // 
            this.rbOther.AutoSize = true;
            this.rbOther.Checked = true;
            this.rbOther.Location = new System.Drawing.Point(301, 23);
            this.rbOther.Name = "rbOther";
            this.rbOther.Size = new System.Drawing.Size(47, 16);
            this.rbOther.TabIndex = 5;
            this.rbOther.TabStop = true;
            this.rbOther.Text = "其他";
            this.rbOther.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rbOther);
            this.groupBox1.Controls.Add(this.txtBands);
            this.groupBox1.Controls.Add(this.txtCalcMothod);
            this.groupBox1.Controls.Add(this.rbSingleBand);
            this.groupBox1.Controls.Add(this.rbNDVI);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 150);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 109);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "运算逻辑";
            // 
            // txtBands
            // 
            this.txtBands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBands.Location = new System.Drawing.Point(103, 82);
            this.txtBands.Name = "txtBands";
            this.txtBands.Size = new System.Drawing.Size(338, 21);
            this.txtBands.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(5, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "表达式对应波段";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(5, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "逻辑表达式描述";
            // 
            // ckbangle
            // 
            this.ckbangle.AutoSize = true;
            this.ckbangle.Location = new System.Drawing.Point(112, 268);
            this.ckbangle.Name = "ckbangle";
            this.ckbangle.Size = new System.Drawing.Size(72, 16);
            this.ckbangle.TabIndex = 8;
            this.ckbangle.Text = "角度信息";
            this.ckbangle.UseVisualStyleBackColor = true;
            // 
            // txtextpars
            // 
            this.txtextpars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtextpars.Location = new System.Drawing.Point(115, 40);
            this.txtextpars.Name = "txtextpars";
            this.txtextpars.Size = new System.Drawing.Size(166, 21);
            this.txtextpars.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(12, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 17);
            this.label6.TabIndex = 9;
            this.label6.Text = "匹配格式字符串";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(285, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "例:*.ldf|*.dat";
            // 
            // frmMaxCsd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 294);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtextpars);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ckbangle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.btnOpenDir);
            this.Controls.Add(this.txtDataSetBands);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFileDir);
            this.Controls.Add(this.labFName);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmMaxCsd";
            this.ShowIcon = false;
            this.Text = "提取晴空数据集...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labFName;
        private System.Windows.Forms.TextBox txtFileDir;
        private System.Windows.Forms.Button btnOpenDir;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.TextBox txtCalcMothod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDataSetBands;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbNDVI;
        private System.Windows.Forms.RadioButton rbSingleBand;
        private System.Windows.Forms.RadioButton rbOther;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBands;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckbangle;
        private System.Windows.Forms.TextBox txtextpars;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}