using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class SelectableColorTargetColorItem:SimpleRgbProcessorArg
    {
        public string Name = null;
        public enumSelectableColor Color = enumSelectableColor.Black;

        public SelectableColorTargetColorItem(string name, enumSelectableColor color)
        {
            Name = name;
            Color = color;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
