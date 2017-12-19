namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class UCConvertAreaRegion
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
            this.lsbAreaRegion = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMinConvertArea = new System.Windows.Forms.TextBox();
            this.txtMaxConvertArea = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lsbAreaRegion
            // 
            this.lsbAreaRegion.FormattingEnabled = true;
            this.lsbAreaRegion.ItemHeight = 12;
            this.lsbAreaRegion.Location = new System.Drawing.Point(3, 32);
            this.lsbAreaRegion.Name = "lsbAreaRegion";
            this.lsbAreaRegion.Size = new System.Drawing.Size(284, 76);
            this.lsbAreaRegion.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "最小覆盖面积";
            // 
            // txtMinConvertArea
            // 
            this.txtMinConvertArea.Location = new System.Drawing.Point(81, 4);
            this.txtMinConvertArea.Name = "txtMinConvertArea";
            this.txtMinConvertArea.Size = new System.Drawing.Size(56, 21);
            this.txtMinConvertArea.TabIndex = 2;
            this.txtMinConvertArea.Text = "0";
            // 
            // txtMaxConvertArea
            // 
            this.txtMaxConvertArea.Location = new System.Drawing.Point(227, 4);
            this.txtMaxConvertArea.Name = "txtMaxConvertArea";
            this.txtMaxConvertArea.Size = new System.Drawing.Size(56, 21);
            this.txtMaxConvertArea.TabIndex = 4;
            this.txtMaxConvertArea.Text = "2500";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "最大覆盖面积";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(2, 114);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(114, 114);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "移除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(215, 114);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // UCConvertAreaRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtMaxConvertArea);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMinConvertArea);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsbAreaRegion);
            this.Name = "UCConvertAreaRegion";
            this.Size = new System.Drawing.Size(290, 155);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lsbAreaRegion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMinConvertArea;
        private System.Windows.Forms.TextBox txtMaxConvertArea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnOK;
    }
}