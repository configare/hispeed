using System;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a progress bar. You can set progress bar appearance in numerous ways.
    ///     For example, you can use dash or dash integral style, set separator color and width, set a
    ///     background image, etc. The RadProgressBar class is a simple wrapper for the
    ///     <see cref="RadProgressBarElement">RadProgressBarElement</see> class. The latter may
    ///     be nested in other telerik controls. All UI and logic functionality is
    ///     implemented by the <see cref="RadProgressBarElement">RadProgressBarElement</see>
    ///     class. RadProgressBar acts to transfer the events to and from the
    ///     <see cref="RadProgressBarElement">RadProgressBarElement</see> class.
    /// </summary>
    [RadThemeDesignerData(typeof(RadProgressBarStyleBuilderData))]
    [ToolboxItem(true)]
    [Description("Displays a bar that fills to display progress information during a long-running operation.")]
    [DefaultProperty("Value"), DefaultEvent("Click")]
    public class RadProgressBar : RadControl
    {
        #region Fields

        RadProgressBarElement progressBarElement;

        /// <summary>
        ///     Represents the method that will handle some of the following events:
        ///     <see cref="ValueChanged">ValueChanged</see>,
        ///     <see cref="StepChanged">StepChanged</see>,
        ///     <see cref="StepWidthChanged">StepWidthChanged</see>,
        ///     <see cref="SeparatorWidthChanged">SeparatorWidthChanged</see>,
        ///     <see cref="MinimumChanged">MinimumChanged</see>,
        ///     <see cref="MaximumChanged">MaximumChanged</see>,
        ///     <see cref="DashChanged">DashChanged</see>,
        ///     <see cref="DashChanged">TextOrientationChanged</see>,
        ///     <param name="sender">Represents the event sender.</param>
        /// 	<param name="e">Represents the event arguments.</param>
        /// </summary>
        public delegate void ProgressBarHandler(object sender, ProgressBarEventArgs e);

        /// <summary>Fires when value is changed.</summary>
        public event ProgressBarHandler ValueChanged;
        /// <summary>Fires when step is changed.</summary>
        public event ProgressBarHandler StepChanged;
        /// <summary>Fires when step width is changed.</summary>
        public event ProgressBarHandler StepWidthChanged;
        /// <summary>Fires when the separator width is changed.</summary>
        public event ProgressBarHandler SeparatorWidthChanged;
        /// <summary>Fires when the minimum property is changed.</summary>
        public event ProgressBarHandler MinimumChanged;
        /// <summary>Fires when the maximum property is changed.</summary>
        public event ProgressBarHandler MaximumChanged;
        /// <summary>Fires when the dash property is changed.</summary>
        public event ProgressBarHandler DashChanged;
        /// <summary>Fires when the hatch property is changed.</summary>
        public event ProgressBarHandler HatchChanged;
        /// <summary>Fires when the integral dash property is changed.</summary>
        public event ProgressBarHandler IntegralDashChanged;
        /// <summary>Fires when the text orientation is changed.</summary>
        public event ProgressBarHandler TextOrientationChanged;
        /// <summary>Fires when the text alignment is changed.</summary>
        public event ProgressBarHandler TextAlignmentChanged;
        /// <summary>Fires when the progress orientation is changed.</summary>
        public event ProgressBarHandler ProgressOrientationChanged;
        /// <summary>Fires when show progress indicators is changed.</summary>
        public event ProgressBarHandler ShowProgressIndicatorsChanged;
        /// <summary>Fires when the separator color is changed.</summary>
        public event ProgressBarHandler SeparatorColorChanged;

        #endregion
        
        #region Initialization

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.progressBarElement = CreateProgressBarElement();
            parent.Children.Add(progressBarElement);
        }

        protected virtual RadProgressBarElement CreateProgressBarElement()
        {
            return new RadProgressBarElement();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of RadProgressBarElement wrapped by this control. RadProgressBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadProgressBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadProgressBarElement ProgressBarElement
        {
            get
            {
                return this.progressBarElement;
            }
        }

        /// <summary>
        /// Gets or sets the value of the first progress line. There could be two progress
        /// lines in the progress bar.
        /// </summary>
        [Description("Current Value of the progress in the range between Minimum and Maximum."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Value1", typeof(RadProgressBarElement))]
        [DefaultValue(0)]
        public int Value1
        {
            get
            {
                return progressBarElement.Value1;
            }
            set
            {
                this.progressBarElement.Value1 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.ValueChanged));
            }
        }

        /// <summary>
        /// Gets or sets the value of the second progress line. There could be two progress
        /// lines in the progress bar.
        /// </summary>
        [Description("Current Value of the progress in the range between Minimum and Maximum."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Value2", typeof(RadProgressBarElement)),
        DefaultValue(0)]
        public int Value2
        {
            get
            {
                return progressBarElement.Value2;
            }
            set
            {
                this.progressBarElement.Value2 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.ValueChanged));
            }
        }

        /// <summary>Gets or sets the minimum value for the progress.</summary>
        [Description("The lower bound of the range this ProgressBar is working with."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Minimum", typeof(RadProgressBarElement)),
        DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return this.progressBarElement.Minimum;
            }
            set
            {
                this.progressBarElement.Minimum = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.MinimumChanged));
            }
        }

        /// <summary>Gets or sets the maximum value for the progress.</summary>
        [Description("The upper bound of the range this ProgressBar is working with"),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Maximum", typeof(RadProgressBarElement)), 
        DefaultValue(100)]
        public int Maximum
        {
            get
            {
                return this.progressBarElement.Maximum;
            }
            set
            {
                this.progressBarElement.Maximum = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.MaximumChanged));
            }
        }

        /// <summary>Gets or sets a value indicating the amount to increment the current value with.</summary>
        [Description("The amount to increment the current value."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Step", typeof(RadProgressBarElement)),
        DefaultValue(10)]
        public int Step
        {
            get
            {
                return this.progressBarElement.Step;
            }
            set
            {
                this.progressBarElement.Step = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.StepChanged));
            }
        }

        /// <summary>Gets or sets the StepWidth between different separators.</summary>
        [Description("Gets or sets the StepWidth between different separators."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("StepWidth", typeof(RadProgressBarElement)),
        DefaultValue(14)]
        public int StepWidth
        {
            get
            {
                return this.progressBarElement.StepWidth;
            }
            set
            {
                this.progressBarElement.StepWidth = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.StepWidthChanged));
            }
        }

        /// <summary>Gets or sets the text associated with this control.</summary>
        [Localizable(true), Bindable(true)]
        [RadDefaultValue("Text", typeof(RadProgressBarElement))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the displayed text.")]
        public override string Text
        {
            get
            {
                return this.progressBarElement.TextElement.Text;
            }
            set
            {
                this.progressBarElement.Text = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.TextChanged));
            }
        }

        /// <summary>
        /// 	<para>Indicates whether the progress bar style is dash. When style is dash
        /// 	the progress line is broken into segments with separators in between them.</para>
        /// </summary>
        [Description("Gets or Sets the style of the ProgressBar to dash."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Dash", typeof(RadProgressBarElement)),
        DefaultValue(false)]
        public bool Dash
        {
            get
            {
                return this.progressBarElement.Dash;
            }
            set
            {
                this.progressBarElement.Dash = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.DashChanged));

            }
        }

        /// <summary>
        /// 	<para>Indicates whether the progress bar style is hatch. When style is hatch
        /// 	the progress line is covered with a hatch. You will have to change the SweepAngle
        /// 	in order to see the style.</para>
        /// </summary>
        [Description("Gets or Sets the style of the ProgressBar to hacth."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("Dash", typeof(RadProgressBarElement)),
        DefaultValue(false)]
        public bool Hatch
        {
            get
            {
                return this.progressBarElement.Hatch;
            }
            set
            {
                this.progressBarElement.Hatch = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.HatchChanged));

            }
        }

        /// <summary>
        /// When style is dash indicates if the progress indicators will progress on steps or smoothly.
        /// </summary>
        [Description("When style is dash indicates if the progress indicators will progress on steps or smoothly."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("IntegralDash", typeof(RadProgressBarElement)),
        DefaultValue(false)]
        public bool IntegralDash
        {
            get
            {
                return this.progressBarElement.IntegralDash;
            }
            set
            {
                this.progressBarElement.IntegralDash = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.IntegralDashChanged));
            }
        }

        /// <summary>Gets or sets the first gradient color for separators</summary>
        [Description("Gets or Sets the first gradient color for separators"),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorColor1", typeof(RadProgressBarElement))]
        public Color SeparatorColor1
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorColor1;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorColor1 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorChanged));
            }
        }

        /// <summary>Gets or sets the second gradient color for separators.</summary>
        [Description("Gets or Sets the second gradient color for separators."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorColor2", typeof(RadProgressBarElement))]
        public Color SeparatorColor2
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorColor2;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorColor2 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorChanged));
            }
        }

        /// <summary>Gets or sets the third gradient color for separators.</summary>
        [Description("Gets or Sets the third gradient color for separators."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorColor3", typeof(RadProgressBarElement))]
        public Color SeparatorColor3
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorColor3;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorColor3 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorChanged));
            }
        }

        /// <summary>Gets or sets the fourth gradient color for separators.</summary>
        [Description("Gets or Sets the fourth gradient color for separators."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorColor4", typeof(RadProgressBarElement))]
        public Color SeparatorColor4
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorColor4;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorColor4 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorChanged));
            }
        }

        /// <summary>Gets or sets the fourth gradient color for separators.</summary>
        [Description("Gets or Sets the angle of the separators gradient."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorGradientAngle", typeof(RadProgressBarElement)),
        DefaultValue(0)]
        public int SeparatorGradientAngle
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorGradientAngle;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorGradientAngle = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorGradientAngleChanged));
            }
        }

        /// <summary>Gets or sets the first color stop in the separator gradient.</summary>
        [Description("Gets or sets the first color stop in the separator gradient."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorGradientPercentage1", typeof(RadProgressBarElement)),
        DefaultValue(0.4f)]
        public float SeparatorGradientPercentage1
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorGradientPercentage1;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorGradientPercentage1 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorStopChanged));
            }
        }

        /// <summary>Gets or sets the second color stop in the separator gradient.</summary>
        [Description("Gets or sets the second color stop in the separator gradient."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorGradientPercentage2", typeof(RadProgressBarElement)),
        DefaultValue(0.6f)]
        public float SeparatorGradientPercentage2
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorGradientPercentage2;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorGradientPercentage2 = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorColorStopChanged));
            }
        }

        /// <summary>Gets or sets the number of colors used in the separator gradient.</summary>
        [Description("Gets or sets the number of colors used in the separator gradient."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorGradientPercentage2", typeof(RadProgressBarElement)),
        DefaultValue(2)]
        public int SeparatorNumberOfColors
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.NumberOfColors;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.NumberOfColors = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorNumberOfColorChanged));
            }
        }
        
        /// <summary>Gets or sets the separators width in pixels.</summary>
        [Description("Gets or Sets the width of separators"),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SeparatorsWidth", typeof(RadProgressBarElement)),
        DefaultValue(3)]
        public int SeparatorWidth
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SeparatorWidth;
            }
            set
            {
                this.progressBarElement.SeparatorsElement.SeparatorWidth = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.SeparatorWidthChanged));
            }
        }

        /// <commentsfrom cref="RadProgressBarElement.Image" filter=""/>        
        [RadDefaultValue("Image", typeof(RadProgressBarElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("Image", typeof(RadProgressBarElement)),
        RefreshProperties(RefreshProperties.All)]
        public Image Image
        {
            get
            {
                return this.progressBarElement.Image;
            }
            set
            {
                this.progressBarElement.Image = value;
            }
        }

        /// <commentsfrom cref="RadProgressBarElement.ImageLayout" filter=""/>        
        [RadDefaultValue("ImageLayout", typeof(RadProgressBarElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageLayout", typeof(RadProgressBarElement)),
        RefreshProperties(RefreshProperties.All)]
        public ImageLayout ImageLayout
        {
            get
            {
                return this.progressBarElement.IndicatorElement1.BackgroundImageLayout;
            }
            set
            {
                this.progressBarElement.IndicatorElement1.BackgroundImageLayout = value;
            }
        }

        /// <commentsfrom cref="RadProgressBarElement.ImageIndex" filter=""/>
        [RadDefaultValue("ImageIndex", typeof(RadProgressBarElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageIndex", typeof(RadProgressBarElement)),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public int ImageIndex
        {
            get
            {
                return this.progressBarElement.ImageIndex;
            }
            set
            {
                this.progressBarElement.ImageIndex = value;
            }
        }

        /// <commentsfrom cref="RadProgressBarElement.ImageKey" filter=""/>
        [RadDefaultValue("ImageKey", typeof(RadProgressBarElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageKey", typeof(RadProgressBarElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string ImageKey
        {
            get
            {
                return this.progressBarElement.ImageKey;
            }
            set
            {
                this.progressBarElement.ImageKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the image of the progress line.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the alignment of the image of the progress line."),
        DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment ImageAlignment
        {
            get
            {
                return this.progressBarElement.ImageAlignment;
            }
            set
            {
                this.progressBarElement.ImageAlignment = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the text orientation in the progress bar.
        /// </summary>
        [Description("Gets or sets the text orientation in the progress bar."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("TextOrientation", typeof(RadProgressBarElement)),
        DefaultValue(Orientation.Horizontal)]
        public Orientation TextOrientation
        {
            get
            {
                return this.progressBarElement.TextOrientation;
            }
            set
            {
                this.progressBarElement.TextOrientation = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.TextOrientationChanged));
            }
        }

        /// <exclude/>
        /// <summary>Gets or sets the alignment of the text content on the drawing surface.</summary>
        /// <excludetoc/>
        [Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the alignment of text content on the drawing surface."),
        DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlignment
        {
            get
            {
                return this.progressBarElement.TextAlignment;
            }
            set
            {
                this.progressBarElement.TextAlignment = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.TextAlignmentChanged));
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Telerik.WinControls.ProgressOrientation">progress
        ///     orientation</see>: Bottom, Left, Right, Top.
        /// </summary>
        [Description("Gets or Sets the progress orientation of progress bar"),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("ProgressOrientation", typeof(RadProgressBarElement)),
        DefaultValue(ProgressOrientation.Left)]
        public ProgressOrientation ProgressOrientation
        {
            get
            {
                return this.progressBarElement.ProgressOrientation;
            }
            set
            {
                this.progressBarElement.ProgressOrientation = value;
                this.progressBarElement.SeparatorsElement.ProgressOrientation = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.ProgressOrientationChanged));
            }
        }

        /// <summary>
        /// Indicates whether the progress bar style is hatch. When true, the style is Hatch.
        /// When both dash and hatch are true the style is hatch.
        /// </summary>
        [Description("Gets or sets whether progress should show percents"),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("ShowProgressIndicator", typeof(RadProgressBarElement)),
        DefaultValue(false)]
        public bool ShowProgressIndicators
        {
            get
            {
                return this.progressBarElement.ShowProgressIndicators;
            }
            set
            {
                this.progressBarElement.ShowProgressIndicators = value;
                OnPropertyChanged(new ProgressBarEventArgs(ProgressBarSenderEvent.ShowProgressIndicatorsChanged));
            }
        }

        /// <summary>
        /// Gets or sets the angle at which the dash or hatch lines are tilted.
        /// </summary>
        [Description("Gets or sets the angle at which the dash or hatch lines are tilted."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All),
        RadPropertyDefaultValue("SweepAngle", typeof(RadProgressBarElement)),
        DefaultValue(90)]
        public int SweepAngle
        {
            get
            {
                return this.progressBarElement.SeparatorsElement.SweepAngle;
            }
            set
            {
                if (value % 180 != 0)
                {
                    this.progressBarElement.SeparatorsElement.SweepAngle = value;
                }
                else
                {
                    throw new ArgumentException("Sweep angle cannot be equal to kπ, where k=(0,1,2...).");
                }
            }
        }

        /// <summary>
        /// Gets or sets the background image of the RadProgressBar.
        /// </summary>
        [Description("Gets or sets the background image of the RadProgressBar."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All)]
        public override Image BackgroundImage
        {
            get
            {
                return this.progressBarElement.BackgroundImage;
            }
            set
            {
                this.progressBarElement.BackgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout of the background image of the RadProgressBar.
        /// </summary>
        [Description("Gets or sets the layout of the background image of the RadProgressBar."),
        Category(RadDesignCategory.AppearanceCategory),
        RefreshProperties(RefreshProperties.All)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return this.progressBarElement.BackgroundImageLayout;
            }
            set
            {
                this.progressBarElement.BackgroundImageLayout = value;
            }
        }

        #endregion

        #region Events

        protected virtual void OnPropertyChanged(ProgressBarEventArgs e)
        {
            switch (e.SenderEvent)
            {
                case ProgressBarSenderEvent.ValueChanged:
                    {
                        if (ValueChanged != null)
                        {
                            ValueChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.MinimumChanged:
                    {
                        if (MinimumChanged != null)
                        {
                            MinimumChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.MaximumChanged:
                    {
                        if (MaximumChanged != null)
                        {
                            MaximumChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.StepChanged:
                    {
                        if (StepChanged != null)
                        {
                            StepChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.StepWidthChanged:
                    {
                        if (StepWidthChanged != null)
                        {
                            StepWidthChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.DashChanged:
                    {
                        if (DashChanged != null)
                        {
                            DashChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.HatchChanged:
                    {
                        if (HatchChanged != null)
                        {
                            HatchChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.TextChanged:
                    {
                        this.OnTextChanged(e);
                    }
                    break;
                case ProgressBarSenderEvent.SeparatorWidthChanged:
                    {
                        if (SeparatorWidthChanged != null)
                        {
                            SeparatorWidthChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.TextOrientationChanged:
                    {
                        if (TextOrientationChanged != null)
                        {
                            TextOrientationChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.TextAlignmentChanged:
                    {
                        if (TextAlignmentChanged != null)
                        {
                            TextAlignmentChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.ProgressOrientationChanged:
                    {
                        if (ProgressOrientationChanged != null)
                        {
                            ProgressOrientationChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.ShowProgressIndicatorsChanged:
                    {
                        if (ShowProgressIndicatorsChanged != null)
                        {
                            ShowProgressIndicatorsChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.SeparatorColorChanged:
                    {
                        if (SeparatorColorChanged != null)
                        {
                            SeparatorColorChanged(this, e);
                        }
                    }
                    break;
                case ProgressBarSenderEvent.IntegralDashChanged:
                    {
                        if (IntegralDashChanged != null)
                        {
                            IntegralDashChanged(this, e);
                        }
                    }
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Exposes the reason for a progress bar or waiting bar event.
    /// </summary>
    public enum ProgressBarSenderEvent
    {
        /// <summary>
        /// Indicates that value1 or value2 has been changed.
        /// </summary>
        ValueChanged,

        /// <summary>
        /// Indicates that the Minimum property has been changed.
        /// </summary>
        MinimumChanged,

        /// <summary>
        /// Indicates that the Maximum property has been changed.
        /// </summary>
        MaximumChanged,

        /// <summary>
        /// Indicates that the Step has been changed.
        /// </summary>
        StepChanged,

        /// <summary>
        /// Indicates that the Step width has been changed.
        /// </summary>
        StepWidthChanged,

        /// <summary>
        /// Indicates that the Dash property has been changed.
        /// </summary>
        DashChanged,

        /// <summary>
        /// Indicates that the Hatch property has been changed.
        /// </summary>
        HatchChanged,

        /// <summary>
        /// Indicates that the IntegralDash property has been changed.
        /// </summary>
        IntegralDashChanged,

        /// <summary>
        /// Indicates that the Text property has been changed.
        /// </summary>
        TextChanged,

        /// <summary>
        /// Indicates that the SeparatorWidth property has been changed.
        /// </summary>
        SeparatorWidthChanged,

        /// <summary>
        /// Indicates that the TextOrientatio property has been changed.
        /// </summary>
        TextOrientationChanged,

        /// <summary>
        /// Indicates that the TextAlignment property has been changed.
        /// </summary>
        TextAlignmentChanged,

        /// <summary>
        /// Indicates that the ProgressOrientation property has been changed.
        /// </summary>
        ProgressOrientationChanged,

        /// <summary>
        /// Indicates that the ProgressOrientation property has been changed.
        /// </summary>
        ShowProgressIndicatorsChanged,

        /// <summary>
        /// Indicates that one of the separator colors property has been changed.
        /// </summary>
        SeparatorColorChanged,

        /// <summary>
        /// Indicates that the separators gradeient angle property has been changed.
        /// </summary>
        SeparatorGradientAngleChanged,

        /// <summary>
        /// Indicates that the separator color stop has changed
        /// </summary>
        SeparatorColorStopChanged,

        /// <summary>
        /// Indicates that the separator number of colors changed.
        /// </summary>
        SeparatorNumberOfColorChanged
    }

    /// <summary>
    ///     Represents event data for some of the progress bar event:
    ///     <see cref="RadProgressBar.ValueChanged">ValueChanged</see>,
    ///     <see cref="RadProgressBar.MinimumChanged">MinimumChanged</see>,
    ///     <see cref="RadProgressBar.MaximumChanged">MaximumChanged</see>,
    ///     <see cref="RadProgressBar.StepChanged">StepChanged</see>,
    ///     <see cref="RadProgressBar.StepWidthChanged">StepWidthChanged</see>,
    ///     <see cref="RadProgressBar.DashChanged">DashChanged</see>,
    ///     <see cref="RadProgressBar.IntegralDashChanged">IntegralDashChanged</see>,
    ///     <see cref="RadProgressBar.TextChanged">TextChanged</see>,
    ///     <see cref="RadProgressBar.SeparatorWidthChanged">SeparatorWidthChanged</see>,
    ///     <see cref="RadProgressBar.TextOrientationChanged">TextOrientationChanged</see>,
    ///     <see cref="RadProgressBar.TextAlignmentChanged">TextAlignmentChanged</see>,
    ///     <see cref="RadProgressBar.ProgressOrientationChanged">ProgressOrientationChanged</see>,
    ///     <see cref="RadProgressBar.ShowProgressIndicatorsChanged">ShowProgressIndicatorsChanged</see> and
    ///     <see cref="RadProgressBar.SeparatorColorChanged">SeparatorColorChanged</see>.
    /// </summary>
    public class ProgressBarEventArgs : EventArgs
    {
        private ProgressBarSenderEvent senderEvent;
        /// <summary>
        /// Initializes a new instance of the ProgressBarEventArgs class using the sender 
        /// of the event.
        /// </summary>
        /// <param name="senderEvent">Represents the event sender.</param>
        public ProgressBarEventArgs(ProgressBarSenderEvent senderEvent)
        {
            this.senderEvent = senderEvent;
        }
        /// <summary>
        /// Gets or sets the event sender.
        /// </summary>
        /// <value>event sender.</value>
        public ProgressBarSenderEvent SenderEvent
        {
            get
            {
                return senderEvent;
            }
            set
            {
                senderEvent = value;
            }
        }

    }

    public class RadProgressBarStyleBuilderData : RadControlDesignTimeData
    {
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            RadProgressBar progressBar = new RadProgressBar();
            progressBar.Size = new Size(300, 40);
            progressBar.Value1 = 66;
            progressBar.Value2 = 33;
            ControlStyleBuilderInfoList styleBuilderInfoList = new ControlStyleBuilderInfoList();
            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(progressBar, progressBar.RootElement);
            designed.MainElementClassName = typeof(RadProgressBarElement).FullName;
            styleBuilderInfoList.Add(designed);

            return styleBuilderInfoList;
        }
    }
}
