using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using GeoDo.RSS.DF.MEM;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class InterestedRaster<T> : IInterestedRaster<T>
    {
        protected RasterIdentify _identify;
        protected string _name;
        protected string _text;
        protected Size _size;
        protected ISpatialReference _spatialRef;
        protected CoordEnvelope _coordEnvelope;
        protected IRasterBand _rasterValues;
        protected IRasterDataProvider _dataProvider;
        protected string _fileName;
        protected int _extHeaderSize;

        /// <summary>
        /// Default:
        /// SpatialRef = Geographics Lon/Lat
        /// </summary>
        public InterestedRaster(RasterIdentify identify, Size size, CoordEnvelope coordEnvelope,int extHeaderSize)
        {
            if (identify == null)
                throw new ArgumentNullException("identify");
            _identify = identify;
            _size = size;
            _extHeaderSize = extHeaderSize;
            _coordEnvelope = coordEnvelope;
            BuildInternalBuffer();
        }

        public InterestedRaster(string fname, Size size, CoordEnvelope coordEnvelope, int extHeaderSize)
        {
            _size = size;
            _extHeaderSize = extHeaderSize;
            _coordEnvelope = coordEnvelope;
            _fileName = fname;
            BuildInternalBuffer(fname);
        }

        public InterestedRaster(RasterIdentify identify, Size size, CoordEnvelope coordEnvelope)
        {
            if (identify == null)
                throw new ArgumentNullException("identify");
            _identify = identify;
            _size = size;
            _coordEnvelope = coordEnvelope;
            BuildInternalBuffer();
        }

        public InterestedRaster(string fname, Size size, CoordEnvelope coordEnvelope)
        {
            _size = size;
            _coordEnvelope = coordEnvelope;
            _fileName = fname;
            BuildInternalBuffer(fname);
        }

        public InterestedRaster(RasterIdentify identify, Size size, CoordEnvelope coordEnvelope, ISpatialReference spatialRef)
        {
            if (identify == null)
                throw new ArgumentNullException("identify");
            _identify = identify;
            _size = size;
            _coordEnvelope = coordEnvelope;
            _spatialRef = spatialRef;
            BuildInternalBuffer();
        }

        public InterestedRaster(string fname, Size size, CoordEnvelope coordEnvelope, ISpatialReference spatialRef)
        {
            _size = size;
            _coordEnvelope = coordEnvelope;
            _spatialRef = spatialRef;
            _fileName = fname;
            BuildInternalBuffer(fname);
        }

        public InterestedRaster(RasterIdentify identify, Size size, CoordEnvelope coordEnvelope, ISpatialReference spatialRef,int extHeaderSize)
        {
            if (identify == null)
                throw new ArgumentNullException("identify");
            _identify = identify;
            _size = size;
            _extHeaderSize = extHeaderSize;
            _coordEnvelope = coordEnvelope;
            _spatialRef = spatialRef;
            BuildInternalBuffer();
        }

        public InterestedRaster(string fname, Size size, CoordEnvelope coordEnvelope, ISpatialReference spatialRef, int extHeaderSize)
        {
            _size = size;
            _extHeaderSize = extHeaderSize;
            _coordEnvelope = coordEnvelope;
            _spatialRef = spatialRef;
            _fileName = fname;
            BuildInternalBuffer(fname);
        }

        private void BuildInternalBuffer()
        {
            if (_identify.IsOutput2WorkspaceDir)
                _fileName = GetWorkspaceFileName(_identify);
            else
                _fileName = MifEnvironment.GetFullFileName(_identify.ToLongString() + ".dat");
            BuildInternalBuffer(_fileName);
        }

        private void BuildInternalBuffer(string _fileName)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            _dataProvider = drv.Create(_fileName, _size.Width, _size.Height, 1, GetDataType(), GetOptions());
            _rasterValues = _dataProvider.GetRasterBand(1);
        }

        public static string GetWorkspaceFileName(RasterIdentify identify)
        {
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(),identify.ProductIdentify);
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            dir = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return identify.ToWksFullFileName(".dat");
        }

        private object[] GetOptions()
        {
            List<string> ops = new List<string>();
            if (_coordEnvelope != null)
                ops.Add(_coordEnvelope.ToMapInfoString(_size));
            if (_spatialRef != null)
            {
                try
                {
                    string spref = _spatialRef.ToProj4String();
                    ops.Add("SPATIALREF=" + spref);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            ops.Add("ExtHeaderSize=" + _extHeaderSize.ToString());
            return ops.Count > 0 ? ops.ToArray() : null;
        }

        private enumDataType GetDataType()
        {
            T[] t = new T[1];
            if (t[0] is byte)
                return enumDataType.Byte;
            else if (t[0] is UInt16)
                return enumDataType.UInt16;
            else if (t[0] is Int16)
                return enumDataType.Int16;
            else if (t[0] is Int32)
                return enumDataType.Int32;
            else if (t[0] is UInt32)
                return enumDataType.UInt32;
            else if (t[0] is Int64)
                return enumDataType.Int64;
            else if (t[0] is UInt64)
                return enumDataType.UInt64;
            else if (t[0] is float)
                return enumDataType.Float;
            else if (t[0] is double)
                return enumDataType.Double;
            throw new NotSupportedException("dataType");
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public RasterIdentify Identify
        {
            get { return _identify; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public IRasterBand RasterValues
        {
            get
            {
                return _rasterValues;
            }
        }

        public IRasterDataProvider HostDataProvider
        {
            get { return _dataProvider; }
        }

        public Size Size
        {
            get { return _size; }
        }

        public ISpatialReference SpatialRef
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }

        public CoordEnvelope CoordEnvelope
        {
            get { return _coordEnvelope; }
            set { _coordEnvelope = value; }
        }

        public void Reset()
        {
            _rasterValues.Fill(0);
        }

        public void Put(double defatultValue)
        {
            _rasterValues.Fill(defatultValue);
        }

        public void Put(int[] indexes, T trueValue)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            int count = indexes.Length;
            int r = 0, c = 0;
            int w = _size.Width;
            int h = _size.Height;
            enumDataType dataType = _dataProvider.DataType;
            T[] buffer = new T[1];
            buffer[0] = trueValue;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                for (int i = 0; i < count; i++)
                {
                    r = indexes[i] / w;
                    c = indexes[i] - r * w;
                    _rasterValues.Write(c, r, 1, 1, ptr, dataType, 1, 1);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public void Put(int[] indexes, T[] features)
        {
            if (indexes == null || indexes.Length == 0)
                return;
            int count = indexes.Length;
            int r = 0, c = 0;
            int w = _size.Width;
            int h = _size.Height;
            enumDataType dataType = _dataProvider.DataType;
            T[] buffer = new T[1];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                for (int i = 0; i < count; i++)
                {
                    r = indexes[i] / w;
                    c = indexes[i] - r * w;
                    buffer[0] = features[i];
                    _rasterValues.Write(c, r, 1, 1, ptr, dataType, 1, 1);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public void Put(int idx, T value)
        {
            int r = 0, c = 0;
            int w = _size.Width;
            enumDataType dataType = _dataProvider.DataType;
            T[] buffer = new T[1];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                r = idx / w;
                c = idx - r * w;
                buffer[0] =value;
                _rasterValues.Write(c, r, 1, 1, ptr, dataType, 1, 1);
            }
            finally
            {
                handle.Free();
            }
        }

        public void Put(IPixelFeatureMapper<T> result)
        {
            int blockSize = Math.Min(10000, result.Count);
            List<int> bufferIdx = new List<int>(blockSize);
            List<T> bufferFeatures = new List<T>(blockSize);
            int i = 0;
            int naturalIdx = 0;
            foreach (int idx in result.Indexes)
            {
                if (i == blockSize)
                {
                    i = 0;
                    Put(bufferIdx.ToArray(), bufferFeatures.ToArray());
                    bufferIdx.Clear();
                    bufferFeatures.Clear();
                }
                bufferIdx.Add(idx);
                bufferFeatures.Add(result.GetValueByIndex(naturalIdx));
                i++;
                naturalIdx++;
            }
            if (bufferIdx.Count > 0)
                Put(bufferIdx.ToArray(), bufferFeatures.ToArray());
        }

        public int Count(int[] aoi, Func<T, bool> filter)
        {
            IArgumentProvider argPrd = new ArgumentProvider(_dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int retCount = 0;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    if (filter(values[0]))
                        retCount++;
                });
            return retCount;
        }

        public int Count(int[] aoi, Func<T, int> weight)
        {
            IArgumentProvider argPrd = new ArgumentProvider(_dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            int retCount = 0;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    retCount += weight(values[0]);
                });
            return retCount;
        }

        public double Area(int[] aoi, Func<T, bool> filter)
        {
            if (_coordEnvelope == null)
                return 0d;
            IArgumentProvider argPrd = new ArgumentProvider(_dataProvider, null);
            argPrd.AOI = aoi;
            IRasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(argPrd);
            double area = 0;
            int row = 0;
            int width = _dataProvider.Width;
            double maxLat = _dataProvider.CoordEnvelope.MaxY;
            double res = _dataProvider.ResolutionX;
            visitor.VisitPixel(new int[] { 1 },
                (idx, values) =>
                {
                    if (filter(values[0]))
                    {
                        row = idx / width;
                        area += ComputePixelArea(row, maxLat, res);
                    }
                }
                );
            return area;
        }

        public static double ComputePixelArea(int row, double maxLat, double resolution)
        {
            double lat = maxLat - row * resolution;
            double a = 6278.137d;
            double c = 6356.7523142d;
            double lon = resolution * 2 * Math.PI * a * c * Math.Sqrt(1 / (c * c + a * a + Math.Tan(lat * lat))) / 360d;
            return lon * lat * resolution;
        }

        public void Dispose()
        {
            _coordEnvelope = null;
            _spatialRef = null;
            _rasterValues = null;
            if (_dataProvider != null)
            {
                _dataProvider.Dispose();
                _dataProvider = null;
            }
        }


        public void SetExtHeader<TExtHanderStruct>(TExtHanderStruct extHeader)
        {
            (_dataProvider as IMemoryRasterDataProvider).SetExtHeader<TExtHanderStruct>(extHeader);
        }

        public TExtHanderStruct GetExtHeader<TExtHanderStruct>()
        {
            return (_dataProvider as IMemoryRasterDataProvider).GetExtHeader<TExtHanderStruct>();
        }
    }
}
