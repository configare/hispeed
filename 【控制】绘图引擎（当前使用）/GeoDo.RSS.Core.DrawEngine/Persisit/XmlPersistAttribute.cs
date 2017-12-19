using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class XmlPersistAttribute:Attribute
    {
        private Type _propertyConverter;

        public XmlPersistAttribute(Type propertyConverter)
        {
            _propertyConverter = propertyConverter;
        }

        public Type PropertyConverter
        {
            get { return _propertyConverter; }
        }
    }
}
