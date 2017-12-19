using System;
using System.Xml.Serialization;

namespace Telerik.WinControls.UI
{
    internal class XmlTreeSerializer : XmlSerializer
    {
        #region Constructors

        public XmlTreeSerializer(Type type, Type[] extraType)
            : base(type, extraType)
        {
            this.UnknownAttribute += new XmlAttributeEventHandler(XmlTreeSerializer_UnknownAttribute);
        }

        public XmlTreeSerializer(Type type)
            : base(type)
        {
            this.UnknownAttribute += new XmlAttributeEventHandler(XmlTreeSerializer_UnknownAttribute);
        }

        #endregion

        #region Event Handlers

        private void XmlTreeSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            IXmlTreeSerializable serializable = e.ObjectBeingDeserialized as IXmlTreeSerializable;

            if (serializable != null)
            {
                serializable.ReadUnknownAttribute(e.Attr);
            }
        }

        #endregion
    }
}
