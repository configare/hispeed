using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.DrawEngine;
using System.Diagnostics;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.RasterDrawing;
using System.Threading.Tasks;
using System.Threading;
using GeoDo.RSS.CA;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;
//using OSGeo.GDAL;
using GeoDo.RSS.Core.VectorDrawing;
using CodeCell.AgileMap.Components;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.View;
using GeoDo.RSS.Core.Grid;
using System.Xml.Linq;
using GeoDo.Core;

namespace Test
{
    public partial class Form2 : Form, IPerformanceWatch, ILoadingPrecentSubscriber
    {
        private ICanvas _canvas = null;
        private IRasterLayer _rasterLayer = null;

        public Form2()
        {
            InitializeComponent();
            Load += new EventHandler(Form1_Load);
            FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            GeoDataDriver.PreLoading();
            BandProviderFactory.PreLoading();
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _canvas.Dispose();
            GC.Collect();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            InitCanvas();
        }

        private void InitCanvas()
        {
            _canvas = canvasHost1.Canvas;
            //userControl11.MouseMove += new MouseEventHandler(userControl11_MouseMove);
            //_canvas.CurrentEnvelope = CoordEnvelope.FromLBWH(0, 0, 4066, 2810);
            ////_canvas.CurrentEnvelope = CoordEnvelope.FromLBWH(713, 921, 5575, 3158);
            //_canvas.CurrentViewControl = new DefaultControlLayer();
            _canvas.PerformanceWatch = this;
        }

        void userControl11_MouseMove(object sender, MouseEventArgs e)
        {
            double prjX = 0, prjY = 0;
            int x = 0, y = 0;
            _canvas.CoordTransform.Screen2Prj(e.X, e.Y, out prjX, out prjY);
            _canvas.CoordTransform.Prj2Screen(prjX, prjY, out x, out y);
            txtPrjCoord.Text = "{prjX=" + prjX.ToString() + ",prjY=" + prjY.ToString() + "} from {" + x.ToString() + "," + y.ToString() + "}";
            //
            //_canvas.CoordTransform.QuickTransform.Transform(ref prjX, ref prjY);
            //Text = "from {" + prjX.ToString() + "," + prjY.ToString() + "}";
            //_canvas.CoordTransform.QuickTransform.InverTransform(ref prjX, ref prjY);
            //Text += " from {" + prjX.ToString() + "," + prjY.ToString() + "}";
            //
            txtScreenCoord.Text = e.Location.ToString();
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            _canvas.CurrentViewControl = new DefaultControlLayer();
        }

