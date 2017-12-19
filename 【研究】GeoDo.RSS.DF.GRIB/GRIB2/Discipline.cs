using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public class Discipline
    {
        private string _name;
        private int _number;
        private List<Category> _categorys;

        public Discipline()
        {
            _name = "undefined";
            _number = -1;
            _categorys = new List<Category>();
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public List<Category> CategoryList
        {
            get { return _categorys; }
            set { _categorys = value; }
        }

        public Category GetCategory(int catNumber)
        {
            if (_categorys == null || _categorys.Count < 1)
                return null;
            foreach (Category item in _categorys)
            {
                if (item.Number == catNumber)
                    return item;
            }
            return null;
        }
    }
}
