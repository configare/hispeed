using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public abstract class XmlSerializerExtention
    {
        // Methods
        protected XmlSerializerExtention()
        {
        }

        public abstract object ProvideValue(IServiceProvider serviceProvider);
    }
}
