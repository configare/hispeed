using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;
using GeoDo.Project;
using System.Drawing;
using System.IO.MemoryMappedFiles;

namespace GeoDo.RSS.DF.MEM
{
    public class MemoryRasterDataProvider : RasterDataProvider, IMemoryRasterDataProvider, IVirtualScan0, IUpdateCoordEnvelope
    {
        protected string _mmfName;
        protected MemoryMappedFile _mmf;
        protected int _headerSize;
        protected FileStream _fileStream;
        protected MemoryMappedFileAccess _access;
        protected int _offsetXScan0;
        protected int _offsetYScan0;
        internal MemoryRasterHeader _header;

        public MemoryRasterDataProvider(string mmfName, string fname, enumDataProviderAccess access, IGeoDataDriver driver)
            : base(fname, driver)
        {
            _mmfName = GetMmfName(mmfName);
            _fileName = fname;
            _access = access == enumDataProviderAccess.ReadOnly ? MemoryMappedFileAccess.Read : MemoryMappedFileAccess.ReadWrite;
            string hdrfile = HdrFile.GetHdrFileName(_fileName);
            if (File.Exists(hdrfile))
                _filelist = new string[] { _fileName, hdrfile };
            else
                _filelist = new string[] { _fileName };
            _header = GetHeader(fname);
            SetFieldsByHeader(_header);
            TrySetEnvelopeAndResolutions();
            LoadBands();
        }

        private string GetMmfName(string mmfName)
        {
            return mmfName.Replace(':', '_').Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('-', '_');
        }

        private static Dictionary<string, object> OpenedMmfs = new Dictionary<string, object>();
        private void LoadBands()
        {
            FileAccess fileAccess = FileAccess.Read;
            if (_access == MemoryMappedFileAccess.ReadWrite)
                fileAccess = FileAccess.ReadWrite;
            _fileStream = new FileStream(_fileName, FileMode.Open, fileAccess, FileShare.Read);
            //_mmf = MemoryMappedFile.CreateFromFile(_fileName, FileMode.Open, _mmfName);
            if (!OpenedMmfs.ContainsKey(_mmfName))
            {
                _mmf = MemoryMappedFile.CreateFromFile(_fileStream, _mmfName, 0, _access, null, HandleInheritability.None, false);
                OpenedMmfs.Add(_mmfName, this);
            }
            else
                _mmf = MemoryMappedFile.OpenExisting(_mmfName);
            for (int b = 1; b <= _bandCount; b++)
            {
                _rasterBands.Add(new MemoryRasterBand(this, _access, _mmf, b));
            }
        }

        private void SetFieldsByHeader(MemoryRasterHeader header)
        {
            _bandCount = header.BandCount;
            _width = header.Width;
            _height = header.Height;
            _dataType = (enumDataType)header.DataType;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
            _spatialRef = GetSpatialRef(header);
            SetCoordType(_spatialRef);
            _headerSize = Marshal.SizeOf(header) + header.ExtendHeaderLength;
            TryCreateCoordTransform(header);
        }

        private void SetCoordType(ISpatialReference spatialRef)
        {
            if (spatialRef == null)
                _coordType = enumCoordType.Raster;
            else if (spatialRef.ProjectionCoordSystem == null)
                _coordType = enumCoordType.GeoCoord;
            else
                _coordType = enumCoordType.PrjCoord;
        }

        private ISpatialReference GetSpatialRef(MemoryRasterHeader header)
        {
            using (SpatialReferenceBuilder b = new SpatialReferenceBuilder())
            {
                return b.GetSpatialRef(header.PrjId, header.SP1,
                    header.SP2, header.Lat0, header.Lon0,
                    header.X0, header.Y0, header.K0);
            }
        }

