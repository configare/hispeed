using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Telerik.WinControls.XmlSerialization;

namespace Telerik.WinControls.Themes.Serialization
{
	/// <summary>
	/// Implements whether an instances of a class need validation after theme 
    /// deserialization.
	/// </summary>
	public interface ISerializationValidatable
	{
		void Validate();
	}

    public class StyleXmlSerializer : ComponentXmlSerializer
    {
        internal bool partialLoad = false;

        public StyleXmlSerializer()
        {
            this.ResolveTypesOnlyInTelerikAssemblies = true;
        }

        public StyleXmlSerializer(bool partialLoad)
        {
            this.partialLoad = partialLoad;
        }

        protected override bool ReadObjectElementOverride(XmlReader reader, object toRead)
        {
            if (this.partialLoad && (toRead is XmlStyleSheet))
            {
                ((XmlStyleSheet)toRead).SetPartiallyLoadedXmlData(reader.ReadInnerXml());
                return true;
            }

            return base.ReadObjectElementOverride(reader, toRead);
        }

    }
}
