namespace GeoDo.RSS.RasterTools
{
    partial class frmSelectExpression
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
            this.ucBandMath1 = new GeoDo.RSS.RasterTools.UCBandMath();
            this.SuspendLayout();
            // 
            // ucBandMath1
            // 
            this.ucBandMath1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBandMath1.Location = new System.Drawing.Point(0, 0);
            this.ucBandMath1.Name = "ucBandMath1";
            this.ucBandMath1.Size = new System.Drawing.Size(632, 355);
            this.ucBandMath1.TabIndex = 0;
            this.ucBandMath1.ApplyClicked += new System.EventHandler(this.ucBandMath1_ApplyClicked);
            this.ucBandMath1.CancelClicked += new System.EventHandler(this.ucBandMath1_CancelClicked);
            // 
            // frmSelectExpression
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 355);
            this.Controls.Add(this.ucBandMath1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSelectExpression";
            this.ShowInTaskbar = false;
            this.Text = "选择波段运算表达式...";
            this.ResumeLayout(false);

        }

        #endregion

        private UCBandMath ucBandMath1;
    }
}