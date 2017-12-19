using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    internal struct VTriangle
    {
        private const byte CONTAINED_BY_A = 1;//0000 0001 
        private const byte CONTAINED_BY_B = 2;//0000 0010 
        private const byte CONTAINED_BY_C = 4;//0000 0100 
        private byte _status;

        public void Reset()
        {
            _status = 0;
        }

        public bool IsAContained
        {
            get { return (_status & CONTAINED_BY_A) == CONTAINED_BY_A; }
        }

        public void SetAContained()
        {
            _status = (byte)(_status ^ CONTAINED_BY_A);
        }

        public bool IsBContained
        {
            get { return (_status & CONTAINED_BY_B) == CONTAINED_BY_B; }
        }

        public void SetBContained()
        {
            _status = (byte)(_status ^ CONTAINED_BY_B);
        }

        public bool IsCContained
        {
            get { return (_status & CONTAINED_BY_C) == CONTAINED_BY_C; }
        }

        public void SetCContained()
        {
            _status = (byte)(_status ^ CONTAINED_BY_C);
        }

        public bool Is2Contained()
        {
            return (IsAContained && IsBContained) ||
                (IsBContained && IsCContained) ||
                (IsAContained && IsCContained);
        }

        public bool Is3Contained()
        {
            return IsAContained && IsBContained && IsCContained;
        }

        public override string ToString()
        {
            return string.Format("A:{0},B:{1},C:{2}", IsAContained, IsBContained, IsCContained);
        }
    }
}
