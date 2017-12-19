namespace GeoDo.RSS.DITest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMVGXMLParser = new System.Windows.Forms.Button();
            this.btPGS = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMVGXMLParser
            // 
            this.btnMVGXMLParser.Location = new System.Drawing.Point(30, 23);
            this.btnMVGXMLParser.Name = "btnMVGXMLParser";
            this.btnMVGXMLParser.Size = new System.Drawing.Size(75, 23);
            this.btnMVGXMLParser.TabIndex = 0;
            this.btnMVGXMLParser.Text = "MVGXMLParser";
            this.btnMVGXMLParser.UseVisualStyleBackColor = true;
            this.btnMVGXMLParser.Click += new System.EventHandler(this.btnMVGXMLParser_Click);
            // 
            // btPGS
            // 
            this.btPGS.Location = new System.Drawing.Point(111, 23);
            this.btPGS.Name = "btPGS";
            this.btPGS.Size = new System.Drawing.Size(75, 23);
            this.btPGS.TabIndex = 1;
            this.btPGS.Text = "PGSImport";
            this.btPGS.UseVisualStyleBackColor = true;
            this.btPGS.Click += new System.EventHandler(this.btPGS_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(192, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "DATImport";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 83);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btPGS);
            this.Controls.Add(this.btnMVGXMLParser);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMVGXMLParser;
        private System.Windows.Forms.Button btPGS;
        private System.Windows.Forms.Button button1;
    }
}

