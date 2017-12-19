namespace GeoDo.RSS.UI.AddIn.ReportService
{
    partial class ReportPageControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.chkHasToDb = new System.Windows.Forms.CheckBox();
            this.btnCheckedAll = new System.Windows.Forms.Button();
            this.btnCheckedNone = new System.Windows.Forms.Button();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnToDb = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.cbSubFolder = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(252, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(621, 536);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.cbSubFolder);
            this.panel2.Controls.Add(this.dateTimePicker1);
            this.panel2.Controls.Add(this.chkHasToDb);
            this.panel2.Controls.Add(this.btnCheckedAll);
            this.panel2.Controls.Add(this.btnCheckedNone);
            this.panel2.Controls.Add(this.btnLoadData);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 536);
            this.panel2.TabIndex = 2;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePicker1.Location = new System.Drawing.Point(26, 14);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(199, 21);
            this.dateTimePicker1.TabIndex = 7;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // chkHasToDb
            // 
            this.chkHasToDb.AutoSize = true;
            this.chkHasToDb.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkHasToDb.Location = new System.Drawing.Point(32, 253);
            this.chkHasToDb.Name = "chkHasToDb";
            this.chkHasToDb.Size = new System.Drawing.Size(139, 20);
            this.chkHasToDb.TabIndex = 5;
            this.chkHasToDb.Text = "显示已提交素材";
            this.chkHasToDb.UseVisualStyleBackColor = true;
            this.chkHasToDb.CheckedChanged += new System.EventHandler(this.chkHasToDb_CheckedChanged);
            // 
            // btnCheckedAll
            // 
            this.btnCheckedAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckedAll.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCheckedAll.Location = new System.Drawing.Point(26, 117);
            this.btnCheckedAll.Name = "btnCheckedAll";
            this.btnCheckedAll.Size = new System.Drawing.Size(199, 56);
            this.btnCheckedAll.TabIndex = 4;
            this.btnCheckedAll.Text = "全选";
            this.btnCheckedAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCheckedAll.UseVisualStyleBackColor = true;
            this.btnCheckedAll.Click += new System.EventHandler(this.btnCheckedAll_Click);
            // 
            // btnCheckedNone
            // 
            this.btnCheckedNone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckedNone.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCheckedNone.Location = new System.Drawing.Point(26, 185);
            this.btnCheckedNone.Name = "btnCheckedNone";
            this.btnCheckedNone.Size = new System.Drawing.Size(199, 56);
            this.btnCheckedNone.TabIndex = 3;
            this.btnCheckedNone.Text = "全不选";
            this.btnCheckedNone.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCheckedNone.UseVisualStyleBackColor = true;
            this.btnCheckedNone.Click += new System.EventHandler(this.btnCheckedNone_Click);
            // 
            // btnLoadData
            // 
            this.btnLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadData.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLoadData.Image = global::GeoDo.RSS.UI.AddIn.ReportService.Properties.Resources.database_refresh;
            this.btnLoadData.Location = new System.Drawing.Point(26, 49);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(199, 56);
            this.btnLoadData.TabIndex = 2;
            this.btnLoadData.Text = "刷新数据";
            this.btnLoadData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnToDb);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 436);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(252, 100);
            this.panel3.TabIndex = 1;
            // 
            // btnToDb
            // 
            this.btnToDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToDb.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnToDb.Image = global::GeoDo.RSS.UI.AddIn.ReportService.Properties.Resources.database_save;
            this.btnToDb.Location = new System.Drawing.Point(26, 20);
            this.btnToDb.Name = "btnToDb";
            this.btnToDb.Size = new System.Drawing.Size(199, 56);
            this.btnToDb.TabIndex = 0;
            this.btnToDb.Text = "提交报告素材";
            this.btnToDb.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnToDb.UseVisualStyleBackColor = true;
            this.btnToDb.Click += new System.EventHandler(this.btnToDb_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbMessage,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 536);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(873, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbMessage
            // 
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(656, 17);
            this.lbMessage.Spring = true;
            this.lbMessage.Text = "消息";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(252, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 536);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // cbSubFolder
            // 
            this.cbSubFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSubFolder.Font = new System.Drawing.Font("SimSun", 10F);
            this.cbSubFolder.FormattingEnabled = true;
            this.cbSubFolder.Location = new System.Drawing.Point(26, 404);
            this.cbSubFolder.Name = "cbSubFolder";
            this.cbSubFolder.Size = new System.Drawing.Size(199, 21);
            this.cbSubFolder.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 12F);
            this.label1.Location = new System.Drawing.Point(23, 374);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "素材子目录";
            // 
            // ReportPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ReportPageControl";
            this.Size = new System.Drawing.Size(873, 558);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnToDb;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnCheckedAll;
        private System.Windows.Forms.Button btnCheckedNone;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbMessage;
        private System.Windows.Forms.CheckBox chkHasToDb;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSubFolder;
    }
}
