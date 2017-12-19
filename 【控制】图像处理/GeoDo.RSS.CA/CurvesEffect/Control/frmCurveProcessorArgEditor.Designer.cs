namespace GeoDo.RSS.CA
{
    partial class frmCurveProcessorArgEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCurveProcessorArgEditor));
            this.labelCoordinates = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.labelHelpText = new System.Windows.Forms.Label();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.maskComboBox = new System.Windows.Forms.ComboBox();
            this.interpolatorTypeComBox = new System.Windows.Forms.ComboBox();
            this.ck3BandView = new System.Windows.Forms.CheckBox();
            this.colourBar1 = new GeoDo.RSS.CA.ColourBar();
            this.colourBar2 = new GeoDo.RSS.CA.ColourBar();
            this.controlPoint1 = new GeoDo.RSS.CA.ControlPoint();
            this.tableLayoutMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(494, 41);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(494, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(494, 70);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(494, 109);
            // 
            // labelCoordinates
            // 
            this.labelCoordinates.AutoSize = true;
            this.labelCoordinates.Location = new System.Drawing.Point(67, 294);
            this.labelCoordinates.Margin = new System.Windows.Forms.Padding(3);
            this.labelCoordinates.Name = "labelCoordinates";
            this.labelCoordinates.Size = new System.Drawing.Size(95, 16);
            this.labelCoordinates.TabIndex = 3;
            this.labelCoordinates.Text = "labelCoordinates";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(494, 153);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 4;
            this.resetButton.Text = "重置";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // labelHelpText
            // 
            this.labelHelpText.AutoSize = true;
            this.labelHelpText.Location = new System.Drawing.Point(13, 327);
            this.labelHelpText.Name = "labelHelpText";
            this.labelHelpText.Size = new System.Drawing.Size(125, 12);
            this.labelHelpText.TabIndex = 6;
            this.labelHelpText.Text = "提示：右击删除控制点";
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 5;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 203F));
            this.tableLayoutMain.Controls.Add(this.colourBar1, 1, 3);
            this.tableLayoutMain.Controls.Add(this.colourBar2, 0, 2);
            this.tableLayoutMain.Controls.Add(this.controlPoint1, 4, 1);
            this.tableLayoutMain.Controls.Add(this.labelCoordinates, 2, 4);
            this.tableLayoutMain.Controls.Add(this.label1, 1, 1);
            this.tableLayoutMain.Controls.Add(this.maskComboBox, 2, 1);
            this.tableLayoutMain.Controls.Add(this.interpolatorTypeComBox, 3, 1);
            this.tableLayoutMain.Location = new System.Drawing.Point(12, 11);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 5;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutMain.Size = new System.Drawing.Size(476, 313);
            this.tableLayoutMain.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(25, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 29);
            this.label1.TabIndex = 9;
            this.label1.Text = "通道";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // maskComboBox
            // 
            this.maskComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maskComboBox.FormattingEnabled = true;
            this.maskComboBox.Items.AddRange(new object[] {
            "Rgb",
            "R",
            "G",
            "B"});
            this.maskComboBox.Location = new System.Drawing.Point(67, 5);
            this.maskComboBox.Name = "maskComboBox";
            this.maskComboBox.Size = new System.Drawing.Size(83, 20);
            this.maskComboBox.TabIndex = 8;
            this.maskComboBox.SelectedIndexChanged += new System.EventHandler(this.maskComboBox_SelectedIndexChanged);
            // 
            // interpolatorTypeComBox
            // 
            this.interpolatorTypeComBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.interpolatorTypeComBox.FormattingEnabled = true;
            this.interpolatorTypeComBox.Items.AddRange(new object[] {
            "曲线",
            "折线"});
            this.interpolatorTypeComBox.Location = new System.Drawing.Point(168, 5);
            this.interpolatorTypeComBox.Name = "interpolatorTypeComBox";
            this.interpolatorTypeComBox.Size = new System.Drawing.Size(83, 20);
            this.interpolatorTypeComBox.TabIndex = 14;
            // 
            // ck3BandView
            // 
            this.ck3BandView.AutoSize = true;
            this.ck3BandView.Location = new System.Drawing.Point(494, 131);
            this.ck3BandView.Name = "ck3BandView";
            this.ck3BandView.Size = new System.Drawing.Size(84, 16);
            this.ck3BandView.TabIndex = 11;
            this.ck3BandView.Text = "分波段预览";
            this.ck3BandView.UseVisualStyleBackColor = true;
            this.ck3BandView.CheckedChanged += new System.EventHandler(this.ck3BandView_CheckedChanged);
            // 
            // colourBar1
            // 
            this.colourBar1.Color1 = System.Drawing.Color.Black;
            this.colourBar1.Color2 = System.Drawing.Color.White;
            this.tableLayoutMain.SetColumnSpan(this.colourBar1, 3);
            this.colourBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colourBar1.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.colourBar1.Location = new System.Drawing.Point(25, 272);
            this.colourBar1.Name = "colourBar1";
            this.colourBar1.Size = new System.Drawing.Size(245, 16);
            this.colourBar1.TabIndex = 9;
            // 
            // colourBar2
            // 
            this.colourBar2.Color1 = System.Drawing.Color.White;
            this.colourBar2.Color2 = System.Drawing.Color.Black;
            this.colourBar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colourBar2.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.colourBar2.Location = new System.Drawing.Point(3, 34);
            this.colourBar2.Name = "colourBar2";
            this.colourBar2.Size = new System.Drawing.Size(16, 232);
            this.colourBar2.TabIndex = 10;
            // 
            // controlPoint1
            // 
            this.controlPoint1.ControlPoints = ((System.Collections.Generic.SortedList<int, int>)(resources.GetObject("controlPoint1.ControlPoints")));
            this.controlPoint1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPoint1.Location = new System.Drawing.Point(276, 5);
            this.controlPoint1.Name = "controlPoint1";
            this.tableLayoutMain.SetRowSpan(this.controlPoint1, 4);
            this.controlPoint1.Size = new System.Drawing.Size(197, 305);
            this.controlPoint1.TabIndex = 11;
            // 
            // frmCurveProcessorArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 345);
            this.Controls.Add(this.ck3BandView);
            this.Controls.Add(this.labelHelpText);
            this.Controls.Add(this.tableLayoutMain);
            this.Controls.Add(this.resetButton);
            this.Name = "frmCurveProcessorArgEditor";
            this.Text = "曲线调整";
            this.Controls.SetChildIndex(this.resetButton, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.tableLayoutMain, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.labelHelpText, 0);
            this.Controls.SetChildIndex(this.ck3BandView, 0);
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCoordinates;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label labelHelpText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private ColourBar colourBar1;
        private ColourBar colourBar2;
        private System.Windows.Forms.ComboBox maskComboBox;
        private ControlPoint controlPoint1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ck3BandView;
        private System.Windows.Forms.ComboBox interpolatorTypeComBox;

    }
}

