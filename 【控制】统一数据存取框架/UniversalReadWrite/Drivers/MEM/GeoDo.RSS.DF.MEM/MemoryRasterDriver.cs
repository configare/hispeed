using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.IO;
using GeoDo.Project;
using System.Drawing;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.DF.MEM
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class MemoryRasterDriver : RasterDataDriver
    {
        public static char[] FileIdentify = new char[] { 'G', 'E', 'O', 'D', 'O', 'M', 'E', 'M', 'V', '0', '1' };

        public MemoryRasterDriver()
        {
            _name = "MEM";
            _fullName = "MEM Raster Driver";
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new MemoryRasterDataProvider(fileName, fileName, access, this);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            if (header1024 == null)
                return false;
            for (int i = 0; i < MemoryRasterDriver.FileIdentify.Length; i++)
                if (MemoryRasterDriver.FileIdentify[i] != header1024[i])
                    return false;
            return true;
        }

        public override void Delete(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private void ParseOptions(int width, int height, object[] options, out ISpatialReference spatialRef, out CoordEnvelope coordEnvelope, out HdrMapInfo mapInfo,
            out bool isWithHdr,out int extHeaderSize)
        {
            mapInfo = null;
            spatialRef = null;
            coordEnvelope = null;
            isWithHdr = true;
            extHeaderSize = 0;
            if (options == null || options.Length == 0)
                return ;
            foreach (object option in options)
            {
                string param = option.ToString();
                int k = param.IndexOf('=');
                string key = param.Substring(0, k).ToUpper().Trim();
                string value = param.Substring(k + 1).ToUpper().Trim();
                switch (key.ToUpper())
                {
                    case "SPATIALREF":
                        spatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(value);
                        break;
                    case "MAPINFO":
                        mapInfo = ParseMapInfoFromOptionValue(value);
                        coordEnvelope = CoordEnvelope.FromMapInfoString(value, new Size(width, height));
                        break;
                    case "WITHHDR":
                        isWithHdr = value.ToUpper() == "TRUE" || value == "1";
                        break;
                    case "EXTHEADERSIZE":
                        extHeaderSize = int.Parse(value);
                        break;
                    default:
                        break;
                }
            }
        }

        private HdrMapInfo ParseMapInfoFromOptionValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            string[] parts = value.Split(':');
            if (parts.Length != 3)
                return null;
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Replace('{', ' ').Replace('}', ' ').Trim();
            string[] values = parts[0].Split(',');
            if (values.Length != 2)
                return null;
            HdrMapInfo mapInfo = new HdrMapInfo();
            mapInfo.BaseRowColNumber = new Point(int.Parse(values[0]), int.Parse(values[1]));
            values = parts[1].Split(',');
            if (values.Length != 2)
                return null;
            mapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(double.Parse(values[0]), double.Parse(values[1]));
            values = parts[2].Split(',');
            if (values.Length != 2)
                return null;
            mapInfo.XYResolution = new HdrGeoPointCoord(double.Parse(values[0]), double.Parse(values[1]));
            return mapInfo;
        }

        private MemoryRasterHeader GetHeader(ISpatialReference spatialRef, CoordEnvelope coordEnvelope)
        {
            MemoryRasterHeader header = new MemoryRasterHeader();
            header.Identify = MemoryRasterDriver.FileIdentify;
            //
            if (spatialRef != null)
            {
                Datum datum = spatialRef.GeographicsCoordSystem.Datum ?? new Datum();
                if (spatialRef.ProjectionCoordSystem != null)
                {
                    header.PrjId = int.Parse(spatialRef.ProjectionCoordSystem.Name.ENVIName);
                    header.A = (float)datum.Spheroid.SemimajorAxis;
                    header.B = (float)datum.Spheroid.SemiminorAxis;
                    header.K0 = (float)datum.Spheroid.InverseFlattening;
                    IProjectionCoordSystem prjCoordSystem = spatialRef.ProjectionCoordSystem;
                    header.SP1 = GetPrjParaValue(prjCoordSystem, "sp1");
                    header.SP2 = GetPrjParaValue(prjCoordSystem, "sp2");
                    header.Lat0 = GetPrjParaValue(prjCoordSystem, "lat0");
                    header.Lon0 = GetPrjParaValue(prjCoordSystem, "lon0");
                    header.X0 = GetPrjParaValue(prjCoordSystem, "x0");
                    header.Y0 = GetPrjParaValue(prjCoordSystem, "y0");
                }
            }
            //
            if (coordEnvelope != null)
            {
                header.MinX = (float)coordEnvelope.MinX;
                header.MaxX = (float)coordEnvelope.MaxX;
                header.MinY = (float)coordEnvelope.MinY;
                header.MaxY = (float)coordEnvelope.MaxY;
            }
            return header;
        }

        private float GetPrjParaValue(IProjectionCoordSystem prjCoordSystem, string paraName)
        {
            NameValuePair v = prjCoordSystem.GetParaByName(paraName);
            return v != null ? (float)v.Value : 0f;
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            ISpatialReference spatialRef;
            CoordEnvelope coordEnvelope;
            bool isWithHdr;
            int extHeaderSize = 0;
            HdrMapInfo mapInfo;
            ParseOptions(xSize, ySize, options, out spatialRef, out coordEnvelope,out mapInfo, out isWithHdr, out extHeaderSize);
            MemoryRasterHeader h = GetHeader(spatialRef, coordEnvelope);
            h.BandCount = bandCount;
            h.DataType = (int)dataType;
            h.Width = xSize;
            h.Height = ySize;
            h.ExtendHeaderLength = extHeaderSize;
            byte[] headerBytes = new byte[1024];
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                int headerSize = Marshal.SizeOf(typeof(MemoryRasterHeader));
                byte[] buffer = new byte[headerSize];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Marshal.StructureToPtr((object)h, handle.AddrOfPinnedObject(), true);
                handle.Free();
                fs.SetLength(headerSize + extHeaderSize + (long)xSize * (long)ySize * (long)bandCount * (long)DataTypeHelper.SizeOf(dataType));
                fs.Write(buffer, 0, buffer.Length);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(headerBytes, 0, (int)Math.Min(headerBytes.Length, fs.Length));
            }

            HdrFile hdrFile = h.ToHdrFile();
            if (spatialRef != null)
                hdrFile.HdrProjectionInfo = GetHdrProjectionInfo(spatialRef);
            if (mapInfo != null)
                hdrFile.MapInfo = mapInfo;

            hdrFile.SaveTo(HdrFile.GetHdrFileName(fileName));
            IGeoDataProvider provider = BuildDataProvider(fileName, headerBytes, enumDataProviderAccess.Update, options);
            return provider as IRasterDataProvider;
        }

        private HdrProjectionInfo GetHdrProjectionInfo(ISpatialReference _spatialRef)
        {
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
                return null;
            HdrProjectionInfo prjInfo = new HdrProjectionInfo();
            prjInfo.Datum = WktToHdrDatum(_spatialRef.GeographicsCoordSystem.Datum.Name);
            prjInfo.Name = _spatialRef.Name ?? string.Empty;
            prjInfo.ProjectionID = int.Parse(_spatialRef.ProjectionCoordSystem.Name.ENVIName);
            float[] args = null;
            string units = null;
            _spatialRef.ToEnviProjectionInfoString(out args, out units);
            prjInfo.PrjArguments = args;
            prjInfo.Units = units;
            return prjInfo;
        }

        private string WktToHdrDatum(string wktName)//D_WGS_1984
        {
            switch (wktName)
            {
                case "D_WGS_1984":
                    return "WGS-84";
                default:
                    return "WGS-84";
            }
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new MemoryRasterDataProvider(fileName, fileName,access, this as RasterDataDriver);
        }
    }
}
