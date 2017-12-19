#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/20 11:01:28
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
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 类名：NA141BDataProvider
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/20 11:01:28
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class NA141BDataProvider : GDALRasterDataProvider, INA141BDataProvider
    {
        protected HdrFile _hdr = null;
        protected NA141BHeader _na141bHeader = null;
        protected int _factOfLine = 0;
        private const int _samples = 2048;
        private const int _sizeOfLine = 14800;
        private const int _offset = 22200;      //TBM 长度 7400+文件头长度7400*2
        private const int _maxFrameOffset = 308;
        private const int _minFrameOffset = 696;
        private FileStream _fsStream = null;
        private BinaryReader _binrayReader = null;

        public NA141BDataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, null, driver, access)
        {
            InitDataIdentify();
            _fsStream = new FileStream(fileName, FileMode.Open, access == enumDataProviderAccess.ReadOnly ? FileAccess.Read : FileAccess.ReadWrite);
            _binrayReader = new BinaryReader(_fsStream);
        }

        public NA141BDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, header1024, driver, access)
        {
            InitDataIdentify();
            _fsStream = new FileStream(fileName, FileMode.Open, access == enumDataProviderAccess.ReadOnly ? FileAccess.Read : FileAccess.ReadWrite);
            _binrayReader = new BinaryReader(_fsStream);
        }

        public HdrFile Hdr
        {
            get { return _hdr; }
        }

        public NA141BHeader Header
        {
            get { return _na141bHeader; }
        }

        /// <summary>
        /// 调用GDAL前生成.hdr文件
        /// </summary>
        protected override void CallGDALBefore()
        {
            base.CallGDALBefore();
            //_na141bHeader = SetFileHeader.Set1BDHeader(fileName);
            //generate hdr file
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                long len = fs.Length;
                _factOfLine = (int)(len / _sizeOfLine - 1);
                this._hdr = new HdrFile();
                _hdr.Lines = _factOfLine;
                //_hdr.BandNames = TryGetBandNames();
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

        private void InitDataIdentify()
        {
            _dataIdentify.Satellite = GetSatellite();
            _dataIdentify.Sensor = GetSensor();
            _dataIdentify.OrbitDateTime = _na141bHeader.CommonInfoForNA141B.OrbitBeginTime;
            _dataIdentify.IsOrbit = true;
            _dataIdentify.IsAscOrbitDirection = _na141bHeader.CommonInfoForNA141B.QualityCheckInfo.IsAscendOrbit;
        }

        private string GetSensor()
        {
            return "AVHRR";
        }

        private string GetSatellite()
        {
            switch (_na141bHeader.CommonInfoForNA141B.SatelliteIdentify)
            {
                case 12:
                    return "NOAA12";
                case 14:
                    return "NOAA14";
                default:
                    return "NOAA";
            }
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
            //int lineIndex = 0;
            //using (FileStream fs = new FileStream(fileName, FileMode.Open))
            //{
            //    using (BinaryReader br = new BinaryReader(fs))
            //    {
            //        fs.Seek(_offset + 48, SeekOrigin.Begin);
            //        while (lineIndex < _factOfLine)
            //        {
            //            //i为通道索引，从0开始
            //            for (int i = 0; i < 3; i++)
            //            {
            //                ExtractVisiCoefficient(br.ReadBytes(20), ref operCoef, lineIndex, i);
            //                ExtractVisiCoefficient(br.ReadBytes(20), ref testCoef, lineIndex, i);
            //                ExtractVisiCoefficient(br.ReadBytes(20), ref beforeSendCoef, lineIndex, i);
            //            }
            //            fs.Seek(_sizeOfLine - 180, SeekOrigin.Current);
            //            lineIndex++;
            //        }
            //    }
            //}
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
            //int lineIndex = 0;
            //using (FileStream fs = new FileStream(fileName, FileMode.Open))
            //{
            //    using (BinaryReader br = new BinaryReader(fs))
            //    {
            //        fs.Seek(_offset + 228, SeekOrigin.Begin);
            //        while (lineIndex < _factOfLine)
            //        {
            //            //i为通道索引，从0开始
            //            for (int i = 0; i < 3; i++)
            //            {
            //                ExtractIRCoefficient(br.ReadBytes(12), ref operCoef, lineIndex, i);
            //                ExtractIRCoefficient(br.ReadBytes(12), ref beforeSendCoef, lineIndex, i);
            //            }
            //            fs.Seek(_sizeOfLine - 72, SeekOrigin.Current);
            //            lineIndex++;
            //        }
            //    }
            //}
        }
        #endregion
    }
}