        private void TryCreateCoordTransform(MemoryRasterHeader header)
        {
            if (_spatialRef == null)
                _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
            else
            {
                _coordTransform = CoordTransoformFactory.GetCoordTransform(
                    new Point(0, 0),
                    new Point(_width, _height),
                    new double[] { double.Parse(header.MinX.ToString()), double.Parse(header.MaxY.ToString()) },
                    new double[] { double.Parse(header.MaxX.ToString()), double.Parse(header.MinY.ToString()) });
            }
        }

        private MemoryRasterHeader GetHeader(string fname)
        {
            using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetHeader(fs);
            }
        }

        private MemoryRasterHeader GetHeader(FileStream fs)
        {
            _headerSize = Marshal.SizeOf(typeof(MemoryRasterHeader));
            byte[] buffer = new byte[_headerSize];
            fs.Read(buffer, 0, buffer.Length);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return (MemoryRasterHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MemoryRasterHeader));
            }
            finally
            {
                handle.Free();
            }
        }

        public int HeaderSize
        {
            get { return _headerSize; }
        }

        public string MmfName
        {
            get { return _mmfName; }
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
            if (_mmf != null && OpenedMmfs.ContainsKey(_mmfName))
            {
                if (OpenedMmfs[_mmfName].Equals(this))
                {
                    OpenedMmfs.Remove(_mmfName);
                    _mmf.Dispose();
                }
                _mmf = null;
            }
            base.Dispose();

            if (_isNeedUpdateHeader)
            {
                TryUpdateHeader();
            }
        }

        int IVirtualScan0.OffsetX
        {
            get { return _offsetXScan0; }
        }

        int IVirtualScan0.OffsetY
        {
            get { return _offsetYScan0; }
        }

        bool IVirtualScan0.IsVirtualScan0
        {
            get { return _offsetXScan0 > 0 || _offsetYScan0 > 0; }
        }

        void IVirtualScan0.SetScan0(int offsetX, int offsetY)
        {
            _offsetXScan0 = offsetX;
            _offsetYScan0 = offsetY;
        }

        void IVirtualScan0.Reset()
        {
            _offsetXScan0 = 0;
            _offsetYScan0 = 0;
        }

        public void SetExtHeader<TExtHanderStruct>(TExtHanderStruct extHeader)
        {
            SetExtHeader<TExtHanderStruct>(_fileStream, extHeader);
        }

        private void SetExtHeader<TExtHanderStruct>(FileStream fileStream, TExtHanderStruct extHeader)
        {
            int headerSize = Marshal.SizeOf(typeof(TExtHanderStruct));
            byte[] buffer = new byte[headerSize];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr((object)extHeader, handle.AddrOfPinnedObject(), true);
                fileStream.Seek(_headerSize - headerSize, SeekOrigin.Begin);
                fileStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        public void SetExtHeader<TExtHanderStruct>(TExtHanderStruct extHeader,int headLength)
        {
            SetExtHeader<TExtHanderStruct>(_fileStream, extHeader, headLength);
        }

        private void SetExtHeader<TExtHanderStruct>(FileStream fileStream, TExtHanderStruct extHeader, int headLength)
        {
            int headerSize = headLength;
            byte[] buffer = new byte[headerSize];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr((object)extHeader, handle.AddrOfPinnedObject(), true);
                fileStream.Seek(_headerSize - headerSize, SeekOrigin.Begin);
                fileStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        public TExtHanderStruct GetExtHeader<TExtHanderStruct>()
        {
            int headerSize = Marshal.SizeOf(typeof(TExtHanderStruct));
            byte[] buffer = new byte[headerSize];
            _headerSize = Marshal.SizeOf(typeof(MemoryRasterHeader));
            _fileStream.Seek(_headerSize, SeekOrigin.Begin);
            _fileStream.Read(buffer, 0, buffer.Length);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return (TExtHanderStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TExtHanderStruct));
            }
            finally
            {
                handle.Free();
            }
        }

        public void Update(CoordEnvelope coordEnvelope)
        {
            _isNeedUpdateHeader = false;
            if (coordEnvelope != null)
            {
                _coordEnvelope = coordEnvelope.Clone();
                _header.MinX = (float)_coordEnvelope.MinX;
                _header.MaxX = (float)_coordEnvelope.MaxX;
                _header.MinY = (float)_coordEnvelope.MinY;
                _header.MaxY = (float)_coordEnvelope.MaxY;
                if (_access == MemoryMappedFileAccess.ReadWrite)
                {
                    //MemoryRasterHeader header = GetExtHeader<MemoryRasterHeader>();
                    SetExtHeader<MemoryRasterHeader>(_header);
                    string hdr = HdrFile.GetHdrFileName(_fileName);
                    if (File.Exists(hdr))
                        File.Delete(hdr);
                }
                else
                {
                    _isNeedUpdateHeader = true;
                }
            }
        }

        private bool _isNeedUpdateHeader = false;

        public bool IsStoreHeaderChanged
        {
            get { return _isNeedUpdateHeader; }
            set { _isNeedUpdateHeader = value; }
        }

        private void TryUpdateHeader()
        {
            using (FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.Write))
            {
                SetExtHeader<MemoryRasterHeader>(fs, _header);
                string hdr = HdrFile.GetHdrFileName(_fileName);
                if (File.Exists(hdr))
                {
                    //HdrFile hdrfile = HdrFile.LoadFrom(hdr);
                    //hdrfile.MapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(_header.MinX, _header.MaxY);
                    //hdrfile.MapInfo.BaseRowColNumber = new Point(1, 1);
                    //hdrfile.SaveTo(hdr);
                    File.Delete(hdr);
                }
            }
        }

        public HdrFile ToHdrFile()
        {
             HdrFile hdrContent = _header.ToHdrFile();
             hdrContent.HdrProjectionInfo = GetHdrProjectionInfo();
             hdrContent.MapInfo = GetHdrMapInfo();
             return hdrContent;
        }

        private HdrProjectionInfo GetHdrProjectionInfo()
        {
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
                return null;
            HdrProjectionInfo prjInfo = new HdrProjectionInfo();
            prjInfo.Datum = _spatialRef.GeographicsCoordSystem.Datum.Name;
            prjInfo.Name = _spatialRef.Name ?? string.Empty;
            prjInfo.ProjectionID = int.Parse(_spatialRef.ProjectionCoordSystem.Name.ENVIName);
            float[] args = null;
            string units = null;
            _spatialRef.ToEnviProjectionInfoString(out args, out units);
            prjInfo.PrjArguments = args;
            prjInfo.Units = units;
            return prjInfo;
        }

        private HdrMapInfo GetHdrMapInfo()
        {
            HdrMapInfo mapInfo = new HdrMapInfo();
            if (_spatialRef != null && _spatialRef.ProjectionCoordSystem != null)
            {
                mapInfo.Name = GetMapInfoName();
                mapInfo.Units = "Meters";
            }
            mapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(_coordEnvelope.MinX, _coordEnvelope.MaxY);
            mapInfo.BaseRowColNumber = new Point(1, 1);
            mapInfo.CoordinateType = GetCoordinateSystemName();
            mapInfo.XYResolution = new HdrGeoPointCoord(Double.Parse(_resolutionX.ToString()), Double.Parse(_resolutionY.ToString()));//低精度向高精度转化
            return mapInfo;
        }

        private string GetCoordinateSystemName()
        {
            if (_spatialRef == null || _spatialRef.GeographicsCoordSystem == null || _spatialRef.GeographicsCoordSystem.Name == null)
                return "WGS-84";
            return _spatialRef.GeographicsCoordSystem.Name;
        }

        private string GetMapInfoName()
        {
            if (_spatialRef.ProjectionCoordSystem != null && _spatialRef.ProjectionCoordSystem.Name != null)
                return _spatialRef.ProjectionCoordSystem.Name.WktName ?? string.Empty;
            return string.Empty;
        }

        public override object GetStretcher(int bandNo)
        {
            return null;
        }
        public override int[] GetDefaultBands()
        {
            return new int[] { 1, 1, 1 };
        }
    }
}
