namespace Telerik.WinControls
{
    partial class PointFAnimationForm
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
            this.generalAnimationSettings1 = new Telerik.WinControls.GeneralAnimationSettings();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseCurrentValue = new System.Windows.Forms.CheckBox();
            this.numericUpDownEnd1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStart1 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.numericUpDownEnd2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStart2 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart2)).BeginInit();
            this.SuspendLayout();
            // 
            // generalAnimationSettings1
            // 
            this.generalAnimationSettings1.AnimationType = Telerik.WinControls.RadAnimationType.ByStep;
            this.generalAnimationSettings1.Location = new System.Drawing.Point(3, 2);
            this.generalAnimationSettings1.Name = "generalAnimationSettings1";
            this.generalAnimationSettings1.Size = new System.Drawing.Size(203, 325);
            this.generalAnimationSettings1.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDownEnd2);
            this.groupBox3.Controls.Add(this.numericUpDownStart2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.checkBoxUseCurrentValue);
            this.groupBox3.Controls.Add(this.numericUpDownEnd1);
            this.groupBox3.Controls.Add(this.numericUpDownStart1);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(212, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(241, 138);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Animation by Start/End value settings";
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
            // numericUpDownEnd1
            // 
            this.numericUpDownEnd1.Location = new System.Drawing.Point(44, 45);
            this.numericUpDownEnd1.Name = "numericUpDownEnd1";
            this.numericUpDownEnd1.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownEnd1.TabIndex = 17;
            // 
            // numericUpDownStart1
            // 
            this.numericUpDownStart1.Location = new System.Drawing.Point(44, 19);
            this.numericUpDownStart1.Name = "numericUpDownStart1";
            this.numericUpDownStart1.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownStart1.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "StartY:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "StartX:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(378, 222);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 25;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(293, 222);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 24;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // numericUpDownEnd2
            // 
            this.numericUpDownEnd2.Location = new System.Drawing.Point(44, 101);
            this.numericUpDownEnd2.Name = "numericUpDownEnd2";
            this.numericUpDownEnd2.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownEnd2.TabIndex = 22;
            // 
            // numericUpDownStart2
            // 
            this.numericUpDownStart2.Location = new System.Drawing.Point(44, 75);
            this.numericUpDownStart2.Name = "numericUpDownStart2";
            this.numericUpDownStart2.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownStart2.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "StartY:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "StartX:";
            // 
            // PointFAnimationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 247);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.generalAnimationSettings1);
            this.Name = "PointFAnimationForm";
            this.Text = "PointFAnimationForm";
            this.Load += new System.EventHandler(this.PointFAnimationForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GeneralAnimationSettings generalAnimationSettings1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxUseCurrentValue;
        private System.Windows.Forms.NumericUpDown numericUpDownEnd1;
        private System.Windows.Forms.NumericUpDown numericUpDownStart1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.NumericUpDown numericUpDownEnd2;
        private System.Windows.Forms.NumericUpDown numericUpDownStart2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}