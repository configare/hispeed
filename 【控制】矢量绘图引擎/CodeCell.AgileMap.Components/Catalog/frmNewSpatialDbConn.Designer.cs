namespace CodeCell.AgileMap.Components
{
    partial class frmNewSpatialDbConn
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
            this.rdMySQL = new System.Windows.Forms.RadioButton();
            this.rdOracle = new System.Windows.Forms.RadioButton();
            this.rdSQLServer = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdMySQL
            // 
            this.rdMySQL.AutoSize = true;
            this.rdMySQL.Checked = true;
            this.rdMySQL.Location = new System.Drawing.Point(6, 20);
            this.rdMySQL.Name = "rdMySQL";
            this.rdMySQL.Size = new System.Drawing.Size(59, 16);
            this.rdMySQL.TabIndex = 0;
            this.rdMySQL.TabStop = true;
            this.rdMySQL.Text = "My SQL";
            this.rdMySQL.UseVisualStyleBackColor = true;
            // 
            // rdOracle
            // 
            this.rdOracle.AutoSize = true;
            this.rdOracle.Location = new System.Drawing.Point(127, 20);
            this.rdOracle.Name = "rdOracle";
            this.rdOracle.Size = new System.Drawing.Size(107, 16);
            this.rdOracle.TabIndex = 1;
            this.rdOracle.Text = "Oracle Spatial";
            this.rdOracle.UseVisualStyleBackColor = true;
            // 
            // rdSQLServer
            // 
            this.rdSQLServer.AutoSize = true;
            this.rdSQLServer.Enabled = false;
            this.rdSQLServer.Location = new System.Drawing.Point(252, 20);
            this.rdSQLServer.Name = "rdSQLServer";
            this.rdSQLServer.Size = new System.Drawing.Size(101, 16);
            this.rdSQLServer.TabIndex = 2;
            this.rdSQLServer.Text = "MS SQL Server";
            this.rdSQLServer.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdOracle);
            this.groupBox1.Controls.Add(this.rdMySQL);
            this.groupBox1.Controls.Add(this.rdSQLServer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 49);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库类型";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(357, 61);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "连接字符窜";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(344, 21);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Data Source=geo;Persist Security Info=True;User ID=spatial;Password=spatial";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(12, 135);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(106, 23);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "测试连接...";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(294, 135);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(213, 135);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmNewSpatialDbConn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 164);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewSpatialDbConn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新建数据库连接...";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdMySQL;
        private System.Windows.Forms.RadioButton rdOracle;
        private System.Windows.Forms.RadioButton rdSQLServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}