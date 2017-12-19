using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Telerik.WinControls;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a dialog containing a color picker
    /// </summary>
	public partial class RadColorDialogForm : RadForm, IRadColorDialog
    {
        #region Fields

        private IColorSelector colorSelector;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Creates instance of RadColorDialog class
        /// </summary>
		public RadColorDialogForm()
		{
            colorSelector = RadColorEditor.CreateColorSelectorInstance();
			UserControl colorSelectorControl = colorSelector as UserControl;

			colorSelector.SelectedColor = System.Drawing.Color.Red;
			colorSelector.OkButtonClicked += delegate(object sender, ColorChangedEventArgs args) 
			{ 
				this.DialogResult = DialogResult.OK;
				this.Close();
			};
			colorSelector.CancelButtonClicked += delegate(object sender, ColorChangedEventArgs args) 
			{ 
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			};

			colorSelectorControl.Dock = DockStyle.Fill;
			this.Controls.Add(colorSelectorControl);

			InitializeComponent();
		}

        #endregion

        #region Properties

        /// <summary>
		/// Gets the color selector
		/// </summary>
		public UserControl RadColorSelector
		{
			get
			{
				return colorSelector as UserControl;
			}
		}

		/// <summary>
		/// Gets or sets the selected color
		/// </summary>
		public Color SelectedColor
		{
			get { return colorSelector.SelectedColor;}
			set { colorSelector.SelectedColor = value;}
		}

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        public HslColor SelectedHslColor
        {
            get { return colorSelector.SelectedHslColor; }
            set { colorSelector.SelectedHslColor = value; }
        }

		/// <summary>
		/// Gets or sets the old color
		/// </summary>
		public Color OldColor
		{
			get { return colorSelector.OldColor;}
			set { colorSelector.OldColor = value;}
		}

		/// <summary>
		/// Gets or sets the active mode of the color tabstrip
		/// </summary>
		public ColorPickerActiveMode ActiveMode
		{
			get { return colorSelector.ActiveMode; }
			set { colorSelector.ActiveMode = value;} 
		}

		/// <summary>
		/// Shows or hides the basic colors tab
		/// </summary>
		public bool ShowBasicColors
		{
			get { return colorSelector.ShowBasicColors; }
			set { colorSelector.ShowBasicColors = value; }
		}

		/// <summary>
		/// Shows or hides the system colors tab
		/// </summary>
		public bool ShowSystemColors 
		{
			get { return colorSelector.ShowSystemColors; }
			set { colorSelector.ShowSystemColors = value; }
		}

		/// <summary>
		/// Shows or hides the web colors tab
		/// </summary>
		public bool ShowWebColors 
		{
			get { return colorSelector.ShowWebColors; }
			set { colorSelector.ShowWebColors = value; }
		}

		/// <summary>
		/// Shows or hides whe professional colors tab
		/// </summary>
		public bool ShowProfessionalColors 
		{
			get { return colorSelector.ShowProfessionalColors; }
			set { colorSelector.ShowProfessionalColors = value; }
		}

		/// <summary>
		/// Shows or hides the custom colors tab
		/// </summary>
		public bool ShowCustomColors 
		{
			get { return colorSelector.ShowCustomColors; }
			set { colorSelector.ShowCustomColors = value; }
		}

		/// <summary>
		/// Shows or hides the hex color value
		/// </summary>
		public bool ShowHEXColorValue 
		{
			get { return colorSelector.ShowHEXColorValue; }
			set { colorSelector.ShowHEXColorValue = value; }
		}

		/// <summary>
		/// Allows or disallows editing the HEX value
		/// </summary>
		public bool AllowEditHEXValue 
		{
			get { return colorSelector.AllowEditHEXValue; }
			set { colorSelector.AllowEditHEXValue = value; }
		}

		/// <summary>
		/// Allows or disallows color picking from the screen
		/// </summary>
		public bool AllowColorPickFromScreen 
		{
			get { return colorSelector.AllowColorPickFromScreen; }
			set { colorSelector.AllowColorPickFromScreen = value; }
		}

		/// <summary>
		/// Allows or disallows color saving
		/// </summary>
		public bool AllowColorSaving 
		{
			get { return colorSelector.AllowColorSaving; }
			set { colorSelector.AllowColorSaving = value; }
		}

		/// <summary>
		/// Gets the custom colors
		/// </summary>
		public Color[] CustomColors 
		{
			get { return colorSelector.CustomColors; } 
		}

		/// <summary>
		/// Gets or sets the heading of the basic colors tab
		/// </summary>
		public string BasicTabHeading 
		{
			get { return colorSelector.BasicTabHeading; }
			set { colorSelector.BasicTabHeading = value; }
		}

		/// <summary>
		/// Gets or sets the heading of the system colors tab
		/// </summary>
		public string SystemTabHeading 
		{
			get { return colorSelector.SystemTabHeading; }
			set { colorSelector.SystemTabHeading = value; }
		}

		/// <summary>
		/// Gets or sets the heading of the web colors tab
		/// </summary>
		public string WebTabHeading 
		{
			get { return colorSelector.WebTabHeading; }
			set { colorSelector.WebTabHeading = value; }
		}

		/// <summary>
		/// Gets or sets the heading of the professional colors tab
		/// </summary>
		public string ProfessionalTabHeading 
		{
			get { return colorSelector.ProfessionalTabHeading; }
			set { colorSelector.ProfessionalTabHeading = value; }
		}

		/// <summary>
		/// Gets or sets the heading of the selected color label
		/// </summary>
		public string SelectedColorLabelHeading 
		{
			get { return colorSelector.SelectedColorLabelHeading; }
			set { colorSelector.SelectedColorLabelHeading = value; }
		}

		/// <summary>
		/// Gets or sets the heading of the old color label
		/// </summary>
		public string OldColorLabelHeading 
		{
			get { return colorSelector.OldColorLabelHeading; }
			set { colorSelector.OldColorLabelHeading = value; }
		}

		/// <summary>
		/// Fires when the selected color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

        #endregion

        #region Event handlers

        protected virtual void OnColorChanged(ColorChangedEventArgs e)
		{
			ColorChangedEventHandler handler = this.ColorChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
            if (keyData == Keys.Return)
            {
                this.DialogResult = DialogResult.OK;
            }

			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		void RadColorDialog_Load(object sender, EventArgs e)
		{
			this.KeyDown += new KeyEventHandler(RadColorDialog_KeyDown);
        }

		void RadColorDialog_KeyDown(object sender, KeyEventArgs e)
		{
			MessageBox.Show(e.KeyCode.ToString());
        }
        
        #endregion
    }
}