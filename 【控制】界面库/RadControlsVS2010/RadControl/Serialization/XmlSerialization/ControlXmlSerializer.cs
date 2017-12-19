using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.XmlSerialization
{
    public class ControlXmlSerializer : ComponentXmlSerializer
    {
        public ControlXmlSerializer(ComponentXmlSerializationInfo componentSerializationInfo): base(componentSerializationInfo)
        {      
      
        }

        public ControlXmlSerializer()
            : base(new ControlXmlSerializationInfo())
        {

        }

        protected override bool ProcessListOverride(System.Xml.XmlReader reader, object listOwner, PropertyDescriptor parentProperty, System.Collections.IList list)
        {
            Control.ControlCollection controlCollection = list as Control.ControlCollection;
            if (controlCollection != null)
            {
                ReadMergeCollection(reader, listOwner, parentProperty, list, "Name");
                return true;
            }

            return base.ProcessListOverride(reader, listOwner, parentProperty, list);
        }
    }
}
