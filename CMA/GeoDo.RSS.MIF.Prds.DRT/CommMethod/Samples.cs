using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class Samples
    {
        public float Ndvi = 0;
        public float Lst = 0;
        public float FDif = 0;

        public Samples()
        { }

        public Samples(float ndvi, float lst)
        {
            Ndvi = ndvi;
            Lst = lst;
        }

        public Samples(float ndvi, float lst, float fdif)
        {
            Ndvi = ndvi;
            Lst = lst;
            FDif = fdif;
        }
    }
}
