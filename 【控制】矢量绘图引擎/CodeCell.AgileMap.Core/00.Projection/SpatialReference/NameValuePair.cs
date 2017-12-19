using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class NameValuePair : ICloneable
    {
        public NameMapItem Name = null;
        public double Value = 0d;

        public NameValuePair()
        {
        }

        public NameValuePair(NameMapItem name, double value)
        {
            Name = name;
            Value = value;
        }

        public object Clone()
        {
            NameValuePair v = new NameValuePair();
            if (this.Name != null)
                v.Name = this.Name.Clone() as NameMapItem;
            v.Value = this.Value;
            return v;
        }
    }
}
