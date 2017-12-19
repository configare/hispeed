using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.GPC
{
    /// <summary>
    /// 整个文件是个二进制格式，其组织如下：
    /// (130 + 130 * 99)* 66 + (130 + 130 * 80)
    /// (每行行头+每个Cell值个数*Cell个数)*行数+(最后一个行)
    /// </summary>
    public class GPCRasterDataProvider : RasterDataProvider
    {
        #region 原始数据
        private GCPRow[] _gcpRows = null;
        #endregion
        private const int MAXLAT = 72;
        private const int MAXLON = 144;
        private int[] _cellIndexs = null;
        private int[] _rowIndexs = null;
        string[] _bandNames = new string[] {
            #region 所有波段的波段名
            "Latitude",
            "Longitude index (equal-area)",
            "Western-most longitude index (equal-angle)",
            "Eastern-most longitude index (equal-angle)",
            "Land water coast code",
            "Number of observations",
            "Number of daytime observations",
            "Mean cloud amount",
            "Mean IR-marginal cloud amount",
            "Frequency of mean cloud amount = 0-10%",
            "Frequency of mean cloud amount = 10-20%",
            "Frequency of mean cloud amount = 20-30%",
            "Frequency of mean cloud amount = 30-40%",
            "Frequency of mean cloud amount = 40-50%",
            "Frequency of mean cloud amount = 50-60%",
            "Frequency of mean cloud amount = 60-70%",
            "Frequency of mean cloud amount = 70-80%",
            "Frequency of mean cloud amount = 80-90%",
            "Frequency of mean cloud amount = 90-100%",
            "Mean cloud top pressure(PC)",
            "Standard deviation of spatial mean over time(PC)",
            "Time mean of standard deviation over space(PC)",
            "Cloud temperature(TC)",
            "Standard deviation of spatial mean over time(TC)",
            "Time mean of standard deviation over space(TC)",
            "Mean cloud optical thickness(TAU)",
            "Standard deviation of spatial mean over time(TAU)",
            "Time mean of standard deviation over space(TAU)",
            "Mean cloud water path(WP)",
            "Standard deviation of spatial mean over time(WP)",
            "Time mean of standard deviation over space(WP)",
            "Mean CA for low-level clouds",
            "Mean PC for low-level clouds",
            "Mean TC for low-level clouds",
            "Mean CA for middle-level clouds",
            "Mean PC for middle-level clouds",
            "Mean TC for middle-level clouds",
            "Mean CA for high-level clouds",
            "Mean PC for high-level clouds",
            "Mean TC for high-level clouds",
            "Mean CA for cloud type 1 = Cumulus, liquid",
            "Mean PC for cloud type 1 = Cumulus, liquid",
            "Mean TC for cloud type 1 = Cumulus, liquid",
            "Mean TAU for cloud type 1 = Cumulus, liquid",
            "Mean WP for cloud type 1 = Cumulus, liquid",
            "Mean CA for cloud type 2 = Stratocumulus, liquid",
            "Mean PC for cloud type 2 = Stratocumulus, liquid",
            "Mean TC for cloud type 2 = Stratocumulus, liquid",
            "Mean TAU for cloud type 2 = Stratocumulus, liquid",
            "Mean WP for cloud type 2 = Stratocumulus, liquid",
            "Mean CA for cloud type 3 = Stratus, liquid",
            "Mean PC for cloud type 3 = Stratus, liquid",
            "Mean TC for cloud type 3 = Stratus, liquid",
            "Mean TAU for cloud type 3 = Stratus, liquid",
            "Mean WP for cloud type 3 = Stratus, liquid",
            "Mean CA for cloud type 4 = Cumulus, ice",
            "Mean PC for cloud type 4 = Cumulus, ice",
            "Mean TC for cloud type 4 = Cumulus, ice",
            "Mean TAU for cloud type 4 = Cumulus, ice",
            "Mean WP for cloud type 4 = Cumulus, ice",
            "Mean CA for cloud type 5 = Stratocumulus, ice",
            "Mean PC for cloud type 5 = Stratocumulus, ice",
            "Mean TC for cloud type 5 = Stratocumulus, ice",
            "Mean TAU for cloud type 5 = Stratocumulus, ice",
            "Mean WP for cloud type 5 = Stratocumulus, ice",
            "Mean CA for cloud type 6 = Stratus, ice",
            "Mean PC for cloud type 6 = Stratus, ice",
            "Mean TC for cloud type 6 = Stratus, ice",
            "Mean TAU for cloud type 6 = Stratus, ice",
            "Mean WP for cloud type 6 = Stratus, ice",
            "Mean CA for cloud type 7 = Altocumulus, liquid",
            "Mean PC for cloud type 7 = Altocumulus, liquid",
            "Mean TC for cloud type 7 = Altocumulus, liquid",
            "Mean TAU for cloud type 7 = Altocumulus, liquid",
            "Mean WP for cloud type 7 = Altocumulus, liquid",
            "Mean CA for cloud type 8 = Altostratus, liquid",
            "Mean PC for cloud type 8 = Altostratus, liquid",
            "Mean TC for cloud type 8 = Altostratus, liquid",
            "Mean TAU for cloud type 8 = Altostratus, liquid",
            "Mean WP for cloud type 8 = Altostratus, liquid",
            "Mean CA for cloud type 9 = Nimbostratus, liquid",
            "Mean PC for cloud type 9 = Nimbostratus, liquid",
            "Mean TC for cloud type 9 = Nimbostratus, liquid",
            "Mean TAU for cloud type 9 = Nimbostratus, liquid",
            "Mean WP for cloud type 9 = Nimbostratus, liquid",
            "Mean CA for cloud type 10 = Altocumulus, ice",
            "Mean PC for cloud type 10 = Altocumulus, ice",
            "Mean TC for cloud type 10 = Altocumulus, ice",
            "Mean TAU for cloud type 10 = Altocumulus, ice",
            "Mean WP for cloud type 10 = Altocumulus, ice",
            "Mean CA for cloud type 11 = Altostratus, ice",
            "Mean PC for cloud type 11 = Altostratus, ice",
            "Mean TC for cloud type 11 = Altostratus, ice",
            "Mean TAU for cloud type 11 = Altostratus, ice",
            "Mean WP for cloud type 11 = Altostratus, ice",
            "Mean CA for cloud type 12 = Nimbostratus, ice",
            "Mean PC for cloud type 12 = Nimbostratus, ice",
            "Mean TC for cloud type 12 = Nimbostratus, ice",
            "Mean TAU for cloud type 12 = Nimbostratus, ice",
            "Mean WP for cloud type 12 = Nimbostratus, ice",
            "Mean CA for cloud type 13 = Cirrus",
            "Mean PC for cloud type 13 = Cirrus",
            "Mean TC for cloud type 13 = Cirrus",
            "Mean TAU for cloud type 13 = Cirrus",
            "Mean WP for cloud type 13 = Cirrus",
            "Mean CA for cloud type 14 = Cirrostratus",
            "Mean PC for cloud type 14 = Cirrostratus",
            "Mean TC for cloud type 14 = Cirrostratus",
            "Mean TAU for cloud type 14 = Cirrostratus",
            "Mean WP for cloud type 14 = Cirrostratus",
            "Mean CA for cloud type 15 = Deep convective",
            "Mean PC for cloud type 15 = Deep convective",
            "Mean TC for cloud type 15 = Deep convective",
            "Mean TAU for cloud type 15 = Deep convective",
            "Mean WP for cloud type 15 = Deep convective",
            "Mean TS from clear sky composite(TS)",
            "Time mean of standard deviation over space(TS)",
            "Mean RS from clear sky composite(RS)",
            "Mean ice snow cover",
            "Mean Surface pressure (PS)",
            "Mean Near-surface air temperature (TSA)",
            "Mean Temperature at 740 mb (T)",
            "Mean Temperature at 500 mb (T)",
            "Mean Temperature at 375 mb (T)",
            "Mean Tropopause pressure (PT)",
            "Mean Tropopause temperature (TT)",
            "Mean Stratosphere temperature at 50 mb (T)",
            "Mean Precipitable water for 1000-680 mb (PW)",
            "Mean Precipitable water for 680-310 mb (PW)",
            "Mean Ozone column abundance (O3)"
                    #endregion
        };

        public GPCRasterDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        {
            _fileName = fileName;
            ReadGPCRow();
            LoadBand();
            EQ2SQLUT(_gcpRows, out _cellIndexs, out _rowIndexs);
            InitAttributes();
        }

        /// <summary>
        /// GPC原始格点数据
        /// </summary>
        public GCPRow[] GCPRows
        {
            get { return _gcpRows; }
        }

        /// <summary>
        /// x轴查找表
        /// </summary>
        public int[] CellIndexLut
        {
            get { return _cellIndexs; }
        }

        /// <summary>
        /// Y轴查找表
        /// </summary>
        public int[] RowIndexLut
        {
            get { return _rowIndexs; }
        }

        private void InitAttributes()
        {
            _bandCount = _rasterBands.Count;
            _width = _rasterBands[0].Width;
            _height = _rasterBands[0].Height;
            _resolutionX = _rasterBands[0].ResolutionX;
            _resolutionY = _rasterBands[0].ResolutionY;
            _coordEnvelope = _rasterBands[0].CoordEnvelope;
            _spatialRef = _rasterBands[0].SpatialRef;
            if (_spatialRef != null)
            {
                if (_spatialRef.ProjectionCoordSystem != null)
                    _coordType = enumCoordType.PrjCoord;
                else
                    _coordType = enumCoordType.GeoCoord;
            }
            _dataType = _rasterBands[0].DataType;
            if(_dataIdentify!=null)
                _dataIdentify.Satellite = "ISCCP";
        }

        private void LoadBand()
        {
            _rasterBands = new List<IRasterBand>();
            for (int i = 0; i < 130; i++)
            {
                GPCRasterBand band = new GPCRasterBand(this, i + 1);
                band.Description = _bandNames[i];
                _rasterBands.Add(band);
            }
        }

        private void ReadGPCRow()
        {
            FileStream fsStream = null;
            BinaryReader binrayReader = null;
            try
            {
                fsStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                binrayReader = new BinaryReader(fsStream);
                _gcpRows = new GCPRow[67];
                for (int i = 0; i < 67; i++)                    //总共67行数据
                {
                    GCPRow row = new GCPRow();
                    row.Profix = binrayReader.ReadBytes(130);   //每行的前缀
                    byte[][] cellValues = new byte[99][];
                    for (int j = 0; j < 99; j++)                //99个Cell
                    {
                        cellValues[j] = binrayReader.ReadBytes(130);//每个Cell130个值
                    }
                    row.GridCell = cellValues;
                    _gcpRows[i] = row;
                }
            }
            finally
            {
                if (binrayReader != null)
                    binrayReader.Dispose();
                if (fsStream != null)
                    fsStream.Dispose();
            }
        }

        private void EQ2SQLUT(GCPRow[] EQMAP, out int[] cellIndexs, out int[] rowIndexs)
        {
            cellIndexs = new int[MAXLAT * MAXLON];
            rowIndexs = new int[MAXLAT * MAXLON];
            int[] equalLatLon = new int[MAXLAT * MAXLON];
            List<int> lstcellIndex = new List<int>();
            List<int> lstrowIndex = new List<int>();
            byte[][] rowdata = new byte[99][];
            int i, j, jlon;
            int lonIdxBegin, lonIdxend, latIdx;
            for (i = 0; i < EQMAP.Length; i++)                   //总共67行数据
            {
                GCPRow row = EQMAP[i];
                rowdata = row.GridCell;
                for (j = 0; j < rowdata.Length; j++)
                {
                    latIdx = rowdata[j][0];//Latitude index(equal-area and equal-angle),1-72
                    if (latIdx <= 72)
                    {
                        latIdx = 72 - latIdx;
                        lonIdxBegin = rowdata[j][2];//Western-most longitude index(equal-angle),1-144
                        lonIdxend = rowdata[j][3];//Eastern-most longitude index(equal-angle),1-144
                        for (jlon = lonIdxBegin; jlon <= lonIdxend; jlon++)//1-144
                        {
                            int lonidx = 0;
                            if (jlon > 72)
                                lonidx = jlon - 72;
                            else
                                lonidx = jlon + 72;
                            cellIndexs[(latIdx) * MAXLON + lonidx - 1] = j;//90~-90,
                            rowIndexs[(latIdx) * MAXLON + lonidx - 1] = i;
                        }
                    }
                }
            }
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override int[] GetDefaultBands()
        {
            return new int[] {8, 8, 8 };
        }

        public override object GetStretcher(int bandNo)
        {
            if(bandNo==5)
                return RgbStretcherFactory.CreateStretcher(enumDataType.Float, 0, 4);
            else
                return base.GetStretcher(bandNo);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
