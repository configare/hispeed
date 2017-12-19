namespace Telerik.WinControls.UI
{
	partial class RadarColors
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.adobeColorsSlider1 = new ProfessionalColorsSlider();
			this.colorWheel1 = new ColorWheel();
			this.SuspendLayout();
			// 
			// adobeColorsSlider1
			// 
			this.adobeColorsSlider1.ColorHSL = HslColor.FromAhsl(0,1,1);
			this.adobeColorsSlider1.ColorMode = ColorModes.Red;
			this.adobeColorsSlider1.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.adobeColorsSlider1.Location = new System.Drawing.Point(196, 15);
			this.adobeColorsSlider1.Name = "adobeColorsSlider1";
			this.adobeColorsSlider1.Position = 0;
			this.adobeColorsSlider1.Size = new System.Drawing.Size(42, 172);
			this.adobeColorsSlider1.TabIndex = 3;
			// 
			// colorWheel1
			// 
			this.colorWheel1.Location = new System.Drawing.Point(18, 15);
			this.colorWheel1.Name = "colorWheel1";
			this.colorWheel1.Size = new System.Drawing.Size(172, 172);
			this.colorWheel1.TabIndex = 2;
			// 
			// RadarColors
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.adobeColorsSlider1);
			this.Controls.Add(this.colorWheel1);
			this.Name = "RadarColors";
			this.Size = new System.Drawing.Size(257, 202);
			this.ResumeLayout(false);

		}

		#endregion

		private ProfessionalColorsSlider adobeColorsSlider1;
		private ColorWheel colorWheel1;
	}
}
