namespace GeoDo.RSS.CA.ArgEditor
{
    partial class frmLevelColorArgEditor
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
            this.panelLevel = new System.Windows.Forms.Panel();
            this.txtInputMid = new System.Windows.Forms.TextBox();
            this.numericInputMax = new System.Windows.Forms.NumericUpDown();
            this.numericInputMin = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericOutputMax = new System.Windows.Forms.NumericUpDown();
            this.numericOutputMin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxChannel = new System.Windows.Forms.ComboBox();
            this.ucHistogram = new GeoDo.RSS.CA.UCHistogram();
            this.ucSlider3 = new GeoDo.RSS.CA.UCSlider();
            this.ucSlider2 = new GeoDo.RSS.CA.UCSlider();
            this.ucSlider1 = new GeoDo.RSS.CA.UCSlider();
            this.panelLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericInputMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericInputMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOutputMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOutputMin)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(270, 35);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(270, 6);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(270, 64);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(270, 103);
            // 
            // panelLevel
            // 
            this.panelLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLevel.Controls.Add(this.txtInputMid);
            this.panelLevel.Controls.Add(this.numericInputMax);
            this.panelLevel.Controls.Add(this.numericInputMin);
            this.panelLevel.Controls.Add(this.label1);
            this.panelLevel.Controls.Add(this.ucHistogram);
            this.panelLevel.Controls.Add(this.ucSlider3);
            this.panelLevel.Controls.Add(this.ucSlider2);
            this.panelLevel.Controls.Add(this.numericOutputMax);
            this.panelLevel.Controls.Add(this.numericOutputMin);
            this.panelLevel.Controls.Add(this.label2);
            this.panelLevel.Controls.Add(this.cbxChannel);
            this.panelLevel.Location = new System.Drawing.Point(6, 6);
            this.panelLevel.Name = "panelLevel";
            this.panelLevel.Size = new System.Drawing.Size(258, 297);
            this.panelLevel.TabIndex = 4;
            // 
            // txtInputMid
            // 
            this.txtInputMid.Location = new System.Drawing.Point(135, 25);
            this.txtInputMid.Name = "txtInputMid";
            this.txtInputMid.Size = new System.Drawing.Size(53, 21);
            this.txtInputMid.TabIndex = 32;
            // 
            // numericInputMax
            // 
            this.numericInputMax.Location = new System.Drawing.Point(194, 25);
            this.numericInputMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericInputMax.Name = "numericInputMax";
            this.numericInputMax.Size = new System.Drawing.Size(50, 21);
            this.numericInputMax.TabIndex = 31;
            this.numericInputMax.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericInputMax.ValueChanged += new System.EventHandler(this.numericInputMax_ValueChanged);
            // 
            // numericInputMin
            // 
            this.numericInputMin.Location = new System.Drawing.Point(79, 25);
            this.numericInputMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericInputMin.Name = "numericInputMin";
            this.numericInputMin.Size = new System.Drawing.Size(50, 21);
            this.numericInputMin.TabIndex = 30;
            this.numericInputMin.ValueChanged += new System.EventHandler(this.numericInputMin_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 29;
            this.label1.Text = "输入色阶：";
            // 
            // numericOutputMax
            // 
            this.numericOutputMax.Location = new System.Drawing.Point(135, 230);
            this.numericOutputMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericOutputMax.Name = "numericOutputMax";
            this.numericOutputMax.Size = new System.Drawing.Size(50, 21);
            this.numericOutputMax.TabIndex = 23;
            this.numericOutputMax.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericOutputMax.ValueChanged += new System.EventHandler(this.numericOutputMax_ValueChanged);
            // 
            // numericOutputMin
            // 
            this.numericOutputMin.Location = new System.Drawing.Point(79, 230);
            this.numericOutputMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericOutputMin.Name = "numericOutputMin";
            this.numericOutputMin.Size = new System.Drawing.Size(50, 21);
            this.numericOutputMin.TabIndex = 22;
            this.numericOutputMin.ValueChanged += new System.EventHandler(this.numericOutputMin_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "输出色阶：";
            // 
            // cbxChannel
            // 
            this.cbxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChannel.FormattingEnabled = true;
            this.cbxChannel.Location = new System.Drawing.Point(-1, -1);
            this.cbxChannel.Name = "cbxChannel";
            this.cbxChannel.Size = new System.Drawing.Size(80, 20);
            this.cbxChannel.TabIndex = 0;
            this.cbxChannel.SelectedIndexChanged += new System.EventHandler(this.cbxChannel_SelectedIndexChanged);
            // 
            // ucHistogram
            // 
            this.ucHistogram.BeginValue = 0;
            this.ucHistogram.Count = 0;
            this.ucHistogram.EndValue = 255;
            this.ucHistogram.histWidth = 240;
            this.ucHistogram.IsShowInfo = true;
            this.ucHistogram.Keduchang = 5;
            this.ucHistogram.Length = 122;
            this.ucHistogram.Location = new System.Drawing.Point(10, 57);
            this.ucHistogram.Name = "ucHistogram";
            this.ucHistogram.Size = new System.Drawing.Size(240, 122);
            this.ucHistogram.Step = 0;
            this.ucHistogram.TabIndex = 27;
            this.ucHistogram.Value = new int[0];
            // 
            // ucSlider3
            // 
            this.ucSlider3.Location = new System.Drawing.Point(10, 264);
            this.ucSlider3.MaxValue = new int[] {
        255,
        255};
            this.ucSlider3.MinValue = new int[] {
        0,
        0};
            this.ucSlider3.Name = "ucSlider3";
            this.ucSlider3.Size = new System.Drawing.Size(240, 25);
            this.ucSlider3.TabIndex = 26;
            this.ucSlider3.Value = new float[] {
        0F,
        0F,
        0F};
            this.ucSlider3.BarValueChanged += new GeoDo.RSS.CA.UCSlider.BarValueChangedHandler(this.ucSlider3_BarValueChanged);
            // 
            // ucSlider2
            // 
            this.ucSlider2.Location = new System.Drawing.Point(10, 200);
            this.ucSlider2.MaxValue = new int[] {
        255,
        255,
        255};
            this.ucSlider2.MinValue = new int[] {
        0,
        0,
        0};
            this.ucSlider2.Name = "ucSlider2";
            this.ucSlider2.Size = new System.Drawing.Size(240, 25);
            this.ucSlider2.TabIndex = 25;
            this.ucSlider2.Value = new float[] {
        0F,
        128F,
        255F};
            this.ucSlider2.BarValueChanged += new GeoDo.RSS.CA.UCSlider.BarValueChangedHandler(this.ucSlider2_BarValueChanged);
            // 
            // ucSlider1
            // 
            this.ucSlider1.Location = new System.Drawing.Point(0, 0);
            this.ucSlider1.MaxValue = null;
            this.ucSlider1.MinValue = null;
            this.ucSlider1.Name = "ucSlider1";
            this.ucSlider1.Size = new System.Drawing.Size(150, 150);
            this.ucSlider1.TabIndex = 0;
            this.ucSlider1.Value = null;
            // 
            // frmLevelColorArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(348, 310);
            this.Controls.Add(this.panelLevel);
            this.Name = "frmLevelColorArgEditor";
            this.Text = "色阶";
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.panelLevel, 0);
            this.panelLevel.ResumeLayout(false);
            this.panelLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericInputMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericInputMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOutputMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOutputMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelLevel;
        private System.Windows.Forms.ComboBox cbxChannel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericOutputMax;
        private System.Windows.Forms.NumericUpDown numericOutputMin;
        private UCSlider ucSlider1;
        private UCSlider ucSlider2;
        private UCSlider ucSlider3;
        private UCHistogram ucHistogram;
        private System.Windows.Forms.TextBox txtInputMid;
        private System.Windows.Forms.NumericUpDown numericInputMax;
        private System.Windows.Forms.NumericUpDown numericInputMin;
        private System.Windows.Forms.Label label1;
    }
}