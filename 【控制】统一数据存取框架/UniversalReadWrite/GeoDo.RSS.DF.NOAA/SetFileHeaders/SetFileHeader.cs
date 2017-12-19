using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.NOAA
{
    public static class SetFileHeader
    {
        public static D1BDHeader Set1BDHeader(string filename)
        {
            FileStream fs = null;
            BinaryReader br = null;
            D1BDHeader d1bdHeader = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs, Encoding.Default);
                d1bdHeader = new SecHlder1BDFileHeader().Create(fs, br, 0, 22016) as D1BDHeader;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (br != null)
                    br.Close();
            }
            return d1bdHeader;
        }

        public static D1A5Header Set1A5Header(string filename)
        {
            FileStream fs = null;
            BinaryReader br = null;
            D1A5Header d1a5Header = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs, Encoding.Default);
                d1a5Header = new SecHlder1A5FileHeader().Create(fs, br, 0, 21980) as D1A5Header;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (br != null)
                    br.Close();
            }
            return d1a5Header;
        }
    }
}
