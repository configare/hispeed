using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB2SectionHeader
    {
        private int _sectionLength;
        private int _sectionNo;

        public GRIB2SectionHeader(FileStream fs)
        {
            _sectionLength = GribNumberHelper.Int4(fs);
            _sectionNo = fs.ReadByte();
        }

        public int SectionLength
        {
            get { return _sectionLength; }
        }

        public int SectionNo
        {
            get { return _sectionNo; }
        }
    }
}
