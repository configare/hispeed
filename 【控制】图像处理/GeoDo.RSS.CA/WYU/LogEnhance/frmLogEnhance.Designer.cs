namespace GeoDo.RSS.CA
{
    partial class frmLogEnhance
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdSingleChannel = new System.Windows.Forms.RadioButton();
            this.rdFullChannel = new System.Windows.Forms.RadioButton();
            this.txtBaseBlue = new System.Windows.Forms.ComboBox();
            this.labelScaleBlue = new System.Windows.Forms.Label();
            this.labelBaseBlue = new System.Windows.Forms.Label();
            this.txtScaleBlue = new System.Windows.Forms.NumericUpDown();
            this.labelBlue = new System.Windows.Forms.Label();
            this.txtBaseGreen = new System.Windows.Forms.ComboBox();
            this.labelScaleGreen = new System.Windows.Forms.Label();
            this.labelBaseRed = new System.Windows.Forms.Label();
            this.txtScaleGreen = new System.Windows.Forms.NumericUpDown();
            this.labelGreen = new System.Windows.Forms.Label();
            this.txtBaseRed = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtScaleRed = new System.Windows.Forms.NumericUpDown();
            this.labelRed = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleRed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(372, 41);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(372, 12);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(372, 70);
            // 
            // ckPreviewing
            // 
            this.ckPreviewing.Location = new System.Drawing.Point(372, 109);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdSingleChannel);
            this.groupBox2.Controls.Add(this.rdFullChannel);
            this.groupBox2.Controls.Add(this.txtBaseBlue);
            this.groupBox2.Controls.Add(this.labelScaleBlue);
            this.groupBox2.Controls.Add(this.labelBaseBlue);
            this.groupBox2.Controls.Add(this.txtScaleBlue);
            this.groupBox2.Controls.Add(this.labelBlue);
            this.groupBox2.Controls.Add(this.txtBaseGreen);
            this.groupBox2.Controls.Add(this.labelScaleGreen);
            this.groupBox2.Controls.Add(this.labelBaseRed);
            this.groupBox2.Controls.Add(this.txtScaleGreen);
            this.groupBox2.Controls.Add(this.labelGreen);
            this.groupBox2.Controls.Add(this.txtBaseRed);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtScaleRed);
            this.groupBox2.Controls.Add(this.labelRed);
            this.groupBox2.Location = new System.Drawing.Point(12, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 182);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数";
            // 
            // rdSingleChannel
            // 
            this.rdSingleChannel.AutoSize = true;
            this.rdSingleChannel.Location = new System.Drawing.Point(163, 21);
            this.rdSingleChannel.Name = "rdSingleChannel";
            this.rdSingleChannel.Size = new System.Drawing.Size(131, 16);
            this.rdSingleChannel.TabIndex = 29;
            this.rdSingleChannel.Text = "各通道分别设置参数";
            this.rdSingleChannel.UseVisualStyleBackColor = true;
            this.rdSingleChannel.CheckedChanged += new System.EventHandler(this.rdSingleChannel_CheckedChanged);
            // 
            // rdFullChannel
            // 
            this.rdFullChannel.AutoSize = true;
            this.rdFullChannel.Checked = true;
            this.rdFullChannel.Location = new System.Drawing.Point(24, 21);
            this.rdFullChannel.Name = "rdFullChannel";
            this.rdFullChannel.Size = new System.Drawing.Size(131, 16);
            this.rdFullChannel.TabIndex = 28;
            this.rdFullChannel.TabStop = true;
            this.rdFullChannel.Text = "各通道使用相同参数";
            this.rdFullChannel.UseVisualStyleBackColor = true;
            this.rdFullChannel.CheckedChanged += new System.EventHandler(this.rdFullChannel_CheckedChanged);
            // 
            // txtBaseBlue
            // 
            this.txtBaseBlue.FormattingEnabled = true;
            this.txtBaseBlue.Items.AddRange(new object[] {
            "2",
            "10",
            "E"});
            this.txtBaseBlue.Location = new System.Drawing.Point(89, 145);
            this.txtBaseBlue.Name = "txtBaseBlue";
            this.txtBaseBlue.Size = new System.Drawing.Size(64, 20);
            this.txtBaseBlue.TabIndex = 27;
            this.txtBaseBlue.TextUpdate += new System.EventHandler(this.txtBaseBlue_TextUpdate);
            // 
            // labelScaleBlue
            // 
            this.labelScaleBlue.AutoSize = true;
            this.labelScaleBlue.Location = new System.Drawing.Point(226, 153);
            this.labelScaleBlue.Name = "labelScaleBlue";
            this.labelScaleBlue.Size = new System.Drawing.Size(53, 12);
            this.labelScaleBlue.TabIndex = 26;
            this.labelScaleBlue.Text = "系数(c):";
            // 
            // labelBaseBlue
            // 
            this.labelBaseBlue.AutoSize = true;
            this.labelBaseBlue.Location = new System.Drawing.Point(38, 153);
            this.labelBaseBlue.Name = "labelBaseBlue";
            this.labelBaseBlue.Size = new System.Drawing.Size(53, 12);
            this.labelBaseBlue.TabIndex = 25;
            this.labelBaseBlue.Text = "底数(b):";
            // 
            // txtScaleBlue
            // 
            this.txtScaleBlue.Location = new System.Drawing.Point(281, 144);
            this.txtScaleBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtScaleBlue.Name = "txtScaleBlue";
            this.txtScaleBlue.Size = new System.Drawing.Size(45, 21);
            this.txtScaleBlue.TabIndex = 24;
            this.txtScaleBlue.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtScaleBlue.ValueChanged += new System.EventHandler(this.txtScaleBlue_ValueChanged);
            // 
            // labelBlue
            // 
            this.labelBlue.AutoSize = true;
            this.labelBlue.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelBlue.ForeColor = System.Drawing.Color.Blue;
            this.labelBlue.Location = new System.Drawing.Point(19, 130);
            this.labelBlue.Name = "labelBlue";
            this.labelBlue.Size = new System.Drawing.Size(29, 25);
            this.labelBlue.TabIndex = 23;
            this.labelBlue.Text = "B:";
            // 
            // txtBaseGreen
            // 
            this.txtBaseGreen.FormattingEnabled = true;
            this.txtBaseGreen.Items.AddRange(new object[] {
            "2",
            "10",
            "E"});
            this.txtBaseGreen.Location = new System.Drawing.Point(89, 101);
            this.txtBaseGreen.Name = "txtBaseGreen";
            this.txtBaseGreen.Size = new System.Drawing.Size(64, 20);
            this.txtBaseGreen.TabIndex = 22;
            this.txtBaseGreen.TextUpdate += new System.EventHandler(this.txtBaseGreen_TextUpdate);
            // 
            // labelScaleGreen
            // 
            this.labelScaleGreen.AutoSize = true;
            this.labelScaleGreen.Location = new System.Drawing.Point(226, 109);
            this.labelScaleGreen.Name = "labelScaleGreen";
            this.labelScaleGreen.Size = new System.Drawing.Size(53, 12);
            this.labelScaleGreen.TabIndex = 21;
            this.labelScaleGreen.Text = "系数(c):";
            // 
            // labelBaseRed
            // 
            this.labelBaseRed.AutoSize = true;
            this.labelBaseRed.Location = new System.Drawing.Point(38, 109);
            this.labelBaseRed.Name = "labelBaseRed";
            this.labelBaseRed.Size = new System.Drawing.Size(53, 12);
            this.labelBaseRed.TabIndex = 20;
            this.labelBaseRed.Text = "底数(b):";
            // 
            // txtScaleGreen
            // 
            this.txtScaleGreen.Location = new System.Drawing.Point(281, 100);
            this.txtScaleGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtScaleGreen.Name = "txtScaleGreen";
            this.txtScaleGreen.Size = new System.Drawing.Size(45, 21);
            this.txtScaleGreen.TabIndex = 19;
            this.txtScaleGreen.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtScaleGreen.ValueChanged += new System.EventHandler(this.txtScaleGreen_ValueChanged);
            // 
            // labelGreen
            // 
            this.labelGreen.AutoSize = true;
            this.labelGreen.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelGreen.ForeColor = System.Drawing.Color.Green;
            this.labelGreen.Location = new System.Drawing.Point(19, 86);
            this.labelGreen.Name = "labelGreen";
            this.labelGreen.Size = new System.Drawing.Size(31, 25);
            this.labelGreen.TabIndex = 18;
            this.labelGreen.Text = "G:";
            // 
            // txtBaseRed
            // 
            this.txtBaseRed.FormattingEnabled = true;
            this.txtBaseRed.Items.AddRange(new object[] {
            "2",
            "10",
            "E"});
            this.txtBaseRed.Location = new System.Drawing.Point(89, 55);
            this.txtBaseRed.Name = "txtBaseRed";
            this.txtBaseRed.Size = new System.Drawing.Size(64, 20);
            this.txtBaseRed.TabIndex = 17;
            this.txtBaseRed.TextChanged += new System.EventHandler(this.txtBaseRed_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "系数(c):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "底数(b):";
            // 
            // txtScaleRed
            // 
            this.txtScaleRed.Location = new System.Drawing.Point(281, 54);
            this.txtScaleRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtScaleRed.Name = "txtScaleRed";
            this.txtScaleRed.Size = new System.Drawing.Size(45, 21);
            this.txtScaleRed.TabIndex = 8;
            this.txtScaleRed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtScaleRed.ValueChanged += new System.EventHandler(this.txtScaleRed_ValueChanged);
            // 
            // labelRed
            // 
            this.labelRed.AutoSize = true;
            this.labelRed.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelRed.ForeColor = System.Drawing.Color.Red;
            this.labelRed.Location = new System.Drawing.Point(19, 40);
            this.labelRed.Name = "labelRed";
            this.labelRed.Size = new System.Drawing.Size(29, 25);
            this.labelRed.TabIndex = 6;
            this.labelRed.Text = "R:";
            // 
            // frmLogEnhance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 191);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmLogEnhance";
            this.ShowIcon = false;
            this.Text = "对数增强(y = c * log_b(X)";
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScaleRed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown txtScaleRed;
        private System.Windows.Forms.Label labelRed;
        private System.Windows.Forms.ComboBox txtBaseBlue;
        private System.Windows.Forms.Label labelScaleBlue;
        private System.Windows.Forms.Label labelBaseBlue;
        private System.Windows.Forms.NumericUpDown txtScaleBlue;
        private System.Windows.Forms.Label labelBlue;
        private System.Windows.Forms.ComboBox txtBaseGreen;
        private System.Windows.Forms.Label labelScaleGreen;
        private System.Windows.Forms.Label labelBaseRed;
        private System.Windows.Forms.NumericUpDown txtScaleGreen;
        private System.Windows.Forms.Label labelGreen;
        private System.Windows.Forms.ComboBox txtBaseRed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdSingleChannel;
        private System.Windows.Forms.RadioButton rdFullChannel;
    }
}