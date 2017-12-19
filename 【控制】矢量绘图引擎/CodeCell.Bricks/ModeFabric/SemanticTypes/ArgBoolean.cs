using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgBoolean : ArgValueType
    {
        public override bool TryParse(string text, out object value)
        {
            value = null;
            if (string.IsNullOrEmpty(text))
                return false;
            text = text.ToUpper();
            if (text == "TRUE" || text == "1" || text == "是")
                return true;
            return false;
        }
    }
}
