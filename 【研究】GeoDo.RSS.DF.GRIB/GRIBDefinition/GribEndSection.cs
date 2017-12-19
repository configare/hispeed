using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GribEndSection
    {
        private bool _isEndFound;

        public bool IsEndFound
        {
            get { return _isEndFound; }
        }

        public GribEndSection(FileStream fs)
        {
            int match = 0;
            long position = fs.Position;
            fs.Seek(fs.Length - 6, SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                // "7" "7" "7" "7"
                byte c = (byte)fs.ReadByte();
                if (c == '7')
                {
                    match += 1;
                }
                else
                    match = 0;
                if (match == 4)
                {
                    _isEndFound = true;
                    break;
                }
            }
            fs.Position = position;
        }
    }
}
