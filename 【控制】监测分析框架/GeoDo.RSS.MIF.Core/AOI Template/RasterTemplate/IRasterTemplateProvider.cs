using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterTemplateProvider
    {
        string[] Names { get; }
        int[] GetAOI(string name, double minX, double maxX, double minY, double maxY, Size outSize);
    }
}
