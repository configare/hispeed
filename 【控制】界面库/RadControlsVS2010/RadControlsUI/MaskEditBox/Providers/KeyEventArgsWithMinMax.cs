using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class KeyEventArgsWithMinMax : KeyEventArgs
    {
        DateTime minDate;
        DateTime maxDate;

        public KeyEventArgsWithMinMax(System.Windows.Forms.Keys keyData, DateTime min, DateTime max):base(keyData)
        {
            this.minDate = min;
            this.maxDate = max;
        }    

        public DateTime MaxDate
        {
            get
            {
                return maxDate;
            }
            set
            {
                maxDate = value;
            }
        }

        public DateTime MinDate
        {
            get
            {
                return minDate;
            }
            set
            {
                minDate = value;
            }
        }
    }
}
