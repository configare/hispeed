namespace GeoDo.RSS.CA
{
    partial class frmNSMCExpEnhanceArgEditor
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
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.maskComboBox = new System.Windows.Forms.ComboBox();
            this.ck3BandView = new System.Windows.Forms.CheckBox();
            this.labelCoordinates = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 256F));
            this.tableLayoutMain.Location = new System.Drawing.Point(42, 32);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 2;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 256F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutMain.Size = new System.Drawing.Size(276, 276);
            this.tableLayoutMain.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "通道";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maskComboBox
            // 
            this.maskComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maskComboBox.FormattingEnabled = true;
            this.maskComboBox.Items.AddRange(new object[] {
            "Rgb",
            "R",
            "G",
            "B"});
            this.maskComboBox.Location = new System.Drawing.Point(73, 7);
            this.maskComboBox.Name = "maskComboBox";
            this.maskComboBox.Size = new System.Drawing.Size(83, 20);
            this.maskComboBox.TabIndex = 10;
            this.maskComboBox.SelectedIndexChanged += new System.EventHandler(this.maskComboBox_SelectedIndexChanged);
            // 
            // ck3BandView
            // 
            this.ck3BandView.AutoSize = true;
            this.ck3BandView.Location = new System.Drawing.Point(439, 131);
            this.ck3BandView.Name = "ck3BandView";
            this.ck3BandView.Size = new System.Drawing.Size(84, 16);
            this.ck3BandView.TabIndex = 12;
            this.ck3BandView.Text = "分波段预览";
            this.ck3BandView.UseVisualStyleBackColor = true;
            this.ck3BandView.CheckedChanged += new System.EventHandler(this.ck3BandView_CheckedChanged);
            // 
            // labelCoordinates
            // 
            this.labelCoordinates.AutoSize = true;
            this.labelCoordinates.Location = new System.Drawing.Point(71, 311);
            this.labelCoordinates.Name = "labelCoordinates";
            this.labelCoordinates.Size = new System.Drawing.Size(53, 12);
            this.labelCoordinates.TabIndex = 13;
            this.labelCoordinates.Text = "当前坐标";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(439, 153);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 14;
            this.resetButton.Text = "重置";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // frmNSMCExpEnhanceArgEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 345);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.labelCoordinates);
            this.Controls.Add(this.ck3BandView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maskComboBox);
            this.Controls.Add(this.tableLayoutMain);
            this.Name = "frmNSMCExpEnhanceArgEditor";
            this.Text = "frmNSMCExpEnhanceArgEditor";
            this.Controls.SetChildIndex(this.tableLayoutMain, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnApply, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.ckPreviewing, 0);
            this.Controls.SetChildIndex(this.maskComboBox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.ck3BandView, 0);
            this.Controls.SetChildIndex(this.labelCoordinates, 0);
            this.Controls.SetChildIndex(this.resetButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox maskComboBox;
        private System.Windows.Forms.CheckBox ck3BandView;
        private System.Windows.Forms.Label labelCoordinates;
        private System.Windows.Forms.Button resetButton;
    }
}