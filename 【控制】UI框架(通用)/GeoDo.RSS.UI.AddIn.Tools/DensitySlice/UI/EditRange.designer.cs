namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class EditRange
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
            this.redTrack = new System.Windows.Forms.TrackBar();
            this.greenTrack = new System.Windows.Forms.TrackBar();
            this.blueTrack = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MinText = new System.Windows.Forms.TextBox();
            this.MaxText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.blueText = new System.Windows.Forms.NumericUpDown();
            this.greenText = new System.Windows.Forms.NumericUpDown();
            this.redText = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.redTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTrack)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redText)).BeginInit();
            this.SuspendLayout();
            // 
            // redTrack
            // 
            this.redTrack.Location = new System.Drawing.Point(51, 50);
            this.redTrack.Maximum = 255;
            this.redTrack.Name = "redTrack";
            this.redTrack.Size = new System.Drawing.Size(131, 45);
            this.redTrack.TabIndex = 2;
            this.redTrack.Scroll += new System.EventHandler(this.redTrack_Scroll);
            // 
            // greenTrack
            // 
            this.greenTrack.Location = new System.Drawing.Point(51, 101);
            this.greenTrack.Maximum = 255;
            this.greenTrack.Name = "greenTrack";
            this.greenTrack.Size = new System.Drawing.Size(131, 45);
            this.greenTrack.TabIndex = 2;
            this.greenTrack.Scroll += new System.EventHandler(this.greenTrack_Scroll);
            // 
            // blueTrack
            // 
            this.blueTrack.Location = new System.Drawing.Point(51, 152);
            this.blueTrack.Maximum = 255;
            this.blueTrack.Name = "blueTrack";
            this.blueTrack.Size = new System.Drawing.Size(131, 45);
            this.blueTrack.TabIndex = 2;
            this.blueTrack.Scroll += new System.EventHandler(this.blueTrack_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Red";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Green";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Blue";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.MinText);
            this.groupBox1.Controls.Add(this.MaxText);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(243, 94);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "最小值:";
            // 
            // MinText
            // 
            this.MinText.Location = new System.Drawing.Point(81, 23);
            this.MinText.Name = "MinText";
            this.MinText.Size = new System.Drawing.Size(100, 21);
            this.MinText.TabIndex = 0;
            // 
            // MaxText
            // 
            this.MaxText.Location = new System.Drawing.Point(81, 61);
            this.MaxText.Name = "MaxText";
            this.MaxText.Size = new System.Drawing.Size(100, 21);
            this.MaxText.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "最大值:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.blueText);
            this.groupBox2.Controls.Add(this.greenText);
            this.groupBox2.Controls.Add(this.redText);
            this.groupBox2.Controls.Add(this.greenTrack);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.blueTrack);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.redTrack);
            this.groupBox2.Location = new System.Drawing.Point(5, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 201);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "颜色设置";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(17, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "Color";
            this.label6.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(19, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(217, 20);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // blueText
            // 
            this.blueText.Location = new System.Drawing.Point(188, 154);
            this.blueText.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueText.Name = "blueText";
            this.blueText.Size = new System.Drawing.Size(50, 21);
            this.blueText.TabIndex = 3;
            this.blueText.ValueChanged += new System.EventHandler(this.blueText_ValueChanged);
            // 
            // greenText
            // 
            this.greenText.Location = new System.Drawing.Point(188, 103);
            this.greenText.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenText.Name = "greenText";
            this.greenText.Size = new System.Drawing.Size(50, 21);
            this.greenText.TabIndex = 3;
            this.greenText.ValueChanged += new System.EventHandler(this.greenText_ValueChanged);
            // 
            // redText
            // 
            this.redText.Location = new System.Drawing.Point(188, 55);
            this.redText.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redText.Name = "redText";
            this.redText.Size = new System.Drawing.Size(50, 21);
            this.redText.TabIndex = 3;
            this.redText.ValueChanged += new System.EventHandler(this.redText_ValueChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(82, 310);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(167, 310);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // EditRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 340);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditRange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "编辑";
            this.Load += new System.EventHandler(this.EditRange_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EditRange_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.redTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTrack)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redText)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar redTrack;
        private System.Windows.Forms.TrackBar greenTrack;
        private System.Windows.Forms.TrackBar blueTrack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox MinText;
        private System.Windows.Forms.TextBox MaxText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown redText;
        private System.Windows.Forms.NumericUpDown blueText;
        private System.Windows.Forms.NumericUpDown greenText;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
    }
}