using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class RequestIsFailed : RequestIndicator
    {
        public RequestIsFailed()
            : base()
        {
        }

        public RequestIsFailed(string msg)
            : base(msg)
        {
        }
    }
}
