using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class LSTArgs
    {
        private Dictionary<string, string> _orbitPath = new Dictionary<string, string>();

        public LSTArgs()
        {
        }

        public Dictionary<string, string> OrbitPath
        {
            get { return _orbitPath; }
            set { _orbitPath = value; }
        }
    }

}
