namespace TestForm
{
    partial class frmProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProject1));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelButton = new System.Windows.Forms.Panel();
            this.gBoxParas = new System.Windows.Forms.GroupBox();
            this.txtBandInfo = new System.Windows.Forms.TextBox();
            this.txtParaInfo = new System.Windows.Forms.TextBox();
            this.gBoxOutput = new System.Windows.Forms.GroupBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.gBoxInput = new System.Windows.Forms.GroupBox();
            this.lbSelctFiles = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnDo = new System.Windows.Forms.Button();
            this.btnOutFile = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnChoose = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.gBoxParas.SuspendLayout();
            this.gBoxOutput.SuspendLayout();
            this.gBoxInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelButton);
            this.panelMain.Controls.Add(this.gBoxParas);
            this.panelMain.Controls.Add(this.gBoxOutput);
            this.panelMain.Controls.Add(this.gBoxInput);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(484, 462);
            this.panelMain.TabIndex = 0;
            // 
            // panelButton
            // 
            this.panelButton.Controls.Add(this.button3);
            this.panelButton.Controls.Add(this.btnDetail);
            this.panelButton.Controls.Add(this.btnQuit);
            this.panelButton.Controls.Add(this.btnDo);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButton.Location = new System.Drawing.Point(0, 221);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(484, 57);
            this.panelButton.TabIndex = 3;
            // 
            // gBoxParas
            // 
            this.gBoxParas.Controls.Add(this.txtBandInfo);
            this.gBoxParas.Controls.Add(this.txtParaInfo);
            this.gBoxParas.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gBoxParas.Location = new System.Drawing.Point(0, 278);
            this.gBoxParas.Name = "gBoxParas";
            this.gBoxParas.Size = new System.Drawing.Size(484, 184);
            this.gBoxParas.TabIndex = 2;
            this.gBoxParas.TabStop = false;
            this.gBoxParas.Text = "参数信息";
            this.gBoxParas.Visible = false;
            // 
            // txtBandInfo
            // 
            this.txtBandInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBandInfo.Location = new System.Drawing.Point(313, 17);
            this.txtBandInfo.Multiline = true;
            this.txtBandInfo.Name = "txtBandInfo";
            this.txtBandInfo.ReadOnly = true;
            this.txtBandInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBandInfo.Size = new System.Drawing.Size(168, 164);
            this.txtBandInfo.TabIndex = 17;
            this.txtBandInfo.Text = "投影波段：\r\n      band_1\r\n      band_2\r\n      band_3\r\n      band_4\r\n      band_5\r\n    " +
                "  band_6\r\n      band_7\r\n      band_8\r\n      band_9\r\n      band_10\r\n      band_11" +
                "";
            // 
            // txtParaInfo
            // 
            this.txtParaInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtParaInfo.Location = new System.Drawing.Point(3, 17);
            this.txtParaInfo.Multiline = true;
            this.txtParaInfo.Name = "txtParaInfo";
            this.txtParaInfo.ReadOnly = true;
            this.txtParaInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParaInfo.Size = new System.Drawing.Size(310, 164);
            this.txtParaInfo.TabIndex = 16;
            this.txtParaInfo.Text = "输出位置：D:\\FY3A.ldf\r\n投影类型：等经纬度投影\r\n输出范围：\r\n是否转化：true \r\n原始行列号订正：true\r\n太阳高度角订正：true";
            // 
            // gBoxOutput
            // 
            this.gBoxOutput.Controls.Add(this.txtOutFile);
            this.gBoxOutput.Controls.Add(this.btnOutFile);
            this.gBoxOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxOutput.Location = new System.Drawing.Point(0, 161);
            this.gBoxOutput.Name = "gBoxOutput";
            this.gBoxOutput.Size = new System.Drawing.Size(484, 60);
            this.gBoxOutput.TabIndex = 1;
            this.gBoxOutput.TabStop = false;
            this.gBoxOutput.Text = "输出位置";
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(12, 23);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(414, 21);
            this.txtOutFile.TabIndex = 7;
            this.txtOutFile.TextChanged += new System.EventHandler(this.txtOutFile_TextChanged);
            // 
            // gBoxInput
            // 
            this.gBoxInput.Controls.Add(this.btnDown);
            this.gBoxInput.Controls.Add(this.btnUp);
            this.gBoxInput.Controls.Add(this.lbSelctFiles);
            this.gBoxInput.Controls.Add(this.btnClear);
            this.gBoxInput.Controls.Add(this.btnRemove);
            this.gBoxInput.Controls.Add(this.btnChoose);
            this.gBoxInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxInput.Location = new System.Drawing.Point(0, 0);
            this.gBoxInput.Name = "gBoxInput";
            this.gBoxInput.Size = new System.Drawing.Size(484, 161);
            this.gBoxInput.TabIndex = 0;
            this.gBoxInput.TabStop = false;
            this.gBoxInput.Text = "选择投影文件";
            // 
            // lbSelctFiles
            // 
            this.lbSelctFiles.FormattingEnabled = true;
            this.lbSelctFiles.HorizontalScrollbar = true;
            this.lbSelctFiles.ItemHeight = 12;
            this.lbSelctFiles.Location = new System.Drawing.Point(12, 49);
            this.lbSelctFiles.Name = "lbSelctFiles";
            this.lbSelctFiles.Size = new System.Drawing.Size(414, 100);
            this.lbSelctFiles.TabIndex = 17;
            this.lbSelctFiles.SelectedIndexChanged += new System.EventHandler(this.lbSelctFiles_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(42, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(63, 36);
            this.button3.TabIndex = 9;
            this.button3.Text = "参数设置";
            this.button3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // btnDetail
            // 
            this.btnDetail.FlatAppearance.BorderSize = 0;
            this.btnDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetail.Image = global::TestForm.Properties.Resources.arrow_down;
            this.btnDetail.Location = new System.Drawing.Point(151, 33);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(135, 18);
            this.btnDetail.TabIndex = 4;
            this.btnDetail.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Image = global::TestForm.Properties.Resources.logout;
            this.btnQuit.Location = new System.Drawing.Point(397, 6);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 40);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "退出";
            this.btnQuit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnDo
            // 
            this.btnDo.FlatAppearance.BorderSize = 0;
            this.btnDo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDo.Image = global::TestForm.Properties.Resources.accept;
            this.btnDo.Location = new System.Drawing.Point(280, 6);
            this.btnDo.Name = "btnDo";
            this.btnDo.Size = new System.Drawing.Size(93, 40);
            this.btnDo.TabIndex = 2;
            this.btnDo.Text = "开始执行";
            this.btnDo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDo.UseVisualStyleBackColor = true;
            this.btnDo.Click += new System.EventHandler(this.btnDo_Click);
            // 
            // btnOutFile
            // 
            this.btnOutFile.FlatAppearance.BorderSize = 0;
            this.btnOutFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOutFile.Image = ((System.Drawing.Image)(resources.GetObject("btnOutFile.Image")));
            this.btnOutFile.Location = new System.Drawing.Point(432, 15);
            this.btnOutFile.Name = "btnOutFile";
            this.btnOutFile.Size = new System.Drawing.Size(40, 34);
            this.btnOutFile.TabIndex = 8;
            this.btnOutFile.UseVisualStyleBackColor = true;
            this.btnOutFile.Click += new System.EventHandler(this.btnOutFile_Click);
            // 
            // btnDown
            // 
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Image = global::TestForm.Properties.Resources.down;
            this.btnDown.Location = new System.Drawing.Point(431, 104);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(47, 45);
            this.btnDown.TabIndex = 19;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Image = global::TestForm.Properties.Resources.up;
            this.btnUp.Location = new System.Drawing.Point(431, 49);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 45);
            this.btnUp.TabIndex = 18;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnClear
            // 
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Image = global::TestForm.Properties.Resources.Clear;
            this.btnClear.Location = new System.Drawing.Point(403, 13);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(69, 31);
            this.btnClear.TabIndex = 16;
            this.btnClear.Text = "清空";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatAppearance.BorderSize = 0;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Image = global::TestForm.Properties.Resources.Remove;
            this.btnRemove.Location = new System.Drawing.Point(334, 13);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnRemove.Size = new System.Drawing.Size(63, 31);
            this.btnRemove.TabIndex = 15;
            this.btnRemove.Text = "移除";
            this.btnRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnChoose
            // 
            this.btnChoose.FlatAppearance.BorderSize = 0;
            this.btnChoose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoose.Image = global::TestForm.Properties.Resources.Add;
            this.btnChoose.Location = new System.Drawing.Point(13, 13);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(68, 31);
            this.btnChoose.TabIndex = 14;
            this.btnChoose.Text = "选择";
            this.btnChoose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChoose.UseVisualStyleBackColor = true;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // frmProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 462);
            this.Controls.Add(this.panelMain);
            this.Name = "frmProject";
            this.Text = "投影";
            this.Move += new System.EventHandler(this.frmProject_Move);
            this.panelMain.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.gBoxParas.ResumeLayout(false);
            this.gBoxParas.PerformLayout();
            this.gBoxOutput.ResumeLayout(false);
            this.gBoxOutput.PerformLayout();
            this.gBoxInput.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.GroupBox gBoxParas;
        private System.Windows.Forms.GroupBox gBoxOutput;
        private System.Windows.Forms.GroupBox gBoxInput;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnChoose;
        private System.Windows.Forms.TextBox txtParaInfo;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Button btnOutFile;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnDo;
        private System.Windows.Forms.TextBox txtBandInfo;
        private System.Windows.Forms.ListBox lbSelctFiles;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button button3;

    }
}

