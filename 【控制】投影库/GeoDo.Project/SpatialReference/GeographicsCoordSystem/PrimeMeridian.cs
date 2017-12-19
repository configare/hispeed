using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    [Serializable]
    public class PrimeMeridian:ICloneable
    {
        public string Name = "Greenwich";
        public double Value = 0d;
        private const double MINI_DOUBLE = 0.00000000001;   //比较时参照的最小正浮点数

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
            return pm != null && Math.Abs(pm.Value - Value) < MINI_DOUBLE;
        }

        public override string ToString()
        {
            return "PRIMEM[\"" + Name + "\"," + Value.ToString() + "]";
        }

        public object Clone()
        {
            return new PrimeMeridian(this.Name, this.Value);
        }
    }
 }
