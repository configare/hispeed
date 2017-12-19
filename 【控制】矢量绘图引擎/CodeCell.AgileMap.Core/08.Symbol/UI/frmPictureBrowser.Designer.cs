namespace CodeCell.AgileMap.Core
{
    partial class frmPictureBrowser
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtCategories = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucPictureBrowser1 = new UCPictureBrowser();
            this.charLarge = new System.Windows.Forms.Label();
            this.btnFromFile = new System.Windows.Forms.Button();
            this.btnNewCategory = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "符号分类";
            // 
            // txtCategories
            // 
            this.txtCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtCategories.FormattingEnabled = true;
            this.txtCategories.Location = new System.Drawing.Point(71, 12);
            this.txtCategories.Name = "txtCategories";
            this.txtCategories.Size = new System.Drawing.Size(344, 20);
            this.txtCategories.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(286, 232);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(367, 232);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.ucPictureBrowser1);
            this.panel1.Location = new System.Drawing.Point(14, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(428, 189);
            this.panel1.TabIndex = 7;
            // 
            // ucPictureBrowser1
            // 
            this.ucPictureBrowser1.Location = new System.Drawing.Point(0, 0);
            this.ucPictureBrowser1.Name = "ucPictureBrowser1";
            this.ucPictureBrowser1.Size = new System.Drawing.Size(424, 187);
            this.ucPictureBrowser1.TabIndex = 0;
            // 
            // charLarge
            // 
            this.charLarge.AutoSize = true;
            this.charLarge.Location = new System.Drawing.Point(143, 208);
            this.charLarge.Name = "charLarge";
            this.charLarge.Size = new System.Drawing.Size(0, 12);
            this.charLarge.TabIndex = 8;
            // 
            // btnFromFile
            // 
            this.btnFromFile.Location = new System.Drawing.Point(14, 232);
            this.btnFromFile.Name = "btnFromFile";
            this.btnFromFile.Size = new System.Drawing.Size(98, 23);
            this.btnFromFile.TabIndex = 10;
            this.btnFromFile.Text = "来自文件...";
            this.btnFromFile.UseVisualStyleBackColor = true;
            this.btnFromFile.Click += new System.EventHandler(this.btnFromFile_Click);
            // 
            // btnNewCategory
            // 
            this.btnNewCategory.Location = new System.Drawing.Point(416, 10);
            this.btnNewCategory.Name = "btnNewCategory";
            this.btnNewCategory.Size = new System.Drawing.Size(24, 23);
            this.btnNewCategory.TabIndex = 9;
            this.btnNewCategory.UseVisualStyleBackColor = true;
            this.btnNewCategory.Visible = false;
            this.btnNewCategory.Click += new System.EventHandler(this.btnNewCategory_Click);
            // 
            // frmPictureBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 263);
            this.Controls.Add(this.btnFromFile);
            this.Controls.Add(this.btnNewCategory);
            this.Controls.Add(this.charLarge);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtCategories);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPictureBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "图片符号选择窗...";
            this.Load += new System.EventHandler(this.frmTrueTypeFontBrowser_Load_1);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox txtCategories;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label charLarge;
        private System.Windows.Forms.Button btnNewCategory;
        private System.Windows.Forms.Button btnFromFile;
        private UCPictureBrowser ucPictureBrowser1;
    }
}