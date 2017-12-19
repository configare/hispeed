namespace TestForm
{
    partial class frmProject1
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
            this.cbIsSolarZenith = new System.Windows.Forms.CheckBox();
            this.cbIsRadiation = new System.Windows.Forms.CheckBox();
            this.gBoxParas = new System.Windows.Forms.GroupBox();
            this.txtBandInfo = new System.Windows.Forms.TextBox();
            this.txtParaInfo = new System.Windows.Forms.TextBox();
            this.gBoxOutput = new System.Windows.Forms.GroupBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.gBoxInput = new System.Windows.Forms.GroupBox();
            this.lbSelctFiles = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnDo = new System.Windows.Forms.Button();
            this.btnOutFile = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.tsbAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbRemove = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tssbProType = new System.Windows.Forms.ToolStripSplitButton();
            this.sssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.水水水ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.等面积投影 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsbChBands = new System.Windows.Forms.ToolStripButton();
            this.btnDetails = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.gBoxParas.SuspendLayout();
            this.gBoxOutput.SuspendLayout();
            this.gBoxInput.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelButton);
            this.panelMain.Controls.Add(this.gBoxParas);
            this.panelMain.Controls.Add(this.gBoxOutput);
            this.panelMain.Controls.Add(this.gBoxInput);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 39);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(484, 373);
            this.panelMain.TabIndex = 0;
            // 
            // panelButton
            // 
            this.panelButton.Controls.Add(this.btnDetails);
            this.panelButton.Controls.Add(this.cbIsSolarZenith);
            this.panelButton.Controls.Add(this.cbIsRadiation);
            this.panelButton.Controls.Add(this.btnQuit);
            this.panelButton.Controls.Add(this.btnDo);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButton.Location = new System.Drawing.Point(0, 180);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(484, 62);
            this.panelButton.TabIndex = 3;
            // 
            // cbIsSolarZenith
            // 
            this.cbIsSolarZenith.AutoSize = true;
            this.cbIsSolarZenith.Location = new System.Drawing.Point(99, 14);
            this.cbIsSolarZenith.Name = "cbIsSolarZenith";
            this.cbIsSolarZenith.Size = new System.Drawing.Size(168, 16);
            this.cbIsSolarZenith.TabIndex = 6;
            this.cbIsSolarZenith.Text = "可见光进行太阳高度角订正";
            this.cbIsSolarZenith.UseVisualStyleBackColor = true;
            // 
            // cbIsRadiation
            // 
            this.cbIsRadiation.AutoSize = true;
            this.cbIsRadiation.Location = new System.Drawing.Point(9, 14);
            this.cbIsRadiation.Name = "cbIsRadiation";
            this.cbIsRadiation.Size = new System.Drawing.Size(84, 16);
            this.cbIsRadiation.TabIndex = 5;
            this.cbIsRadiation.Text = "转化为亮温";
            this.cbIsRadiation.UseVisualStyleBackColor = true;
            // 
            // gBoxParas
            // 
            this.gBoxParas.Controls.Add(this.txtBandInfo);
            this.gBoxParas.Controls.Add(this.txtParaInfo);
            this.gBoxParas.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gBoxParas.Location = new System.Drawing.Point(0, 242);
            this.gBoxParas.Name = "gBoxParas";
            this.gBoxParas.Size = new System.Drawing.Size(484, 131);
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
            this.txtBandInfo.Size = new System.Drawing.Size(168, 111);
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
            this.txtParaInfo.Size = new System.Drawing.Size(310, 111);
            this.txtParaInfo.TabIndex = 16;
            this.txtParaInfo.Text = "输出位置：D:\\FY3A.ldf\r\n投影类型：等经纬度投影\r\n输出范围：\r\n是否转化：true \r\n原始行列号订正：true\r\n太阳高度角订正：true";
            // 
            // gBoxOutput
            // 
            this.gBoxOutput.Controls.Add(this.txtOutFile);
            this.gBoxOutput.Controls.Add(this.btnOutFile);
            this.gBoxOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxOutput.Location = new System.Drawing.Point(0, 126);
            this.gBoxOutput.Name = "gBoxOutput";
            this.gBoxOutput.Size = new System.Drawing.Size(484, 54);
            this.gBoxOutput.TabIndex = 1;
            this.gBoxOutput.TabStop = false;
            this.gBoxOutput.Text = "输出位置";
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(12, 21);
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
            this.gBoxInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxInput.Location = new System.Drawing.Point(0, 0);
            this.gBoxInput.Name = "gBoxInput";
            this.gBoxInput.Size = new System.Drawing.Size(484, 126);
            this.gBoxInput.TabIndex = 0;
            this.gBoxInput.TabStop = false;
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
            this.lbSelctFiles.SelectedIndexChanged += new System.EventHandler(this.lbSelctFiles_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAdd,
            this.tsbRemove,
            this.tsbClear,
            this.toolStripSeparator1,
            this.tssbProType,
            this.toolStripButton1,
            this.tsbChBands});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(484, 39);
            this.toolStrip1.TabIndex = 18;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnQuit
            // 
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Image = global::TestForm.Properties.Resources.logout;
            this.btnQuit.Location = new System.Drawing.Point(406, 1);
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
            this.btnDo.Location = new System.Drawing.Point(295, 1);
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
            this.btnOutFile.Location = new System.Drawing.Point(434, 12);
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
            this.btnDown.Location = new System.Drawing.Point(431, 67);
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
            this.btnUp.Location = new System.Drawing.Point(431, 12);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 45);
            this.btnUp.TabIndex = 18;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // tsbAdd
            // 
            this.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdd.Image = global::TestForm.Properties.Resources.Add;
            this.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAdd.Name = "tsbAdd";
            this.tsbAdd.Size = new System.Drawing.Size(36, 36);
            this.tsbAdd.Text = "选择文件";
            this.tsbAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbRemove
            // 
            this.tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemove.Image = global::TestForm.Properties.Resources.Remove;
            this.tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemove.Name = "tsbRemove";
            this.tsbRemove.Size = new System.Drawing.Size(36, 36);
            this.tsbRemove.Text = "移除";
            this.tsbRemove.Click += new System.EventHandler(this.tsbRemove_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClear.Image = global::TestForm.Properties.Resources.Clear;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(36, 36);
            this.tsbClear.Text = "清空";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // tssbProType
            // 
            this.tssbProType.AutoSize = false;
            this.tssbProType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sssToolStripMenuItem,
            this.水水水ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.等面积投影,
            this.toolStripMenuItem5});
            this.tssbProType.Image = ((System.Drawing.Image)(resources.GetObject("tssbProType.Image")));
            this.tssbProType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbProType.Name = "tssbProType";
            this.tssbProType.Size = new System.Drawing.Size(130, 36);
            this.tssbProType.Text = "等经纬度投影";
            // 
            // sssToolStripMenuItem
            // 
            this.sssToolStripMenuItem.Name = "sssToolStripMenuItem";
            this.sssToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.sssToolStripMenuItem.Text = "麦卡托投影";
            // 
            // 水水水ToolStripMenuItem
            // 
            this.水水水ToolStripMenuItem.Name = "水水水ToolStripMenuItem";
            this.水水水ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.水水水ToolStripMenuItem.Text = "等经纬度投影";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem1.Text = "极射赤面投影";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem2.Text = "兰勃托投影";
            // 
            // 等面积投影
            // 
            this.等面积投影.Name = "等面积投影";
            this.等面积投影.Size = new System.Drawing.Size(148, 22);
            this.等面积投影.Text = "等面积投影";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItem5.Text = "更多类型...";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(92, 36);
            this.toolStripButton1.Text = "范围设置";
            // 
            // tsbChBands
            // 
            this.tsbChBands.Image = ((System.Drawing.Image)(resources.GetObject("tsbChBands.Image")));
            this.tsbChBands.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChBands.Name = "tsbChBands";
            this.tsbChBands.Size = new System.Drawing.Size(92, 36);
            this.tsbChBands.Text = "波段设置";
            // 
            // btnDetails
            // 
            this.btnDetails.BackgroundImage = global::TestForm.Properties.Resources.tool_down;
            this.btnDetails.FlatAppearance.BorderSize = 0;
            this.btnDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetails.Location = new System.Drawing.Point(158, 46);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(168, 13);
            this.btnDetails.TabIndex = 7;
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // frmProject1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 412);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmProject1";
            this.Text = "投影";
            this.Move += new System.EventHandler(this.frmProject_Move);
            this.panelMain.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.gBoxParas.ResumeLayout(false);
            this.gBoxParas.PerformLayout();
            this.gBoxOutput.ResumeLayout(false);
            this.gBoxOutput.PerformLayout();
            this.gBoxInput.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.GroupBox gBoxParas;
        private System.Windows.Forms.GroupBox gBoxOutput;
        private System.Windows.Forms.GroupBox gBoxInput;
        private System.Windows.Forms.TextBox txtParaInfo;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Button btnOutFile;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnDo;
        private System.Windows.Forms.TextBox txtBandInfo;
        private System.Windows.Forms.ListBox lbSelctFiles;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbAdd;
        private System.Windows.Forms.ToolStripButton tsbRemove;
        private System.Windows.Forms.ToolStripButton tsbClear;
        private System.Windows.Forms.ToolStripSplitButton tssbProType;
        private System.Windows.Forms.ToolStripMenuItem sssToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 水水水ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbChBands;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 等面积投影;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckBox cbIsSolarZenith;
        private System.Windows.Forms.CheckBox cbIsRadiation;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Button btnDetails;

    }
}

