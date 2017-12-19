using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class LayoutHost : ILayoutHost, IScaleRulerHelper, IElementsEditOperator
    {
        public static Action<string, ILayoutHost, IDataFrame, XElement> LoadDataFrameExecutor;
        public static Func<XElement, ILayer> LoadGeoGridLayerExecutor;

        private LayoutControl _containter;
        private float _dpi;
        private EventHandler _sizeChangedHandler;
        private ILayoutRuntime _layoutRuntime;
        internal ILayout _layout;
        private IDrawArgs _drawArgs;
        internal ILayoutTemplate _template;
        //为了美观,在缩放到适合窗口大小时上下左右各留出8个像素
        private const int MARGIN = 16;
        //缺省专题图大小
        private SizeF DEFAULT_SIZE = new SizeF(800, 600);
        private IControlTool _currentTool;
        private CanvasEventArgs _eventArgs = new CanvasEventArgs();
        private ISelectedEditBoxManager _selectedEditBoxManager;
        /// <summary>
        /// 标记拖拽事件接收的数据
        /// </summary>
        public const string cstDragDropDataFormat = "CreatElement";
        //当元素被拖入
        private EventHandler _onElementIsDragDroped;

        public LayoutHost(LayoutControl container)
        {
            _containter = container;
            CreateSelectedEditBoxManager();
            SetScaleRulerHelper();
            SetDPI();
            AttachEvents();
            CreateLayoutRuntime();
            _drawArgs = new DrawArgs(_layoutRuntime);
        }

        private void CreateSelectedEditBoxManager()
        {
            _selectedEditBoxManager = new SelectedEditBoxManager();
        }

        public EventHandler OnElementIsDragDroped
        {
            get { return _onElementIsDragDroped; }
            set { _onElementIsDragDroped = value; }
        }

        public Control Container
        {
            get { return _containter; }
        }

        public ILayoutTemplate Template
        {
            get { return _template; }
        }

        public ISelectedEditBoxManager SelectedEditBoxManager
        {
            get { return _selectedEditBoxManager; }
        }

        public IControlTool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
                if (_currentTool != null)
                    _containter.Cursor = _currentTool.Cursor;
                else
                    _containter.Cursor = Cursors.Default;
            }
        }

        private void SetScaleRulerHelper()
        {
            if (_containter is LayoutControl)
                (_containter as LayoutControl).SetScaleRulerHelper(this);
        }

        private void CreateLayoutRuntime()
        {
            ILayout layout = LoadLayout();
            _layoutRuntime = new LayoutRuntime(layout, this);
        }

        private ILayout LoadLayout()
        {
            Border b = new Border(DEFAULT_SIZE);
            _layout = new Layout(enumLayoutUnit.Pixel);
            _layout.Elements.Add(b);
            return _layout;
        }

        private void AttachEvents()
        {
            _containter.ActualContainer.AllowDrop = true;
            _containter.ActualContainer.SizeChanged += new EventHandler(_containter_SizeChanged);
            _containter.ActualContainer.Paint += new PaintEventHandler(_containter_Paint);
            _containter.ActualContainer.MouseDown += new MouseEventHandler(ActualContainer_MouseDown);
            _containter.ActualContainer.MouseMove += new MouseEventHandler(ActualContainer_MouseMove);
            _containter.ActualContainer.MouseUp += new MouseEventHandler(ActualContainer_MouseUp);
            _containter.ActualContainer.MouseWheel += new MouseEventHandler(ActualContainer_MouseWheel);
            _containter.ActualContainer.PreviewKeyDown += new PreviewKeyDownEventHandler(ActualContainer_PreviewKeyDown);
            _containter.ActualContainer.KeyUp += new KeyEventHandler(ActualContainer_KeyUp);
            _containter.ActualContainer.MouseDoubleClick += new MouseEventHandler(ActualContainer_MouseDoubleClick);
            _containter.ActualContainer.DragDrop += new DragEventHandler(ActualContainer_DragDrop);
            _containter.ActualContainer.DragEnter += new DragEventHandler(ActualContainer_DragEnter);
        }

        private void RemoveEvents()
        {
            if (_containter != null)
            {
                _containter.ActualContainer.SizeChanged -= new EventHandler(_containter_SizeChanged);
                _containter.ActualContainer.Paint -= new PaintEventHandler(_containter_Paint);
                _containter.ActualContainer.MouseDown -= new MouseEventHandler(ActualContainer_MouseDown);
                _containter.ActualContainer.MouseMove -= new MouseEventHandler(ActualContainer_MouseMove);
                _containter.ActualContainer.MouseUp -= new MouseEventHandler(ActualContainer_MouseUp);
                _containter.ActualContainer.MouseWheel -= new MouseEventHandler(ActualContainer_MouseWheel);
                _containter.ActualContainer.PreviewKeyDown -= new PreviewKeyDownEventHandler(ActualContainer_PreviewKeyDown);
                _containter.ActualContainer.KeyUp -= new KeyEventHandler(ActualContainer_KeyUp);
                _containter.ActualContainer.MouseDoubleClick -= new MouseEventHandler(ActualContainer_MouseDoubleClick);
                _containter.ActualContainer.DragDrop -= new DragEventHandler(ActualContainer_DragDrop);
                _containter.ActualContainer.DragEnter -= new DragEventHandler(ActualContainer_DragEnter);
            }
        }

        void ActualContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                FillCoords(e);
                _eventArgs.WheelDelta = e.Delta;
                _currentTool.Event(this, enumCanvasEventType.DoubleClick, _eventArgs);
            }
        }

        void ActualContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void FillCoords(MouseEventArgs e)
        {
            _eventArgs.ScreenX = e.X;
            _eventArgs.ScreenY = e.Y;
            _eventArgs.LayoutX = e.X;
            _eventArgs.LayoutY = e.Y;
            _eventArgs.E = e;
            _layoutRuntime.Screen2Layout(ref _eventArgs.LayoutX, ref _eventArgs.LayoutY);
        }

        void ActualContainer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                FillCoords(e);
                _eventArgs.WheelDelta = e.Delta;
                _currentTool.Event(this, enumCanvasEventType.MouseWheel, _eventArgs);
            }
        }

        void ActualContainer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (_currentTool != null)
            {
                _eventArgs.E = e;
                _currentTool.Event(this, enumCanvasEventType.KeyDown, _eventArgs);
            }
        }

        void ActualContainer_KeyUp(object sender, KeyEventArgs e)
        {
            if (_currentTool != null)
            {
                _eventArgs.E = null;
            }
        }

        void ActualContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                FillCoords(e);
                _currentTool.Event(this, enumCanvasEventType.MouseUp, _eventArgs);
            }
        }

        void ActualContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                FillCoords(e);
                _currentTool.Event(this, enumCanvasEventType.MouseMove, _eventArgs);
            }
        }

        void ActualContainer_MouseDown(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                FillCoords(e);
                _currentTool.Event(this, enumCanvasEventType.MouseDown, _eventArgs);
            }
        }

        void _containter_Paint(object sender, PaintEventArgs e)
        {
            _drawArgs.Reset(e.Graphics);
            DrawLayoutRuntime();
            DrawSelectedEditBoxes(e);
            DrawCurrentControlTool();
        }

        private void DrawLayoutRuntime()
        {
            _layoutRuntime.Render(_drawArgs);
        }

        private void DrawCurrentControlTool()
        {
            if (_currentTool != null)
                _currentTool.Render(this, _drawArgs);
        }

        private void DrawSelectedEditBoxes(PaintEventArgs e)
        {
            _selectedEditBoxManager.Render(this, _drawArgs);
        }

        public Bitmap ExportToBitmap(PixelFormat pixelFormat, Size size)
        {
            using (Bitmap bm = ExportToBitmap(pixelFormat))
            {
                Bitmap retBmp = new Bitmap(size.Width, size.Height);
                float scale = Math.Min(size.Width / (float)bm.Width, size.Height / (float)bm.Height);
                using (Graphics g = Graphics.FromImage(retBmp))
                {
                    g.Clear(Color.Transparent);
                    float offsetX = (retBmp.Width - bm.Width * scale) / 2f;
                    float offsetY = (retBmp.Height - bm.Height * scale) / 2f;
                    using (Matrix m = new Matrix())
                    {
                        m.Scale(scale, scale);
                        m.Translate(offsetX, offsetY,MatrixOrder.Append);
                        g.Transform = m;
                        g.DrawImage(bm, 0, 0);
                    }
                }
                return retBmp;
            }
        }

        public Bitmap ExportToBitmap(PixelFormat pixelFormat)
        {
            DrawArgs.IsExporting = true;
            try
            {
                float oldScale = _layoutRuntime.Scale;
                SizeF size = _layout.Size;
                Bitmap bm = CreatBitmapByArg(size, pixelFormat);
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    float x = 0, y = 0;
                    _layoutRuntime.Scale = 1f;
                    _layoutRuntime.Layout2Screen(ref x, ref y);
                    Matrix matrix = new Matrix();
                    matrix.Translate(-(int)x, -(int)y);
                    g.Transform = matrix;
                    _drawArgs.Reset(g);
                    _layoutRuntime.Render(_drawArgs);
                }
                _layoutRuntime.Scale = oldScale;
                Render();
                return bm;
            }
            finally
            {
                DrawArgs.IsExporting = false;
            }
        }

        private Bitmap CreatBitmapByArg(SizeF size, PixelFormat pixelFormat)
        {
            float width = size.Width;
            float height = size.Height;
            _layoutRuntime.Layout2Pixel(ref width, ref height);
            return new Bitmap((int)width, (int)height, pixelFormat);
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <returns></returns>
        public Bitmap SaveToBitmap()
        {
            DrawArgs.IsExporting = true;
            try
            {
                SizeF size = _layout.Size;
                Bitmap bm = new Bitmap((int)(size.Width * _layoutRuntime.Scale),
                    (int)(size.Height * _layoutRuntime.Scale), PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.White);
                    float x = 0, y = 0;
                    _layoutRuntime.Layout2Screen(ref x, ref y);
                    Matrix matrix = new Matrix();
                    matrix.Translate(-x, -y);
                    g.Transform = matrix;
                    _drawArgs.Reset(g);
                    _layout = _layoutRuntime.Layout;
                    List<IElement> eles = _layout.Elements;
                    for (int i = 0; i < eles.Count; i++)
                    {
                        if (eles[i] is IDataFrame)
                        {
                            InitDataFrame(_template.FullPath, eles[i],true);
                        }
                    }
                    _layoutRuntime.Render(_drawArgs);
                }
                Render();
                return bm;
            }
            finally
            {
                DrawArgs.IsExporting = false;
            }
        }

        void _containter_SizeChanged(object sender, EventArgs e)
        {
            if (this.CanvasSizeChanged != null)
                this.CanvasSizeChanged(this, e);
            _containter.Invalidate();
            _containter.ActualContainer.Invalidate();
        }

        private void SetDPI()
        {
            using (Graphics g = _containter.CreateGraphics())
                _dpi = g.DpiX;
        }

        public ILayoutRuntime LayoutRuntime
        {
            get { return _layoutRuntime; }
        }

        public void Render()
        {
            _layoutRuntime.UpdateMatrix();
            _containter.Invalidate();
            _containter.ActualContainer.Invalidate();
        }

        public void Render(bool strongRefreshData)
        {
            try
            {
                DrawArgs.IsStrongRefreshData = true;
                _layoutRuntime.UpdateMatrix();
                _containter.Refresh();
                _containter.ActualContainer.Refresh();
            }
            finally 
            {
                DrawArgs.IsStrongRefreshData = false;
            }
        }

        public void ToSuitedSize()
        {
            Size wndSize = _containter.ActualContainer.Size;
            wndSize.Height -= MARGIN;
            wndSize.Width -= MARGIN;
            SizeF layoutSize = _layout.Size;
            float w = layoutSize.Width;
            float h = layoutSize.Height;
            _layoutRuntime.Layout2Pixel(ref w, ref h);
            float scaleX = wndSize.Width / w;
            float scaleY = wndSize.Height / h;
            _layoutRuntime.ResetOffets();
            _layoutRuntime.Scale = Math.Min(scaleX, scaleY);
            Render();
        }

        public void ToSuitedSize(ILayout layout)
        {
            Size wndSize = _containter.ActualContainer.Size;
            wndSize.Height -= MARGIN;
            wndSize.Width -= MARGIN;
            SizeF layoutSize = layout.Size;
            float w = layoutSize.Width;
            float h = layoutSize.Height;
            _layoutRuntime.Layout2Pixel(ref w, ref h);
            float scaleX = wndSize.Width / w;
            float scaleY = wndSize.Height / h;
            _layoutRuntime.ResetOffets();
            _layoutRuntime.Scale = Math.Min(scaleX, scaleY);
            Render();
        }

        public IDataFrame ActiveDataFrame
        {
            get { return GetActiveDataFrame(); }
            set
            {
                if (value == null || !(value is IDataFrame))
                    return;
            }
        }

        public Size CanvasSize
        {
            get { return _containter.ActualContainer.Size; }
        }

        public float DPI
        {
            get { return _dpi; }
        }

        public EventHandler CanvasSizeChanged
        {
            get { return _sizeChangedHandler; }
            set { _sizeChangedHandler = value; }
        }

        ILayoutRuntime IScaleRulerHelper.LayoutRuntime
        {
            get { return _layoutRuntime; }
        }

        ILayout IScaleRulerHelper.Layout
        {
            get { return _layout; }
        }

        public void ApplyGxdDocument(IGxdDocument doc)
        {
            if (doc == null)
                return;
            //
            Dictionary<string, ILayer> gridLayers = null;
            IGxdTemplateHost tempHost = doc.GxdTemplateHost;
            if (tempHost != null)
                gridLayers = ApplyTemplate(tempHost.LayoutTemplate, false);
            //
            List<IGxdDataFrame> dfs = doc.DataFrames;
            if (dfs != null && dfs.Count > 0)
            {
                foreach (IGxdDataFrame df in dfs)
                {
                    if (GxdDocument.GxdAddDataFrameExecutor != null)
                        GxdDocument.GxdAddDataFrameExecutor(doc.FullPath, df, this);
                    //将经纬网格添加到最上层
                    if (gridLayers != null && gridLayers.ContainsKey(df.Name))
                    {
                        IElement[] eles = _layout.QueryElements((e) => { return e is IDataFrame && (e as IDataFrame).Name == df.Name; });
                        if (eles != null && eles.Length > 0)
                            ((eles[0] as IDataFrame).Provider as IDataFrameDataProvider).Canvas.LayerContainer.Layers.Add(gridLayers[df.Name]);
                    }
                }
            }
        }

        public void ApplyTemplate(ILayoutTemplate template)
        {
            Dictionary<string, ILayer> gridLayers = ApplyTemplate(template, true);

            if (gridLayers != null && gridLayers.Count != 0)
            {
                IElement[] eles = _layout.QueryElements((e) => { return e is IDataFrame; });
                if (eles != null && eles.Length > 0)
                    ((eles[0] as IDataFrame).Provider as IDataFrameDataProvider).Canvas.LayerContainer.Layers.Add(gridLayers.Values.ToArray()[0]);
            }
        }

        public Dictionary<string,ILayer> ApplyTemplate(ILayoutTemplate template,bool isLoadGeoGridLayer)
        {
            if (template == null)
                return null;
            _template = template;
            _layout = template.Layout;
            if (_layout == null)
                return null;
            Dictionary<string, ILayer> gridLayers = new Dictionary<string, ILayer>();
            List<IElement> eles = _layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i] is IDataFrame)
                {
                    ILayer gridLyr = InitDataFrame(template.FullPath, eles[i],isLoadGeoGridLayer);
                    if (gridLyr != null)
                        gridLayers.Add((eles[i] as IDataFrame).Name, gridLyr);
                }
            }
            _layoutRuntime.ChangeLayout(_layout);
            ToSuitedSize();
            return gridLayers;
        }

        private ILayer InitDataFrame(string gxfile, IElement ele, bool isLoadGeoGridLayer)
        {
            IDataFrame df = ele as IDataFrame;
            df.BorderColor = (ele as IDataFrame).BorderColor;
            df.BorderWidth = (ele as IDataFrame).BorderWidth;
            df.Location = (ele as IDataFrame).Location;
            df.Size = (ele as IDataFrame).Size;
            df.Angle = (ele as IDataFrame).Angle;
            df.Update(this);
            IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;

            ILayer gridLayer = null;
            if (df.GeoGridXml != null)
            {
                gridLayer = LoadGeoGridLayerExecutor(df.GeoGridXml);
            }
            if (provider != null)
            {
                ICanvas c = provider.Canvas;
                if (c != null)
                {
                    if (c.CanvasSetting != null)
                    {
                        if (c.CanvasSetting.RenderSetting != null)
                            c.CanvasSetting.RenderSetting.BackColor = Color.White;
                    }
                    if (df.Data != null && LoadDataFrameExecutor != null)
                    {
                        LoadDataFrameExecutor(gxfile, this, df, df.Data as XElement);
                    }
                }
            }
            return gridLayer;
        }

        #region apply template and elements by arguments
        /// <summary>
        /// 应用传入的参数
        /// </summary>
        /// <param name="arguments">
        /// template:模版名称   => ApplyTemplate(...)
        /// bitmap                   => PictureElement
        /// Dictionary<string,object>//需要传进来的参数
        /// Size
        /// </param>
        public void ApplyArguments(params object[] arguments)
        {
            if (arguments == null)
                return;
            foreach (object arg in arguments)
            {
                if (arg is String)
                    ApplyTemplateByName(arg.ToString());
                if (arg is Bitmap)
                    AddBitmapToLayout(arg as Bitmap);
                if (arg is Size)
                    _layout.Size = (Size)arg;
                if (arg is Dictionary<string, string>)
                    ApplyVariableByArguments(arg);
            }
        }

        /// <summary>
        /// 应用通过参数传进来的模版
        /// </summary>
        /// <param name="arg"></param>
        private void ApplyTemplateByName(string arg)
        {
            if (String.IsNullOrEmpty(arg))
                return;
            if (!arg.Contains("template:"))
                return;
            string[] parts = arg.Split(':');
            ILayoutTemplate template = LayoutTemplate.FindTemplate(parts[1]);
            ApplyTemplate(template);
        }

        /// <summary>
        /// 应用通过参数传进来的位图
        /// </summary>
        /// <param name="bitmap"></param>
        private void AddBitmapToLayout(Bitmap bitmap)
        {
            if (bitmap == null)
                return;
            PictureElement picEle = new PictureElement(bitmap);
            AdjustElementSize(bitmap, ref picEle);
            float a = (_layout.Size.Width - picEle.Size.Width) / 2f;
            float b = (_layout.Size.Height - picEle.Size.Height) / 2f;
            picEle.Location = new PointF(a, b);
            _layout.Elements.Add(picEle);
        }

        /// <summary>
        /// 应用通过参数传进来的变量(文本)
        /// </summary>
        /// <param name="arg"></param>
        private void ApplyVariableByArguments(object arg)
        {
            if (arg == null)
                return;
            Dictionary<string, string> textArg = arg as Dictionary<string, string>;
            if (textArg == null)
                return;
            List<IElement> elements = _layout.Elements;
            if (elements == null || elements.Count == 0)
                return;
            TextElement text = null;
            foreach (IElement ele in elements)
            {
                if (!(ele is TextElement))
                    continue;
                text = ele as TextElement;
                if (String.IsNullOrEmpty(text.Text))
                    return;
                if (textArg.Keys.Contains(text.Text))
                    text.Text = textArg[text.Text];
            }
        }

        private void AdjustElementSize(Bitmap bitmap, ref PictureElement picEle)
        {
            float orginWid = bitmap.Size.Width;
            float orginHei = bitmap.Size.Height;
            if (orginWid == 0 || orginHei == 0)
                return;
            if (orginWid <= _layout.Size.Width && orginHei <= _layout.Size.Height)
            {
                picEle.Size = new SizeF(bitmap.Size.Width * 0.8f, bitmap.Size.Height * 0.8f);
                return;
            }
            float a = (float)_layout.Size.Width;
            float b = (float)_layout.Size.Height;
            float scaleX = (0.8f * a) / orginWid;
            float scaleY = (0.8f * b) / orginHei;
            float scale = Math.Min(scaleX, scaleY);
            picEle.Size = new SizeF(orginWid * scale, orginHei * scale);
        }
        #endregion

        public ILayoutTemplate ToTemplate()
        {
            return new LayoutTemplate(this);
        }

        public void SaveAsDocument(string gxdfilename)
        {
            IGxdDocument doc = GxdDocument.GenerateFrom(this);
            if (doc != null)
                doc.SaveAs(gxdfilename);
        }

        public void Group()
        {
            ElementsEditor.ElementsToGroup(_layoutRuntime.Selection, this);
            Render(true);
        }

        public void Ungroup()
        {
            ElementsEditor.ElementsUnGroup(_layoutRuntime.Selection, this);
            Render(true);
        }

        public void Aligment(enumElementAligment style)
        {
            switch (style)
            {
                case enumElementAligment.Left:
                    ElementsEditor.AligmentLeft(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.Right:
                    ElementsEditor.AligmentRight(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.Top:
                    ElementsEditor.AligmentTop(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.Bottom:
                    ElementsEditor.AligmentBottom(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.LeftRightMid:
                    ElementsEditor.AligmentLeftRightMid(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.TopBottomMid:
                    ElementsEditor.AligmentTopBottomMid(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.Vertical: //纵向分布
                    ElementsEditor.AligmentVertical(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.Horizontal: // 横向分布
                    ElementsEditor.AligmentHorizontal(_layoutRuntime.Selection, this);
                    return;
                case enumElementAligment.HorizontalStrech:
                    ElementsEditor.HorizontalStrech(this);
                    return;
                case enumElementAligment.VerticalStrech:
                    ElementsEditor.VerticalStrech(this);
                    return;
            }
        }

        public void SetActiveDataFrame2CurrentTool()
        {
            IDataFrame df = ActiveDataFrame;
            if (df != null)
                _currentTool = df as IControlTool;
        }

        private IDataFrame GetActiveDataFrame()
        {
            IElement[] eles = _layoutRuntime.QueryElements((ele) =>{return ele is IDataFrame;},false);
            if (eles == null)
                return null;
            else if (eles.Length == 1)
                return eles[0] as IDataFrame;
            else
                foreach (IElement ele in eles)
                {
                    if (ele.IsSelected)
                    {
                        return ele as IDataFrame;
                    }
                }
            return null;
        }

        #region  DragDrop
        /// <summary>
        /// 接收拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ActualContainer_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject obj = e.Data;
            if (obj == null)
                return;
            object elements = null;
            if (obj.GetDataPresent(cstDragDropDataFormat))
                elements = AcceptSelectedElement(obj.GetData(cstDragDropDataFormat), e);
            else if (obj.GetDataPresent(DataFormats.Bitmap))
                elements = AcceptBitmap(obj.GetData(DataFormats.Bitmap) as Bitmap, null, e);
            else if (obj.GetDataPresent(DataFormats.FileDrop))
                elements = AcceptFile(obj.GetData(DataFormats.FileDrop), e);
            else if (obj.GetDataPresent(DataFormats.Text))
                elements = AcceptText(obj.GetData(DataFormats.Text), e);
            else if (obj.GetDataPresent(DataFormats.UnicodeText))
                elements = AcceptUnicodeText(obj.GetData(DataFormats.UnicodeText), e);
            else
                return;
            if (_onElementIsDragDroped != null)
                _onElementIsDragDroped(elements, null);
        }

        private IElement AcceptSelectedElement(object obj, DragEventArgs e)
        {
            ISizableElement element = Activator.CreateInstance(obj as Type) as ISizableElement;
            if (element == null)
                return null;
            element.Location = GetMouseLocation(e);
            if (element is PictureElement)
                SetPictureElementSize(element as PictureElement);
            _layout.Elements.Add(element);
            Render();
            return element;
        }

        private IElement AcceptBitmap(Bitmap bm, string name, DragEventArgs e)
        {
            if (bm == null)
                return null;
            PictureElement pElement = new PictureElement(bm);
            if (name != null)
                pElement.Name = name;
            pElement.Location = GetMouseLocation(e);
            SetPictureElementSize(pElement);
            _layout.Elements.Add(pElement);
            Render();
            return pElement;
        }

        private void SetPictureElementSize(PictureElement pElement)
        {
            float w = pElement.Size.Width;
            float h = pElement.Size.Height;
            _layoutRuntime.Pixel2Layout(ref w, ref h);
            pElement.Size = new System.Drawing.SizeF(w, h);
        }

        private IElement[] AcceptFile(object obj, DragEventArgs e)
        {
            string[] fs = obj as string[];
            if (fs == null || fs.Length == 0)
                return null;
            Bitmap bm = null;
            string extension;
            List<IElement> retElements = new List<IElement>();
            foreach (string f in fs)
            {
                extension = Path.GetExtension(f).ToLower();
                if (extension != ".bmp" && extension != ".jpg" && extension != ".png"
                    && extension != ".gif")
                    continue;
                bm = Bitmap.FromFile(f) as Bitmap;
                if (bm != null)
                {
                    IElement ele = AcceptBitmap(bm, Path.GetFileName(f), e);
                    retElements.Add(ele);
                }
            }
            return retElements.Count > 0 ? retElements.ToArray() : null;
        }

        private IElement AcceptText(object obj, DragEventArgs e)
        {
            if (obj == null || obj.ToString() == string.Empty)
                return null;
            TextElement ele = new TextElement(obj.ToString());
            ele.Location = GetMouseLocation(e);
            _layout.Elements.Add(ele);
            Render();
            return ele;
        }

        private IElement AcceptUnicodeText(object obj, DragEventArgs e)
        {
            return AcceptText(obj, e);
        }

        private PointF GetMouseLocation(DragEventArgs e)
        {
            Point pt = _containter.ActualContainer.PointToClient(new Point(e.X, e.Y));
            float x = pt.X;
            float y = pt.Y;
            _layoutRuntime.Screen2Layout(ref x, ref y);
            return new PointF(x, y);
        }
        #endregion

        public void Dispose()
        {
            _sizeChangedHandler = null;
            _onElementIsDragDroped = null;
            RemoveEvents();
            if (_selectedEditBoxManager != null)
            {
                (_selectedEditBoxManager as SelectedEditBoxManager).Dispose();
                _selectedEditBoxManager = null;
            }
            if (_layout != null)
            {
                _layout.Dispose();
                _layout = null;
            }
            if (_template != null)
            {
                _template.Dispose();
                _template = null;
            }
            if (_drawArgs != null)
            {
                _drawArgs.Reset(null);
                _drawArgs = null;
            }
            if (_layoutRuntime != null)
            {
                _layoutRuntime.Dispose();
                _layoutRuntime = null;
            }
        }
    }
}
