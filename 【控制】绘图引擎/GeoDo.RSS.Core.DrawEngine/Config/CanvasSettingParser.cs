using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CanvasSettingParser : ICanvasSettingParser
    {
        private CacheSetting _cacheSetting;
        private CanvasSetting _canvasSetting;
        private ZoomSetting _zoomSetting;
        private RenderSetting _renderSetting;
        private ZoomStepByScale _zoomStepByScale;
        private List<ZoomStepByScale> _items;
        private XElement _xElemRoot;
        private XElement _subElem;

        public CanvasSettingParser()
        { }

        public CanvasSetting Parse(string fname)
        {
            _xElemRoot = XElement.Load(fname, LoadOptions.None);
            string name = (_xElemRoot.Attribute("name") != null) ? _xElemRoot.Attribute("name").Value : string.Empty;
            float xexpand ,yexpand;
            GetExpandValues(out xexpand,out yexpand);
            _canvasSetting = new CanvasSetting(name,GetCacheSetting(),GetTileSetting(), GetRenderSetting(),GetZoomSetting(),xexpand,yexpand);
            return _canvasSetting;
        }

        private void GetExpandValues(out float xexpand,out float yexpand)
        {
            xexpand = 0.5f;
            yexpand = 0.5f;
            // <CurrentViewExtandPercent xexpand="0.5" yexpand="0.5"/>
            _subElem = _xElemRoot.Element("CurrentViewExtandPercent");
             if (_subElem != null)
             {
                 xexpand = float.Parse(_subElem.Attribute("xexpand").Value);
                 yexpand = float.Parse(_subElem.Attribute("yexpand").Value);
             }
        }

        private CacheSetting GetCacheSetting()
        {
            _subElem = _xElemRoot.Element("CacheSetting");
            if (_subElem != null)
            {
                string dir = _subElem.Attribute("dir").Value;
                int maxsizemb = int.Parse(_subElem.Attribute("maxsizemb").Value);
                int minsizemb = int.Parse(_subElem.Attribute("minsizemb").Value);
                return new CacheSetting(dir, maxsizemb, minsizemb);
            }
            return new CacheSetting();
        }

        private TileSetting GetTileSetting()
        {
            _subElem = _xElemRoot.Element("TileSetting");
            if (_subElem != null)
            {
                int tileSize = int.Parse(_subElem.Attribute("tilesize").Value);
                int sampleratio = int.Parse(_subElem.Attribute("sampleratio").Value);
                return new TileSetting(tileSize, sampleratio);
            }
            return new TileSetting();
        }

        private RenderSetting GetRenderSetting()
        {
            _renderSetting = new RenderSetting();
            _subElem = _xElemRoot.Element("RenderSetting");
            if (_subElem != null)
            {
                _renderSetting.BackColor = GetColor(_subElem.Attribute("backcolor"));
                _renderSetting.EnabledParallel = (_subElem.Attribute("enabledparallel") != null) ? Convert.ToBoolean(_subElem.Attribute("enabledparallel").Value) : false;
                _renderSetting.EnabledDummymode = (_subElem.Attribute("enableddummymode") != null) ? Convert.ToBoolean(_subElem.Attribute("enableddummymode").Value) : false;
            }
            return _renderSetting;
        }

        private Color GetColor(XAttribute att)
        {
            if(att == null)
                return Color.White;
            string rgb = att.Value;
            if (string.IsNullOrEmpty(rgb))
                return Color.White;
            string[] parts = rgb.Split(',');
            if (parts.Length == 3)
                return Color.FromArgb(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
            else if (parts.Length == 4)
                return Color.FromArgb(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]), byte.Parse(parts[3]));
            else
                return Color.White; 
        }

        private ZoomSetting GetZoomSetting()
        {
            _zoomSetting = new ZoomSetting(GetZoomStepByScales());
            _subElem = _xElemRoot.Element("ZoomSetting");
            _zoomSetting.Zoomfactor = JudgeDoubleIsNull(_subElem.Attribute("zoomfactor"));
            _zoomSetting.Zoomminpercent = JudgeDoubleIsNull(_subElem.Attribute("zoomminpercent"));
            _zoomSetting.Zoommaxpercent = JudgeDoubleIsNull(_subElem.Attribute("zoommaxpercent"));
            return _zoomSetting;
        }

        private List<ZoomStepByScale> GetZoomStepByScales()
        {
            _items = new List<ZoomStepByScale>();
            foreach (XElement item in _xElemRoot.Descendants("ZoomStepByScale"))
            {
                _zoomStepByScale = new ZoomStepByScale();
                _zoomStepByScale.Minscale = JudgeDoubleIsNull(item.Attribute("minscale"));
                _zoomStepByScale.Maxscale =JudgeDoubleIsNull(item.Attribute("maxscale"));
                _zoomStepByScale.Steps = (item.Attribute("steps")!=null)?Convert.ToInt32(item.Attribute("steps").Value):0;
                _items.Add(_zoomStepByScale);
            }
            return _items;
        }

        private double JudgeDoubleIsNull(XAttribute xAtt)
        {
            return (xAtt != null) ? Convert.ToDouble(xAtt.Value) : 0;
        }

        public void Dispose()
        {
            _canvasSetting = null;
            _zoomSetting = null;
            _renderSetting = null;
            _zoomStepByScale = null;
            _items = null;
            _xElemRoot = null;
            _subElem = null;
        }
    }
}
