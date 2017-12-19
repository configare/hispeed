using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    internal class AttToFieldMapAttribute:Attribute
    {
        private string _fieldName = null;
        private Type _fieldType = null;

        public AttToFieldMapAttribute(string fieldName, Type fieldType)
        {
            _fieldName = fieldName;
            _fieldType = fieldType;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public Type FieldType
        {
            get { return _fieldType; }
        }
    }
}
