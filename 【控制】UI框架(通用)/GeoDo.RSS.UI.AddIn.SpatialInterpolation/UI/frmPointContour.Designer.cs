namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    partial class frmPointContour
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
            this.FieldName = new System.Windows.Forms.ComboBox();
            this.label_field = new System.Windows.Forms.Label();
            this.btnAddValue = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnStat = new System.Windows.Forms.Button();
            this.txtSpan = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.lbSpan = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.lbMinValue = new System.Windows.Forms.Label();
            this.lvValues = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ckNeedLabel = new System.Windows.Forms.CheckBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.ckIsNeedDisplay = new System.Windows.Forms.CheckBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.txtSaveAs = new System.Windows.Forms.TextBox();
            this.lbOutShp = new System.Windows.Forms.Label();
            this.ckNeedOutputShp = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FieldName
            // 
            this.FieldName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FieldName.FormattingEnabled = true;
            this.FieldName.Location = new System.Drawing.Point(72, 16);
            this.FieldName.Name = "FieldName";
            this.FieldName.Size = new System.Drawing.Size(169, 20);
            this.FieldName.TabIndex = 3;
            // 
            // label_field
            // 
            this.label_field.AutoSize = true;
            this.label_field.Location = new System.Drawing.Point(15, 21);
            this.label_field.Name = "label_field";
            this.label_field.Size = new System.Drawing.Size(53, 12);
            this.label_field.TabIndex = 2;
            this.label_field.Text = "字段名：";
            // 
            // btnAddValue
            // 
            this.btnAddValue.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAddValue.Location = new System.Drawing.Point(123, 98);
            this.btnAddValue.Name = "btnAddValue";
            this.btnAddValue.Size = new System.Drawing.Size(59, 34);
            this.btnAddValue.TabIndex = 13;
            this.btnAddValue.Text = "增加(+)";
            this.btnAddValue.UseVisualStyleBackColor = true;
            this.btnAddValue.Click += new System.EventHandler(this.btnAddValue_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnStat);
            this.panel1.Controls.Add(this.btnAddValue);
            this.panel1.Controls.Add(this.txtSpan);
            this.panel1.Controls.Add(this.txtMaxValue);
            this.panel1.Controls.Add(this.txtMinValue);
            this.panel1.Controls.Add(this.lbSpan);
            this.panel1.Controls.Add(this.lbMaxValue);
            this.panel1.Controls.Add(this.lbMinValue);
            this.panel1.Location = new System.Drawing.Point(336, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(186, 140);
            this.panel1.TabIndex = 17;
            // 
            // btnStat
            // 
            this.btnStat.Location = new System.Drawing.Point(7, 99);
            this.btnStat.Name = "btnStat";
            this.btnStat.Size = new System.Drawing.Size(110, 24);
            this.btnStat.TabIndex = 14;
            this.btnStat.Text = "统计";
            this.btnStat.UseVisualStyleBackColor = true;
            this.btnStat.Click += new System.EventHandler(this.btnStat_Click);
            // 
            // txtSpan
            // 
            this.txtSpan.Location = new System.Drawing.Point(64, 67);
            this.txtSpan.Name = "txtSpan";
            this.txtSpan.Size = new System.Drawing.Size(114, 21);
            this.txtSpan.TabIndex = 5;
            this.txtSpan.Text = "50";
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(64, 38);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(115, 21);
            this.txtMaxValue.TabIndex = 4;
            this.txtMaxValue.Text = "255";
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(64, 9);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(115, 21);
            this.txtMinValue.TabIndex = 3;
            this.txtMinValue.Text = "0";
            // 
            // lbSpan
            // 
            this.lbSpan.AutoSize = true;
            this.lbSpan.Location = new System.Drawing.Point(14, 73);
            this.lbSpan.Name = "lbSpan";
            this.lbSpan.Size = new System.Drawing.Size(41, 12);
            this.lbSpan.TabIndex = 2;
            this.lbSpan.Text = "间隔值";
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.AutoSize = true;
            this.lbMaxValue.Location = new System.Drawing.Point(14, 44);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(41, 12);
            this.lbMaxValue.TabIndex = 1;
            this.lbMaxValue.Text = "最大值";
            // 
            // lbMinValue
            // 
            this.lbMinValue.AutoSize = true;
            this.lbMinValue.Location = new System.Drawing.Point(13, 14);
            this.lbMinValue.Name = "lbMinValue";
            this.lbMinValue.Size = new System.Drawing.Size(41, 12);
            this.lbMinValue.TabIndex = 0;
            this.lbMinValue.Text = "最小值";
            // 
            // lvValues
            // 
            this.lvValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvValues.FullRowSelect = true;
            this.lvValues.Location = new System.Drawing.Point(17, 63);
            this.lvValues.Name = "lvValues";
            this.lvValues.Size = new System.Drawing.Size(309, 120);
            this.lvValues.TabIndex = 20;
            this.lvValues.UseCompatibleStateImageBehavior = false;
            this.lvValues.View = System.Windows.Forms.View.Details;
            this.lvValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvValues_MouseDoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "等值";
            this.columnHeader3.Width = 107;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "绘制颜色";
            this.columnHeader4.Width = 112;
            // 
            // ckNeedLabel
            // 
            this.ckNeedLabel.AutoSize = true;
            this.ckNeedLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedLabel.Location = new System.Drawing.Point(336, 16);
            this.ckNeedLabel.Name = "ckNeedLabel";
            this.ckNeedLabel.Size = new System.Drawing.Size(75, 21);
            this.ckNeedLabel.TabIndex = 22;
            this.ckNeedLabel.Text = "是否标注";
            this.ckNeedLabel.UseVisualStyleBackColor = true;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(15, 189);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAll.TabIndex = 18;
            this.btnRemoveAll.Text = "移除所有";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // ckIsNeedDisplay
            // 
            this.ckIsNeedDisplay.AutoSize = true;
            this.ckIsNeedDisplay.Checked = true;
            this.ckIsNeedDisplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsNeedDisplay.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckIsNeedDisplay.Location = new System.Drawing.Point(254, 16);
            this.ckIsNeedDisplay.Name = "ckIsNeedDisplay";
            this.ckIsNeedDisplay.Size = new System.Drawing.Size(75, 21);
            this.ckIsNeedDisplay.TabIndex = 21;
            this.ckIsNeedDisplay.Text = "是否显示";
            this.ckIsNeedDisplay.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(96, 189);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 19;
            this.btnRemove.Text = "移除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(154, 270);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(315, 270);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSaveAs.Location = new System.Drawing.Point(491, 235);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(31, 23);
            this.btnSaveAs.TabIndex = 27;
            this.btnSaveAs.Text = "...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // txtSaveAs
            // 
            this.txtSaveAs.Enabled = false;
            this.txtSaveAs.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSaveAs.Location = new System.Drawing.Point(126, 235);
            this.txtSaveAs.Name = "txtSaveAs";
            this.txtSaveAs.Size = new System.Drawing.Size(359, 23);
            this.txtSaveAs.TabIndex = 26;
            // 
            // lbOutShp
            // 
            this.lbOutShp.AutoSize = true;
            this.lbOutShp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbOutShp.Location = new System.Drawing.Point(16, 238);
            this.lbOutShp.Name = "lbOutShp";
            this.lbOutShp.Size = new System.Drawing.Size(104, 17);
            this.lbOutShp.TabIndex = 25;
            this.lbOutShp.Text = "输出矢量文件名：";
            // 
            // ckNeedOutputShp
            // 
            this.ckNeedOutputShp.AutoSize = true;
            this.ckNeedOutputShp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckNeedOutputShp.Location = new System.Drawing.Point(419, 16);
            this.ckNeedOutputShp.Name = "ckNeedOutputShp";
            this.ckNeedOutputShp.Size = new System.Drawing.Size(99, 21);
            this.ckNeedOutputShp.TabIndex = 28;
            this.ckNeedOutputShp.Text = "是否输出矢量";
            this.ckNeedOutputShp.UseVisualStyleBackColor = true;
            this.ckNeedOutputShp.CheckedChanged += new System.EventHandler(this.ckNeedOutputShp_CheckedChanged);
            // 
            // frmPointContour
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 303);
            this.Controls.Add(this.ckNeedOutputShp);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.txtSaveAs);
            this.Controls.Add(this.lbOutShp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lvValues);
            this.Controls.Add(this.ckNeedLabel);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.ckIsNeedDisplay);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.FieldName);
            this.Controls.Add(this.label_field);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPointContour";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "等值线参数设置";
            this.Load += new System.EventHandler(this.frmPointContour_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FieldName;
        private System.Windows.Forms.Label label_field;
        private System.Windows.Forms.Button btnAddValue;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStat;
        private System.Windows.Forms.TextBox txtSpan;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.Label lbSpan;
        private System.Windows.Forms.Label lbMaxValue;
        private System.Windows.Forms.Label lbMinValue;
        private System.Windows.Forms.ListView lvValues;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.CheckBox ckNeedLabel;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.CheckBox ckIsNeedDisplay;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.TextBox txtSaveAs;
        private System.Windows.Forms.Label lbOutShp;
        private System.Windows.Forms.CheckBox ckNeedOutputShp;
    }
}