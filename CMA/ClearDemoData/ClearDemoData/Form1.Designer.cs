namespace ClearDemoData
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
            this.btDemo = new System.Windows.Forms.Button();
            this.txtDemo = new System.Windows.Forms.TextBox();
            this.txtPython = new System.Windows.Forms.TextBox();
            this.txtWorkspace = new System.Windows.Forms.TextBox();
            this.btPython = new System.Windows.Forms.Button();
            this.btWorkspace = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btSetting = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btDemo
            // 
            this.btDemo.Location = new System.Drawing.Point(479, 12);
            this.btDemo.Name = "btDemo";
            this.btDemo.Size = new System.Drawing.Size(75, 23);
            this.btDemo.TabIndex = 0;
            this.btDemo.Text = "选择";
            this.btDemo.UseVisualStyleBackColor = true;
            this.btDemo.Click += new System.EventHandler(this.btDemo_Click);
            // 
            // txtDemo
            // 
            this.txtDemo.Location = new System.Drawing.Point(68, 12);
            this.txtDemo.Name = "txtDemo";
            this.txtDemo.Size = new System.Drawing.Size(405, 21);
            this.txtDemo.TabIndex = 1;
            // 
            // txtPython
            // 
            this.txtPython.Location = new System.Drawing.Point(68, 45);
            this.txtPython.Name = "txtPython";
            this.txtPython.Size = new System.Drawing.Size(405, 21);
            this.txtPython.TabIndex = 1;
            // 
            // txtWorkspace
            // 
            this.txtWorkspace.Location = new System.Drawing.Point(68, 78);
            this.txtWorkspace.Name = "txtWorkspace";
            this.txtWorkspace.Size = new System.Drawing.Size(405, 21);
            this.txtWorkspace.TabIndex = 1;
            // 
            // btPython
            // 
            this.btPython.Location = new System.Drawing.Point(479, 45);
            this.btPython.Name = "btPython";
            this.btPython.Size = new System.Drawing.Size(75, 23);
            this.btPython.TabIndex = 0;
            this.btPython.Text = "选择";
            this.btPython.UseVisualStyleBackColor = true;
            this.btPython.Click += new System.EventHandler(this.btPython_Click);
            // 
            // btWorkspace
            // 
            this.btWorkspace.Location = new System.Drawing.Point(479, 78);
            this.btWorkspace.Name = "btWorkspace";
            this.btWorkspace.Size = new System.Drawing.Size(75, 23);
            this.btWorkspace.TabIndex = 0;
            this.btWorkspace.Text = "选择";
            this.btWorkspace.UseVisualStyleBackColor = true;
            this.btWorkspace.Click += new System.EventHandler(this.btWorkspace_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "演示数据";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Python ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "工作空间";
            // 
            // btSetting
            // 
            this.btSetting.Location = new System.Drawing.Point(11, 125);
            this.btSetting.Name = "btSetting";
            this.btSetting.Size = new System.Drawing.Size(75, 23);
            this.btSetting.TabIndex = 0;
            this.btSetting.Text = "保存设置";
            this.btSetting.UseVisualStyleBackColor = true;
            this.btSetting.Click += new System.EventHandler(this.btSetting_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(479, 125);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "清理";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 150);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtWorkspace);
            this.Controls.Add(this.txtPython);
            this.Controls.Add(this.txtDemo);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btSetting);
            this.Controls.Add(this.btWorkspace);
            this.Controls.Add(this.btPython);
            this.Controls.Add(this.btDemo);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "演示数据清理工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btDemo;
        private System.Windows.Forms.TextBox txtDemo;
        private System.Windows.Forms.TextBox txtPython;
        private System.Windows.Forms.TextBox txtWorkspace;
        private System.Windows.Forms.Button btPython;
        private System.Windows.Forms.Button btWorkspace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSetting;
        private System.Windows.Forms.Button btOK;
    }
}

