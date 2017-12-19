using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeCell.AgileMap.Components
{
    public class FieldMap
    {
        private FieldDef _resField = null;
        private FieldDef _desField = null;

        public FieldMap()
        { 
        }

        public FieldMap(FieldDef resField, FieldDef desField)
        {
            _resField = resField;
            _desField = desField;
        }

        [DisplayName("源字段"),Description("{FieldName,FieldType,FieldLength}")]
        public FieldDef ResField
        {
            get { return _resField; }
            set { _resField = value; }
        }

        [DisplayName("目标字段"), Description("{FieldName,FieldType,FieldLength}")]
        public FieldDef DesField
        {
            get { return _desField; }
            set { _desField = value; }
        }

        public override string ToString()
        {
            if (_resField == null || _desField == null)
                return string.Empty;
            return "{" + _resField.ToString() + "-" + _desField.ToString() + "}";
        }

        public static FieldMap FromString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            try
            {
                text = text.Substring(1, text.Length - 2);
                string[] parts = text.Split('-');
                return new FieldMap(FieldDef.FromString(parts[0]), FieldDef.FromString(parts[1]));
            }
            catch 
            {
                return null;
            }
        }
    }
}
