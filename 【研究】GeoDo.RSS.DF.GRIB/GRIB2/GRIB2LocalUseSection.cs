using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// GRIB2本地使用段 Section 2：段长、段号、编报中心附加的本地使用信息
    /// </summary>
    public class GRIB2LocalUseSection
    {
        private GRIB2SectionHeader _sectionHeader;
        private byte[] _localUse;

        public GRIB2LocalUseSection(FileStream fs)
        {
            long position = fs.Position;
            _sectionHeader = new GRIB2SectionHeader(fs);
            if (_sectionHeader.SectionNo != 2)
            {
                fs.Seek(-5, SeekOrigin.Current);
                return;
            }
            _localUse = new byte[_sectionHeader.SectionLength - 5];
            fs.Read(_localUse, 0, _localUse.Length);
            fs.Seek(position + _sectionHeader.SectionLength, SeekOrigin.Begin);
        }
    }
}
