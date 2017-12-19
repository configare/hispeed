using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.FileProject;
using CodeCell.AgileMap.Core;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class firstPageContent : UserControl,IDisposable
    {
        private ISmartSession _smartSession;
        private ISimpleMapControl _simpleMapControl;
        private ISimpleVectorObjectHost _aoiHost;
        private ISimpleVectorObjectHost _curFileHost;

        private firstPageContent()
        {
            InitializeComponent();
            this.Text = "首页";
            LoadMapViews();
            ucDefinedRegion1.CheckedChanged += new Action<PrjEnvelopeItem[]>(ucDefinedRegion1_CheckedChanged);
        }

        public firstPageContent(ISmartSession smartSession)
            :this()
        {
            this._smartSession = smartSession;
        }

        void ucDefinedRegion1_CheckedChanged(PrjEnvelopeItem[] obj)
        {
            UpdateBlock(obj);
        }

        private void UpdateBlock(PrjEnvelopeItem[] obj)
        {
            if (obj == null || obj.Length == 0)
                return;
            ShapeRing[] rings = new ShapeRing[obj.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                PrjEnvelope prjEnv = obj[i].PrjEnvelope;
                rings[i] = new ShapeRing(
                    new ShapePoint[]
                    {
                        new ShapePoint(prjEnv.LeftTop.X,prjEnv.LeftTop.Y),
                        new ShapePoint(prjEnv.LeftTop.X,prjEnv.RightBottom.Y),
                        new ShapePoint(prjEnv.RightBottom.X,prjEnv.RightBottom.Y),
                        new ShapePoint(prjEnv.RightBottom.X,prjEnv.LeftTop.Y)
                    });
            }
            ShapePolygon sp = new ShapePolygon(rings);
            _aoiObj.SetGeometry(sp);
        }

        SimpleVectorObject _aoiObj;

        private void AddVector(string name, PrjEnvelope prjEnv)
        {
            Core.DrawEngine.CoordEnvelope env = new Core.DrawEngine.CoordEnvelope(prjEnv.MinX, prjEnv.MaxX, prjEnv.MinY, prjEnv.MaxY);
            _aoiHost.Add(new SimpleVectorObject(name, env));
        }

        private void RemoveVector(string name)
        {
            _aoiHost.Remove(new Func<ISimpleVectorObject, bool>((i) => { return i.Name == name; }));
        }

        private void LoadMapViews()
        {
            try
            {
                UCSimpleMapControl map = new UCSimpleMapControl();
                _simpleMapControl = map as ISimpleMapControl;
                map.Dock = DockStyle.Fill;
                cvPanel.Visible = true;
                cvPanel.Controls.Add(map);
                map.Load += new EventHandler(map_Load);
            }
            catch
            {
                MsgBox.ShowInfo("缩略图加载失败，暂时不能使用缩略图功能");
            }
        }

        void map_Load(object sender, EventArgs e)
        {
            if (_simpleMapControl == null)
                return;
            _aoiHost = _simpleMapControl.CreateObjectHost("AOI");
            _curFileHost = _simpleMapControl.CreateObjectHost("CurFile");
            _aoiObj = new SimpleVectorObject("AOI", null);
            _aoiHost.Add(_aoiObj);
        }

        private void RefreshOverView()
        {
            _simpleMapControl.RemoveAllImageLayers();
            //for (int i = 0; i < _mosaicProjectionFileProvider.FileItems.Length; i++)
            //{
            //    if (_mosaicProjectionFileProvider.FileItems[i] != null)
            //    {
            //        string filename = _mosaicProjectionFileProvider.FileItems[i].MainFile.fileName;
            //        PrjEnvelope prjEnv = _mosaicProjectionFileProvider.FileItems[i].Envelope;
            //        Bitmap bmp = _mosaicProjectionFileProvider.FileItems[i].OverViewBmp;
            //        if (bmp != null && prjEnv != null)
            //        {
            //            Core.DrawEngine.CoordEnvelope env = new Core.DrawEngine.CoordEnvelope(prjEnv.MinX, prjEnv.MaxX, prjEnv.MinY, prjEnv.MaxY);
            //            _simpleMapControl.AddImageLayer(filename, bmp, env, true);
            //        }
            //    }
            //}
        }

        private void AddOverView(string pngfilename)
        {
            if (!File.Exists(pngfilename))
                return;
            string hdrfilename = pngfilename.Replace(".overview.png", ".hdr");
            if (!File.Exists(hdrfilename))
                return;
            HdrFile hdrFile = HdrFile.LoadFrom(hdrfilename);
            Core.DrawEngine.CoordEnvelope env = CoordEnvelopeFromHdr(hdrFile);
            Bitmap bmp = LoadImage(pngfilename);
            _simpleMapControl.AddImageLayer(pngfilename, bmp, env, true);
            _simpleMapControl.Render();
        }

        private Bitmap LoadImage(string filePath)
        {
            Bitmap bmp;
            using (Bitmap load = new Bitmap(filePath))
            {
                bmp = new Bitmap(load);
            }
            return bmp;
        }

        private void RemoveOverView(string pngfilename)
        {
            _simpleMapControl.RemoveImageLayer(pngfilename);
            _simpleMapControl.Render();
        }

        private Core.DrawEngine.CoordEnvelope CoordEnvelopeFromHdr(HdrFile hdrFile)
        {
            double w = hdrFile.Samples * hdrFile.MapInfo.XYResolution.Longitude;
            double h = hdrFile.Lines * hdrFile.MapInfo.XYResolution.Latitude;

            double minX = hdrFile.MapInfo.BaseMapCoordinateXY.Longitude - (hdrFile.MapInfo.BaseRowColNumber.X - 1) * hdrFile.MapInfo.XYResolution.Longitude;
            double maxX = w + minX;
            double maxY =hdrFile.MapInfo.BaseMapCoordinateXY.Latitude + (hdrFile.MapInfo.BaseRowColNumber.Y - 1) * hdrFile.MapInfo.XYResolution.Latitude;
            double minY = maxY - h;
            return new Core.DrawEngine.CoordEnvelope(minX, maxX, minY, maxY);
        }

        SmpTaskScheduler stc =null;

        private void InitStc()
        {
            stc.MoniDir = @"D:\\Moni";
            stc.FileAdded += new Action<string[]>(stc_FileAdded);
            stc.FileRemoved += new Action<string[]>(stc_FileRemoved);
            stc.MessageSend += new Action<string>(stc_MessageSend);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (stc == null)
            {
                stc = new SmpTaskScheduler();
                InitStc();
            }
            if (!stc.IsStarting)
            {
                stc.StartListen();
                btnMoni.Image = imageList1.Images["bullet-black.png"];
            }
            else
            {
                stc.StopListen();
                btnMoni.Image = imageList1.Images["bullet_green.png"];
            }
        }

        void stc_MessageSend(string obj)
        {
            this.Invoke(new Action(()=> 
            {
                this.richTextBox1.AppendText(obj + Environment.NewLine);
            }));
        }

        void stc_FileRemoved(string[] obj)
        {
            this.Invoke(new Action(() =>
            {
                foreach (string o in obj)
                {
                    this.richTextBox1.AppendText("Removed:" + o + Environment.NewLine);
                    RemoveOverView(o);
                }
            }));
        }

        void stc_FileAdded(string[] obj)
        {
            this.Invoke(new Action(() =>
            {
                foreach (string o in obj)
                {
                    this.richTextBox1.AppendText("Added:" + o + Environment.NewLine);
                    AddOverView(o);
                }
            }));
        }
    }
}
