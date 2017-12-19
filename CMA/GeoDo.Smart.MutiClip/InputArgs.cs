using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.MutiClip
{
    public  class InputArgs
    {

        public List<string> ListInputFiles { get; set; }
        public string InputDir { get; set; }
        public string OutDir { get; set; }
        public List<CoordInfo> ListCoord = new List<CoordInfo>();
    }
    public class CoordInfo
    {
        public string CoordName { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

    }
}
