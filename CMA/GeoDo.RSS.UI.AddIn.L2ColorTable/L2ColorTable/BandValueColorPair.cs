using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public class BandValueColorPair
    {
        public int MinValue = -1;
        public int MaxValue = -1;
        public Color Color = Color.Transparent;
        public bool IsDisplay = false;
        public Int16 TransferValue = 0;
        public Color TransferColor = Color.Transparent;
        public string Description = string.Empty;

        public BandValueColorPair(int minValue, int maxValue, Color color)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Color = color;
        }

        public BandValueColorPair(int minValue, int maxValue, Color color, bool isDisplay, Int16 transferValue, Color transferColor, string description)
            :this(minValue, maxValue, color)
        {
            IsDisplay = isDisplay;
            TransferValue = transferValue;
            TransferColor = transferColor;
            Description = description;
        }
    }
}
