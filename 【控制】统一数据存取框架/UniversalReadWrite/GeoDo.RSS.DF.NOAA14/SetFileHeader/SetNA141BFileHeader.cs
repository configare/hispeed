#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/21 16:08:17
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

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 类名：SetNA141BFileHeader
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/21 16:08:17
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SetNA141BFileHeader
    {
        private const int cstTBMInfoOffset = 75;
        private const int cstTBMInfoEndOffset = 7400;
        private const int cstCommonInfoOffset = 7400;
        private const int cstCommonInfoEndOffset = 14800;

        public SetNA141BFileHeader()
        {
        }

        public object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {

            NA141BHeader hInfo = new NA141BHeader(
                new SetNA141BFileHeaderTBDInfo_NA14().Create(fileStream, binaryReader, cstTBMInfoOffset, cstTBMInfoEndOffset),
                new SetNA141BFileHeaderCommonInfo_NA14().Create(fileStream, binaryReader, cstCommonInfoOffset, cstCommonInfoEndOffset));
            return hInfo;
        }
    }

    public class SetNA141BFileHeaderTBDInfo_NA14 : SectionHandler
    {
        public SetNA141BFileHeaderTBDInfo_NA14()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            TBMInfoForNA141B tbdInfo = new TBMInfoForNA141B();
            fileStream.Seek(offset, SeekOrigin.Begin);
            tbdInfo.MinLat = ToLocalEndian.ToDouble64FromBig(binaryReader.ReadBytes(3));
            tbdInfo.MaxLat = ToLocalEndian.ToDouble64FromBig(binaryReader.ReadBytes(3));
            tbdInfo.MinLon = ToLocalEndian.ToDouble64FromBig(binaryReader.ReadBytes(4));
            tbdInfo.MaxLon = ToLocalEndian.ToDouble64FromBig(binaryReader.ReadBytes(4));
            tbdInfo.BeginHour = ToLocalEndian.ToInt16FromBig(binaryReader.ReadBytes(2));
            tbdInfo.BeginMinite = ToLocalEndian.ToInt16FromBig(binaryReader.ReadBytes(2));
            tbdInfo.DataTime = ToLocalEndian.ToInt32FromBig(binaryReader.ReadBytes(3));
            tbdInfo.IsAddData = ToLocalEndian.ToInt16FromBig(binaryReader.ReadBytes(1)) == 1 ? true : false;      
            base.Create(fileStream, binaryReader, offset, endOffset);
            return tbdInfo;
        }
    }

    public class SetNA141BFileHeaderCommonInfo_NA14 : SectionHandler
    {

        public SetNA141BFileHeaderCommonInfo_NA14()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            CommonInfoForNA141B commonInfo = new CommonInfoForNA141B();
            fileStream.Seek(offset, SeekOrigin.Begin);
            commonInfo.SatelliteIdentify = binaryReader.ReadByte();
            commonInfo.InformationType = binaryReader.ReadByte();
            commonInfo.OrbitBeginTime = GetOrbitTime(binaryReader.ReadBytes(6));
            commonInfo.RecordCount = ToLocalEndian.ToUInt16FromLittle(binaryReader.ReadBytes(2));
            commonInfo.OrbitFinishTime = GetOrbitTime(binaryReader.ReadBytes(6));
            commonInfo.OrbitOrderCode = ToLocalEndian.ReadString2(binaryReader.ReadChars(56));
            commonInfo.SlopeCorrection = binaryReader.ReadByte();
            commonInfo.MissCount = binaryReader.ReadByte();
            commonInfo.QualityCheckInfo=GetAualityCheckInfo(binaryReader.ReadBytes(6));
            commonInfo.CalibrationParameters = ToLocalEndian.ToUInt16FromLittle(binaryReader.ReadBytes(2));
            return commonInfo;
        }

        private DateTime GetOrbitTime(byte[] timeBytes)
        {
            //7bit为年份，9bit为天数，后三字节为毫秒数
            if (timeBytes == null || timeBytes.Length != 6)
                return DateTime.MinValue;
            BitStream bs = new BitStream(new MemoryStream(timeBytes));
            uint year;
            int bitIndex = 0, bitCount = 7;
            bs.Read(out year, bitIndex, bitCount);
            uint dayCount;
            bitIndex = 7;
            bitCount = 9;
            bs.Read(out dayCount, bitIndex, bitCount);
            uint msCount;
            bitIndex = 24;
            bitCount = 27;
            bs.Read(out msCount, bitIndex, bitCount);
            DateTime time = DateTime.Parse(year.ToString() + "-01-01").AddDays(dayCount - 1).AddMilliseconds(msCount);
            return time;
        }

        private QualityCheckInfoForNA141B GetAualityCheckInfo(byte[] infoBytes)
        {
            QualityCheckInfoForNA141B checkInfo = new QualityCheckInfoForNA141B();
            BitStream bs = new BitStream(new MemoryStream(infoBytes));
            bool value;
            bs.Read(out value);
            checkInfo.IsValid = value;
            bs.Read(out value);
            checkInfo.IsErrorTime = value;
            bs.Read(out value);
            checkInfo.IsOverBorder = value;
            bs.Read(out value);
            checkInfo.IsRepeat = value;
            bs.Read(out value);
            checkInfo.IsErrorLocation = value;
            bs.Read(out value);
            checkInfo.IsErrorEarthLocation = value;
            bs.Read(out value);
            checkInfo.IsAscendOrbit = value;
            bs.Read(out value);
            checkInfo.IsNotNoise = value;
            return checkInfo;
        }
    }
}
