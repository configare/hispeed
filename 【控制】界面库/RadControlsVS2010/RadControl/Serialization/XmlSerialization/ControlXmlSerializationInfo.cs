using System.ComponentModel;

namespace Telerik.WinControls.XmlSerialization
{
    public class ControlXmlSerializationInfo : ComponentXmlSerializationInfo
    {
        public ControlXmlSerializationInfo(): base (new PropertySerializationMetadataCollection())
        {
            SerializationMetadata.Add(typeof(System.Windows.Forms.Control), "Tag", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            SerializationMetadata.Add(typeof(RadControl), "RootElement", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            SerializationMetadata.Add(typeof(System.Windows.Forms.Control), "DataBindings", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
        }
    }
}