using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 数据段 section 7：段长、段号、数据值
    /// </summary>
    public class GRIB2DataSection
    {
        private GRIB2SectionHeader _sectionHeader;
        private int _width = 331;
        private int _height = 191;
        private long _dataOffset = 0;
        private int _dataLength = 0;

        public GRIB2DataSection(FileStream fs,long dataOffset)
        {
            long position = fs.Position;
            _sectionHeader = new GRIB2SectionHeader(fs);
            _dataOffset = dataOffset;
            _dataLength = _sectionHeader.SectionLength - 5;
            fs.Seek(position + _sectionHeader.SectionLength, SeekOrigin.Begin);
        }

        public long DataOffset
        {
            get { return _dataOffset; }
        }

        public int DataLength
        {
            get { return _dataLength; }
        }

        public GRIB2SectionHeader SectionHeader
        {
            get { return _sectionHeader; }
        }
    }
}
