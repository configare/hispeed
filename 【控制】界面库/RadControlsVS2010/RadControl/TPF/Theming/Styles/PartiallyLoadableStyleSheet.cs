using System;
using System.IO;
using System.Xml;
using Telerik.WinControls.Themes.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    /// This class supports internal TPF infrastructure and is not ment to be used elsewere.
    /// </summary>
    internal class PartiallyLoadableStyleSheet: StyleSheet
    {
        bool loaded = false;
        private string partiallyLoadedXmlData = null;
        private XmlStyleSheet loadedXmlStyleSheet = null;
        private IPropertiesProvider propertiesProvider;
        private static object syncRoot = new object();

        public PartiallyLoadableStyleSheet(IPropertiesProvider propertiesProvider, string partiallyLoadedXmlData, string themeLocation): base(themeLocation)
        {
            this.propertiesProvider = propertiesProvider;
            this.partiallyLoadedXmlData = partiallyLoadedXmlData;
        }

        public PartiallyLoadableStyleSheet(XmlStyleSheet loadedXmlStyleSheet, string themeLocation)
            : base(themeLocation)
        {
            this.loadedXmlStyleSheet = loadedXmlStyleSheet;
        }

        /// <summary>
        /// Occurs when the PropertySettingGroups are finally loaded from the underlaying data
        /// </summary>
        public event EventHandler LoadedCompletely;

        public override PropertySettingGroupCollection PropertySettingGroups
        {
            get
            {
                if (!this.loaded)
                {
                    this.Deserialize();
                }

                return base.PropertySettingGroups;
            }
        }

        private void Deserialize()
        {
            //ensure thread safety
            lock (syncRoot)
            {
                if (!string.IsNullOrEmpty(this.partiallyLoadedXmlData))
                {
                    XmlStyleSheet xmlStyleSheet = new XmlStyleSheet();
                    using (TextReader textReader = new StringReader(this.partiallyLoadedXmlData))
                    {
                        using (XmlReader reader = XmlReader.Create(textReader))
                        {
                            StyleXmlSerializer ser = new StyleXmlSerializer(false);
                            ser.PropertiesProvider = this.propertiesProvider;

                            ser.ReadObjectElement(reader, xmlStyleSheet);
                        }
                    }

                    this.partiallyLoadedXmlData = null;

                    base.PropertySettingGroups.Clear();
                    xmlStyleSheet.DeserializePropertySettingGroups(base.PropertySettingGroups);
                }
                else if (loadedXmlStyleSheet != null)
                {
                    base.PropertySettingGroups.Clear();
                    loadedXmlStyleSheet.DeserializePropertySettingGroups(base.PropertySettingGroups);
                    this.loadedXmlStyleSheet = null;
                }

                this.loaded = true;

                if (this.LoadedCompletely != null)
                {
                    this.LoadedCompletely(this, EventArgs.Empty);

                    //this event occurs only once in the lifetime of the object, so remove the event handlers
                    this.LoadedCompletely = null;
                }
            }
        }

        public bool IsLoadedCompletely
        {
            get { return this.loaded; }
        }
    }
}
