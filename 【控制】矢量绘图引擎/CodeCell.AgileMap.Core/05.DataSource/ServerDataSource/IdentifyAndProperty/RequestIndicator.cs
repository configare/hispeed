using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public abstract class RequestIndicator
    {
        protected string _msg = null;

        public RequestIndicator()
        { 
        }

        public RequestIndicator(string msg)
            : this()
        {
            _msg = msg;
        }

        public string Msg
        {
            get { return _msg; }
        }
    }
}
