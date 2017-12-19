using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using System.Runtime.InteropServices;
using System.IO;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// cloudsat数据投影处理：
    /// 选择一层数据，执行投影：目标文件在外部创建，多点映射为一点时候取均值
    /// 实际就是矢量数据数栅格化
    /// </summary>
    public class PointProjection
    {
        /// <summary>
        /// 矢量点输出为栅格
        /// 采用直接映射投影点的方式
        /// 多点投影为同一点，取平均值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="longs"></param>
        /// <param name="lats"></param>
        /// <param name="values"></param>
        /// <param name="outValues"></param>
        /// <param name="indexs"></param>
        public static void PointToRasterInt16(float[] longs, float[] lats, short[] values,
            CoordEnvelope env, float resolutionX, float resolutionY, Size size, ref short[] outValues)
        {
            double minx = env.MinX;
            double maxy = env.MaxY;
            int x = size.Width;
            int y = size.Height;
            int m = -1;
            int n = -1;
            int index = -1;
            int[] sums = new int[x * y];
            int[] count = new int[x * y];
            //下面的这种处理方案，适合输出栅格尺度比较大，但是矢量点个数有限的情况。
            List<int> outIndexs = new List<int>();
            List<short> outCount = new List<short>();
            for (int i = 0; i < longs.Length; i++)
            {
                m = (int)((longs[i] - minx) / resolutionX + 0.5);
                n = (int)((maxy - lats[i]) / resolutionY + 0.5);
                n = n == y ? y - 1 : n;
                index = n * x + m;
                sums[index] += values[i];
                count[index]++;
            }
            for (int i = 0; i < x * y; i++)
            {
                if(count[i]!=0)
                    outValues[i] = (short)(sums[i] / count[i]);
            }
        }
    }

    /// <summary>
    /// cloudsat数据中一个数据层栅格化
    /// </summary>
    public class CloudsatToRaster
    {
        private string[] _cloudsatfiles = null;
        private int _bandNo = 1;
        private IRasterBand _band = null;
        private int _heightLevel = 1;   //0-124

        public CloudsatToRaster()
        {
        }

        public void ToRaster(string[] cloudsatfiles, int bandNo, int heightLevel, IRasterDataProvider oraster)
        {
            _cloudsatfiles = cloudsatfiles;
            _bandNo = bandNo;
            _band = oraster.GetRasterBand(1);
            _heightLevel = heightLevel;
            int ox = oraster.Width;
            int oy = oraster.Height;
            float resolutionx = oraster.ResolutionX;
            float resolutiony = oraster.ResolutionY;
            Size osize = new Size(oraster.Width, oraster.Height);
            GeoDo.RSS.Core.DF.CoordEnvelope env = oraster.CoordEnvelope;
            short[] refValues = new short[ox * oy];
            for (int i = 0; i < _cloudsatfiles.Length; i++)
            {
                float[] latitudes;
                float[] longitudes;
                int h;
                string fullfilename = _cloudsatfiles[i];
                int cou, minX, maxX;
                short[] datas = null;
                int level = _heightLevel;//
                int xn;
                using (CloudsatDataProvider raster = GeoDataDriver.Open(fullfilename) as CloudsatDataProvider)
                {
                    CloudSatRasterBand band = raster.GetRasterBand(_bandNo) as CloudSatRasterBand;
                    h = band.Height;//37081
                    latitudes = raster.ReadVdata("Latitude", null, 0, h) as float[];
                    longitudes = raster.ReadVdata("Longitude", null, 0, h) as float[];
                    cou = 0;
                    minX = 0;
                    maxX = h;
                    IntersectEnvelope(latitudes, longitudes, env, ref cou, ref minX, ref maxX);
                    xn = maxX - minX + 1;
                    datas = readData(new int[] { minX, level }, new int[] { xn, 1 }, band);
                }
                if (cou > 0 && (minX != 0 || maxX != h))
                {
                    float[] lats = new float[xn];
                    float[] longs = new float[xn];
                    Buffer.BlockCopy(latitudes, minX * 4, lats, 0, xn * 4);
                    Buffer.BlockCopy(longitudes, minX * 4, longs, 0, xn * 4);
                    latitudes = lats;
                    longitudes = longs;
                }
                PointProjection.PointToRasterInt16(longitudes, latitudes, datas, env, resolutionx, resolutiony, osize, ref refValues);
            }
            GCHandle handle = GCHandle.Alloc(refValues, GCHandleType.Pinned);
            try
            {
                _band.Write(0, 0, ox, oy, handle.AddrOfPinnedObject(), enumDataType.Int16, ox, oy);
            }
            finally
            {
                handle.Free();
            }
        }

        private void IntersectEnvelope(float[] Latitude, float[] Longtitude, GeoDo.RSS.Core.DF.CoordEnvelope env,
            ref int count, ref int minIndex, ref int maxIndex)
        {
            minIndex = -1;
            maxIndex = -1;
            for (int i = 0; i < Latitude.Length; i++)
            {
                if (Longtitude[i] >= env.MinX && Longtitude[i] <= env.MaxX && Latitude[i] >= env.MinY && Latitude[i] <= env.MaxY)
                {
                    if (minIndex == -1)
                    {
                        minIndex = i;
                        maxIndex = i;
                    }
                    else
                    {
                        if (minIndex > i)
                            minIndex = i;
                        if (maxIndex < i)
                            maxIndex = i;
                    }
                    count++;
                }
            }
        }

        private static short[] readData(int[] start, int[] count, CloudSatRasterBand band)
        {
            if (start.Length != 2 || count.Length != 2)
                return null;
            int height = count[0];
            int width = count[1];
            int buffersize = width * height;
            int typesize = 4;
            IntPtr ptr = Marshal.AllocHGlobal(buffersize * typesize);
            try
            {
                band.Read(start[1], start[0], width, height, ptr, enumDataType.Int16, width, height);
                short[] buffer = new short[buffersize];
                Marshal.Copy(ptr, buffer, 0, buffersize);
                return buffer;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// 测试生成cloudsat栅格数据，这部分最后有界面调用。
        /// </summary>
        /// <param name="oraster"></param>
        public static void test(IRasterDataProvider oraster)
        {
            string fullfilename = @"E:\Smart\CloudArgs\2007109075222_05184_CS_2B-GEOPROF_GRANULE_P_R04_E02.hdf";
            List<string> files = new List<string>();
            files.Add(fullfilename);
            CloudsatToRaster r = new CloudsatToRaster();
            r.ToRaster(files.ToArray(), 1, 50, oraster);
        }

        public static void TestFromRaster(IRasterDataProvider raster)
        {
            string filename = Path.GetFileNameWithoutExtension(raster.fileName) + "_" + 1;
            filename = MifEnvironment.GetFullFileName(filename + ".ldf");
            IRasterDataProvider oraster = RasterMosaic.CreateRaster(filename, raster.CoordEnvelope, raster.ResolutionX * 2, raster.ResolutionY * 2, 1, raster);
            CloudsatToRaster.test(oraster);
            oraster.Dispose();
            
        }
    }
}
