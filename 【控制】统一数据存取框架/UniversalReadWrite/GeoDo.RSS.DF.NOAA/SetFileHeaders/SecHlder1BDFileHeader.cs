using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace GeoDo.RSS.DF.NOAA
{
    public class SecHlder1BDFileHeader : SectionHandler
    {
        private const int cstCommonInfoOffset = 0;
        private const int cstCommonInfoEndOffset = 116;
        private const int cstQualityInfoOffset = 117;
        private const int cstQualityInfoEndOffset = 186;
        private const int cstScaleInfoOffset = 187;
        private const int cstScaleInfoEndOffset = 256;
        private const int cstRadiantionInfoOffset = 257;
        private const int cstRadiantionInfoEndOffset = 328;
        private const int cstGeographInfoOffset = 329;
        private const int cstGeographInfoEndOffset = 424;
        private const int cstSimulateInfoOffset = 425;
        private const int cstSimluateInfoEndOffset = 22016;
        private const int cstDataheaderOffset = 1264;
        /// <summary>
        /// 扫描线的bit域意义：
        /// bit 15：（0=升轨， 1=降轨）
        /// bit 14：（1=经过时钟漂移修正的扫描时间）
        /// bit 13：（1=经过TIP姿态修正的地球定位）
        /// bit 0：通道3选择开关（0=3A， 1=3B）
        /// </summary>
        private const int cstNomalInfoSectionOffset = 22028;

        public SecHlder1BDFileHeader()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            bool isBig = IsBigEndian(fileStream, binaryReader);

            D1BDHeader hInfo = new D1BDHeader(
                new SecHlder1BDFileHeaderCommonInfo_NOAA(isBig).Create(fileStream, binaryReader, cstCommonInfoOffset, cstCommonInfoEndOffset),
                new SecHlder1BDFileHeaderQualityInfo_NOAA(isBig).Create(fileStream, binaryReader, cstQualityInfoOffset, cstQualityInfoEndOffset),
                new SecHlder1BDFileHeaderScaleInfo_NOAA(isBig).Create(fileStream, binaryReader, cstScaleInfoOffset, cstScaleInfoEndOffset),
                new SecHlder1BDFileHeaderRadiantionInfo_NOAA(isBig).Create(fileStream, binaryReader, cstRadiantionInfoOffset, cstRadiantionInfoEndOffset),
                new SecHlder1BDFileHeaderGeographInfo_NOAA(isBig).Create(fileStream, binaryReader, cstGeographInfoOffset, cstGeographInfoEndOffset),
                new SecHlder1BDFileHeaderSimluateInfo_NOAA().Create(fileStream, binaryReader, cstSimulateInfoOffset, cstSimluateInfoEndOffset),
                new NomalInfoSection().Create(fileStream, binaryReader, cstNomalInfoSectionOffset, cstSimluateInfoEndOffset)
                , isBig);
            return hInfo;
        }

        private bool IsBigEndian(Stream fileStream, BinaryReader binaryReader)
        {
            byte[] buffer = new byte[2];
            fileStream.Seek(10, SeekOrigin.Begin);
            buffer = binaryReader.ReadBytes(2);
            if (ToLocalEndian_Core.ToInt16FromBig(buffer) == 22016)
                return true;
            else
                return false;
        }
    }

    public class SecHlder1BDFileHeaderCommonInfo_NOAA : SectionHandler
    {
        private const int _cstRecordBlockLen = 22016;
        private bool _isBigEndian = true;

        public SecHlder1BDFileHeaderCommonInfo_NOAA(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            CommonInfoFor1BD commInfo = new CommonInfoFor1BD();
            fileStream.Seek(offset, SeekOrigin.Begin);
            if (_isBigEndian)
            {
                commInfo.FileGenPlace = ToLocalEndian_Core.ReadString(binaryReader.ReadChars(3));
                commInfo.ASCIIBank = binaryReader.ReadChar();
                commInfo.Version = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.VersionYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.VersionDay = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.LogicalRecordLen = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.RecordBlockLen = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.HeaderRecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.FillFlag17_22 = new UInt16[] { ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2)), ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2)), ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2)) };
                commInfo.DatasetName = ToLocalEndian_Core.ReadString2(binaryReader.ReadChars(42));
                commInfo.HandleBlockFlag = ToLocalEndian_Core.ReadString2(binaryReader.ReadChars(8));
                commInfo.SatelliteIdentify = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.SensorIdentify = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.DataType = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.TIPCode = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.BeginDayFrom1950 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                commInfo.DataBeginYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.DataBeginDayOfYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.DataBeginUTC = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                commInfo.EndDayFrom1950 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                commInfo.DataEndYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.DataEndDayOfYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                commInfo.DataEndUTC = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                binaryReader.ReadBytes(21924);
                UInt16 is3A_int = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                string is3A_str = Convert.ToString(is3A_int, 2).PadLeft(16, '0');
                commInfo.Current3A = is3A_str[0] == '0' ? true : false;
                commInfo.CurrentRise = is3A_str[15] == '0' ? true : false;
                commInfo.SatelliteName = commInfo.SatelliteIdentify == 3 ? "NOAA16" : commInfo.SatelliteIdentify == 11 ? "NOAA17" : (commInfo.SatelliteIdentify == 13 || commInfo.SatelliteIdentify == 7) ? "NOAA18" : "";
                commInfo.SensorName = commInfo.SensorIdentify == 0 ? "AVHRR" : "";
                commInfo.FullRecordCount = new FileInfo((fileStream as FileStream).Name).Length / _cstRecordBlockLen - 1;
                commInfo.OrbitBeginTime = DateTime.Parse(commInfo.DataBeginYear.ToString() + "-01-01").AddDays(commInfo.DataBeginDayOfYear - 1).AddMilliseconds(commInfo.DataBeginUTC);
            }
            else
            {
                commInfo.FileGenPlace = ToLocalEndian_Core.ReadString(binaryReader.ReadChars(3));
                commInfo.ASCIIBank = binaryReader.ReadChar();
                commInfo.Version = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.VersionYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.VersionDay = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.LogicalRecordLen = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.RecordBlockLen = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.HeaderRecordCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.FillFlag17_22 = new UInt16[] { ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2)), 
                    ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2)),
                    ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2)) };
                commInfo.DatasetName = ToLocalEndian_Core.ReadString2(binaryReader.ReadChars(42));
                commInfo.HandleBlockFlag = ToLocalEndian_Core.ReadString2(binaryReader.ReadChars(8));
                commInfo.SatelliteIdentify = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.SensorIdentify = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.DataType = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.TIPCode = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.BeginDayFrom1950 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                commInfo.DataBeginYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.DataBeginDayOfYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.DataBeginUTC = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                commInfo.EndDayFrom1950 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                commInfo.DataEndYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.DataEndDayOfYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                commInfo.DataEndUTC = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                binaryReader.ReadBytes(21924);
                UInt16 is3A_int = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                string is3A_str = Convert.ToString(is3A_int, 2).PadLeft(16, '0');
                commInfo.Current3A = is3A_str[0] == '0' ? true : false;
                commInfo.CurrentRise = is3A_str[15] == '0' ? true : false;
                commInfo.SatelliteName = commInfo.SatelliteIdentify == 3 ? "NOAA16" : commInfo.SatelliteIdentify == 11 ? "NOAA17" : (commInfo.SatelliteIdentify == 13 || commInfo.SatelliteIdentify == 7) ? "NOAA18" : "";
                commInfo.SensorName = commInfo.SensorIdentify == 0 ? "AVHRR" : "";
                commInfo.FullRecordCount = new FileInfo((fileStream as FileStream).Name).Length / _cstRecordBlockLen - 1;
                commInfo.OrbitBeginTime = DateTime.Parse(commInfo.DataBeginYear.ToString() + "-01-01").AddDays(commInfo.DataBeginDayOfYear - 1).AddMilliseconds(commInfo.DataBeginUTC);
            }
            base.Create(fileStream, binaryReader, offset, endOffset);
            return commInfo;
        }
    }

    public class SecHlder1BDFileHeaderQualityInfo_NOAA : SectionHandler
    {
        private bool _isBigEndian = true;

        public SecHlder1BDFileHeaderQualityInfo_NOAA(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            QualityCheckInfoFor1BD qInfo = new QualityCheckInfoFor1BD();
            fileStream.Seek(116, SeekOrigin.Begin);
            if (_isBigEndian)
            {
                UInt32 yqState_Int = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                string yqState_Str = Convert.ToString(yqState_Int, 2).PadLeft(32, '0');
                qInfo.PatchControl = yqState_Str[1] == '0' ? false : true;
                qInfo.EarthShadow = yqState_Str[2] == '0' ? false : true;
                qInfo.RemotesensingLock = yqState_Str[3] == '0' ? false : true;
                qInfo.ScanMotor = yqState_Str[4] == '0' ? false : true;
                qInfo.CoolerHot = yqState_Str[5] == '0' ? false : true;
                qInfo.VStandard = yqState_Str[6] == '0' ? false : true;
                qInfo.Used3AOr3B = yqState_Str[7] == '0' ? false : true;
                qInfo.EnableChannel5 = yqState_Str[8] == '0' ? false : true;
                qInfo.EnableChannel4 = yqState_Str[9] == '0' ? false : true;
                qInfo.EnableChannel3B = yqState_Str[10] == '0' ? false : true;
                qInfo.EnableChannel3A = yqState_Str[11] == '0' ? false : true;
                qInfo.EnableChannel2 = yqState_Str[12] == '0' ? false : true;
                qInfo.EnableChannel1 = yqState_Str[13] == '0' ? false : true;
                qInfo.UsedElectron = yqState_Str[14] == '0' ? false : true;
                qInfo.UsedMotor = yqState_Str[15] == '0' ? false : true;
                fileStream.Seek(2, SeekOrigin.Current);
                qInfo.StateChangeCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.SecondYQState = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
                qInfo.RecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.MarkRecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.LostRecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.DataKXCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorFrameCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorTIPCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorAssistCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorTimeOrder = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorTimeOrderCode = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.UpdateSOCC = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorEarthLocation = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                qInfo.ErrorEarthLocationCode = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                int statePACS_Int = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                string statePACS_Str = Convert.ToString(statePACS_Int, 2).PadLeft(16, '0');
                qInfo.DataMode = statePACS_Str[0] == '0' ? false : true;
                qInfo.TapeDirection = statePACS_Str[1] == '0' ? false : true;
                qInfo.PseudoNoise = statePACS_Str[2] == '0' ? false : true;
                qInfo.PACSource = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            }
            else
            {
                UInt32 yqState_Int = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                string yqState_Str = Convert.ToString(yqState_Int, 2).PadLeft(32, '0');
                qInfo.PatchControl = yqState_Str[1] == '0' ? false : true;
                qInfo.EarthShadow = yqState_Str[2] == '0' ? false : true;
                qInfo.RemotesensingLock = yqState_Str[3] == '0' ? false : true;
                qInfo.ScanMotor = yqState_Str[4] == '0' ? false : true;
                qInfo.CoolerHot = yqState_Str[5] == '0' ? false : true;
                qInfo.VStandard = yqState_Str[6] == '0' ? false : true;
                qInfo.Used3AOr3B = yqState_Str[7] == '0' ? false : true;
                qInfo.EnableChannel5 = yqState_Str[8] == '0' ? false : true;
                qInfo.EnableChannel4 = yqState_Str[9] == '0' ? false : true;
                qInfo.EnableChannel3B = yqState_Str[10] == '0' ? false : true;
                qInfo.EnableChannel3A = yqState_Str[11] == '0' ? false : true;
                qInfo.EnableChannel2 = yqState_Str[12] == '0' ? false : true;
                qInfo.EnableChannel1 = yqState_Str[13] == '0' ? false : true;
                qInfo.UsedElectron = yqState_Str[14] == '0' ? false : true;
                qInfo.UsedMotor = yqState_Str[15] == '0' ? false : true;
                fileStream.Seek(2, SeekOrigin.Current);
                qInfo.StateChangeCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.SecondYQState = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4));
                qInfo.RecordCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.MarkRecordCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.LostRecordCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.DataKXCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorFrameCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorTIPCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorAssistCount = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorTimeOrder = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorTimeOrderCode = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.UpdateSOCC = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorEarthLocation = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                qInfo.ErrorEarthLocationCode = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                int statePACS_Int = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                string statePACS_Str = Convert.ToString(statePACS_Int, 2).PadLeft(16, '0');
                qInfo.DataMode = statePACS_Str[0] == '0' ? false : true;
                qInfo.TapeDirection = statePACS_Str[1] == '0' ? false : true;
                qInfo.PseudoNoise = statePACS_Str[2] == '0' ? false : true;
                qInfo.PACSource = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
            }
            base.Create(fileStream, binaryReader, offset, endOffset);
            return qInfo;
        }
    }

    public class SecHlder1BDFileHeaderScaleInfo_NOAA : SectionHandler
    {
        private bool _isBigEndian = true;

        public SecHlder1BDFileHeaderScaleInfo_NOAA(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            ScaleInfoFor1BD sInfo = new ScaleInfoFor1BD();
            fileStream.Seek(188, SeekOrigin.Begin);
            if (_isBigEndian)
            {
                sInfo.NearSunCHScaleYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
                sInfo.NearSunChScaleDay = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            }
            else
            {
                sInfo.NearSunCHScaleYear = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
                sInfo.NearSunChScaleDay = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2));
            }
            base.Create(fileStream, binaryReader, offset, endOffset);
            return sInfo;
        }
    }

    public class SecHlder1BDFileHeaderRadiantionInfo_NOAA : SectionHandler
    {
        private bool _isBigEndian = true;

        public SecHlder1BDFileHeaderRadiantionInfo_NOAA(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            fileStream.Seek(offset - 1, SeekOrigin.Begin);
            RadiantionConvertArgsInfoFor1BD rInfo = new RadiantionConvertArgsInfoFor1BD();
            if (_isBigEndian)
            {
                rInfo.SunFilterRadiantionCH1 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH1 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.SunFilterRadiantionCH2 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH2 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.SunFilterRadiantionCH3A = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH3A = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.CenterWaveNumberCH3B = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 100f;
                rInfo.C1ConstCH3B = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH3B = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000000f;
                rInfo.CenterWaveNumberCH4 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.C1ConstCH4 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH4 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000000f;
                rInfo.CenterWaveNumberCH5 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.C1ConstCH5 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH5 = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4)) / 1000000f;
            }
            else
            {
                rInfo.SunFilterRadiantionCH1 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH1 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.SunFilterRadiantionCH2 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH2 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.SunFilterRadiantionCH3A = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 10f;
                rInfo.EquivalentFilterWidthCH3A = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.CenterWaveNumberCH3B = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 100f;
                rInfo.C1ConstCH3B = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH3B = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000000f;
                rInfo.CenterWaveNumberCH4 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.C1ConstCH4 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH4 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000000f;
                rInfo.CenterWaveNumberCH5 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000f;
                rInfo.C1ConstCH5 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 100000f;
                rInfo.C2ConstCH5 = ToLocalEndian_Core.ToUInt32FromLittle(binaryReader.ReadBytes(4)) / 1000000f;
            }
            base.Create(fileStream, binaryReader, offset, endOffset);
            return rInfo;
        }
    }

    public class SecHlder1BDFileHeaderGeographInfo_NOAA : SectionHandler
    {
        private bool _isBigEndian = true;

        public SecHlder1BDFileHeaderGeographInfo_NOAA(bool isBigEndian)
        {
            _isBigEndian = isBigEndian;
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            GeographLocationInfoFor1BD gInfo = new GeographLocationInfoFor1BD();
            fileStream.Seek(offset - 1, SeekOrigin.Begin);
            gInfo.ReferenceEllipse = ToLocalEndian_Core.ReadString(binaryReader.ReadChars(8));
            if (_isBigEndian)
            {
                gInfo.EarthLocationGap = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2)) / 10f;
            }
            else
                gInfo.EarthLocationGap = ToLocalEndian_Core.ToUInt16FromLittle(binaryReader.ReadBytes(2)) / 10f;
            base.Create(fileStream, binaryReader, offset, endOffset);
            return gInfo;
        }
    }

    public class SecHlder1BDFileHeaderSimluateInfo_NOAA : SectionHandler
    {

        public SecHlder1BDFileHeaderSimluateInfo_NOAA()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            return base.Create(fileStream, binaryReader, offset, endOffset);
        }
    }

    /// <summary>
    /// 白天为ch3a，夜间为ch3b
    /// 扫描线的bit域意义：
    /// bit 15：（0=升轨， 1=降轨）2013年6月4日修正为：（1=升轨， 0=降轨）
    /// bit 14：（1=经过时钟漂移修正的扫描时间）
    /// bit 13：（1=经过TIP姿态修正的地球定位）
    /// bit 0：通道3选择开关（0=3A， 1=3B）2013年6月4日修正为：（1=3A， 0=3B）
    /// byte[0] byte[1]
    /// 暂时觉得应当理解为：
    /// bit
    /// 目前测试的数据
    /// NOAA18_AVHRR_2013_05_10_14_59.1BD：bit域【00100110 100000000】
    /// 对应的却为：[7][6][5][4][3][2][1][0] [15][14][13][12][11][10][9][8] （目前解析方式,相当于字节小端序处理后的排序）
    /// </summary>
    public class NomalInfoSection : SectionHandler
    {
        public NomalInfoSection()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            NomalHeaderInfo info = new NomalHeaderInfo();
            fileStream.Seek(offset - 1, SeekOrigin.Begin);
            byte[] bits = binaryReader.ReadBytes(2);
            int bit0 = (bits[0] & (1 << 0));//最右面
            int bit15 = (bits[1] & (1 << 7));//最左面
            info.DayOrNight = bit0 == 0 ? 0 : 1;        //bit0
            info.AscOrbit = bit15 == 0 ? false : true;  //bit15
            base.Create(fileStream, binaryReader, offset, endOffset);
            return info;
        }
    }
}
