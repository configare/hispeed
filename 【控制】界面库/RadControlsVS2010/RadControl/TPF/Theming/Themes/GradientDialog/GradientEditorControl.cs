using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls
{
    [ToolboxItem(false)]
    public partial class GradientEditorControl : UserControl
    {
        internal bool IsLoading;
        private const int offsetValue = 50;

        public GradientEditorControl()
        {
            InitializeComponent();

            this.colorBoxSolid.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxRadialStart.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxSurround1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxSurround2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxSurround3.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGelColor1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGelColor2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGlass1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGlass2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGlass3.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxGlass4.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlass1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlass2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlass3.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlass4.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlassRect1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlassRect2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlassRect3.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxOfficeGlassRect4.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxVista1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxVista2.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxVista3.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.colorBoxVista4.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);
            this.gradientSlider1.ColorChanged += new ColorChangedEventHandler(colorBox_ColorChanged);

            this.gradientBox1.Paint += new PaintEventHandler(gradientBox1_Paint);

            this.Initialize();

        }
        //IFillElement
        public FillPrimitive Fill
        {
            get
            {
                return this.gradientBox1.Fill;
            }
        }

        internal void Initialize()
        {
            IsLoading = true;

            this.colorBoxSolid.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxRadialStart.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxGelColor1.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxGlass1.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxOfficeGlass1.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxOfficeGlassRect1.BackColor = this.gradientBox1.Fill.BackColor;
            this.colorBoxVista1.BackColor = this.gradientBox1.Fill.BackColor;

            this.colorBoxSurround1.BackColor = this.gradientBox1.Fill.BackColor2;
            this.colorBoxGelColor2.BackColor = this.gradientBox1.Fill.BackColor2;
            this.colorBoxGlass2.BackColor = this.gradientBox1.Fill.BackColor2;
            this.colorBoxOfficeGlass2.BackColor = this.gradientBox1.Fill.BackColor2;
            this.colorBoxOfficeGlassRect2.BackColor = this.gradientBox1.Fill.BackColor2;
            this.colorBoxVista2.BackColor = this.gradientBox1.Fill.BackColor2;

            this.colorBoxSurround2.BackColor = this.gradientBox1.Fill.BackColor3;
            this.colorBoxGlass3.BackColor = this.gradientBox1.Fill.BackColor3;
            this.colorBoxOfficeGlass3.BackColor = this.gradientBox1.Fill.BackColor3;
            this.colorBoxOfficeGlassRect3.BackColor = this.gradientBox1.Fill.BackColor3;
            this.colorBoxVista3.BackColor = this.gradientBox1.Fill.BackColor3;

            this.colorBoxSurround3.BackColor = this.gradientBox1.Fill.BackColor4;
            this.colorBoxGlass4.BackColor = this.gradientBox1.Fill.BackColor4;
            this.colorBoxOfficeGlass4.BackColor = this.gradientBox1.Fill.BackColor4;
            this.colorBoxOfficeGlassRect4.BackColor = this.gradientBox1.Fill.BackColor4;
            this.colorBoxVista4.BackColor = this.gradientBox1.Fill.BackColor4;

            this.numericLinearAngle.Value = (Decimal)this.gradientBox1.Fill.GradientAngle;

            this.numericGelPercentage.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericGlassPercentage.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericOfficePercentage1.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericOfficePercentage2.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage2;
            this.numericOfficeRectPercentage1.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericOfficeRectPercentage2.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage2;
            this.numericVistaPercentage1.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericVistaPercentage2.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage2;
            this.numericRadial1.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage;
            this.numericRadial2.Value = (Decimal)this.gradientBox1.Fill.GradientPercentage2;

            this.SetupGradientSlider();

            this.SetSelectedTab();

            this.gradientSlider1.Invalidate();
            this.gradientBox1.Invalidate();

            IsLoading = false;
        }

        private void SetSelectedTab()
        {
            if (this.Fill.BackColor == Color.Transparent &&
                this.Fill.BackColor2 == Color.Transparent &&
                this.Fill.BackColor3 == Color.Transparent &&
                this.Fill.BackColor4 == Color.Transparent
            )
            {
                this.tabControl1.SelectedIndex = 0;
            }
            else
            {
                this.tabControl1.SelectedIndex = (int)this.gradientBox1.Fill.GradientStyle + 1;
            }
        }

        void gradientBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.tabPage9)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawString("No Preview", new Font(FontFamily.GenericSansSerif, 25), Brushes.Gray, new PointF(199, 45));
            }
        }

        private void SetupGradientSlider()
        {
            this.gradientSlider1.Values.Clear();

            this.gradientSlider1.Values.Add(new GradientColorValue(this.gradientBox1.Fill.BackColor, 0f));

            if (this.gradientBox1.Fill.NumberOfColors > 1)
            {
                float percent = this.gradientBox1.Fill.NumberOfColors == 2 ? 1.0f : this.gradientBox1.Fill.GradientPercentage;
                this.gradientSlider1.Values.Add(new GradientColorValue(this.gradientBox1.Fill.BackColor2, percent));
            }

            if (this.gradientBox1.Fill.NumberOfColors > 2)
            {
                float percent = this.gradientBox1.Fill.NumberOfColors == 3 ? 1.0f : this.gradientBox1.Fill.GradientPercentage2;
                this.gradientSlider1.Values.Add(new GradientColorValue(this.gradientBox1.Fill.BackColor3, percent));
            }

            if (this.gradientBox1.Fill.NumberOfColors > 3)
                this.gradientSlider1.Values.Add(new GradientColorValue(this.gradientBox1.Fill.BackColor4, 1f));
        }

        private void colorBox_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (IsLoading)
                return;

            if (sender == this.colorBoxSolid ||
                sender == colorBoxRadialStart ||
                sender == colorBoxGelColor1 ||
                sender == colorBoxGlass1 ||
                sender == colorBoxOfficeGlass1 ||
                sender == colorBoxOfficeGlassRect1 ||
                sender == colorBoxVista1)
            {
                this.gradientBox1.Fill.BackColor = args.SelectedColor;
            }
            else if (sender == colorBoxSurround1 ||
                sender == colorBoxGelColor2 ||
                sender == colorBoxGlass2 ||
                sender == colorBoxOfficeGlass2 ||
                sender == colorBoxOfficeGlassRect2 ||
                sender == colorBoxVista2)
            {
                this.gradientBox1.Fill.BackColor2 = args.SelectedColor;
            }
            else if (sender == colorBoxSurround2 ||
                sender == colorBoxGlass3 ||
                sender == colorBoxOfficeGlass3 ||
                sender == colorBoxOfficeGlassRect3 ||
                sender == colorBoxVista3)
            {
                this.gradientBox1.Fill.BackColor3 = args.SelectedColor;
            }
            else if (sender == colorBoxSurround3 ||
                sender == colorBoxGlass4 ||
                sender == colorBoxOfficeGlass4 ||
                sender == colorBoxOfficeGlassRect4 ||
                sender == colorBoxVista4)
            {
                this.gradientBox1.Fill.BackColor4 = args.SelectedColor;
            }
            else if (sender == gradientSlider1)
            {
                this.gradientBox1.Fill.BackColor = this.gradientSlider1.Values[0].ColorValue;

                if (this.gradientSlider1.Values.Count > 1)
                {
                    this.gradientBox1.Fill.BackColor2 = this.gradientSlider1.Values[1].ColorValue;
                    this.gradientBox1.Fill.GradientPercentage = this.gradientSlider1.Values[1].ColorPosition;
                }

                if (this.gradientSlider1.Values.Count > 2)
                {
                    this.gradientBox1.Fill.BackColor3 = this.gradientSlider1.Values[2].ColorValue;
                    this.gradientBox1.Fill.GradientPercentage2 = this.gradientSlider1.Values[2].ColorPosition;
                }

                if (this.gradientSlider1.Values.Count > 3)
                {
                    this.gradientBox1.Fill.BackColor4 = this.gradientSlider1.Values[3].ColorValue;
                }

                this.gradientBox1.Fill.NumberOfColors = this.gradientSlider1.Values.Count;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (this.tabControl1.SelectedIndex)
            {
                case 1:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Solid;
                    this.gradientBox1.Fill.BackColor = this.colorBoxSolid.BackColor;
                    break;

                case 2:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Linear;
                    this.gradientBox1.Fill.BackColor = this.gradientSlider1.Values[0].ColorValue;

                    if (this.gradientSlider1.Values.Count == 2)
                    {
                        this.gradientBox1.Fill.BackColor2 = this.gradientSlider1.Values[1].ColorValue;

                    }
                    else if (this.gradientSlider1.Values.Count == 3)
                    {

                        this.gradientBox1.Fill.BackColor2 = this.gradientSlider1.Values[1].ColorValue;
                        this.gradientBox1.Fill.GradientPercentage = this.gradientSlider1.Values[1].ColorPosition;
                        this.gradientBox1.Fill.BackColor3 = this.gradientSlider1.Values[2].ColorValue;
                    }
                    else if (this.gradientSlider1.Values.Count == 4)
                    {
                        this.gradientBox1.Fill.BackColor2 = this.gradientSlider1.Values[1].ColorValue;
                        this.gradientBox1.Fill.GradientPercentage = this.gradientSlider1.Values[1].ColorPosition;
                        this.gradientBox1.Fill.BackColor3 = this.gradientSlider1.Values[2].ColorValue;
                        this.gradientBox1.Fill.GradientPercentage2 = this.gradientSlider1.Values[2].ColorPosition;
                        this.gradientBox1.Fill.BackColor4 = this.gradientSlider1.Values[3].ColorValue;
                    }

                    break;

                case 3:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Radial;
                    this.gradientBox1.Fill.BackColor = this.colorBoxRadialStart.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxSurround1.BackColor;
                    this.gradientBox1.Fill.BackColor3 = this.colorBoxSurround2.BackColor;
                    this.gradientBox1.Fill.BackColor4 = this.colorBoxSurround3.BackColor;
                    this.gradientBox1.Fill.NumberOfColors = 4;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericRadial1.Value;
                    this.gradientBox1.Fill.GradientPercentage2 = (float)this.numericRadial2.Value;

                    break;

                case 7:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Gel;
                    this.gradientBox1.Fill.BackColor = this.colorBoxGelColor1.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxGelColor2.BackColor;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericGelPercentage.Value;
                    break;

                case 4:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Glass;
                    this.gradientBox1.Fill.BackColor = this.colorBoxGlass1.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxGlass2.BackColor;
                    this.gradientBox1.Fill.BackColor3 = this.colorBoxGlass3.BackColor;
                    this.gradientBox1.Fill.BackColor4 = this.colorBoxGlass4.BackColor;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericGlassPercentage.Value;
                    break;

                case 5:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.OfficeGlass;
                    this.gradientBox1.Fill.BackColor = this.colorBoxOfficeGlass1.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxOfficeGlass2.BackColor;
                    this.gradientBox1.Fill.BackColor3 = this.colorBoxOfficeGlass3.BackColor;
                    this.gradientBox1.Fill.BackColor4 = this.colorBoxOfficeGlass4.BackColor;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericOfficePercentage1.Value;
                    this.gradientBox1.Fill.GradientPercentage2 = (float)this.numericOfficePercentage2.Value;

                    break;

                case 6:
                    this.gradientBox1.Fill.GradientStyle = GradientStyles.OfficeGlassRect;
                    this.gradientBox1.Fill.BackColor = this.colorBoxOfficeGlassRect1.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxOfficeGlassRect2.BackColor;
                    this.gradientBox1.Fill.BackColor3 = this.colorBoxOfficeGlassRect3.BackColor;
                    this.gradientBox1.Fill.BackColor4 = this.colorBoxOfficeGlassRect4.BackColor;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericOfficeRectPercentage1.Value;
                    this.gradientBox1.Fill.GradientPercentage2 = (float)this.numericOfficeRectPercentage2.Value;

                    break;

                case 8:

                    this.gradientBox1.Fill.GradientStyle = GradientStyles.Vista;
                    this.gradientBox1.Fill.BackColor = this.colorBoxVista1.BackColor;
                    this.gradientBox1.Fill.BackColor2 = this.colorBoxVista2.BackColor;
                    this.gradientBox1.Fill.BackColor3 = this.colorBoxVista3.BackColor;
                    this.gradientBox1.Fill.BackColor4 = this.colorBoxVista4.BackColor;
                    this.gradientBox1.Fill.GradientPercentage = (float)this.numericVistaPercentage1.Value;
                    this.gradientBox1.Fill.GradientPercentage2 = (float)this.numericVistaPercentage2.Value;

                    break;

                case 0:
                    this.gradientBox1.Fill.BackColor = Color.Transparent;
                    this.gradientBox1.Fill.BackColor2 = Color.Transparent;
                    this.gradientBox1.Fill.BackColor3 = Color.Transparent;
                    this.gradientBox1.Fill.BackColor4 = Color.Transparent;
                    break;
            }


            this.gradientSlider1.Invalidate();
        }

        private void gradientAngle_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                this.gradientBox1.Fill.GradientAngle = (float)this.numericLinearAngle.Value;
                this.tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
                this.gradientAngleControl1.GradientAngle = (float)this.numericLinearAngle.Value;
            }
        }

        private void gradientPercentage_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                this.gradientBox1.Fill.GradientPercentage = (float)((NumericUpDown)numericGelPercentage).Value;
                this.tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void gradientPercentage2_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                this.gradientBox1.Fill.GradientPercentage2 = (float)((NumericUpDown)numericGelPercentage).Value;
                this.tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.Red;
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);

                        break;
                    }
                case 1:
                    {

                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(0, 255, 0);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R, colorBoxOfficeGlass4.BackColor.G - offsetValue, colorBoxOfficeGlass4.BackColor.B);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R, colorBoxOfficeGlass2.BackColor.G - offsetValue, colorBoxOfficeGlass2.BackColor.B);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R, colorBoxOfficeGlass3.BackColor.G - offsetValue, colorBoxOfficeGlass3.BackColor.B);
                        break;


                    }
                case 2:
                    {

                        colorBoxOfficeGlass4.BackColor = Color.Blue;
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R, colorBoxOfficeGlass4.BackColor.G, colorBoxOfficeGlass4.BackColor.B - offsetValue);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R, colorBoxOfficeGlass2.BackColor.G, colorBoxOfficeGlass2.BackColor.B - offsetValue);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R, colorBoxOfficeGlass3.BackColor.G, colorBoxOfficeGlass3.BackColor.B - offsetValue);
                        break;

                    }
                case 3:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(255, 255, 0);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R - offsetValue, colorBoxOfficeGlass4.BackColor.G - offsetValue, colorBoxOfficeGlass4.BackColor.B);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R - offsetValue, colorBoxOfficeGlass2.BackColor.G - offsetValue, colorBoxOfficeGlass2.BackColor.B);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R - offsetValue, colorBoxOfficeGlass3.BackColor.G - offsetValue, colorBoxOfficeGlass3.BackColor.B);

                        break;
                    }
                case 4:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(0, 0, 0);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R + offsetValue, colorBoxOfficeGlass4.BackColor.G + offsetValue, colorBoxOfficeGlass4.BackColor.B + offsetValue);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R + offsetValue, colorBoxOfficeGlass2.BackColor.G + offsetValue, colorBoxOfficeGlass2.BackColor.B + offsetValue);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R + offsetValue, colorBoxOfficeGlass3.BackColor.G + offsetValue, colorBoxOfficeGlass3.BackColor.B + offsetValue);

                        break;
                    }
                case 5:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(255, 255, 255);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R - offsetValue, colorBoxOfficeGlass4.BackColor.G - offsetValue, colorBoxOfficeGlass4.BackColor.B - offsetValue);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R - offsetValue, colorBoxOfficeGlass2.BackColor.G - offsetValue, colorBoxOfficeGlass2.BackColor.B - offsetValue);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R - offsetValue, colorBoxOfficeGlass3.BackColor.G - offsetValue, colorBoxOfficeGlass3.BackColor.B - offsetValue);

                        break;
                    }
                case 6:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(255, 128, 0);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R - offsetValue, colorBoxOfficeGlass4.BackColor.G, colorBoxOfficeGlass4.BackColor.B);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R - offsetValue, colorBoxOfficeGlass2.BackColor.G, colorBoxOfficeGlass2.BackColor.B);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R - offsetValue, colorBoxOfficeGlass3.BackColor.G - offsetValue, colorBoxOfficeGlass3.BackColor.B);
                        break;
                    }
                case 7:
                    {
                        colorBoxOfficeGlass4.BackColor = Color.FromArgb(128, 0, 128);
                        colorBoxOfficeGlass2.BackColor = Color.FromArgb(colorBoxOfficeGlass4.BackColor.R + offsetValue - 30, colorBoxOfficeGlass4.BackColor.G, colorBoxOfficeGlass4.BackColor.B + offsetValue - 30);
                        colorBoxOfficeGlass3.BackColor = Color.FromArgb(colorBoxOfficeGlass2.BackColor.R + offsetValue - 30, colorBoxOfficeGlass2.BackColor.G, colorBoxOfficeGlass2.BackColor.B + offsetValue - 30);
                        colorBoxOfficeGlass1.BackColor = Color.FromArgb(colorBoxOfficeGlass3.BackColor.R + offsetValue - 30, colorBoxOfficeGlass3.BackColor.G, colorBoxOfficeGlass3.BackColor.B + offsetValue - 30);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox8.SelectedIndex)
            {
                case 0:
                    colorBoxOfficeGlassRect4.BackColor = Color.Red;
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                    break;

                case 1:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(0, 255, 0);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R, colorBoxOfficeGlassRect4.BackColor.G - offsetValue, colorBoxOfficeGlassRect4.BackColor.B);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R, colorBoxOfficeGlassRect2.BackColor.G - offsetValue, colorBoxOfficeGlassRect2.BackColor.B);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R, colorBoxOfficeGlassRect3.BackColor.G - offsetValue, colorBoxOfficeGlassRect3.BackColor.B);
                    break;

                case 2:
                    colorBoxOfficeGlassRect4.BackColor = Color.Blue;
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R, colorBoxOfficeGlassRect4.BackColor.G, colorBoxOfficeGlassRect4.BackColor.B - offsetValue);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R, colorBoxOfficeGlassRect2.BackColor.G, colorBoxOfficeGlassRect2.BackColor.B - offsetValue);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R, colorBoxOfficeGlassRect3.BackColor.G, colorBoxOfficeGlassRect3.BackColor.B - offsetValue);
                    break;

                case 3:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(255, 255, 0);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R - offsetValue, colorBoxOfficeGlassRect4.BackColor.G - offsetValue, colorBoxOfficeGlassRect4.BackColor.B);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R - offsetValue, colorBoxOfficeGlassRect2.BackColor.G - offsetValue, colorBoxOfficeGlassRect2.BackColor.B);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R - offsetValue, colorBoxOfficeGlassRect3.BackColor.G - offsetValue, colorBoxOfficeGlassRect3.BackColor.B);
                    break;

                case 4:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(0, 0, 0);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R + offsetValue, colorBoxOfficeGlassRect4.BackColor.G + offsetValue, colorBoxOfficeGlassRect4.BackColor.B + offsetValue);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R + offsetValue, colorBoxOfficeGlassRect2.BackColor.G + offsetValue, colorBoxOfficeGlassRect2.BackColor.B + offsetValue);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R + offsetValue, colorBoxOfficeGlassRect3.BackColor.G + offsetValue, colorBoxOfficeGlassRect3.BackColor.B + offsetValue);
                    break;

                case 5:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(255, 255, 255);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R - offsetValue, colorBoxOfficeGlassRect4.BackColor.G - offsetValue, colorBoxOfficeGlassRect4.BackColor.B - offsetValue);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R - offsetValue, colorBoxOfficeGlassRect2.BackColor.G - offsetValue, colorBoxOfficeGlassRect2.BackColor.B - offsetValue);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R - offsetValue, colorBoxOfficeGlassRect3.BackColor.G - offsetValue, colorBoxOfficeGlassRect3.BackColor.B - offsetValue);
                    break;

                case 6:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(255, 128, 0);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R - offsetValue, colorBoxOfficeGlassRect4.BackColor.G, colorBoxOfficeGlassRect4.BackColor.B);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R - offsetValue, colorBoxOfficeGlassRect2.BackColor.G, colorBoxOfficeGlassRect2.BackColor.B);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R - offsetValue, colorBoxOfficeGlassRect3.BackColor.G - offsetValue, colorBoxOfficeGlassRect3.BackColor.B);
                    break;

                case 7:
                    colorBoxOfficeGlassRect4.BackColor = Color.FromArgb(128, 0, 128);
                    colorBoxOfficeGlassRect2.BackColor = Color.FromArgb(colorBoxOfficeGlassRect4.BackColor.R + offsetValue - 30, colorBoxOfficeGlassRect4.BackColor.G, colorBoxOfficeGlassRect4.BackColor.B + offsetValue - 30);
                    colorBoxOfficeGlassRect3.BackColor = Color.FromArgb(colorBoxOfficeGlassRect2.BackColor.R + offsetValue - 30, colorBoxOfficeGlassRect2.BackColor.G, colorBoxOfficeGlassRect2.BackColor.B + offsetValue - 30);
                    colorBoxOfficeGlassRect1.BackColor = Color.FromArgb(colorBoxOfficeGlassRect3.BackColor.R + offsetValue - 30, colorBoxOfficeGlassRect3.BackColor.G, colorBoxOfficeGlassRect3.BackColor.B + offsetValue - 30);
                    break;
            }

            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        this.colorBoxGelColor1.BackColor = Color.Red;
                        this.colorBoxGelColor2.BackColor =
                          Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);

                        break;
                    }
                case 1:
                    {

                        colorBoxGelColor1.BackColor = Color.FromArgb(0, 255, 0);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R, colorBoxGelColor1.BackColor.G - offsetValue, colorBoxGelColor1.BackColor.B);
                        break;
                    }
                case 2:
                    {

                        colorBoxGelColor1.BackColor = Color.Blue;
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R, colorBoxGelColor1.BackColor.G, colorBoxGelColor1.BackColor.B - offsetValue);
                        break;

                    }
                case 3:
                    {
                        colorBoxGelColor1.BackColor = Color.FromArgb(255, 255, 0);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R - offsetValue, colorBoxGelColor1.BackColor.G - offsetValue, colorBoxGelColor1.BackColor.B);

                        break;
                    }
                case 4:
                    {
                        colorBoxGelColor1.BackColor = Color.FromArgb(0, 0, 0);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R + offsetValue, colorBoxGelColor1.BackColor.G + offsetValue, colorBoxGelColor1.BackColor.B + offsetValue);

                        break;
                    }
                case 5:
                    {
                        colorBoxGelColor1.BackColor = Color.FromArgb(255, 255, 255);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R - offsetValue, colorBoxGelColor1.BackColor.G - offsetValue, colorBoxGelColor1.BackColor.B - offsetValue);

                        break;
                    }
                case 6:
                    {
                        colorBoxGelColor1.BackColor = Color.FromArgb(255, 128, 0);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R - offsetValue, colorBoxGelColor1.BackColor.G, colorBoxGelColor1.BackColor.B);
                        break;
                    }
                case 7:
                    {
                        colorBoxGelColor1.BackColor = Color.FromArgb(128, 0, 128);
                        colorBoxGelColor2.BackColor = Color.FromArgb(colorBoxGelColor1.BackColor.R + offsetValue - 30, colorBoxGelColor1.BackColor.G, colorBoxGelColor1.BackColor.B + offsetValue - 30);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    {
                        colorBoxVista4.BackColor = Color.Red;
                        colorBoxVista2.BackColor = Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                        break;
                    }
                case 1:
                    {

                        colorBoxVista4.BackColor = Color.FromArgb(0, 255, 0);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R, colorBoxVista4.BackColor.G - offsetValue, colorBoxVista4.BackColor.B);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R, colorBoxVista2.BackColor.G - offsetValue, colorBoxVista2.BackColor.B);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R, colorBoxVista3.BackColor.G - offsetValue, colorBoxVista3.BackColor.B);
                        break;


                    }
                case 2:
                    {

                        colorBoxVista4.BackColor = Color.Blue;
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R, colorBoxVista4.BackColor.G, colorBoxVista4.BackColor.B - offsetValue);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R, colorBoxVista2.BackColor.G, colorBoxVista2.BackColor.B - offsetValue);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R, colorBoxVista3.BackColor.G, colorBoxVista3.BackColor.B - offsetValue);
                        break;

                    }
                case 3:
                    {
                        colorBoxVista4.BackColor = Color.FromArgb(255, 255, 0);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R - offsetValue, colorBoxVista4.BackColor.G - offsetValue, colorBoxVista4.BackColor.B);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R - offsetValue, colorBoxVista2.BackColor.G - offsetValue, colorBoxVista2.BackColor.B);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R - offsetValue, colorBoxVista3.BackColor.G - offsetValue, colorBoxVista3.BackColor.B);

                        break;
                    }
                case 4:
                    {
                        colorBoxVista4.BackColor = Color.FromArgb(0, 0, 0);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R + offsetValue, colorBoxVista4.BackColor.G + offsetValue, colorBoxVista4.BackColor.B + offsetValue);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R + offsetValue, colorBoxVista2.BackColor.G + offsetValue, colorBoxVista2.BackColor.B + offsetValue);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R + offsetValue, colorBoxVista3.BackColor.G + offsetValue, colorBoxVista3.BackColor.B + offsetValue);

                        break;
                    }
                case 5:
                    {
                        colorBoxVista4.BackColor = Color.FromArgb(255, 255, 255);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R - offsetValue, colorBoxVista4.BackColor.G - offsetValue, colorBoxVista4.BackColor.B - offsetValue);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R - offsetValue, colorBoxVista2.BackColor.G - offsetValue, colorBoxVista2.BackColor.B - offsetValue);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R - offsetValue, colorBoxVista3.BackColor.G - offsetValue, colorBoxVista3.BackColor.B - offsetValue);

                        break;
                    }
                case 6:
                    {
                        colorBoxVista4.BackColor = Color.FromArgb(255, 128, 0);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R - offsetValue, colorBoxVista4.BackColor.G, colorBoxVista4.BackColor.B);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R - offsetValue, colorBoxVista2.BackColor.G, colorBoxVista2.BackColor.B);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R - offsetValue, colorBoxVista3.BackColor.G - offsetValue, colorBoxVista3.BackColor.B);
                        break;
                    }
                case 7:
                    {
                        colorBoxVista4.BackColor = Color.FromArgb(128, 0, 128);
                        colorBoxVista2.BackColor = Color.FromArgb(colorBoxVista4.BackColor.R + offsetValue - 30, colorBoxVista4.BackColor.G, colorBoxVista4.BackColor.B + offsetValue - 30);
                        colorBoxVista3.BackColor = Color.FromArgb(colorBoxVista2.BackColor.R + offsetValue - 30, colorBoxVista2.BackColor.G, colorBoxVista2.BackColor.B + offsetValue - 30);
                        colorBoxVista1.BackColor = Color.FromArgb(colorBoxVista3.BackColor.R + offsetValue - 30, colorBoxVista3.BackColor.G, colorBoxVista3.BackColor.B + offsetValue - 30);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);

        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox7.SelectedIndex)
            {
                case 0:
                    {
                        this.colorBoxSolid.BackColor = Color.Red;

                        break;
                    }
                case 1:
                    {

                        colorBoxSolid.BackColor = Color.FromArgb(0, 255, 0);
                        break;


                    }
                case 2:
                    {

                        colorBoxSolid.BackColor = Color.Blue;
                        break;

                    }
                case 3:
                    {
                        colorBoxSolid.BackColor = Color.FromArgb(255, 255, 0);

                        break;
                    }
                case 4:
                    {
                        colorBoxSolid.BackColor = Color.FromArgb(0, 0, 0);

                        break;
                    }
                case 5:
                    {
                        colorBoxSolid.BackColor = Color.FromArgb(255, 255, 255);

                        break;
                    }
                case 6:
                    {
                        colorBoxSolid.BackColor = Color.FromArgb(255, 128, 0);
                        break;
                    }
                case 7:
                    {
                        colorBoxSolid.BackColor = Color.FromArgb(128, 0, 128);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox5.SelectedIndex)
            {
                case 0:
                    {
                        this.colorBoxRadialStart.BackColor = Color.Red;
                        colorBoxSurround1.BackColor = Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);

                        break;
                    }
                case 1:
                    {

                        colorBoxRadialStart.BackColor = Color.FromArgb(0, 255, 0);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R, colorBoxRadialStart.BackColor.G - offsetValue, colorBoxRadialStart.BackColor.B);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R, colorBoxSurround1.BackColor.G - offsetValue, colorBoxSurround1.BackColor.B);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R, colorBoxSurround2.BackColor.G - offsetValue, colorBoxSurround2.BackColor.B);
                        break;


                    }
                case 2:
                    {

                        colorBoxRadialStart.BackColor = Color.Blue;
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R, colorBoxRadialStart.BackColor.G, colorBoxRadialStart.BackColor.B - offsetValue);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R, colorBoxSurround1.BackColor.G, colorBoxSurround1.BackColor.B - offsetValue);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R, colorBoxSurround2.BackColor.G, colorBoxSurround2.BackColor.B - offsetValue);
                        break;

                    }
                case 3:
                    {
                        colorBoxRadialStart.BackColor = Color.FromArgb(255, 255, 0);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R - offsetValue, colorBoxRadialStart.BackColor.G - offsetValue, colorBoxRadialStart.BackColor.B);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R - offsetValue, colorBoxSurround1.BackColor.G - offsetValue, colorBoxSurround1.BackColor.B);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R - offsetValue, colorBoxSurround2.BackColor.G - offsetValue, colorBoxSurround2.BackColor.B);

                        break;
                    }
                case 4:
                    {
                        colorBoxRadialStart.BackColor = Color.FromArgb(0, 0, 0);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R + offsetValue, colorBoxRadialStart.BackColor.G + offsetValue, colorBoxRadialStart.BackColor.B + offsetValue);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R + offsetValue, colorBoxSurround1.BackColor.G + offsetValue, colorBoxSurround1.BackColor.B + offsetValue);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R + offsetValue, colorBoxSurround2.BackColor.G + offsetValue, colorBoxSurround2.BackColor.B + offsetValue);

                        break;
                    }
                case 5:
                    {
                        colorBoxRadialStart.BackColor = Color.FromArgb(255, 255, 255);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R - offsetValue, colorBoxRadialStart.BackColor.G - offsetValue, colorBoxRadialStart.BackColor.B - offsetValue);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R - offsetValue, colorBoxSurround1.BackColor.G - offsetValue, colorBoxSurround1.BackColor.B - offsetValue);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R - offsetValue, colorBoxSurround2.BackColor.G - offsetValue, colorBoxSurround2.BackColor.B - offsetValue);

                        break;
                    }
                case 6:
                    {
                        colorBoxRadialStart.BackColor = Color.FromArgb(255, 128, 0);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R - offsetValue, colorBoxRadialStart.BackColor.G, colorBoxRadialStart.BackColor.B);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R - offsetValue, colorBoxSurround1.BackColor.G, colorBoxSurround1.BackColor.B);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R - offsetValue, colorBoxSurround2.BackColor.G - offsetValue, colorBoxSurround2.BackColor.B);
                        break;
                    }
                case 7:
                    {
                        colorBoxRadialStart.BackColor = Color.FromArgb(128, 0, 128);
                        colorBoxSurround1.BackColor = Color.FromArgb(colorBoxRadialStart.BackColor.R + offsetValue - 30, colorBoxRadialStart.BackColor.G, colorBoxRadialStart.BackColor.B + offsetValue - 30);
                        colorBoxSurround2.BackColor = Color.FromArgb(colorBoxSurround1.BackColor.R + offsetValue - 30, colorBoxSurround1.BackColor.G, colorBoxSurround1.BackColor.B + offsetValue - 30);
                        colorBoxSurround3.BackColor = Color.FromArgb(colorBoxSurround2.BackColor.R + offsetValue - 30, colorBoxSurround2.BackColor.G, colorBoxSurround2.BackColor.B + offsetValue - 30);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            switch (comboBox4.SelectedIndex)
            {
                case 0:
                    {
                        colorBoxGlass4.BackColor = Color.Red;
                        colorBoxGlass2.BackColor = Color.FromArgb(Color.Red.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R - offsetValue, Color.Red.G, Color.Red.B);

                        break;
                    }
                case 1:
                    {

                        colorBoxGlass4.BackColor = Color.FromArgb(0, 255, 0);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R, colorBoxGlass4.BackColor.G - offsetValue, colorBoxGlass4.BackColor.B);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R, colorBoxGlass2.BackColor.G - offsetValue, colorBoxGlass2.BackColor.B);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R, colorBoxGlass3.BackColor.G - offsetValue, colorBoxGlass3.BackColor.B);
                        break;


                    }
                case 2:
                    {

                        colorBoxGlass4.BackColor = Color.Blue;
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R, colorBoxGlass4.BackColor.G, colorBoxGlass4.BackColor.B - offsetValue);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R, colorBoxGlass2.BackColor.G, colorBoxGlass2.BackColor.B - offsetValue);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R, colorBoxGlass3.BackColor.G, colorBoxGlass3.BackColor.B - offsetValue);
                        break;

                    }
                case 3:
                    {
                        colorBoxGlass4.BackColor = Color.FromArgb(255, 255, 0);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R - offsetValue, colorBoxGlass4.BackColor.G - offsetValue, colorBoxGlass4.BackColor.B);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R - offsetValue, colorBoxGlass2.BackColor.G - offsetValue, colorBoxGlass2.BackColor.B);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R - offsetValue, colorBoxGlass3.BackColor.G - offsetValue, colorBoxGlass3.BackColor.B);

                        break;
                    }
                case 4:
                    {
                        colorBoxGlass4.BackColor = Color.FromArgb(0, 0, 0);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R + offsetValue, colorBoxGlass4.BackColor.G + offsetValue, colorBoxGlass4.BackColor.B + offsetValue);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R + offsetValue, colorBoxGlass2.BackColor.G + offsetValue, colorBoxGlass2.BackColor.B + offsetValue);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R + offsetValue, colorBoxGlass3.BackColor.G + offsetValue, colorBoxGlass3.BackColor.B + offsetValue);

                        break;
                    }
                case 5:
                    {
                        colorBoxGlass4.BackColor = Color.FromArgb(255, 255, 255);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R - offsetValue, colorBoxGlass4.BackColor.G - offsetValue, colorBoxGlass4.BackColor.B - offsetValue);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R - offsetValue, colorBoxGlass2.BackColor.G - offsetValue, colorBoxGlass2.BackColor.B - offsetValue);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R - offsetValue, colorBoxGlass3.BackColor.G - offsetValue, colorBoxGlass3.BackColor.B - offsetValue);

                        break;
                    }
                case 6:
                    {
                        colorBoxGlass4.BackColor = Color.FromArgb(255, 128, 0);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R - offsetValue, colorBoxGlass4.BackColor.G, colorBoxGlass4.BackColor.B);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R - offsetValue, colorBoxGlass2.BackColor.G, colorBoxGlass2.BackColor.B);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R - offsetValue, colorBoxGlass3.BackColor.G - offsetValue, colorBoxGlass3.BackColor.B);
                        break;
                    }
                case 7:
                    {
                        colorBoxGlass4.BackColor = Color.FromArgb(128, 0, 128);
                        colorBoxGlass2.BackColor = Color.FromArgb(colorBoxGlass4.BackColor.R + offsetValue - 30, colorBoxGlass4.BackColor.G, colorBoxGlass4.BackColor.B + offsetValue - 30);
                        colorBoxGlass3.BackColor = Color.FromArgb(colorBoxGlass2.BackColor.R + offsetValue - 30, colorBoxGlass2.BackColor.G, colorBoxGlass2.BackColor.B + offsetValue - 30);
                        colorBoxGlass1.BackColor = Color.FromArgb(colorBoxGlass3.BackColor.R + offsetValue - 30, colorBoxGlass3.BackColor.G, colorBoxGlass3.BackColor.B + offsetValue - 30);
                        break;
                    }
            }
            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            this.gradientBox1.Fill.NumberOfColors = 4;

            while (this.gradientSlider1.Values.Count < 4)
                this.gradientSlider1.Values.Insert(1, new GradientColorValue());

            switch (comboBox6.SelectedIndex)
            {
                case 0:
                    {
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 238, 156, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 0, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 178, 34, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 114, 19, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;
                    }
                case 1:
                    {
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 183, 238, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 85, 255, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 82, 178, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 51, 114, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;
                    }
                case 2:
                    {
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 156, 201, 238);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 0, 140, 255);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 34, 113, 178);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 19, 71, 114);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;

                    }
                case 3:
                    {

                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 238, 224, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 213, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 178, 154, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 114, 98, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;
                    }
                case 4:
                    {/*BLACK
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 238, 156, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 0, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 178, 34, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 114, 19, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;
                      */

                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 0, 0, 0);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.39f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 59, 59, 59);
                        this.gradientSlider1.Values[2].ColorPosition = 0.60f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 44, 44, 44);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 0, 0, 0);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;
                    }
                case 5:
                    {/* WHITE
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 238, 156, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 0, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 178, 34, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 114, 19, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f; */

                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 255, 255, 255);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.3f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 255, 255);
                        this.gradientSlider1.Values[2].ColorPosition = 0.5f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 245, 245, 245);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 202, 202, 202);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;

                        break;
                    }
                case 6:
                    {
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 238, 204, 156);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 149, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 178, 118, 34);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 114, 74, 19);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;
                        /*this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 255, 208, 69);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.5f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 255, 128, 0);
                        this.gradientSlider1.Values[2].ColorPosition = 0.79f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 255, 111, 0);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 255, 174, 0);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;*/

                        break;
                    }
                case 7:
                    {
                        this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 204, 156, 238);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.15f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 149, 0, 255);
                        this.gradientSlider1.Values[2].ColorPosition = 0.7f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 118, 34, 178);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 74, 19, 114);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;
                        /*this.gradientSlider1.Values[0].ColorValue = Color.FromArgb(255, 143, 52, 255);
                        this.gradientSlider1.Values[0].ColorPosition = 0f;
                        this.gradientSlider1.Values[1].ColorPosition = 0.2f;
                        this.gradientSlider1.Values[1].ColorValue = Color.FromArgb(255, 198, 144, 255);
                        this.gradientSlider1.Values[2].ColorPosition = 0.75f;
                        this.gradientSlider1.Values[2].ColorValue = Color.FromArgb(255, 97, 15, 197);
                        this.gradientSlider1.Values[3].ColorValue = Color.FromArgb(255, 124, 0, 255);
                        this.gradientSlider1.Values[3].ColorPosition = 1.0f;*/

                        break;
                    }
            }


            tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void numericRadial1_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                this.gradientBox1.Fill.GradientPercentage = (float)((NumericUpDown)numericRadial1).Value;
                this.tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void numericRadial2_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                this.gradientBox1.Fill.GradientPercentage = (float)((NumericUpDown)numericRadial2).Value;
                this.tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }
    }
}
