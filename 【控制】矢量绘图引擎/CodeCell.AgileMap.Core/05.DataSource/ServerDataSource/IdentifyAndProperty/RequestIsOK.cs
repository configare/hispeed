using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class RequestIsOK:RequestIndicator
    {
        public RequestIsOK()
            : base()
        { 
        }

        public RequestIsOK(string msg)
            : base(msg)
        { 
        }
    }
}
