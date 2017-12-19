using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeoDo.RSS.DF.NOAA
{
    public class D1BDDataProvider : GDALRasterDataProvider, ID1BDDataProvider    
    {
        protected HdrFile _hdr = null;
        protected D1BDHeader _d1bdHeader = null;
        protected int _factOfLine = 0;
        private const int _sizeOfLine = 22016;   //bytes
        private const int _samples = 2048;
        private const int _offset = 22016;
        private const int _maxFrameOffset = 1264;
        private const int _minFrameOffset = 272;

        public D1BDDataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, null, driver, access)
        {
            InitDataIdentify();    
        }

        public D1BDDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, header1024, driver, access)
        {
            InitDataIdentify();
        }

        public HdrFile Hdr
        {
            get { return _hdr; }
        }

        public D1BDHeader Header
        {
            get { return _d1bdHeader; }
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
            _d1bdHeader = SetFileHeader.Set1BDHeader(fileName);
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
                if (_d1bdHeader.IsBigEndian)
                    _hdr.ByteOrder = enumHdrByteOder.Network_IEEE;
                else
                    _hdr.ByteOrder = enumHdrByteOder.Host_intel;
                _hdr.MajorFrameOffsets[0] = _maxFrameOffset;
                _hdr.MajorFrameOffsets[1] = _minFrameOffset;
                string fname = HdrFile.GetHdrFileName(this._fileName);
                _hdr.SaveTo(fname);
            }
        }

        /// <summary>
        /// noaa 1b数据判断升降轨的方式
        /// 1、使用头定义（目前，这种不保险了，因为听张里阳说是系统调整过，有的表示0升1降，有的1升0降）
        /// 2、根据经验，目前noaa9,11,14,16,18,19的下午星数据一般就是升轨的，
        ///    noaa12,15,17上午星时候就是降轨的。
        /// 轨道序号：目前NOAA 1bd里面没有合适的。
        /// </summary>
        private void InitDataIdentify()
        {
            _dataIdentify.Satellite = GetSatellite();
            _dataIdentify.Sensor = GetSensor();
            _dataIdentify.OrbitDateTime = _d1bdHeader.CommonInfoFor1BD.OrbitBeginTime;
            _dataIdentify.IsOrbit = true;
            _dataIdentify.IsAscOrbitDirection = _d1bdHeader.NomalHeaderInfo.AscOrbit;
        }

        private string GetSensor()
        {
            switch (_d1bdHeader.CommonInfoFor1BD.SensorIdentify)
            {
                case 0:
                    return "AVHRR";
                default:
                    return "AVHRR";
            }
        }

        /// <summary>
        /// 73-74: NOAA卫星识别码（4 = NOAA-15）
        /// </summary>
        private string GetSatellite()
        {
            switch (_d1bdHeader.CommonInfoFor1BD.SatelliteIdentify)
            { 
                case 4:
                    return "NOAA15";
                case 3:
                    return "NOAA16";
                case 11:
                    return "NOAA17";
                case 13:
                case 7:
                    return "NOAA18";
                default:
                    return "NOAA";
            }
        }
        
        /// <summary>
        /// 获取波段名列表
        /// </summary>
        /// <returns>波段名</returns>
        private string[] TryGetBandNames()
        {
            string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.NOAA.BandNames.xml");
            string[] bandNames= null;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.DF.NOAA.GeoDo.Rss.DF.NOAA.BandNames.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                doc.Save(configfile);
            }
            
            using (NOAABandNamesXmlParser xml = new NOAABandNamesXmlParser(configfile))
            {
                bandNames = xml.GetBandNames(this._d1bdHeader.CommonInfoFor1BD.SatelliteName,this._d1bdHeader.CommonInfoFor1BD.SensorName);
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
                if (_d1bdHeader.IsBigEndian)
                {
                    switch (j)
                    {
                        case 0:
                        case 2:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 10);
                            break;
                        case 1:
                        case 3:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 7);
                            break;
                        case 4:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef);
                            break;
                    }
                }
                else
                {
                    switch (j)
                    {
                        case 0:
                        case 2:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 10);
                            break;
                        case 1:
                        case 3:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 7);
                            break;
                        case 4:
                            coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef);
                            break;
                    }
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
                if(_d1bdHeader.IsBigEndian)
                coefficient[lineIndex, j + channelIndex * 3] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 6);  
                else
                    coefficient[lineIndex, j + channelIndex * 3] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 6);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #region ID1BDDataProvider Members

        /// <summary>
        /// 读取可见光定标系数
        /// </summary>
        public void ReadVisiCoefficient(ref double[,] operCoef, ref double[,] testCoef, ref double[,] beforeSendCoef)
        {
            #region 可见光定标系数(分组读取，共三组：业务用；测试用；发射前。)
            //业务用（按照通道1通道2通道3A的顺序对五个系数进行读取）
            operCoef = new double[_factOfLine, 15];
            //测试用
            testCoef = new double[_factOfLine, 15];
            //发射前
            beforeSendCoef = new double[_factOfLine, 15];
            #endregion
            int lineIndex = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(_offset + 48, SeekOrigin.Begin);
                    while (lineIndex < _factOfLine)
                    {
                        //i为通道索引，从0开始
                        for (int i = 0; i < 3; i++)
                        {
                            ExtractVisiCoefficient(br.ReadBytes(20), ref operCoef, lineIndex, i);
                            ExtractVisiCoefficient(br.ReadBytes(20), ref testCoef, lineIndex, i);
                            ExtractVisiCoefficient(br.ReadBytes(20), ref beforeSendCoef, lineIndex, i);
                        }
                        fs.Seek(_sizeOfLine - 180, SeekOrigin.Current);
                        lineIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// 读取IR定标系数
        /// </summary>
        /// <param name="ceofInfo">读取的二进制数据</param>
        /// <param name="operCoef">业务用系数数组</param>
        /// <param name="beforeSendCoef">发射前系数数组</param>
        public void ReadIRCoefficient(ref double[,] operCoef, ref double[,] beforeSendCoef)
        {
            #region IR定标系数(分组读取，共两组：业务用；发射前)
            //业务用（按照通道3B通道4通道5的顺序对三个系数进行读取）
            operCoef = new double[_factOfLine, 9];
            //发射前
            beforeSendCoef = new double[_factOfLine, 9];
            #endregion
            int lineIndex = 0;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(_offset + 228, SeekOrigin.Begin);
                    while (lineIndex < _factOfLine)
                    {
                        //i为通道索引，从0开始
                        for (int i = 0; i < 3; i++)
                        {
                            ExtractIRCoefficient(br.ReadBytes(12), ref operCoef, lineIndex, i);
                            ExtractIRCoefficient(br.ReadBytes(12), ref beforeSendCoef, lineIndex, i);
                        }
                        fs.Seek(_sizeOfLine - 72, SeekOrigin.Current);
                        lineIndex++;
                    }
                }
            }
        }
        #endregion

    }
}
