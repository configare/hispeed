using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class NameValuePair
    {
        public NameMapItem Name = null;
        public double Value = 0d;

        public NameValuePair(NameMapItem name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
