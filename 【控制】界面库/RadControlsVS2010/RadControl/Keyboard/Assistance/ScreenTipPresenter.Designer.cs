
namespace Telerik.WinControls
{
	internal partial class ScreenTipPresenter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Dispose
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (this.BackgroundImage != null)
                {
                    this.BackgroundImage.Dispose();
                    this.BackgroundImage = null;
                }
                if (this.Region != null)
                {
                    this.Region.Dispose();
                    this.Region = null;
                }
				if (radScreenTipControl1 != null)
				{
					radScreenTipControl1.Dispose();
                    
				}
                if (this.timer != null)
                {
                    timer.Dispose();
                }
            }
            base.Dispose(disposing);
        } 
        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            radScreenTipControl1 = new RadScreenTipPlaceholder();
            radScreenTipControl1.BeginInit();
            this.SuspendLayout();
			// 
			// ScreenTipPresenter
			// 
            this.Controls.Add(this.radScreenTipControl1);
            this.radScreenTipControl1.Location = new System.Drawing.Point(0, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.AutoSize = false;
            this.TopLevel = true;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Opacity = .99;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(2, 2);
			this.Name = "ScreenTipPresenter";
			this.Text = "ScreenTipForm";
			this.ResumeLayout(false);
            radScreenTipControl1.EndInit();
        }

        #endregion

        private RadScreenTipPlaceholder radScreenTipControl1;
	}
}