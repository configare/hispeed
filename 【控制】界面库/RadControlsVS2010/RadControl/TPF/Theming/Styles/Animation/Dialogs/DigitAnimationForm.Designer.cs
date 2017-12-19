namespace Telerik.WinControls
{
    partial class DigitAnimationForm
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseCurrentValue = new System.Windows.Forms.CheckBox();
            this.numericUpDownEnd = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStart = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxAutomaticReverse = new System.Windows.Forms.CheckBox();
            this.numericUpDownReverseStep = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownStep = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.generalAnimationSettings1 = new Telerik.WinControls.GeneralAnimationSettings();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReverseStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(288, 317);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 12;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(379, 317);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxUseCurrentValue);
            this.groupBox3.Controls.Add(this.numericUpDownEnd);
            this.groupBox3.Controls.Add(this.numericUpDownStart);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(213, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(241, 79);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Animation by Start/End value settings";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // checkBoxUseCurrentValue
            // 
            this.checkBoxUseCurrentValue.AutoSize = true;
            this.checkBoxUseCurrentValue.Location = new System.Drawing.Point(117, 20);
            this.checkBoxUseCurrentValue.Name = "checkBoxUseCurrentValue";
            this.checkBoxUseCurrentValue.Size = new System.Drawing.Size(113, 17);
            this.checkBoxUseCurrentValue.TabIndex = 18;
            this.checkBoxUseCurrentValue.Text = "Use  current value";
            this.checkBoxUseCurrentValue.UseVisualStyleBackColor = true;
            this.checkBoxUseCurrentValue.CheckedChanged += new System.EventHandler(this.checkBoxUseCurrentValue_CheckedChanged);
            // 
            // numericUpDownEnd
            // 
            this.numericUpDownEnd.Location = new System.Drawing.Point(44, 45);
            this.numericUpDownEnd.Name = "numericUpDownEnd";
            this.numericUpDownEnd.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownEnd.TabIndex = 17;
            // 
            // numericUpDownStart
            // 
            this.numericUpDownStart.Location = new System.Drawing.Point(44, 19);
            this.numericUpDownStart.Name = "numericUpDownStart";
            this.numericUpDownStart.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownStart.TabIndex = 16;
            this.numericUpDownStart.ValueChanged += new System.EventHandler(this.numericUpDownStart_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "End:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Start:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxAutomaticReverse);
            this.groupBox4.Controls.Add(this.numericUpDownReverseStep);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.numericUpDownStep);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(213, 90);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(241, 147);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Animation by step settings";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // checkBoxAutomaticReverse
            // 
            this.checkBoxAutomaticReverse.AutoSize = true;
            this.checkBoxAutomaticReverse.Location = new System.Drawing.Point(9, 86);
            this.checkBoxAutomaticReverse.Name = "checkBoxAutomaticReverse";
            this.checkBoxAutomaticReverse.Size = new System.Drawing.Size(134, 17);
            this.checkBoxAutomaticReverse.TabIndex = 21;
            this.checkBoxAutomaticReverse.Text = "Automatic reverse step";
            this.checkBoxAutomaticReverse.UseVisualStyleBackColor = true;
            this.checkBoxAutomaticReverse.CheckedChanged += new System.EventHandler(this.checkBoxAutomaticReverse_CheckedChanged);
            // 
            // numericUpDownReverseStep
            // 
            this.numericUpDownReverseStep.Location = new System.Drawing.Point(85, 50);
            this.numericUpDownReverseStep.Name = "numericUpDownReverseStep";
            this.numericUpDownReverseStep.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownReverseStep.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Reverse step:";
            // 
            // numericUpDownStep
            // 
            this.numericUpDownStep.Location = new System.Drawing.Point(85, 24);
            this.numericUpDownStep.Name = "numericUpDownStep";
            this.numericUpDownStep.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownStep.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Step:";
            // 
            // generalAnimationSettings1
            // 
            this.generalAnimationSettings1.AnimationType = Telerik.WinControls.RadAnimationType.ByStep;
            this.generalAnimationSettings1.Location = new System.Drawing.Point(4, 2);
            this.generalAnimationSettings1.Name = "generalAnimationSettings1";
            this.generalAnimationSettings1.Size = new System.Drawing.Size(203, 326);
            this.generalAnimationSettings1.TabIndex = 16;
            this.generalAnimationSettings1.Load += new System.EventHandler(this.generalAnimationSettings1_Load);
            // 
            // DigitAnimationForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(463, 352);
            this.Controls.Add(this.generalAnimationSettings1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DigitAnimationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Numeric property animation settings";
            this.Load += new System.EventHandler(this.IntAnimationForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReverseStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStep)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxUseCurrentValue;
        private System.Windows.Forms.NumericUpDown numericUpDownEnd;
        private System.Windows.Forms.NumericUpDown numericUpDownStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numericUpDownStep;
        private System.Windows.Forms.Label label6;
        private GeneralAnimationSettings generalAnimationSettings1;
        private System.Windows.Forms.CheckBox checkBoxAutomaticReverse;
        private System.Windows.Forms.NumericUpDown numericUpDownReverseStep;
        private System.Windows.Forms.Label label1;
    }
}