using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 指示段："GRIB"、资料长度
    /// </summary>
    public class GRIB2IndicatorSection : IGRIB2IndicatorSection
    {
        private int _sectionLength;
        private int _discipline;
        private int _gribEdition;
        private long _gribLength;

        public GRIB2IndicatorSection(FileStream fs)
        {
            //if Grib edition 1, get bytes for the gribLength
            int[] data = new int[3];
            for (int i = 0; i < 3; i++)
            {
                data[i] = fs.ReadByte();
            }
            _gribEdition = fs.ReadByte();
            if (_gribEdition == 2)
            {
                _discipline = data[2];
                _gribLength = GribNumberHelper.Int8(fs);
                _sectionLength = 16;
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

        /// <summary>
        /// 学科
        /// </summary>
        public int Displine
        {
            get { return _discipline; }
        }

        public string DisplineName
        {
            get { return GetDisciplineName(); }
        }

        /// <summary>
        /// 获取Discipline名称
        /// </summary>
        /// <returns></returns>
        private string GetDisciplineName()
        {
            switch (_discipline)
            {
                case 0: return "Meteorological Products"; //气象产品
                case 1: return "Hydrological Products";  //水文
                case 2: return "Land surface Products";  //地表
                case 3: return "Space Products";        //空间
                case 4: return "Space Weather Products";
                case 10: return "Oceanographic Products"; //海洋地理
                default: return "Unknown";
            }
        }
    }
}
