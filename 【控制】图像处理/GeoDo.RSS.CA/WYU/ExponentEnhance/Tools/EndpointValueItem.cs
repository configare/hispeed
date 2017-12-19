using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    public class EndpointValueItem
    {
        private string _name = string.Empty;
        private double _minValue = 0;
        private double _maxValue = 0;
        private bool _checked = true;

        public EndpointValueItem()
        { 
        }

        public EndpointValueItem(double minValue, double maxValue,string name)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _name = name;
        }

        public EndpointValueItem(double minValue, double maxValue, string name,bool isChecked)
            :this(minValue,maxValue,name)
        {
            _checked = isChecked;
        }

        public double MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public string Name
        {
            get { return _name != null ? _name : string.Empty; }
            set { _name = value; }
        }

        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }

        public override string ToString()
        {
            return Name + " " + MinValue.ToString() + "," + MaxValue.ToString();
        }
    }
}
