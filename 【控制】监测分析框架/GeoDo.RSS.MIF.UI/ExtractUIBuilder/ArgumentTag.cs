using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.UI
{
    public class ArgumentTag
    {
        public ArgumentTag(ArgumentBase arg, Control[] labels)
        {
            Arg = arg;
            Labels = labels;
        }

        public ArgumentBase Arg;
        public Control[] Labels;
    }
}
