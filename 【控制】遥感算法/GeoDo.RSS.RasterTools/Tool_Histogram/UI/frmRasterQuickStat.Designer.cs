namespace GeoDo.RSS.RasterTools
{
    partial class frmRasterQuickStat
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ucHistogramGraph1 = new GeoDo.RSS.RasterTools.UCHistogramGraph();
            this.ucHistogramValues1 = new GeoDo.RSS.RasterTools.UCHistogramValues();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ucHistogramGraph1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ucHistogramValues1);
            this.splitContainer1.Size = new System.Drawing.Size(595, 484);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 0;
            // 
            // ucHistogramGraph1
            // 
            this.ucHistogramGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucHistogramGraph1.Location = new System.Drawing.Point(0, 0);
            this.ucHistogramGraph1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.ucHistogramGraph1.Name = "ucHistogramGraph1";
            this.ucHistogramGraph1.Size = new System.Drawing.Size(595, 300);
            this.ucHistogramGraph1.TabIndex = 0;
            // 
            // ucHistogramValues1
            // 
            this.ucHistogramValues1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucHistogramValues1.Location = new System.Drawing.Point(0, 0);
            this.ucHistogramValues1.Name = "ucHistogramValues1";
            this.ucHistogramValues1.Size = new System.Drawing.Size(595, 180);
            this.ucHistogramValues1.TabIndex = 0;
            // 
            // frmRasterQuickStat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 484);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmRasterQuickStat";
            this.ShowInTaskbar = false;
            this.Text = "直方图统计...";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private UCHistogramValues ucHistogramValues1;
        private UCHistogramGraph ucHistogramGraph1;
    }
}