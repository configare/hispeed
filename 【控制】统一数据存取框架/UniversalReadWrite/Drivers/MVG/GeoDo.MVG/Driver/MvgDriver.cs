using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.DF.MVG
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    internal class MvgDriver : RasterDataDriver, IMvgDriver
    {
        public MvgDriver()
            : base()
        {
            _name = "MVG";
            _fullName = "MVG Driver";
        }

        public MvgDriver(string name, string fullname)
            : base(name, fullname)
        {
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fileExtension = Path.GetExtension(fileName).ToUpper();
            return MvgHeader.IsMVG(header1024, fileExtension);
        }

        public override void Delete(string fileName)
        {
            string hdrFile = HdrFile.GetHdrFileName(fileName);
            string[] files = null;
            if (File.Exists(hdrFile))
                files = new string[] { fileName, hdrFile };
            else
                files = new string[] { fileName };
            foreach (string f in files)
                File.Delete(f);
        }

        /*
         *  VALUECOUNT = 2         //default:0
         *  VALUES = "{0,1}"       //
         *  VALUENAMES             //
            SPATIALREF=Proj4                  //default:wgs-84
            MAPINFO={X,Y}:{Col,Row}:{ResolutionX,ResolutionY} //default:null
            WITHHDR=[TRUE|FALSE]      //default:true
        */
        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            ISpatialReference spatialRef;
            MvgHeader mvgHeader;
            HdrMapInfo mapInfo = null;
            bool isWithHdr = true;
            Int16 valueCount = 0;
            Int16[] values = null;
            string[] valueNames = null;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                ParserMvgHeaderParams(options, out spatialRef, out mapInfo, out isWithHdr, out valueCount, out values, out valueNames);
                mvgHeader = CreateMvgHeader(xSize, ySize, spatialRef, mapInfo, valueCount, values, valueNames);
                int headerSize = mvgHeader.HeaderSize;
                int fileLength = mvgHeader.HeaderSize + xSize * ySize * bandCount * DataTypeHelper.SizeOf(enumDataType.Int16);
                fileStream.SetLength(fileLength);
                fileStream.Write(mvgHeader.ToBytes(), 0, headerSize);
            }
            if (isWithHdr)
                mvgHeader.ToHdrFile().SaveTo(HdrFile.GetHdrFileName(fileName));
            IGeoDataProvider provider = BuildDataProviderByArgs(fileName, enumDataProviderAccess.Update, mvgHeader);
            return provider as IRasterDataProvider;
        }

        //适用于打开文件的情况
        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new MvgDataProvider(fileName, this as RasterDataDriver, access == enumDataProviderAccess.ReadOnly);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new MvgDataProvider(fileName, this as RasterDataDriver, access == enumDataProviderAccess.ReadOnly);
        }

        private IGeoDataProvider BuildDataProviderByArgs(string fileName, enumDataProviderAccess access, MvgHeader mvgHeader)
        {
            return new MvgDataProvider(fileName, this as RasterDataDriver, mvgHeader, access == enumDataProviderAccess.ReadOnly);
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            string[] srcFiles = dataProvider.GetFileList();
            string tdir = Path.GetDirectoryName(fileName);
            foreach (string fname in srcFiles)
            {
                string ext = Path.GetExtension(fname);
                File.Copy(fname, Path.Combine(tdir, Path.GetFileNameWithoutExtension(fileName) + ext), true);
            }
            return new MvgDataProvider(fileName, this as RasterDataDriver, false);
        }

        private MvgHeader CreateMvgHeader(int xSize, int ySize, ISpatialReference spatialRef, HdrMapInfo mapInfo, Int16 valueCount, Int16[] values, string[] valueNames)
        {
            if (values == null || valueNames == null || valueCount == 0)
                throw new NotSupportedException("所给参数(values,valueNames,valueCount)不完整！");
            MvgHeader mvgHeader = new MvgHeader();
            mvgHeader.Width = (Int16)xSize;
            mvgHeader.Height = (Int16)ySize;
            mvgHeader.SpatialRef = spatialRef;
            mvgHeader.ValueCount = valueCount;
            mvgHeader.Values = values;
            mvgHeader.ValueNames = valueNames;
            mvgHeader.SetMapInfo(mapInfo);
            return mvgHeader;
        }

        internal void ParserMvgHeaderParams(object[] options, out ISpatialReference spatialRef, out HdrMapInfo mapInfo, out bool isWithHdr,
                                                              out Int16 valueCount, out Int16[] values, out string[] valueNames)
        {
            spatialRef = null;
            mapInfo = null;
            isWithHdr = true;
            valueCount = 0;
            values = null;
            valueNames = null;
            if (options == null || options.Length == 0)
                return;
            foreach (object option in options)
            {
                string param = option.ToString();
                int k = param.IndexOf('=');
                string key = param.Substring(0, k).ToUpper().Trim();
                string value = param.Substring(k + 1).ToUpper().Trim();
                switch (key)
                {
                    case "VALUECOUNT":
                        valueCount = Convert.ToInt16(value);
                        break;
                    case "VALUES":
                        values = ParserValues(value);
                        break;
                    case "VALUENAMES":
                        valueNames = ParserValueNames(value);
                        break;
                    case "SPATIALREF":
                        spatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(value);
                        break;
                    case "MAPINFO":
                        mapInfo = ParseMapInfo(value);
                        break;
                    case "WITHHDR":
                        isWithHdr = value.ToUpper() == "TRUE" || value == "1";
                        break;
                    default:
                        break;
                }
            }
        }

        private static string[] ParserValueNames(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            string[] parts = value.Split(',');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Replace('{', ' ').Replace('}', ' ').Trim();
            return parts;
        }

        private static short[] ParserValues(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            string[] parts = value.Split(',');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Replace('{', ' ').Replace('}', ' ').Trim();
            Int16[] values = new Int16[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                values[i] = Convert.ToInt16(parts[i]);
            return values;
        }

        private static HdrMapInfo ParseMapInfo(string value)
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
    }
}
