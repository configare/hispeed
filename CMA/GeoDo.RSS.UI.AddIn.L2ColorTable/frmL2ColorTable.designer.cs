namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    partial class frmL2ColorTable
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tvColotTables = new System.Windows.Forms.TreeView();
            this.txtTips = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.WebBrowser();
            this.ucColorRampUseBlocks1 = new GeoDo.RSS.UI.AddIn.L2ColorTable.UCColorRampUseBlocks();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(445, 353);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(82, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(621, 353);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(82, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(533, 353);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "填色方案:";
            // 
            // tvColotTables
            // 
            this.tvColotTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tvColotTables.HideSelection = false;
            this.tvColotTables.Location = new System.Drawing.Point(15, 29);
            this.tvColotTables.Name = "tvColotTables";
            this.tvColotTables.Size = new System.Drawing.Size(194, 254);
            this.tvColotTables.TabIndex = 5;
            this.tvColotTables.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvColotTables_MouseClick);
            // 
            // txtTips
            // 
            this.txtTips.AutoSize = true;
            this.txtTips.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTips.ForeColor = System.Drawing.Color.Blue;
            this.txtTips.Location = new System.Drawing.Point(215, 13);
            this.txtTips.Name = "txtTips";
            this.txtTips.Size = new System.Drawing.Size(55, 14);
            this.txtTips.TabIndex = 7;
            this.txtTips.Text = "label2";
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.Location = new System.Drawing.Point(217, 55);
            this.txtContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(485, 228);
            this.txtContent.TabIndex = 8;
            // 
            // ucColorRampUseBlocks1
            // 
            this.ucColorRampUseBlocks1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucColorRampUseBlocks1.Location = new System.Drawing.Point(17, 289);
            this.ucColorRampUseBlocks1.Name = "ucColorRampUseBlocks1";
            this.ucColorRampUseBlocks1.Size = new System.Drawing.Size(685, 48);
            this.ucColorRampUseBlocks1.TabIndex = 10;
            // 
            // frmL2ColorTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 384);
            this.Controls.Add(this.ucColorRampUseBlocks1);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.txtTips);
            this.Controls.Add(this.tvColotTables);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmL2ColorTable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "定量产品分段填色...";
            this.Load += new System.EventHandler(this.frmL2ColorTable_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvColotTables;
        private System.Windows.Forms.Label txtTips;
        private System.Windows.Forms.WebBrowser txtContent;
        private UCColorRampUseBlocks ucColorRampUseBlocks1;
    }
}