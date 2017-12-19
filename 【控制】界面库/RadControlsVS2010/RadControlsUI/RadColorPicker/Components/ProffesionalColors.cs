using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.RadColorPicker
{   /// <exclude/>
    [ToolboxItem(false), ComVisible(false)]
    public partial class ProfessionalColors : UserControl
    {
        private HslColor colorHsl = HslColor.FromAhsl(255);
        private Color colorRgb = Color.Empty;
        private ColorModes colorMode = ColorModes.Hue;
        private int suppressProfessionalColorsEvents = 0;        
        private bool suppressSpinEditorEvents;

        public ProfessionalColors()
        {
            InitializeComponent();

            colorMode = ColorModes.Hue;

            this.proColors2DBox1.ColorMode = colorMode;
            this.proColorsSlider1.ColorMode = colorMode;

            this.proColors2DBox1.ColorChanged += new ColorChangedEventHandler(proColors2DBox1_ColorChanged);
            this.proColorsSlider1.ColorChanged += new ColorChangedEventHandler(proColorsSlider1_ColorChanged);

            this.proColorsSlider1.Position = this.proColorsSlider1.Height;
        }

        /// <summary>
        /// Gets or sets the color shown in RGB format
        /// </summary>
        public Color ColorRgb
        {
            get
            {
                return colorRgb;
            }
            set
            {
                this.colorRgb = value;
                this.colorHsl = HslColor.FromColor(value);

                UpdateColorComponentsSpinEditors();
                UpdateProfessionalColorControls();
                OnColorChanged();
            }
        }

        /// <summary>
        /// Gets or sets the color shown in HSL format
        /// </summary>
        public HslColor ColorHsl
        {
            get { return colorHsl; }
            set
            {
                if (colorHsl != value)
                {
                    colorHsl = value;
                    colorRgb = colorHsl.RgbValue;

                    UpdateColorComponentsSpinEditors();
                    UpdateProfessionalColorControls();
                    OnColorChanged();
                }
            }
        }

        private void UpdateUIFromRgbControlChange(Color newColor)
        {
            this.colorHsl = HslColor.FromColor(newColor);
            this.colorRgb = newColor;

            UpdateProfessionalColorControls();
            OnColorChanged();
        }

        private void UpdateColorComponentsSpinEditors()
        {
            this.suppressSpinEditorEvents = true;

            this.numAlpha.Value = colorRgb.A;
            this.numRed.Value = colorRgb.R;
            this.numGreen.Value = colorRgb.G;
            this.numBlue.Value = colorRgb.B;


            this.numHue.Value = (int)((Decimal)colorHsl.H * 360);
            this.numSaturation.Value = (int)((Decimal)colorHsl.S * 100);
            this.numLuminance.Value = (int)((Decimal)colorHsl.L * 100);
            
            this.suppressSpinEditorEvents = false;
        }

        private void UpdateProfessionalColorControls()
        {
            suppressProfessionalColorsEvents++;

            //update the values of the slider and the 2D box
            this.proColorsSlider1.ColorHSL = colorHsl;
            this.proColors2DBox1.ColorHSL = colorHsl;

            suppressProfessionalColorsEvents--;
        }

        private void OnColorChanged()
        {
            if (ColorChanged != null)
                ColorChanged(this, new ColorChangedEventArgs(colorHsl));
        }

        private void UpdateUIFromHslControlChange(HslColor newColor)
        {
            this.colorHsl = newColor;
            this.colorRgb = newColor.RgbValue;

            UpdateProfessionalColorControls();
            OnColorChanged();
        }
        
        /// <summary>
        /// Fires when the selected color has changed
        /// </summary>
        public event ColorChangedEventHandler ColorChanged;

        private void proColors2DBox1_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (suppressProfessionalColorsEvents > 0)
            {
                return;
            }

            HslColor newHslColor = this.proColors2DBox1.ColorHSL; //HslColor.FromAhsl((int)numAlpha.Value, this.colorHsl.H, this.proColors2DBox1.ColorHSL.L, this.proColors2DBox1.ColorHSL.S);
            newHslColor.A = (int) numAlpha.Value;

            colorHsl = newHslColor;
            colorRgb = colorHsl.RgbValue;

            if (ColorChanged != null)
                ColorChanged(this, new ColorChangedEventArgs(colorHsl));

            suppressProfessionalColorsEvents++;
            this.proColorsSlider1.ColorHSL = colorHsl;
            suppressProfessionalColorsEvents--;

            this.UpdateColorComponentsSpinEditors();
        }

        private void proColorsSlider1_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (suppressProfessionalColorsEvents > 0)
            {
                return;
            }

            HslColor newHslColor = this.proColorsSlider1.ColorHSL; //HslColor.FromAhsl((int)numAlpha.Value, this.proColorsSlider1.ColorHSL.H, this.colorHsl.S, this.colorHsl.L);
            newHslColor.A = (int)numAlpha.Value;

            colorHsl = newHslColor;
            colorRgb = colorHsl.RgbValue;

            if (ColorChanged != null)
                ColorChanged(this, new ColorChangedEventArgs(colorHsl));

            suppressProfessionalColorsEvents++;
            this.proColors2DBox1.ColorHSL = colorHsl;
            suppressProfessionalColorsEvents--;

            this.UpdateColorComponentsSpinEditors();
        }

        private void colorModeChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                if (sender == radioH) colorMode = ColorModes.Hue;
                else if (sender == radioS) colorMode = ColorModes.Saturation;
                else if (sender == radioL) colorMode = ColorModes.Luminance;
                else if (sender == radioR) colorMode = ColorModes.Red;
                else if (sender == radioG) colorMode = ColorModes.Green;
                else if (sender == radioB) colorMode = ColorModes.Blue;

                this.proColorsSlider1.ColorMode = colorMode;
                this.proColors2DBox1.ColorMode = colorMode;
            }
        }        

        private void numAlpha_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            UpdateUIFromRgbControlChange(Color.FromArgb((int)numAlpha.Value, (int)numRed.Value, (int)numGreen.Value, (int)numBlue.Value));
        }

        private void numRed_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            UpdateUIFromRgbControlChange(Color.FromArgb((int)numAlpha.Value, (int)numRed.Value, (int)numGreen.Value, (int)numBlue.Value));
        }

        private void numGreen_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            UpdateUIFromRgbControlChange(Color.FromArgb((int)numAlpha.Value, (int)numRed.Value, (int)numGreen.Value, (int)numBlue.Value));
        }

        private void numBlue_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            UpdateUIFromRgbControlChange(Color.FromArgb((int)numAlpha.Value, (int)numRed.Value, (int)numGreen.Value, (int)numBlue.Value));
        }

        private void numHue_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            HslColor newHslColor =
                HslColor.FromAhsl(this.colorHsl.A, ((int)this.numHue.Value) / 360f, this.colorHsl.S, this.colorHsl.L);
            UpdateUIFromHslControlChange(newHslColor);
        }

        private void numSaturation_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            HslColor newHslColor =
                HslColor.FromAhsl(this.colorHsl.A, this.colorHsl.H, ((int)this.numSaturation.Value) / 100f, this.colorHsl.L);
            UpdateUIFromHslControlChange(newHslColor);
        }

        private void numLuminance_ValueChanged(object sender, EventArgs e)
        {
            if (suppressSpinEditorEvents)
                return;

            HslColor newHslColor =
                HslColor.FromAhsl(this.colorHsl.A, this.colorHsl.H, this.colorHsl.S, ((int)this.numLuminance.Value) / 100f);
            UpdateUIFromHslControlChange(newHslColor);
        }

        internal void SetColorSilently(HslColor color)
        {
            this.colorHsl = color;
            this.colorRgb = color.RgbValue;
            UpdateProfessionalColorControls();
            UpdateColorComponentsSpinEditors();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.label1.Padding = (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) ? new Padding(0, 0, 15, 0) : new Padding(15, 0, 0, 0);
        }
    }
}