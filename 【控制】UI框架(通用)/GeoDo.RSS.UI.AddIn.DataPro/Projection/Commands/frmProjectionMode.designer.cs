namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class frmProjectionMode
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFullFile = new System.Windows.Forms.Button();
            this.btnAOI = new System.Windows.Forms.Button();
            this.btnBlock = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnInput = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(379, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 80);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFullFile
            // 
            this.btnFullFile.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.btnFullFile.Location = new System.Drawing.Point(3, 3);
            this.btnFullFile.Name = "btnFullFile";
            this.btnFullFile.Size = new System.Drawing.Size(80, 80);
            this.btnFullFile.TabIndex = 6;
            this.btnFullFile.Text = "整轨";
            this.btnFullFile.UseVisualStyleBackColor = true;
            this.btnFullFile.Click += new System.EventHandler(this.btnFullFile_Click);
            // 
            // btnAOI
            // 
            this.btnAOI.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.btnAOI.Location = new System.Drawing.Point(97, 3);
            this.btnAOI.Name = "btnAOI";
            this.btnAOI.Size = new System.Drawing.Size(80, 80);
            this.btnAOI.TabIndex = 7;
            this.btnAOI.Text = "AOI";
            this.btnAOI.UseVisualStyleBackColor = true;
            this.btnAOI.Click += new System.EventHandler(this.btnAOI_Click);
            // 
            // btnBlock
            // 
            this.btnBlock.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.btnBlock.Location = new System.Drawing.Point(191, 3);
            this.btnBlock.Name = "btnBlock";
            this.btnBlock.Size = new System.Drawing.Size(80, 80);
            this.btnBlock.TabIndex = 8;
            this.btnBlock.Text = "分幅";
            this.btnBlock.UseVisualStyleBackColor = true;
            this.btnBlock.Click += new System.EventHandler(this.btnBlock_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9997F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9997F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9997F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.99771F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0032F));
            this.tableLayoutPanel1.Controls.Add(this.btnInput, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnBlock, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAOI, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnFullFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 4, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 101);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // btnInput
            // 
            this.btnInput.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.btnInput.Location = new System.Drawing.Point(285, 3);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(80, 80);
            this.btnInput.TabIndex = 10;
            this.btnInput.Text = "交互";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // frmProjectionMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 122);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmProjectionMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择投影模式";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnFullFile;
        private System.Windows.Forms.Button btnAOI;
        private System.Windows.Forms.Button btnBlock;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnInput;
    }
}