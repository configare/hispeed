using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgString : ArgValueType
    {
        public override bool TryParse(string text, out object value)
        {
            value = text;
            return true;
        }
    }
}
