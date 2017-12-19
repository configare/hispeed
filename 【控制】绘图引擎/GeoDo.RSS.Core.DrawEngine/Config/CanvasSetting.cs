using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CanvasSetting
    {
        private string _name;
        private CacheSetting _cacheSetting;
        private RenderSetting _renderSetting;
        private ZoomSetting _zoomSetting;
        private TileSetting _tileSetting;
        private float _xexpand = 0.5f;
        private float _yexpand = 0.5f;

        public CanvasSetting(string name,
            CacheSetting cacheSetting,
            TileSetting tileSetting,
            RenderSetting renderSetting,
            ZoomSetting zoomSetting,
            float xexpand,
            float yexpand)
        {
            _name = name;
            _cacheSetting = cacheSetting;
            _tileSetting = tileSetting;
            _renderSetting = renderSetting;
            _zoomSetting = zoomSetting;
            _xexpand = xexpand;
            _yexpand = yexpand;
        }

        public string Name
        {
            get { return _name; }
        }

        public float XExpand
        {
            get { return _xexpand; }
        }

        public float YExpand
        {
            get { return _yexpand; }
        }

        public CacheSetting CacheSetting
        {
            get { return _cacheSetting; }
        }

        public TileSetting TileSetting
        {
            get { return _tileSetting; }
        }

        public RenderSetting RenderSetting
        {
            get { return _renderSetting; }
        }

        public ZoomSetting ZoomSetting
        {
            get { return _zoomSetting; }
        }
    }
}
