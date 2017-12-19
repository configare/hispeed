namespace GeoDo.RSS.CA
{
    partial class frmBrightContrastColor
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
            this.cbxChannel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarContrast = new System.Windows.Forms.TrackBar();
            this.trackBarBright = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChangeValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericBright = new System.Windows.Forms.NumericUpDown();
            this.numericContrast = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBright)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBright)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxChannel
            // 
            this.cbxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChannel.FormattingEnabled = true;
            this.cbxChannel.Location = new System.Drawing.Point(33, 15);
            this.cbxChannel.Name = "cbxChannel";
            this.cbxChannel.Size = new System.Drawing.Size(80, 20);
            this.cbxChannel.TabIndex = 17;
            this.cbxChannel.SelectedIndexChanged += new System.EventHandler(this.cbxChannel_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "亮度(B)";
            // 
            // trackBarContrast
            // 
            this.trackBarContrast.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarContrast.Location = new System.Drawing.Point(89, 157);
            this.trackBarContrast.Maximum = 100;
            this.trackBarContrast.Minimum = -100;
            this.trackBarContrast.Name = "trackBarContrast";
            this.trackBarContrast.Size = new System.Drawing.Size(280, 45);
            this.trackBarContrast.TabIndex = 12;
            this.trackBarContrast.TickFrequency = 10;
            this.trackBarContrast.ValueChanged += new System.EventHandler(this.trackBarContrast_ValueChanged);
            // 
            // trackBarBright
            // 
            this.trackBarBright.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarBright.Location = new System.Drawing.Point(89, 70);
            this.trackBarBright.Maximum = 100;
            this.trackBarBright.Minimum = -100;
            this.trackBarBright.Name = "trackBarBright";
            this.trackBarBright.Size = new System.Drawing.Size(280, 45);
            this.trackBarBright.TabIndex = 11;
            this.trackBarBright.TickFrequency = 10;
            this.trackBarBright.ValueChanged += new System.EventHandler(this.trackBarBright_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "对比度(C)";
            // 
            // txtChangeValue
            // 
            this.txtChangeValue.Location = new System.Drawing.Point(191, 15);
            this.txtChangeValue.Name = "txtChangeValue";
            this.txtChangeValue.Size = new System.Drawing.Size(50, 21);
            this.txtChangeValue.TabIndex = 18;
            this.txtChangeValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChangeValue_KeyDown);
            this.txtChangeValue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtChangeValue_KeyUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "调整值";
            // 
            // numericBright
            // 
            this.numericBright.Location = new System.Drawing.Point(375, 72);
            this.numericBright.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericBright.Name = "numericBright";
            this.numericBright.Size = new System.Drawing.Size(50, 21);
            this.numericBright.TabIndex = 20;
            this.numericBright.ValueChanged += new System.EventHandler(this.numericBright_ValueChanged);
            // 
            // numericContrast
            // 
            this.numericContrast.Location = new System.Drawing.Point(375, 157);
            this.numericContrast.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericContrast.Name = "numericContrast";
            this.numericContrast.Size = new System.Drawing.Size(50, 21);
            this.numericContrast.TabIndex = 21;
            this.numericContrast.ValueChanged += new System.EventHandler(this.numericContrast_ValueChanged);
            // 
            // frmBrightContrastColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(525, 229);
            this.Controls.Add(this.numericContrast);
            this.Controls.Add(this.numericBright);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxChannel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBarBright);
            this.Controls.Add(this.trackBarContrast);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChangeValue);
            this.Name = "frmBrightContrastColor";
            this.Text = "亮度/对比度";
            this.Controls.SetChildIndex(this.txtChangeValue, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.trackBarContrast, 0);
            this.Controls.SetChildIndex(this.trackBarBright, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cbxChannel, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.numericBright, 0);
            this.Controls.SetChildIndex(this.numericContrast, 0);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBright)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBright)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarContrast;
        private System.Windows.Forms.TrackBar trackBarBright;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChangeValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericBright;
        private System.Windows.Forms.NumericUpDown numericContrast;

    }
}