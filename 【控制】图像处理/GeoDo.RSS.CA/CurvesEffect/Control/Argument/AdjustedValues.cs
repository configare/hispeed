using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    public class AdjustedValues
    {
        private bool _isEmpty = true;
        private byte[] _values = null;//len = 256

        public AdjustedValues(byte[] values)
        {
            _values = values;
            CheckIsEmpty();
        }

        private void CheckIsEmpty()
        {
            _isEmpty = _values == null || _values.Length != 256;
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public byte[] Values
        {
            get
            {
                return _values;
            }
            set 
            {
                _values = value;
                CheckIsEmpty();
            }
        }
    }
}
