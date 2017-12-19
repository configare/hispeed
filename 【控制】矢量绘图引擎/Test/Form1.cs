using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using System.Drawing.Imaging;
using CodeCell.AgileMap.Components;

namespace Test
{
    public partial class Form1 : Form
    {
        IMapImageGenerator gen = new MapImageGeneratorDefault();

        public Form1()
        {
            InitializeComponent();

            //IMap map = MapFactory.LoadMapFrom(@"E:\开发项目\AgileMapDevelopeWorkspace\配置好的地图\Map1\CC_MapLocalFile-Default_mercator.mcd");
            //gen.ApplyMap(map);
            //68.9, 3.14, 141.55, 54
            //minLat.Text = "3.14";
            //maxLat.Text = "54";
            //minLon.Text = "68.9";
            //maxLon.Text = "141.66";

            //double res = TileComputer.GroundResolution(0, 1);

            //int level = TileComputer.GetLevelOfDetail(new SizeF(40075020f, 39986840f), new SizeF(40075020f, 39986840f),20);

            //RectangleF fullRect = new RectangleF(-20037510f, -19993420, 40075020f, 39986840f);
            //TileSystemHelper h = new TileSystemHelper(fullRect,new Size(256,256), 20);
            //int totalWidth = 0, totalHeight = 0;
            //TileDef[] tiles = h.ComputeTiles(1, new RectangleF(-20037510f, -19993420, 40075020f, 39986840f),out totalWidth,out totalHeight);
            //tiles = h.ComputeTiles(2, new RectangleF(-20037510f, -19993420, 40075020f, 39986840f), out totalWidth, out totalHeight);
            //tiles = h.ComputeTiles(3, new RectangleF(-20037510f, -19993420, 40075020f, 39986840f), out totalWidth, out totalHeight);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RectangleF rectf = gen.GeoEnvelope2Viewport(new Envelope(-180, -90, 180, 90));
            RectangleF fullRect = new RectangleF(-20037510f, -19993420, 40075020f, 39986840f);
            TileSystemHelper h = new TileSystemHelper(rectf, new Size(256, 256), 20);
            int totalWidth = 0, totalHeight = 0;
            TileDef[] tiles = h.ComputeTiles(2, new RectangleF(-20037510f, -19993420, 40075020f, 39986840f), out totalWidth, out totalHeight); ;
            foreach (TileDef tile in tiles)
            {
                Size size = new Size(256, 256);
                Image img = new Bitmap(size.Width, size.Height);
                RectangleF rect = gen.GetMapImage(tile.Rect, size, ref img);
                img.Save("d:\\temp\\" + tile.Quadkey + ".png", ImageFormat.Png);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //IMap map = MapFactory.LoadMapFrom(@"E:\AgileMap项目\配置好的地图\Map1\CC_MapLocalFile-Default.mcd");
            IMap map = MapFactory.LoadMapFrom(@"d:\m.mcd");
            agileMapControl1.Apply(map);

            ShapePoint pt = new ShapePoint(116, 39);
            Feature fet = new Feature(0, pt, null, null, null);
            MemoryDataSource ds = new MemoryDataSource("memeory", enumShapeType.Point);
          
            FeatureClass fetc = new FeatureClass(ds);
            FeatureLayer fetlay = new FeatureLayer("memory", fetc);
            SimpleMarkerSymbol sym = new SimpleMarkerSymbol(masSimpleMarkerStyle.Circle);
            sym.Size = new System.Drawing.Size(10,10);
            fetlay.Renderer = new SimpleFeatureRenderer(sym);
            ds.AddFeatures(new Feature[] { fet });
            agileMapControl1.Map.LayerContainer.Append(fetlay);
          }

        private void button4_Click(object sender, EventArgs e)
        {
            //ShapePoint hitpt = new ShapePoint(116.5, 38.5);
            //agileMapControl1.ProjectionTransform.Transform(hitpt);

            //IFeatureLayer lyr = agileMapControl1.Map.LayerContainer.GetLayerByName("tes1");
            //Feature[] fets = lyr.Identify(hitpt, 0.0001);
            ////
            //PointF pixelPt = new PointF((float)hitpt.X,(float)hitpt.Y);
            //PointF[] pts = new PointF[] { pixelPt };
            //agileMapControl1.CoordinateTransfrom.PrjCoord2PixelCoord(pts);

            //lyr.FeatureClass.Remove(0);
            //(lyr.FeatureClass.DataSource as Ds).Refresh();

            ShapePoint pt = new ShapePoint(115, 39);
            Feature fet = new Feature(0, pt, null, null, null);

            IFeatureLayer lyr = agileMapControl1.Map.LayerContainer.GetLayerByName("memory") as IFeatureLayer;
            IFeatureClass fetclass = lyr.Class as IFeatureClass;
            (fetclass.DataSource as MemoryDataSource).AddFeatures(new Feature[] { fet });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            agileMapControl1.Map.SaveTo("d:\\m.mcd", false);

            //IFeatureLayer lyr = agileMapControl1.Map.LayerContainer.GetLayerByName("tes1");
            //(lyr.FeatureClass.DataSource as Ds).AddFeature();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //IFeatureLayer lyr = agileMapControl1.Map.LayerContainer.GetLayerByName("tes1");
            //(lyr.FeatureClass.DataSource as Ds).RemoveFeature();

            frmLayerManager lyrm = new frmLayerManager();
            lyrm.Apply(agileMapControl1.Map);
            lyrm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string f = @"F:\开发项目\城市应急平台\遥感数据\GeoData\World.rst"; ;
            RstReader r = new RstReader(f);
            Bitmap bm = r.Read(new Envelope(-180, -90, 180, 90), 512,256);
            bm.Save("f:\\1.bmp", ImageFormat.Bmp);
        }
    }

