using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.XmlSerialization;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking.Serialization
{
    public class FloatingWindowsXmlNonResolvingSerializer : ComponentXmlSerializer
    {
        public FloatingWindowsXmlNonResolvingSerializer(ComponentXmlSerializationInfo componentSerializationInfo)
            : base(componentSerializationInfo)
        {            
        }

        protected override bool ProcessListOverride(System.Xml.XmlReader reader, object propertyOwner, PropertyDescriptor parentProperty, System.Collections.IList list)
        {
            Control.ControlCollection controlCollection = list as Control.ControlCollection;
            if (controlCollection != null)
            {
                if (list is RadSplitContainer.ControlCollection)
                {
                    return base.ProcessListOverride(reader, propertyOwner, parentProperty, list);
                }
                else
                {
                    ReadMergeCollection(reader, propertyOwner, parentProperty, list, "DockWindowName", true);
                }
                return true;
            }

            return base.ProcessListOverride(reader, propertyOwner, parentProperty, list);
        }

    }
}
