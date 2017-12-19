using System;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXFile
    {
        public AWXLevel1Header L1Header { get; set; }
        public AWXLevel2Header L2Header { get; set; }
        public AWXLevel2HeaderEx L2HeaderEx { get; set; }
        public byte[] Padding { get; set; }
        public AWXProduct Product { get; set; }

        public void Read(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] awxLevel1HeaderBytes = br.ReadBytes(40);
                    L1Header = new AWXLevel1Header();
                    L1Header.Read(new MemoryStream(awxLevel1HeaderBytes));
                    switch (L1Header.ProductType)
                    {
                        case 1:
                            L2Header = new AWXLevel2HeaderImageGeostationary();
                            break;
                        case 2:
                            L2Header = new AWXLevel2HeaderImagePolarOrbit();
                            break;
                        default:
                            L2Header = new AWXLevel2HeaderImagePolarOrbit();
                            break;
                    }
                    byte[] awxLevel2HeaderBytes = br.ReadBytes(L1Header.Level2HeaderLength);
                    L2Header.Read(new MemoryStream(awxLevel2HeaderBytes));
                    switch (L1Header.ProductType)
                    {
                        case 1:
                            AWXLevel2HeaderImage L2HeaderImageGeostationary = L2Header as AWXLevel2HeaderImage;
                            if (L2HeaderImageGeostationary.ToningTableDataBlockLength
                                + L2HeaderImageGeostationary.CalibrationDataBlockLength
                                + L2HeaderImageGeostationary.LocationDataBlockLength > 0)
                            {
                                L2HeaderEx = new AWXLevel2HeaderExImage(
                                    L2HeaderImageGeostationary.ToningTableDataBlockLength,
                                    L2HeaderImageGeostationary.CalibrationDataBlockLength,
                                    L2HeaderImageGeostationary.LocationDataBlockLength,
                                    new CalibrationDataBlockGeostationary());
                                L2HeaderEx.Read(stream);
                            }
                            break;
                        case 2:
                            AWXLevel2HeaderImage L2HeaderImagePolarOrbit = L2Header as AWXLevel2HeaderImage;
                            if (L2HeaderImagePolarOrbit.ToningTableDataBlockLength
                                + L2HeaderImagePolarOrbit.CalibrationDataBlockLength
                                + L2HeaderImagePolarOrbit.LocationDataBlockLength > 0)
                            {
                                L2HeaderEx = new AWXLevel2HeaderExImage(
                                    L2HeaderImagePolarOrbit.ToningTableDataBlockLength,
                                    L2HeaderImagePolarOrbit.CalibrationDataBlockLength,
                                    L2HeaderImagePolarOrbit.LocationDataBlockLength,
                                    new CalibrationDataBlockGeostationary());
                                L2HeaderEx.Read(stream);
                            }
                            break;
                    }
                }
            }
        }

        public void Write(string hdr, string dat,bool isPolar = false)
        {
            //string[] polarsat = new string[] { "FY1", "FY3", "EOSA", "EOST", "NOAA", "TERRA", "AQUA","MST" };
            ////根据文件名判断轨道类型(极轨/静止)、数据时间；
            RasterIdentify id = new RasterIdentify(dat);
            //isPolar = true;
            //foreach (string sat in polarsat)
            //{
            //    if (id.Satellite.ToUpper().Contains(sat))
            //    {
            //        isPolar = true;
            //        break;
            //    }
            //}
            if (isPolar)
            {
                WritePolarAWX(hdr,dat,id);
            }
            else
            {
                WriteStationaryAWX(hdr, dat, id);
            }
        }

        private void WritePolarAWX(string hdr, string dat,RasterIdentify id)
        {
            HdrFile hdrObj = HdrFile.LoadFrom(hdr);
            L1Header = new AWXLevel1HeaderImagePolarOrbit();
            L1Header.Write(hdrObj);
            L2Header = new AWXLevel2HeaderImagePolarOrbit();
            if (hdrObj.MapInfo.Name == "Geographic Lat/Lon")
            {
                L2Header.ProjectMode = 4;
            }
            using(IRasterDataProvider dataprd =GeoDataDriver.Open(dat) as IRasterDataProvider)
            {
                if (dataprd != null && dataprd.BandCount!=0)
                {
                    L2Header.GeographicalScopeNorthLatitude = (short)(dataprd.CoordEnvelope.MaxY*100);
                    L2Header.GeographicalScopeSouthLatitude = (short)(dataprd.CoordEnvelope.MinY * 100);
                    L2Header.GeographicalScopeWestLongitude = (short)(dataprd.CoordEnvelope.MinX * 100);
                    L2Header.GeographicalScopeEastLongitude = (short)(dataprd.CoordEnvelope.MaxX * 100);

                    L2Header.ProjectCenterLatitude = (short)((L2Header.GeographicalScopeNorthLatitude+L2Header.GeographicalScopeSouthLatitude)/2);
                    L2Header.ProjectCenterLongitude = (short)((L2Header.GeographicalScopeEastLongitude + L2Header.GeographicalScopeWestLongitude) / 2);
                    L2Header.ProjectHorizontalResolution = (short)(dataprd.ResolutionX * 100 * 100);
                    L2Header.ProjectVerticalResolution = (short)(dataprd.ResolutionY * 100 * 100);
                }
            }
            L2Header.Write(hdrObj, id);
            Padding = new byte[L1Header.RecordLength - L1Header.Level1HeaderLength - L1Header.Level2HeaderLength];
            L1Header.WriteFile(hdr);
            L2Header.WriteFile(hdr);
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(Padding);
                }
            }
            using (FileStream stream = new FileStream(dat, FileMode.Open))
            {
                stream.Seek(hdrObj.HeaderOffset, SeekOrigin.Begin);
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] awxProductBytes = br.ReadBytes(Convert.ToInt32(stream.Length - stream.Position));
                    Product = new AWXProduct();
                    Product.Read(new MemoryStream(awxProductBytes));
                }
            }
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(Product.Data);
                }
            }
        }

        private void WriteStationaryAWX(string hdr, string dat, RasterIdentify id)
        {
            HdrFile hdrObj = HdrFile.LoadFrom(hdr);
            L1Header = new AWXLevel1HeaderImageGeostationaryOrbit();
            L1Header.Write(hdrObj);
            L2Header = new AWXLevel2HeaderImageGeostationary();
            if (hdrObj.MapInfo.Name == "Geographic Lat/Lon")
            {
                L2Header.ProjectMode = 4;
            }
            enumDataType datatype = enumDataType.Unknow;
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(dat) as IRasterDataProvider)
            {
                if (dataprd != null && dataprd.BandCount != 0)
                {
                    datatype = dataprd.DataType;
                    L2Header.GeographicalScopeNorthLatitude = (short)(dataprd.CoordEnvelope.MaxY * 100);
                    L2Header.GeographicalScopeSouthLatitude = (short)(dataprd.CoordEnvelope.MinY * 100);
                    L2Header.GeographicalScopeWestLongitude = (short)(dataprd.CoordEnvelope.MinX * 100);
                    L2Header.GeographicalScopeEastLongitude = (short)(dataprd.CoordEnvelope.MaxX * 100);
                    L2Header.ProjectCenterLatitude = (short)((L2Header.GeographicalScopeNorthLatitude + L2Header.GeographicalScopeSouthLatitude) / 2);
                    L2Header.ProjectCenterLongitude = (short)((L2Header.GeographicalScopeEastLongitude + L2Header.GeographicalScopeWestLongitude) / 2);
                    L2Header.ProjectHorizontalResolution = (short)(dataprd.ResolutionX * 100 * 100);
                    L2Header.ProjectVerticalResolution = (short)(dataprd.ResolutionY * 100 * 100);
                }
            }
            L2Header.Write(hdrObj, id);
            Padding = new byte[L1Header.RecordLength - L1Header.Level1HeaderLength - L1Header.Level2HeaderLength];
            L1Header.WriteFile(hdr);
            L2Header.WriteFile(hdr);
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(Padding);
                }
            } 
            using (FileStream stream = new FileStream(dat, FileMode.Open))
            {
                stream.Seek(hdrObj.HeaderOffset, SeekOrigin.Begin);
                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] awxProductBytes = new Byte[L2Header.ImageHeight * L2Header.ImageWidth];
                    switch (datatype)
                    {
                        case enumDataType.Byte:
                            awxProductBytes = br.ReadBytes(Convert.ToInt32(stream.Length - stream.Position));
                            break;
                        case enumDataType.Int16:
                            for (int i = 0; i < awxProductBytes.Length;i++ )
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadInt16())[0];
                            break;
                        case enumDataType.UInt16:
                            for (int i = 0; i < awxProductBytes.Length; i++)
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadUInt16())[0];
                            break;
                        case enumDataType.Int32:
                            for (int i = 0; i < awxProductBytes.Length; i++)
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadInt32())[0];
                            break;
                        case enumDataType.UInt32:
                            for (int i = 0; i < awxProductBytes.Length; i++)
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadUInt32())[0];
                            break;
                        case enumDataType.Int64:
                            for (int i = 0; i < awxProductBytes.Length; i++)
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadInt64())[0];
                            break;
                        case enumDataType.UInt64:
                            for (int i = 0; i < awxProductBytes.Length; i++)
                                awxProductBytes[i] = BitConverter.GetBytes(br.ReadUInt64())[0];
                            break;
                        default:
                            awxProductBytes = br.ReadBytes(Convert.ToInt32(stream.Length - stream.Position));
                            break;
                    }
                    Product = new AWXProduct();
                    Product.Read(new MemoryStream(awxProductBytes));
                }
            }
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(Product.Data);
                }
            }
        }
    }
}
