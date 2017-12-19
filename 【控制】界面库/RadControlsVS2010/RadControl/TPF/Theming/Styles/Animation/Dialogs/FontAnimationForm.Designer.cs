namespace Telerik.WinControls
{
    partial class FontAnimationForm
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbReversedStepSize = new System.Windows.Forms.TextBox();
            this.tbStepSize = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAutomaticReverse = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbEndSize = new System.Windows.Forms.TextBox();
            this.tbStartSize = new System.Windows.Forms.TextBox();
            this.checkBoxUseCurrentValue = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.generalAnimationSettings1 = new Telerik.WinControls.GeneralAnimationSettings();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(464, 307);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(383, 307);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 23;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbReversedStepSize);
            this.groupBox4.Controls.Add(this.tbStepSize);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.checkBoxAutomaticReverse);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(216, 91);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(324, 150);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Animation by step settings";
            // 
            // tbReversedStepSize
            // 
            this.tbReversedStepSize.Location = new System.Drawing.Point(104, 84);
            this.tbReversedStepSize.Name = "tbReversedStepSize";
            this.tbReversedStepSize.Size = new System.Drawing.Size(60, 20);
            this.tbReversedStepSize.TabIndex = 35;
            this.tbReversedStepSize.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // tbStepSize
            // 
            this.tbStepSize.Location = new System.Drawing.Point(104, 37);
            this.tbStepSize.Name = "tbStepSize";
            this.tbStepSize.Size = new System.Drawing.Size(60, 20);
            this.tbStepSize.TabIndex = 34;
            this.tbStepSize.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(101, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Size change";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(101, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Size change";
            // 
            // checkBoxAutomaticReverse
            // 
            this.checkBoxAutomaticReverse.AutoSize = true;
            this.checkBoxAutomaticReverse.Location = new System.Drawing.Point(13, 116);
            this.checkBoxAutomaticReverse.Name = "checkBoxAutomaticReverse";
            this.checkBoxAutomaticReverse.Size = new System.Drawing.Size(134, 17);
            this.checkBoxAutomaticReverse.TabIndex = 21;
            this.checkBoxAutomaticReverse.Text = "Automatic reverse step";
            this.checkBoxAutomaticReverse.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Reverse step:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Step:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbEndSize);
            this.groupBox3.Controls.Add(this.tbStartSize);
            this.groupBox3.Controls.Add(this.checkBoxUseCurrentValue);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(216, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 79);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Animation by Start/End value settings";
            // 
            // tbEndSize
            // 
            this.tbEndSize.Location = new System.Drawing.Point(95, 50);
            this.tbEndSize.Name = "tbEndSize";
            this.tbEndSize.Size = new System.Drawing.Size(60, 20);
            this.tbEndSize.TabIndex = 20;
            this.tbEndSize.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // tbStartSize
            // 
            this.tbStartSize.Location = new System.Drawing.Point(95, 18);
            this.tbStartSize.Name = "tbStartSize";
            this.tbStartSize.Size = new System.Drawing.Size(60, 20);
            this.tbStartSize.TabIndex = 19;
            this.tbStartSize.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // checkBoxUseCurrentValue
            // 
            this.checkBoxUseCurrentValue.AutoSize = true;
            this.checkBoxUseCurrentValue.Location = new System.Drawing.Point(183, 22);
            this.checkBoxUseCurrentValue.Name = "checkBoxUseCurrentValue";
            this.checkBoxUseCurrentValue.Size = new System.Drawing.Size(113, 17);
            this.checkBoxUseCurrentValue.TabIndex = 18;
            this.checkBoxUseCurrentValue.Text = "Use  current value";
            this.checkBoxUseCurrentValue.UseVisualStyleBackColor = true;
            this.checkBoxUseCurrentValue.Click += new System.EventHandler(this.checkBoxUseCurrentValue_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "End font size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Start font size:";
            // 
            // generalAnimationSettings1
            // 
            this.generalAnimationSettings1.AnimationType = Telerik.WinControls.RadAnimationType.ByStep;
            this.generalAnimationSettings1.Location = new System.Drawing.Point(7, 3);
            this.generalAnimationSettings1.Name = "generalAnimationSettings1";
            this.generalAnimationSettings1.Size = new System.Drawing.Size(203, 327);
            this.generalAnimationSettings1.TabIndex = 20;
            // 
            // FontAnimationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 336);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.generalAnimationSettings1);
            this.Name = "FontAnimationForm";
            this.Text = "Font property animation settings";
            this.Click += new System.EventHandler(this.FontAnimationForm_Load);
            this.Load += new System.EventHandler(this.FontAnimationForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxAutomaticReverse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxUseCurrentValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private GeneralAnimationSettings generalAnimationSettings1;
        private System.Windows.Forms.TextBox tbEndSize;
        private System.Windows.Forms.TextBox tbStartSize;
        private System.Windows.Forms.TextBox tbReversedStepSize;
        private System.Windows.Forms.TextBox tbStepSize;
    }
}