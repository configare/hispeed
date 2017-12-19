using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.Project;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class RasterResample
    {
        public class FileResampleSetting
        {
            protected float _outResolutionX = 0;
            protected float _outResolutionY = 0;
            protected CoordEnvelope _outEnvelope = null;
            //如果_outFormat为非内存，则需要指定该参数
            private string _outPathAndFileName = null;
            private object[] _extArgs = null;
            private Size _outSize = Size.Empty;

            public float OutResolutionX
            {
                get { return _outResolutionX; }
                set { _outResolutionX = value; }
            }

            public float OutResolutionY
            {
                get { return _outResolutionY; }
                set { _outResolutionY = value; }
            }
            public CoordEnvelope OutEnvelope
            {
                get { return _outEnvelope; }
                set { _outEnvelope = value; }
            }

            public string OutPathAndFileName
            {
                get { return _outPathAndFileName; }
                set { _outPathAndFileName = value; }
            }

            public Size OutSize
            {
                get { return _outSize = Size.Empty; }
                set { _outSize = value; }
            }

            public object[] ExtArgs
            {
                get { return _extArgs; }
                set { _extArgs = value; }
            }
        }
        public RasterResample()
        {

        }

        private FileResampleSetting _resSettings;
        protected ISpatialReference _dstSpatialRef = SpatialReference.GetDefault();
        public void Resample(RSS.Core.DF.IRasterDataProvider srcRaster, FileResampleSetting resSettings, Action<int, string> progressCallback)
        {
            if (srcRaster == null)
                throw new ArgumentNullException("srcRaster", "待重采样数据为空");
            if (resSettings == null)
                throw new ArgumentNullException("prjSettings", "重采样参数为空");
            ReadExtArgs(resSettings);
            string outfilename = resSettings.OutPathAndFileName;
            IRasterDataProvider outwriter = null;
            try
            {
                Size outSize = _resSettings.OutSize;
                string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + _dstSpatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + _resSettings.OutEnvelope.MinX + "," + _resSettings.OutEnvelope.MaxY + "}:{" + _resSettings.OutResolutionX + "," + _resSettings.OutResolutionY + "}"
                        };
                outwriter = CreateOutFile(outfilename, srcRaster.BandCount, outSize, srcRaster.DataType, options);
                if (File.Exists(outfilename))
                    ResampleRaster(srcRaster, outwriter, progressCallback);
                else
                    throw new FileLoadException("目标文件" + outfilename+"不存在！");
            }
            catch (IOException ex)
            {
                if (ex.Message == "磁盘空间不足。\r\n" && File.Exists(outfilename))
                    File.Delete(outfilename);
                throw ex;
            }
            finally
            {
                if (outwriter != null)
                {
                    outwriter.Dispose();
                    outwriter = null;
                }
            }
        }

        private double _xzoom = 1d;
        private double _yzoom = 1d;
        private double? _fillValue = null;
        private void ReadExtArgs(FileResampleSetting prjSettings)
        {
            if (prjSettings.ExtArgs != null && prjSettings.ExtArgs.Length != 0)
            {
                foreach (object arg in prjSettings.ExtArgs)
                {
                    if (arg is Dictionary<string, double>)
                    {
                        Dictionary<string, double> exAtg = arg as Dictionary<string, double>;
                        if (exAtg.ContainsKey("xzoom"))
                            _xzoom = exAtg["xzoom"];
                        if (exAtg.ContainsKey("yzoom"))
                            _yzoom = exAtg["yzoom"];
                        if (exAtg.ContainsKey("FillValue"))
                            _fillValue = exAtg["FillValue"];
                    }
                }
            }
            if (prjSettings.OutEnvelope!=null)
            {
                _resSettings = prjSettings;
                _resSettings.OutSize = GetSize(prjSettings.OutResolutionX, prjSettings.OutEnvelope);
            }
        }

        public Size GetSize(double resolution,CoordEnvelope env)
        {
            return new Size((int)(env.Width / resolution + 0.5d), (int)(env.Height / resolution + 0.5d));
        }

        internal IRasterDataProvider CreateOutFile(string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            string dir =Path.GetDirectoryName(outfilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, dataType, options) as IRasterDataProvider;
        }

        protected void ResampleRaster(IRasterDataProvider srcImgRaster, IRasterDataProvider prdWriter, Action<int, string> progressCallback)
        {
            //要写成T型
            enumDataType dataType = srcImgRaster.DataType;
            Size bufferSize = _resSettings.OutSize;
            int srcwidth = srcImgRaster.Width, srcheight = srcImgRaster.Height;
            //执行投影
            float[] srcBandData = null, dstBandData = null; 
            for (int i = 0; i < srcImgRaster.BandCount; i++)  //读取原始通道值，投影到目标区域
            {
                srcBandData = new float[srcwidth * srcwidth];
                IRasterBand srcBand = srcImgRaster.GetRasterBand(i + 1); ;//
                GCHandle srch = GCHandle.Alloc(srcBandData, GCHandleType.Pinned);
                try
                {
                    IntPtr bufferPtr = srch.AddrOfPinnedObject();
                    srcBand.Read(0, 0, srcBand.Width, srcBand.Height, bufferPtr, dataType, srcBand.Width, srcBand.Height);
                }
                finally
                {
                    srch.Free();
                }
                AnaliysisDataPreprocess.MedianRead(srcBandData, new Size(srcwidth, srcheight), bufferSize, out dstBandData);
                IRasterBand band = prdWriter.GetRasterBand(i + 1);
                GCHandle dsth = GCHandle.Alloc(dstBandData, GCHandleType.Pinned);
                try
                {
                    IntPtr bufferPtr = dsth.AddrOfPinnedObject();
                    band.Write(0, 0, bufferSize.Width, bufferSize.Height, bufferPtr, dataType, bufferSize.Width, bufferSize.Height);
                }
                finally
                {
                    dsth.Free();
                }
                dstBandData = null;
            }

        }

    }
}
