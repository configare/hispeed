using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represent the ColorChangedEventArgs class
    /// </summary>
    public class ColorChangedEventArgs : EventArgs
    {
        private Color selectedColor;
        private HslColor selectedHslColor;

        /// <summary>
        /// Represents event arguments for the 
        /// %ColorChanged:Telerik.WinControls.CaptureBox.ColorChanged% event.
        /// </summary>
        /// <param name="selectedHslColor">
        /// Represents the changed color. 
        ///</param>
        public ColorChangedEventArgs(HslColor selectedHslColor)
        {
            this.selectedColor = selectedHslColor.RgbValue;
            this.selectedHslColor = selectedHslColor;
        }

        /// <summary>
        /// Represents event arguments for the 
        /// <see cref="Telerik.WinControls.ColorChangedEventHandler"/> event.
        /// </summary>
        /// <param name="selectedColor">
        /// Represents the changed color. 
        ///</param>
        public ColorChangedEventArgs(Color selectedColor)
        {
            this.selectedColor = selectedColor;
            this.selectedHslColor = HslColor.FromColor(selectedColor);
        }

        public Color SelectedColor
        {
            get { return selectedColor; }            
        }

        public HslColor SelectedHslColor
        {
            get { return selectedHslColor; }
        }
    }
}
