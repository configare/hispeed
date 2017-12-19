namespace GeoDo.RSS.MIF.Prds.LST
{
    partial class UCRegionArgs
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbxValues = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.cbCycType = new System.Windows.Forms.ComboBox();
            this.ckTime = new System.Windows.Forms.CheckBox();
            this.ckSatellite = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ckYear = new System.Windows.Forms.CheckBox();
            this.ck90Days = new System.Windows.Forms.CheckBox();
            this.ckMonth = new System.Windows.Forms.CheckBox();
            this.ck10Days = new System.Windows.Forms.CheckBox();
            this.ckDay = new System.Windows.Forms.CheckBox();
            this.ckWeek = new System.Windows.Forms.CheckBox();
            this.panelTime = new System.Windows.Forms.Panel();
            this.DTEnd = new System.Windows.Forms.DateTimePicker();
            this.DTBegin = new System.Windows.Forms.DateTimePicker();
            this.panel1.SuspendLayout();
            this.panelTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 125);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(101, 125);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "开始时间";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "结束时间";
            // 
            // lbxValues
            // 
            this.lbxValues.FormattingEnabled = true;
            this.lbxValues.ItemHeight = 12;
            this.lbxValues.Location = new System.Drawing.Point(3, 43);
            this.lbxValues.Name = "lbxValues";
            this.lbxValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxValues.Size = new System.Drawing.Size(272, 76);
            this.lbxValues.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(199, 125);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cbCycType);
            this.panel1.Controls.Add(this.ckTime);
            this.panel1.Controls.Add(this.ckSatellite);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ckYear);
            this.panel1.Controls.Add(this.ck90Days);
            this.panel1.Controls.Add(this.ckMonth);
            this.panel1.Controls.Add(this.ck10Days);
            this.panel1.Controls.Add(this.ckDay);
            this.panel1.Controls.Add(this.ckWeek);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 104);
            this.panel1.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(198, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = ">=最小 <最大";
            // 
            // cbCycType
            // 
            this.cbCycType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCycType.FormattingEnabled = true;
            this.cbCycType.Location = new System.Drawing.Point(75, 9);
            this.cbCycType.Name = "cbCycType";
            this.cbCycType.Size = new System.Drawing.Size(200, 20);
            this.cbCycType.TabIndex = 16;
            this.cbCycType.SelectedIndexChanged += new System.EventHandler(this.cbCycType_SelectedIndexChanged);
            // 
            // ckTime
            // 
            this.ckTime.AutoSize = true;
            this.ckTime.Location = new System.Drawing.Point(120, 78);
            this.ckTime.Name = "ckTime";
            this.ckTime.Size = new System.Drawing.Size(72, 16);
            this.ckTime.TabIndex = 14;
            this.ckTime.Text = "分时间段";
            this.ckTime.UseVisualStyleBackColor = true;
            this.ckTime.CheckedChanged += new System.EventHandler(this.ckTime_CheckedChanged);
            // 
            // ckSatellite
            // 
            this.ckSatellite.AutoSize = true;
            this.ckSatellite.Location = new System.Drawing.Point(6, 78);
            this.ckSatellite.Name = "ckSatellite";
            this.ckSatellite.Size = new System.Drawing.Size(108, 16);
            this.ckSatellite.TabIndex = 14;
            this.ckSatellite.Text = "分卫星、传感器";
            this.ckSatellite.UseVisualStyleBackColor = true;
            this.ckSatellite.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "合成方式：";
            // 
            // ckYear
            // 
            this.ckYear.AutoSize = true;
            this.ckYear.Location = new System.Drawing.Point(226, 42);
            this.ckYear.Name = "ckYear";
            this.ckYear.Size = new System.Drawing.Size(36, 16);
            this.ckYear.TabIndex = 14;
            this.ckYear.Text = "年";
            this.ckYear.UseVisualStyleBackColor = true;
            this.ckYear.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ck90Days
            // 
            this.ck90Days.AutoSize = true;
            this.ck90Days.Location = new System.Drawing.Point(182, 42);
            this.ck90Days.Name = "ck90Days";
            this.ck90Days.Size = new System.Drawing.Size(36, 16);
            this.ck90Days.TabIndex = 14;
            this.ck90Days.Text = "季";
            this.ck90Days.UseVisualStyleBackColor = true;
            this.ck90Days.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ckMonth
            // 
            this.ckMonth.AutoSize = true;
            this.ckMonth.Location = new System.Drawing.Point(138, 42);
            this.ckMonth.Name = "ckMonth";
            this.ckMonth.Size = new System.Drawing.Size(36, 16);
            this.ckMonth.TabIndex = 14;
            this.ckMonth.Text = "月";
            this.ckMonth.UseVisualStyleBackColor = true;
            this.ckMonth.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ck10Days
            // 
            this.ck10Days.AutoSize = true;
            this.ck10Days.Location = new System.Drawing.Point(94, 42);
            this.ck10Days.Name = "ck10Days";
            this.ck10Days.Size = new System.Drawing.Size(36, 16);
            this.ck10Days.TabIndex = 14;
            this.ck10Days.Text = "旬";
            this.ck10Days.UseVisualStyleBackColor = true;
            this.ck10Days.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ckDay
            // 
            this.ckDay.AutoSize = true;
            this.ckDay.Location = new System.Drawing.Point(6, 42);
            this.ckDay.Name = "ckDay";
            this.ckDay.Size = new System.Drawing.Size(36, 16);
            this.ckDay.TabIndex = 14;
            this.ckDay.Text = "日";
            this.ckDay.UseVisualStyleBackColor = true;
            this.ckDay.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ckWeek
            // 
            this.ckWeek.AutoSize = true;
            this.ckWeek.Location = new System.Drawing.Point(50, 42);
            this.ckWeek.Name = "ckWeek";
            this.ckWeek.Size = new System.Drawing.Size(36, 16);
            this.ckWeek.TabIndex = 14;
            this.ckWeek.Text = "周";
            this.ckWeek.UseVisualStyleBackColor = true;
            this.ckWeek.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // panelTime
            // 
            this.panelTime.Controls.Add(this.DTEnd);
            this.panelTime.Controls.Add(this.DTBegin);
            this.panelTime.Controls.Add(this.btnAdd);
            this.panelTime.Controls.Add(this.btnDelete);
            this.panelTime.Controls.Add(this.label1);
            this.panelTime.Controls.Add(this.label2);
            this.panelTime.Controls.Add(this.lbxValues);
            this.panelTime.Controls.Add(this.btnOK);
            this.panelTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTime.Enabled = false;
            this.panelTime.Location = new System.Drawing.Point(0, 104);
            this.panelTime.Name = "panelTime";
            this.panelTime.Size = new System.Drawing.Size(292, 157);
            this.panelTime.TabIndex = 15;
            // 
            // DTEnd
            // 
            this.DTEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.DTEnd.Location = new System.Drawing.Point(198, 11);
            this.DTEnd.Name = "DTEnd";
            this.DTEnd.Size = new System.Drawing.Size(77, 21);
            this.DTEnd.TabIndex = 13;
            this.DTEnd.Value = new System.DateTime(2013, 12, 26, 20, 0, 0, 0);
            this.DTEnd.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // DTBegin
            // 
            this.DTBegin.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.DTBegin.Location = new System.Drawing.Point(58, 11);
            this.DTBegin.Name = "DTBegin";
            this.DTBegin.Size = new System.Drawing.Size(72, 21);
            this.DTBegin.TabIndex = 13;
            this.DTBegin.Value = new System.DateTime(2013, 12, 26, 6, 0, 0, 0);
            this.DTBegin.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // UCRegionArgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelTime);
            this.Controls.Add(this.panel1);
            this.Name = "UCRegionArgs";
            this.Size = new System.Drawing.Size(292, 261);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelTime.ResumeLayout(false);
            this.panelTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbxValues;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker DTEnd;
        private System.Windows.Forms.DateTimePicker DTBegin;
        private System.Windows.Forms.CheckBox ckTime;
        private System.Windows.Forms.CheckBox ckSatellite;
        private System.Windows.Forms.CheckBox ckYear;
        private System.Windows.Forms.CheckBox ck90Days;
        private System.Windows.Forms.CheckBox ckMonth;
        private System.Windows.Forms.CheckBox ck10Days;
        private System.Windows.Forms.CheckBox ckWeek;
        private System.Windows.Forms.Panel panelTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCycType;
        private System.Windows.Forms.CheckBox ckDay;
        private System.Windows.Forms.Label label4;
    }
}
