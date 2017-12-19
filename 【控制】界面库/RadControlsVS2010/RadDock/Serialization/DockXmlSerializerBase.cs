using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.XmlSerialization;
using System.ComponentModel;
using System.Collections;
using Telerik.WinControls.UI;
using System.Xml;

namespace Telerik.WinControls.UI.Docking.Serialization
{
    public class DockXmlSerializerBase : ComponentXmlSerializer
    {
        private RadDock dock;
        private Dictionary<string, DockWindow> dockWindows;

		/// <summary>
        /// Creates a new instance of DockXmlSerializer
        /// </summary>
        /// <param name="dock">RadDock component that is used to write DockWindow states and info or to map existing DockWindows when deserializing</param>
        /// <param name="componentSerializationInfo">Serialization info. By default <see cref="RadDock.XmlSerializationInfo"/> could be used.</param>
        public DockXmlSerializerBase(RadDock dock, ComponentXmlSerializationInfo componentSerializationInfo)
            : base(componentSerializationInfo)
        {
            this.dock = dock;

            this.dockWindows = new Dictionary<string, DockWindow>();

            foreach (DockWindow window in this.dock.DockWindows)
            {
                this.dockWindows.Add(window.Name, window);
            }
        }

		protected RadDock Dock
		{
			get
			{
				return dock;
			}
		}

        protected override object MatchExistingItem(XmlReader reader, IList toRead, object parent, PropertyDescriptor parentProperty, string propertyToMatch, string uniquePropertyValue, System.Collections.IList existingInstancesToMatch, ref int foundAtIndex)
        {
            foundAtIndex = -1;

            object res = null;

            if (string.IsNullOrEmpty(uniquePropertyValue))
            {
                return null;
            }

            if (parentProperty.Name == "Controls")
            {
                if (parent is TabStripPanel && this.dockWindows != null)
                {
                    DockWindow existingWindow;
                    if (this.dockWindows.TryGetValue(uniquePropertyValue, out existingWindow))
                    {
                        res = existingWindow;
                    }
                }
                else
                    if (reader.Name == typeof(DocumentContainer).FullName)
                {
                    res = this.SearchDocumentContainer(uniquePropertyValue);
                }
            }

            if (res == null)
            {
                res = base.MatchExistingItem(reader, toRead, parent, parentProperty, propertyToMatch, uniquePropertyValue, existingInstancesToMatch,
                                             ref foundAtIndex);
            }

            return res;
        }

        private object SearchDocumentContainer(string uniquePropertyValue)
        {
            if (this.dock.MainDocumentContainer.Name == uniquePropertyValue)
            {
                return this.dock.MainDocumentContainer;
            }

            return null;
        }
    }
}
