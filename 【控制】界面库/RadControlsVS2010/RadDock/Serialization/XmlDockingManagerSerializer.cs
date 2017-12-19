namespace Microsoft.Xml.Serialization.GeneratedAssembly
{
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class XmlDockingManagerSerializer : XmlSerializer1
    {
        public override bool CanDeserialize(XmlReader xmlReader)
        {
            return xmlReader.IsStartElement("DockingTree", "");
        }

        protected override object Deserialize(XmlSerializationReader reader)
        {
            return ((XmlSerializationReader1)reader).Read39_DockingTree();
        }

		//protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		//{
		//    ((XmlSerializationWriter1)writer).Write39_DockingTree(objectToSerialize);
		//}
    }
}

