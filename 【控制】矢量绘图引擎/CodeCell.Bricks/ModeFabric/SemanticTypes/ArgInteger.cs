using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgInteger:ArgNumber
    {
        public override bool TryParse(string text, out object value)
        {
            value = null;
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return true;
            }
            else
            {
                int v = 0;
                bool isOK = int.TryParse(text, out v);
                if (isOK)
                    value = v;
                return isOK;
            }
        }
    }
}