        private void btnZoomBox_Click(object sender, EventArgs e)
        {
            _canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomIn);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomOut);
            //_canvas.CurrentViewControl = new PencilDrawingTool();
        }

        private Stopwatch _stopWatch = new Stopwatch();

        #region IPerformanceWatch 成员

        public void BeginWatch(string identify)
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        public void EndWatch(string identify)
        {
            _stopWatch.Stop();
            this.Text = (identify ?? string.Empty) + ":" + _stopWatch.ElapsedMilliseconds.ToString();
        }

        #endregion

        private void btnDummy_Click(object sender, EventArgs e)
        {
            //IPencilDrawingTool tool = _canvas.CurrentViewControl as IPencilDrawingTool;
            //if (tool != null)
            //{
            //    GeoDo.RSS.Core.DrawEngine.CoordEnvelope env = tool.GetAOI().Envelopes[0];
            //}

            _canvas.Scale = 1f;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _canvas.Scale = (float)numericUpDown1.Value / 100f;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnCreateRasterDrawing_Click(object sender, EventArgs e)
        {
            string fname = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "(*.ldf)|*.ldf";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fname = dlg.FileName;
                else
                    return;
            }

            string ops = null;
            if (fname.EndsWith(".txt"))
                ops = "ComponentID=0000000";
            IRasterDrawing drawing = new RasterDrawing(fname, _canvas, null, ops);
            drawing.LoadingSubscribers.Add(this);
            drawing.SelectedBandNos = new int[] { 1,2,3 };
            IRasterLayer lyr = new RasterLayer(drawing);
            _canvas.LayerContainer.Layers.Add(lyr);
            _canvas.PrimaryDrawObject = drawing;
            _canvas.CurrentEnvelope = drawing.OriginalEnvelope;
            drawing.StartLoading((t, p) => { Text = p.ToString() + "/" + t.ToString(); });
            _canvas.Refresh(enumRefreshType.All);
            _rasterLayer = lyr;
            //_canvas.OnEnvelopeChanged += new EventHandler(CanvasEnvelopeChanged);
        }

        void CanvasEnvelopeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("//////////////////////////////////////////////");
            Console.WriteLine("//////////////////////////////////////////////");
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope dataEvp = drawing.OriginalEnvelope;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope wndEvp = _canvas.CurrentEnvelope;
            //Console.WriteLine(dataEvp.ToString());
            //Console.WriteLine(wndEvp.ToString());
            int bCol = (int)((wndEvp.MinX - dataEvp.MinX) / drawing.OriginalResolutionX);
            int bRow = (int)((dataEvp.MaxY - wndEvp.MaxY) / drawing.OriginalResolutionX);
            int width = (int)(wndEvp.Width / drawing.OriginalResolutionX);
            int height = (int)(wndEvp.Height / drawing.OriginalResolutionX);
            Rectangle rect = new Rectangle(bCol, bRow, width, height);
            //Console.WriteLine(rect.ToString());
            float scale = 1 / (_canvas.ResolutionX / drawing.OriginalResolutionX);
            if (_tileMemeoryCacheManager == null)
                btnSchedulerII_Click(null, null);
            _tileMemeoryCacheManager.UpdateRasterEnvelope(scale, bCol, bRow, width, height);
        }

        private void canvasHost1_MouseMove(object sender, MouseEventArgs e)
        {
            double prjX = 0, prjY = 0;
            int x = 0, y = 0;
            _canvas.CoordTransform.Screen2Prj(e.X, e.Y, out prjX, out prjY);
            _canvas.CoordTransform.Prj2Screen(prjX, prjY, out x, out y);
            txtPrjCoord.Text = "{prjX=" + prjX.ToString() + ",prjY=" + prjY.ToString() + "} from {" + x.ToString() + "," + y.ToString() + "}";
            int rasterX, rasterY;
            _canvas.CoordTransform.Screen2Raster((float)e.X, (float)e.Y, out rasterY, out rasterX);
            double geoX, geoY;
            _canvas.CoordTransform.Prj2Geo(prjX, prjY, out geoX, out geoY);
            txtRasterCoord.Text = string.Format("geoX:{0},geoY:{1}", geoX, geoY);
            _canvas.CoordTransform.Prj2Screen(prjX, prjY, out x, out y);
            label4.Text = "ScrX:" + x.ToString() + "," + "SrcY:" + y.ToString();
            //_canvas.CoordTransform.Raster2Geo(rasterX, rasterY, out geoX, out geoY);
            Text = string.Format("rasterX:{0},rasterY:{1}", rasterX.ToString(), rasterY.ToString());
            //
            //_canvas.CoordTransform.QuickTransform.Transform(ref prjX, ref prjY);
            //Text = "from {" + prjX.ToString() + "," + prjY.ToString() + "}";
            //_canvas.CoordTransform.QuickTransform.InverTransform(ref prjX, ref prjY);
            //Text += " from {" + prjX.ToString() + "," + prjY.ToString() + "}";
            //
            //txtScreenCoord.Text = e.Location.ToString();
        }

        private void btnChangeBands_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            drawing.SelectedBandNos = new int[] { 3, 2, 1 };
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            drawing.SelectedBandNos = new int[] { 1, 2, 3 };
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Size size = new System.Drawing.Size(10, 10);
            Int16[] data = new Int16[size.Width * size.Height];
            for (int r = 0; r < size.Height; r++)
            {
                for (int c = 0; c < size.Width; c++)
                {
                    data[r * size.Width + c] = 0;
                }
            }
            Dictionary<Int16, Color> colormap = new Dictionary<short, Color>();
            colormap.Add(0, Color.FromArgb(64, 0, 0, 255));
            //
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(
                117.66, 118.66, 47.80, 48.80);
            MemoryRaster<Int16> raster = new MemoryRaster<short>("FIRE", data, size, 2, evp, true);

            MemoryBitmapLayer<Int16> layer = new MemoryBitmapLayer<short>(raster, colormap);
            //
            _canvas.LayerContainer.Layers.Add(layer);
            _canvas.Refresh(enumRefreshType.All);
            //layer.Update();
            //layer.UpdateColorMap(Dictionary<Int16,Color>

        }

        private void btnGDALInfo_Click(object sender, EventArgs e)
        {
            //Gdal.SetCacheMax(1024 * 1024 * 500);
            //int maxCacheSize = Gdal.GetCacheMax();
            //int usedCacheSize = Gdal.GetCacheUsed();
            //Text = maxCacheSize.ToString("###,###") + "              " + usedCacheSize.ToString("###,###");
        }

        public void Percent(string identfy, int percent)
        {
            Text = identfy + " : " + percent.ToString() + "%";
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            (_rasterLayer.Drawing as IRasterDrawing).Active();
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnDeactive_Click(object sender, EventArgs e)
        {
            (_rasterLayer.Drawing as IRasterDrawing).Deactive();
            _canvas.Refresh(enumRefreshType.All);
        }

        private VectorHostLayer _vectorHostLayer = null;
        private void btnAddVector_Click(object sender, EventArgs e)
        {
            if (_vectorHostLayer != null)
                return;
            string spaRef = null;
            if (_rasterLayer != null)
            {
                IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
                //GeoDo.Project.ISpatialReference spatialRef = drawing.DataProvider.SpatialRef;
                //spaRef = spatialRef.ToWKTString();
                spaRef = drawing.SpatialRef;
            }
            _vectorHostLayer = new VectorHostLayer(spaRef);
            _canvas.LayerContainer.Layers.Add(_vectorHostLayer);
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnLyrMgr_Click(object sender, EventArgs e)
        {
            object c = _canvas.LayerContainer.VectorHost;

            frmLayerManager frm = new frmLayerManager();
            frm.Apply(_vectorHostLayer.Map as IMap);
            frm.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //GeoDo.RSS.Core.DrawEngine.ILayer l = _canvas.LayerContainer.GetByName("FIRE");
            //_canvas.LayerContainer.Layers.Remove(l);
            //_canvas.Refresh(enumRefreshType.FlyLayer);
            SaveMapDialog.Save(_vectorHostLayer.Map as IMap, true);
        }

        private void btnAddData_Click(object sender, EventArgs e)
        {
            if (_vectorHostLayer == null)
                btnAddVector_Click(null, null);
            //using (OpenFileDialog dlg = new OpenFileDialog())
            //{
            //    dlg.Filter = "Shape Files(*.shp)|*.shp";
            //    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        _vectorHostLayer.AddData(dlg.FileName, null);

            //        _canvas.Refresh(enumRefreshType.All);
            //    }
            //}
            string fname = @"F:\产品与项目\MAS-II\SMART0718\SMART\【控制】UI框架(通用)\Demo\bin\Release\数据引用\基础矢量\行政区划\面\县级行政区域_面.shp";
            _vectorHostLayer.AddData(fname, null);
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (_vectorHostLayer == null)
                btnAddVector_Click(null, null);
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Map Config File(*.mcd)|*.mcd";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _vectorHostLayer.ChangeMap(dlg.FileName);
                    _canvas.Refresh(enumRefreshType.All);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            (_rasterLayer.Drawing as IRasterDrawing).TryCreateOrbitPrjection();
        }

        ITileMemoryCacheManager _tileMemeoryCacheManager = null;
        private void btnSchedulerII_Click(object sender, EventArgs e)
        {
            CacheSettings settings = new CacheSettings();
            settings.CacheDir = "E:\\SMART_CACHE";
            settings.WndExtandMultiple = 3;
            settings.TileSize = 512;
            _tileMemeoryCacheManager = new TileMemoryCacheManager(settings, _canvas.Container.Size,
                (_rasterLayer.Drawing as IRasterDrawing).SelectedBandNos, GetDataProvider(_rasterLayer.Drawing as IRasterDrawing));
            _tileMemeoryCacheManager.LoadBySync_MaxLevel();
            _tileMemeoryCacheManager.Start();
        }

        private unsafe IRasterDataProvider GetDataProvider(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(drawing.FileName) as IRasterDataProvider;
            return prd;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            _tileMemeoryCacheManager.ReduceMemory();
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnGetVisibleTiles_Click(object sender, EventArgs e)
        {
            Console.WriteLine("////////////");
            Console.WriteLine("///Visible Tiles:");
            TileData[][] tiles = _tileMemeoryCacheManager.GetVisibleTileDatas();
            for (int b = 0; b < tiles.Length; b++)
            {
                TileData[] bandTiles = tiles[b];
                Console.WriteLine("/// Band " + b.ToString());
                foreach (TileData t in bandTiles)
                {
                    Console.WriteLine("     " + t.Tile.ToString());
                    using (IBitmapBuilder<byte> builder = BitmapBuilderFactory.CreateBitmapBuilderByte())
                    {
                        Bitmap bm = new Bitmap(t.Tile.Width, t.Tile.Height, PixelFormat.Format8bppIndexed);
                        bm.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                        builder.Build(t.Tile.Width, t.Tile.Height, t.Data as Byte[], ref bm);
                        bm.Save("d:\\B" + t.BandNo.ToString() + "_L" + t.Tile.LevelNo.ToString() + "_R" +
                            t.Tile.Row.ToString() + "_C" + t.Tile.Col.ToString() + ".bmp", ImageFormat.Bmp);
                    }
                }
            }
            //
            BuildBitmap(tiles);
        }

        private void BuildBitmap(TileData[][] tiles)
        {
            WindowBitmapBuilder wbb = new WindowBitmapBuilder(_canvas.Container.Size, _tileMemeoryCacheManager.TileComputer);
            wbb.Build(tiles, _tileMemeoryCacheManager.RasterEnvelopeOfWnd);
            Bitmap bm = wbb.Bitmap;
            bm.Save("d:\\1.bmp", ImageFormat.Bmp);
        }

        private unsafe void button4_Click(object sender, EventArgs e)
        {
            int count = 100;
            Stopwatch sw = new Stopwatch();
            byte[] buffer = new byte[512 * 512 * 3];
            fixed (byte* ptr = buffer)
            {
                IntPtr scan0 = new IntPtr(ptr);
                sw.Start();
                while (count-- > 0)
                {
                    using (Bitmap bm = new Bitmap(512, 512, 512 * 3, PixelFormat.Format24bppRgb, scan0))
                    {
                    }
                }
                sw.Stop();
            }
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            _rasterLayer = null;
            this.Controls.Remove(canvasHost1);
            _canvas = null;
            CanvasHost host = this.canvasHost1;
            this.canvasHost1 = null;
            (host as IDisposable).Dispose();
            (host as UserControl).Dispose();
            GC.Collect();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            GeoGridLayer_I layer = new GeoGridLayer_I();
            _canvas.LayerContainer.Layers.Add(layer);
            _canvas.Refresh(enumRefreshType.All);
        }


        private void button28_Click(object sender, EventArgs e)
        {
            GeoGridLayer layer = new GeoGridLayer();
            //layer.GridSpan = 1f;
            _canvas.LayerContainer.Layers.Add(layer);
            _canvas.Refresh(enumRefreshType.All);
        }

        GeoDo.RSS.Core.DF.CoordEnvelope oldEvp;
        GeoDo.RSS.Core.DF.CoordEnvelope Evp;
        private void button13_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            oldEvp = drawing.DataProvider.CoordEnvelope;
            drawing.Start();
            drawing.ApplyAdjust(10d, 10d);
            drawing.Stop(true);
            Evp = drawing.DataProvider.CoordEnvelope;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            IFloatToolBarLayer toolbar = new FloatToolBarLayer();
            toolbar.ToolItemClicked = (it) => { Console.WriteLine("clicked:" + it.Text); };
            //toolbar.IsAutoHide = false;
            LoadToolItems(toolbar);
            _canvas.LayerContainer.Layers.Add(toolbar as GeoDo.RSS.Core.DrawEngine.ILayer);
            _canvas.Refresh(enumRefreshType.All);
        }

        private void LoadToolItems(IFloatToolBarLayer toolbar)
        {
            FloatToolItem it;
            Image img = Image.FromFile(@"F:\杂项\服务.png");
            it = new FloatToolItem("AOI(自由多边形)", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("AOI区域(矩形)", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("AOI区域(圆形)", img);
            toolbar.ToolItems.Add(it);
            toolbar.ToolItems.Add(new FloatToolItem("-"));
            it = new FloatToolItem("橡皮檫", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("魔术棒", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("闪烁", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("撤销", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("重做", img);
            toolbar.ToolItems.Add(it);
            it = new FloatToolItem("清除判识结果", img);
            toolbar.ToolItems.Add(it);
        }

        //PointF[] ptfs;
        //Point[] pts;
        GeometryOfDrawed _result;
        IAOIContainerLayer _aoiLayer;
        private void button15_Click(object sender, EventArgs e)
        {
            if (_aoiLayer == null)
            {
                _aoiLayer = new AOIContainerLayer();
                _aoiLayer.Color = Color.Red;
                _canvas.LayerContainer.Layers.Add(_aoiLayer as GeoDo.RSS.Core.DrawEngine.ILayer);
            }
            IPencilToolLayer layer = new PencilToolLayer();
            layer.PencilType = enumPencilType.FreeCurve;
            layer.PencilIsFinished = (result) =>
            {
                //ptfs = result.GeoPoints;
                //pts = result.RasterPoints;
                _result = result;
                _aoiLayer.AddAOI(_result);
            };
            _canvas.CurrentViewControl = layer;
        }

        ILayerFlashControler fsh;
        private void button16_Click(object sender, EventArgs e)
        {
            if (fsh == null)
                fsh = new LayerFlashControler(_canvas, _canvas.LayerContainer.Layers[0]);
            //fsh.AutoFlash(1000);
            fsh.Flash();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            drawing.Bitmap.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ColorMapTable<double> colorTable = new ColorMapTable<double>();
            colorTable.Items.Add(new ColorMapItem<double>(0, 35, Color.Red));
            colorTable.Items.Add(new ColorMapItem<double>(35, 36, Color.Green));
            colorTable.Items.Add(new ColorMapItem<double>(36, 37, Color.Blue));
            colorTable.Items.Add(new ColorMapItem<double>(37, 38, Color.Green));
            colorTable.Items.Add(new ColorMapItem<double>(38, 39, Color.Yellow));
            colorTable.Items.Add(new ColorMapItem<double>(39, 40, Color.YellowGreen));
            //
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            drawing.ApplyColorMapTable(colorTable);
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Bitmap bm = _canvas.ToBitmap();
            bm.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _rasterLayer.Drawing as IRasterDrawing;
            Bitmap bm = _canvas.FullRasterRangeToBitmap();
            if (bm != null)
                bm.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(dlg.FileName) as IVectorFeatureDataReader;
                    Feature fet = dr.FetchFirstFeature();
                    //fet = dr.FetchFeature((f) => { return f.GetFieldValue("CNTRY_NAME") == "China"; });
                    _aoiLayer.AddAOI(fet);
                    _canvas.Refresh(enumRefreshType.All);
                }
            }
        }

        private void AVILayer_Click(object sender, EventArgs e)
        {
            string[] fnames = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "栅格文件(*.dat)|*.dat";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fnames = dlg.FileNames;
                else
                    return;
            }
            IRasterDataProvider prd;
            List<Bitmap> bmps = new List<Bitmap>();
            Bitmap bp;
            foreach (string fname in fnames)
            {
                using (prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
                {
                    IOverviewGenerator ovg = prd as IOverviewGenerator;
                    Size size = ovg.ComputeSize(800);
                    bp = new Bitmap(size.Width, size.Height, PixelFormat.Format8bppIndexed);
                    bp.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                    ovg.Generate(new int[] { 1 }, ref bp);
                    if (bp != null)
                        bmps.Add(bp);
                }
            }
            if (bmps.Count == 0)
                return;
            //IAVILayer aviLyr = new AVILayer(bmps.ToArray(), new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(112, 113, 39, 40), 500);
            //aviLyr.IsRunning = true;
            //_canvas.LayerContainer.Layers.Add(aviLyr);
            ////_canvas.PrimaryDrawObject = drawings[0];
            ////_canvas.CurrentEnvelope = drawings[0].OriginalEnvelope;
            ////for (int i = 0; i < drawings.Length; i++)
            ////    drawings[i].StartLoading((t, p) => { Text = p.ToString() + "/" + t.ToString(); });
            //_canvas.Refresh(enumRefreshType.All);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_canvas.Scale.ToString());
        }

        private void button23_Click(object sender, EventArgs e)
        {
            _canvas.Refresh(enumRefreshType.All);
        }

        SimpleVectorObjectHost _simpleVectorObjectHost;
        private void button24_Click(object sender, EventArgs e)
        {
            if (_simpleVectorObjectHost == null)
                _simpleVectorObjectHost = new SimpleVectorObjectHost("AOI", _canvas);
            _simpleVectorObjectHost.Add(GetVectorObject());
        }

        private ISimpleVectorObject GetVectorObject()
        {
            return new SimpleVectorObject("AB", new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(110, 112, 38, 40));
        }

        private void button25_Click(object sender, EventArgs e)
        {
            _canvas.StrongRefresh();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            GeoGridLayer_I g = new GeoGridLayer_I();
            XElement x = g.ToXml();
            GeoGridLayer_I b = GeoGridLayer_I.FromXml(x);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            _canvas.CanvasSetting.RenderSetting.BackColor = Color.FromArgb(180, 198, 212);
            string fname = @"F:\产品与项目\MAS-II\SMART0718\SMART\【控制】UI框架(通用)\demo\bin\Release\数据引用\基础矢量\矢量模版\海陆模版.shp";
            BackgroundLayer lyr = new BackgroundLayer(fname);
            _canvas.LayerContainer.Layers.Add(lyr);
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            _canvas.SetToFullEnvelope();
            _canvas.Refresh(enumRefreshType.All);

        }

        private void button30_Click(object sender, EventArgs e)
        {
            IVectorDataGlobalCacher cacher = new VectorDataGlobalCacher();
            //
            GlobalCacher.VectorDataGlobalCacher = cacher;
            //
            ICachedVectorData data;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    data = cacher.Request(dlg.FileName);
                }
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            ScaleRulerLayer lyr = new ScaleRulerLayer();
            _canvas.LayerContainer.Layers.Add(lyr);
        }

        private void button32_Click(object sender, EventArgs e)
        {
           
        }

        private void button33_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "bitmap files(*.bmp)|*.bmp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bm = drawing.GetBitmapUseOriginResolution();
                    bm.Save(dlg.FileName, ImageFormat.Bmp);
                }
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            IPencilToolLayer lyr = new PencilToolLayer();
            lyr.PencilType = enumPencilType.ControlFreeCurve;
            //
            _canvas.CurrentViewControl = lyr;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Type type = typeof(ContourLayer);
            Text = type.Name;
        }

        private void button35_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "等值线(*.xml)|*.xml";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IContourLayer lyr = ContourLayer.FromXml(dlg.FileName, true);
                    if (lyr != null)
                    {
                        _canvas.LayerContainer.Layers.Add(lyr);
                        _canvas.Refresh(enumRefreshType.All);
                    }
                    //
                    //Object2Xml obj2xml = new Object2Xml();
                    //obj2xml.ToXmlFile(lyr, "f:\\xxx.xml");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button36_Click(object sender, EventArgs e)
        {
            RulerLayer lyr = new RulerLayer();
            _canvas.LayerContainer.Layers.Add(lyr);
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnToXml_Click(object sender, EventArgs e)
        {
            if (_canvas == null)
                return;
            Object2Xml obj2xml = new Object2Xml();
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer lyr in _canvas.LayerContainer.Layers)
            {
                obj2xml.ToXmlFile(lyr, "f:\\" + lyr.Name + ".xml");
            }
        }

        private void button31_Click_1(object sender, EventArgs e)
        {
            _canvas.IsReverseDirection = !_canvas.IsReverseDirection;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            _canvas.LayerContainer.Layers.Remove(_rasterLayer);
            _canvas.PrimaryDrawObject = null;
            _rasterLayer.Dispose();
            _rasterLayer = null;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            _canvas.LayerContainer.Layers.Remove(_vectorHostLayer);
            _vectorHostLayer.Dispose();
            _vectorHostLayer = null;
            _canvas.Refresh(enumRefreshType.All);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            //IDiskCacheManager disk = new DiskCacheManager(new TileSetting(512,2), @"f:\\cache");
            //string fname = @"E:\DATA\VIRR\FY3A_VIRRX_WHOLE_GLL_L1_20110322_0525_1000M_MS.LDF";
            //fname = @"D:\Z709_BUG\ljm.tif";
            ////fname = @"E:\DATA\MERSI\FY3A_MERSI_WHOLE_GLL_L1_20110501_0250_0250M_MS.LDF";
            ////fname = @"E:\DATA\MERSI\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS.HDF";
            ////fname = @"E:\DATA\MERSI\FY3A_MERSI_GBAL_L1_20110501_0245_1000M_MS.HDF";
            //fname = @"E:\DATA\NM\AQUA_X_2013_05_12_13_35_A.MOD02QKM_GLL_WHOLE.LDF";
            //IFileDiskCacheManager fCache = disk.Active(fname);
            //fCache.Start();
            //disk.Dispose();
            //Text = "finished.";
        }
    }
}
