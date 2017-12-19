namespace test
{
    partial class SandExtract
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
            this.陆地_FY3A = new System.Windows.Forms.Button();
            this.简单判识_海上 = new System.Windows.Forms.Button();
            this.简单判识_云检测 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.陆地_NOAA18 = new System.Windows.Forms.Button();
            this.陆地_MODIS = new System.Windows.Forms.Button();
            this.海陆模板 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // 陆地_FY3A
            // 
            this.陆地_FY3A.Location = new System.Drawing.Point(12, 45);
            this.陆地_FY3A.Name = "陆地_FY3A";
            this.陆地_FY3A.Size = new System.Drawing.Size(106, 23);
            this.陆地_FY3A.TabIndex = 0;
            this.陆地_FY3A.Text = "陆地：FY3A";
            this.陆地_FY3A.UseVisualStyleBackColor = true;
            this.陆地_FY3A.Click += new System.EventHandler(this.简单判识_陆地_Click);
            // 
            // 简单判识_海上
            // 
            this.简单判识_海上.Location = new System.Drawing.Point(12, 116);
            this.简单判识_海上.Name = "简单判识_海上";
            this.简单判识_海上.Size = new System.Drawing.Size(106, 23);
            this.简单判识_海上.TabIndex = 1;
            this.简单判识_海上.Text = "海上:FY3A";
            this.简单判识_海上.UseVisualStyleBackColor = true;
            this.简单判识_海上.Click += new System.EventHandler(this.简单判识_海上_Click);
            // 
            // 简单判识_云检测
            // 
            this.简单判识_云检测.Location = new System.Drawing.Point(12, 185);
            this.简单判识_云检测.Name = "简单判识_云检测";
            this.简单判识_云检测.Size = new System.Drawing.Size(106, 23);
            this.简单判识_云检测.TabIndex = 2;
            this.简单判识_云检测.Text = "云：FY3A";
            this.简单判识_云检测.UseVisualStyleBackColor = true;
            this.简单判识_云检测.Click += new System.EventHandler(this.简单判识_云检测_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "陆地简单判识:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "海上简单判识:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "云检测:";
            // 
            // 陆地_NOAA18
            // 
            this.陆地_NOAA18.Location = new System.Drawing.Point(124, 45);
            this.陆地_NOAA18.Name = "陆地_NOAA18";
            this.陆地_NOAA18.Size = new System.Drawing.Size(106, 23);
            this.陆地_NOAA18.TabIndex = 6;
            this.陆地_NOAA18.Text = "陆地：NOAA18";
            this.陆地_NOAA18.UseVisualStyleBackColor = true;
            this.陆地_NOAA18.Click += new System.EventHandler(this.陆地_NOAA18_Click);
            // 
            // 陆地_MODIS
            // 
            this.陆地_MODIS.Location = new System.Drawing.Point(236, 45);
            this.陆地_MODIS.Name = "陆地_MODIS";
            this.陆地_MODIS.Size = new System.Drawing.Size(106, 23);
            this.陆地_MODIS.TabIndex = 7;
            this.陆地_MODIS.Text = "陆地：MODIS";
            this.陆地_MODIS.UseVisualStyleBackColor = true;
            this.陆地_MODIS.Click += new System.EventHandler(this.陆地_MODIS_Click);
            // 
            // 海陆模板
            // 
            this.海陆模板.Location = new System.Drawing.Point(11, 236);
            this.海陆模板.Name = "海陆模板";
            this.海陆模板.Size = new System.Drawing.Size(106, 23);
            this.海陆模板.TabIndex = 8;
            this.海陆模板.Text = "海陆模板";
            this.海陆模板.UseVisualStyleBackColor = true;
            this.海陆模板.Click += new System.EventHandler(this.海陆模板_Click);
            // 
            // SandExtract
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 432);
            this.Controls.Add(this.海陆模板);
            this.Controls.Add(this.陆地_MODIS);
            this.Controls.Add(this.陆地_NOAA18);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.简单判识_云检测);
            this.Controls.Add(this.简单判识_海上);
            this.Controls.Add(this.陆地_FY3A);
            this.Name = "SandExtract";
            this.Text = "SandExtract";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button 陆地_FY3A;
        private System.Windows.Forms.Button 简单判识_海上;
        private System.Windows.Forms.Button 简单判识_云检测;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button 陆地_NOAA18;
        private System.Windows.Forms.Button 陆地_MODIS;
        private System.Windows.Forms.Button 海陆模板;
    }
}