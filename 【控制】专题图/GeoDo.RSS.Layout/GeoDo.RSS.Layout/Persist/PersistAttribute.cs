using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public enum enumAttType
    { 
        ValueType,
        UnValueType
    }

    public class PersistAttribute:Attribute
    {
        private enumAttType _attType = enumAttType.ValueType;

        public PersistAttribute()
        { 
        }

        public PersistAttribute(enumAttType attType)
        {
            _attType = attType;
        }

        public enumAttType AttType
        {
            get { return _attType; }
        }
    }
}
