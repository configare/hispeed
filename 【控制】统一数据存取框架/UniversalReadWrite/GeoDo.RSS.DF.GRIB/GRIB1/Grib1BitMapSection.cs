using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 描述Grib1位图片段，某格网点没有值时用0表示
    /// </summary>
    public class Grib1BitMapSection : IGribBitMapSection
    {
        private int _sectionLength;
        private bool[] _bitmap;

        public Grib1BitMapSection(FileStream fs)
        {
            int[] bitmask = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };

            // octet 1-3 (length of section)
            _sectionLength = GribNumberHelper.Uint3(fs);

            // octet 4 unused bits
            int unused = fs.ReadByte();

            // octets 5-6
            int bm = GribNumberHelper.Int2(fs);
            sbyte[] data = new sbyte[_sectionLength - 6];
            StreamReadHelper.ReadInput(fs, data, 0, data.Length);

            // 创建新位图, octet 4包含末尾使用过的位数
            _bitmap = new bool[(_sectionLength - 6) * 8 - unused];

            // 填充位图
            for (int i = 0; i < _bitmap.Length; i++)
                _bitmap[i] = (data[i / 8] & bitmask[i % 8]) != 0;
        }

        public int SectionLength
        {
            get { return  _sectionLength; }
        }

        public bool[] Bitmap
        {
            get { return _bitmap; }
        }


    }
}
