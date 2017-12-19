using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class PrimeMeridian
    {
        public string Name = "Greenwich";
        public double Value = 0d;

        public PrimeMeridian(string name, double value)
        {
            Name = name;
            Value = value;
        }

        //PRIMEM["Greenwich",0.0]
        public PrimeMeridian(WktItem wktItem)
        {
            string[] vs = wktItem.Value.Split(',');
            Name = vs[0].Replace("\"", string.Empty);
            Value = double.Parse(vs[1]);
        }

        public bool IsSame(PrimeMeridian pm)
        {
            return pm != null && Math.Abs(pm.Value - Value) < double.Epsilon;
        }

        public override string ToString()
        {
            return "PRIMEM[\"" + Name + "\"," + Value.ToString() + "]";
        }
    }
 }
