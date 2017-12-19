using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public class BandValueIndexPair
    {
        public int MinValue = -1;
        public int MaxValue = -1;
        public int Index = -1;
        public Int16 TransValue = -1;
        public bool IsDisplay;
        public Color TransRGB;
        public string Description = string.Empty;

        public BandValueIndexPair(int minValue, int maxValue, int index)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Index = index;
        }

        public BandValueIndexPair(int minValue, int maxValue, int index, Int16 transValue, bool isDisplay, Color transRGB, string description)
            :this(minValue, maxValue, index)
        {
            TransRGB = transRGB;
            IsDisplay = isDisplay;
            TransValue = transValue;
            Description = description;
        }
    }
}
