using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using CodeCell.Bricks.Serial;


namespace CodeCell.AgileMap.Core
{
    public enum LabelDrawMode
    {
        SingleValue,
        Express
    }

    public enum masLabelPosition
    {
        Center,  //point,line,ply
        RightCenter,
        LeftCenter,
        UpCenter,
        BottomCenter,
        LeftUpCorner,
        RightUpCorner,
        RightDownCorner,
        LeftDownCorner,
        Online,  //line
        Inside,   //ply
        None
    }

    [Serializable]
    public class LabelDef:IDisposable,IFormattable,IFieldNamesProvider,ILabelDef,IPersistable
    {
        protected string _fieldname = null;
        protected bool _enableLabeling = false;
        protected string _invalidValue = "";
        protected Font _labelFont = null;
        protected Color _foreColor = Color.Black;
        protected Color _maskColor = Color.Empty;
        [NonSerialized]
        protected Brush _labelBrush = Brushes.Black;
        [NonSerialized]
        protected Brush _maskBrush = null;
        protected masLabelPosition _masLabelRuler = masLabelPosition.Center;
        protected string[] _fields = null;
        protected ScaleRange _displayScaleRange = new ScaleRange(-1,1);
        protected enumLabelSource _labelSource = enumLabelSource.Label;
        protected IContainerSymbol _containerSymbol = null;
        protected bool _autoToNewline = false;
        protected int _charcountPerLine = 6;
        protected int _angle = 0;

        public LabelDef()
        { 
        }

        public LabelDef(string fieldname,string[] fields)
        {
            _fields = fields;
            _labelFont = new Font("", 9);
            _labelBrush = Brushes.Black;
            _fieldname = fieldname;
        }

        [DisplayName("标注类型")]
        public enumLabelSource LabelSource
        {
            get { return _labelSource; }
            set { _labelSource = value; }
        }

        [DisplayName("是否显示")]
        public bool EnableLabeling
        {
            get { return _enableLabeling; }
            set
            {
                _enableLabeling = value;
            }
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
            set { _displayScaleRange = value; }
        }

        public bool VisibleAtScale(int scale)
        { 
            if(_displayScaleRange.Enable)
                return !(scale > _displayScaleRange.DisplayScaleOfMin || scale < _displayScaleRange.DisplayScaleOfMax);
            return true;
        }

        [DisplayName("标注字段")]
        [EditorAttribute(typeof(UIFieldTypeEditor), typeof(UITypeEditor))]        
        public string Fieldname
        {
            get { return _fieldname; }
            set 
            {
                _fieldname = value;
            }
        }

        [DisplayName("标注位置")]
        public masLabelPosition MasLabelRuler
        {
            get { return _masLabelRuler; }
            set
            {
                _masLabelRuler = value;
            }
        }

        [DisplayName("标注字体")]
        public Font LabelFont
        {
            get { return _labelFont; }
            set
            {
                _labelFont = value;
            }
        }

