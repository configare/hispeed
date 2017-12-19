using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ValueExtractResult:IValueExtractResult
    {
        protected double _value;
        protected string _name;
        protected bool _display = true;
        protected string _outIdentify;

        public ValueExtractResult(string name,double value)
        {
            _name = name;
            _value = value;
        }

        public void SetDispaly(bool display)
        {
            _display = display;
        }

        public void SetOutIdentify(string outIdentify)
        {
            _outIdentify = outIdentify;
        }

        public string Name 
        {
            get { return _name; }
        }

        public double Value
        {
            get { return _value; }
        }

        public bool Display
        {
            get { return _display; }
        }

        public string OutIdentify
        {
            get { return _outIdentify; }
        }
    }
}
