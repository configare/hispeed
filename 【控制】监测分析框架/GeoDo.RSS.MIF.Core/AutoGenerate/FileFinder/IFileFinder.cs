using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IFileFinder
    {
        string[] Find(string currentRasterFile,ref string extinfo,string argument);
    }
}
