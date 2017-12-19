using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class MaskPart
    {
        public string maskPart;
        public int start;
        public int len;
        public bool month;
        public bool year;
        public bool day;
        public bool readOnly;
        public PartTypes type;
        public int offset = 0;
        public int min = 0;
        public int max = 2500;
        public int value;
        public string charValue;
        public bool hasZero = false;

        public void Validate()
        {
            if (this.value > this.max)
            {
                this.value = this.max;
            }

            if (this.value < this.min)
            {
                this.value = this.min;
            }
        }
    }
}
