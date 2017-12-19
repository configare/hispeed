namespace testCN
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
            this.btSeaIceNDSI = new System.Windows.Forms.Button();
            this.btFogBB = new System.Windows.Forms.Button();
            this.btCSR = new System.Windows.Forms.Button();
            this.btOPTD = new System.Windows.Forms.Button();
            this.btImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btSeaIceNDSI
            // 
            this.btSeaIceNDSI.Location = new System.Drawing.Point(74, 28);
            this.btSeaIceNDSI.Name = "btSeaIceNDSI";
            this.btSeaIceNDSI.Size = new System.Drawing.Size(75, 23);
            this.btSeaIceNDSI.TabIndex = 0;
            this.btSeaIceNDSI.Text = "海冰ndsi";
            this.btSeaIceNDSI.UseVisualStyleBackColor = true;
            this.btSeaIceNDSI.Click += new System.EventHandler(this.btSeaIceNDSI_Click);
            // 
            // btFogBB
            // 
            this.btFogBB.Location = new System.Drawing.Point(74, 90);
            this.btFogBB.Name = "btFogBB";
            this.btFogBB.Size = new System.Drawing.Size(75, 23);
            this.btFogBB.TabIndex = 0;
            this.btFogBB.Text = "大雾必备";
            this.btFogBB.UseVisualStyleBackColor = true;
            this.btFogBB.Click += new System.EventHandler(this.btFogBB_Click);
            // 
            // btCSR
            // 
            this.btCSR.Location = new System.Drawing.Point(74, 143);
            this.btCSR.Name = "btCSR";
            this.btCSR.Size = new System.Drawing.Size(75, 23);
            this.btCSR.TabIndex = 0;
            this.btCSR.Text = "大雾CSR";
            this.btCSR.UseVisualStyleBackColor = true;
            this.btCSR.Click += new System.EventHandler(this.btCSR_Click);
            // 
            // btOPTD
            // 
            this.btOPTD.Location = new System.Drawing.Point(74, 196);
            this.btOPTD.Name = "btOPTD";
            this.btOPTD.Size = new System.Drawing.Size(75, 23);
            this.btOPTD.TabIndex = 0;
            this.btOPTD.Text = "大雾OPTD";
            this.btOPTD.UseVisualStyleBackColor = true;
            this.btOPTD.Click += new System.EventHandler(this.btOPTD_Click);
            // 
            // btImage
            // 
            this.btImage.Location = new System.Drawing.Point(197, 90);
            this.btImage.Name = "btImage";
            this.btImage.Size = new System.Drawing.Size(75, 23);
            this.btImage.TabIndex = 0;
            this.btImage.Text = "Image";
            this.btImage.UseVisualStyleBackColor = true;
            this.btImage.Click += new System.EventHandler(this.btImage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btOPTD);
            this.Controls.Add(this.btCSR);
            this.Controls.Add(this.btFogBB);
            this.Controls.Add(this.btImage);
            this.Controls.Add(this.btSeaIceNDSI);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btSeaIceNDSI;
        private System.Windows.Forms.Button btFogBB;
        private System.Windows.Forms.Button btCSR;
        private System.Windows.Forms.Button btOPTD;
        private System.Windows.Forms.Button btImage;
    }
}

