namespace Telerik.WinControls
{
    partial class GeneralAnimationSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonByStep = new System.Windows.Forms.RadioButton();
            this.radioButtonByStartEnd = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxUnApplyET = new System.Windows.Forms.ComboBox();
            this.comboBoxApplyET = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxInfinitePositions = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownFrames = new System.Windows.Forms.NumericUpDown();
            this.comboBoxAnimationStyle = new System.Windows.Forms.ComboBox();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonByStep);
            this.groupBox2.Controls.Add(this.radioButtonByStartEnd);
            this.groupBox2.Location = new System.Drawing.Point(5, 242);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(195, 64);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation type";
            // 
            // radioButtonByStep
            // 
            this.radioButtonByStep.AutoSize = true;
            this.radioButtonByStep.Location = new System.Drawing.Point(9, 42);
            this.radioButtonByStep.Name = "radioButtonByStep";
            this.radioButtonByStep.Size = new System.Drawing.Size(61, 17);
            this.radioButtonByStep.TabIndex = 13;
            this.radioButtonByStep.TabStop = true;
            this.radioButtonByStep.Text = "by Step";
            this.radioButtonByStep.UseVisualStyleBackColor = true;
            // 
            // radioButtonByStartEnd
            // 
            this.radioButtonByStartEnd.AutoSize = true;
            this.radioButtonByStartEnd.Location = new System.Drawing.Point(9, 19);
            this.radioButtonByStartEnd.Name = "radioButtonByStartEnd";
            this.radioButtonByStartEnd.Size = new System.Drawing.Size(114, 17);
            this.radioButtonByStartEnd.TabIndex = 12;
            this.radioButtonByStartEnd.TabStop = true;
            this.radioButtonByStartEnd.Text = "by Start/End value";
            this.radioButtonByStartEnd.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxUnApplyET);
            this.groupBox1.Controls.Add(this.comboBoxApplyET);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBoxInfinitePositions);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownFrames);
            this.groupBox1.Controls.Add(this.comboBoxAnimationStyle);
            this.groupBox1.Controls.Add(this.numericUpDownInterval);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 233);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Animation Settings";
            // 
            // comboBoxUnApplyET
            // 
            this.comboBoxUnApplyET.FormattingEnabled = true;
            this.comboBoxUnApplyET.Location = new System.Drawing.Point(9, 200);
            this.comboBoxUnApplyET.Name = "comboBoxUnApplyET";
            this.comboBoxUnApplyET.Size = new System.Drawing.Size(147, 21);
            this.comboBoxUnApplyET.TabIndex = 16;
            // 
            // comboBoxApplyET
            // 
            this.comboBoxApplyET.FormattingEnabled = true;
            this.comboBoxApplyET.Location = new System.Drawing.Point(9, 158);
            this.comboBoxApplyET.Name = "comboBoxApplyET";
            this.comboBoxApplyET.Size = new System.Drawing.Size(147, 21);
            this.comboBoxApplyET.TabIndex = 15;
            this.comboBoxApplyET.SelectedIndexChanged += new System.EventHandler(this.comboBoxApplyET_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "ApplyEasingType";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "UnApplyEasingType";
            // 
            // checkBoxInfinitePositions
            // 
            this.checkBoxInfinitePositions.AutoSize = true;
            this.checkBoxInfinitePositions.Location = new System.Drawing.Point(9, 122);
            this.checkBoxInfinitePositions.Name = "checkBoxInfinitePositions";
            this.checkBoxInfinitePositions.Size = new System.Drawing.Size(108, 17);
            this.checkBoxInfinitePositions.TabIndex = 12;
            this.checkBoxInfinitePositions.Text = "Infinite repetitions";
            this.checkBoxInfinitePositions.UseVisualStyleBackColor = true;
            this.checkBoxInfinitePositions.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Animation style:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Frame Rate (fps):";
            // 
            // numericUpDownFrames
            // 
            this.numericUpDownFrames.Location = new System.Drawing.Point(120, 91);
            this.numericUpDownFrames.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFrames.Name = "numericUpDownFrames";
            this.numericUpDownFrames.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownFrames.TabIndex = 9;
            // 
            // comboBoxAnimationStyle
            // 
            this.comboBoxAnimationStyle.FormattingEnabled = true;
            this.comboBoxAnimationStyle.Location = new System.Drawing.Point(9, 32);
            this.comboBoxAnimationStyle.Name = "comboBoxAnimationStyle";
            this.comboBoxAnimationStyle.Size = new System.Drawing.Size(147, 21);
            this.comboBoxAnimationStyle.TabIndex = 11;
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(121, 64);
            this.numericUpDownInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownInterval.TabIndex = 8;
            this.numericUpDownInterval.ValueChanged += new System.EventHandler(this.numericUpDownInterval_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Duration (ms)";
            // 
            // GeneralAnimationSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "GeneralAnimationSettings";
            this.Size = new System.Drawing.Size(203, 317);
            this.Load += new System.EventHandler(this.GeneralAnimationSettings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonByStep;
        private System.Windows.Forms.RadioButton radioButtonByStartEnd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxInfinitePositions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownFrames;
        private System.Windows.Forms.ComboBox comboBoxAnimationStyle;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxUnApplyET;
        private System.Windows.Forms.ComboBox comboBoxApplyET;
        private System.Windows.Forms.Label label5;
    }
}
