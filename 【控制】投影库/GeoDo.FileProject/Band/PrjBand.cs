using System;
using System.Collections.Generic;
using System.Collections;

namespace GeoDo.FileProject
{
    public class PrjBand
    {
        private float _resolution;
        private string _sensorType;
        public string BandDesc;

        public PrjBand(string sensorType, float resolution, string bandName, int dataSetIndex, string prefix, string bandDesc, string dataSetName)
        {
            this._sensorType = sensorType;
            this._resolution = resolution;
            this.BandName = bandName;
            this.BandDesc = bandDesc;
            this.DataSetName = dataSetName;
            this.DataSetIndex = dataSetIndex;
            this.Prefix = prefix;
        }

        /// <summary>
        /// 波段名
        /// 例如
        /// 1,2,3,4,5,5,6,7,8...13lo,13hi
        /// </summary>
        public string BandName;
        /// <summary>
        /// hdf数据集名字
        /// </summary>
        public string DataSetName;
        /// <summary>
        /// 在hdf数据集中的索引号（从0开始）
        /// </summary>
        public int DataSetIndex;
        public string Prefix;

        public float Resolution
        {
            get
            {
                return this._resolution;
            }
            set
            {
                this._resolution = value;
            }
        }

        public string SensorType
        {
            get
            {
                return this._sensorType;
            }
            set
            {
                this._sensorType = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Prefix, this.BandName.PadRight(4, ' '), this.BandDesc);
        }

        public string toHDRStr()
        {
            return Prefix + "   " + BandName + "   " + BandDesc + "   ,";
        }

        public static PrjBand[] VIRR_1000_Orbit
        {
            get { return OrbitBandDefCollection.VIRR_1000_OrbitDefCollecges(); }
        }

        public static PrjBand[] MERSI_1000_Orbit
        {
            get { return OrbitBandDefCollection.MERSI_1000_OrbitDefCollecges(); }
        }

        public static PrjBand[] MERSI_0250_Orbit
        {
            get { return OrbitBandDefCollection.MERSI_0250_OrbitDefCollecges(); }
        }

        public static PrjBand[] AVHRR_1000_Orbit
        {
            get { return OrbitBandDefCollection.AVHRR_1000_OrbitDefCollecges(); }
        }

        public static PrjBand[] MODIS_250_Orbit
        {
            get { return OrbitBandDefCollection.MODIS_250_OrbitDefCollecges(); }
        }

        public static PrjBand[] MODIS_500_Orbit
        {
            get { return OrbitBandDefCollection.MODIS_500_OrbitDefCollecges(); }
        }

        public static PrjBand[] MODIS_1000_Orbit
        {
            get { return OrbitBandDefCollection.MODIS_1000_OrbitDefCollecges(); }
        }

        public static PrjBand[] VISSR_5000_Orbit
        {
            get { return OrbitBandDefCollection.FY2_5000_OrbitDefCollecges(); }
        }
    }

    class PrjBandCollection : ICollection
    {
        private List<PrjBand> _prjBands = new List<PrjBand>();

        public PrjBand this[int index]
        {
            get
            {
                return _prjBands[index];
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _prjBands == null ? 0 : _prjBands.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public IEnumerator GetEnumerator()
        {
            return _prjBands.GetEnumerator();
        }
    }
}

