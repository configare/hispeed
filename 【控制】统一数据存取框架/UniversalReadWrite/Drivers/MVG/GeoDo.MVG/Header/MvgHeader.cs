using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.DF.MVG;
using System.Drawing;
using GeoDo.RSS.DF.LDF;

namespace GeoDo.RSS.DF.MVG
{
    public class MvgHeader 
    {
        protected ISpatialReference _spatialRef = null;
        protected HdrMapInfo _mapInfo = null;

        #region  Define the head
        /// <summary>
        /// 1-2:版本号
        /// </summary>
        protected Int16 _version = -1;
        /// <summary>
        /// 3-4:多值图文件标识，固定为字符串“MV”//2
        /// </summary>
        protected char[] _fileID = new char[2] { 'M', 'V' };
        /// <summary>
        /// 5-6:多值图文件头大小(多值的数目对应的文件头大小 1—102 + 20*4；2—140 + 20*4；3—178 + 20*4 byte）
        /// </summary>
        public Int16 HeaderSize = 0;
        /// <summary>
        /// 7-16:多值图文件创建时间,年月日时分//10bit
        /// </summary>
        protected DateTime _createdDateTime = DateTime.MinValue;
        /// <summary>
        /// 17-18:多值的数目
        /// </summary>
        protected Int16 _valueCount = 0;
        /// <summary>
        /// 多值图文件内的具体值
        /// </summary>
        protected Int16[] _values = null;
        /// <summary>
        /// 具体值对应的名称和阈值 例如：云：110；陆地：120；水：0~100 //Values*32
        /// </summary>
        public string[] ValueNames = null;
        /// <summary>
        /// 是否使用颜色表  //2
        /// </summary>
        public bool IsUseColorTable = false;
        /// <summary>
        /// 颜色表信息  //valueCount *2
        /// </summary>
        protected int[] _colorTable = null;
        /// <summary>
        /// 投影方式 0: 不投影, 1: 等角投影，2: 麦卡托投影, 3: 兰布托投影，4: 极射赤面投影 //2
        /// </summary>
        public  Int16 ProjectType = 0;
        /// <summary>
        /// 多值图文件列数  //2
        /// </summary>
        protected Int16 _width = 0;
        /// <summary>
        /// 多值图文件行数 //2
        /// </summary>
        protected Int16 _height = 0;
        /// <summary>
        /// 分辨率单位  0：度，1：米，2：千米  //2
        /// </summary>
        public Int16 ResolutionUnit = 0;
        /// <summary>
        /// 经度分辨率 等角投影或x分辨率 -- 麦卡托、兰布托、极射赤面投影  //4
        /// </summary>
        public float LongitudeResolution = 0;
        /// <summary>
        /// 纬度分辨率 等角投影或y分辨率 -- 麦卡托、兰布托、极射赤面投影 //4
        /// </summary>
        public float LatitudeResolution = 0;
        /// <summary>
        /// /标准纬度1--麦卡托、兰布托、极射赤面投影有效  //4
        /// </summary>
        public float StandardLatitude1 = 0;
        /// <summary>
        /// 标准纬度2 -- 兰布托投影有效 //4
        /// </summary>
        public float StandardLatitude2 = 0;
        /// <summary>
        /// 地球半径--麦卡托、兰布托、极射赤面投影有效  //4
        /// </summary>
        public float EarthRadius = 0;
        /// <summary>
        /// 最小纬度
        /// </summary>
        public float MinLat = 0;
        /// <summary>
        /// 最大纬度
        /// </summary>
        public float MaxLat = 0;
        /// <summary>
        /// 最小经度
        /// </summary>
        public float MinLon = 0;
        /// <summary>
        /// 最大经度
        /// </summary>
        public float MaxLon = 0;
        /// <summary>
        /// 多值图文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 投影参数信息,一期版本2,按数组获取投影参数
        /// </summary>
        protected float[] _prjArguments = new float[20];
        /// <summary>
        /// 二期进行扩展，版本3
        /// </summary>
        public string GeoDoIdentify = null;//Length = 5,['G','E','O','D','O']
        public Int16 PrjId = 0;  //2
        public float A = 0;  //4
        public float B = 0;  //4
        public float Sp1 = 0;  //4
        public float Sp2 = 0;  //4
        public float Lat0 = 0;  //4
        public float Lon0 = 0;  //4
        public float X0 = 0;   //4
        public float Y0 = 0;  //4
        public float K0 = 0;  //4
        public byte[] GeoDoSkip = null; //37

        protected bool _isExtend = false;

        #endregion

        #region Constractions
        public MvgHeader()
        {
        }

