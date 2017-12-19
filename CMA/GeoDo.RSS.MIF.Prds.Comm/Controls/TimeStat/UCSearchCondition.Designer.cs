namespace GeoDo.RSS.MIF.Prds.Comm
{
    partial class UCSearchCondition
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dbBegin = new System.Windows.Forms.DateTimePicker();
            this.dbEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.rbTimeFrame = new System.Windows.Forms.RadioButton();
            this.rbTimeRegion = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbLake = new System.Windows.Forms.ComboBox();
            this.cbResolution = new System.Windows.Forms.ComboBox();
            this.btEditArgs = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ucStatTimeFrame1 = new GeoDo.RSS.MIF.Prds.Comm.UCStatTimeFrame();
            this.ucStatTimeRegion1 = new GeoDo.RSS.MIF.Prds.Comm.UCStatTimeRegion();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "时间范围";
            // 
            // dbBegin
            // 
            this.dbBegin.Location = new System.Drawing.Point(65, 17);
            this.dbBegin.Name = "dbBegin";
            this.dbBegin.Size = new System.Drawing.Size(110, 21);
            this.dbBegin.TabIndex = 1;
            this.dbBegin.ValueChanged += new System.EventHandler(this.dbBegin_ValueChanged);
            // 
            // dbEnd
            // 
            this.dbEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dbEnd.Location = new System.Drawing.Point(182, 17);
            this.dbEnd.Name = "dbEnd";
            this.dbEnd.Size = new System.Drawing.Size(110, 21);
            this.dbEnd.TabIndex = 1;
            this.dbEnd.ValueChanged += new System.EventHandler(this.dbEnd_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "段/区间";
            // 
            // rbTimeFrame
            // 
            this.rbTimeFrame.AutoSize = true;
            this.rbTimeFrame.Checked = true;
            this.rbTimeFrame.Location = new System.Drawing.Point(65, 49);
            this.rbTimeFrame.Name = "rbTimeFrame";
            this.rbTimeFrame.Size = new System.Drawing.Size(59, 16);
            this.rbTimeFrame.TabIndex = 2;
            this.rbTimeFrame.TabStop = true;
            this.rbTimeFrame.Text = "时间段";
            this.rbTimeFrame.UseVisualStyleBackColor = true;
            this.rbTimeFrame.CheckedChanged += new System.EventHandler(this.rbTimeFrame_CheckedChanged);
            // 
            // rbTimeRegion
            // 
            this.rbTimeRegion.AutoSize = true;
            this.rbTimeRegion.Location = new System.Drawing.Point(189, 47);
            this.rbTimeRegion.Name = "rbTimeRegion";
            this.rbTimeRegion.Size = new System.Drawing.Size(71, 16);
            this.rbTimeRegion.TabIndex = 2;
            this.rbTimeRegion.Text = "时间区间";
            this.rbTimeRegion.UseVisualStyleBackColor = true;
            this.rbTimeRegion.CheckedChanged += new System.EventHandler(this.rbTimeRegion_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "区域名称";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "分辨率";
            // 
            // cbLake
            // 
            this.cbLake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLake.FormattingEnabled = true;
            this.cbLake.Location = new System.Drawing.Point(65, 134);
            this.cbLake.Name = "cbLake";
            this.cbLake.Size = new System.Drawing.Size(82, 20);
            this.cbLake.TabIndex = 5;
            this.cbLake.SelectedIndexChanged += new System.EventHandler(this.cbLake_SelectedIndexChanged);
            // 
            // cbResolution
            // 
            this.cbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResolution.FormattingEnabled = true;
            this.cbResolution.Items.AddRange(new object[] {
            "250M",
            "1000M"});
            this.cbResolution.Location = new System.Drawing.Point(202, 134);
            this.cbResolution.Name = "cbResolution";
            this.cbResolution.Size = new System.Drawing.Size(82, 20);
            this.cbResolution.TabIndex = 5;
            this.cbResolution.SelectedIndexChanged += new System.EventHandler(this.cbResolution_SelectedIndexChanged);
            // 
            // btEditArgs
            // 
            this.btEditArgs.Location = new System.Drawing.Point(209, 162);
            this.btEditArgs.Name = "btEditArgs";
            this.btEditArgs.Size = new System.Drawing.Size(75, 23);
            this.btEditArgs.TabIndex = 6;
            this.btEditArgs.Text = "编辑参数";
            this.btEditArgs.UseVisualStyleBackColor = true;
            this.btEditArgs.Click += new System.EventHandler(this.btEditArgs_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btEditArgs);
            this.groupBox1.Controls.Add(this.cbResolution);
            this.groupBox1.Controls.Add(this.cbLake);
            this.groupBox1.Controls.Add(this.rbTimeRegion);
            this.groupBox1.Controls.Add(this.rbTimeFrame);
            this.groupBox1.Controls.Add(this.dbEnd);
            this.groupBox1.Controls.Add(this.dbBegin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ucStatTimeFrame1);
            this.groupBox1.Controls.Add(this.ucStatTimeRegion1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 191);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询条件设置";
            // 
            // ucStatTimeFrame1
            // 
            this.ucStatTimeFrame1.Location = new System.Drawing.Point(0, 76);
            this.ucStatTimeFrame1.Name = "ucStatTimeFrame1";
            this.ucStatTimeFrame1.Size = new System.Drawing.Size(300, 55);
            this.ucStatTimeFrame1.TabIndex = 4;
            // 
            // ucStatTimeRegion1
            // 
            this.ucStatTimeRegion1.Location = new System.Drawing.Point(0, 72);
            this.ucStatTimeRegion1.Name = "ucStatTimeRegion1";
            this.ucStatTimeRegion1.Size = new System.Drawing.Size(300, 59);
            this.ucStatTimeRegion1.TabIndex = 3;
            this.ucStatTimeRegion1.Visible = false;
            // 
            // UCSearchCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "UCSearchCondition";
            this.Size = new System.Drawing.Size(300, 191);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dbBegin;
        private System.Windows.Forms.DateTimePicker dbEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbTimeFrame;
        private System.Windows.Forms.RadioButton rbTimeRegion;
        private UCStatTimeRegion ucStatTimeRegion1;
        private UCStatTimeFrame ucStatTimeFrame1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbLake;
        private System.Windows.Forms.ComboBox cbResolution;
        private System.Windows.Forms.Button btEditArgs;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
