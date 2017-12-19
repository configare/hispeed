namespace GeoDo.RSS.CA
{
    partial class frmCurveProcessorArgEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblInput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.grpControlPoints = new System.Windows.Forms.GroupBox();
            this.lstControlPoints = new System.Windows.Forms.ListBox();
            this.rdPoint = new System.Windows.Forms.RadioButton();
            this.txtInputPoint = new System.Windows.Forms.TextBox();
            this.rdSegment = new System.Windows.Forms.RadioButton();
            this.lblTip = new System.Windows.Forms.Label();
            this.btnEmpty = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cbxBandSelect = new System.Windows.Forms.ComboBox();
            this.cbxInterpolatorType = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.ck3BandView = new System.Windows.Forms.CheckBox();
            this.grpControlPoints.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(515, 41);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(515, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(516, 99);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(515, 154);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 322);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(142, 312);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(29, 12);
            this.lblOutput.TabIndex = 3;
            this.lblOutput.Text = "输出";
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(19, 312);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(29, 12);
            this.lblInput.TabIndex = 3;
            this.lblInput.Text = "输入";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(174, 307);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(60, 21);
            this.txtOutput.TabIndex = 4;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(50, 308);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(60, 21);
            this.txtInput.TabIndex = 4;
            // 
            // grpControlPoints
            // 
            this.grpControlPoints.Controls.Add(this.lstControlPoints);
            this.grpControlPoints.Controls.Add(this.rdPoint);
            this.grpControlPoints.Controls.Add(this.txtInputPoint);
            this.grpControlPoints.Controls.Add(this.rdSegment);
            this.grpControlPoints.Location = new System.Drawing.Point(296, 12);
            this.grpControlPoints.Name = "grpControlPoints";
            this.grpControlPoints.Size = new System.Drawing.Size(206, 322);
            this.grpControlPoints.TabIndex = 5;
            this.grpControlPoints.TabStop = false;
            this.grpControlPoints.Text = "控制点";
            // 
            // lstControlPoints
            // 
            this.lstControlPoints.FormattingEnabled = true;
            this.lstControlPoints.ItemHeight = 12;
            this.lstControlPoints.Location = new System.Drawing.Point(7, 36);
            this.lstControlPoints.Name = "lstControlPoints";
            this.lstControlPoints.Size = new System.Drawing.Size(191, 196);
            this.lstControlPoints.TabIndex = 8;
            // 
            // rdPoint
            // 
            this.rdPoint.AutoSize = true;
            this.rdPoint.Checked = true;
            this.rdPoint.Location = new System.Drawing.Point(10, 18);
            this.rdPoint.Name = "rdPoint";
            this.rdPoint.Size = new System.Drawing.Size(35, 16);
            this.rdPoint.TabIndex = 0;
            this.rdPoint.TabStop = true;
            this.rdPoint.Text = "点";
            this.rdPoint.UseVisualStyleBackColor = true;
            this.rdPoint.CheckedChanged += new System.EventHandler(this.rdPoint_CheckedChanged);
            // 
            // txtInputPoint
            // 
            this.txtInputPoint.Location = new System.Drawing.Point(7, 263);
            this.txtInputPoint.Name = "txtInputPoint";
            this.txtInputPoint.Size = new System.Drawing.Size(193, 21);
            this.txtInputPoint.TabIndex = 5;
            this.txtInputPoint.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInputPoint_KeyPress);
            // 
            // rdSegment
            // 
            this.rdSegment.AutoSize = true;
            this.rdSegment.Location = new System.Drawing.Point(51, 18);
            this.rdSegment.Name = "rdSegment";
            this.rdSegment.Size = new System.Drawing.Size(35, 16);
            this.rdSegment.TabIndex = 0;
            this.rdSegment.Text = "段";
            this.rdSegment.UseVisualStyleBackColor = true;
            this.rdSegment.CheckedChanged += new System.EventHandler(this.rdSegment_CheckedChanged);
            // 
            // lblTip
            // 
            this.lblTip.Location = new System.Drawing.Point(300, 303);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(200, 29);
            this.lblTip.TabIndex = 9;
            this.lblTip.Text = "(oRgb,tRgb)";
            // 
            // btnEmpty
            // 
            this.btnEmpty.Location = new System.Drawing.Point(438, 247);
            this.btnEmpty.Name = "btnEmpty";
            this.btnEmpty.Size = new System.Drawing.Size(57, 23);
            this.btnEmpty.TabIndex = 7;
            this.btnEmpty.Text = "清空";
            this.btnEmpty.UseVisualStyleBackColor = true;
            this.btnEmpty.Click += new System.EventHandler(this.btnEmpty_Click);
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(360, 247);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(50, 23);
            this.btnSub.TabIndex = 7;
            this.btnSub.Text = "-";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(302, 247);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(50, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // cbxBandSelect
            // 
            this.cbxBandSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBandSelect.FormattingEnabled = true;
            this.cbxBandSelect.Location = new System.Drawing.Point(18, 9);
            this.cbxBandSelect.Name = "cbxBandSelect";
            this.cbxBandSelect.Size = new System.Drawing.Size(62, 20);
            this.cbxBandSelect.TabIndex = 0;
            // 
            // cbxInterpolatorType
            // 
            this.cbxInterpolatorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxInterpolatorType.FormattingEnabled = true;
            this.cbxInterpolatorType.Location = new System.Drawing.Point(87, 9);
            this.cbxInterpolatorType.Name = "cbxInterpolatorType";
            this.cbxInterpolatorType.Size = new System.Drawing.Size(121, 20);
            this.cbxInterpolatorType.TabIndex = 1;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(515, 70);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "恢复默认值";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // ck3BandView
            // 
            this.ck3BandView.AutoSize = true;
            this.ck3BandView.Location = new System.Drawing.Point(515, 177);
            this.ck3BandView.Name = "ck3BandView";
            this.ck3BandView.Size = new System.Drawing.Size(84, 16);
            this.ck3BandView.TabIndex = 10;
            this.ck3BandView.Text = "分波段预览";
            this.ck3BandView.UseVisualStyleBackColor = true;
            this.ck3BandView.CheckedChanged += new System.EventHandler(this.threeBandViewer_CheckedChanged);
            // 
            // frmCurveProcessorArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 342);
            this.Controls.Add(this.ck3BandView);
            this.Controls.Add(this.cbxBandSelect);
            this.Controls.Add(this.cbxInterpolatorType);
            this.Controls.Add(this.btnEmpty);
            this.Controls.Add(this.btnSub);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblTip);
            this.Controls.Add(this.grpControlPoints);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmCurveProcessorArgEditor";
            this.Text = "曲线调整";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCurveProcessorArgEditor_FormClosed);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.grpControlPoints, 0);
            this.Controls.SetChildIndex(this.lblTip, 0);
            this.Controls.SetChildIndex(this.txtInput, 0);
            this.Controls.SetChildIndex(this.txtOutput, 0);
            this.Controls.SetChildIndex(this.lblInput, 0);
            this.Controls.SetChildIndex(this.lblOutput, 0);
            this.Controls.SetChildIndex(this.btnReset, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.btnSub, 0);
            this.Controls.SetChildIndex(this.btnEmpty, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.cbxInterpolatorType, 0);
            this.Controls.SetChildIndex(this.cbxBandSelect, 0);
            this.Controls.SetChildIndex(this.ck3BandView, 0);
            this.grpControlPoints.ResumeLayout(false);
            this.grpControlPoints.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.ComboBox cbxInterpolatorType;
        private System.Windows.Forms.ComboBox cbxBandSelect;
        private System.Windows.Forms.GroupBox grpControlPoints;
        private System.Windows.Forms.Button btnEmpty;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtInputPoint;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton rdSegment;
        private System.Windows.Forms.RadioButton rdPoint;
        private System.Windows.Forms.ListBox lstControlPoints;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.CheckBox ck3BandView;
        
    }
}