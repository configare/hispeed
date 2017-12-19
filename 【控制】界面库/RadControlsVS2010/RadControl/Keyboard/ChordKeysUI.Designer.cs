namespace Telerik.WinControls.Keyboard
{
    partial class ChordKeysUI
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

		private System.Windows.Forms.TextBox chordBox;
        private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Button btnAssign;
		private System.Windows.Forms.Label lblModifiers;
		private System.Windows.Forms.CheckBox chkCtrl;
		private System.Windows.Forms.CheckBox chkShift;
		private System.Windows.Forms.CheckBox chkAlt;
		private System.Windows.Forms.Label lblKeys;

        #region Component Designer generated code

		private void InitializeComponent()
		{
			this.chordBox = new System.Windows.Forms.TextBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.btnAssign = new System.Windows.Forms.Button();
			this.lblModifiers = new System.Windows.Forms.Label();
			this.chkCtrl = new System.Windows.Forms.CheckBox();
			this.chkShift = new System.Windows.Forms.CheckBox();
			this.chkAlt = new System.Windows.Forms.CheckBox();
			this.lblKeys = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// chordBox
			// 
			this.chordBox.Location = new System.Drawing.Point(28, 70);
			this.chordBox.Name = "chordBox";
			this.chordBox.Size = new System.Drawing.Size(178, 20);
			this.chordBox.TabIndex = 0;
			this.chordBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ChordBoxKeyUp);
			this.chordBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ChordBoxKeyPress);
			this.chordBox.TextChanged += new System.EventHandler(this.ChordBoxTextChanged);
			this.chordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChordBoxKeyDown);
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(131, 105);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 1;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.ResetClick);
			// 
			// btnAssign
			// 
			this.btnAssign.Location = new System.Drawing.Point(28, 105);
			this.btnAssign.Name = "btnAssign";
			this.btnAssign.Size = new System.Drawing.Size(75, 23);
			this.btnAssign.TabIndex = 2;
			this.btnAssign.Text = "Assign";
			this.btnAssign.UseVisualStyleBackColor = true;
			this.btnAssign.Click += new System.EventHandler(this.AssignClick);
			// 
			// lblModifiers
			// 
			this.lblModifiers.AutoSize = true;
			this.lblModifiers.Location = new System.Drawing.Point(17, 4);
			this.lblModifiers.Name = "lblModifiers";
			this.lblModifiers.Size = new System.Drawing.Size(52, 13);
			this.lblModifiers.TabIndex = 3;
			this.lblModifiers.Text = "Modifiers:";
			// 
			// chkCtrl
			// 
			this.chkCtrl.AutoSize = true;
			this.chkCtrl.Location = new System.Drawing.Point(28, 20);
			this.chkCtrl.Name = "chkCtrl";
			this.chkCtrl.Size = new System.Drawing.Size(41, 17);
			this.chkCtrl.TabIndex = 4;
			this.chkCtrl.Text = "&Ctrl";
			this.chkCtrl.UseVisualStyleBackColor = true;
			this.chkCtrl.CheckedChanged += new System.EventHandler(this.ModifierChanged);
			// 
			// chkShift
			// 
			this.chkShift.AutoSize = true;
			this.chkShift.Location = new System.Drawing.Point(95, 20);
			this.chkShift.Name = "chkShift";
			this.chkShift.Size = new System.Drawing.Size(47, 17);
			this.chkShift.TabIndex = 5;
			this.chkShift.Text = "&Shift";
			this.chkShift.UseVisualStyleBackColor = true;
			this.chkShift.CheckedChanged += new System.EventHandler(this.ModifierChanged);
			// 
			// chkAlt
			// 
			this.chkAlt.AutoSize = true;
			this.chkAlt.Location = new System.Drawing.Point(168, 20);
			this.chkAlt.Name = "chkAlt";
			this.chkAlt.Size = new System.Drawing.Size(38, 17);
			this.chkAlt.TabIndex = 6;
			this.chkAlt.Text = "&Alt";
			this.chkAlt.UseVisualStyleBackColor = true;
			this.chkAlt.CheckedChanged += new System.EventHandler(this.ModifierChanged);
			// 
			// lblKeys
			// 
			this.lblKeys.AutoSize = true;
			this.lblKeys.Location = new System.Drawing.Point(17, 54);
			this.lblKeys.Name = "lblKeys";
			this.lblKeys.Size = new System.Drawing.Size(33, 13);
			this.lblKeys.TabIndex = 7;
			this.lblKeys.Text = "Keys:";
			// 
			// ChordKeysUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblKeys);
			this.Controls.Add(this.chkAlt);
			this.Controls.Add(this.chkShift);
			this.Controls.Add(this.chkCtrl);
			this.Controls.Add(this.lblModifiers);
			this.Controls.Add(this.btnAssign);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.chordBox);
			this.Name = "ChordKeysUI";
			this.Padding = new System.Windows.Forms.Padding(4);
			this.Size = new System.Drawing.Size(229, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

        #endregion
    }
}