    public class Ds : FeatureDataSourceBase
    {
        public Ds(string name):base(name)
        { 
        }

        public override void BeginRead()
        {
            
            base.BeginRead();
        }

        public void Refresh()
        {
            _readIsFinished = false;
        }

        protected override void Init()
        {
            _isReady = true;
            _isInited = true;
            _shapeType = enumShapeType.Polygon;
            _coordType = enumCoordinateType.Geographic;
            _gridDefinition = new GridDefinition(360, 180);
            _fullEnvelope = new Envelope(-180, -90, 180, 90);
            _gridStateIndicator = new GridStateIndicator(_fullEnvelope.Clone() as Envelope, _gridDefinition);
            _fullGridCount = _gridStateIndicator.Width * _gridStateIndicator.Height;     
        }

        public override PersistObject ToPersistObject()
        {
            return null;
        }

        public override void EndRead()
        {
            base.EndRead();
        }



        public override IGrid ReadGrid(int gridNo)
        {
            //Feature fet = new Feature(0,new ShapePoint(112,39),null,null,null);
            Feature fet = GetFeature();
            IGrid gd = new Grid(gridNo, new Envelope(-180, -90, 180, 90), new Feature[] { fet });
            return gd;
        }

        private Feature GetFeature()
        {
            ShapePoint[] pts = new ShapePoint[] 
            {
                new ShapePoint(116,39),
                new ShapePoint(117,39),
                new ShapePoint(117,38),
                new ShapePoint(116,38)
            };
            ShapeRing ring = new ShapeRing(pts);
            ShapePolygon ply = new ShapePolygon(new ShapeRing[] { ring });
            Feature fet = new Feature(0, ply, null, null, null);
            return fet;
        }

        internal void AddFeature()
        {
            _featureClass.Grids[0].VectorFeatures.Add(GetFeature());
            _featureClass.TryProject(_featureClass.Grids[0]);
        }

        internal void RemoveFeature()
        {
            _featureClass.Grids[0].VectorFeatures.Clear();
        }
    }
}
