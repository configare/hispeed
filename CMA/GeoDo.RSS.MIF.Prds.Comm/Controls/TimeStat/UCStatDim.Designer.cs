namespace GeoDo.RSS.MIF.Prds.Comm
{
    partial class UCStatDim
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
            this.label1 = new System.Windows.Forms.Label();
            this.ckStatDim = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ckStatCompound = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbDayMosaicType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "统计维度";
            // 
            // ckStatDim
            // 
            this.ckStatDim.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ckStatDim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ckStatDim.FormattingEnabled = true;
            this.ckStatDim.Location = new System.Drawing.Point(71, 7);
            this.ckStatDim.Name = "ckStatDim";
            this.ckStatDim.Size = new System.Drawing.Size(175, 20);
            this.ckStatDim.TabIndex = 1;
            this.ckStatDim.SelectedIndexChanged += new System.EventHandler(this.ckStatDim_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "统计分类";
            // 
            // ckStatCompound
            // 
            this.ckStatCompound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ckStatCompound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ckStatCompound.FormattingEnabled = true;
            this.ckStatCompound.Location = new System.Drawing.Point(71, 33);
            this.ckStatCompound.Name = "ckStatCompound";
            this.ckStatCompound.Size = new System.Drawing.Size(175, 20);
            this.ckStatCompound.TabIndex = 1;
            this.ckStatCompound.SelectedIndexChanged += new System.EventHandler(this.ckStatCompound_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "日合成方式";
            // 
            // cbDayMosaicType
            // 
            this.cbDayMosaicType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDayMosaicType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDayMosaicType.FormattingEnabled = true;
            this.cbDayMosaicType.Location = new System.Drawing.Point(71, 59);
            this.cbDayMosaicType.Name = "cbDayMosaicType";
            this.cbDayMosaicType.Size = new System.Drawing.Size(175, 20);
            this.cbDayMosaicType.TabIndex = 1;
            this.cbDayMosaicType.SelectedIndexChanged += new System.EventHandler(this.cbDayMosaicType_SelectedIndexChanged);
            // 
            // UCStatDim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbDayMosaicType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ckStatCompound);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ckStatDim);
            this.Controls.Add(this.label1);
            this.Name = "UCStatDim";
            this.Size = new System.Drawing.Size(254, 88);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ckStatDim;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ckStatCompound;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbDayMosaicType;
    }
}
