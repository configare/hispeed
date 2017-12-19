using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.NOAA
{
    public class D1A5DataProvider : GDALRasterDataProvider, ID1A5DataProvider 
    {
        protected FileStream _fs = null;
        protected BinaryReader _br = null;
        protected HdrFile _hdr = null;
        protected D1A5Header _header = null;
        //原始数据经纬度值
        private double[] _lons = null;
        private double[] _lats = null;
        protected int _factOfLine = 0;
        private const int _sizeOfLine = 21980;   //bytes
        private const int _samples = 2048;
        private const int _offset = 21980;
        private const int _maxFrameOffset = 1500;
        private const int _minFrameOffset = 0;

        public D1A5DataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName,null, driver, access)
        {
            _dataIdentify.Satellite = GetSatellite();
            //_dataIdentify.Sensor = GetSensor();
            _dataIdentify.IsOrbit = true;
            if(_header!=null)
                _dataIdentify.IsAscOrbitDirection = (_header.AscDescendTag == 1);
        }

        public D1A5DataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, header1024, driver, access)
        {
            _dataIdentify.Satellite = GetSatellite();
            //_dataIdentify.Sensor = GetSensor();
            _dataIdentify.IsOrbit = true;
            if (_header != null)
                _dataIdentify.IsAscOrbitDirection = (_header.AscDescendTag == 1);
        }

        public HdrFile Hdr
        {
            get { return _hdr; }
        }

        public int FactOfLine
        {
            get { return _factOfLine; }
        }

        public D1A5Header Header
        {
            get { return _header; }
        }

        protected override void CallGDALBefore()
        { 
            base.CallGDALBefore();
            _header = SetFileHeader.Set1A5Header(fileName);
            //generate hdr file
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                long len = fs.Length;
                _factOfLine = (int)((len - _offset) / _sizeOfLine);
                this._hdr = new HdrFile();
                _hdr.Lines = _factOfLine;
                _hdr.BandNames = TryGetBandNames();
                _hdr.Bands = _hdr.BandNames.Count();
                _hdr.Samples = _samples;
                _hdr.HeaderOffset = _offset;
                _hdr.ByteOrder = enumHdrByteOder.Network_IEEE;
                _hdr.MajorFrameOffsets[0] = _maxFrameOffset;
                _hdr.MajorFrameOffsets[1] = _minFrameOffset;
                string fname = HdrFile.GetHdrFileName(this._fileName);
                _hdr.SaveTo(fname);
            }
        }


        /// <summary>
        /// 73-74: NOAA卫星识别码（4 = NOAA-15）
        /// </summary>
        private string GetSatellite()
        {
            switch (_header.SatelliteIdentify)
            {
                case 5:
                    return "NOAA-14";
                case 7:
                    return "NOAA-15";
                case 3:
                    return "NOAA-16";
                case 9:
                    return "NOAA-12";
                default:
                    return null;
            }
        }
        private string[] TryGetBandNames()
        {
            string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.NOAA.BandNames.xml");
            string[] bandNames = null;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.DF.NOAA.GeoDo.Rss.DF.NOAA.BandNames.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                doc.Save(configfile);
            }

            using (NOAABandNamesXmlParser xml = new NOAABandNamesXmlParser(configfile))
            {
                bandNames = xml.GetBandNames(this._header.SatelliteName);
            }
            return bandNames;
        }

        /// <summary>
        /// 读取未插值的经纬度数组
        /// </summary>
        public void ReadPointsGeoInfo()
        {
            _lons = new double[51 * _factOfLine];
            _lats = new double[51 * _factOfLine];
            int lineIndex = 1;
            _br = new BinaryReader(_fs);
            _fs.Seek(_offset + 260, SeekOrigin.Begin);
            while (lineIndex <= _factOfLine)
            {
                ExtractGeoInfo(_br.ReadBytes(408), ref _lons, ref _lats, lineIndex);
                _fs.Seek(_sizeOfLine - 408, SeekOrigin.Current);
                lineIndex++;
            }
        }

        /// <summary>
        /// 获取插值后的经纬度（第一行为纬度，第二行为经度）
        /// </summary>
        public double[,] GetLonLats()
        {
            ReadPointsGeoInfo();
            int pointNumber = 2048 * _factOfLine;
            double[,] latlons = new double[2, pointNumber];
            Parallel.For(0, _factOfLine, (lineIndex) =>
            {
                int m = 0;
                for (int pointIndex = 24; m < 50; m++, pointIndex += 40)
                {
                    double lonInterval = IntervalForGeoLocation(_lons[m + 51 * lineIndex], _lons[m + 51 * lineIndex + 1], 40);
                    double latInterval = IntervalForGeoLocation(_lats[m + 51 * lineIndex], _lats[m + 51 * lineIndex], 40);
                    for (int i = 0; i < 40; i++)
                    {
                        latlons[1, pointIndex + lineIndex * 2048 + i] = _lons[m + 51 * lineIndex] + i * lonInterval;
                        latlons[0, pointIndex + lineIndex * 2048 + i] = _lats[m + 51 * lineIndex] + i * latInterval;
                    }
                    if (m == 0)
                    {
                        for (int j = 0; j < 24; j++)
                        {
                            latlons[1, lineIndex * 2048 + j] = _lons[51 * lineIndex] - (24 - j) * lonInterval;
                            latlons[0, lineIndex * 2048 + j] = _lats[51 * lineIndex] - (24 - j) * latInterval;
                        }
                    }
                }
                if (m == 50)
                {
                    double lonIntervalLast = IntervalForGeoLocation(_lons[(m - 1) + 51 * lineIndex], _lons[m + 51 * lineIndex], 40);
                    double latIntervalLast = IntervalForGeoLocation(_lats[(m - 1) + 51 * lineIndex], _lats[m + 51 * lineIndex], 40);
                    for (int i = 0; i < 23; i++)
                    {
                        latlons[1, 2025 + lineIndex * 2048 + i] = _lons[m + 51 * lineIndex] + i * lonIntervalLast;
                        latlons[0, 2025 + lineIndex * 2048 + i] = _lats[m + 51 * lineIndex] + i * latIntervalLast;
                    }
                }
            });
            return latlons;
        }

        private void ExtractGeoInfo(byte[] geoInfo, ref double[] lons, ref double[] lats, int index)
        {
            for (int i = 0; i < 51; i++)
            {
                float v1 = BitConverter.ToSingle(geoInfo, i * 8);
                float v2 = BitConverter.ToSingle(geoInfo, i * 8 + 4);
                lons[i + (index - 1) * 51] = v2;
                lats[i + (index - 1) * 51] = v1;
            }          
        }
        
        /// <summary>
        /// 获取某个角度值数组
        /// </summary>
        /// <param name="angleInfo">读取的二进制值</param>
        /// <param name="angles">角度值数组</param>
        /// <param name="index">行索引</param>
        private void ExtractAngleInfo(byte[] angleInfo, ref double[] angles, int lineIndex)
        {
            byte[] byteAngle = new byte[4];
            for (int j = 0; j < 51; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                   angles[j + (lineIndex - 1) * 51] = BitConverter.ToSingle(angleInfo,i*4);
                }       
            }
        }

        /// <summary>
        /// 获取插值后的角度信息
        /// </summary>
        /// <returns>角度信息（第一行为太阳天顶角；第二行为卫星天顶角；第三行为相对方位角）</returns>
        public double[,] GetAnglesInfo()
        {
            //未插值前角度数组
            double[] solZ = new double[_factOfLine * 51];
            double[] senZ = new double[_factOfLine * 51];
            double[] azi = new double[_factOfLine * 51];
            //三行分别为三个角度：太阳天顶角；卫星天顶角；相对方位角
            double[,] angles = new double[3, _factOfLine * 2048];

            ReadAngleInfo(ref solZ, ref senZ, ref azi);
            Parallel.For(0, _factOfLine, (lineIndex) =>
            {
                int m = 0;
                for (int pointIndex = 24; m < 50; m++, pointIndex += 40)
                {
                    double solZInterval = IntervalForGeoLocation(solZ[m + 51 * lineIndex], solZ[m + 51 * lineIndex + 1], 40);
                    double senZInterval = IntervalForGeoLocation(senZ[m + 51 * lineIndex], senZ[m + 51 * lineIndex], 40);
                    double aziInterval = IntervalForGeoLocation(azi[m + 51 * lineIndex], azi[m + 51 * lineIndex], 40);
                    for (int i = 0; i < 40; i++)
                    {
                        angles[0, pointIndex + lineIndex * 2048 + i] = solZ[m + 51 * lineIndex] + i * solZInterval;
                        angles[1, pointIndex + lineIndex * 2048 + i] = senZ[m + 51 * lineIndex] + i * senZInterval;
                        angles[2, pointIndex + lineIndex * 2048 + i] = azi[m + 51 * lineIndex] + i * aziInterval;
                    }
                    if (m == 0)
                    {
                        for (int j = 0; j < 24; j++)
                        {
                            angles[0, lineIndex * 2048 + j] = solZ[51 * lineIndex] - (24 - j) * solZInterval;
                            angles[1, lineIndex * 2048 + j] = senZ[51 * lineIndex] - (24 - j) * senZInterval;
                            angles[2, lineIndex * 2048 + j] = azi[51 * lineIndex] - (24 - j) * aziInterval;
                        }
                    }
                }
                if (m == 50)
                {
                    double solZIntervalLast = IntervalForGeoLocation(solZ[(m - 1) + 51 * lineIndex], solZ[m + 51 * lineIndex], 40);
                    double senZIntervalLast = IntervalForGeoLocation(senZ[(m - 1) + 51 * lineIndex], senZ[m + 51 * lineIndex], 40);
                    double aziIntervalLast = IntervalForGeoLocation(azi[(m - 1) + 51 * lineIndex], azi[m + 51 * lineIndex], 40);
                    for (int i = 0; i < 23; i++)
                    {
                        angles[0, 2025 + lineIndex * 2048 + i] = solZ[m + 51 * lineIndex] + i * solZIntervalLast;
                        angles[1, 2025 + lineIndex * 2048 + i] = senZ[m + 51 * lineIndex] + i * senZIntervalLast;
                        angles[2, 2025 + lineIndex * 2048 + i] = azi[m + 51 * lineIndex] + i * aziIntervalLast;
                    }
                }
            });
            return angles;
        }

        /// <summary>
        /// 获取未插值的角度信息
        /// </summary>
        private void ReadAngleInfo(ref double[] solZ, ref double[] senZ, ref double[] azi)
        {
            int lineIndex = 1;
            _br = new BinaryReader(_fs);
            _fs.Seek(_offset + 28, SeekOrigin.Begin);
            while (lineIndex <= _factOfLine)
            {
                ExtractAngleInfo(_br.ReadBytes(204), ref solZ,lineIndex);
                _fs.Seek(614, SeekOrigin.Current);
                ExtractAngleInfo(_br.ReadBytes(204), ref senZ, lineIndex);
                ExtractAngleInfo(_br.ReadBytes(204),ref azi, lineIndex);
                lineIndex++;
            }
        }

        /// <summary>
        /// 读取可见光定标系数
        /// </summary>
        public void ReadVisiCoefficient(ref double[,] channel1Coef, ref double[,] channel2Coef, ref double[,] channel3Coef)
        {
            #region 可见光定标系数
            //按照通道1通道2通道3A的顺序对个系数进行读取
            channel1Coef = new double[_factOfLine, 5];
            //通道2
            channel2Coef = new double[_factOfLine, 5];
            //通道3
            channel3Coef = new double[_factOfLine, 5];
            #endregion
            int lineIndex = 0;
            if (_br == null)
            {
                _br = new BinaryReader(_fs);
            }
            _fs.Seek(_offset + 1282, SeekOrigin.Begin);
            while (lineIndex < _factOfLine)
            {
                ExtractVisiCoefficient(_br.ReadBytes(16), ref channel1Coef, lineIndex);
                ExtractVisiCoefficient(_br.ReadBytes(16), ref channel2Coef, lineIndex);
                ExtractVisiCoefficient(_br.ReadBytes(16), ref channel3Coef, lineIndex);
                _fs.Seek(_sizeOfLine - 48, SeekOrigin.Current);
                lineIndex++;
            }
        }

        private void ExtractVisiCoefficient(byte[] coefInfo, ref double[,] coefficient, int lineIndex)
        {
            byte[] coef = new byte[4];
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    coef[i] = coefInfo[i + j * 4];
                }
                coefficient[lineIndex, j] = ToLocalEndian_Core.ToFloatFromLittle(coefInfo);
            }

        }

        public void ReadIRCoefficient()
        {

        }

        private double IntervalForGeoLocation(double prevalue, double sufvalue, int number)
        {
            double interval = float.MinValue;
            interval = (sufvalue - prevalue) / number;
            return interval;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_fs != null)
            {
                _fs.Dispose();
                _fs = null;
            }
            if (_br != null)
            {
                _br.Close();
                _br = null;
            }
        }
    }
}
