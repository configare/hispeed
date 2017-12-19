using System.IO;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel2HeaderImagePolarOrbit : AWXLevel2HeaderImage
    {
        public short BeginTimeYear { get; set; }
        public short BeginTimeMonth { get; set; }
        public short BeginTimeDay { get; set; }
        public short BeginTimeHour { get; set; }
        public short BeginTimeMinute { get; set; }
        public short EndTimeYear { get; set; }
        public short EndTimeMonth { get; set; }
        public short EndTimeDay { get; set; }
        public short EndTimeHour { get; set; }
        public short EndTimeMinute { get; set; }
        public short RChannelNumber { get; set; }
        public short GChannelNumber { get; set; }
        public short BChannelNumber { get; set; }
        public short AscendDescendMark { get; set; }
        public short OrbitNumber { get; set; }
        public short BytesPerPixel { get; set; }
        public short ProductType { get; set; }

        public override void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                Satellite = Encoding.ASCII.GetString(br.ReadBytes(8));
                BeginTimeYear = br.ReadInt16();
                BeginTimeMonth = br.ReadInt16();
                BeginTimeDay = br.ReadInt16();
                BeginTimeHour = br.ReadInt16();
                BeginTimeMinute = br.ReadInt16();
                EndTimeYear = br.ReadInt16();
                EndTimeMonth = br.ReadInt16();
                EndTimeDay = br.ReadInt16();
                EndTimeHour = br.ReadInt16();
                EndTimeMinute = br.ReadInt16();
                ChannelNumber = br.ReadInt16();
                RChannelNumber = br.ReadInt16();
                GChannelNumber = br.ReadInt16();
                BChannelNumber = br.ReadInt16();
                AscendDescendMark = br.ReadInt16();
                OrbitNumber = br.ReadInt16();
                BytesPerPixel = br.ReadInt16();
                ProjectMode = br.ReadInt16();//83-84
                ProductType = br.ReadInt16();
                ImageWidth = br.ReadInt16();
                ImageHeight = br.ReadInt16();
                ImageUpperLeftScanlineNumber = br.ReadInt16();
                ImageUpperLeftPixelNumber = br.ReadInt16();
                SamplingRate = br.ReadInt16();
                GeographicalScopeNorthLatitude = br.ReadInt16() ;
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

        public override void Write(HdrFile hdr, RasterIdentify id)
        {
            base.Write(hdr,id);
            BytesPerPixel = GetBytesPerPixel(hdr.DataType);
            BeginTimeYear = (short)id.OrbitDateTime.Year;
            BeginTimeMonth = (short)id.OrbitDateTime.Month;
            BeginTimeDay = (short)id.OrbitDateTime.Day;
            BeginTimeHour = (short)id.OrbitDateTime.Hour;
            BeginTimeMinute = (short)id.OrbitDateTime.Minute;
            EndTimeYear = (short)id.OrbitDateTime.Year;
            EndTimeMonth = (short)id.OrbitDateTime.Month;
            EndTimeDay = (short)id.OrbitDateTime.Day;
            EndTimeHour = (short)id.OrbitDateTime.Hour;
            EndTimeMinute = (short)id.OrbitDateTime.Minute;
            if (hdr.BandNums.Length==3)
            {
                ChannelNumber = 0;
                RChannelNumber = (short)hdr.BandNums[0];
                GChannelNumber = (short)hdr.BandNums[1];
                BChannelNumber = (short)hdr.BandNums[2];
            }
            else if (hdr.BandNums.Length == 1)
            {
                ChannelNumber = 6;
            }
        }

        private Int16 GetBytesPerPixel(int envidatatype)
        {
            switch (envidatatype)
            {
                case 1:
                    return 1;
                case 2:
                case 12:
                    return 2;
                case 3:
                case 13:
                case 4:
                    return 4;
                case 14:
                case 15:
                case 5:
                    return 8;
                default:
                    return 1;
            }
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
                    bw.Write(BeginTimeYear);
                    bw.Write(BeginTimeMonth);
                    bw.Write(BeginTimeDay);
                    bw.Write(BeginTimeHour);
                    bw.Write(BeginTimeMinute);
                    bw.Write(EndTimeYear);
                    bw.Write(EndTimeMonth);
                    bw.Write(EndTimeDay);
                    bw.Write(EndTimeHour);
                    bw.Write(EndTimeMinute);
                    bw.Write(ChannelNumber);
                    bw.Write(RChannelNumber);
                    bw.Write(GChannelNumber);
                    bw.Write(BChannelNumber);
                    bw.Write(AscendDescendMark);
                    bw.Write(OrbitNumber);
                    bw.Write(BytesPerPixel);
                    bw.Write(ProjectMode);
                    bw.Write(ProductType);
                    bw.Write(ImageWidth);
                    bw.Write(ImageHeight);
                    bw.Write(ImageUpperLeftScanlineNumber);
                    bw.Write(ImageUpperLeftPixelNumber);
                    bw.Write(SamplingRate);
                    bw.Write((short)GeographicalScopeNorthLatitude );
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
