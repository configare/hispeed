using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DF
{
    /*
      ENVI
      description = { File Imported into ENVI.}
      samples = 600
      lines   = 600
      bands   = 5
      header offset = 128
      file type = ENVI Standard
      data type = 12
      interleave = bsq
      sensor type = Unknown
      byte order = 0                           width/2 height/2
      map info = {Geographic Lat/Lon, 300, 300, 120.398750, 31.301250, 0.002500, 0.002500, WGS-84, units=Degrees}
      geo points = {1.0000, 1.0000, 32.048752, 119.651253,600, 1.000, 32.048752, 121.148750,1.0000, 600, 30.551250, 119.651253,600, 600, 30.551250, 121.148750}
      band names = {反射  1   620- 670 云/陆地边界  ,反射  2   841- 876 云/陆地边界 ,反射  6   1628-1652 云/陆地特性 ,辐射 31   10.78-11.28 地表/云顶温度 ,辐射 32   11.77-12.27 地表/云顶温度  }
     */
    public class HdrFile
    {
        public class Envelope : ICloneable
        {
            private double _minX;
            private double _maxX;
            private double _minY;
            private double _maxY;
            //
            public Envelope()
            {
            }

            public Envelope(double minX, double minY, double maxX, double maxY)
            {
                _minX = minX;
                _minY = minY;
                _maxX = maxX;
                _maxY = maxY;
                _centerPoint = PointF.Empty;
            }

            public void UnionWith(Envelope envelope)
            {
                _minX = Math.Min(_minX, envelope._minX);
                _minY = Math.Min(_minY, envelope._minY);
                _maxX = Math.Max(_maxX, envelope._maxX);
                _maxY = Math.Max(_maxY, envelope._maxY);
            }

            [DisplayName("最小经度"), ReadOnly(true)]
            public double MinX
            {
                get { return _minX; }
                set { _minX = value; }
            }

            [DisplayName("最小纬度"), ReadOnly(true)]
            public double MinY
            {
                get { return _minY; }
                set { _minY = value; }
            }

            [DisplayName("最大经度"), ReadOnly(true)]
            public double MaxX
            {
                get { return _maxX; }
                set { _maxX = value; }
            }

            [DisplayName("最大纬度"), ReadOnly(true)]
            public double MaxY
            {
                get { return _maxY; }
                set { _maxY = value; }
            }

            public bool IsEmpty()
            {
                return _minX == 0d && _minY == 0d && _maxX == 0d && _maxY == 0d;
            }

            public PointF LeftUpPoint
            {
                get { return new PointF((float)_minX, (float)_maxY); }
            }

            public PointF RightDownPoint
            {
                get { return new PointF((float)_maxX, (float)_minY); }
            }

            public PointF[] Points
            {
                get
                {
                    return new PointF[] { LeftUpPoint, new PointF((float)_maxX, (float)_maxY), RightDownPoint, new PointF((float)_minX, (float)_minY) };
                }
            }

            public bool IsContains(PointF pt)
            {
                return pt.X > _minX && pt.X < _maxX && pt.Y > _minY && pt.Y < _maxY;
            }

            public bool IsEqual(Envelope nEnvelope, double precisionX, double precisionY)
            {
                return Math.Abs(_maxX - nEnvelope.MaxX) < precisionX && Math.Abs(_maxY - nEnvelope._maxY) < precisionY
                    && Math.Abs(_minX - nEnvelope._minX) < precisionX && Math.Abs(_minY - nEnvelope._minY) < precisionY;
            }

            public bool IsInteractived(Envelope envelope)
            {
                if (envelope.MaxX < _minX ||
                    envelope.MinX > _maxX ||
                    envelope.MinY > _maxY ||
                    envelope._maxY < _minY)
                    return false;
                return true;
            }

            public bool IsInteractived(Envelope envelope, ref bool fullInternal)
            {
                if (envelope.MaxX < _minX || envelope.MinX > _maxX || envelope.MinY > _maxY || envelope._maxY < _minY)
                    return false;
                fullInternal = envelope.MinX > _minX && envelope._maxX < _maxX && envelope.MinY > _minY && envelope._maxY < _maxY;
                return true;
            }

            public Envelope IntersectWith(Envelope envelope)
            {
                RectangleF a = new RectangleF((float)_minX, (float)_minY, (float)_maxX - (float)_minX, (float)_maxY - (float)_minY);
                RectangleF b = new RectangleF((float)envelope._minX, (float)envelope._minY, (float)envelope._maxX - (float)envelope._minX, (float)envelope._maxY - (float)envelope._minY);
                a.Intersect(b);
                if (Math.Abs(a.Left) < float.Epsilon && Math.Abs(a.Top) < float.Epsilon && Math.Abs(a.Right) < float.Epsilon && Math.Abs(a.Bottom) < float.Epsilon)
                    return null;
                return new Envelope(a.Left, a.Top, a.Right, a.Bottom);
            }

            public bool Contains(Envelope envelope)
            {
                return envelope.MinX > _minX &&
                       envelope.MaxX < _maxX &&
                       envelope.MinY > _minY &&
                       envelope._maxY < _maxY;
            }

            private PointF _centerPoint;
            [Browsable(false)]
            public PointF CenterPoint
            {
                get
                {
                    if (_centerPoint == null)
                    {
                        _centerPoint = new PointF();
                        _centerPoint.X = (float)((_maxX + _minX) / 2d);
                        _centerPoint.Y = (float)((_maxY + _minY) / 2d);
                    }
                    return _centerPoint;
                }
            }

            [Browsable(false)]
            public double Width
            {
                get { return _maxX - _minX; }
            }

            [Browsable(false)]
            public double Height
            {
                get { return _maxY - _minY; }
            }

            public override string ToString()
            {
                return "{" + string.Format("MinLon:{0},MaxLon:{1},MinLat:{2},MaxLat:{3}",
                                               _minX.ToString("0.####"),
                                               _maxX.ToString("0.####"),
                                               _minY.ToString("0.####"),
                                               _maxY.ToString("0.####") + "}");
            }

            public static bool TryParse(string text, out Envelope envelope)
            {
                envelope = null;
                if (string.IsNullOrEmpty(text))
                    return false;
                string exp = @"^{MinLon:(?<MinLon>[-]?\d+(\.\d+)?),MaxLon:(?<MaxLon>[-]?\d+(\.\d+)?),MinLat:(?<MinLat>[-]?\d+(\.\d+)?),MaxLat:(?<MaxLat>[-]?\d+(\.\d+)?)}$";
                Match m = Regex.Match(text, exp);
                if (!m.Success)
                    return false;
                envelope = new Envelope(double.Parse(m.Groups["MinLon"].Value),
                                        double.Parse(m.Groups["MinLat"].Value),
                                        double.Parse(m.Groups["MaxLon"].Value),
                                        double.Parse(m.Groups["MaxLat"].Value));
                return true;
            }

            #region ICloneable 成员

            public object Clone()
            {
                return new Envelope(_minX, _minY, _maxX, _maxY);
            }

            #endregion

            /// <summary>
            /// 将该封套外扩delta个单位，返回扩大后的封套
            /// </summary>
            /// <param name="delta"></param>
            /// <returns></returns>
            public Envelope Expand(float delta)
            {
                return new Envelope(_minX - delta, _minY - delta, _maxX + delta, _maxY + delta);
            }

            public bool IsEquals(Envelope evp)
            {
                if (evp == null)
                    return false;
                return (Math.Abs(evp.MinX - MinX) < double.Epsilon) &&
                           (Math.Abs(evp.MinY - MinY) < double.Epsilon) &&
                           (Math.Abs(evp.MaxX - MaxX) < double.Epsilon) &&
                           (Math.Abs(evp.MaxY - MaxY) < double.Epsilon);
            }

            public bool IsGeoRange()
            {
                if (_minX < -180 ||
                  _maxX > 180 ||
                    _minY < -90 ||
                    _maxY > 90)
                    return false;
                return true;
            }

            public RectangleF ToRectangleF()
            {
                return RectangleF.FromLTRB((float)Math.Min(_minX, _maxX),
                                           (float)Math.Min(-_minY, -_maxY),//- is important
                                           (float)Math.Max(_minX, _maxX),
                                           (float)Math.Max(-_minY, -_maxY));
            }
        }
        //
        private string _description = "File Imported into ENVI";
        private int _samples = 0;
        private int _lines = 0;
        private int _bands = 0;
        private int _headerOffset = 0;
        private string _fileType = "ENVI Standard";
        private int _dataType = 12;
        private enumInterleave _interleave = enumInterleave.BIP;
        private int[] _majorFrameOffsets = {0,0};
        public string _satellite = null;
        private string _sensorType = "Unknown";
        private enumHdrByteOder _byteOrder = enumHdrByteOder.Host_intel;
        private HdrMapInfo _mapInfo = null;
        private HdrProjectionInfo _projectionInfo = null;
        private HdrGeoPoint[] _geoPoints = null;
        private string[] _bandNames = null;
        private string _regexStr = @"^(?<num>\d+)\S*$";
        private List<int> _bandNums = new List<int>();
        private string _filename = null;

        public HdrFile() { }

        public string Description
        {
            get { return _description; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _description = value;
            }
        }

        public int Samples
        {
            get { return _samples; }
            set { if (value > 0) _samples = value; }
        }

        public int Lines
        {
            get { return _lines; }
            set { if (value > 0) _lines = value; }
        }

        public int Bands
        {
            get { return _bands; }
            set { _bands = value; }
        }

        public int[] BandNums
        {
            get
            {
                if (_bandNames == null || _bandNames.Length == 0)
                    return null;
                if (_bandNums.Count != 0)
                    return _bandNums.ToArray();
                bool process = false;
                Regex reg = new Regex(_regexStr);
                foreach (string bandname in _bandNames)
                {
                    string[] tempNum = bandname.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempNum.Length == 1 && tempNum[0].ToLower().IndexOf("band") != -1)
                    {
                        _bandNums.Add(int.Parse(tempNum[0].ToLower().Trim().Replace("band", "")));
                        process = true;
                    }
                    else if (reg.IsMatch(tempNum[1].Trim()) && tempNum[1].IndexOf("-") == -1)
                    {
                        Match m = reg.Match(tempNum[1].Trim());
                        _bandNums.Add(int.Parse(m.Groups["num"].Value));
                        if (process == true)
                            process = false;
                    }
                    else if (reg.IsMatch(tempNum[0].Trim()))
                    {
                        Match m = reg.Match(tempNum[0].Trim());
                        _bandNums.Add(int.Parse(m.Groups["num"].Value));
                    }
                    else
                        _bandNums.Add(-1);
                }
                if (process)
                    ProcessModisBand();
                return _bandNums.Count == 0 ? null : _bandNums.ToArray();
            }
        }

        private void ProcessModisBand()
        {
            if (_bandNums == null || _bandNums.Count == 0 || _bandNums.IndexOf(38) == -1)
                return;
            int length = _bandNums.Count;
            for (int i = 0; i < length; i++)
            {
                if (_bandNums[i] > 16 && _bandNums[i] < 30000)
                    _bandNums[i] -= 2;
                else if (_bandNums[i] > 13 && _bandNums[i] < 17)
                    _bandNums[i] -= 1;
            }
        }

        public int HeaderOffset
        {
            get { return _headerOffset; }
            set { if (value > 0) _headerOffset = value; }
        }

        public string FileType
        {
            get { return _fileType; }
            set { if (!(string.IsNullOrEmpty(value))) _fileType = value; }
        }

        public int DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public enumInterleave Intertleave
        {
            get { return _interleave; }
            set { _interleave = value; }
        }

        public int[] MajorFrameOffsets
        {
            get { return _majorFrameOffsets;}
            set { _majorFrameOffsets = value; }
        }

        public string SensorType
        {
            get { return _sensorType; }
            set { if (!(string.IsNullOrEmpty(value)))  _sensorType = value; }
        }

        public enumHdrByteOder ByteOrder
        {
            get { return _byteOrder; }
            set { _byteOrder = value; }
        }

        public HdrMapInfo MapInfo
        {
            get { return _mapInfo; }
            set { _mapInfo = value; }
        }

        public HdrGeoPoint[] GeoPoints
        {
            get { return _geoPoints; }
            set { _geoPoints = value; }
        }

        public string[] BandNames
        {
            get { return _bandNames; }
            set { _bandNames = value; }
        }

        public Envelope GetEnvelope()
        {
            if (_mapInfo == null)
                return null;
            Envelope evp = new Envelope();
            double xwidth = _mapInfo.XYResolution.Longitude * _samples;
            double yheight = _mapInfo.XYResolution.Latitude * _lines;
            evp.MinX = _mapInfo.BaseMapCoordinateXY.Longitude - (_mapInfo.BaseRowColNumber.X - 1) * _mapInfo.XYResolution.Longitude;
            evp.MaxY = _mapInfo.BaseMapCoordinateXY.Latitude + (_mapInfo.BaseRowColNumber.Y - 1) * _mapInfo.XYResolution.Latitude;
            evp.MaxX = evp.MinX + xwidth;
            evp.MinY = Math.Round(evp.MaxY - yheight, 8);// by luoke添加Math.Round，防止出现很长无效值
            return evp;
        }

        public HdrProjectionInfo HdrProjectionInfo
        {
            get { return _projectionInfo; }
            set { _projectionInfo = value; }
        }

        public void Update()
        {
            if (_filename == null)
                return;
            SaveTo(_filename);
        }

        public void SaveTo(string fileName)
        {
            _filename = fileName;
            SaveTo(fileName, this);
        }

        public static int DataTypeToIntValue(Type type)
        {
            if (type == typeof(byte) || type == typeof(char))
                return 1;
            else if (type == typeof(Int16))
                return 2;
            else if (type == typeof(Int32))
                return 3;
            else if (type == typeof(float))
                return 4;
            else if (type == typeof(Double))
                return 5;
            else if (type == typeof(UInt16))
                return 12;
            else if (type == typeof(UInt32))
                return 13;
            else if (type == typeof(Int64))
                return 14;
            else if (type == typeof(UInt64))
                return 15;
            throw new Exception("指定的数据类型\"" + type.ToString() + "\"没有对应的hdr data type");
        }

        public static Type IntToDataTypeValue(int dataType)
        {
            if (dataType == 1)
                return typeof(byte);
            else if (dataType == 2)
                return typeof(Int16);
            else if (dataType == 3)
                return typeof(Int32);
            else if (dataType == 4)
                return typeof(float);
            else if (dataType == 5)
                return typeof(Double);
            else if (dataType == 12)
                return typeof(UInt16);
            else if (dataType == 13)
                return typeof(UInt32);
            else if (dataType == 14)
                return typeof(Int64);
            else if (dataType == 15)
                return typeof(UInt64);
            else if (dataType == 0)
                return typeof(UInt16);
            return null;
        }

        public static int GetDataSizeFromDataType(Type type)
        {
            if (type == typeof(byte))
                return 1;
            else if (type == typeof(Int16))
                return 2;
            else if (type == typeof(Int32))
                return 4;
            else if (type == typeof(float))
                return 4;
            else if (type == typeof(Double))
                return 8;
            else if (type == typeof(UInt16))
                return 2;
            else if (type == typeof(UInt32))
                return 4;
            else if (type == typeof(Int64))
                return 8;
            else if (type == typeof(UInt64))
                return 8;
            return 0;
        }

        public static int GetDataSizeFromDataType(int dataType)
        {
            if (dataType == 1)
                return 1;
            else if (dataType == 2)
                return 2;
            else if (dataType == 3)
                return 4;
            else if (dataType == 4)
                return 4;
            else if (dataType == 5)
                return 8;
            else if (dataType == 12)
                return 2;
            else if (dataType == 13)
                return 4;
            else if (dataType == 14)
                return 8;
            else if (dataType == 15)
                return 8;
            return 0;
        }

        public static HdrFile LoadFrom(string hdrfile)
        {
            return HdrFileParser.ParseFromHdrfile(hdrfile);
        }

        public static void SaveTo(string hdrfilename, HdrFile hdrfile)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(hdrfilename, false, Encoding.Default))
                {
                    sw.WriteLine("ENVI");
                    sw.WriteLine("description = {" + hdrfile.Description + "}");
                    sw.WriteLine(string.Format("samples = {0}", hdrfile.Samples));
                    sw.WriteLine(string.Format("lines = {0}", hdrfile.Lines));
                    sw.WriteLine(string.Format("bands = {0}", hdrfile.Bands));
                    sw.WriteLine(string.Format("header offset = {0}", hdrfile.HeaderOffset));
                    sw.WriteLine("major frame offsets = {" + hdrfile.MajorFrameOffsets[0] + "," + hdrfile.MajorFrameOffsets[1] + "}");
                    sw.WriteLine(string.Format("file type = {0}", hdrfile.FileType));
                    sw.WriteLine(string.Format("data type = {0}", hdrfile.DataType));
                    sw.WriteLine(string.Format("interleave = {0}", hdrfile.Intertleave.ToString().ToLower()));
                    sw.WriteLine(string.Format("sensor type = {0}", hdrfile.SensorType));
                    sw.WriteLine(string.Format("byte order = {0}", (int)hdrfile.ByteOrder));
                    if (hdrfile.MapInfo != null && !hdrfile.MapInfo.IsEmpty())
                    {
                        string mapInfos = "map info = {" + hdrfile.MapInfo.Name + "," +
                            hdrfile.MapInfo.BaseRowColNumber.X.ToString() + "," + hdrfile.MapInfo.BaseRowColNumber.Y.ToString() + "," +
                            hdrfile.MapInfo.BaseMapCoordinateXY.Longitude.ToString() + "," + hdrfile.MapInfo.BaseMapCoordinateXY.Latitude.ToString() + "," +
                            hdrfile.MapInfo.XYResolution.Longitude.ToString() + "," + hdrfile.MapInfo.XYResolution.Latitude.ToString() + "," +
                            hdrfile.MapInfo.CoordinateType + ",units = " + hdrfile.MapInfo.Units + "}";
                        sw.WriteLine(mapInfos);
                    }
                    if (hdrfile.HdrProjectionInfo != null)
                    {
                        sw.WriteLine(hdrfile.HdrProjectionInfo.ToString());
                    }
                    if (hdrfile.GeoPoints != null)
                    {
                        string sGeoPoints = "geo points = {";
                        foreach (HdrGeoPoint pt in hdrfile.GeoPoints)
                        {
                            sGeoPoints += (pt.PixelPoint.Y.ToString() + "," + pt.PixelPoint.X.ToString() + "," + pt.GeoPoint.Latitude.ToString() + pt.GeoPoint.Longitude.ToString() + ",");
                        }
                        if (sGeoPoints.EndsWith(","))
                            sGeoPoints = sGeoPoints.Substring(0, sGeoPoints.Length - 1);
                        sGeoPoints += "}";
                        sw.WriteLine(sGeoPoints);
                    }
                    if (hdrfile.BandNames != null)
                    {
                        string bandNames = string.Empty;
                        foreach (string name in hdrfile.BandNames)
                            bandNames += (name + ",");
                        if (bandNames.EndsWith(","))
                            bandNames = bandNames.Substring(0, bandNames.Length - 1);
                        sw.WriteLine("band names = {" + bandNames + "}");
                    }
                }
            }
            catch(Exception e)
            {
                return;
            }
        }

        public static string GetHdrFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".hdr");
        }
    }
}
