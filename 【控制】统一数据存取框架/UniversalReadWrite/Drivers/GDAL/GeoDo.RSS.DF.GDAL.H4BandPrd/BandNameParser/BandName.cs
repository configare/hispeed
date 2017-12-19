using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GDAL.H4BandPrd
{
    internal class BandName:IComparable
    {
        public int Index = 0;
        public string Name = null;

        public BandName()
        { 
        }

        public BandName(int index)
        {
            Index = index;
            Name = Index.ToString();
        }

        public BandName(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public int CompareTo(object obj)
        {
            return Index - (obj as BandName).Index;
        }
    }
}
