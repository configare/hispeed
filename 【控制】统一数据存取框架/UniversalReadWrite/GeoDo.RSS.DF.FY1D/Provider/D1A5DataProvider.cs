#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/27 11:06:24
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：FY1_1A5DataProvider
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/27 11:06:24
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class FY1_1A5DataProvider:GDALRasterDataProvider,ID1A5DataProvider
    {
        protected FileStream _fs = null;
        protected BinaryReader _br = null;
        protected HdrFile _hdr = null;
        protected D1A5Header _header = null;
        //原始数据经纬度值
        private double[] _lons = null;
        private double[] _lats = null;
        protected int _factOfLine = 0;
        private const int _sizeOfLine = 9744;   //bytes
        private const int _samples = 1018;
        private const int _offset = 9744;
        private const int _maxFrameOffset = 1400;
        private const int _minFrameOffset = 200;

         public FY1_1A5DataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, null, driver, access)
        {
            InitDataIdentify();    
        }

         public FY1_1A5DataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, header1024, driver, access)
        {
            InitDataIdentify();
        }

        public HdrFile Hdr
        {
            get { return _hdr; }
        }

        public D1A5Header Header
        {
            get { return _header; }
        }

        public int FactOfLine
        {
            get { return _factOfLine; }
        }

        /// <summary>
        /// 调用GDAL前生成.hdr文件
        /// </summary>
        protected override void CallGDALBefore()
        {
            base.CallGDALBefore();
            _header = FileHeaderSetterFor1A5.Set1A5Header(fileName);
            //generate hdr file
            using ( FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                long len = fs.Length;
                _factOfLine = (int)(len / _sizeOfLine - 1);
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
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void GetPositionInfo(out double[] firstLon, out double[] firstLat, out double[] lastLon, out double[] lastLat)
        {
            FileStream fs = null;
            BinaryReader br = null;
            firstLon = new double[51];
            firstLat = new double[51];
            lastLon = new double[51];
            lastLat = new double[51];
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs, Encoding.Default);
                int recondOffset = 252;
                fs.Seek(_offset + recondOffset, SeekOrigin.Begin);
                ExtractLatAddLonInfo(br.ReadBytes(408), out firstLat, out firstLon);
                fs.Seek(_offset * (_factOfLine - 1) +recondOffset, SeekOrigin.Begin);
                ExtractLatAddLonInfo(br.ReadBytes(408), out lastLat, out lastLon);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (br != null)
                    br.Close();
            }
        }

        private void ExtractLatAddLonInfo(byte[] geoInfo,out double[] lats,out double[] lons)
        {
            byte[] lon = new byte[4];
            byte[] lat = new byte[4];
            lons = new double[51];
            lats = new double[51];
            int pt = 0;
            for (int j = 0; j < 51; j++)
            {
                for (int k=0,i=4; i < 8&&k<4; i++,k++)
                {
                    lon[pt] = geoInfo[i + 8 * j];
                    lat[pt++] = geoInfo[k + 8 * j];
                }
                pt = 0;
                lons[j] = (float)(ToLocalEndian.ToFloatFromBig(lon));
                lats[j] = (float)(ToLocalEndian.ToFloatFromBig(lat));
            }
        }

        /// <summary>
        /// 获取轨道根信息
        /// </summary>
        /// <returns></returns>
        public string GetHeaderInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("辅助信息:");
            sb.AppendLine("观测开始时间：" + _header.OrbitBeginTime);
            sb.AppendLine("观测开始时间（年）:" + _header.DataBeginYear);
            sb.AppendLine("观测开始时间（毫秒）:" + _header.DataBeginMilliSecond);
            sb.AppendLine("观测开始时间（天数）:" + _header.DataBeginDayNums);
            sb.AppendLine("观测结束时间（年）:" + _header.DataEndYear);
            sb.AppendLine("观测结束时间（毫秒）:" + _header.DataEndMilliSecond);
            sb.AppendLine("观测结束时间（天数）" + _header.DataEndDayNums);
            sb.AppendLine("观测开始时间（从80年开始以秒为单位）：" + _header.DataBeginSecond);
            sb.AppendLine("观测结束时间（从80年开始以秒为单位）：" + _header.DataEndSecond);
            sb.AppendLine("好扫描线数：" + _header.RecordCount);
            sb.AppendLine("末扫描线序号：" + _header.LastRecord);
            sb.AppendLine("同步码错数：" + _header.ErrorFrameCount);
            sb.AppendLine("误码率数：" + _header.BitErrorRatio);
            sb.AppendLine("时序错数：" + _header.ErrorTimeOrder);
            sb.AppendLine("扫描线丢失次数：" + _header.LostRecordCount);
            sb.AppendLine("斜坡分析结果：" + _header.SlopeAnalyseResult);
            sb.AppendLine("轨道序号：" + _header.TrackNumber);
            sb.AppendLine("历元轨道时间：" + _header.Time.ToString());
            sb.AppendLine("历元轨道时间（从80年开始以秒为单位）：" + _header.EpochTrackTime);
            sb.AppendLine("轨道半长轴：" + _header.OrbitSemiMajorAxis);
            sb.AppendLine("轨道偏心率：" + _header.OrbitEccentricity);
            sb.AppendLine("轨道倾角：" + _header.OrbitInclination);
            sb.AppendLine("轨道升交点赤经：" + _header.LongitudeAscendingNode);
            sb.AppendLine("轨道近地点幅角：" + _header.PerigeeAngle);
            sb.AppendLine("轨道平近点角：" + _header.MeanAnomaly);
            string ascDescend = _header.AscDescendTag == 0 ? "Desc" : "Asc";
            sb.AppendLine("升降轨标记：" + ascDescend);
            sb.AppendLine("定位所用资料类型：" + _header.ResurceType);
            sb.AppendLine("历元轨道序号：" + _header.OrbitNumber);
            sb.AppendLine("卫星轨道周期：" + _header.OrbitCycle);
            sb.AppendLine("姿态角_滚动角：" + _header.Angles[0]);
            sb.AppendLine("姿态角_俯仰角：" + _header.Angles[1]);
            sb.AppendLine("姿态角_偏航角：" + _header.Angles[2]);
            sb.AppendLine("四角经度(左下 右下  左上  右上)：" + string.Join(",", _header.Lons));
            sb.AppendLine("四角纬度(左下 右下  左上  右上)：" + string.Join(",", _header.Lats));
            return sb.ToString();
        }

        private void InitDataIdentify()
        {
            _dataIdentify.Satellite = GetSatellite();
            _dataIdentify.Sensor = GetSensor();
            _dataIdentify.OrbitDateTime = _header.OrbitBeginTime;
            _dataIdentify.IsOrbit = true;
            _dataIdentify.IsAscOrbitDirection = (_header.AscDescendTag==1);
        }

        private string GetSensor()
        {
            return "AVHRR";
        }

        /// <summary>
        /// 0-1: 卫星识别码
        /// </summary>
        private string GetSatellite()
        {
            switch (_header.SatelliteIdentify)
            { 
                case 114:
                    return "FY1D";
                case 113:
                    return "FY1C";
                default:
                    return "FY1D";
            }
        }
        
        /// <summary>
        /// 获取波段名列表
        /// </summary>
        /// <returns>波段名</returns>
        private string[] TryGetBandNames()
        {
            string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.FY1D.BandNames.xml");
            string[] bandNames = null;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.DF.FY1D.GeoDo.RSS.DF.FY1D.BandNames.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                doc.Save(configfile);
            }
            using (FY1DBandNamesXmlParser xml = new FY1DBandNamesXmlParser(configfile))
            {
                bandNames = xml.GetBandNames(_header.SatelliteName, GetSensor());
            }
            return bandNames;
        }

        private void ExtractVisiCoefficient(byte[] coefInfo,ref double[,] coefficient,int lineIndex,int channelIndex)
        {
            byte[] coef = new byte[4];
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    coef[i] = coefInfo[i+j*4];
                }
                switch (j)
                {
                    case 0:
                    case 2:
                        coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian.ToInt32FromLittle(coef) / Math.Pow(10, 10);
                        break;
                    case 1:
                    case 3:
                        coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian.ToInt32FromLittle(coef) / Math.Pow(10, 7);
                        break;
                    case 4:
                        coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian.ToInt32FromLittle(coef);
                        break;
                }
            }

        }

        private void ExtractIRCoefficient(byte[] coefInfo,ref double[,] coefficient,int lineIndex,int channelIndex)
        {
            byte[] coef = new byte[4];
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    coef[i] = coefInfo[i + j * 4];
                }
                coefficient[lineIndex, j + channelIndex * 3] = (float)ToLocalEndian.ToInt32FromLittle(coef) / Math.Pow(10, 6);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
