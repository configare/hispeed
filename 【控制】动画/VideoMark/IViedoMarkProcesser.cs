using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.VideoMark
{
  public  interface IViedoMarkProcesser
    {
        bool Support(string aviFilename);
        bool Mark(string avifilename, System.Drawing.Size size, int interval, System.Drawing.Image[] res, Action<int, string> progress);
    }
}
