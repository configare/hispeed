using System;
using System.Collections.Generic;

namespace GeoDo.HDF5
{
    public class HDFAttributeDef 
    {
        private string _name = null;
        private Type _type = null;
        private int _size = 0;
        private object _value = null;

        public HDFAttributeDef() { }

        public HDFAttributeDef(string name, Type type, int size, object value)
        {
            _name = name;
            _type = type;
            _size = size;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public class HDFAttributeDefCollection
    {
        private List<HDFAttributeDef> _attributes = null;

        public HDFAttributeDefCollection(HDFAttributeDef[] attributes)
        {
            _attributes = new List<HDFAttributeDef>();
            _attributes.AddRange(attributes);
        }

        public HDFAttributeDef[] Attributes
        {
            get { return _attributes != null ? _attributes.ToArray():null; }
        }
    }
}
