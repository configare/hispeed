using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 位图段  section 6
    /// </summary>
    public class GRIB2BitMapSection:IGribBitMapSection
    {
        private GRIB2SectionHeader _sectionHeader;
        /// <summary> Bit-map indicator (see Code Table 6.0 and Note (1))</summary>
        private int _bitMapIndicator;
        private bool[] _bitmap = null;//是否是位图

        public GRIB2BitMapSection(FileStream fs,int numberPoints)
        {
            int[] bitmask = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };
            _sectionHeader = new GRIB2SectionHeader(fs);
            _bitMapIndicator = fs.ReadByte();
            // no bitMap
            if (_bitMapIndicator != 0)
                return;
            sbyte[] data = new sbyte[_sectionHeader.SectionLength - 6];
            StreamReadHelper.ReadInput(fs, data, 0, data.Length);
            // create new bit map, octet 4 contains number of unused bits at the end
            _bitmap = new bool[numberPoints];
            // fill bit map
            for (int i = 0; i < _bitmap.Length; i++)
                _bitmap[i] = (data[i / 8] & bitmask[i % 8]) != 0;
        }

        public int SectionLength
        {
            get { return _sectionHeader.SectionLength; }
        }

        public bool[] Bitmap
        {
            get { return _bitmap; }
        }
    }
}
