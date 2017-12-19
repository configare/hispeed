namespace Microsoft.Xml.Serialization.GeneratedAssembly
{
    using System.Xml.Serialization;

    internal abstract class XmlSerializer1 : XmlSerializer
    {
        protected XmlSerializer1()
        {
        }

        protected override XmlSerializationReader CreateReader()
        {
            return new XmlSerializationReader1();
        }

		//protected override XmlSerializationWriter CreateWriter()
		//{
		//    return new XmlSerializationWriter1();
		//}
    }
}

