using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public class Category
    {
        private int _number;
        private string _name;
        private List<Parameter> _parameters;

        public Category()
        {
            _number = -1;
            _name = "undefined";
            _parameters = new List<Parameter>();
        }

        public Category(int number, string name)
        {
            _name = name;
            _number = number;
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<Parameter> ParameterList
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public Parameter GetParameter(int parameterNo)
        {
            if (_parameters == null || _parameters.Count < 1)
                return null;
            foreach (Parameter item in _parameters)
            {
                if (item.ParameterNo == parameterNo)
                    return item;
            }
            return null;
        }
    }
}
