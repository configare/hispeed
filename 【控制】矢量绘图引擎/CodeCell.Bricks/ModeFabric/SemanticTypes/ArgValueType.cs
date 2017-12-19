using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgValueType:ArgSemanticType
    {
        public override bool TryParse(string text, out object value)
        {
            if(_dataType.Equals(typeof(string)))
            {
                value = text;
            }
            else if (_dataType.Equals(typeof(Int32)))
            {
                value = Int32.Parse(text);
            }
            else if (_dataType.Equals(typeof(Int16)))
            {
                value = Int16.Parse(text);
            }
            else if (_dataType.Equals(typeof(Int64)))
            {
                value = Int64.Parse(text);
            }
            else if (_dataType.Equals(typeof(byte)))
            {
                value = byte.Parse(text);
            }
            else if (_dataType.Equals(typeof(bool)))
            {
                text = text.ToUpper();
                if (text == "T" || text == "TRUE" || text == "是")
                    value = true;
                else
                    value = false;
            }
            else if (_dataType.Equals(typeof(double)))
            {
                value = double.Parse(text);
            }
            else if (_dataType.Equals(typeof(float)))
            {
                value = float.Parse(text);
            }
            else
            {
                return base.TryParse(text, out value);
            }
            return true;
        }

        public override string ToString(object value)
        {
            return base.ToString(value);
        }
    }
}
