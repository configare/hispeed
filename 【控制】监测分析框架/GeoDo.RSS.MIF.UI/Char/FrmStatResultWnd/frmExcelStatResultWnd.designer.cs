namespace Geodo.RSS.MIF.UI
{
    partial class frmExcelStatResultWnd
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
            this.winExcelControl1 = new WinExcelControl();
            this.SuspendLayout();
            // 
            // winExcelControl1
            // 
            this.winExcelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winExcelControl1.Location = new System.Drawing.Point(0, 0);
            this.winExcelControl1.Name = "winExcelControl1";
            this.winExcelControl1.Size = new System.Drawing.Size(561, 339);
            this.winExcelControl1.TabIndex = 0;
            // 
            // frmTextStatResultWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 339);
            this.Controls.Add(this.winExcelControl1);
            this.Name = "frmTextStatResultWnd";
            this.Text = "统计结果... ";
            this.ResumeLayout(false);

        }

        #endregion

        private WinExcelControl winExcelControl1;

    }
}