namespace TestDst
{
    partial class SandDust
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
            this.沙尘能见度 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.沙尘能见度AOI = new System.Windows.Forms.Button();
            this.XML解析测试 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // 沙尘能见度
            // 
            this.沙尘能见度.Location = new System.Drawing.Point(13, 13);
            this.沙尘能见度.Name = "沙尘能见度";
            this.沙尘能见度.Size = new System.Drawing.Size(101, 33);
            this.沙尘能见度.TabIndex = 0;
            this.沙尘能见度.Text = "沙尘能见度";
            this.沙尘能见度.UseVisualStyleBackColor = true;
            this.沙尘能见度.Click += new System.EventHandler(this.沙尘能见度_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // 沙尘能见度AOI
            // 
            this.沙尘能见度AOI.Location = new System.Drawing.Point(120, 13);
            this.沙尘能见度AOI.Name = "沙尘能见度AOI";
            this.沙尘能见度AOI.Size = new System.Drawing.Size(101, 33);
            this.沙尘能见度AOI.TabIndex = 2;
            this.沙尘能见度AOI.Text = "沙尘能见度AOI";
            this.沙尘能见度AOI.UseVisualStyleBackColor = true;
            this.沙尘能见度AOI.Click += new System.EventHandler(this.沙尘能见度AOI_Click);
            // 
            // XML解析测试
            // 
            this.XML解析测试.Location = new System.Drawing.Point(13, 110);
            this.XML解析测试.Name = "XML解析测试";
            this.XML解析测试.Size = new System.Drawing.Size(101, 23);
            this.XML解析测试.TabIndex = 3;
            this.XML解析测试.Text = "XML解析测试";
            this.XML解析测试.UseVisualStyleBackColor = true;
            this.XML解析测试.Click += new System.EventHandler(this.XML解析测试_Click);
            // 
            // SandDust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 334);
            this.Controls.Add(this.XML解析测试);
            this.Controls.Add(this.沙尘能见度AOI);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.沙尘能见度);
            this.Name = "SandDust";
            this.Text = "SandDust";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button 沙尘能见度;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button 沙尘能见度AOI;
        private System.Windows.Forms.Button XML解析测试;
    }
}