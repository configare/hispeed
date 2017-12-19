using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a progress bar element. <see cref="RadProgressBar">RadProgressBar</see>
    ///     is a simple wrapper for RadProgressBarElement. The latter may be included in other
    ///     telerik controls. All graphical and logic functionality is implemented by
    ///     RadProgressBarElement. The <see cref="RadProgressBar">RadProgressBar</see> acts to
    ///     transfer the events to and from its RadProgressBarElement instance.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadProgressBarElement : LightVisualElement
    {
        #region Fields

        private UpperProgressIndicatorElement indicatorElement1;
        private ProgressIndicatorElement indicatorElement2;
        private SeparatorsElement separatorsElement;
        private ProgressBarTextElement textElement;
        private String oldText;

        #endregion

        #region RadProperties

        public static RadProperty ValueProperty1 = RadProperty.Register("Value1", typeof(int),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                0, ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ValueProperty2 = RadProperty.Register("Value2", typeof(int),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                0, ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty MinimumProperty = RadProperty.Register("Minimum", typeof(int),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty MaximumProperty = RadProperty.Register("Maximum", typeof(int),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                100, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty StepProperty = RadProperty.Register("Step", typeof(int),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                10, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty DashProperty = RadProperty.Register("Dash", typeof(bool),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HatchProperty = RadProperty.Register("Hatch", typeof(bool),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IntegralDashProperty = RadProperty.Register("IntegralDash", typeof(bool),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ProgressOrientationProperty = RadProperty.Register("Orientation", typeof(ProgressOrientation),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                ProgressOrientation.Left, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.InvalidatesLayout | 
                ElementPropertyOptions.AffectsArrange));

        public static RadProperty ShowProgressIndicatorsProperty = RadProperty.Register(
            "ShowProgressIndicators", typeof(bool), typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty IsVerticalProperty = RadProperty.Register("IsVertical", typeof(bool),
            typeof(RadProgressBarElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value for the first progress indicator.
        /// </summary>
        [DefaultValue(0)]
        public int Value1
        {
            get
            {
                return (int)this.GetValue(ValueProperty1);
            }
            set
            {
                if (value < this.Minimum || value > this.Maximum)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value1'.\n" +
                                                "'Value1' must be between 'Minimum' and 'Maximum'.");
                }
                this.SetValue(ValueProperty1, value);
            }
        }

        /// <summary>
        /// Gets or sets the value for the second progress indicator.
        /// </summary>
        [DefaultValue(0)]
        public int Value2
        {
            get
            {
                return (int)this.GetValue(ValueProperty2);
            }
            set
            {
                if (value < this.Minimum || value > this.Maximum)
                {
                    throw new ArgumentException("'" + value + "' is not a valid value for 'Value2'.\n" +
                                                "'Value2' must be between 'Minimum' and 'Maximum'.");
                }
                this.SetValue(ValueProperty2, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum possible value for the progress bar Value1(2).
        /// </summary>
        [DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return (int)this.GetValue(MinimumProperty);
            }
            set
            {
                this.SetValue(MinimumProperty, value);
                if (this.Minimum > this.Maximum)
                    this.Maximum = this.Minimum;
                if (this.Minimum > this.Value1)
                    this.Value1 = this.Minimum;
                if (this.Minimum > this.Value2)
                    this.Value2 = this.Minimum;
            }
        }

        /// <summary>
        /// Gets or sets the maximum possible value for the progress bar Value1(2).
        /// </summary>
        [DefaultValue(100)]
        public int Maximum
        {
            get
            {
                return (int)this.GetValue(MaximumProperty);
            }
            set
            {
                this.SetValue(MaximumProperty, value);
                if (this.Maximum < this.Value1)
                    this.Value1 = this.Maximum;
                if (this.Maximum < this.Value2)
                    this.Value2 = this.Maximum;
                if (this.Maximum < this.Minimum)
                    this.Minimum = this.Maximum;
            }
        }

        /// <summary>
        /// Gets or sets the value with which the progress bar Value1(2) will 
        /// increments/decrements.
        /// </summary>
        [DefaultValue(1)]
        public int Step
        {
            get
            {
                return (int)this.GetValue(StepProperty);
            }
            set
            {
                this.SetValue(StepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step width in pixels with which the progress bar 
        /// indicator will move if style is dash.
        /// </summary>
        [DefaultValue(3)]
        public int StepWidth
        {
            get
            {
                return this.separatorsElement.StepWidth;
            }
            set
            {
                this.separatorsElement.StepWidth = value;
            }
        }     
    
        /// <summary>
        /// Gets or sets the progress orientation of the progress bar indicator. 
        /// Bottom, Left, Right, Top
        /// </summary>
        [DefaultValue(ProgressOrientation.Left)]
        public ProgressOrientation ProgressOrientation
        {
            get
            {
                return (ProgressOrientation)this.GetValue(ProgressOrientationProperty);
            }
            set
            {
                this.SetValue(ProgressOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets if the progress should be show with percentages.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowProgressIndicators
        {
            get
            {
                return (bool)this.GetValue(ShowProgressIndicatorsProperty);
            }
            set
            {
                this.SetValue(ShowProgressIndicatorsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style to dash.
        /// </summary>
        [DefaultValue(false)]
        public bool Dash
        {
            get
            {
                return (bool)this.GetValue(DashProperty);
            }
            set
            {
                this.SetValue(DashProperty, value);
                this.separatorsElement.Dash = value;
            }
        }

        /// <summary>
        /// Gets or sets the style to hatch.
        /// </summary>
        [DefaultValue(false)]
        public bool Hatch
        {
            get
            {
                return (bool)this.GetValue(HatchProperty);
            }
            set
            {
                this.SetValue(HatchProperty, value);
                this.separatorsElement.Hatch = value;
            }
        }

        /// <summary>
        /// Gets or sets the style to integral dash. To set IntegralDash you need
        /// to first set dash to true.
        /// </summary>
        [DefaultValue(false)]
        public bool IntegralDash
        {
            get
            {
                return (bool)this.GetValue(IntegralDashProperty);
            }
            set
            {
                this.SetValue(IntegralDashProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the progress bar indicator image.
        /// </summary>
        public override Image Image
        {
            get
            {
                return this.indicatorElement1.Image;
            }
            set
            {
                if (value == null)
                {
                    this.indicatorElement1.DrawFill = true;
                }
                else
                {
                    this.indicatorElement1.DrawFill = false;
                }
                this.indicatorElement1.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout of the image in the progress indicator.
        /// </summary>
        public ImageLayout ImageLayout
        {
            get
            {
                return this.indicatorElement1.ImageLayout;
            }
            set
            {
                this.indicatorElement1.ImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the image index of the progress bar indicator image.
        /// </summary>
        public override int ImageIndex
        {
            get
            {
                return this.indicatorElement1.ImageIndex;
            }
            set
            {
                this.indicatorElement1.ImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the image key for the progress bar indicator image.
        /// </summary>
        public override string ImageKey
        {
            get
            {
                return this.indicatorElement1.ImageKey;
            }
            set
            {
                this.indicatorElement1.ImageKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the image in the progress line.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public override ContentAlignment ImageAlignment
        {
            get
            {
                return this.indicatorElement1.ImageAlignment;
            }
            set
            {
                this.indicatorElement1.ImageAlignment = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="UpperProgressIndicatorElement"/> class
        /// that represents the progress indicator of the progress bar.
        /// </summary>
        public UpperProgressIndicatorElement IndicatorElement1
        {
            get
            {
                return indicatorElement1;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="ProgressIndicatorElement"/> class
        /// that represents the progress bar indicator.
        /// </summary>
        public ProgressIndicatorElement IndicatorElement2
        {
            get
            {
                return indicatorElement2;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="SeparatorsElement"/> class
        /// that represents the separators on the progress bar indicator.
        /// </summary>
        public SeparatorsElement SeparatorsElement
        {
            get
            {
                return separatorsElement;
            }
        }

        /// <summary>Gets or sets the separators width in pixels.</summary>
        [DefaultValue(3)]
        public int SeparatorWidth
        {
            get
            {
                return this.SeparatorsElement.SeparatorWidth;
            }
            set
            {
                this.SeparatorsElement.SeparatorWidth = value;
            }
        }

        /// <summary>Gets or sets the first gradient color for separators</summary>
        public Color SeparatorColor1
        {
            get
            {
                return this.SeparatorsElement.SeparatorColor1;
            }
            set
            {
                this.SeparatorsElement.SeparatorColor1 = value;
            }
        }

        /// <summary>Gets or sets the second gradient color for separators</summary>
        public Color SeparatorColor2
        {
            get
            {
                return this.SeparatorsElement.SeparatorColor2;
            }
            set
            {
                this.SeparatorsElement.SeparatorColor2 = value;
            }
        }

        /// <summary>Gets or sets the third gradient color for separators</summary>
        public Color SeparatorColor3
        {
            get
            {
                return this.SeparatorsElement.SeparatorColor3;
            }
            set
            {
                this.SeparatorsElement.SeparatorColor3 = value;
            }
        }

        /// <summary>Gets or sets the fourth gradient color for separators</summary>
        public Color SeparatorColor4
        {
            get
            {
                return this.SeparatorsElement.SeparatorColor4;
            }
            set
            {
                this.SeparatorsElement.SeparatorColor4 = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle of the separators gradient
        /// </summary>
        public int SeparatorGradientAngle
        {
            get
            {
                return this.separatorsElement.SeparatorGradientAngle;
            }
            set
            {
                this.separatorsElement.SeparatorGradientAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the first color percentage in the separator gradient.
        /// </summary>
        public float SeparatorGradientPercentage1
        {
            get
            {
                return this.separatorsElement.SeparatorGradientPercentage1;
            }
            set
            {
                this.separatorsElement.SeparatorGradientPercentage1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second color percentage in the separator gradient.
        /// </summary>
        public float SeparatorGradientPercentage2
        {
            get
            {
                return this.separatorsElement.SeparatorGradientPercentage2;
            }
            set
            {
                this.separatorsElement.SeparatorGradientPercentage2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of colors used in the separator gradient.
        /// </summary>
        public int SeparatorNumberOfColors
        {
            get
            {
                return this.separatorsElement.NumberOfColors;
            }
            set
            {
                this.separatorsElement.NumberOfColors = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="ProgressBarTextElement"/> class
        /// that represents the text of the progress bar.
        /// </summary>
        public ProgressBarTextElement TextElement
        {
            get
            {
                return textElement;
            }
        }

        /// <summary>Gets or sets the text associated with this control.</summary>
        public override string Text
        {
            get
            {
                return this.textElement.Text;
            }
            set
            {
                this.textElement.Text = value;
                this.oldText = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle at which the dash or hatch lines are tilted.
        /// </summary>
        public int SweepAngle
        {
            get
            {
                return this.separatorsElement.SweepAngle;
            }
            set
            {
                this.separatorsElement.SweepAngle = value;
            }
        }
        
        #endregion

        #region Initialization

        static RadProgressBarElement()
        {
            new Themes.ControlDefault.ProgressBar().DeserializeTheme();
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadProgressBarStateManager(), typeof(RadProgressBarElement));
        }
        
        protected override void CallCreateChildElements()
        {
            base.CallCreateChildElements();

            this.indicatorElement1 = new UpperProgressIndicatorElement();

            this.indicatorElement2 = new ProgressIndicatorElement();

            this.separatorsElement = new SeparatorsElement();

            this.textElement = new ProgressBarTextElement();
            this.textElement.StretchHorizontally = true;
            this.textElement.StretchVertically = true;

            this.Children.Add(indicatorElement2);
            this.Children.Add(indicatorElement1);
            this.Children.Add(separatorsElement);
            this.Children.Add(textElement);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DefaultSize = new Size(50, 20);
            this.ClipDrawing = true;
        }
        
        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = SizeF.Empty;
            RectangleF clientRect = new RectangleF(PointF.Empty, availableSize);
            Padding borderThickness = GetBorderThickness(false);
            clientRect.Width -= this.Padding.Horizontal + borderThickness.Horizontal;
            clientRect.Height -= this.Padding.Vertical + borderThickness.Vertical;

            RectangleF arrangeRectangleProgressFill1 = GetProgressIndicatorFinalSize(indicatorElement1, clientRect, this.Value1);
            RectangleF arrangeRectangleProgressFill2 = GetProgressIndicatorFinalSize(indicatorElement2, clientRect, this.Value2);
            RectangleF arrangeRectangleSeparators = GetSeparatorsFinalSize(arrangeRectangleProgressFill1, arrangeRectangleProgressFill2);

            this.indicatorElement1.Measure(arrangeRectangleProgressFill1.Size);
            this.indicatorElement2.Measure(arrangeRectangleProgressFill2.Size);
            this.separatorsElement.Measure(arrangeRectangleSeparators.Size);
            this.textElement.Measure(clientRect.Size);

            if (float.IsInfinity(arrangeRectangleSeparators.Size.Width))
            {
                desiredSize.Width = this.textElement.DesiredSize.Width;
            }
            else
            {
                desiredSize.Width = Math.Max(arrangeRectangleSeparators.Size.Width, this.textElement.DesiredSize.Width);
            }

            if (float.IsInfinity(arrangeRectangleSeparators.Size.Height))
            {
                desiredSize.Height = this.textElement.DesiredSize.Height;
            }
            else
            {
                desiredSize.Height = Math.Max(arrangeRectangleSeparators.Size.Height, this.textElement.DesiredSize.Height);
            }

            desiredSize.Width += this.Padding.Horizontal + borderThickness.Horizontal;
            desiredSize.Height += this.Padding.Vertical + borderThickness.Vertical;

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            RectangleF arrangeRectangleProgressFill1 = GetProgressIndicatorFinalSize(indicatorElement1, clientRect, this.Value1);
            RectangleF arrangeRectangleProgressFill2 = GetProgressIndicatorFinalSize(indicatorElement2, clientRect, this.Value2);

            RectangleF arrangeRectangleSeparators = GetSeparatorsFinalSize(arrangeRectangleProgressFill1, arrangeRectangleProgressFill2);
            RectangleF arrangeRectangleTextElement = GetClientRectangle(false, finalSize);

            this.indicatorElement1.Arrange(arrangeRectangleProgressFill1);
            this.indicatorElement2.Arrange(arrangeRectangleProgressFill2);
            this.separatorsElement.Arrange(arrangeRectangleSeparators);
            this.textElement.Arrange(arrangeRectangleTextElement);

            return finalSize;
        }

        protected RectangleF GetProgressIndicatorFinalSize(ProgressIndicatorElement element, RectangleF clientRect, int value)
        {
            if (value == Minimum)
            {
                element.Visibility = ElementVisibility.Collapsed;
                return RectangleF.Empty;
            }

            element.Visibility = ElementVisibility.Visible;

            if (value == Maximum)
            {
                return clientRect;
            }

            int step = this.separatorsElement.SeparatorWidth + this.separatorsElement.StepWidth;

            if (this.ProgressOrientation == ProgressOrientation.Left ||
                this.ProgressOrientation == ProgressOrientation.Right)
            {
                return GetHorizontalProgressIndicatorFinalSize(clientRect, value, step);
            }
            else
            {
                return GetVerticalProgressIndicatorFinalSize(clientRect, value, step);
            }
        }

        protected RectangleF GetVerticalProgressIndicatorFinalSize(RectangleF clientRect, int value, int step)
        {
            float height = 0;
            float width = 0;
            float range = this.Maximum - this.Minimum;
            float realValue = value - this.Minimum;

            width = clientRect.Width;
            height = (realValue * clientRect.Height) / range;
            height = (float)Math.Floor(height);

            if (this.Dash && !this.IntegralDash)
            {
                height = height - (height % step);

                if (height <= 0 && value > 0)
                {
                    height = step;
                }
            }
            
            PointF startPoint = clientRect.Location;
            if (this.ProgressOrientation == ProgressOrientation.Bottom)
            {
                startPoint.Y = clientRect.Height - height + this.BorderTopWidth + this.Padding.Top + 1;
                if (this.Dash && !this.IntegralDash)
                {
                    startPoint.Y += this.separatorsElement.SeparatorWidth;
                    height -= this.separatorsElement.SeparatorWidth;
                }
            }

            if (height > clientRect.Height)
            {
                startPoint.Y = clientRect.Y;
                height = clientRect.Height;
            }

            return new RectangleF(startPoint, new SizeF(width, height));
        }

        protected RectangleF GetHorizontalProgressIndicatorFinalSize(RectangleF clientRect, int value, int step)
        {
            float height = 0;
            float width = 0;
            float range = this.Maximum - this.Minimum;
            float realValue = value - this.Minimum;

            height = clientRect.Height;
            width = (realValue * clientRect.Width) / range;
            width = (float)Math.Floor(width);

            if (this.Dash && !this.IntegralDash)
            {
                width = width - (width % step);

                if (width <= 0 && value > 0)
                {
                    width = step;
                }
            }
            
            PointF startPoint = clientRect.Location;
            if (this.ProgressOrientation == ProgressOrientation.Right)
            {
                startPoint.X = clientRect.Width - width + this.BorderLeftWidth + this.Padding.Left + 1;
                if (this.Dash && !this.IntegralDash)
                {
                    startPoint.X += this.separatorsElement.SeparatorWidth;
                    width -= this.separatorsElement.SeparatorWidth;
                }
            }

            if (width > clientRect.Width)
            {
                startPoint.X = clientRect.X;
                width = clientRect.Width;
            }

            return new RectangleF(startPoint, new SizeF(width, height));
        }

        protected RectangleF GetSeparatorsFinalSize(RectangleF progressBar1Rectangle, RectangleF progressBar2Rectangle)
        {
            RectangleF result = RectangleF.Empty;

            if (this.ProgressOrientation == ProgressOrientation.Left || this.ProgressOrientation == ProgressOrientation.Right)
            {
                if (progressBar1Rectangle.Width > progressBar2Rectangle.Width)
                {
                    result = progressBar1Rectangle;
                }
                else
                {
                    result = progressBar2Rectangle;
                }
            }
            else
            {
                if (progressBar1Rectangle.Height > progressBar2Rectangle.Height)
                {
                    result = progressBar1Rectangle;
                }
                else
                {
                    result = progressBar2Rectangle;
                }
            }

            return result;
        }

        #endregion

        #region Painting

        protected override void PaintChildren(Paint.IGraphics graphics, Rectangle clipRectange, float angle, SizeF scale, bool useRelativeTransformation)
        {
            if (this.Shape != null)
            {
                GraphicsPath path = Shape.GetElementShape(this);

                Graphics g = graphics.UnderlayGraphics as Graphics;
                if (g != null)
                {
                    g.Clip = new Region(path);
                }
            }

            base.PaintChildren(graphics, clipRectange, angle, scale, useRelativeTransformation);
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == ShowProgressIndicatorsProperty)
            {
                UpdateProgressIndicator();
            }
            if (e.Property == DashProperty || e.Property == HatchProperty)
            {
                if (this.Dash || this.Hatch)
                {
                    this.separatorsElement.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.separatorsElement.Visibility = ElementVisibility.Collapsed;
                }
            }
            if (e.Property == ValueProperty1 || e.Property == ValueProperty2)
            {
                if (this.indicatorElement1.AutoOpacity)
                {
                    ControlProgressIndicatorsOpacity();
                }
                UpdateProgressIndicator();
            }
            if (e.Property == ProgressOrientationProperty)
            {
                ProgressOrientation progressOrientation = (ProgressOrientation)e.NewValue;

                bool isVertical;

                if (progressOrientation == ProgressOrientation.Top || progressOrientation == ProgressOrientation.Bottom)
                {
                    isVertical = true;
                }
                else
                {
                    isVertical = false;
                }

                this.SetValue(RadProgressBarElement.IsVerticalProperty, isVertical);
                this.indicatorElement1.SetValue(ProgressIndicatorElement.IsVerticalProperty, isVertical);
                this.indicatorElement2.SetValue(ProgressIndicatorElement.IsVerticalProperty, isVertical);
            }

            base.OnPropertyChanged(e);
        }

        private void UpdateProgressIndicator()
        {
            if (this.ShowProgressIndicators == true)
            {
                int progress1 = 100 * this.Value1 / (Maximum - Minimum);
                this.textElement.Text = progress1 + " %";
            }
            else
            {
                if (String.IsNullOrEmpty(this.oldText))
                {
                    this.textElement.Text = String.Empty;
                }
                else
                {
                    this.textElement.Text = this.oldText;
                }
            }
        }
        
        private void ControlProgressIndicatorsOpacity()
        {
            if (Value1 != 0 & Value2 != 0)
            {
                double ratio = ((double)Value2) / Value1;
                if (ratio > (2d - this.indicatorElement1.AutoOpacityMinimum))
                {
                    this.indicatorElement1.Opacity = 1d;
                }
                else if (ratio < 1d)
                {
                    this.indicatorElement1.Opacity = this.indicatorElement1.AutoOpacityMinimum;
                }
                else
                {
                    double opacity = this.indicatorElement1.AutoOpacityMinimum;
                    opacity = opacity + (ratio - 1);
                    this.indicatorElement1.Opacity = opacity;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Advances the
        /// current position of the progress bar by the amount of the Step property</span>
        /// </summary>
        public void PerformStepValue1()
        {
            if (Value1 < Maximum)
                Value1 += Step;
            else
                Value1 = Maximum;
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Reverses the
        /// advance of the current position of the second progress bar by the amount of the Step
        /// property.</span>
        /// </summary>
        public void PerformStepBackValue1()
        {
            if (Value1 > Minimum)
                Value1 -= Step;
            else
                Value1 = Minimum;
        }

        /// <summary>Increments Value1 with the given argument value.</summary>
        public void IncrementValue1(int value)
        {
            if (Value1 < Maximum)
                Value1 += value;
            else
                Value1 = Maximum;
        }

        /// <summary>Decrements Value1 with the given argument value.</summary>
        public void DecrementValue1(int value)
        {
            if (Value1 > Minimum)
                Value1 -= value;
            else
                Value1 = Minimum;
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Advances the
        /// current position of the first progress bar by the amount of the Step
        /// property.</span>
        /// </summary>
        public void PerformStepValue2()
        {
            if (Value2 < Maximum)
                Value2 += Step;
            else
                Value2 = Maximum;
        }

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Advances the
        /// current position of the first progress bar by the amount of the Step
        /// property.</span>
        /// </summary>
        public void PerformStepBackValue2()
        {
            if (Value2 > Minimum)
                Value2 -= Step;
            else
                Value2 = Minimum;
        }

        /// <summary>Increments Value2 with the given argument value.</summary>
        public void IncrementValue2(int value)
        {
            if (Value2 < Maximum)
                Value2 += value;
            else
                Value2 = Maximum;
        }

        /// <summary>Decrements Value2 with the given argument value.</summary>
        public void DecrementValue2(int value)
        {
            if (Value2 > Minimum)
                Value2 -= value;
            else
                Value2 = Minimum;
        }
        #endregion
    }
}
