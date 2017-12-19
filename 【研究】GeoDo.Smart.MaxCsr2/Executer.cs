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

namespace GeoDo.Smart.MaxCsr
{
    /// <summary>
    /// 时序（长时间序列）数据处理
    /// long time sequence
    /// </summary>
    public class Executer
    {
        private VirtualData vData = new VirtualData();
        private IRasterDataProvider _outputDataProvider;
        //private IRasterBand _outputRasterBand;
        private IRasterBand[] _outputRasterBands;

        private IRasterDataProvider _outputCalcDataProvider;
        private IRasterBand _outputCalcRasterBand;

        private CoordEnvelope _coordEnvelope;
        private ISpatialReference _spatialRef;
        public enumDataType CalcType = enumDataType.Int16;
        private int[] _ndviBands = null;

        /// <summary>
        /// 长时间序列像素点运算
        /// </summary>
        /// <param name="files"></param>
        /// <param name="bandNos">通道数据集</param>
        /// <param name="progress"></param>
        public void StatFiles(string[] files, int[] bandNos, string outDir, int[] ndviBands, Action<int, string> progress)
        {
            DateTime dt = DateTime.Now;
            _ndviBands = ndviBands;
            List<IRasterDataProvider> rads = new List<IRasterDataProvider>();
            try
            {
                foreach (string file in files)
                {
                    IRasterDataProvider prd = null;
                    List<IRasterBand> bands = new List<IRasterBand>();
                    try
                    {
                        prd = RasterDataDriver.Open(file) as IRasterDataProvider;
                        foreach (int bandNo in bandNos)
                        {
                            IRasterBand band = prd.GetRasterBand(bandNo);
                            bands.Add(band);
                        }
                        _coordEnvelope = prd.CoordEnvelope;
                        _spatialRef = prd.SpatialRef;
                        vData.AddTimeBands(bands.ToArray());
                        rads.Add(prd);
                    }
                    catch
                    {
                        foreach (IRasterBand band in bands)
                        {
                            if (band != null)
                                band.Dispose();
                        }
                        if (prd == null)
                            prd.Dispose();
                    }
                }

                RasterIdentify id = new RasterIdentify(Path.GetFileName(files[0]));
                id.ProductIdentify = "FOG";
                id.SubProductIdentify = "0CSD";
                string filename = GetWorkspaceFileName(outDir, id);
                BuildInternalBuffer(filename);
                id.SubProductIdentify = "MAXN";
                string calcFileName = GetWorkspaceFileName(outDir, id);

                BuildCalcBuffer(calcFileName);

                StatMax(progress);

                Console.WriteLine(dt.ToString() + "到" + DateTime.Now.ToString());
                Console.WriteLine(filename);

            }
            finally
            {
                for (int i = 0; i < rads.Count; i++)
                {
                    rads[i].Dispose();
                }
            }
        }

        private void StatMax(Action<int, string> progress)
        {
            switch (vData.DataType)
            {
                case enumDataType.Int16:
                    Stat<short, short>
                        (
                        (bands) =>
                        {
                            if ((bands[_ndviBands[0]] + bands[_ndviBands[1]]) == 0)
                                return short.MinValue;
                            else
                                return (short)(((bands[_ndviBands[0]] - bands[_ndviBands[1]]) * 1000f) / (bands[_ndviBands[0]] + bands[_ndviBands[1]]));
                        },
                        (band1, band2) => { return band1 > band2; },
                        0,
                        progress);
                    break;
                case enumDataType.UInt16:
                    Stat<ushort, short>
                        (
                        (bands) =>
                        {
                            if ((bands[_ndviBands[0]] + bands[_ndviBands[1]]) == 0)
                                return short.MinValue;
                            else
                                return (short)(((bands[_ndviBands[0]] - bands[_ndviBands[1]]) * 1000f) / (bands[_ndviBands[0]] + bands[_ndviBands[1]]));
                        },
                        (band1, band2) => { return band1 > band2; },
                        0,
                        progress);
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }
        }

