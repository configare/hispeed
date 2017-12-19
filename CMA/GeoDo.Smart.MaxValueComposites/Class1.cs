using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.Smart.MaxValueComposites
{
    /// <summary>
    /// 时序（长时间序列）数据处理
    /// long time sequence
    /// </summary>
    public class Class1
    {
        VirtualData vData = new VirtualData();
        private IRasterBand _rasterBand;
        IRasterDataProvider _dataProvider;

        private CoordEnvelope _coordEnvelope;
        private ISpatialReference _spatialRef;

        /// <summary>
        /// 长时间序列像素点运算
        /// </summary>
        /// <param name="files"></param>
        /// <param name="bandNo"></param>
        /// <param name="progress"></param>
        public void StatFiles(string[] files, int bandNo, Action<int, string> progress)
        {
            DateTime dt = DateTime.Now;
            List<IRasterDataProvider> rads = new List<IRasterDataProvider>();
            foreach (string file in files)
            {
                IRasterDataProvider rad = null;
                IRasterBand band = null;
                try
                {
                    rad = RasterDataDriver.Open(file) as IRasterDataProvider;
                    band = rad.GetRasterBand(bandNo);
                    _coordEnvelope = rad.CoordEnvelope;
                    _spatialRef = rad.SpatialRef;
                    vData.AddBand(band);
                    rads.Add(rad);
                }
                catch
                {
                    if (band != null)
                        band.Dispose();
                    if (rad == null)
                        rad.Dispose();
                }
                finally
                {
                }
            }
            RasterIdentify id = new RasterIdentify(Path.GetFileName(files[0]));
            id.SubProductIdentify = "0MAX";
            string filename = GetWorkspaceFileName(id);
            BuildInternalBuffer(filename);

            StatMax(progress);

            for (int i = 0; i < rads.Count; i++)
            {
                rads[i].Dispose();
            }

            Console.WriteLine(dt.ToString() + "到" + DateTime.Now.ToString());
            Console.WriteLine(filename);
        }

        private void StatMax(Action<int, string> progress)
        {
            switch (vData.DataType)
            {
                case enumDataType.Atypism:
                    break;
                case enumDataType.Bits:
                    Stat<byte, byte>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Byte:
                    Stat<byte, byte>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Double:
                    Stat<double, double>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Float:
                    Stat<float, float>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Int16:
                    Stat<short, short>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Int32:
                    Stat<int, int>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Int64:
                    Stat<long, long>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.UInt16:
                    Stat<ushort, ushort>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.UInt32:
                    Stat<uint, uint>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.UInt64:
                    Stat<ulong, ulong>((v) => { return v.Max(); }, progress);
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }
        }

        private void Stat<T, TResult>(Func<T[], TResult> stat, Action<int, string> progress)
        {
            for (int line = 0; line < vData.Height; line++)
            {
                if (progress != null)
                {
                    int p = (int)((line + 1) * 100.0 / vData.Height);
                    progress(p, p.ToString());
                }
                TResult[] lineStat = new TResult[vData.Width];
                T[][] vdata = vData.ReadLine<T>(line);
                T[] bandsValue = new T[vData.BandCount];
                for (int sampe = 0; sampe < vData.Width; sampe++)
                {
                    for (int bandIndex = 0; bandIndex < vData.BandCount; bandIndex++)
                    {
                        bandsValue[bandIndex] = vdata[bandIndex][sampe];
                    }
                    lineStat[sampe] = stat(bandsValue);// bandsValue.Max();
                }
                WriteLine<TResult>(line, lineStat);
            }
        }

        public void Dispose()
        {
            if (_dataProvider != null)
                _dataProvider.Dispose();
            if (_rasterBand != null)
                _rasterBand.Dispose();
        }

        public static string GetWorkspaceFileName(RasterIdentify identify)
        {
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), identify.ProductIdentify);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            dir = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return identify.ToWksFullFileName(".dat");
        }
        
        private void WriteLine<T>(int line, T[] lineData)
        {
            GCHandle handle = GCHandle.Alloc(lineData, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                _rasterBand.Write(0, line, vData.Width, 1, ptr, vData.DataType, vData.Width, 1);
            }
            finally
            {
                handle.Free();
            }
        }

        private void BuildInternalBuffer(string fileName)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            _dataProvider = drv.Create(fileName, vData.Width, vData.Height, 1, vData.DataType, GetOptions());
            _rasterBand = _dataProvider.GetRasterBand(1);
        }

        public IRasterBand RasterValues
        {
            get
            {
                return _rasterBand;
            }
        }

        private object[] GetOptions()
        {
            List<string> ops = new List<string>();
            if (_coordEnvelope != null)
                ops.Add(_coordEnvelope.ToMapInfoString(new Size(vData.Width,vData.Height)));
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
            //ops.Add("ExtHeaderSize=" + _extHeaderSize.ToString());
            return ops.Count > 0 ? ops.ToArray() : null;
        }
    }
}
