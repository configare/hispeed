using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public struct Range
    {
        private int min, max;
        private bool isNotNull;

        public Range(int min, int max)
        {
            this.min = min;
            this.max = max;

            this.isNotNull = true;
        }

        public int Min
        {
            get
            {
                return this.min;
            }
        }

        public int Max
        {
            get
            {
                return this.max;
            }
        }

        public int Count
        {
            get
            {
                if (this.IsNull)
                {
                    return 0;
                }

                return ((this.max - this.min) + 1);
            }
        }

        public bool IsNull
        {
            get
            {
                return !this.isNotNull;
            }
        }
    }
}