        private void Stat<T, TResult>(
            Func<T[], TResult> bandCalc,
            Func<TResult, TResult, bool> bijiao,
            int resultIndex,
            Action<int, string> progress)
        {
            for (int line = 0; line < vData.Height; line++)
            {
                if (progress != null)
                {
                    int p = (int)((line + 1) * 100.0 / vData.Height);
                    progress(p, p.ToString());
                }
                T[][] allBandsLineData = new T[vData.BandCount][]; //最大值所在文件的所有通道、的一行数据。                
                for (int i = 0; i < vData.BandCount; i++)
                {
                    allBandsLineData[i] = new T[vData.Width];
                }

                //T[] lineStat = new T[vData.Width];
                TResult[] calcValue = new TResult[vData.Width];

                T[][][] vdata = vData.ReadLine<T>(line);//所有文件、所有通道、的一行数据。

                T[] bandsValue = new T[vData.BandCount];
                for (int sampe = 0; sampe < vData.Width; sampe++)
                {
                    List<TResult> ts = new List<TResult>();//所有文件的植被指数值
                    for (int time = 0; time < vData.TimesCount; time++)
                    {
                        List<T> ds = new List<T>();
                        for (int bandIndex = 0; bandIndex < vData.BandCount; bandIndex++)
                        {
                            T d = vdata[time][bandIndex][sampe];
                            ds.Add(d);
                        }
                        ts.Add(bandCalc(ds.ToArray()));
                    }
                    int maxValueTimeIndex = 0;//植被指数最大的值所在文件序号
                    for (int t = 0; t < ts.Count; t++)
                    {
                        if (bijiao(ts[t], ts[maxValueTimeIndex]))
                            maxValueTimeIndex = t;
                    }
                    //lineStat[sampe] = vdata[maxValueTimeIndex][resultIndex][sampe];//取得那个文件的这行数据

                    for (int i = 0; i < vData.BandCount; i++)
                    {
                        allBandsLineData[i][sampe] = vdata[maxValueTimeIndex][i][sampe];
                    }

                    calcValue[sampe] = ts[maxValueTimeIndex];                      //最大值数据
                }

                //WriteLine<T>(_outputRasterBand, line, lineStat, vData.DataType);//输出原始通道数据
                WriteLine<T>(_outputRasterBands, line, allBandsLineData, vData.DataType);//输出原始通道数据

                WriteLine<TResult>(_outputCalcRasterBand, line, calcValue, CalcType);//输出最大值数据 
            }
        }

        public void Dispose()
        {
            if (_outputDataProvider != null)
                _outputDataProvider.Dispose();
            for (int i = 0; i < _outputRasterBands.Length; i++)
            {
                if (_outputRasterBands[i] != null)
                    _outputRasterBands[i].Dispose();
            }
        }

        public static string GetWorkspaceFileName(string outdir, RasterIdentify identify)
        {
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);
            return Path.Combine(outdir, Path.GetFileName(identify.ToWksFullFileName(".LDF")));
        }

        private void WriteLine<T>(IRasterBand[] outputRasterBands, int line, T[][] allBandsLineData, enumDataType dataType)
        {
            int bandCount = allBandsLineData.Length;
            for (int i = 0; i < bandCount; i++)
            {
                T[] lineData = allBandsLineData[i];
                GCHandle handle = GCHandle.Alloc(lineData, GCHandleType.Pinned);
                try
                {
                    IntPtr ptr = handle.AddrOfPinnedObject();
                    outputRasterBands[i].Write(0, line, vData.Width, 1, ptr, dataType, vData.Width, 1);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        private void WriteLine<T>(IRasterBand outputRasterBand, int line, T[] lineData, enumDataType dataType)
        {
            GCHandle handle = GCHandle.Alloc(lineData, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                outputRasterBand.Write(0, line, vData.Width, 1, ptr, dataType, vData.Width, 1);
            }
            finally
            {
                handle.Free();
            }
        }

        private void BuildInternalBuffer(string fileName)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            _outputDataProvider = drv.Create(fileName, vData.Width, vData.Height, vData.BandCount, vData.DataType, GetOptions());
            _outputRasterBands = new IRasterBand[vData.BandCount];
            for (int i = 0; i < vData.BandCount; i++)
            {
                _outputRasterBands[i] = _outputDataProvider.GetRasterBand(i + 1);
            }
        }

        private void BuildCalcBuffer(string fileName)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            _outputCalcDataProvider = drv.Create(fileName, vData.Width, vData.Height, 1, CalcType, GetOptions());
            _outputCalcRasterBand = _outputCalcDataProvider.GetRasterBand(1);
        }

        private object[] GetOptions()
        {
            List<string> ops = new List<string>();
            if (_coordEnvelope != null)
                ops.Add(_coordEnvelope.ToMapInfoString(new Size(vData.Width, vData.Height)));
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
            return ops.Count > 0 ? ops.ToArray() : null;
        }
    }
}
