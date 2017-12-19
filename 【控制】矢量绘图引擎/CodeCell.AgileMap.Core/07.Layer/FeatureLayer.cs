using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using System.Reflection;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public class FeatureLayer:IFeatureLayer,ILayerDrawable,ISupportOutsideIndicator,IPersistable,IIdentifyFeatures
    {
        protected string _name = null;
        protected ScaleRange _displayScaleRange = null;
        protected RotateFieldDef _rotateFieldDef = null;
        protected ILabelDef _labelDef = null;
        protected IFeatureRenderer _featureRender = null;
        protected IClass _class = null;
        protected bool _disposed = false;
        protected bool _visible = true;
        internal IFeatureRenderEnvironment _environment = null;
        private IOutsideIndicator _outsideIndicator = null;
        protected bool _enabledDisplayLevel = true;
        private bool _twoStepFlag = false;
        private bool _isInited = false;
        internal bool _isRendered = false;

        public FeatureLayer()
        {
            _displayScaleRange = new ScaleRange(-1,1);
            _outsideIndicator = new OutsideIndicator();
        }

        public FeatureLayer(string name)
            :this()
        {
            _name = name;
        }

        public FeatureLayer(string name, string filename)
            : this(name)
        {
            _class = new FeatureClass(filename);
            TryInitByFeatureClass();
        }

        public FeatureLayer(string name, IFeatureClass fetclass)
            :this(name)
        {
            _class = fetclass;
            _class.Name = name;
            TryInitByFeatureClass();
        }

        public FeatureLayer(string name, IFeatureClass fetclass, IFeatureRenderer render)
            :this(name,fetclass)
        {
            _featureRender = render;
            if((_featureRender as BaseFeatureRenderer) != null)
                (_featureRender as BaseFeatureRenderer)._name = name;
        }

        public FeatureLayer(string name, IFeatureClass fetclass, IFeatureRenderer render, ILabelDef labelDef)
            :this(name,fetclass,render)
        {
            _labelDef = labelDef;
        }

        private void TryInitByFeatureClass()
        {
            if (_class == null )
            {
                _featureRender = null;
                _labelDef = null;
                _rotateFieldDef = null;
            }
            else
            {
                if (!(_class.DataSource as IFeatureDataSource).IsReady)
                    (_class.DataSource as FeatureDataSourceBase).TryInit();
                if ((_class.DataSource as IFeatureDataSource).IsReady)
                {
                    (_class as FeatureClass).GetArgsFromDataSource();
                    DoInit();
                }
            }
        }

        private void DoInit()
        {
            IFeatureClass fetclass = _class as IFeatureClass;
            //_featureRenderer
            switch (fetclass.ShapeType)
            {
                case enumShapeType.Point:
                    _featureRender = new SimpleFeatureRenderer(new SimpleMarkerSymbol());
                    break;
                case enumShapeType.Polyline:
                    _featureRender = new SimpleFeatureRenderer(new SimpleLineSymbol());
                    break;
                case enumShapeType.Polygon:
                    _featureRender = new SimpleFeatureRenderer(new SimpleFillSymbol());
                    break;
                default:
                    throw new NotSupportedException("矢量要素层对象暂不支持\"" + fetclass.ShapeType.ToString() + "\"几何类型的数据。");
            }
            if ((_featureRender as BaseFeatureRenderer) != null)
                (_featureRender as BaseFeatureRenderer)._name = _name;
            string[] fldnames = fetclass.FieldNames;
            _labelDef = new LabelDef(TryGetKeyField(fldnames, new string[] { "NAME", "名称" }), fldnames);
            _rotateFieldDef = new RotateFieldDef(TryGetKeyField(fldnames, new string[] { "ANGLE", "角度" }), 0f, fldnames);
            _featureRender.RotateFieldDef = _rotateFieldDef;
            _isInited = true;
        }

        private string TryGetKeyField(string[] fieldNames, string[] keys)
        {
            if (fieldNames == null || fieldNames.Length == 0 || keys == null || keys.Length == 0)
                return null;
            foreach (string fld in fieldNames)
            {
                if (fld == null)
                    continue;
                foreach (string key in keys)
                    if (fld.ToUpper().Contains(key.ToUpper()))
                        return fld;
            }
            return null;
        }

        internal void InternalInit(IFeatureRenderEnvironment environment)
        {
            _environment = environment ;
            if (_featureRender != null)
                (_featureRender as BaseFeatureRenderer).SetFeatureLayerEnvironment(environment);
        }

        #region IFeatureLayer 成员

        [Browsable(false)]
        public string Id
        {
            get { return _class != null ? _class.ID.ToString() : string.Empty; }
        }

        [Browsable(false)]
        public bool IsRendered
        {
            get { return _isRendered; }
        }

        [DisplayName("名称")]
        public string Name
        {
            get { return _name == null ? ToString() : _name; }
            set { _name = value; }
        }

        [DisplayName("是否显示")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("启用稀疏化")]
        public bool EnabledDisplayLevel
        {
            get { return _enabledDisplayLevel; }
            set { _enabledDisplayLevel = value; }
        }

        [DisplayName("按比例显示"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ScaleRange DisplayScaleRange
        {
            get
            {
                if (_displayScaleRange == null)
                    _displayScaleRange = new ScaleRange(-1,1);
                return _displayScaleRange;
            }
            set 
            {
                if(value != null)
                    _displayScaleRange = value;
            }
        }

        public bool VisibleAtScale(int scale)
        {
            if(_displayScaleRange.Enable)
                return !(scale > _displayScaleRange.DisplayScaleOfMin || scale < _displayScaleRange.DisplayScaleOfMax);
            return true;
        }

        [DisplayName("旋转字段"),TypeConverter(typeof(ExpandableObjectConverter))]
        public RotateFieldDef RotateFieldDef
        {
            get { return _rotateFieldDef; }
            set { _rotateFieldDef = value; }
        }

        [DisplayName("标注设置"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ILabelDef LabelDef
        {
            get { return _labelDef; }
            set { _labelDef = value; }
        }

        [DisplayName("渲染符号")]
        [EditorAttribute(typeof(UIFeatureRendererTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ExpandableObjectConverter))]
        public IFeatureRenderer Renderer
        {
            get { return _featureRender; }
            set 
            {
                if (value == null)
                    return;
                if (value != null && !value.Equals(_featureRender))
                {
                    if (_featureRender != null)
                        _featureRender.Dispose();
                }
                _featureRender = value;
                if ((_featureRender as BaseFeatureRenderer) != null)
                    (_featureRender as BaseFeatureRenderer)._name = _name;
            }
        }

        [DisplayName("矢量要素类"),TypeConverter(typeof(ExpandableObjectConverter))]
        public IClass Class
        {
            get { return _class; }
            set
            {
                IFeatureClass fetclass = _class as IFeatureClass;
                if (_class != null)
                    fetclass.Dispose();
                _class = value;
            }
        }

        [DisplayName("两阶段标记")]
        public bool IsTwoStepDraw
        {
            get { return _twoStepFlag; }
            set { _twoStepFlag = value; }
        }

        [Browsable(false)]
        public bool Disposed
        {
            get { return _disposed; }
        }

        public string TipText(double mapX, double mapY, double Tolerance)
        {
            return string.Empty;
        }

        #endregion

        #region IFeatureLayerDrawable 成员

         [Browsable(false)]
        public bool IsReady
        {
            get
            {
                if (!_isInited)
                    TryInitByFeatureClass();
                return _isInited; 
            }
        }

        public void Render(Graphics g,QuickTransformArgs quickTransfrom)
        {
            RenderFeatures(g, quickTransfrom);
        }

        private void RenderFeatures(Graphics g, QuickTransformArgs quickTransfrom)
        {
            IFeatureClass fetclass = _class as IFeatureClass;
            IGrid[] grids = fetclass.Grids;
            if (grids == null || grids.Length == 0)
                return;
           try
            {
                fetclass.RepeatFeatureRecorder.BeginRender();
                int gridCount = grids.Length;
                Envelope currentExtent = _environment.ExtentOfProjectionCoord;
                Grid gd = null;
                IOutsideIndicator outsideIndicator = null;
                //
                for (int i = 0; i < gridCount; i++)
                {
                    gd = grids[i] as Grid;
                    if (gd.IsEmpty())
                        continue;
                    outsideIndicator = (gd as ISupportOutsideIndicator).OutsideIndicator;
                    //test,draw grids
                    //DrawTestGrid(gd, g, quickTransfrom);
                    //
                    /* 为了测试其他投影矢量绘制注释掉
                    outsideIndicator.SetOutside(!currentExtent.IsInteractived(gd.GridEnvelope,ref gd._isfullInternal));
                    if (outsideIndicator.IsOutside)
                        continue;
                     */
                    outsideIndicator.SetOutside(!currentExtent.IsInteractived(gd.GridEnvelope, ref gd._isfullInternal));
                    if (outsideIndicator.IsOutside)
                        continue;
                    _featureRender.Render(_enabledDisplayLevel, quickTransfrom, gd, g, currentExtent, fetclass.RepeatFeatureRecorder);
                }
            }
            finally 
            {
                fetclass.RepeatFeatureRecorder.EndRender();
            }
        }

        private void DrawTestGrid(IGrid gd, Graphics g,QuickTransformArgs quickTransform)
        {
            Envelope evp = gd.GridEnvelope.Clone() as Envelope;
            ShapePoint[] pts = evp.Points;
            PointF[] ptfs = new PointF[pts.Length];
            int ptIdx = 0;
            foreach (ShapePoint pt in pts)
            {
                pt.X = pt.X * quickTransform.kLon + quickTransform.bLon;
                pt.Y = pt.Y * quickTransform.kLat + quickTransform.bLat;
                ptfs[ptIdx].X = (float)pt.X;
                ptfs[ptIdx].Y = (float)pt.Y;
                ptIdx++;
            }
            g.DrawPolygon(Pens.Red, ptfs);
            g.DrawString(gd.GridNo.ToString(), new Font("宋体", 9), Brushes.Red, new PointF(ptfs[0].X + 6, ptfs[0].Y + 6));
        }

        #endregion

        #region IIdentifyFeatures 成员

        public Feature[] Identify(Shape geometry,double tolerance)
        {
            if (_class == null)
                return null;
            IFeatureClass fetclass = _class as IFeatureClass;
            return fetclass.Identify(geometry, tolerance);
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            try
            {
                IMapRuntime r = _environment as IMapRuntime;
                if (r != null && r.RuntimeExchanger != null)
                    r.RuntimeExchanger.RemoveLayer(this);
                if (_class != null)
                    _class.Dispose();
                if (_featureRender != null)
                    _featureRender.Dispose();
            }
            finally 
            {
                _disposed = true;
            }
        }

        #endregion

        #region ISupportOutsideIndicator Members

        [Browsable(false)]
        public IOutsideIndicator OutsideIndicator
        {
            get { return _outsideIndicator; }
        }

        #endregion

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Layer");
            obj.AddAttribute("name", _name != null ?_name :string.Empty);
            obj.AddAttribute("visible", _visible.ToString());
            obj.AddAttribute("enabledisplayLevel", _enabledDisplayLevel.ToString());
            obj.AddAttribute("twostepflag", _twoStepFlag.ToString());
            //displayScaleRange
            if (_displayScaleRange == null)
                _displayScaleRange = new ScaleRange(-1, 1);
            obj.AddSubNode((_displayScaleRange as IPersistable).ToPersistObject());
            //label def
            if (_labelDef != null)
                obj.AddSubNode((_labelDef as IPersistable).ToPersistObject());
            //renderer
            if (_featureRender != null)
                obj.AddSubNode((_featureRender as IPersistable).ToPersistObject());
            //rotate field
            if (_rotateFieldDef != null)
                obj.AddSubNode((_rotateFieldDef as IPersistable).ToPersistObject());
            //feature class
            if (_class != null)
                obj.AddSubNode((_class as IPersistable).ToPersistObject());
            return obj;
        }

        #endregion

        public static IFeatureLayer FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = ele.Attribute("name").Value;
            bool visible = bool.Parse(ele.Attribute("visible").Value);
            bool enableDisplayLevel = true ;
            if (ele.Attribute("enabledisplayLevel") != null)
                enableDisplayLevel = bool.Parse(ele.Attribute("enabledisplayLevel").Value);
            ScaleRange displayScaleRange = ScaleRange.FromXElement(ele.Element("DisplayScaleRange"));
            LabelDef labelDef = AgileMap.Core.LabelDef.FromXElement(ele.Element("LabelDef"));
            IFeatureRenderer renderer = PersistObject.ReflectObjFromXElement(ele.Element("Renderer")) as IFeatureRenderer;
            RotateFieldDef rotateFieldDef = RotateFieldDef.FromXElement(ele.Element("RotateField"));
            IFeatureClass fetclass = AgileMap.Core.FeatureClass.FromXElement(ele.Element("FeatureClass"));
            if (fetclass == null)
                return null;
            fetclass.Name = name;
            IFeatureLayer lyr = new FeatureLayer(name, fetclass, renderer, labelDef);
            lyr.RotateFieldDef = rotateFieldDef;
            lyr.DisplayScaleRange = displayScaleRange;
            lyr.EnabledDisplayLevel = enableDisplayLevel;
            (lyr as ILayerDrawable).Visible = visible;
            //
            if (ele.Attribute("twostepflag") != null)
            {
                string twoStepflagstring = ele.Attribute("twostepflag").Value;
                lyr.IsTwoStepDraw = bool.Parse(twoStepflagstring);
            }
            //
            return lyr;
        }
    }
}
