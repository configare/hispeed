using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB2DataSection
    {
        private GRIB2SectionHeader _sectionHeader;

        public GRIB2DataSection(FileStream fs)
        {
            _sectionHeader = new GRIB2SectionHeader(fs);
            if (_sectionHeader.SectionLength > 0 && _sectionHeader.SectionLength < fs.Length)
            {
                fs.Seek(_sectionHeader.SectionLength - 5, SeekOrigin.Current);
            }
        }

        public GRIB2SectionHeader SectionHeader
        {
            get { return _sectionHeader; }
        }
    }
}
