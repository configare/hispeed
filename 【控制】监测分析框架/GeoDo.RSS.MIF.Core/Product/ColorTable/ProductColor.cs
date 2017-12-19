using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
   public class ProductColor
    {
       public Color Color;
       public float MaxValue;
       public float MinValue;
       public string LableText = "";
       public bool DisplayLengend = true;

       public bool IsContains(float v)
       {
           return v >= MinValue && v < MaxValue;
       }
    }
}
