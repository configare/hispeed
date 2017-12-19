namespace Telerik.WinControls.UI
{
    public partial class ShapedForm
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
			ThemeResolutionService.ApplicationThemeChanged -= new ThemeChangedHandler(ApplicationThemeChanged);

            if (this.Region != null)
                this.Region.Dispose();

            if (this.borderPath != null)
                this.borderPath.Dispose();

            if (this.outerPath != null)
                this.outerPath.Dispose();
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // ShapedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ShapedForm";
            this.Text = "ShapedForm";
            this.ResumeLayout(false);
		}

		#endregion
	}
}

