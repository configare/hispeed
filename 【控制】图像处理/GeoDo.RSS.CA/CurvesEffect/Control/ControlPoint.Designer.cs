namespace GeoDo.RSS.CA
{
    partial class ControlPoint
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
            this.lstControlPoints = new System.Windows.Forms.ListBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtInputPoint = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rdPoint = new System.Windows.Forms.RadioButton();
            this.rdSegment = new System.Windows.Forms.RadioButton();
            this.lblTip = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstControlPoints
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lstControlPoints, 4);
            this.lstControlPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstControlPoints.FormattingEnabled = true;
            this.lstControlPoints.ItemHeight = 12;
            this.lstControlPoints.Location = new System.Drawing.Point(3, 25);
            this.lstControlPoints.Name = "lstControlPoints";
            this.lstControlPoints.Size = new System.Drawing.Size(208, 201);
            this.lstControlPoints.TabIndex = 9;
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClear.Location = new System.Drawing.Point(154, 232);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 24);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(59, 232);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(50, 23);
            this.btnSub.TabIndex = 11;
            this.btnSub.Text = "-";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 232);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(50, 23);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtInputPoint
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtInputPoint, 4);
            this.txtInputPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInputPoint.Location = new System.Drawing.Point(3, 262);
            this.txtInputPoint.Name = "txtInputPoint";
            this.txtInputPoint.Size = new System.Drawing.Size(208, 21);
            this.txtInputPoint.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.rdPoint, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdSegment, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lstControlPoints, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnAdd, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSub, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtInputPoint, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblTip, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(214, 319);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // rdPoint
            // 
            this.rdPoint.AutoSize = true;
            this.rdPoint.Checked = true;
            this.rdPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdPoint.Location = new System.Drawing.Point(3, 3);
            this.rdPoint.Name = "rdPoint";
            this.rdPoint.Size = new System.Drawing.Size(50, 16);
            this.rdPoint.TabIndex = 1;
            this.rdPoint.TabStop = true;
            this.rdPoint.Text = "点";
            this.rdPoint.UseVisualStyleBackColor = true;
            this.rdPoint.CheckedChanged += new System.EventHandler(this.rdPoint_CheckedChanged);
            // 
            // rdSegment
            // 
            this.rdSegment.AutoSize = true;
            this.rdSegment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdSegment.Location = new System.Drawing.Point(59, 3);
            this.rdSegment.Name = "rdSegment";
            this.rdSegment.Size = new System.Drawing.Size(50, 16);
            this.rdSegment.TabIndex = 2;
            this.rdSegment.Text = "段";
            this.rdSegment.UseVisualStyleBackColor = true;
            this.rdSegment.CheckedChanged += new System.EventHandler(this.rdSegment_CheckedChanged);
            // 
            // lblTip
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lblTip, 4);
            this.lblTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTip.Location = new System.Drawing.Point(3, 292);
            this.lblTip.Margin = new System.Windows.Forms.Padding(3);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(208, 24);
            this.lblTip.TabIndex = 14;
            this.lblTip.Text = "(oRgb,tRgb)";
            // 
            // ControlPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlPoint";
            this.Size = new System.Drawing.Size(214, 319);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstControlPoints;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtInputPoint;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton rdPoint;
        private System.Windows.Forms.RadioButton rdSegment;
        private System.Windows.Forms.Label lblTip;
    }
}
