using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls.XmlSerialization;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking.Serialization
{
    public class FloatingWindowsXmlResolvingSerializer : DockXmlSerializerBase
    {
        public FloatingWindowsXmlResolvingSerializer(RadDock dock, ComponentXmlSerializationInfo componentSerializationInfo)
            : base(dock, componentSerializationInfo)
        {
        }

        protected override bool ProcessListOverride(System.Xml.XmlReader reader, object propertyOwner, PropertyDescriptor parentProperty, System.Collections.IList list)
        {
            Control.ControlCollection controlCollection = list as Control.ControlCollection;
            if (controlCollection != null)
            {
                if (propertyOwner is TabStripPanel)
                {
                    ReadMergeCollection(reader, propertyOwner, parentProperty, list, "DockWindowName", true);
                }
                else
                {
                    ReadMergeCollection(reader, propertyOwner, parentProperty, list, "Name", true);
                }

                return true;
            }

            return base.ProcessListOverride(reader, propertyOwner, parentProperty, list);
        }

        protected override string GetElementNameByType(Type elementType)
        {
            if (typeof(DockWindow).IsAssignableFrom(elementType))
            {
                return typeof(DockWindowPlaceholder).FullName;
            }

            if (typeof(FloatingWindow).IsAssignableFrom(elementType))
            {
                return typeof(SerializableFloatingWindow).FullName;
            }

            return base.GetElementNameByType(elementType);
        }

        public override void WriteObjectElement(System.Xml.XmlWriter writer, object toWrite)
        {
            DockWindow dockWindow = toWrite as DockWindow;
            if (dockWindow != null)
            {
                DockWindowPlaceholder placeHolder = new DockWindowPlaceholder(dockWindow);
                base.WriteObjectElement(writer, placeHolder);
                return;
            }

            FloatingWindow floatingWindow = toWrite as FloatingWindow;
            if (floatingWindow != null)
            {
                SerializableFloatingWindow seriazlializable = new SerializableFloatingWindow(floatingWindow);
                base.WriteObjectElement(writer, seriazlializable);
                return;
            }

            base.WriteObjectElement(writer, toWrite);
        }
    }
}