        [DisplayName("标注颜色")]
        public Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (_foreColor == value)
                    return;
                _foreColor = value;
                _labelBrush = new SolidBrush(value);
            }
        }

        [DisplayName("边框颜色")]
        public Color MaskColor
        {
            get
            {
                return _maskColor;
            }
            set
            {
                if (_maskColor == value)
                    return;
                _maskColor = value;
                if (!IsEmptyColor(value))
                    _maskBrush = new SolidBrush(value);
                else
                    _maskBrush = null;
            }
        }

        [DisplayName("旋转角度")]
        public int Angle
        {
            get { return _angle; }
            set 
            {
                _angle = value;
                if (_angle < 0)
                    _angle = 0;
                else if (_angle > 360)
                    _angle = 360;
            }
        }

        bool IsEmptyColor(Color c)
        {
            return (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0) ||
                (c.A ==0 && c.R == 255 && c.G == 255 && c.B == 255);
        }

        /// <summary>
        /// 无效值
        /// </summary>
        [DisplayName("无效值"),Description("不需要标注的属性值")]
        public string InvalidValue
        {
            get { return _invalidValue; }
            set 
            {
                _invalidValue = value;
            }
        }

        [DisplayName("背景符号"), TypeConverter(typeof(ExpandableObjectConverter))]
        [EditorAttribute(typeof(UIContainerSymbolEditor), typeof(UITypeEditor))]        
        public IContainerSymbol ContainerSymbol
        {
            get { return _containerSymbol; }
            set 
            {
                if(value != null)
                    _containerSymbol = value;
            }
        }

        [DisplayName("自动换行")]
        public bool AutoToNewline
        {
            get { return _autoToNewline; }
            set { _autoToNewline = value; }
        }

        [DisplayName("每行字符数")]
        public int CharcountPerLine
        {
            get { return _charcountPerLine; }
            set 
            {
                if (value > 0)
                {
                    _charcountPerLine = value;
                }
            }
        }
 
        [Browsable(false)]
        public Brush LabelBrush
        {
            get 
            {
                if(_labelBrush == null)
                    _labelBrush = new SolidBrush(_foreColor);
                return _labelBrush; 
            }
        }

        [Browsable(false)]
        public Brush MaskBrush
        {
            get
            {
                //if (_maskBrush == null)
                //{
                //    if (!IsEmptyColor(_maskColor))
                //        _maskBrush = new SolidBrush(_maskColor);
                //    else
                //        _maskBrush = null;
                //}
                return _maskBrush; 
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_labelBrush != null)
                _labelBrush.Dispose();
        }

        #endregion

        public override string ToString()
        {
            return ToString(null,null);
        }

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format==null)
                return String.Format("({0},{1})",_fieldname,EnableLabeling.ToString() );
            return String.Format("{0}", format);
        }

        #endregion

        #region IFieldNamesProvider 成员

        [Browsable(false)]
        public string[] Fields
        {
            get { return _fields; }
        }

        #endregion

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("LabelDef");
            obj.AddAttribute("source", _labelSource.ToString());
            obj.AddAttribute("enabled", _enableLabeling.ToString());
            obj.AddAttribute("field", _fieldname != null ? _fieldname : string.Empty);
            obj.AddAttribute("fields", _fields != null ? string.Join(",", _fields) : string.Empty);
            obj.AddAttribute("ruler", _masLabelRuler.ToString());
            obj.AddAttribute("invalidvalue", _invalidValue != null ? _invalidValue.ToString() : string.Empty);
            obj.AddAttribute("autotonewline", _autoToNewline.ToString());
            obj.AddAttribute("charcountperline", _charcountPerLine.ToString());
            //Font
            PersistObject fontObj = new PersistObject("Font");
            fontObj.AddAttribute("font", FontHelper.FontToString(_labelFont));
            fontObj.AddAttribute("color", ColorHelper.ColorToString(_foreColor));
            fontObj.AddAttribute("maskcolor", ColorHelper.ColorToString(_maskColor));
            obj.AddSubNode(fontObj);
            //_scaleRange
            if (_displayScaleRange == null)
                _displayScaleRange = new ScaleRange(-1, 1);
            PersistObject rangeObj = (_displayScaleRange as IPersistable).ToPersistObject();
            obj.AddSubNode(rangeObj);
            //_containerSymbol
            if (_containerSymbol != null)
            {
                PersistObject csymobj = _containerSymbol.ToPersistObject();
                obj.AddSubNode(csymobj);
            }
            return obj;
        }

        #endregion

      //<LabelDef enabled="False" field="ADMIN_NAME" fields="FIPS_ADMIN,GMI_ADMIN,ADMIN_NAME,FIPS_CNTRY,GMI_CNTRY,CNTRY_NAME,POP_ADMIN,TYPE_ENG,TYPE_LOC,SQKM,SQMI,COLOR_MAP" ruler="Center" invalidvalue="">
      //  <Font font="Microsoft Sans Serif,9,Regular" color="255,0,0,0" maskcolor="0,0,0,0" />
      //  <DisplayScaleRange enabled="False" minscale="-1" maxscale="1" />
      //</LabelDef>
        public static LabelDef FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            enumLabelSource source = enumLabelSource.Label;
            if (ele.Attribute("source") != null)
            {
                string sourceString = ele.Attribute("source").Value;
                foreach (enumLabelSource s in Enum.GetValues(typeof(enumLabelSource)))
                {
                    if (s.ToString() == sourceString)
                    {
                        source = s;
                        break;
                    }
                }
            }
            Font font = FontHelper.StringToFont(ele.Element("Font").Attribute("font").Value);
            ScaleRange range = ScaleRange.FromXElement(ele.Element("DisplayScaleRange"));
            Color color = ColorHelper.StringToColor(ele.Element("Font").Attribute("color").Value);
            Color maskcolor = ColorHelper.StringToColor(ele.Element("Font").Attribute("maskcolor").Value);
            string field = ele.Attribute("field").Value;
            string fields = ele.Attribute("fields").Value; 
            bool enabled = bool.Parse(ele.Attribute("enabled").Value);
            bool autotonewline = false;
            if(ele.Attribute("autotonewline")!=null)
                autotonewline = bool.Parse(ele.Attribute("autotonewline").Value);
            int charcount = 6;
            if (ele.Attribute("charcountperline") != null)
                charcount = int.Parse(ele.Attribute("charcountperline").Value);
            masLabelPosition ruler = masLabelPosition.Center;
            string rulerstring = ele.Attribute("ruler").Value;
            foreach (masLabelPosition r in Enum.GetValues(typeof(masLabelPosition)))
            {
                if (r.ToString() == rulerstring)
                {
                    ruler = r;
                    break;
                }
            }
            string invalidvalue = ele.Attribute("invalidvalue").Value;
            //
            IContainerSymbol csym = null;
            if (ele.Element("ContainerSymbol") != null)
                csym = PersistObject.ReflectObjFromXElement(ele.Element("ContainerSymbol")) as IContainerSymbol;
            //
            LabelDef def = new LabelDef(field, fields.Trim() != string.Empty ? fields.Split(',') : null);
            def.LabelFont = font;
            def.DisplayScaleRange = range;
            def.EnableLabeling = enabled;
            def.ForeColor = color;
            def.MaskColor = maskcolor;
            def.MasLabelRuler = ruler;
            def.InvalidValue = invalidvalue;
            def.LabelSource = source;
            def.ContainerSymbol = csym;
            def.AutoToNewline = autotonewline;
            def.CharcountPerLine = charcount;
            return def;
        }
    }
}
