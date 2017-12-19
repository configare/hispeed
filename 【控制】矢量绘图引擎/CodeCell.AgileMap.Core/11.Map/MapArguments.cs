using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Linq;

using System.Drawing.Drawing2D;
using CodeCell.Bricks.Serial;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using CodeCell.Bricks.UIs;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    internal class UISpatialRefFileEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "ESRI Prj File(*.prj)|*.prj";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ISpatialReference sf = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(dlg.FileName);
                    if (sf == null)
                    {
                        MsgBox.ShowInfo("文件\"" + dlg.FileName + "\"不是标准的OGC WKT空间参考描述格式。");
                        return value;
                    }
                    if (sf.ProjectionCoordSystem == null)
                    {
                        MsgBox.ShowInfo("您只能选择投影坐标系统。");
                        return value;
                    }
                    MsgBox.ShowInfo("空间参考设置成功。\n要想生效请先保存地图配置文件(*.mcd),然后重新打开。");
                    return sf;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }

    public class MapArguments : IPersistable
    {
        private Envelope _fullExtent = null;
        private Envelope _extent = null;
        private Color _backColor = Color.Transparent;
        private SmoothingMode _smoothingMode = SmoothingMode.Default;
        private ISpatialReference _targetSpatialReference = null;

        public MapArguments()
        {
        }

        [Browsable(false)]
        public Envelope Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        [Browsable(false)]
        public Envelope FullExtent
        {
            get { return _fullExtent; }
            set { _fullExtent = value; }
        }

        [DisplayName("背景颜色")]
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        [DisplayName("平滑模式")]
        public SmoothingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value; }
        }

        [DisplayName("显示空间参考"), Description("显示过程中将图层统一转换为该空间参考"), TypeConverter(typeof(ExpandableObjectConverter))]
        [EditorAttribute(typeof(UISpatialRefFileEditor), typeof(UITypeEditor))]
        public ISpatialReference TargetSpatialReference
        {
            get { return _targetSpatialReference; }
            set
            {
                _targetSpatialReference = value;
            }
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("MapArguments");
            if (_fullExtent != null)
                obj.AddSubNode(EnvelopeToPersisitObject(_fullExtent, "FullExtent"));
            if (_extent != null)
                obj.AddSubNode(EnvelopeToPersisitObject(_extent, "Extent"));
            obj.AddAttribute("backcolor", ColorHelper.ColorToString(_backColor));
            obj.AddAttribute("smoothingmode", _smoothingMode.ToString());
            obj.AddAttribute("spatialreference", _targetSpatialReference != null ? _targetSpatialReference.ToWKTString() : string.Empty);
            return obj;
        }

        public static MapArguments FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            MapArguments arg = new MapArguments();
            arg.BackColor = ColorHelper.StringToColor(ele.Attribute("backcolor").Value);
            if (ele.Element("FullExtent") != null)
                arg.FullExtent = XElementToEnvelope(ele.Element("FullExtent"));
            if (ele.Element("Extent") != null)
                arg.Extent = XElementToEnvelope(ele.Element("Extent"));
            if (ele.Attribute("smoothingmode") != null)
            {
                string sm = ele.Attribute("smoothingmode").Value;
                SmoothingMode smode = SmoothingMode.Default;
                foreach (SmoothingMode s in Enum.GetValues(typeof(SmoothingMode)))
                {
                    if (s.ToString() == sm)
                    {
                        smode = s;
                        break;
                    }
                }
                arg.SmoothingMode = smode;
            }
            if (ele.Attribute("spatialreference") != null)
            {
                try
                {
                    string sf = ele.Attribute("spatialreference").Value;
                    arg._targetSpatialReference = SpatialReferenceFactory.GetSpatialReferenceByWKT(sf, enumWKTSource.EsriPrjFile);
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                }
            }
            return arg;
        }

        #endregion

        private static Envelope XElementToEnvelope(XElement ele)
        {
            if (ele == null)
                return null;
            Envelope evp = new Envelope();
            evp.MinX = double.Parse(ele.Attribute("MinX").Value);
            evp.MinY = double.Parse(ele.Attribute("MinY").Value);
            evp.MaxX = double.Parse(ele.Attribute("MaxX").Value);
            evp.MaxY = double.Parse(ele.Attribute("MaxY").Value);
            return evp;
        }

        private PersistObject EnvelopeToPersisitObject(Envelope evp, string name)
        {
            PersistObject obj = new PersistObject(name);
            obj.AddAttribute("MinX", evp.MinX.ToString());
            obj.AddAttribute("MinY", evp.MinY.ToString());
            obj.AddAttribute("MaxX", evp.MaxX.ToString());
            obj.AddAttribute("MaxY", evp.MaxY.ToString());
            return obj;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
