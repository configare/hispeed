using System.IO;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel2HeaderImageGeostationary : AWXLevel2HeaderImage
    {
        public short TimeYear { get; set; }
        public short TimeMonth { get; set; }
        public short TimeDay { get; set; }
        public short TimeHour { get; set; }
        public short TimeMinute { get; set; }

        public override void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                Satellite = Encoding.ASCII.GetString(br.ReadBytes(8));
                TimeYear = br.ReadInt16();
                TimeMonth = br.ReadInt16();
                TimeDay = br.ReadInt16();
                TimeHour = br.ReadInt16();
                TimeMinute = br.ReadInt16();
                ChannelNumber = br.ReadInt16();
                ProjectMode = br.ReadInt16();//61-62
                ImageWidth = br.ReadInt16();
                ImageHeight = br.ReadInt16();
                ImageUpperLeftScanlineNumber = br.ReadInt16();
                ImageUpperLeftPixelNumber = br.ReadInt16();
                SamplingRate = br.ReadInt16();
                GeographicalScopeNorthLatitude = br.ReadInt16();
                GeographicalScopeSouthLatitude = br.ReadInt16() ;
                GeographicalScopeWestLongitude = br.ReadInt16() ;
                GeographicalScopeEastLongitude = br.ReadInt16() ;
                ProjectCenterLatitude = br.ReadInt16() ;
                ProjectCenterLongitude = br.ReadInt16() ;
                ProjectStandardLatitude1 = br.ReadInt16() ;
                ProjectStandardLatitude2 = br.ReadInt16() ;
                ProjectHorizontalResolution = br.ReadInt16() ;
                ProjectVerticalResolution = br.ReadInt16() ;
                GeographyGridSuperposeMark = br.ReadInt16();
                GeographyGridSuperposeValue = br.ReadInt16();
                ToningTableDataBlockLength = br.ReadInt16();
                CalibrationDataBlockLength = br.ReadInt16();
                LocationDataBlockLength = br.ReadInt16();
                Reserve = br.ReadInt16();
            }
        }

        public override void Write(HdrFile hdr,RasterIdentify id)
        {
            base.Write(hdr,id);
            TimeYear = (short)id.OrbitDateTime.Year;
            TimeMonth = (short)id.OrbitDateTime.Month;
            TimeDay = (short)id.OrbitDateTime.Day;
            TimeHour = (short)id.OrbitDateTime.Hour;
            TimeMinute = (short)id.OrbitDateTime.Minute;
            //ImageUpperLeftScanlineNumber = 11;
            //ImageUpperLeftPixelNumber = 12;
            //SamplingRate = 13;
            //ProjectStandardLatitude1 = 20;
            //ProjectStandardLatitude2 = 21;
            //GeographyGridSuperposeMark = 24;
            //GeographyGridSuperposeValue = 25;
            if (hdr.Bands == 1&&hdr.BandNums != null && hdr.BandNums.Length>=1)
                ChannelNumber = (short)hdr.BandNums[0];
            else
                ChannelNumber = 6;
        }

        public override void WriteFile(string hdr)
        {
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(Satellite);
                }
            }
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(TimeYear);
                    bw.Write(TimeMonth);
                    bw.Write(TimeDay);
                    bw.Write(TimeHour);
                    bw.Write(TimeMinute);
                    bw.Write(ChannelNumber);
                    bw.Write(ProjectMode);
                    bw.Write(ImageWidth);
                    bw.Write(ImageHeight);
                    bw.Write(ImageUpperLeftScanlineNumber);
                    bw.Write(ImageUpperLeftPixelNumber);
                    bw.Write(SamplingRate);
                    bw.Write((short)GeographicalScopeNorthLatitude);
                    bw.Write((short)GeographicalScopeSouthLatitude );
                    bw.Write((short)GeographicalScopeWestLongitude );
                    bw.Write((short)GeographicalScopeEastLongitude );
                    bw.Write((short)ProjectCenterLatitude );
                    bw.Write((short)ProjectCenterLongitude );
                    bw.Write((short)ProjectStandardLatitude1 );
                    bw.Write((short)ProjectStandardLatitude2 );
                    bw.Write((short)ProjectHorizontalResolution );
                    bw.Write((short)ProjectVerticalResolution );
                    bw.Write(GeographyGridSuperposeMark);
                    bw.Write(GeographyGridSuperposeValue);
                    bw.Write(ToningTableDataBlockLength);
                    bw.Write(CalibrationDataBlockLength);
                    bw.Write(LocationDataBlockLength);
                    bw.Write(Reserve);
                }
            }
        }
    }
}
