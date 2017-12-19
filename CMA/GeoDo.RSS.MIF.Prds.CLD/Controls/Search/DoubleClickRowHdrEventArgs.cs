using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class DoubleClickRowHdrEventArgs : EventArgs
    {
        string _filename;

        public DoubleClickRowHdrEventArgs(string filename )
        {
            _filename = filename;
        }
        public string FullName
        {
            get
            {
                return _filename;
            }
        }
    }
}
