namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class UCSelectBandForRgb
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
            this.rdGray = new System.Windows.Forms.RadioButton();
            this.rdRGB = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBlue = new System.Windows.Forms.ComboBox();
            this.txtGreen = new System.Windows.Forms.ComboBox();
            this.txtRed = new System.Windows.Forms.ComboBox();
            this.rdBlue = new System.Windows.Forms.RadioButton();
            this.rdGreen = new System.Windows.Forms.RadioButton();
            this.rdRed = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdGray
            // 
            this.rdGray.AutoSize = true;
            this.rdGray.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rdGray.Location = new System.Drawing.Point(3, 17);
            this.rdGray.Name = "rdGray";
            this.rdGray.Size = new System.Drawing.Size(124, 16);
            this.rdGray.TabIndex = 0;
            this.rdGray.TabStop = true;
            this.rdGray.Text = "灰度";
            this.rdGray.UseVisualStyleBackColor = true;
            // 
            // rdRGB
            // 
            this.rdRGB.AutoSize = true;
            this.rdRGB.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rdRGB.Location = new System.Drawing.Point(133, 17);
            this.rdRGB.Name = "rdRGB";
            this.rdRGB.Size = new System.Drawing.Size(124, 16);
            this.rdRGB.TabIndex = 1;
            this.rdRGB.TabStop = true;
            this.rdRGB.Text = "RGB合成";
            this.rdRGB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtBlue);
            this.groupBox2.Controls.Add(this.txtGreen);
            this.groupBox2.Controls.Add(this.txtRed);
            this.groupBox2.Controls.Add(this.rdBlue);
            this.groupBox2.Controls.Add(this.rdGreen);
            this.groupBox2.Controls.Add(this.rdRed);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(0, 36);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 135);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // txtBlue
            // 
            this.txtBlue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBlue.FormattingEnabled = true;
            this.txtBlue.Location = new System.Drawing.Point(94, 97);
            this.txtBlue.Name = "txtBlue";
            this.txtBlue.Size = new System.Drawing.Size(160, 25);
            this.txtBlue.TabIndex = 5;
            // 
            // txtGreen
            // 
            this.txtGreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGreen.FormattingEnabled = true;
            this.txtGreen.Location = new System.Drawing.Point(94, 56);
            this.txtGreen.Name = "txtGreen";
            this.txtGreen.Size = new System.Drawing.Size(160, 25);
            this.txtGreen.TabIndex = 4;
            // 
            // txtRed
            // 
            this.txtRed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRed.FormattingEnabled = true;
            this.txtRed.Location = new System.Drawing.Point(94, 16);
            this.txtRed.Name = "txtRed";
            this.txtRed.Size = new System.Drawing.Size(160, 25);
            this.txtRed.TabIndex = 3;
            // 
            // rdBlue
            // 
            this.rdBlue.AutoSize = true;
            this.rdBlue.ForeColor = System.Drawing.Color.Blue;
            this.rdBlue.Location = new System.Drawing.Point(11, 101);
            this.rdBlue.Name = "rdBlue";
            this.rdBlue.Size = new System.Drawing.Size(71, 21);
            this.rdBlue.TabIndex = 2;
            this.rdBlue.Text = "蓝(Blue)";
            this.rdBlue.UseVisualStyleBackColor = true;
            // 
            // rdGreen
            // 
            this.rdGreen.AutoSize = true;
            this.rdGreen.ForeColor = System.Drawing.Color.Lime;
            this.rdGreen.Location = new System.Drawing.Point(11, 58);
            this.rdGreen.Name = "rdGreen";
            this.rdGreen.Size = new System.Drawing.Size(81, 21);
            this.rdGreen.TabIndex = 1;
            this.rdGreen.Text = "绿(Green)";
            this.rdGreen.UseVisualStyleBackColor = true;
            // 
            // rdRed
            // 
            this.rdRed.AutoSize = true;
            this.rdRed.Checked = true;
            this.rdRed.ForeColor = System.Drawing.Color.Red;
            this.rdRed.Location = new System.Drawing.Point(11, 17);
            this.rdRed.Name = "rdRed";
            this.rdRed.Size = new System.Drawing.Size(69, 21);
            this.rdRed.TabIndex = 0;
            this.rdRed.TabStop = true;
            this.rdRed.Text = "红(Red)";
            this.rdRed.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.rdGray, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdRGB, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(260, 36);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // UCSelectBandForRgb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UCSelectBandForRgb";
            this.Size = new System.Drawing.Size(260, 171);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.RadioButton rdGray;
        private System.Windows.Forms.RadioButton rdRGB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox txtBlue;
        private System.Windows.Forms.ComboBox txtGreen;
        private System.Windows.Forms.ComboBox txtRed;
        private System.Windows.Forms.RadioButton rdBlue;
        private System.Windows.Forms.RadioButton rdGreen;
        private System.Windows.Forms.RadioButton rdRed;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
