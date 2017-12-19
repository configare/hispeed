using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GribIndicatorSection:IGribIndicatorSection
    {
        private int _gribEdition;
        private long _gribLength;
        private int _sectionLength;
        private int _discipline;     //学科

        public GribIndicatorSection(FileStream fs)
        {
            long mark = fs.Position;
            //if Grib edition 1, get bytes for the gribLength
            fs.Seek(3, SeekOrigin.Current);
            // edition of GRIB specification
            _gribEdition = fs.ReadByte();
            if (_gribEdition == 1)
            {
                // length of GRIB record
                // Reset to beginning, then read 3 bytes
                fs.Position = mark;
                _gribLength = (long)GribNumberHelper.Uint3(fs);
                // Skip next byte, edition already read
                fs.ReadByte();
                _sectionLength = 8;
            }
            else if (_gribEdition == 2)
            {
                fs.Position = mark + 2;
                // length of GRIB record
                _discipline = fs.ReadByte();
                fs.ReadByte();
                _gribLength = GribNumberHelper.Int8(fs);
                _sectionLength = 16;
            }
            else
            {
                throw new NotSupportedException("GRIB edition " + _gribEdition + " is not yet supported");
            }
        }

        public int GribEdition
        {
            get { return _gribEdition; }
        }

        public long GribLength
        {
            get { return _gribLength; }
        }

        public int SectionLength
        {
            get { return _sectionLength; }
        }
    }
}
