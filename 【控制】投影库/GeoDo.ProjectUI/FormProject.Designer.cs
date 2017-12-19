namespace GeoDo.ProjectUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProject));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelBands = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelButton = new System.Windows.Forms.Panel();
            this.btnDetails = new System.Windows.Forms.Button();
            this.cbIsSolarZenith = new System.Windows.Forms.CheckBox();
            this.cbIsRadiation = new System.Windows.Forms.CheckBox();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnDo = new System.Windows.Forms.Button();
            this.gBoxOutput = new System.Windows.Forms.GroupBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.btnOutFile = new System.Windows.Forms.Button();
            this.gBoxInput = new System.Windows.Forms.GroupBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.lbSelctFiles = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbRemove = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tss = new System.Windows.Forms.ToolStripSeparator();
            this.tssbProType = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmMKT = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDJWD = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmJSCM = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLBT = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDMJ = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoreType = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbOutRange = new System.Windows.Forms.ToolStripButton();
            this.tsbChBands = new System.Windows.Forms.ToolStripButton();
            this.gBoxParas = new System.Windows.Forms.GroupBox();
            this.txtBandInfo = new System.Windows.Forms.TextBox();
            this.txtParaInfo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.gBoxOutput.SuspendLayout();
            this.gBoxInput.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.gBoxParas.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelBands);
            this.panelMain.Controls.Add(this.panel2);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(664, 412);
            this.panelMain.TabIndex = 0;
            // 
            // panelBands
            // 
            this.panelBands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBands.Location = new System.Drawing.Point(485, 0);
            this.panelBands.Name = "panelBands";
            this.panelBands.Size = new System.Drawing.Size(179, 412);
            this.panelBands.TabIndex = 4;
            this.panelBands.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelButton);
            this.panel2.Controls.Add(this.gBoxOutput);
            this.panel2.Controls.Add(this.gBoxInput);
            this.panel2.Controls.Add(this.toolStrip1);
            this.panel2.Controls.Add(this.gBoxParas);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(485, 412);
            this.panel2.TabIndex = 5;
            // 
            // panelButton
            // 
            this.panelButton.Controls.Add(this.btnDetails);
            this.panelButton.Controls.Add(this.cbIsSolarZenith);
            this.panelButton.Controls.Add(this.cbIsRadiation);
            this.panelButton.Controls.Add(this.btnQuit);
            this.panelButton.Controls.Add(this.btnDo);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButton.Location = new System.Drawing.Point(0, 219);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(485, 53);
            this.panelButton.TabIndex = 3;
            // 
            // btnDetails
            // 
            this.btnDetails.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDetails.BackgroundImage")));
            this.btnDetails.FlatAppearance.BorderSize = 0;
            this.btnDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetails.Location = new System.Drawing.Point(50, 43);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(390, 10);
            this.btnDetails.TabIndex = 7;
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
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
            this.cbIsSolarZenith.CheckedChanged += new System.EventHandler(this.cbIsSolarZenith_CheckedChanged);
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
            this.cbIsRadiation.CheckedChanged += new System.EventHandler(this.cbIsRadiation_CheckedChanged);
            // 
            // btnQuit
            // 
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.btnDo.Location = new System.Drawing.Point(295, 1);
            this.btnDo.Name = "btnDo";
            this.btnDo.Size = new System.Drawing.Size(93, 40);
            this.btnDo.TabIndex = 2;
            this.btnDo.Text = "开始执行";
            this.btnDo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDo.UseVisualStyleBackColor = true;
            this.btnDo.Click += new System.EventHandler(this.btnDo_Click);
            // 
            // gBoxOutput
            // 
            this.gBoxOutput.Controls.Add(this.txtOutFile);
            this.gBoxOutput.Controls.Add(this.btnOutFile);
            this.gBoxOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxOutput.Location = new System.Drawing.Point(0, 165);
            this.gBoxOutput.Name = "gBoxOutput";
            this.gBoxOutput.Size = new System.Drawing.Size(485, 54);
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
            // btnOutFile
            // 
            this.btnOutFile.FlatAppearance.BorderSize = 0;
            this.btnOutFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOutFile.Location = new System.Drawing.Point(434, 12);
            this.btnOutFile.Name = "btnOutFile";
            this.btnOutFile.Size = new System.Drawing.Size(40, 34);
            this.btnOutFile.TabIndex = 8;
            this.btnOutFile.UseVisualStyleBackColor = true;
            this.btnOutFile.Click += new System.EventHandler(this.btnOutFile_Click);
            // 
            // gBoxInput
            // 
            this.gBoxInput.Controls.Add(this.btnDown);
            this.gBoxInput.Controls.Add(this.btnUp);
            this.gBoxInput.Controls.Add(this.lbSelctFiles);
            this.gBoxInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.gBoxInput.Location = new System.Drawing.Point(0, 39);
            this.gBoxInput.Name = "gBoxInput";
            this.gBoxInput.Size = new System.Drawing.Size(485, 126);
            this.gBoxInput.TabIndex = 0;
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
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Location = new System.Drawing.Point(431, 14);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 45);
            this.btnUp.TabIndex = 18;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
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
            this.tss,
            this.tssbProType,
            this.tsbOutRange,
            this.tsbChBands});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(485, 39);
            this.toolStrip1.TabIndex = 18;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbAdd
            // 
            this.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAdd.Name = "tsbAdd";
            this.tsbAdd.Size = new System.Drawing.Size(23, 36);
            this.tsbAdd.Text = "选择文件";
            this.tsbAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbRemove
            // 
            this.tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemove.Name = "tsbRemove";
            this.tsbRemove.Size = new System.Drawing.Size(23, 36);
            this.tsbRemove.Text = "移除";
            this.tsbRemove.Click += new System.EventHandler(this.tsbRemove_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(23, 36);
            this.tsbClear.Text = "清空";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // tss
            // 
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(6, 39);
            // 
            // tssbProType
            // 
            this.tssbProType.AutoSize = false;
            this.tssbProType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmMKT,
            this.tsmDJWD,
            this.tsmJSCM,
            this.tsmLBT,
            this.tsmDMJ,
            this.tsmiMoreType});
            this.tssbProType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbProType.Name = "tssbProType";
            this.tssbProType.Size = new System.Drawing.Size(130, 36);
            this.tssbProType.Text = "投影类型设置";
            // 
            // tsmMKT
            // 
            this.tsmMKT.Name = "tsmMKT";
            this.tsmMKT.Size = new System.Drawing.Size(152, 22);
            this.tsmMKT.Text = "麦卡托投影";
            // 
            // tsmDJWD
            // 
            this.tsmDJWD.Name = "tsmDJWD";
            this.tsmDJWD.Size = new System.Drawing.Size(152, 22);
            this.tsmDJWD.Text = "等经纬度投影";
            // 
            // tsmJSCM
            // 
            this.tsmJSCM.Name = "tsmJSCM";
            this.tsmJSCM.Size = new System.Drawing.Size(152, 22);
            this.tsmJSCM.Text = "极射赤面投影";
            // 
            // tsmLBT
            // 
            this.tsmLBT.Name = "tsmLBT";
            this.tsmLBT.Size = new System.Drawing.Size(152, 22);
            this.tsmLBT.Text = "兰勃托投影";
            // 
            // tsmDMJ
            // 
            this.tsmDMJ.Name = "tsmDMJ";
            this.tsmDMJ.Size = new System.Drawing.Size(152, 22);
            this.tsmDMJ.Text = "等面积投影";
            // 
            // tsmiMoreType
            // 
            this.tsmiMoreType.Name = "tsmiMoreType";
            this.tsmiMoreType.Size = new System.Drawing.Size(152, 22);
            this.tsmiMoreType.Text = "更多类型...";
            this.tsmiMoreType.Click += new System.EventHandler(this.tsmiMoreType_Click);
            // 
            // tsbOutRange
            // 
            this.tsbOutRange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOutRange.Name = "tsbOutRange";
            this.tsbOutRange.Size = new System.Drawing.Size(60, 36);
            this.tsbOutRange.Text = "范围设置";
            this.tsbOutRange.Click += new System.EventHandler(this.tsbOutRange_Click);
            // 
            // tsbChBands
            // 
            this.tsbChBands.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbChBands.Name = "tsbChBands";
            this.tsbChBands.Size = new System.Drawing.Size(60, 36);
            this.tsbChBands.Text = "波段设置";
            this.tsbChBands.Click += new System.EventHandler(this.tsbChBands_Click);
            // 
            // gBoxParas
            // 
            this.gBoxParas.Controls.Add(this.txtBandInfo);
            this.gBoxParas.Controls.Add(this.txtParaInfo);
            this.gBoxParas.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gBoxParas.Location = new System.Drawing.Point(0, 274);
            this.gBoxParas.Name = "gBoxParas";
            this.gBoxParas.Size = new System.Drawing.Size(485, 138);
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
            this.txtBandInfo.Size = new System.Drawing.Size(169, 118);
            this.txtBandInfo.TabIndex = 17;
            // 
            // txtParaInfo
            // 
            this.txtParaInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtParaInfo.Location = new System.Drawing.Point(3, 17);
            this.txtParaInfo.Multiline = true;
            this.txtParaInfo.Name = "txtParaInfo";
            this.txtParaInfo.ReadOnly = true;
            this.txtParaInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParaInfo.Size = new System.Drawing.Size(310, 118);
            this.txtParaInfo.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(485, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(149, 412);
            this.panel1.TabIndex = 4;
            // 
            // frmProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 412);
            this.Controls.Add(this.panelMain);
            this.Name = "frmProject";
            this.Text = "投影";
            this.panelMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.gBoxOutput.ResumeLayout(false);
            this.gBoxOutput.PerformLayout();
            this.gBoxInput.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.gBoxParas.ResumeLayout(false);
            this.gBoxParas.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ToolStripMenuItem tsmMKT;
        private System.Windows.Forms.ToolStripMenuItem tsmDJWD;
        private System.Windows.Forms.ToolStripButton tsbChBands;
        private System.Windows.Forms.ToolStripMenuItem tsmJSCM;
        private System.Windows.Forms.ToolStripMenuItem tsmLBT;
        private System.Windows.Forms.ToolStripMenuItem tsmDMJ;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoreType;
        private System.Windows.Forms.ToolStripSeparator tss;
        private System.Windows.Forms.CheckBox cbIsSolarZenith;
        private System.Windows.Forms.CheckBox cbIsRadiation;
        private System.Windows.Forms.ToolStripButton tsbOutRange;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Panel panelBands;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private Geodo.ProjectUI.UCSelectBands ucSelectBands;

    }
}

