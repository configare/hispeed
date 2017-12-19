using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Themes.Serialization;
using Telerik.WinControls.XmlSerialization;
using Telerik.WinControls.Themes;
using System.Diagnostics;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a theme for a telerik control. Themes can be serialized and 
    /// deserialized, thus saving and loading the theme for a given control.
    /// XmlTheme implements IXmlSerializable which provides custom formatting for 
    /// XML serialization and deserialization.
    /// </summary>
    /// Removed the Serializable attribute since many of the classes used in this class are also not serializable.
    /// This causes the deserialization of the theme to fail when requested in the Visual Studio designer from the
    /// theme components context menu.
    ///[Serializable]
    [XmlInclude(typeof(XmlStyleBuilderRegistration))]
    public class XmlTheme: IXmlSerializable
    {
        private string themeVersion = "1.0";
        private string themeName;
        private XmlStyleBuilderRegistration[] builderRegistrations;
        private ThemePropertyCollection themeProperties = new ThemePropertyCollection();
        private XmlStyleRepository repository;

        /// <summary>
        /// Initializes a new instance of the XmlTheme class.
        /// </summary>
        public XmlTheme()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlTheme class from an existing theme.
        /// </summary>
        /// <param name="theme"></param>
        public XmlTheme(Theme theme)
        {
            this.themeName = theme.ThemeName;

            foreach (KeyValuePair<string, object> entry in theme.ThemeProperties)
            {
                this.ThemeProperties.Add(entry);
            }

            ArrayList list = new ArrayList();
            foreach (StyleBuilderRegistration registration in theme.GetRegisteredStyleBuilders())
            {
                XmlStyleBuilderRegistration xmlReg = new XmlStyleBuilderRegistration(registration);
                list.Add(xmlReg);
            }

            builderRegistrations = new XmlStyleBuilderRegistration[list.Count];
            if ( list.Count > 0 )
            {                
                list.CopyTo(builderRegistrations, 0);
            }

            this.repository = theme.Repository;
        }
        /// <summary>
        /// Initializes a new instance of the XmlTheme class from XmlStyleSheet, 
        /// control type, and element type.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="controlType"></param>
        /// <param name="elementType"></param>
        public XmlTheme(XmlStyleSheet style, string controlType, string elementType)
        {
            this.themeName = "";
            this.builderRegistrations = new XmlStyleBuilderRegistration[] 
            { 
                new XmlStyleBuilderRegistration(style, controlType, elementType)
            };
        }

        /// <summary>
        /// Get the StyleRepository associated with this theme.
        /// <remarks>
        /// StyleReposity contains named lists of PropertySettings, reffered by Key property, that can be inherited by the PropertySettingGroups of this theme.
        /// This is done by associating BsedOn property of the property setting group with PropertySettings list key.
        /// Since each theme can have only one repository, when different XmlTheme are registered with repositories <see cref="ThemeResolutionService "/> for the same theme 
        /// the repositories are merged. If a PropertySettings list with the same Key is defined in several XmlTheme repository instances, the last laoded one overrides 
        /// any existing list.
        /// </remarks>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XmlStyleRepository StyleRepository
        {
            get
            {
                if (this.repository == null)
                {
                    repository = new XmlStyleRepository();
                }

                return repository;
            }            
        }

        /// <summary>
        /// Gets value indicating whether this XmlTheme defines StyleRepository
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRepository 
        { 
            get
            {
                return this.repository != null && this.repository.RepositoryItems.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets a string value indicating the theme name.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(null)]
        public string ThemeName
        {
            get { return themeName; }
            set { themeName = value; }
        }

        [XmlAttribute]
        [DefaultValue("1.0")]
        public string ThemeVersion
        {
            get
            {
                return this.themeVersion;
            }
            set
            {
                this.themeVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the Builder Registration for the theme. Each builder registration
        /// corresponds to a theme for single telerik control.
        /// </summary>
        [DefaultValue(null)]
        public XmlStyleBuilderRegistration[] BuilderRegistrations
        {
            get { return builderRegistrations; }
            set { builderRegistrations = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ThemePropertyCollection ThemeProperties
        {
            get { return themeProperties; }
        }

        /// <summary>
        /// Retrieves the serialization string of the given type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeType(Type value)
        {
            return value.FullName;
        }

        /// <summary>
        /// Deserializes the provided deserialization string.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type DeserializeType(string className)
        {
            Type objectType = Type.GetType(className);
            if (objectType == null)
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    objectType = asm.GetType(className);
                    if (objectType != null)
                    {
                        break;
                    }
                }

                if (objectType == null)
                {
                    throw new ArgumentException("Type not found during theme deserialization: " + className + ". Please make sure all required asseblies are referenced.");
                }
            }

            return objectType;
        }

        private ThemeSource themeSource;
        //private bool loadSucceeded;
        private Assembly callingAssembly;
        
        /// <summary>
        /// Loads a theme partially from a System.IO.Stream instance. 
        /// </summary>        
        public void LoadPartiallyFromStorage(ThemeSource themeSource)
        {
            this.themeSource = themeSource;

            themeSource.loadSucceeded = false;

            if (themeSource.OwnerThemeManager != null &&
                themeSource.OwnerThemeManager.IsDesignMode &&
                themeSource.OwnerThemeManager.Site != null)
            {
                try
                {
                    if (LoadThemeInDesingMode(themeSource))
                        return;
                }
                catch (Exception e) //catch any COM or other exceptions if not working with VS 2005
                {
                    themeSource.loadError = e.ToString();
                    return;
                }
            }
            
            if (this.themeSource != null)
            {
                this.callingAssembly = this.themeSource.GetInitialCallingAssembly();
            }

            if (this.callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }

            if (this.themeSource.StorageType == ThemeStorageType.File)
            {
                string themeLoc = this.GetThemeLocation();
                try
                {
                    using (XmlReader reader = XmlReader.Create(themeLoc))
                    {
                        this.DeserializePartiallyThemeFromReader(reader, themeLoc);
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = "Error loading theme from file  " + themeLoc + ": " + ex.ToString();

                    Debug.Fail(errorMessage);
#if !DEBUG
                    Trace.Write(errorMessage);
#endif

                    themeSource.loadError = errorMessage;
                }
            }
            else if (this.themeSource.StorageType == ThemeStorageType.Resource)
            {
                try
                {
                    themeSource.loadError = "Resource not found";

                    string themeLocation = this.themeSource.ThemeLocation.Trim();
                    Stream stream = callingAssembly.GetManifestResourceStream(themeLocation);

                    if (stream == null)
                    {
                        stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(themeLocation);
                    }

                    if (stream == null)
                    {
                        Assembly asm = Assembly.GetEntryAssembly();
                        //there is not any entry assembly in design mode!
                        if (asm != null)
                        {
                            stream = asm.GetManifestResourceStream(themeLocation);
                        }
                    }

                    if (stream != null)
                    {
                        using (stream)
                        {
                            using (XmlReader reader = XmlReader.Create(stream))
                            {
                                this.DeserializePartiallyThemeFromReader(reader, themeLocation);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = "Error loading theme from resource " + this.themeSource.ThemeLocation + ": " +
                                          ex.ToString();

                    Debug.Fail(errorMessage);
#if !DEBUG
                    Trace.Write(errorMessage);
#endif
                    themeSource.loadError = errorMessage;
                }
            }

            themeSource.loadSucceeded = true;
            themeSource.loadError = string.Empty;
            callingAssembly = null;
        }

        private bool LoadThemeInDesingMode(ThemeSource themeSource)
        {
            ProjectManagement pm = ProjectManagement.GetProjectManagementInstance(themeSource.OwnerThemeManager.Site);
            string projectFullPath = pm.Services.GetActiveProjectFullPath();
            
            //return true - means no further processing required
			if (!themeSource.SettingsAreValid)
			{
				return true;
			}

            string baseFolder = null;
            string fileName = null;

            string outputPath = (string) pm.Services.GetProjectConfigurationPropertyValue("OutputPath");

            if (string.IsNullOrEmpty(projectFullPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            baseFolder = Path.Combine(projectFullPath, outputPath);

            string validFilePath = null;

            if (themeSource.StorageType == ThemeStorageType.File)
            {
                if (Path.IsPathRooted(themeSource.ThemeLocation))
                    return false;

                fileName = themeSource.ThemeLocation;

                if (baseFolder != null && !string.IsNullOrEmpty(fileName))
                {
                    fileName = fileName.Replace("~\\", "");
                    fileName = fileName.Replace("~", "");
                    validFilePath = Path.Combine(baseFolder, fileName);

                    if (!File.Exists(validFilePath))
                    {
                        themeSource.loadError = "Path not found: " + validFilePath;
                        return true;
                    }
                }
            }
            else if (themeSource.StorageType == ThemeStorageType.Resource)
            {
                string themeLocation = themeSource.ThemeLocation;
                string[] fileNameParts = themeLocation.Split('.');

                validFilePath = SearchFile(projectFullPath, fileNameParts);

                if (validFilePath == null)
                {
                    themeSource.loadError = 
                        string.Format(                        
                        "Unable locate Resource file '{0}' in the project folder '{1}'",
                        string.Join(Path.DirectorySeparatorChar.ToString(), themeLocation.Split('.')),
                        projectFullPath
                        );

                    return true;
                }
            }

            if (validFilePath == null)
            {
                themeSource.loadError = "Unable to determine active project path.";
                return true;
            }

            using (XmlReader reader = XmlReader.Create(validFilePath))
            {
                this.DeserializePartiallyThemeFromReader(reader, validFilePath);
            }

            themeSource.loadSucceeded = true;

            return true;
        }

        private static string SearchFile(string baseFolder, string[] fileNameParts)
        {
            if (fileNameParts == null || fileNameParts.Length < 2)
                    return null;

            int i = fileNameParts.Length - 2;
            string fileName = string.Join(".", fileNameParts, i, 2);

            //part with index 0 is always project name
            do
            {
                string validFilePath = Path.Combine(baseFolder, fileName);                
                if (File.Exists(validFilePath))
                {
                    return validFilePath;
                }
                
                i--;
                if (i > 0)
                    fileName = fileNameParts[i] + Path.DirectorySeparatorChar + fileName;
            } while (i > 0);


            return SearchFileInSubDirectories(baseFolder, fileName);
        }

        private static string SearchFileInSubDirectories(string baseFolder, string fileName)
        {
            foreach(string folders in Directory.GetDirectories(baseFolder))
            {
                string file = Path.Combine(folders, fileName);
                if( File.Exists( file ))
                {
                    return file;
                }
            }

            return null;
        }

        internal void DeserializePartiallyThemeFromReader(XmlReader reader)
        {
            this.DeserializePartiallyThemeFromReader(reader, null);
        }

        private void DeserializePartiallyThemeFromReader(XmlReader reader, string themeLocation)
        {
            if (reader.ReadToFollowing("XmlTheme"))
            {
                new StyleXmlSerializer(true).ReadObjectElement(reader, this);
                reader.ReadEndElement();
            }
            else
            {
                MessageBox.Show("Error reading theme: element XmlTheme not found in the xml file '" + this.themeSource.ThemeLocation + "'");
            }

            if (this.builderRegistrations == null)
            {
                return;
            }

            foreach (XmlStyleBuilderRegistration reg in this.BuilderRegistrations)
            {
                XmlStyleSheet styleSheet = (reg.BuilderData as XmlStyleSheet);
                if (styleSheet != null)
                {
                    if (themeLocation != null)
                    {
                        styleSheet.SetThemeLocation(themeLocation);
                    }
                    styleSheet.SetThemeName(this.ThemeName);
                }
            }
        }

        private string GetThemeLocation()
        {
            if (string.IsNullOrEmpty(this.themeSource.ThemeLocation))
            {
                return string.Empty;
            }

            return this.themeSource.ThemeLocation.Replace("~", System.Windows.Forms.Application.StartupPath);
        }

        /// <summary>
        /// Loads a theme from a System.IO.Stream instance. 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XmlTheme LoadFromStram(Stream stream)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlTheme));
            XmlTheme xmlTheme = (XmlTheme)ser.Deserialize(stream);

            return xmlTheme;
        }

        /// <summary>
        /// Load a XML theme from a TextReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static XmlTheme LoadFromReader(TextReader reader)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlTheme));
            XmlTheme xmlTheme = (XmlTheme)ser.Deserialize(reader);

            return xmlTheme;
        }
        /// <summary>
        /// Loads a theme from a XML reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static XmlTheme LoadFromReader(XmlReader reader)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlTheme));
            XmlTheme xmlTheme = (XmlTheme)ser.Deserialize(reader);

            return xmlTheme;
        }
        /// <summary>
        /// Saves the theme to a XMLWriter.
        /// </summary>
        /// <param name="writer"></param>
        public void SaveToWriter(XmlWriter writer)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlTheme));
            ser.Serialize(writer, this);
        }

		public void SaveToStream(Stream stream)
		{
			XmlTextWriter textWriter = new XmlTextWriter(stream, Encoding.UTF8);
			textWriter.Formatting = Formatting.Indented;

			this.SaveToWriter(textWriter);
		}

        #region IXmlSerializable implementation

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            //reader.Read();
            new StyleXmlSerializer().ReadObjectElement(reader, this);
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(writer.LookupPrefix("http://www.w3.org/2001/XMLSchema-instance")))
            {
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            }
            //xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            new StyleXmlSerializer().WriteObjectElement(writer, this);
        }

        #endregion
        /// <summary>
        /// Merges the original theme given as the first parameter with the theme of the
        /// current instance and writes the union to an XMLWriter.
        /// </summary>
        /// <param name="originalTheme"></param>
        /// <param name="writer"></param>
		public void SaveMergeToWriter(XmlTheme originalTheme, XmlWriter writer)
		{			
			if (originalTheme.BuilderRegistrations != null)
			{
				List<XmlStyleBuilderRegistration> resList = new List<XmlStyleBuilderRegistration>(originalTheme.BuilderRegistrations);
				foreach (XmlStyleBuilderRegistration available in this.BuilderRegistrations)
				{
					int foundIndex = 0;
					bool found = false;
					foreach (XmlStyleBuilderRegistration currOriginalReg in originalTheme.BuilderRegistrations)
					{
						//test if this is the stylesheet
						foreach (RadStylesheetRelation relation in available.StylesheetRelations)
						{
							foreach (RadStylesheetRelation originalRelation in currOriginalReg.StylesheetRelations)
							{
								if (originalRelation.RegistrationType == relation.RegistrationType &&
								    this.CompareRelationAttributes(originalRelation.ElementType, relation.ElementType) &&
								    this.CompareRelationAttributes(originalRelation.ElementName, relation.ElementName) &&
								    this.CompareRelationAttributes(originalRelation.ControlName, relation.ControlName) &&
								    this.CompareRelationAttributes(originalRelation.ControlType, relation.ControlType))
								{
									found = true;
									break;
								}
							}

							if (found)
								break;
						}

						if (found)
						{
							//Now get the relations from the existing theme and merge them with the new one							
							foreach (RadStylesheetRelation originalRelation in currOriginalReg.StylesheetRelations)
							{
								bool relationFound = false;
								foreach (RadStylesheetRelation relation in available.StylesheetRelations)
								{
									if (originalRelation.RegistrationType == relation.RegistrationType &&
									    this.CompareRelationAttributes(originalRelation.ElementType, relation.ElementType) &&
									    this.CompareRelationAttributes(originalRelation.ElementName, relation.ElementName) &&
									    this.CompareRelationAttributes(originalRelation.ControlName, relation.ControlName) &&
									    this.CompareRelationAttributes(originalRelation.ControlType, relation.ControlType))
									{
										relationFound = true;
										break;
									}
								}

								if (!relationFound)
								{
									available.StylesheetRelations.Add(originalRelation);
								}
							}

							//break searching the stylesheet
							break;
						}

						foundIndex++;
					}

					if (found)
						resList[foundIndex] = available;
					else
						resList.Add(available);
				}

				this.BuilderRegistrations = new XmlStyleBuilderRegistration[resList.Count];

				resList.CopyTo(this.BuilderRegistrations, 0);
			}

			this.SaveToWriter(writer);
		}

        private bool CompareRelationAttributes(string attribure1, string attribure2)
        {
            return attribure1 == attribure2 ||
                   attribure1 == null && attribure2 == string.Empty ||
                   attribure1 == string.Empty && attribure2 == null;
        }        
    }        
}
