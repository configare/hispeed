using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class IceEdgeFileNameHelper
    {
        public static string GetIceEdgeControlInfoFilename(string iceEdgeFilename)
        {
            string filename = Path.GetFileName(iceEdgeFilename);
            if (filename.Contains("EDGE"))
            {
                filename = filename.Replace("EDGE", "EDGC");
                return Path.Combine(Path.GetDirectoryName(iceEdgeFilename), filename);
            }
            else
            {
                return Path.ChangeExtension(iceEdgeFilename, ".controlpoint.shp");
            }
        }
    }
}
