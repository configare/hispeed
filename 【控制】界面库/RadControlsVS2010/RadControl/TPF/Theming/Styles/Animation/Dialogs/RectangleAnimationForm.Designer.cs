namespace Telerik.WinControls
{
    partial class RectangleAnimationForm
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
            this.tbReversedStepRectangle = new System.Windows.Forms.TextBox();
            this.tbStepRectangle = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAutomaticReverse = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbEndRectangle = new System.Windows.Forms.TextBox();
            this.tbStartRectangle = new System.Windows.Forms.TextBox();
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
            this.buttonCancel.Location = new System.Drawing.Point(470, 336);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 29;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(374, 336);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 28;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbReversedStepRectangle);
            this.groupBox4.Controls.Add(this.tbStepRectangle);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.checkBoxAutomaticReverse);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(221, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(324, 149);
            this.groupBox4.TabIndex = 27;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Animation by step settings";
            // 
            // tbReversedStepRectangle
            // 
            this.tbReversedStepRectangle.Location = new System.Drawing.Point(104, 84);
            this.tbReversedStepRectangle.Name = "tbReversedStepRectangle";
            this.tbReversedStepRectangle.Size = new System.Drawing.Size(73, 20);
            this.tbReversedStepRectangle.TabIndex = 35;
            this.tbReversedStepRectangle.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // tbStepRectangle
            // 
            this.tbStepRectangle.Location = new System.Drawing.Point(104, 37);
            this.tbStepRectangle.Name = "tbStepRectangle";
            this.tbStepRectangle.Size = new System.Drawing.Size(73, 20);
            this.tbStepRectangle.TabIndex = 34;
            this.tbStepRectangle.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(101, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Rectangle change";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(101, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Rectangle change";
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
            this.groupBox3.Controls.Add(this.tbEndRectangle);
            this.groupBox3.Controls.Add(this.tbStartRectangle);
            this.groupBox3.Controls.Add(this.checkBoxUseCurrentValue);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(221, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 79);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Animation by Start/End value settings";
            // 
            // tbEndRectangle
            // 
            this.tbEndRectangle.Location = new System.Drawing.Point(95, 50);
            this.tbEndRectangle.Name = "tbEndRectangle";
            this.tbEndRectangle.Size = new System.Drawing.Size(82, 20);
            this.tbEndRectangle.TabIndex = 20;
            this.tbEndRectangle.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
            // 
            // tbStartRectangle
            // 
            this.tbStartRectangle.Location = new System.Drawing.Point(95, 18);
            this.tbStartRectangle.Name = "tbStartRectangle";
            this.tbStartRectangle.Size = new System.Drawing.Size(82, 20);
            this.tbStartRectangle.TabIndex = 19;
            this.tbStartRectangle.Validating += new System.ComponentModel.CancelEventHandler(this.tbStartSize_Validating);
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
            this.checkBoxUseCurrentValue.CheckedChanged += new System.EventHandler(this.checkBoxUseCurrentValue_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "End rectangle:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Start rectangle:";
            // 
            // generalAnimationSettings1
            // 
            this.generalAnimationSettings1.AnimationType = Telerik.WinControls.RadAnimationType.ByStep;
            this.generalAnimationSettings1.Location = new System.Drawing.Point(12, 12);
            this.generalAnimationSettings1.Name = "generalAnimationSettings1";
            this.generalAnimationSettings1.Size = new System.Drawing.Size(203, 325);
            this.generalAnimationSettings1.TabIndex = 25;
            // 
            // RectangleAnimationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 371);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.generalAnimationSettings1);
            this.Name = "RectangleAnimationForm";
            this.Text = "Rectangle property animation settings";
            this.Load += new System.EventHandler(this.RectangleAnimationForm_Load);
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
        private System.Windows.Forms.TextBox tbReversedStepRectangle;
        private System.Windows.Forms.TextBox tbStepRectangle;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxAutomaticReverse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbEndRectangle;
        private System.Windows.Forms.TextBox tbStartRectangle;
        private System.Windows.Forms.CheckBox checkBoxUseCurrentValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private GeneralAnimationSettings generalAnimationSettings1;
    }
}