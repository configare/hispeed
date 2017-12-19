using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.Collections;

namespace CodeCell.AgileMap.Components
{
    public class ArgFieldDefs : ArgRefType
    {
        public ArgFieldDefs()
        { 
        }

        public override bool IsNeedInput
        {
            get
            {
                return true;
            }
        }

        public override bool TryParse(string text, out object value)
        {
            value = null;
            try
            {
                string[] flds = text.Split(';');
                List<FieldDef> defs = new List<FieldDef>();
                foreach (string f in flds)
                {
                    FieldDef field = FieldDef.FromString(f);
                    if (field != null)
                        defs.Add(field);
                }
                value = defs.Count > 0 ? defs.ToArray() : null;
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public override object GetValue(object sender)
        {
            return base.GetValue(sender);
        }
    }
}
