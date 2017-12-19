using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DI.MVG
{
    public class TabelDef
    {
        private float _smartValue;
        private float _fileValue;

        public TabelDef(float smartValue, float fileValue)
        {
            _smartValue = smartValue;
            _fileValue = fileValue;
        }

        public float SmartValue
        {
            get { return _smartValue; }
        }

        public float FileVale
        {
            get { return _fileValue; }
        }
    }
}
