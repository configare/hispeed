using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    public class LogInterpolator:IInterpolator
    {
        private int _factor = 0;
        private int _logbase = 0;

        public LogInterpolator()
        {
            _factor = 30;
            _logbase = 2;
        }

        public LogInterpolator(int factor, int logbase)
        {
            _factor = factor;
            _logbase = logbase;
        }

        public void Updata(int factor, int logbase)
        {
            _factor = factor;
            _logbase = logbase;
        }

        public byte Interpolate(byte x)
        {
            return ColorMath.FixByte(_factor * Math.Log(x, _logbase));
        }
    }
}
