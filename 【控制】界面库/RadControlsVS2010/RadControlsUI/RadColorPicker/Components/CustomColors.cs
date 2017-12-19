using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Telerik.WinControls.Themes.ColorDialog;

namespace Telerik.WinControls.UI.RadColorPicker
{
	/// <summary>
	/// A panel holding a collection of saved colors
	/// </summary>
	[ToolboxItem(false)]
    public partial class CustomColors : UserControl
	{
		private int selectedPanelIndex = -1;
		private Color[] customColors = new Color[10];
		private Panel[] colorPanels = new Panel[10];
		private string configFilename = "custom_colors.cfg";
        private string configLocation;
		/// <summary>
		/// Fires when the selected color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

        /// <summary>
        /// Fires when custom colors configuration is about to be saved or loaded.
        /// Can be used to change the default location of the configuration file.
        /// </summary>
        public event CustomColorsEventHandler CustomColorsConfigLocationNeeded;


		public CustomColors()
		{
			
			InitializeComponent();

			//initialize custom colors to white
			for (int i = 0; i < customColors.Length; ++i)
				customColors[i] = Color.White;


            this.configLocation = this.GetConfigFileLocation();
            //load the pre-saved colors if they are available
			LoadXML();

			int colorIndex = 0;
			foreach (Control ctrl in this.Controls)
			{
				if (ctrl is Panel)
				{
					ctrl.Click += new EventHandler(ctrl_Click);
					ctrl.BackColor = customColors[colorIndex];
					ctrl.Tag = colorIndex;
					colorPanels[colorIndex] = ctrl as Panel;
					++colorIndex;
				}
			}


		}

        /// <summary>
        /// Safely tries to find the path to the local app data folder.
        /// If no path is found, tries to find the path to the common app data folder.
        /// </summary>
        /// <returns></returns>
        private string GetConfigFileLocation()
        {
            string result = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (result == String.Empty)
            {
                result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            return result;
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.FindForm() != null)
				this.FindForm().FormClosing += new FormClosingEventHandler(CustomColors_FormClosing);
		}

        private void CustomColors_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.SaveXML();
		}

		/// <summary>Serializes the custom colors.</summary>
		public void SaveXML()
		{
            if (this.CustomColorsConfigLocationNeeded != null)
            {
                CustomColorsEventArgs args = new CustomColorsEventArgs(this.configLocation, this.configFilename);

                this.CustomColorsConfigLocationNeeded(this, args);

                this.configLocation = args.ConfigLocation;
                this.configFilename = args.ConfigFilename;
            }

            string fileName = this.configLocation + "\\" + configFilename;
			FileStream fs = new FileStream(fileName, FileMode.Create);
			
			XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument();
			writer.WriteStartElement("Colors");

			for (int i = 0; i < customColors.Length; ++i)
				writer.WriteElementString("Color", customColors[i].ToArgb().ToString());

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
		}

		/// <summary>Deserializes the custom colors.</summary>
		public void LoadXML()
		{

            if (this.CustomColorsConfigLocationNeeded != null)
            {
                CustomColorsEventArgs args = new CustomColorsEventArgs(this.configLocation, this.configFilename);

                this.CustomColorsConfigLocationNeeded(this, args);

                this.configLocation = args.ConfigLocation;
                this.configFilename = args.ConfigFilename;
            }

			string fileName = this.configLocation + "\\" + this.configFilename;
			if (!File.Exists(fileName))
				return;

			FileStream fs = new FileStream(fileName, FileMode.Open);
			XmlTextReader reader = new XmlTextReader(fs);
			int index = 0;
			while (!reader.EOF)
			{
				if (reader.ReadToFollowing("Color"))
				{
					Color newColor = Color.Empty;
					try
					{
						newColor = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
					}
					catch(ArgumentException)
					{
						newColor = Color.White;
					}

					customColors[index] = newColor;
					++index;
					if (index == customColors.Length)
						break;
				}
			}

			reader.Close();
		}

        private void ctrl_Click(object sender, EventArgs e)
		{
			SelectPanel((int)((Panel)sender).Tag);
			//raise an event indicating the selected color has changed
			if (ColorChanged != null)
				ColorChanged(this, new ColorChangedEventArgs(colorPanels[selectedPanelIndex].BackColor));
		}

		private void SelectPanel(int newPanelSelectedIndex)
		{
			//resetting the border of the old selected panel
			if (selectedPanelIndex != -1)
				colorPanels[selectedPanelIndex].BorderStyle = BorderStyle.Fixed3D;

			selectedPanelIndex = newPanelSelectedIndex;
			//setting the new selected panel and setting its border
			colorPanels[selectedPanelIndex].BorderStyle = BorderStyle.FixedSingle;
		}

        /// <summary>
        /// Gets or sets the custom directory path which will be used
        /// when the custom colors XML file is stored on the hard drive.
        /// </summary>
        public string CustomColorsConfigLocation
        {
            get
            {
                return this.configLocation;
            }
        }

		/// <summary>
		/// Gets or sets the index of the currently selected color
		/// </summary>
		public int SelectedColorIndex
		{
			get
			{
				return selectedPanelIndex;
			}
			set
			{
				selectedPanelIndex = value;
				//if the value is invalid, set selected to empty
				if (!(selectedPanelIndex < colorPanels.Length))
					selectedPanelIndex = -1;
			}
		}
		/// <summary>
		/// Save the color to the next color slot
		/// </summary>
		/// <param name="color"></param>
		public void SaveColor(Color color)
		{
			//in case there's no panel selected, select the first panel
			if (selectedPanelIndex == -1)
				selectedPanelIndex = 0;

			customColors[selectedPanelIndex] = color;
			colorPanels[selectedPanelIndex].BackColor = color;
			colorPanels[selectedPanelIndex].Refresh();

			//selecting the next panel in order
			SelectPanel((selectedPanelIndex + 1) % colorPanels.Length);
		}
		/// <summary>
		/// Gets the currently selected color
		/// </summary>
		public Color SelectedPanelColor
		{
			get
			{
				return colorPanels[selectedPanelIndex].BackColor;
			}
		}
		/// <summary>
		/// Gets all the colors in the saved colors collection
		/// </summary>
		public Color[] Colors
		{
			get
			{
				return customColors;
			}
		}
	}
}
