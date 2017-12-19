using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Depricated class, provided for backward compatibility only. Please use HSLColor instead
    /// </summary>
    [Obsolete("Please use Telerik.WinControls.HSLColor class instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class HSLColor
    {
        private HslColor hslColor;
        
        public HSLColor(double h, double s, double l)
        {
            this.hslColor = HslColor.FromAhsl(h, s, l);
        }

        public double H
        {
            get { return this.hslColor.H; }
            set { this.hslColor = HslColor.FromAhsl(value, this.S, this.L); }
        }

        public double S
        {
            get { return this.hslColor.S; }
            set { this.hslColor = HslColor.FromAhsl(this.H, value, this.L); }
        }

        public double L
        {
            get { return this.hslColor.L; }
            set { this.hslColor = HslColor.FromAhsl(this.H, this.S, value); }
        }

        public Color RGB
        {
            get
            {
                return this.hslColor.RgbValue;
            }
            set
            {
                this.hslColor = HslColor.FromColor(value);
            }
        }
    }
}
