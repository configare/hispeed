using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing.Imaging;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout.DataFrm;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Xml.Linq;
using GeoDo.RSS.Core.VectorDrawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace test
{
    public partial class Form1 : Form
    {
        ILayoutHost _host;

        public Form1()
        {
            InitializeComponent();
            Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            _host = new LayoutHost(layoutControl1);
            _txt = new TextBox();
            _txt.BorderStyle = BorderStyle.None;
            _txt.Location = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            _txt.Visible = false;
            layoutControl1.Controls.Add(_txt);
            _txt.BringToFront();
            _txt.KeyDown += new KeyEventHandler(txt_KeyDown);
            //_host.LayoutRuntime.Scale = 0.7f;
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            _host.LayoutRuntime.Scale = (float)numericUpDown1.Value / 100f;
            _host.Render();
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnScale_Click(null, null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _host.ToSuitedSize();
            numericUpDown1.Value = (int)(_host.LayoutRuntime.Scale * 100);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _host.LayoutRuntime.Layout.Size = new SizeF(300, 300);
            _host.Render();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {

                dlg.Filter = "所有图像(*.bmp,*.jpg,*.jpeg,*.png,*.tiff,*.tif)|*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.png";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ImageFormat imgFormat = ImageFormat.Bmp;
                    string extName = Path.GetExtension(dlg.FileName.ToUpper());
                    switch (extName)
                    {
                        case ".BMP":
                            imgFormat = ImageFormat.Bmp;
                            break;
                        case ".JPEG":
                        case ".JPG":
                            imgFormat = ImageFormat.Jpeg;
                            break;
                        case ".TIF":
                        case ".TIFF":
                            imgFormat = ImageFormat.Tiff;
                            break;
                        case ".PNG":
                            imgFormat = ImageFormat.Png;
                            break;
                    }
                    using (Bitmap bmp = _host.ExportToBitmap(PixelFormat.Format32bppArgb))
                    {
                        bmp.Save(dlg.FileName, imgFormat);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _host.CurrentTool = new DefaultZoomTool();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _host.CurrentTool = new ArrowTool();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "JPEG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bp = Bitmap.FromFile(dlg.FileName) as Bitmap;
                    PictureElement pic = new PictureElement(bp);
                    //pic.Size = new System.Drawing.SizeF(_host.LayoutRuntime.Layout.Size.Width - 3,
                    //                                    _host.LayoutRuntime.Layout.Size.Height - 3);
                    //   pic.Angle = 45;
                    _host.LayoutRuntime.Layout.Elements.Add(pic);
                    _host.Render();
                }
            }
        }

        private string _text;
        TextBox _txt;
        private void AddText_Click(object sender, EventArgs e)
        {
            TextElement text = new TextElement("减肥");
            //text.Angle = -45;
            text.Location = new PointF(40, 40);
            text.DisplayMaskColor = false;
            text.MaskColor = Color.Yellow;
            //_host.LayoutRuntime.Layout.Elements.Add(text);

            float angle = GetAngleText();

            TextElement lean = new TextElement("减肥");
            lean.Location = new PointF(40, 40);
            lean.Color = Color.Red;
            lean.LeanAngle = angle == 0 ? 45 : angle;
            _host.LayoutRuntime.Layout.Elements.Add(lean);

            _host.Render();
            //_txt.Text = null;
            //_txt.Visible = true;
            //_txt.Capture = true;
        }

        private float GetAngleText()
        {
            string txt = txtLeanAngle.Text;
            if (string.IsNullOrEmpty(txt))
                return 0;
            float angle = 0;
            bool ok = float.TryParse(txt, out angle);
            if (ok)
                return angle;
            else
                return 0;
        }

        void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (_txt == null)
                return;
            if (String.IsNullOrEmpty(_txt.Text))
                return;
            _text = _txt.Text;
            _txt.Visible = false;
            TextElement text = new TextElement(_text);
            text.Location = _txt.Location;
            float x = _txt.Location.X;
            float y = _txt.Location.Y;
            text.Location = new PointF(x, y);
            text.Size = new SizeF(80, 20);
            _host.LayoutRuntime.Layout.Elements.Add(text);
            // text.OriginSize = text.Size;
            //text.IsLocked = true;
            _host.Render();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                WidthChanged();
            }
        }

        private void WidthChanged()
        {
            if (textBox1.Text != null && textBox1.Text.Length != 0)
            {
                int width = 0;
                bool ok = int.TryParse(textBox1.Text, out width);
            }
        }

        private void group_Click(object sender, EventArgs e)
        {
            _host.Group();
            _host.Render(true);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Left);
            _host.Render();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Top);
            _host.Render();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Right);
            _host.Render();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Bottom);
            _host.Render();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.LeftRightMid);
            _host.Render();
        }

        private void btnUngroup_Click(object sender, EventArgs e)
        {
            _host.Ungroup();
            _host.Render();
        }

        //保存模板
        private void button13_Click(object sender, EventArgs e)
        {
            string fname = "e:\\TemplateFile.gxt";
            // LayoutToFile.SaveToFile(fname, _host.LayoutRuntime.Layout);
            ILayoutTemplate template = new LayoutTemplate(_host); // 构造当前的模板对象
            template.SaveTo(fname);
        }

        //应用模板
        private void FromXml_Click(object sender, EventArgs e)
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate\\自定义\\热岛效应强度分布图.gxt";
            //fname = @"E:\气象局项目\MAS二期\【控制】代码工程\【控制】UI框架\SMART\bin\Release\LayoutTemplate\通用专题图\陆表温度专题图模板.gxt";
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "GXT(*.gxt)|*.gxt";
                if (dlg.ShowDialog() == DialogResult.OK)
                    fname = dlg.FileName;
            }
            ILayoutTemplate template = new LayoutTemplate(fname);

            _host.ApplyTemplate(template);
            _host.Render();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object x = Activator.CreateInstance(typeof(enumAttType));
            // x = enumAttType.FontType;
            if (typeof(enumAttType).IsEnum)
            {
            }
            else if (typeof(enumAttType).IsValueType)
            {

            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            // Bitmap bm = _host.SaveToBitmap();
            ILayoutTemplate template = new LayoutTemplate(_host);
            Bitmap bm = template.GetOverview(new System.Drawing.Size(165, 165));
            bm.Save("e:\\ov.bmp", ImageFormat.Bmp);
        }

        private void 指北针_Click(object sender, EventArgs e)
        {
            PointF point = new PointF(1, 2);
            NorthArrowElement northArrow = new NorthArrowElement(point);
            _host.LayoutRuntime.Layout.Elements.Add(northArrow);
            _host.Render();
        }

        private void 箭头_Click(object sender, EventArgs e)
        {
            PointF point = new PointF(0, 0);
            SizeF size = new SizeF(70, 120);
            ArrowElement arrow = new ArrowElement(point);
            arrow.Color = Color.Yellow;
            arrow.Size = size;
            _host.LayoutRuntime.Layout.Elements.Add(arrow);

            //ArrowLineElement line = new ArrowLineElement(point);
            //_host.LayoutRuntime.Layout.Elements.Add(line);
            _host.Render();
        }

        private void 比例尺条_Click(object sender, EventArgs e)
        {

            //if (_host.ActiveDataFrame == null)
            //    return;
            //float s = _host.ActiveDataFrame.LayoutScale;
            //Text = "1:" + Math.Round(s).ToString();
            ScaleBarElement scaleBar = new ScaleBarElement();
            scaleBar.Location = new PointF(200, 100);
            //if (s != 0)
            //scaleBar.Scale = s;
            //  scaleBar.Parts = new int[] { 1, 1, 5 };
            _host.LayoutRuntime.Layout.Elements.Add(scaleBar);
            _host.Render();
        }

        private void 上下居中_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.TopBottomMid);
            _host.Render();
        }

        private void 横向分布_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Horizontal);
            _host.Render();
        }

        private void 纵向分布_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.Vertical);
            _host.Render();
        }

        private void 背景颜色_Click(object sender, EventArgs e)
        {
            IBorder border = _host.LayoutRuntime.Layout.GetBorder();
            border.BackColor = Color.Red;
            _host.Render();
        }

        private void 图标_Click(object sender, EventArgs e)
        {
            CityElement city = new CityElement();
            PointF point = new PointF(400, 300);
            SizeF size = new SizeF(25, 50);
            city.Location = point;
            _host.LayoutRuntime.Layout.Elements.Add(city);
            _host.Render();
        }

        private void 应用模版_Click(object sender, EventArgs e)
        {
            _host.ApplyArguments("template:城市热岛专题图模板.gxt");
            _host.Render();
        }

        private void 应用图片_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "JPEG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bp = Bitmap.FromFile(dlg.FileName) as Bitmap;
                    _host.ApplyArguments(bp);
                    _host.Render();
                }
            }
        }

        private void 应用尺寸_Click(object sender, EventArgs e)
        {
            Size size = new Size(400, 500);
            _host.ApplyArguments(size);
            _host.Render();
        }

        private void 区界标注文本颜色_Click(object sender, EventArgs e)
        {
            TextElementQBG5 text = new TextElementQBG5();
            text.Location = new PointF(200, 200);
            _host.LayoutRuntime.Layout.Elements.Add(text);

            PictureElementSouthChinaSea557 ele = new PictureElementSouthChinaSea557();
            ele.Location = new PointF(300, 300);
            _host.LayoutRuntime.Layout.Elements.Add(ele);
            _host.Render();
        }

        private void 保存模版_Click(object sender, EventArgs e)
        {
            ILayoutTemplate temp = _host.Template;
            if (!String.IsNullOrEmpty(temp.FullPath))
                temp.SaveTo(temp.FullPath);
        }

        private void 应用变量_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("减肥", "区界");
            _host.ApplyArguments(dic);
            _host.Render();
        }

        private void angle_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            TextBox box = sender as TextBox;
            if (String.IsNullOrEmpty(box.Text))
                return;
            float angle;
            bool ok = float.TryParse(box.Text, out angle);
            if (!ok)
                return;
            if (_host.LayoutRuntime == null)
                return;
            Rotate(_host.LayoutRuntime, angle);
            _host.Render();
        }

        private void Rotate(ILayoutRuntime runtime, float angle)
        {
            if (runtime == null)
                return;
            IElement[] eles = runtime.Selection;
            if (eles == null || eles.Length == 0)
                return;
            foreach (IElement ele in eles)
            {
                if (!(ele is SizableElement))
                    continue;
                (ele as SizableElement).Angle = angle;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DataFrame df = new DataFrame(_host);
            df.Location = new PointF(2, 3);
            //  df.Size = new SizeF(500, 500);
            //df.BorderColor = Color.Red;
            //df.BorderWidth = 10;
            _host.LayoutRuntime.Layout.Elements.Add(df);
            _host.Render();
            _host.ActiveDataFrame = df;
            df.IsLocked = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            IDataFrame df = _host.ActiveDataFrame;
            if (df == null)
                return;
            string fname = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fname = dlg.FileName;
                else
                    return;
            }
            ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
            IRasterDrawing drawing = new RasterDrawing(fname, canvas, null);
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            IRasterLayer lyr = new RasterLayer(drawing);
            canvas.LayerContainer.Layers.Add(lyr);
            canvas.PrimaryDrawObject = drawing;
            canvas.CurrentEnvelope = drawing.OriginalEnvelope;
            //  AddVectorHost(canvas);
            drawing.StartLoading(null);
            canvas.Refresh(enumRefreshType.All);

            //
            fname = @"F:\产品与项目\MAS-II\源代码0618-night\【控制】UI框架\SMART\bin\Release\数据引用\基础矢量\矢量地图\world.mcd";
            IVectorHostLayer vHost = new VectorHostLayer(null);
            vHost.IsEnableDummyRender = false;
            canvas.LayerContainer.Layers.Add(vHost as ILayer);
            canvas.Refresh(enumRefreshType.All);
            //
            _host.Render();
        }

        private int[] GetDefaultBands(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            return prd.GetDefaultBands() == null ? new int[] { 1, 2, 3 } : prd.GetDefaultBands();
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            _host.SetActiveDataFrame2CurrentTool();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (_host.ActiveDataFrame == null)
                return;
            float s = _host.ActiveDataFrame.LayoutScale;
            Text = "1:" + Math.Round(s).ToString();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (_host.ActiveDataFrame == null)
                return;
            float s = _host.ActiveDataFrame.LayoutScale;
            _host.ActiveDataFrame.LayoutScale = s;
            _host.Render();
        }

        private void 填充_Click(object sender, EventArgs e)
        {
            _host.Aligment(enumElementAligment.HorizontalStrech);
            // _host.Aligment(enumElementAligment.VerticalStrech);
            _host.Render();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            GxdDocumentParser p = new GxdDocumentParser();
            string fname = "f:\\gxd.xml";
            IGxdDocument doc = p.Parse(fname);
            GxdRasterItem it = new GxdRasterItem("f:\\1.bmp", new PointF());
            XElement ele = it.ToXml();
            ele = doc.GxdTemplateHost.ToXml();
            doc.SaveAs("f:\\gxd2.xml");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Smart Layout Document(*.gxd)|*.gxd";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _host.SaveAsDocument(dlg.FileName);
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Smart Layout Document(*.gxd)|*.gxd";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IGxdDocument doc = GxdDocument.LoadFrom(dlg.FileName);
                    _host.ApplyGxdDocument(doc);
                    _host.Render();
                }
            }
        }

        private void 锁定_Click(object sender, EventArgs e)
        {
            ILayoutRuntime runtime = _host.LayoutRuntime;
            if (runtime.Selection == null || runtime.Selection.Length == 0)
                return;
            IElement[] elements = runtime.Selection;
            foreach (IElement element in elements)
                element.IsLocked = true;
            _host.Render();
        }

        private void 解锁_Click(object sender, EventArgs e)
        {
            ILayoutRuntime runtime = _host.LayoutRuntime;
            if (runtime.Selection == null || runtime.Selection.Length == 0)
                return;
            IElement[] elements = runtime.Selection;
            foreach (IElement element in elements)
                element.IsLocked = false;
            _host.Render();
        }

        private void AVILayer_Click(object sender, EventArgs e)
        {
            IDataFrame df = _host.ActiveDataFrame;
            if (df == null)
                return;
            ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
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
            //IDrawingsCreator creat = new DrawingsCreator();
            if (canvas == null)
                return;
            //IRasterDrawing[] drawings = creat.DrawingsMaker(fnames, canvas, null, null);
            //if (drawings == null || drawings.Length == 0)
            //    return;
            //for (int i = 0; i < drawings.Length; i++)
            //    drawings[i].SelectedBandNos = new int[] { 1, 2, 3 };
            //IAVILayer aviLyr = new AVILayer(drawings, 500);
            //aviLyr.OnTicked += new EventHandler(RefreshDataFrame);
            //aviLyr.IsRunning = true;
            //canvas.LayerContainer.Layers.Add(aviLyr);
            //canvas.PrimaryDrawObject = drawings[0];
            //canvas.CurrentEnvelope = drawings[0].OriginalEnvelope;
            //for (int i = 0; i < drawings.Length; i++)
            //{
            //    drawings[i].StartLoading((t, p) => { Text = p.ToString() + "/" + t.ToString(); });
            //}
            //canvas.Refresh(enumRefreshType.All);
            //_host.Render();
        }

        private void RefreshDataFrame(object sender, EventArgs e)
        {
            _host.Render();
        }

        //private IAVIHelper _avi = null;
        private void 导出avi_Click(object sender, EventArgs e)
        {
            //_avi = new AVIHelper(_host);
            //_avi.OnTimerStopped += new EventHandler(GetBitmaps);
        }

        //private void GetBitmaps(object sender, EventArgs e)
        //{
        //gline: Bitmap[] bmps = _avi.Bitmaps;
        //    if (bmps == null)
        //    {
        //        _avi.OnTimerStopped -= new EventHandler(GetBitmaps);
        //        goto gline;
        //    }
        //    for (int i = 0; i < bmps.Length; i++)
        //        bmps[i].Save("e:\\aviBms." + i + ".bmp", ImageFormat.Bmp);
        //    _avi.Dispose();
        //}

        private void 图例_Click(object sender, EventArgs e)
        {
            ILegendElement lv = new LegendELementH();
            lv.Location = new PointF(30, 40);
            _host.LayoutRuntime.Layout.Elements.Add(lv);

            TextElement te = new TextElement("图例", lv.LegendTextFont);
            _host.LayoutRuntime.Layout.Elements.Add(te);

            //ILegendElement linear = new LinearLegendElement();
            //linear.Text = "";
            //linear.Location = new PointF(300, 400);
            //linear.LegendTextSpan = 20;
            //_host.LayoutRuntime.Layout.Elements.Add(linear);
            _host.Render();
        }

        private void 图例2_Click(object sender, EventArgs e)
        {
            ILegendElement lv = new LegendELementH();
            lv.Text = "";
            lv.Location = new PointF(200, 40);
            //lv.LegendTextSpan = 20;
            //_host.LayoutRuntime.Layout.Elements.Add(lv);

            ILegendElement lvv = new LegendElementV();
            lvv.Location = new PointF(30, 40);
            lvv.Text = "";
            (lvv as LegendElementV).LegendItemSpan = 0;
            //lvv.LegendItems = new LegendItem[] { new LegendItem("沙尘",Color.Yellow)};
            _host.LayoutRuntime.Layout.Elements.Add(lvv);
            _host.Render();

            string text = "{Satellite}/{Sensor}监测图";
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("{Satellite}", "FY3A");
            vars.Add("{Sensor}", "VIRR");
            foreach (string key in vars.Keys)
            {
                if (text.Contains(key))
                    text = text.Replace(key, vars[key]);
            }
        }

        IDrawingElement _ele = new SimpleDrawingElement();
        private void button22_Click(object sender, EventArgs e)
        {
            _ele = new FeatureDrawingElement();
            _ele.RefLayerName = "中国行政区";
            _ele.SetDataFrame(_host.ActiveDataFrame);
            _ele.Location = new PointF(3, 2);
            _host.LayoutRuntime.Layout.Elements.Add(_ele as IElement);
            _host.Render();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            using (Form frm = new Form())
            {
                PropertyGrid grid = new PropertyGrid();
                grid.Dock = DockStyle.Fill;
                grid.SelectedObject = _ele;
                frm.Controls.Add(grid);
                frm.ShowDialog();
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load("f:\\地图文档.gxd");
            UpdateXElement(doc.Root);
            doc.Save("f:\\地图文档.gxd");
        }

        private void UpdateXElement(XElement xElement)
        {
            if (xElement.Name == "DataSource")
            {
                xElement.SetAttributeValue("fileurl", CorrectFilePath(xElement.Attribute("fileurl").Value));
            }
            var eles = xElement.Elements();
            if (eles != null)
                foreach (XElement ele in eles)
                    UpdateXElement(ele);
        }

        private string CorrectFilePath(string fileUrl)
        {
            throw new NotImplementedException();
        }

        private void 线图例_Click(object sender, EventArgs e)
        {
            ISingleDrawingElement lineLegend = new LineDrawingElement();
            lineLegend.Location = new PointF(100, 100);
            //_host.LayoutRuntime.Layout.Elements.Add(lineLegend);

            ISingleDrawingElement rect = new RectDrawingElement();
            rect.Location = new PointF(300, 300);

            //rect.Color = Color.Red;
            _host.LayoutRuntime.Layout.Elements.Add(rect);
            _host.Render();
        }

        private void txtLeanAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            float angle = GetAngleText();
            List<IElement> eles = _host.LayoutRuntime.Layout.Elements;
            if (eles == null || eles.Count == 0)
                return;
            TextElement textEle = null;
            foreach (IElement ele in eles)
            {
                if (ele is TextElement)
                {
                    textEle = ele as TextElement;
                    break;
                }
            }
            if (textEle == null)
                return;
            textEle.LeanAngle = angle;
            _host.Render();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AddVectorLayer(dlg.FileName);
                }
            }
        }

        private void AddVectorLayer(string fname)
        {
            IDataFrame df = _host.ActiveDataFrame;
            if (df == null)
                return;
            ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
            IVectorHostLayer vectorHostLayer = canvas.LayerContainer.VectorHost;
            vectorHostLayer.AddData(fname,null);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "等值线(*.xml)|*.xml";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IContourLayer lyr = ContourLayer.FromXml(dlg.FileName, true);
                    if (lyr != null)
                    {
                        IDataFrame df = _host.ActiveDataFrame;
                        if (df == null)
                            return;
                        ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
                        canvas.LayerContainer.Layers.Add(lyr);
                        canvas.Refresh(enumRefreshType.All);
                    }
                }
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            using (frmCustomTemplate frm = new frmCustomTemplate())
            {
                frm.ShowDialog();
            }
        }
    }
}
