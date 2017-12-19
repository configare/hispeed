using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.VideoMark
{
    public interface IViedoMarker
    {
        bool Mark(string avifilename, string fileType,Size size, int interval, Image[] res,Action<int,string> progress);
    }
}
