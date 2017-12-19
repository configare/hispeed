namespace GeoDo.RSS.UI.AddIn.HdService
{
    partial class monitoringPage
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
            if (_hdDataMonitor != null)
                _hdDataMonitor.Dispose();
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbBlock = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBlock005 = new System.Windows.Forms.RadioButton();
            this.btnBlock010 = new System.Windows.Forms.RadioButton();
            this.btnBlockCustom = new System.Windows.Forms.RadioButton();
            this.lbMosaic = new System.Windows.Forms.Label();
            this.lbFiveMini = new System.Windows.Forms.Label();
            this.notifyRadioButtonBlock = new GeoDo.RSS.UI.AddIn.HdService.NotifyLabel();
            this.notifyRadioButtonMosaic = new GeoDo.RSS.UI.AddIn.HdService.NotifyLabel();
            this.notifyRadioButtonProjection = new GeoDo.RSS.UI.AddIn.HdService.NotifyLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbDate = new System.Windows.Forms.Label();
            this.label1 = new GeoDo.RSS.UI.AddIn.HdService.ClockLabel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.dataFileterControl1 = new GeoDo.RSS.UI.AddIn.HdService.DataFileterControl();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3,
            this.progressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 519);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1083, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStatusLabel2.Image = global::GeoDo.RSS.UI.AddIn.HdService.Properties.Resources._011;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(16, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "实时监控中";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(751, 17);
            this.toolStripStatusLabel3.Spring = true;
            this.toolStripStatusLabel3.Text = "消息";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar1
            // 
            this.progressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(313, 519);
            this.panel2.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbBlock);
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Controls.Add(this.lbMosaic);
            this.panel1.Controls.Add(this.lbFiveMini);
            this.panel1.Controls.Add(this.notifyRadioButtonBlock);
            this.panel1.Controls.Add(this.notifyRadioButtonMosaic);
            this.panel1.Controls.Add(this.notifyRadioButtonProjection);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(313, 437);
            this.panel1.TabIndex = 25;
            // 
            // lbBlock
            // 
            this.lbBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbBlock.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbBlock.Location = new System.Drawing.Point(6, 247);
            this.lbBlock.Name = "lbBlock";
            this.lbBlock.Size = new System.Drawing.Size(300, 59);
            this.lbBlock.TabIndex = 29;
            this.lbBlock.Text = " 截止 : 共收到分幅数据 0条\r\n";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btnBlock005);
            this.flowLayoutPanel2.Controls.Add(this.btnBlock010);
            this.flowLayoutPanel2.Controls.Add(this.btnBlockCustom);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 394);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(313, 43);
            this.flowLayoutPanel2.TabIndex = 9;
            // 
            // btnBlock005
            // 
            this.btnBlock005.Checked = true;
            this.btnBlock005.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBlock005.Location = new System.Drawing.Point(3, 3);
            this.btnBlock005.Name = "btnBlock005";
            this.btnBlock005.Size = new System.Drawing.Size(62, 33);
            this.btnBlock005.TabIndex = 25;
            this.btnBlock005.TabStop = true;
            this.btnBlock005.Text = "5度";
            this.btnBlock005.UseVisualStyleBackColor = true;
            this.btnBlock005.CheckedChanged += new System.EventHandler(this.btnBlock005_CheckedChanged);
            // 
            // btnBlock010
            // 
            this.btnBlock010.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBlock010.Location = new System.Drawing.Point(71, 3);
            this.btnBlock010.Name = "btnBlock010";
            this.btnBlock010.Size = new System.Drawing.Size(78, 33);
            this.btnBlock010.TabIndex = 25;
            this.btnBlock010.Text = "10度";
            this.btnBlock010.UseVisualStyleBackColor = true;
            this.btnBlock010.CheckedChanged += new System.EventHandler(this.btnBlock010_CheckedChanged);
            // 
            // btnBlockCustom
            // 
            this.btnBlockCustom.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBlockCustom.Location = new System.Drawing.Point(155, 3);
            this.btnBlockCustom.Name = "btnBlockCustom";
            this.btnBlockCustom.Size = new System.Drawing.Size(128, 33);
            this.btnBlockCustom.TabIndex = 27;
            this.btnBlockCustom.Text = "自定义分幅";
            this.btnBlockCustom.UseVisualStyleBackColor = true;
            this.btnBlockCustom.CheckedChanged += new System.EventHandler(this.btnBlockCustom_CheckedChanged);
            // 
            // lbMosaic
            // 
            this.lbMosaic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMosaic.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbMosaic.Location = new System.Drawing.Point(6, 141);
            this.lbMosaic.Name = "lbMosaic";
            this.lbMosaic.Size = new System.Drawing.Size(300, 52);
            this.lbMosaic.TabIndex = 28;
            this.lbMosaic.Text = " 截止：共收到拼接数据0条";
            // 
            // lbFiveMini
            // 
            this.lbFiveMini.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFiveMini.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFiveMini.Location = new System.Drawing.Point(6, 46);
            this.lbFiveMini.Name = "lbFiveMini";
            this.lbFiveMini.Size = new System.Drawing.Size(300, 51);
            this.lbFiveMini.TabIndex = 27;
            this.lbFiveMini.Text = " 截止：共收到5分钟段数据0条";
            // 
            // notifyRadioButtonBlock
            // 
            this.notifyRadioButtonBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.notifyRadioButtonBlock.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.notifyRadioButtonBlock.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.notifyRadioButtonBlock.Location = new System.Drawing.Point(5, 193);
            this.notifyRadioButtonBlock.Name = "notifyRadioButtonBlock";
            this.notifyRadioButtonBlock.NotifyFont = new System.Drawing.Font("微软雅黑", 15F);
            this.notifyRadioButtonBlock.NotifyMessage = 0;
            this.notifyRadioButtonBlock.Size = new System.Drawing.Size(302, 41);
            this.notifyRadioButtonBlock.TabIndex = 26;
            this.notifyRadioButtonBlock.Text = "分幅数据";
            // 
            // notifyRadioButtonMosaic
            // 
            this.notifyRadioButtonMosaic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.notifyRadioButtonMosaic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.notifyRadioButtonMosaic.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.notifyRadioButtonMosaic.Location = new System.Drawing.Point(3, 97);
            this.notifyRadioButtonMosaic.Name = "notifyRadioButtonMosaic";
            this.notifyRadioButtonMosaic.NotifyFont = new System.Drawing.Font("微软雅黑", 15F);
            this.notifyRadioButtonMosaic.NotifyMessage = 0;
            this.notifyRadioButtonMosaic.Size = new System.Drawing.Size(301, 44);
            this.notifyRadioButtonMosaic.TabIndex = 25;
            this.notifyRadioButtonMosaic.Text = "投影拼接数据";
            // 
            // notifyRadioButtonProjection
            // 
            this.notifyRadioButtonProjection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.notifyRadioButtonProjection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notifyRadioButtonProjection.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.notifyRadioButtonProjection.Location = new System.Drawing.Point(6, 6);
            this.notifyRadioButtonProjection.Name = "notifyRadioButtonProjection";
            this.notifyRadioButtonProjection.NotifyFont = new System.Drawing.Font("微软雅黑", 15F);
            this.notifyRadioButtonProjection.NotifyMessage = 0;
            this.notifyRadioButtonProjection.Size = new System.Drawing.Size(302, 36);
            this.notifyRadioButtonProjection.TabIndex = 24;
            this.notifyRadioButtonProjection.TabStop = true;
            this.notifyRadioButtonProjection.Text = "5分钟段数据";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbDate);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(313, 82);
            this.panel3.TabIndex = 20;
            // 
            // lbDate
            // 
            this.lbDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDate.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbDate.ForeColor = System.Drawing.Color.Red;
            this.lbDate.Location = new System.Drawing.Point(0, 0);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(313, 50);
            this.lbDate.TabIndex = 18;
            this.lbDate.Text = "2010年10月30日";
            this.lbDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.DateFormat = "HH:mm:ss";
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 32);
            this.label1.TabIndex = 24;
            this.label1.Text = "10:44:37";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Location = new System.Drawing.Point(313, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 519);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // mapPanel
            // 
            this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel.Location = new System.Drawing.Point(316, 0);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(767, 479);
            this.mapPanel.TabIndex = 10;
            // 
            // dataFileterControl1
            // 
            this.dataFileterControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataFileterControl1.Location = new System.Drawing.Point(316, 479);
            this.dataFileterControl1.Name = "dataFileterControl1";
            this.dataFileterControl1.Size = new System.Drawing.Size(767, 40);
            this.dataFileterControl1.TabIndex = 28;
            this.dataFileterControl1.Text = "dataFileterControl1";
            // 
            // monitoringPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapPanel);
            this.Controls.Add(this.dataFileterControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Name = "monitoringPage";
            this.Size = new System.Drawing.Size(1083, 541);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Panel panel3;
        private ClockLabel label1;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private NotifyLabel notifyRadioButtonProjection;
        private NotifyLabel notifyRadioButtonBlock;
        private NotifyLabel notifyRadioButtonMosaic;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.RadioButton btnBlockCustom;
        private System.Windows.Forms.RadioButton btnBlock010;
        private System.Windows.Forms.RadioButton btnBlock005;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private DataFileterControl dataFileterControl1;
        private System.Windows.Forms.Label lbFiveMini;
        private System.Windows.Forms.Label lbMosaic;
        private System.Windows.Forms.Label lbBlock;
    }
}