        /// <summary>
        /// 通过打开文件创建头对象
        /// </summary>
        /// <param name="fname"></param>
        public MvgHeader(string fname)
        {
            Stream stream = new FileStream(fname, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(stream);
            FillFieldsByMvgHeaderStream(br);
            Dispose(stream, br);
        }

        /// <summary>
        /// 通过若干个头字节创建头对象
        /// </summary>
        /// <param name="header"></param>
        public MvgHeader(byte[] header)
        {
            Stream stream = new MemoryStream(header);
            BinaryReader br = new BinaryReader(stream);
            FillFieldsByMvgHeaderStream(br);
            Dispose(stream, br);
        }
        #endregion

        #region Attributes
        public Int16 Version
        {
            get { return _version; }
        }

        public char[] FileID
        {
            get { return _fileID; }
        }

        public DateTime CreatedDateTime
        {
            get
            {
                return _createdDateTime;
            }
            set
            {
                if (value == null)
                {
                    value = DateTime.Now;
                }
                _createdDateTime = value;
            }
        }

        public Int16 ValueCount
        {
            get
            {
                return _valueCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("多值图内的值数目不能设置为负！");
                }
                HeaderSize = Convert.ToInt16(64 + 38 * value);
                _valueCount = value;
            }
        }

        public Int16[] Values
        {
            get
            {
                return _values;
            }
            set
            {
                HeaderSize = Convert.ToInt16(64 + 38 * value.Length + 20 * 4);//32个字符的NAME,4个字节颜色值,2个字节的具体值
                _valueCount = (Int16)value.Length;
                _values = value;
            }
        }

        public int[] ColorTable
        {
            get
            {
                return _colorTable;
            }
            set
            {
                if (value.Length != _valueCount)
                {
                    throw new Exception("多值图内的颜色表数组的长度与多值数量设置不一致！");
                }
                _colorTable = value;
            }
        }

        public Int16 Width
        {
            get { return _width; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("多值图内的列数不能设置为负！");
                }
                _width = value;
            }
        }

