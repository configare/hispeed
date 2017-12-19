using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.NOAA
{
    public class SecHlder1A5FileHeader : SectionHandler
    {
        public SecHlder1A5FileHeader()
        {
        }

        public override object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            D1A5Header hInfo = new D1A5Header();
            fileStream.Seek(offset, SeekOrigin.Begin);
            hInfo.SatelliteIdentify = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.DataBeginYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.DataBeginMilliSecond = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
            hInfo.DataBeginDayNums = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.DataEndYear = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.DataEndMilliSecond = ToLocalEndian_Core.ToUInt32FromBig(binaryReader.ReadBytes(4));
            hInfo.DataEndDayNums = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.RecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.LastRecord = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.ErrorFrameCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.BitErrorRatio = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            fileStream.Seek(2, SeekOrigin.Current);
            hInfo.ErrorTimeOrder = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            hInfo.LostRecordCount = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            fileStream.Seek(84,SeekOrigin.Current);
            hInfo.TrackNumber = ToLocalEndian_Core.ToUInt16FromBig(binaryReader.ReadBytes(2));
            fileStream.Seek(74,SeekOrigin.Current);
            hInfo.DataBeginSecond = ToLocalEndian_Core.ToUInt64FromBig(binaryReader.ReadBytes(8));
            hInfo.DataEndSecond = ToLocalEndian_Core.ToUInt64FromBig(binaryReader.ReadBytes(8));

            hInfo.SatelliteName = hInfo.SatelliteIdentify == 3 ? "NOAA16" : hInfo.SatelliteIdentify == 5 ? "NOAA14" : hInfo.SatelliteIdentify == 7 ? "NOAA15" : hInfo.SatelliteIdentify == 9 ? "NOAA12" : "";
            if (hInfo.SatelliteName == "")
            {
                if (hInfo.SatelliteIdentify == 0x0b)
                    hInfo.SatelliteName = "NOAA17";
                else if (hInfo.SatelliteIdentify == 0x0d)
                    hInfo.SatelliteName = "NOAA18";
            }
            
            base.Create(fileStream, binaryReader, offset, endOffset);
            return hInfo;
        }
    }
}
