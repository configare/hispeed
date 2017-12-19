using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Components
{
    public class FieldDef
    {
        private string _fieldname = null;
        private enumFieldTypes _fieldType = enumFieldTypes.Text;
        private int _fieldLength = 64;//only valid for enumFieldTypes.Text

        public FieldDef()
        { 
        }

        public FieldDef(string fieldName)
        {
            _fieldname = fieldName;
        }

        public FieldDef(string fieldName, int fieldLength)
            :this(fieldName)
        {
            _fieldLength = fieldLength;
        }

        public FieldDef(string fieldName, enumFieldTypes fieldType)
            : this(fieldName)
        {
            _fieldType = fieldType;
        }

        public FieldDef(string fieldName, enumFieldTypes fieldType,int fieldLength)
            : this(fieldName,fieldType)
        {
            _fieldLength = fieldLength;
        }

        [DisplayName("字段名称")]
        public string FieldName
        {
            get { return _fieldname; }
            set { _fieldname = value; }
        }

        [DisplayName("字段类型")]
        public enumFieldTypes FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }

        [DisplayName("字段长度"),Description("只对Text类型的字段有效")]
        public int FieldLength
        {
            get { return _fieldLength; }
            set { _fieldLength = value; }
        }

        public override string ToString()
        {
            if (_fieldname == null)
                return string.Empty;
            return "{" + _fieldname + "," + _fieldType.ToString() + "," + _fieldLength.ToString() + "}";
        }

        public static FieldDef FromString(string flddef)
        {
            if (string.IsNullOrEmpty(flddef))
                return null;
            try
            {
                string[] parts = flddef.Replace("{", string.Empty).Replace("}", string.Empty).Split(',');
                enumFieldTypes type = enumFieldTypes.Text;
                foreach (enumFieldTypes t in Enum.GetValues(typeof(enumFieldTypes)))
                {
                    if (t.ToString() == parts[1])
                    {
                        type = t;
                        break;
                    }
                }
                return new FieldDef(parts[0], type, int.Parse(parts[2]));
            }
            catch
            {
                return null;
            }
        }
    }
}