        public Int16 Height
        {
            get { return _height; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("多值图内的行数不能设置为负！");
                }
                _height = value;
            }
        }

        public float[] PrjArguments
        {
            get { return _prjArguments; }
            set
            {
                if (value == null || value.Length == 0)
                    return;
                int n = Math.Min(20, value.Length);
                for (int i = 0; i < n; i++)
                    _prjArguments[i] = value[i];
            }
        }

        #endregion

        public ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }

        public bool IsExtend
        {
            get { return _isExtend; }
            set
            {
                if (value)
                {
                    _isExtend = value;
                    _version = 3;
                }
            }
        }

        public static bool IsMVG(byte[] bytes, string fileExtension)
        {
            if (fileExtension != ".MVG")
                return false;
            else if (bytes[2] == 'M' && bytes[3] == 'V')
                return true;
            else
                return false;
        }

        public HdrFile ToHdrFile()
        {
            HdrFile hdr = new HdrFile();
            hdr.Samples = _width;
            hdr.Lines = _height;
            hdr.DataType = HdrFile.DataTypeToIntValue(typeof(Int16));
            hdr.Intertleave = enumInterleave.BSQ;
            hdr.Bands = 1;
            hdr.HeaderOffset = HeaderSize;
            hdr.ByteOrder = enumHdrByteOder.Host_intel;
            hdr.HdrProjectionInfo = GetHdrProjectionInfo();
            hdr.MapInfo = GetHdrMapInfo();
            hdr.GeoPoints = null;
            return hdr;
        }

        public byte[] ToBytes()
        {
            InitHeaderBySpatialRef(_spatialRef);
            byte[] bytes = new byte[HeaderSize];
            try
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        _version = 3;
                        bw.Write(Convert.ToInt16(_version));   //2
                        bw.Write(_fileID);  //2
                        bw.Write(Convert.ToInt16(HeaderSize));
                        bw.Write(Convert.ToInt16(_createdDateTime.Year));
                        bw.Write(Convert.ToInt16(_createdDateTime.Month));
                        bw.Write(Convert.ToInt16(_createdDateTime.Day));
                        bw.Write(Convert.ToInt16(_createdDateTime.Hour));
                        bw.Write(Convert.ToInt16(_createdDateTime.Minute));
                        bw.Write(Convert.ToInt16(_valueCount));
                        for (int i = 0; i < _valueCount; i++)
                        {
                            bw.Write(Convert.ToInt16(_values[i]));
                        }
                        byte[] valueName = new byte[32];

                        string temp = string.Empty;
                        for (int i = 0; i < _valueCount; i++)
                        {
                            temp = ValueNames[i];
                            byte[] value = Encoding.Default.GetBytes(temp);
                            if (value.Length < 32)
                            {
                                value.CopyTo(valueName, 0);
                            }
                            bw.Write(valueName);
                        }
                        bw.Write(Convert.ToInt16(IsUseColorTable));
                        for (int i = 0; i < _valueCount; i++)
                        {
                            if (!IsUseColorTable)
                            {
                                bw.Write(int.MinValue);
                            }
                            else
                            {
                                bw.Write(_colorTable[i]);
                            }
                        }

                        bw.Write(Convert.ToInt16(ProjectType));
                        bw.Write(Convert.ToInt16(_width));
                        bw.Write(Convert.ToInt16(_height));
                        bw.Write(Convert.ToInt16(ResolutionUnit));
                        bw.Write(LongitudeResolution);
                        bw.Write(LatitudeResolution);
                        bw.Write(StandardLatitude1);
                        bw.Write(StandardLatitude2);
                        bw.Write(EarthRadius);
                        bw.Write(Convert.ToSingle(MinLat));
                        bw.Write(Convert.ToSingle(MaxLat));
                        bw.Write(Convert.ToSingle(MinLon));
                        bw.Write(Convert.ToSingle(MaxLon));
                        /*
                         * 写投影参数
                         * 二期重新扩展,48字节
                         */
                        InitHeaderBySpatialRef(_spatialRef);
                        bw.Write(new char[] { 'G', 'E', 'O', 'D', 'O' });
                        bw.Write(PrjId);
                        bw.Write(A);
                        bw.Write(B);
                        bw.Write(Sp1);
                        bw.Write(Sp2);
                        bw.Write(Lat0);
                        bw.Write(Lon0);
                        bw.Write(X0);
                        bw.Write(Y0);
                        bw.Write(K0);
                        bw.Write(new byte[37]);
                        //
                    }
                }
                return bytes;
            }
            catch
            {
                throw new Exception();
            }
        }

        private void InitHeaderBySpatialRef(ISpatialReference spatialRef)
        {
            if (spatialRef == null)
                return;
            if (spatialRef.GeographicsCoordSystem == null || spatialRef.ProjectionCoordSystem == null)
            {
                PrjId = 1;//Geographic
            }
            else
            {
                Datum datum = spatialRef.GeographicsCoordSystem.Datum ?? new Datum();
                PrjId = Int16.Parse(spatialRef.ProjectionCoordSystem.Name.ENVIName);
                A = (float)datum.Spheroid.SemimajorAxis;
                B = (float)datum.Spheroid.SemiminorAxis;
                K0 = (float)datum.Spheroid.InverseFlattening;
                IProjectionCoordSystem prjCoordSystem = spatialRef.ProjectionCoordSystem;
                Sp1 = GetPrjParaValue(prjCoordSystem, "sp1");
                Sp2 = GetPrjParaValue(prjCoordSystem, "sp2");
                Lat0 = GetPrjParaValue(prjCoordSystem, "lat0");
                Lon0 = GetPrjParaValue(prjCoordSystem, "lon0");
                X0 = GetPrjParaValue(prjCoordSystem, "x0");
                Y0 = GetPrjParaValue(prjCoordSystem, "y0");
            }
        }

        private float GetPrjParaValue(IProjectionCoordSystem prjCoordSystem, string paraName)
        {
            NameValuePair v = prjCoordSystem.GetParaByName(paraName);
            return v != null ? (float)v.Value : 0f;
        }

        internal void SetMapInfo(HdrMapInfo mapInfo)
        {
            if (mapInfo == null)
                return;
            LongitudeResolution = (float)mapInfo.XYResolution.Longitude;
            LatitudeResolution = (float)mapInfo.XYResolution.Latitude;
            MinLon = (float)mapInfo.BaseMapCoordinateXY.Longitude - LongitudeResolution * (mapInfo.BaseRowColNumber.X - 1);/*ENVI pixel from 1*/
            MaxLat = (float)mapInfo.BaseMapCoordinateXY.Latitude + LatitudeResolution * (mapInfo.BaseRowColNumber.Y - 1);
            MaxLon = MinLon + LongitudeResolution * _width;
            MinLat = MaxLat - LatitudeResolution * _height;
        }

        private void FillFieldsByMvgHeaderStream(BinaryReader br)
        {
            int version = br.ReadInt16();
            br.ReadChars(2);
            HeaderSize = br.ReadInt16();
            _createdDateTime = new DateTime(br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), 0);
            _valueCount = br.ReadInt16();
            _values = new Int16[_valueCount];
            for (int i = 0; i < _valueCount; i++)
            {
                _values[i] = br.ReadInt16();
            }
            byte[] valueNames = new byte[_valueCount];
            string[] vn = new string[_valueCount];
            for (int i = 0; i < _valueCount; i++)
            {
                valueNames = br.ReadBytes(32);
                vn[i] = Encoding.Default.GetString(valueNames);
            }
            ValueNames = vn;
            IsUseColorTable = Convert.ToBoolean(br.ReadInt16());
            _colorTable = new int[_valueCount];
            for (int i = 0; i < _valueCount; i++)
            {
                _colorTable[i] = br.ReadInt32();
            }
            ProjectType = br.ReadInt16();
            _width = br.ReadInt16();
            _height = br.ReadInt16();
            ResolutionUnit = br.ReadInt16();
            LongitudeResolution = br.ReadSingle();
            LatitudeResolution = br.ReadSingle(); ;
            StandardLatitude1 = br.ReadSingle();
            StandardLatitude2 = br.ReadSingle();
            EarthRadius = br.ReadSingle();
            MinLat = br.ReadSingle();
            MaxLat = br.ReadSingle();
            MinLon = br.ReadSingle();
            MaxLon = br.ReadSingle();
            //一期版本2按字节读投影参数
            if (version == 2)
            {
                for (int i = 0; i < 20; i++)
                    _prjArguments[i] = br.ReadSingle();
            }
            //二期扩展投影参数的定义(与Ldf保持一致)，版本3
            else if (version == 3)
            {
                GeoDoIdentify = DataTypeConvertor.CharsToString(br.ReadChars(5));
                if (GeoDoIdentify != "GEODO")
                {
                    br.ReadBytes(73);
                    GetSpatialRefFromHeaderDefault(ProjectType, StandardLatitude1, StandardLatitude2, 0);
                }
                else
                {
                    PrjId = br.ReadInt16();
                    A = br.ReadSingle();
                    B = br.ReadSingle();
                    Sp1 = br.ReadSingle();
                    Sp2 = br.ReadSingle();
                    Lat0 = br.ReadSingle();
                    Lon0 = br.ReadSingle();
                    X0 = br.ReadSingle();
                    Y0 = br.ReadSingle();
                    K0 = br.ReadSingle();
                    GetSpatialRefFromHeader_GeoDo();
                }
            }
        }

        private void GetSpatialRefFromHeader_GeoDo()
        {
            using (SpatialReferenceBuilder builder = new SpatialReferenceBuilder())
            {
                //与Ldf头中投影方式的定义相同
                _spatialRef = builder.GetSpatialRef(PrjId, Sp1, Sp2, Lat0, Lon0, X0, Y0, K0);
            }
        }

        private void GetSpatialRefFromHeaderDefault(int prjType, float sp1, float sp2, float standarLon)
        {
            using (SpatialReferenceBuilder builder = new SpatialReferenceBuilder())
            {
                _spatialRef = builder.GetSpatialRef(prjType, sp1, sp2, standarLon);
            }
        }

        private HdrMapInfo GetHdrMapInfo()
        {
            HdrMapInfo mapInfo = new HdrMapInfo();
            if (_spatialRef != null && _spatialRef.ProjectionCoordSystem != null)
            {
                mapInfo.Name = GetMapInfoName();
                mapInfo.Units = "Meters";
            }
            mapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(MinLon, MaxLat);
            mapInfo.BaseRowColNumber = new Point(1, 1);
            mapInfo.CoordinateType = GetCoordinateSystemName();
            mapInfo.XYResolution = new HdrGeoPointCoord(Double.Parse(LongitudeResolution.ToString()), Double.Parse(LatitudeResolution.ToString()));//低精度向高精度转化
            return mapInfo;
        }

        private string GetCoordinateSystemName()
        {
            if (_spatialRef == null || _spatialRef.GeographicsCoordSystem == null || _spatialRef.GeographicsCoordSystem.Name == null)
                return "WGS-84";
            return _spatialRef.GeographicsCoordSystem.Name;
        }

        public void GetCoordEnvelope(out double minX, out double minY, out double maxX, out double maxY)
        {
            minX = MinLon;
            minY = MinLat;
            maxX = MaxLon;
            maxY = MaxLat;
        }

        private string GetMapInfoName()
        {
            if (_spatialRef.ProjectionCoordSystem != null && _spatialRef.ProjectionCoordSystem.Name != null)
                return _spatialRef.ProjectionCoordSystem.Name.WktName ?? string.Empty;
            return string.Empty;
        }

        private HdrProjectionInfo GetHdrProjectionInfo()
        {
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
                return null;
            HdrProjectionInfo prjInfo = new HdrProjectionInfo();
            prjInfo.Datum = _spatialRef.GeographicsCoordSystem.Datum.Name;
            prjInfo.Name = _spatialRef.Name ?? string.Empty;
            prjInfo.ProjectionID = int.Parse(_spatialRef.ProjectionCoordSystem.Name.ENVIName);
            float[] args = null;
            string units = null;
            _spatialRef.ToEnviProjectionInfoString(out args, out units);
            prjInfo.PrjArguments = args;
            prjInfo.Units = units;
            return prjInfo;
        }


        private void Dispose(Stream stream, BinaryReader br)
        {
            if (br != null)
            {
                br.Dispose();
                br = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
