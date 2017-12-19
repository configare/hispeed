using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using GeoDo.RSS.UI.WinForm;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using System.Diagnostics;
using System.IO;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    [Export(typeof(GeoDo.RSS.Core.UI.ICommand))]
    public class CommandCloudsatDisplay : Command
    {
        #region 测试效率
        Stopwatch sw = new Stopwatch();
        long elapsed = 0;
        #endregion

        public CommandCloudsatDisplay()
            : base()
        {
            _id = 29001;
            _name = "CloudsatDisplay";
            _text = "Cloudsat显示";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            string fullfilename;// = @"E:\原始数据\CLOUDSAT\2007\4\2007101052404_05066_CS_2B-GEOPROF_GRANULE_P_R04_E02.hdf";
            string maskpng = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\CLD\Cloudsat_Overview_withcountry.png";

            if (string.IsNullOrWhiteSpace(argument) || !File.Exists(argument))
                return;
            fullfilename = argument;
            ICommand cmd = _smartSession.CommandEnvironment.Get(2000);
            if (cmd != null)
                cmd.Execute(maskpng);
            GeoDo.RSS.Core.DF.CoordEnvelope env = TryGetActiveViewerEnv();
            if (env == null)
                env = new RSS.Core.DF.CoordEnvelope(65, 145, 10, 60);
            //如果测试整个影像数据，则输出图像为：0,37081,37081
            int x1 = 0;
            int x2 = 37081;
            int outnx = 2000;
            int h1 = -1;     //# km
            int h2 = 21;     //# km
            int nz = 400;    //# Number of pixels in the vertical.（这个偏移需要指定）
            int dtnx = 0;    //125
            int dtny = 0;    //13000
            int xn = x2 - x1;//值从1700到2000,要读取的实际横轴方向数据。（最后这个偏移会通过地理范围，或者时间范围，计算出来）
            //return;
            CloudsatDataProvider raster = GeoDataDriver.Open(fullfilename) as CloudsatDataProvider;
            CloudSatRasterBand band = raster.GetRasterBand(1) as CloudSatRasterBand;
            dtnx = band.Width;//层数
            dtny = band.Height;//点数
            sw.Start();
            float[] Latitude = raster.ReadVdata("Latitude", null, x1, x2) as float[];
            float[] Longtitude = raster.ReadVdata("Longitude", null, x1, x2) as float[];
            sw.Stop();
            elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine("ReadVdata" + elapsed + "毫秒");
            int cou = 0;
            int minIndex = 0;
            int maxIndex = Latitude.Length;
            IntersectEnvelope(Latitude, Longtitude, env, ref cou, ref minIndex, ref maxIndex);

            Console.WriteLine(cou + "点在指定范围内");
            float[] times = raster.ReadVdata("Profile_time", null, x1, x2) as float[];
            if (cou > 0 && (x1 != minIndex || x2 != maxIndex))
            {
                x1 = minIndex;
                x2 = maxIndex;
                xn = x2 - x1 + 1;
                float[] lats = new float[xn];
                float[] longs = new float[xn];
                float[] ts = new float[xn];
                Buffer.BlockCopy(Latitude, x1 * 4, lats, 0, xn * 4);
                Buffer.BlockCopy(Longtitude, x1 * 4, longs, 0, xn * 4);
                Buffer.BlockCopy(times, x1 * 4, ts, 0, xn * 4);
                Latitude = lats;
                Longtitude = longs;
                times = ts;
            }
            string start_time = raster.ReadAttribute("start_time") as string;
            DateTime dt_start_time;
            dt_start_time = DateTime.ParseExact(start_time, "yyyyMMddHHmmss", null);
            List<DateTime> dts = new List<DateTime>();
            foreach (float time in times)
            {
                dts.Add(dt_start_time.AddMinutes(time));//应为AddSeconds()
            }
            short[] heights = readSDS("Height", new int[] { x1, 0 }, new int[] { xn, dtnx }, raster);//p1c1,p1c2,..,p1c125,p2c1,...
            short[] datas = readData(new int[] { x1, 0 }, new int[] { xn, dtnx }, band);

            float[] X = new float[xn];//原始数据中的数据点号
            for (int i = 0; i < xn; i++)
            {
                X[i] = i + x1;
            }
            //heights数据是x=125,y=100=>Z[100,125]
            float[,] Z = new float[xn, dtnx];   //每个点的高度xn = 100, dtnx =125;
            for (int i = 0; i < dtnx; i++)//heights的列,层数
            {
                for (int j = 0; j < xn; j++)//heights的行,点数
                {
                    Z[j, i] = heights[j * dtnx + i] * 0.001f;//m-->km
                }
            }

            short[,] newdata = new short[xn, dtnx];//点数，层数
            var dest = Marshal.UnsafeAddrOfPinnedArrayElement(newdata, 0);
            Marshal.Copy(datas, 0, dest, datas.Length);//将数据的一维数组变化为2维数组
            Marshal.Release(dest);

            short[,] dat = Rote<short>(newdata);//调换数组的行列，dat为层数×点数的数组；

            Bitmap bmp = null;
            IntPtr src;
            outnx = xn < 4000 ? xn : 4000;
            float[,] dataf = interp2d_12(newdata, X, Z, x1, x2, outnx, h2, h1, nz);   //将数据进行插值         
            float[,] newdataf = Rote<float>(dataf);//调换数组的行列，dat为层数×点数的数组；
            int count = newdataf.Length;
            float[] datac = new float[count];
            src = Marshal.UnsafeAddrOfPinnedArrayElement(newdataf, 0);//
            Marshal.Copy(src, datac, 0, newdataf.Length);
            Marshal.Release(dest);
            DisplayLonLat(Latitude, Longtitude, fullfilename);
            //string[] dtimes = new string[dts.Count];
            //for(int i =0;i< dts.Count;i++)
            //{
            //    DateTime dt  = dts[i];
            //    dtimes[i] = dt.ToString("HH:mm:ss");
            //}
            //try
            //{
            //    bmp = ToBitmap(outnx, nz, datac);//点数(宽)，层数(高)
            //    if (bmp != null)
            //    {
            //        string bmpfilename = MifEnvironment.GetFullFileName("2B-GEOPROF.Radar_Reflectivity.bmp");
            //        bmp.Save(bmpfilename);
            //        //OpenFileFactory.Open(bmpfilename);

            //        CloudsatPlotWnd wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id) as CloudsatPlotWnd;
            //        if (wnd != null)
            //        {
            //            _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(System.Windows.Forms.DockStyle.Bottom, false));
            //            wnd.Reset(fullfilename, bmp, x1, x2, h1, h2, dtimes, null);
            //        }

            //        frmPlot frm = new frmPlot();
            //        ucCloudsatPlot plots = frm.plots;
            //        plots.AddYAxis(dtimes);
            //        plots.Reset(fullfilename, bmp, x1, x2, h1, h2, null);
            //        //plots.Rerender();
            //        //frm.Reset(fullfilename, bmp, x1, x2, h1, h2, dtimes, null);
            //        frm.Show();
            //    }
            //}
            //finally
            //{
            //    //if (bmp != null)
            //    //    bmp.Dispose();
            //}
        }

        /// <summary>
        /// 判断经纬度数据集与空间范围的交集
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longtitude"></param>
        /// <param name="env"></param>空间范围
        /// <param name="count"></param>范围内点数
        /// <param name="minIndex"></param>起始点序号
        /// <param name="maxIndex"></param>终止点序号
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

        /// <summary>
        /// 对数组进行转置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private T[,] Rote<T>(T[,] data)
        {
            int x = data.GetUpperBound(0) + 1;
            int y = data.GetUpperBound(1) + 1;
            T[,] retData = new T[y, x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    retData[j, i] = data[i, j];
                }
            }
            return retData;
        }

        private static short[] readSDS(string sdsName, int[] start, int[] count, CloudsatDataProvider raster)
        {
            int width = count[0];
            int height = count[1];
            int buffersize = width * height;
            int typesize = 2;
            IntPtr ptr = Marshal.AllocHGlobal(buffersize * typesize);
            try
            {
                bool stat = raster.ReadSDS(sdsName, start, count, ptr);
                if (stat)
                {
                    short[] buffer = new short[buffersize];
                    Marshal.Copy(ptr, buffer, 0, buffersize);
                    return buffer;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
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
            band.Read(start[1], start[0], width, height, ptr, enumDataType.Int16, width, height);
            short[] buffer = new short[buffersize];
            Marshal.Copy(ptr, buffer, 0, buffersize);
            Marshal.FreeHGlobal(ptr);
            return buffer;
        }

        /// <summary>
        /// 将数据转换为图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="width"></param>数据宽度
        /// <param name="height"></param>数据高度
        /// <param name="buffer"></param>数据
        /// <returns></returns>
        private Bitmap ToBitmap<T>(int width, int height, T[] buffer)
        {
            string colorTableName = string.Format("Cloudsat.{0}", "2B-GEOPROF.Radar_Reflectivity");
            ProductColorTable productColorTable = ProductColorTableFactory.GetColorTable(colorTableName);
            RgbStretcherProvider stretcherProvier = new RgbStretcherProvider();
            ColorMapTable<int> colorMapTable = null;
            Func<T, byte> stretcher = null;
            IBitmapBuilder<T> builder = null;
            TypeCode t = Type.GetTypeCode(default(T).GetType());
            if (t == TypeCode.Single)
            {
                stretcher = stretcherProvier.GetStretcher(enumDataType.Float, productColorTable, out colorMapTable) as Func<T, byte>;
                builder = BitmapBuilderFactory.CreateBitmapBuilderFloat() as IBitmapBuilder<T>;
            }
            else if (t == TypeCode.Int16)
            {
                stretcher = stretcherProvier.GetStretcher(enumDataType.Int16, productColorTable, out colorMapTable) as Func<T, byte>;
                builder = BitmapBuilderFactory.CreateBitmapBuilderInt16() as IBitmapBuilder<T>;
            }
            Bitmap bitmap = null;
            bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            bitmap.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
            try
            {
                builder.Build(width, height, buffer, stretcher, ref bitmap);
                if (colorMapTable != null)
                {
                    ColorPalette plt = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                    for (int i = 0; i < 256; i++)
                        plt.Entries[i] = Color.Black;
                    int idx = 1;
                    foreach (ColorMapItem<int> item in colorMapTable.Items)
                    {
                        for (int v = item.MinValue; v < item.MaxValue; v++)
                            plt.Entries[idx] = item.Color;
                        idx++;
                    }
                    bitmap.Palette = plt;
                }
                return bitmap;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 展示cloudsat数据，相当于针对cloudsat数据在横轴和垂直高度上的投影或插值
        /// </summary>
        /// <param name="data">原始cloudsat数据集</param>
        /// <param name="X">X轴每个像元数据集【记录每个data每个像元的x值】</param>
        /// <param name="Z">Z轴数据集【记录了data对应每个像元的Z值】</param>
        /// <param name="x1">投影后x起始值【左】</param>
        /// <param name="x2">投影后x结束值【右】</param>
        /// <param name="nx">投影后x拉伸像素个数</param>
        /// <param name="z1">投影后z起始值[上]</param>
        /// <param name="z2">投影后z结束值[下]</param>
        /// <param name="nz">投影后z拉伸像素个数</param>
        /// <returns>投影后数据集[nx,nz]</returns>
        public float[,] interp2d_12(short[,] data, float[] X, float[,] Z,
            float x1, float x2, int nx,
            float z1, float z2, int nz)
        {
            float n1, n2;
            float m1, m2;
            float xs, zs;
            int w, h;
            xs = (x2 - x1) / nx;
            zs = (z2 - z1) / nz;
            w = data.GetUpperBound(0) + 1;
            h = data.GetUpperBound(1) + 1;
            float[,] output = new float[nx, nz];
            int[,] q = new int[nx, nz];
            for (int i = 0; i < w; i++)
            {
                n1 = i - 1 >= 0 ? ((X[i - 1] + X[i]) / 2 - x1) / xs : -1;   //x左边界.距离x1的像素数（左右边界是通过实际每个像素的X值计算的）
                n2 = i + 1 < w ? ((X[i + 1] + X[i]) / 2 - x1) / xs : nx;    //x右边界.距离x1的像素数
                if (n2 - n1 <= 1)
                    n1 = n2 = (X[i] - x1) / xs;
                for (int j = 0; j < h; j++)
                {
                    m1 = j - 1 >= 0 ? ((Z[i, j - 1] + Z[i, j]) / 2 - z1) / zs : -1; //z上边界.距离z1的像素数（上下边界是通过实际的Z值计算的）
                    m2 = j + 1 < h ? ((Z[i, j + 1] + Z[i, j]) / 2 - z1) / zs : nz;  //z下边界.距离z1的像素数
                    if (m2 - m1 <= 1)//如果上下边界小于1，则此方向不需要插值,上下边界统一为当前像素的值
                        m1 = m2 = (Z[i, j] - z1) / zs;
                    for (int n = (int)(n1 + 0.5); n < (int)(n2 + 0.5 + 1); n++)
                    {
                        for (int m = (int)(m1 + 0.5); m < (int)(m2 + 0.5 + 1); m++)
                        {
                            if (n < 0 || n >= nx) continue;
                            if (m < 0 || m >= nz) continue;
                            output[n, m] += data[i, j];
                            q[n, m] += 1;
                        }
                    }
                }
            }
            for (int n = 0; n < nx; n++)
            {
                for (int m = 0; m < nz; m++)
                    output[n, m] /= q[n, m];
            }
            return output;
        }

        public void DisplayLonLat(float[] lats, float[] lons,string fname)
        {
            Feature[] fs = ConstructPoint(lats, lons);

            //CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _smartSession);
            //_smartSession.SmartWindowManager.DisplayWindow(cv);

            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            ICanvas c = cv.Canvas;
            if (c == null)
                return;
            CodeCell.AgileMap.Core.IFeatureLayer layer = CreateAndLoadVectorLayerForMicaps(_smartSession, cv.Canvas, fname, fs);

        }

        private Feature[] ConstructPoint(float[] lats, float[] lons)
        {
            List<Feature> features = new List<Feature>();
            ShapePoint pt;
            string[] fieldValues = null;
            string[] fieldNames = null;
            //除去经度、纬度属性
            //int fieldCount = _fields.Length;
            int pointCount = lats.Length;
            List<ShapePoint> sps = new List<ShapePoint>();
            for (int oid = 0; oid < pointCount; oid++)
            {
                pt = new ShapePoint(lons[oid],lats[oid]);
                sps.Add(pt);
            }
            ShapeLineString[] parts = new ShapeLineString[] { new ShapeLineString(sps.ToArray()) };
            ShapePolyline spl = new ShapePolyline(parts);
            Feature f = new Feature(0, spl, fieldNames, fieldValues, null);
            features.Add(f);
            return features.ToArray();
        }

        public static CodeCell.AgileMap.Core.IFeatureLayer CreateAndLoadVectorLayerForMicaps(ISmartSession session, ICanvas canvas, string fname, Feature[] features)
        {
            if (string.IsNullOrEmpty(fname) || !File.Exists(fname))
                return null;
            if (features != null)
            {
                MemoryDataSource mds = new MemoryDataSource(fname, enumShapeType.Polyline);
                IFeatureClass fetClass = new FeatureClass(mds);
                mds.AddFeatures(features);
                CodeCell.AgileMap.Core.IFeatureLayer fetLayer = new FeatureLayer(fname, fetClass);
                //TryApplyStyle(fetLayer, dataTypeId);
                IVectorHostLayer host = canvas.LayerContainer.VectorHost as IVectorHostLayer;
                if (host != null)
                {
                    host.Set(canvas);
                    IMapRuntime mapRuntime = host.MapRuntime as IMapRuntime;
                    if (mapRuntime != null)
                    {
                        IMap map = mapRuntime.Map as IMap;
                        if (map != null)
                        {
                            map.LayerContainer.Append(fetLayer);
                            FeatureLayer fetL = map.LayerContainer.Layers[0] as FeatureLayer;
                            FeatureClass fetC = fetL.Class as FeatureClass;
                            Envelope evp = fetC.FullEnvelope.Clone() as Envelope;
                            GeoDo.RSS.Core.DrawEngine.CoordEnvelope cvEvp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
                            canvas.CurrentEnvelope = cvEvp;
                            canvas.Refresh(enumRefreshType.All);
                        }
                    }
                    return fetLayer;
                }
            }
            return null;
        }

        public void AddVectorDataToCanvasViewer(string fname, bool isNeedCheckNormalImage, params object[] options)
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (v == null)
                return;
            ICanvas c = v.Canvas;
            if (c == null)
                return;
            IVectorHostLayer host = c.LayerContainer.VectorHost as IVectorHostLayer;
            if (host == null)
                return;
        }

        private GeoDo.RSS.Core.DF.CoordEnvelope TryGetActiveViewerEnv()
        {
            ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (v == null)
                return null;
            IRasterDrawing drawing = v.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return null;
            IRasterDataProvider raster = drawing.DataProvider;
            return raster.CoordEnvelope;
        }
    }
}
