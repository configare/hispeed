using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.Components
{
    public class ArgFieldMaps:ArgRefType
    {
        public ArgFieldMaps()
        { 
        }

        public override bool TryParse(string text, out object value)
        {
            value = null;
            try
            {
                string[] mapStrs = text.Split(';');
                List<FieldMap> maps = new List<FieldMap>();
                foreach (string f in mapStrs)
                {
                    FieldMap mp = FieldMap.FromString(f);
                    if (mp != null)
                        maps.Add(mp);
                }
                value = maps.Count > 0 ? maps.ToArray() : null;
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
