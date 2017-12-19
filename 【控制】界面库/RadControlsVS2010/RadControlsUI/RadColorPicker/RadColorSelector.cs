using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Themes.ColorDialog;
using Telerik.WinControls.UI.RadColorPicker;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a color selector control
    /// </summary>    
    [DefaultEvent("ColorChanged")]
    [DefaultProperty("SelectedColor")]
    [ToolboxItem(false)]
    [Description("Enables users to select colors from presets and the continuous RgbValue or HSL color spaces using a highly customizable interface")]
    public partial class RadColorSelector : UserControl, IColorSelector
    {
        private ColorPickerActiveMode colorPickerActiveMode = ColorPickerActiveMode.Basic;
        private CaptureBox captureBox = new CaptureBox();
        private HslColor selectedHslColor;
        private Color selectedColor;
        private bool supressTextBoxColorChange;

        /// <summary>
        /// Fires when custom colors configuration is about to be saved or loaded.
        /// Can be used to change the default location of the configuration file.
        /// </summary>
        public event CustomColorsEventHandler CustomColorsConfigLocationNeeded;


        /// <summary>
        /// Fires when the selected color changes
        /// </summary>
        public event ColorChangedEventHandler ColorChanged;

        /// <summary>
        /// Fires when the OK button is clicked
        /// </summary>
        public event ColorChangedEventHandler OkButtonClicked;

        /// <summary>
        /// Fires when the Cancel button is clicked
        /// </summary>
        public event ColorChangedEventHandler CancelButtonClicked;

        public RadColorSelector()
        {
            InitializeComponent();
            this.Controls.Add(captureBox);
            captureBox.ColorChanged += colorDialog_ColorChanged;
            customColors.ColorChanged += colorDialog_ColorChanged;
            customColors.CustomColorsConfigLocationNeeded += new CustomColorsEventHandler(customColors_CustomColorsConfigLocationNeeded);

            professionalColorsControl.ColorChanged += colorDialog_ColorChanged;
            professionalColorsControl.MouseDoubleClick += new MouseEventHandler(professionalColorsControl_MouseDoubleClick);
            ColorDialogLocalizationProvider.CurrentProviderChanged += ColorDialogLocalizationProvider_CurrentProviderChanged;

            ((RadPageViewStripElement)this.radPageView1.ViewElement).StripButtons = StripViewButtons.None;
            ((RadPageViewStripElement)this.radPageView1.ViewElement).DrawFill = false;
            ((RadPageViewStripElement)this.radPageView1.ViewElement).DrawBorder = false;
        }
        
        void ColorDialogLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            this.newLabel.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogNewColorLabel);
            this.currentLabel.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogCurrentColorLabel);
            this.btnAddNewColor.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogAddCustomColorButton);
            this.radPageViewPage1.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogBasicTab);
            this.radPageViewPage2.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogSystemTab);
            this.radPageViewPage3.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogWebTab);
            this.radPageViewPage4.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogProfessionalTab);
            this.radButton3.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogCancelButton);
            this.radButton1.Text = ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(ColorDialogStringId.ColorDialogOKButton);
        }


        public RadPageView ControlsHolderPageView
        {
            get
            {
                return this.radPageView1;
            }
        }

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory),
        Description("Gets or sets the new color.")]
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                if (selectedColor != value)
                {
                    this.professionalColorsControl.ColorRgb = value;
                    this.labelColor.BackColor = value;
                    this.labelOldColor.BackColor = value;
                    this.selectedHslColor = HslColor.FromColor(value);
                    this.selectedColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory),
        Description("Gets or sets the new RGB color.")]
        public Color SelectedRgbColor
        {
            get
            {
                return selectedHslColor.RgbValue;
            }
            set
            {
                if (selectedHslColor.RgbValue != value)
                {
                    this.SelectedColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected HSL color
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory),
        Description("Gets or sets selected HSL color.")]
        public HslColor SelectedHslColor
        {
            get
            {
                return this.selectedHslColor;
            }
            set
            {
                if (selectedHslColor != value)
                {
                    this.SelectedColor = value.RgbValue;
                }
            }
        }

        private void SetSelectedColor(Color color)
        {
            this.selectedHslColor = HslColor.FromColor(color);
            this.selectedColor = color;
            this.labelColor.BackColor = color;
        }

        /// <summary>
        /// Gets or sets the old color
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory),
        Description("Gets or sets the old color.")]
        public Color OldColor
        {
            get
            {
                return labelOldColor.BackColor;
            }
            set
            {
                labelOldColor.BackColor = value;
            }
        }

        /// <summary>
        /// Gets the list of custom colors
        /// </summary>
        public Color[] CustomColors
        {
            get
            {
                return customColors.Colors;
            }
        }

        /// <summary>
        /// Shows or hides the web colors tab
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory), DefaultValue(true),
        Description("Shows or hides the web colors tab.")]
        public bool ShowWebColors
        {
            get
            {
                return radPageView1.Pages[2].Item.Visibility == ElementVisibility.Visible;
            }
            set
            {
                //in case the other tabs are hidden, do not allow this one to be hidden
                if (radPageView1.Pages[0].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[1].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[3].Item.Visibility != ElementVisibility.Visible)
                    return;

                //update the tabstrip and set active item
                if (value)
                {
                    radPageView1.Pages[2].Item.Visibility = ElementVisibility.Visible;
                    radPageView1.SelectedPage = radPageView1.Pages[2];
                }
                else
                {
                    bool reselect = object.ReferenceEquals(radPageView1.Pages[2], radPageView1.SelectedPage);
                    radPageView1.Pages[2].Item.Visibility = ElementVisibility.Collapsed;

                    if (reselect)
                    {
                        if (radPageView1.Pages[3].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[3];
                        else if (radPageView1.Pages[1].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[1];
                        else if (radPageView1.Pages[0].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[0];
                    }
                }
            }
        }

        /// <summary>
        /// Shows or hides the basic colors tab
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory), DefaultValue(true),
        Description("Shows or hides the basic colors tab.")]
        public bool ShowBasicColors
        {
            get
            {
                return radPageView1.Pages[0].Item.Visibility == ElementVisibility.Visible;
            }
            set
            {
                //in case the other tabs are hidden, do not allow this one to be hidden
                if (radPageView1.Pages[1].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[2].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[3].Item.Visibility != ElementVisibility.Visible)
                    return;

                //update the tabstrip and set active item
                if (value)
                {
                    radPageView1.Pages[0].Item.Visibility = ElementVisibility.Visible;
                    radPageView1.SelectedPage = radPageView1.Pages[0];
                }
                else
                {
                    bool reselect = object.ReferenceEquals(radPageView1.Pages[0], radPageView1.SelectedPage);
                    radPageView1.Pages[0].Item.Visibility = ElementVisibility.Collapsed;

                    if (reselect)
                    {
                        if (radPageView1.Pages[1].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[1];
                        else if (radPageView1.Pages[2].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[2];
                        else if (radPageView1.Pages[3].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[3];
                    }
                }
            }
        }

        /// <summary>
        /// Shows or hides the system colors tab
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory), DefaultValue(true),
        Description("Shows or hides the system colors tab.")]
        public bool ShowSystemColors
        {
            get
            {
                return radPageView1.Pages[1].Item.Visibility == ElementVisibility.Visible;
            }
            set
            {
                //in case the other tabs are hidden, do not allow this one to be hidden
                if (radPageView1.Pages[0].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[2].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[3].Item.Visibility != ElementVisibility.Visible)
                    return;

                //update the tabstrip and set active item
                if (value)
                {
                    radPageView1.Pages[1].Item.Visibility = ElementVisibility.Visible;
                    radPageView1.SelectedPage = radPageView1.Pages[1];
                }
                else
                {
                    bool reselect = object.ReferenceEquals(radPageView1.Pages[1], radPageView1.SelectedPage);
                    radPageView1.Pages[1].Item.Visibility = ElementVisibility.Collapsed;

                    if (reselect)
                    {
                        if (radPageView1.Pages[2].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[2];
                        else if (radPageView1.Pages[0].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[0];
                        else if (radPageView1.Pages[3].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[3];
                    }
                }
            }
        }

        /// <summary>
        /// Shows or hides the professional colors tab
        /// </summary>
        [Category(RadDesignCategory.LayoutCategory), DefaultValue(true),
        Description("Shows or hides the professional colors tab.")]
        public bool ShowProfessionalColors
        {
            get
            {
                return radPageView1.Pages[3].Item.Visibility == ElementVisibility.Visible;
            }
            set
            {
                //in case the other tabs are hidden, do not allow this one to be hidden
                if (radPageView1.Pages[0].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[1].Item.Visibility != ElementVisibility.Visible && 
                    radPageView1.Pages[2].Item.Visibility != ElementVisibility.Visible)
                    return;

                //update the tabstrip and set active item
                if (value)
                {
                    radPageView1.Pages[3].Item.Visibility = ElementVisibility.Visible;
                    radPageView1.SelectedPage = radPageView1.Pages[3];
                }
                else
                {
                    bool reselect = object.ReferenceEquals(radPageView1.Pages[3], radPageView1.SelectedPage);
                    radPageView1.Pages[3].Item.Visibility = ElementVisibility.Collapsed;

                    if (reselect)
                    {
                        if (radPageView1.Pages[0].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[0];
                        else if (radPageView1.Pages[1].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[1];
                        else if (radPageView1.Pages[2].Item.Visibility == ElementVisibility.Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[2];
                    }
                }
            }
        }

        /// <summary>
        /// Shows or hides the system colors tab
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory), DefaultValue(true),
        Description("Shows or hides the custom colors panel.")]
        public bool ShowCustomColors
        {
            get
            {
                return customColors.Visible;
            }
            set
            {
                customColors.Visible = value;
                btnAddNewColor.Visible = value;
            }
        }

        /// <summary>
        /// Shows or hides the hex color textbox
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory), DefaultValue(true),
        Description("Shows or hides the HEX color box.")]
        public bool ShowHEXColorValue
        {
            get
            {
                return textBoxColor.Visible;
            }
            set
            {
                hexHeadingLabel.Visible = value;
                textBoxColor.Visible = value;
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether the user can edit the hexadecimal color value
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory), DefaultValue(true),
        Description("Allows or disallows editing the HEX value.")]
        public bool AllowEditHEXValue
        {
            get
            {
                return !textBoxColor.ReadOnly;
            }
            set
            {
                textBoxColor.ReadOnly = !value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the user can pick a color from the screen
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory), DefaultValue(true),
        Description("Allows or disallows picking a color from screen.")]
        public bool AllowColorPickFromScreen
        {
            get
            {
                return btnScreenColorPick.Visible;
            }
            set
            {
                btnScreenColorPick.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the user can save colors
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory), DefaultValue(true),
        Description("Allows or disallows saving custom colors.")]
        public bool AllowColorSaving
        {
            get
            {
                return btnAddNewColor.Visible;
            }
            set
            {
                btnAddNewColor.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the add new color button
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        Description("Gets or sets the text of the add new color button.")]
        public string AddNewColorButtonText
        {
            get
            {
                return btnAddNewColor.Text;
            }
            set
            {
                btnAddNewColor.Text = value;
            }
        }

        /// <summary>
        /// Sets or gets the active mode of the RadColorPicker
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(ColorPickerActiveMode.Basic),
        Description("Gets or sets the mode in which the color picker operates.")]
        public ColorPickerActiveMode ActiveMode
        {
            get
            {
                return colorPickerActiveMode;
            }
            set
            {
                colorPickerActiveMode = value;
                switch (colorPickerActiveMode)
                {
                    case ColorPickerActiveMode.Basic:
                        if (radPageView1.Pages[0].Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[0];
                        break;
                    case ColorPickerActiveMode.System:
                        if (radPageView1.Pages[1].Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[1];
                        break;
                    case ColorPickerActiveMode.Web:
                        if (radPageView1.Pages[2].Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[2];
                        break;
                    case ColorPickerActiveMode.Professional:
                        if (radPageView1.Pages[3].Visible)
                            radPageView1.SelectedPage = radPageView1.Pages[3];
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the heading of the basic colors tab
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the basic colors tab.")]
        public string BasicTabHeading
        {
            get
            {
                return radPageView1.Pages[0].Text;
            }
            set
            {
                radPageView1.Pages[0].Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading of the system colors tab
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the system colors tab.")]
        public string SystemTabHeading
        {
            get
            {
                return radPageView1.Pages[1].Text;
            }
            set
            {
                radPageView1.Pages[1].Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading of the web colors tab
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the web colors tab.")]
        public string WebTabHeading
        {
            get
            {
                return radPageView1.Pages[2].Text;
            }
            set
            {
                radPageView1.Pages[2].Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading of the professional colors tab
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the professional colors tab.")]
        public string ProfessionalTabHeading
        {
            get
            {
                return radPageView1.Pages[3].Text;
            }
            set
            {
                radPageView1.Pages[3].Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading of the new color label
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the selected color label.")]
        public string SelectedColorLabelHeading
        {
            get
            {
                return newLabel.Text;
            }
            set
            {
                newLabel.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading of the old color label
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
        Description("Gets or sets the heading of the old color label.")]
        public string OldColorLabelHeading
        {
            get
            {
                return currentLabel.Text;
            }
            set
            {
                currentLabel.Text = value;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                if (OkButtonClicked != null)
                    OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
            else if (keyData == Keys.Escape)
                if (CancelButtonClicked != null)
                    CancelButtonClicked(this, new ColorChangedEventArgs(this.OldColor));
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RadColorDialog_Load(object sender, EventArgs e)
        {
            listBox1.DataSource = ColorProvider.SystemColors;
            listBox1.ColorChanged += colorDialog_ColorChanged;
            listBox2.DataSource = ColorProvider.NamedColors;
            listBox2.ColorChanged += colorDialog_ColorChanged;
            listBox2.MouseDoubleClick += new MouseEventHandler(listBox2_MouseDoubleClick);
            listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_MouseDoubleClick);
            listBox1.KeyDown += new KeyEventHandler(listBox1_KeyDown);
            listBox2.KeyDown += new KeyEventHandler(listBox2_KeyDown);

            discreteColorHexagon.ColorChanged += colorDialog_ColorChanged;
            discreteColorHexagon.MouseDoubleClick += new MouseEventHandler(colorPalette1_MouseDoubleClick);
            discreteColorHexagon.KeyDown += new KeyEventHandler(colorPalette1_KeyDown);

            this.KeyDown += new KeyEventHandler(RadColorDialog_KeyDown);
        }

        /// <summary>
        /// Gets the DiscreteColorHexagon control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DiscreteColorHexagon DiscreteColorHexagon
        {
            get
            {
                return discreteColorHexagon;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == (int)Keys.Return)
            {
                if (OkButtonClicked != null)
                    OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == (int)Keys.Return)
            {
                if (OkButtonClicked != null)
                    OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
        }

        private void colorPalette1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == (int)Keys.Return)
            {
                if (OkButtonClicked != null)
                    OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
        }

        private void RadColorDialog_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.KeyCode.ToString());
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RadElement elementUnderMouse = listBox2.RootElement.ElementTree.GetElementAtPoint(
                listBox2.RootElement, e.Location, null);

            if (elementUnderMouse is RadListBoxItem && OkButtonClicked != null)
            {
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RadElement elementUnderMouse = listBox1.RootElement.ElementTree.GetElementAtPoint(
                listBox1.RootElement, e.Location, null);

            if (elementUnderMouse is RadListBoxItem && OkButtonClicked != null)
            {
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
            }
        }

        private void radarColors1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OkButtonClicked != null)
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
        }

        private void professionalColorsControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OkButtonClicked != null)
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
        }

        private void colorPalette1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OkButtonClicked != null)
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
        }

        private void buttonColorPick_Click(object sender, EventArgs e)
        {
            captureBox.Start();
        }

        private void colorDialog_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (selectedHslColor.IsEmpty)
            {
                this.labelOldColor.BackColor = args.SelectedHslColor.RgbValue;
            }

            if (sender != this.professionalColorsControl)
            {
                this.professionalColorsControl.SetColorSilently(args.SelectedHslColor);
            }

            this.supressTextBoxColorChange = true;
            this.textBoxColor.Text = ColorProvider.ColorToHex(args.SelectedColor);
            this.supressTextBoxColorChange = false;

            this.SetSelectedColor(args.SelectedColor);

            if (ColorChanged != null)
                ColorChanged(this, args);
        }

        //void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //selected tab is the system tab
        //    if (this.radTabStrip1.SelectedTab == tabItem2)
        //        colorDialog_ColorChanged(this, new ColorChangedEventArgs((Color)this.listBox1.SelectedItem));
        //    //selected tab is the web tab
        //    else if (this.radTabStrip1.SelectedTab == tabItem3)
        //        colorDialog_ColorChanged(this, new ColorChangedEventArgs((Color)this.listBox2.SelectedItem));
        //}

        private void customColors_CustomColorsConfigLocationNeeded(object sender, CustomColorsEventArgs args)
        {
            if (this.CustomColorsConfigLocationNeeded != null)
            {
                this.CustomColorsConfigLocationNeeded(sender, args);
            }
        }


        private void textBoxColor_TextChanged(object sender, EventArgs e)
        {
            if (this.supressTextBoxColorChange)
                return;

            Color color = ColorProvider.HexToColor(textBoxColor.Text);

            if (color == Color.Empty)
            {
                return;
            }

            this.SetSelectedColor(color);

            this.professionalColorsControl.SetColorSilently(HslColor.FromColor(color));

            if (ColorChanged != null)
                ColorChanged(this, new ColorChangedEventArgs(color));
        }

        private void radButton4_Click(object sender, EventArgs e)
        {
            customColors.SaveColor(this.labelColor.BackColor);
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (OkButtonClicked != null)
                OkButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            if (CancelButtonClicked != null)
                CancelButtonClicked(this, new ColorChangedEventArgs(this.SelectedColor));
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            //NOTE: this works arround an issue with inheriting the RightToLeft property from parent
            this.professionalColorsControl.RightToLeft = RightToLeft.No;
            this.professionalColorsControl.RightToLeft = RightToLeft.Yes;
            this.professionalColorsControl.RightToLeft = this.RightToLeft;

            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.listBox1.RightToLeft = this.RightToLeft;

            this.listBox2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.listBox2.RightToLeft = this.RightToLeft;
        }
    }
}
