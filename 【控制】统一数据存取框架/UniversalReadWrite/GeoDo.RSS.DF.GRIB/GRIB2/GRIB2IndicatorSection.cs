using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
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
                case 0: return "Meteorological Products";
                case 1: return "Hydrological Products";
                case 2: return "Land surface Products";
                case 3: return "Space Products";
                case 4: return "Space Weather Products";
                case 10: return "Oceanographic Products";
                default: return "Unknown";
            }
        }
    }
}
